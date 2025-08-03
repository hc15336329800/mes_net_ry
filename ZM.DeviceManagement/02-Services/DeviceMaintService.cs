using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using RuoYi.Framework.DependencyInjection;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Interceptors;
using ZM.Device.Tool;
using RuoYi.Framework;
using Microsoft.VisualBasic;



namespace ZM.Device.Services
{    
    /// <summary>
    ///  保养记录表 Service
    ///  author ruoyi.net
    ///  date   2025-04-01 16:06:17
    /// </summary>
    public class DeviceMaintService : BaseService<DeviceMaint, DeviceMaintDto>, ITransient
    {
        private readonly ILogger<DeviceMaintService> _logger;
        private readonly DeviceMaintRepository _deviceMaintRepository;
        private readonly DeviceManagementMaintRepository _deviceManagementMaintRepository;

        private readonly DeviceManagementService _deviceManagementService; // 
        private readonly DeviceManagementRepository _deviceManagementRepository;




        public DeviceMaintService(ILogger<DeviceMaintService> logger,
            DeviceManagementService deviceManagementService,
             DeviceManagementRepository deviceManagementRepository,
            DeviceMaintRepository deviceMaintRepository,
            DeviceManagementMaintRepository deviceManagementMaintRepository)
        {
            BaseRepo = deviceMaintRepository;
            _deviceManagementService = deviceManagementService; //设备
            _deviceManagementRepository = deviceManagementRepository;

            _logger = logger;
            _deviceMaintRepository = deviceMaintRepository;
            _deviceManagementMaintRepository = deviceManagementMaintRepository; 
        }

        /////////////////////////////////////新增的额业务/////////////////////////////////////





        //todo:新增一个更新方法  只更新 LastMaintenanceTime 根据设备id （使用纯sql）
        public async Task<bool> UpdateLastMaintTimeRawAsync(long DeviceId,DateTime lastMaintenanceTime)
        {
            var rows = await _deviceManagementService.BaseRepo.Repo.Context.Ado.ExecuteCommandAsync(
                 "UPDATE device_management SET last_maintenance_time = @lastMaintenanceTime WHERE id = @id",
                 new List<SugarParameter>
                 {
                    new SugarParameter("@id", DeviceId),
                    new SugarParameter("@lastMaintenanceTime", lastMaintenanceTime)
                 }
             );


            return rows > 0;
        }




        /// <summary>
        /// 返回保养列表  <-- 中间表 <-- 设备ID
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceMaintDto>> GetDeviceMaintPagedListById(long deviceId)
        {
            // 第一步：查中间表，获取所有 maint_id
            var maintIds = await _deviceManagementMaintRepository.Repo.AsQueryable()
                .Where(x => x.DeviceId == deviceId)
                .Select(x => x.MaintId)
                .ToListAsync();

            // 无保养记录直接返回空分页
            if(maintIds.IsNullOrEmpty())
            {
                return new SqlSugarPagedList<DeviceMaintDto>
                {
                    PageIndex = 1,
                    PageSize = 10,
                    Total = 0,
                    Rows = new List<DeviceMaintDto>(),
                    Code = 200,
                    HasPrevPages = false,
                    HasNextPages = false
                };
            }

            // 第二步：根据 maintIds 查询保养表（device_maint），分页查询
            var query = _deviceMaintRepository.Repo.AsQueryable()
                .Where(x => maintIds.Contains(x.Id))
                .Select(x => new DeviceMaintDto
                {
                    Id = x.Id,
                     Content = x.Content,
                    Remark = x.Remark,
                    //UpdateTime = x.UpdateTime,
                    CreateTime = x.CreateTime,
                    ImageUrl   = x.ImageUrl,

                });

            // 使用通用分页扩展 ToPagedListAsync
            var pagedResult = await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);
            pagedResult.Code = 200;

            return pagedResult;
        }


        /// <summary>
        /// 新增保养记录（并绑定设备）
        /// </summary>
        [Transactional] // 启用事务
        public virtual async Task<bool> AddWithRelationAsync(DeviceMaintDto dto)
        {
            if(dto.DeviceId <= 0)
                throw new ArgumentException("设备ID无效");

            var entity = dto.Adapt<DeviceMaint>();
            entity.Id = NextId.Id13();

            bool inserted = await _deviceMaintRepository.InsertAsync(entity); //插入保养信息
            if(!inserted) return false;

            var middle = new DeviceManagementMaint
            {
                DeviceId = dto.DeviceId,
                MaintId = entity.Id,
                TenantId = 0
            };
            bool relationInserted = await _deviceManagementMaintRepository.InsertAsync(middle); //插入中间表信息


            //todo:修改设备最后维保时间字段
            //DeviceManagementDto dt = new DeviceManagementDto();
            //dt.Id = dto.DeviceId;
            //dt.LastMaintenanceTime=  DateTime.Now;
            //int success = await _deviceManagementService.UpdateAsync(dt);


            bool updated = await UpdateLastMaintTimeRawAsync(dto.DeviceId,DateTime.Now);
            if(!updated)
            {
                _logger.LogWarning($"未能更新设备 {dto.DeviceId} 的最后保养时间");
            }
 
            return updated;
        }

        /// <summary>
        /// 更新保养记录（仅允许当天创建的记录可修改）
        /// </summary>
        public virtual async Task<AjaxResult> UpdateWithDateLimitAsync(DeviceMaintDto dto)
        {

            // ✅ 使用 code_id 查询实体, 查询创建时间。（使用 BaseRepo 提供的统一查询方式）
            var entity = await FirstOrDefaultAsync(x => x.Id == dto.Id); if(entity == null)
            {
                return AjaxResult.Error("保养记录不存在！");
            }
 
            if(!entity.CreateTime.HasValue || entity.CreateTime.Value.Date != DateTime.Today)
            {
                return AjaxResult.Error("只能修改当天创建的数据！");
            }
 
            if(dto.Id <= 0) //前端可能传过来ID
            {
                dto.Id = entity.Id; // 仅在未设置主键时赋值，防止覆盖已有 Id
            }
 
            var updateCount = await UpdateAsync(dto);
            return updateCount > 0 ? AjaxResult.Success("修改成功") : AjaxResult.Error("修改失败");
        }


        /// <summary>
        /// 删除保养记录（含中间表）
        /// </summary
        [Transactional]
        public virtual async Task<bool> DeleteWithRelationAsync(long id)
        {
            if(id <= 0)
                throw new ArgumentException("主键 ID 无效");

            // 1. 查询保养记录，获取 codeId
            var entity = await FirstOrDefaultAsync(x => x.Id == id);
            if(entity == null)
                throw new Exception("保养记录不存在");

            // 2. 删除中间表
            await _deviceManagementMaintRepository.Repo.DeleteAsync(x => x.MaintId == entity.Id);

            // 3. 删除主表
            var deleted = await DeleteAsync(id);
            return deleted > 0;
        }





        /////////////////////////////////////生成器自动生成的代码/////////////////////////////////////



        /// <summary>
        /// 查询 保养记录表 详情
        /// </summary>
        public async Task<DeviceMaint> GetAsync(int id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            return entity;
        }

        /// <summary>
        /// 查询 保养记录表 详情
        /// </summary>
        public async Task<DeviceMaintDto> GetDtoAsync(int id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            var dto = entity.Adapt<DeviceMaintDto>();
            // TODO 填充关联表数据
            return dto;
        }
    }
}