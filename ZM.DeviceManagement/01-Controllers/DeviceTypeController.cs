using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Device.Entities;
using RuoYi.Framework;
using SqlSugar;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    /// <summary>
    /// 设备类别
    /// </summary>
    [ApiDescriptionSettings("zm/devicetype")]
    [Route("zm/devicetype")]
    [AllowAnonymous]
    public class DeviceTypeController : ControllerBase
    {
        private readonly ILogger<DeviceTypeController> _logger;
        private readonly DeviceTypeService _deviceTypeService;

        public DeviceTypeController(ILogger<DeviceTypeController> logger,
            DeviceTypeService deviceTypeService)
        {
            _logger = logger;
            _deviceTypeService = deviceTypeService;
        }


        /////////////////////////////////////////////列表树/////////////////////////////////////////////////


        /// <summary>
        /// 根据ID获取详细信息
        /// </summary>
        [HttpGet("info/{deptId}")]
        public async Task<AjaxResult> Get(long deptId)
        {
            //await _deviceTypeService.CheckDeptDataScopeAsync(deptId); //部门权限检查
            var data = await _deviceTypeService.GetDtoAsync(deptId);
            return AjaxResult.Success(data);
        }




        /// <summary>
        /// 查询列表
        /// </summary>
        [HttpGet("list")]
        public async Task<AjaxResult> GetSysDictTypeList([FromQuery] DeviceTypeDto dto)
        {
            List<DeviceTypeDto> data = await _deviceTypeService.GetDtoListAsync(dto);
            return AjaxResult.Success(data);
        }



        /// <summary>
        /// 查询分页列表
        /// </summary>
        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<DeviceTypeDto>> GetSysDictTypePageList([FromQuery] DeviceTypeDto dto)
        {
             return await _deviceTypeService.GetDtoPagedListAsync(dto);
        }



        /// <summary>
        /// 新增 租户表
        /// </summary>
        [HttpPost("add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Add([FromBody] DeviceTypeDto tenant)
        {
            // 查询tid
            //if(tenant.TenantId <= 0)
            //{
            //    long tid = SecurityUtils.GetTenantId();
            //    tenant.TenantId = tid;
            //}


            if(!await _deviceTypeService.CheckDeptNameUniqueAsync(tenant))
            {
                return AjaxResult.Error($"新增租户'{tenant.DeptName} '失败，租户名称已存在");
            }
            var data = await _deviceTypeService.InsertDeptAsync(tenant);
            return AjaxResult.Success(data);
        }




        /// <summary>
        /// 删除 租户表
        /// </summary>
         [HttpGet("delete/{deptId}")]
         public async Task<AjaxResult> Remove(long deptId)
        {
            if(await _deviceTypeService.HasChildByDeptIdAsync(deptId))
            {
                return AjaxResult.Error("存在下级租户,不允许删除");
            }
            if(await _deviceTypeService.CheckDeptExistUserAsync(deptId))
            {
                return AjaxResult.Error("租户存在用户,不允许删除");
            }
            // 检查部门是否有部门权限
            //await _deviceTypeService.CheckDeptDataScopeAsync(deptId);
            var data = await _deviceTypeService.DeleteDeptByIdAsync(deptId);
            return AjaxResult.Success(data);
 
        }


        /// <summary>
        /// 修改  
        /// </summary>
        [HttpPost("update")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Edit([FromBody] DeviceTypeDto dt)
        {
            long deptId = dt.Id;
            //await _deviceTypeService.CheckDeptDataScopeAsync(deptId);// 权限验证
            if(!await _deviceTypeService.CheckDeptNameUniqueAsync(dt))
            {
                return AjaxResult.Error("修改租户'" + dt.DeptName + "'失败，租户名称已存在"); // 已验证
            }
            else if(dt.ParentId.Equals(deptId))
            {
                return AjaxResult.Error("修改租户'" + dt.DeptName + "'失败，上级租户不能是自己"); // 已验证
            }
            // 备用：如果要停用的话，需要确保子节点都设置了停用。
            //else if(UserConstants.DEPT_DISABLE.Equals(dt.Status) && await _deviceTypeService.CountNormalChildrenDeptByIdAsync(deptId) > 0)
            //{
            //    return AjaxResult.Error("该租户包含未停用的子租户！");
            //}
 
            var data = await _deviceTypeService.UpdateDeptAsync(dt);
            return AjaxResult.Success(data);
         }


        /// <summary>
        /// 查询租户表列表    主要是处理Ancestors 字段包含逻辑
        /// </summary>
        [HttpGet("list/exclude/{deptId}")]
        public async Task<AjaxResult> ExcludeChildList(long? deptId)
        {
            var list = await _deviceTypeService.GetDtoListAsync(new DeviceTypeDto());
            var id = deptId ?? 0;
            var data = list.Where(d => d.Id != id || (!d.Ancestors?.Split(",").Contains(id.ToString()) ?? false)).ToList();
            return AjaxResult.Success(data);
        }




        /////////////////////////////////////////////EL树/////////////////////////////////////////////////


        /// <summary>
        /// 查询EL树结构
        /// </summary>
        [HttpGet("getTreeNode")]
        public async Task<AjaxResult> getTreeNode([FromQuery] DeviceTypeDto dto)
        {

            var data = await _deviceTypeService.GetTreeNodeAsync(dto);
            return AjaxResult.Success(data);
        }





    }


}
