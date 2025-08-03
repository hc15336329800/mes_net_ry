using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Data.Models;
using RuoYi.Data.Slave.Dtos;
using RuoYi.Data.Slave.Entities;
using RuoYi.Device.Entities;
using RuoYi.Framework.DependencyInjection;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Interceptors;
using SqlSugar;
using ZM.Device.Tool;
using ZM.Device.Entities;
using ZM.Device.Repositories;

namespace ZM.Device.Services
{
    /// <summary>
    /// 设备表
    /// </summary>
    public class DeviceManagementService : BaseService<DeviceManagement,DeviceManagementDto>, ITransient
    {
        private readonly ILogger<DeviceManagementService> _logger;
        private readonly DeviceManagementRepository _deviceManagementRepository;
        private readonly DeviceManagementTypeRepository _deviceManagementTypeRepository;


        public DeviceManagementService(
             ILogger<DeviceManagementService> logger,
             DeviceManagementRepository deviceManagementRepository,
             DeviceManagementTypeRepository deviceManagementTypeRepository)
        {
            _logger = logger;
            _deviceManagementRepository = deviceManagementRepository;
            _deviceManagementTypeRepository = deviceManagementTypeRepository;
            BaseRepo = deviceManagementRepository; // 注意
        }


        /// <summary>
        /// 分页查询-按 maintenance_cycle 升序排序的逻辑
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleAsync(DeviceManagementDto dto)
        {
            return await _deviceManagementRepository.GetDtoPagedListOrderedByMaintenanceCycleAsync(dto);
        }


        /// <summary>
        /// 查询分页列表  ID倒序
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleIdAsync(DeviceManagementDto dto)
        {
            return await _deviceManagementRepository.GetDtoPagedListOrderedByMaintenanceCycleIDAsync(dto);
        }


        /// <summary>
        /// 查询设备分类下的所有设备 （自定义SQL-涉及中间表）
        /// </summary>
        /// <param name="deviceTypeId">分类ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagement>> GetPagedListByDeviceTypeAsync(long deviceTypeId,int pageIndex,int pageSize)
        {
            //  备用方法：
            //// 调用仓储层方法，获取设备管理实体分页数据
            //var pagedEntities = await _deviceManagementRepository.GetPagedListByDeviceTypeAsync(deviceTypeId,pageIndex,pageSize);
            //// 将实体集合转换为 DTO 集合（使用 Mapster 进行转换）
            //var dtoRows = pagedEntities.Rows.Adapt<List<DeviceManagementDto>>();
            //// 构造一个新的分页对象，将分页信息及转换后的 DTO 列表赋值
            //var pagedDto = new SqlSugarPagedList<DeviceManagementDto>()
            //{
            //    PageIndex = pagedEntities.PageIndex,
            //    PageSize = pagedEntities.PageSize,
            //    Total = pagedEntities.Total,
            //    Rows = dtoRows,
            //    Code = pagedEntities.Code,
            //    HasPrevPages = pagedEntities.HasPrevPages,
            //    HasNextPages = pagedEntities.HasNextPages
            //};
            //return pagedDto;

            // 调用仓储层方法，获取设备管理实体分页数据
            return  await _deviceManagementRepository.GetPagedListByDeviceTypeAsync(deviceTypeId,pageIndex,pageSize);


        }


 


        /// <summary>
        /// 根据设备ID集合查询设备信息
        /// </summary>
        public async Task<List<DeviceManagementDto>> GetListByDeviceIdsAsync(List<long> deviceIds)
        {
            if(deviceIds == null || !deviceIds.Any())
            {
                return new List<DeviceManagementDto>();
            }

            // 使用 BaseRepo 的查询方式（统一方式，避免裸调用）
            var query = BaseRepo.DtoQueryable(new DeviceManagementDto())
                            .Where(x => deviceIds.Contains(x.Id));

            return await query.ToListAsync();
        }





        /// <summary>
        /// 新增设备，同时新增中间表记录（devicetype_id 取自 dto.DeviceType）
        /// </summary>
        /// <param name="dto">包含设备信息和设备类型ID的DTO</param>
        /// <returns>操作是否成功</returns>
        [Transactional]
        public async Task<bool> AddDeviceWithTypeAsync(DeviceManagementDto dto)
        {

            dto.LastMaintenanceTime = DateTime.Now;//初始化维保时间  默认当前时间


            // 校验设备类型不能为空
            if(string.IsNullOrEmpty(dto.DeviceType))
            {
                throw new ArgumentException("设备类型不能为空");
            }

            // 将 DTO 转换为 DeviceManagement 实体
            var entity = dto.Adapt<DeviceManagement>();

            // 插入设备记录（DeviceManagement 表中，主键是自增的）

            entity.Id = NextId.Id13(); //id算法
            bool inserted = await _deviceManagementRepository.InsertAsync(entity);
            if(!inserted)
            {
                return false;
            }

            // 获取新插入设备的主键ID
            long newDeviceId = entity.Id;
            if(newDeviceId <= 0)
            {
                throw new Exception("新增设备记录后获取的DeviceId无效");
            }

            // 雪花ID:因为这里使用雪花生成的DeviceId当做业务主键，所以这里需要修改为下面的。
            newDeviceId = entity.Id;

            // 将传入的设备类型转换为 long，确保转换结果有效
            long deviceTypeId = long.Parse(dto.DeviceType);
            if(deviceTypeId <= 0)
            {
                throw new Exception("设备类型ID无效");
            }

            // 新增中间表记录
            // 注意：device_id 和 devicetype_id 是复合主键，不是自增字段，所以直接使用 InsertAsync 插入记录
            bool relationInserted = await _deviceManagementTypeRepository.InsertAsync(new DeviceManagementType
            {
                DeviceId = newDeviceId,
                DeviceTypeId = deviceTypeId,
                TenantId = 0 // 根据实际需求设置租户ID
            });

            return relationInserted;
        }


        /// <summary>
        /// 更新设备，同时更新中间表记录（先删除再新增）
        /// </summary>
        /// <param name="dto">包含设备信息和设备类型ID的DTO</param>
        /// <returns>操作是否成功</returns>
        /// <summary>
        /// 更新设备，同时更新中间表记录（先删除原中间表记录，再新增新的记录）
        /// </summary>
        /// <param name="dto">包含设备信息和设备类型ID的DTO，其中 deviceType 为前端传来的设备类型ID</param>
        /// <returns>操作是否成功</returns>
        [Transactional]
        public async Task<bool> UpdateDeviceWithTypeAsync(DeviceManagementDto dto)
        {
            // 校验设备类型不能为空
            if(string.IsNullOrEmpty(dto.DeviceType))
            {
                throw new ArgumentException("设备类型不能为空");
            }

            // 获取原始设备记录（根据dto.Id查询）
            var originalEntity = await _deviceManagementRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if(originalEntity == null)
            {
                throw new Exception("原始设备记录不存在");
            }

            // 判断设备类型是否发生变化
            bool typeChanged = !string.Equals(originalEntity.DeviceType,dto.DeviceType,StringComparison.OrdinalIgnoreCase);

            // 将 DTO 转换为 DeviceManagement 实体
            // 注意：更新时不要重新生成 DeviceId，保留原始的 DeviceId
            var entity = dto.Adapt<DeviceManagement>();
            entity.Id = originalEntity.Id;

            // 更新设备管理记录（调用 BaseService 中的 UpdateAsync）
            int updateCount = await UpdateAsync(entity);
            if(updateCount <= 0)
            {
                return false;
            }

            // 如果设备类型没有变化，则不需要更新中间表记录，直接返回
            if(!typeChanged)
            {
                return true;
            }

            // 获取更新后设备的主键ID（DeviceManagement 表中主键为自增）
            long deviceId = entity.Id;
            if(deviceId <= 0)
            {
                throw new Exception("设备记录更新后获取的DeviceId无效");
            }

            // 删除原有的中间表记录（根据 DeviceId 删除）
            int deleteCount = await _deviceManagementTypeRepository.Repo.Context
                .Deleteable<DeviceManagementType>()
                .Where(x => x.DeviceId == deviceId)
                .ExecuteCommandAsync();
            // 此处不必判断删除数量，可记录日志

            // 将传入的设备类型转换为 long，确保转换结果有效
            long deviceTypeId = long.Parse(dto.DeviceType);
            if(deviceTypeId <= 0)
            {
                throw new Exception("设备类型ID无效");
            }

            // 插入新的中间表记录
            bool relationInserted = await _deviceManagementTypeRepository.InsertAsync(new DeviceManagementType
            {
                DeviceId = deviceId,
                DeviceTypeId = deviceTypeId,
                TenantId = 0 // 根据实际需求设置租户ID
            });

            return relationInserted;
        }


        /// <summary>
        /// 删除设备，同时删除中间表记录（复合主键）
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>操作是否成功</returns>
        [Transactional]
        public async Task<bool> DeleteDeviceWithTypeAsync(long deviceId)
        {
            // 1. 删除设备管理记录，调用 BaseService 中的 DeleteAsync 方法
            int deleteCount = await DeleteAsync(deviceId);
            if(deleteCount <= 0)
            {
                return false;
            }

            // 2. 删除中间表记录（删除条件：DeviceId 等于传入 deviceId）
            int relationDeleteCount = await _deviceManagementTypeRepository.Repo.Context
                .Deleteable<DeviceManagementType>()
                .Where(x => x.DeviceId == deviceId)
                .ExecuteCommandAsync();

            return relationDeleteCount >= 0; // 允许中间表没有关联记录时返回0
        }






    }
}
