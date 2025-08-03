using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Framework.DependencyInjection;
 
using RuoYi.System.Repositories;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;

namespace ZM.Device.Services
{    
    /// <summary>
    ///  设备巡检操作票表 Service
    ///  author ruoyi.net
    ///  date   2025-04-07 10:31:04
    /// </summary>
    public class DeviceInspectionTicketService : BaseService<DeviceInspectionTicket, DeviceInspectionTicketDto>, ITransient
    {
        private readonly ILogger<DeviceInspectionTicketService> _logger;
        private readonly DeviceInspectionTicketRepository _deviceInspectionTicketRepository;

        public DeviceInspectionTicketService(ILogger<DeviceInspectionTicketService> logger,
            DeviceInspectionTicketRepository deviceInspectionTicketRepository)
        {
            BaseRepo = deviceInspectionTicketRepository;

            _logger = logger;
            _deviceInspectionTicketRepository = deviceInspectionTicketRepository;
        }

        /// <summary>
        /// 查询 设备巡检操作票表 详情
        /// </summary>
        public async Task<DeviceInspectionTicket> GetAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            return entity;
        }

        /// <summary>
        /// 查询 设备巡检操作票表 详情
        /// </summary>
        public async Task<DeviceInspectionTicketDto> GetDtoAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            var dto = entity.Adapt<DeviceInspectionTicketDto>();
            // TODO 填充关联表数据
            return dto;
        }
    }
}