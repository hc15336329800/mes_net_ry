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
    ///  设备信息与设备保养中间表 Service
    ///  author ruoyi.net
    ///  date   2025-04-01 16:11:48
    /// </summary>
    public class DeviceManagementMaintService : BaseService<DeviceManagementMaint, DeviceManagementMaintDto>, ITransient
    {
        private readonly ILogger<DeviceManagementMaintService> _logger;
        private readonly DeviceManagementMaintRepository _deviceManagementMaintRepository;

        public DeviceManagementMaintService(ILogger<DeviceManagementMaintService> logger,
            DeviceManagementMaintRepository deviceManagementMaintRepository)
        {
            BaseRepo = deviceManagementMaintRepository;

            _logger = logger;
            _deviceManagementMaintRepository = deviceManagementMaintRepository;
        }

        /// <summary>
        /// 查询 设备信息与设备保养中间表 详情
        /// </summary>
        public async Task<DeviceManagementMaint> GetAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.DeviceId == id);
            return entity;
        }

        /// <summary>
        /// 查询 设备信息与设备保养中间表 详情
        /// </summary>
        public async Task<DeviceManagementMaintDto> GetDtoAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.DeviceId == id);
            var dto = entity.Adapt<DeviceManagementMaintDto>();
            // TODO 填充关联表数据
            return dto;
        }
    }
}