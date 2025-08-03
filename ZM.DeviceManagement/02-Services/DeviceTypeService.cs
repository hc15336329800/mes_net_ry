using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Constants;
using RuoYi.Common.Data;
using RuoYi.Data;
using RuoYi.Data.Entities;
using RuoYi.Data.Models;
using RuoYi.Device.Entities;
using RuoYi.Framework.DependencyInjection;
using RuoYi.Framework.Exceptions;
using RuoYi.Framework.Extensions;
using ZM.Device.Repositories;


namespace ZM.Device.Services
{

    /// <summary>
    /// 设备分类
    /// </summary>
    public class DeviceTypeService : BaseService<DeviceType,DeviceTypeDto>, ITransient
    {
        private readonly ILogger<DeviceTypeService> _logger;
        private readonly DeviceTypeRepository _DeviceTypeRepository;

        public DeviceTypeService(ILogger<DeviceTypeService> logger,
            DeviceTypeRepository deviceTypeRepository)
        {
            _logger = logger;
            _DeviceTypeRepository = deviceTypeRepository;
            BaseRepo = deviceTypeRepository; //注意
        }




        ////////////////////////////////////////树形表格////////////////////////////////////////////////////



        /// <summary>
        /// 新增 
        /// </summary>
        public async Task<bool> InsertDeptAsync(DeviceTypeDto dept)
        {
            DeviceType info = await _DeviceTypeRepository.FirstOrDefaultAsync(d => d.Id == dept.ParentId); // 父节点
                                                                                                         // 如果父节点不为正常状态,则不允许新增子节点
            if(!UserConstants.DEPT_NORMAL.Equals(info.Status))
            {
                throw new ServiceException("部门停用，不允许新增");
            }
            dept.Ancestors = info.Ancestors + "," + dept.ParentId;
            dept.DelFlag = DelFlag.No;
            return await _DeviceTypeRepository.InsertAsync(dept);
        }


        /// <summary>
        /// 删除 
        /// </summary>
        public async Task<int> DeleteDeptByIdAsync(long deptId)
        {
            return await _DeviceTypeRepository.DeleteDeptByIdAsync(deptId);
        }


        /// <summary>
        /// 修改 
        /// </summary>
        public async Task<int> UpdateDeptAsync(DeviceTypeDto dept)
        {
            DeviceType newParentDept = await this.GetAsync(dept.ParentId);
            DeviceType oldDept = await this.GetAsync(dept.Id);
            if(newParentDept != null && oldDept != null)
            {
                string newAncestors = newParentDept.Ancestors + "," + newParentDept.Id;
                string oldAncestors = oldDept.Ancestors!;
                dept.Ancestors = newAncestors;
                await UpdateDeptChildrenAsync(dept.Id,newAncestors,oldAncestors);//修改子元素
            }
            int result = await _DeviceTypeRepository.UpdateAsync(dept,true);

            //if(UserConstants.DEPT_NORMAL.Equals(dept.Status) && StringUtils.IsNotEmpty(dept.Ancestors)
            //        && !StringUtils.Equals("0",dept.Ancestors))
            //{
            //    // 如果该部门是启用状态，则启用该部门的所有上级部门
            //    await UpdateParentDeptStatusNormalAsync(dept);
            //}
            return result;
        }


        /// <summary>
        /// 根据id查询详情
        /// </summary>
        public async Task<DeviceType> GetAsync(long id)
        {
            return await base.FirstOrDefaultAsync(e => e.Id == id);
        }
     
        
        /// <summary>
        /// 根据id查询详情 DTO
        /// </summary>
        public async Task<DeviceTypeDto> GetDtoAsync(long id)
        {
            var entity = await GetAsync(id);
            return entity.Adapt<DeviceTypeDto>();
        }

        ////////////////////////////////////////校验工具



        /// <summary>
        /// 修改子元素关系
        /// </summary>
        /// <param name="deptId">被修改的部门ID</param>
        /// <param name="newAncestors">新的父ID集合</param>
        /// <param name="oldAncestors">旧的父ID集合</param>
        public async Task UpdateDeptChildrenAsync(long deptId,string newAncestors,string oldAncestors)
        {
            List<DeviceType> children = await _DeviceTypeRepository.GetChildrenDeptByIdAsync(deptId);
            foreach(DeviceType child in children)
            {
                child.Ancestors = child.Ancestors!.ReplaceFirst(oldAncestors,newAncestors);
            }
            if(children.Count > 0)
            {
                await _DeviceTypeRepository.UpdateAsync(children);
            }
        }
 
        /// <summary>
        /// 校验部门名称是否唯一
        /// </summary>
        public async Task<bool> CheckDeptNameUniqueAsync(DeviceTypeDto dept)
        {
            DeviceType info = await _DeviceTypeRepository.GetFirstAsync(new DeviceTypeDto { DeptName = dept.DeptName,ParentId = dept.ParentId });
            if(info != null && info.Id != dept.Id)
            {
                return UserConstants.NOT_UNIQUE;
            }
            return UserConstants.UNIQUE;
        }

        /// <summary>
        /// 当前节点是否存在子节点
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public async Task<bool> HasChildByDeptIdAsync(long deptId)
        {
            return await _DeviceTypeRepository.HasChildByDeptIdAsync(deptId);
        }

        /// <summary>
        /// 查询分类是否存在设备
        /// </summary>
        /// <param name="deptId">分类ID</param>
        /// <returns></returns>
        public async Task<bool> CheckDeptExistUserAsync(long deptId)
        {
            return await _DeviceTypeRepository.CheckDeviceTypeExistInManagementTypeAsync(deptId);
        }




        ////////////////////////////////////////EL树组件数据格式////////////////////////////////////////////////////

        #region EL树组件

        /// <summary>
        /// 查询树结构信息
        /// </summary>
        public async Task<List<TreeEl<DeviceTypeDto>>> GetTreeNodeAsync(DeviceTypeDto dto)
        {
            var depts = await _DeviceTypeRepository.GetDtoListAsync(dto);
            return BuildTreeNode(depts);
        }

        /// <summary>
        /// 构建前端所需要下拉树结构
        /// </summary>
        private List<TreeEl<DeviceTypeDto>> BuildTreeNode(List<DeviceTypeDto> depts)
        {
            // 先构建树形结构（包含子节点）
            var deptTrees = BuildTree(depts);
            // 使用 TreeEl 构造函数，通过委托指定如何获取 Id、Label 和 Children
            return deptTrees.Select(dept => new TreeEl<DeviceTypeDto>(
                dept,
                d => d.Id,
                d => d.DeptName ?? string.Empty,
                d => d.Children ?? new List<DeviceTypeDto>()
            )).ToList();
        }

        /// <summary>
        /// 构建树形结构：将顶级节点和其子节点构成嵌套列表
        /// </summary>
        private List<DeviceTypeDto> BuildTree(List<DeviceTypeDto> depts)
        {
            // 假设顶级节点 ParentId 为 0
            var topNodes = depts.Where(d => d.ParentId == 0).ToList();
            foreach(var top in topNodes)
            {
                SetTreeChildrenNode(depts,top);
            }
            // 如果没有找到顶级节点，则返回全部（防止出错）
            return topNodes.Any() ? topNodes : depts;
        }

        /// <summary>
        /// 递归设置每个节点的子节点
        /// </summary>
        private void SetTreeChildrenNode(List<DeviceTypeDto> allNodes,DeviceTypeDto parent)
        {
            // 找到当前节点的直接子节点
            var children = allNodes.Where(n => n.ParentId == parent.Id).ToList();
            parent.Children = children;
            foreach(var child in children)
            {
                SetTreeChildrenNode(allNodes,child);
            }
        }

        #endregion


    }
}
