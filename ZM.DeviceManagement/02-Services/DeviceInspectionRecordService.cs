using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Framework.DependencyInjection;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;

namespace ZM.Device.Services
{    
    /// <summary>
    ///  巡检记录表 Service
    ///  author zgr.net
    ///  date   2025-04-09 14:10:45
    /// </summary>
    public class DeviceInspectionRecordService : BaseService<DeviceInspectionRecord, DeviceInspectionRecordDto>, ITransient
    {
        private readonly ILogger<DeviceInspectionRecordService> _logger;
        private readonly DeviceInspectionRecordRepository _deviceInspectionRecordRepository;

        public DeviceInspectionRecordService(ILogger<DeviceInspectionRecordService> logger,
            DeviceInspectionRecordRepository deviceInspectionRecordRepository)
        {
            BaseRepo = deviceInspectionRecordRepository;

            _logger = logger;
            _deviceInspectionRecordRepository = deviceInspectionRecordRepository;
        }

        /// <summary>
        /// 查询 巡检记录表 详情
        /// </summary>
        public async Task<DeviceInspectionRecord> GetAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            return entity;
        }

        /// <summary>
        /// 查询 巡检记录表 详情
        /// </summary>
        public async Task<DeviceInspectionRecordDto> GetDtoAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            var dto = entity.Adapt<DeviceInspectionRecordDto>();
            // TODO 填充关联表数据
            return dto;
        }
    }
}