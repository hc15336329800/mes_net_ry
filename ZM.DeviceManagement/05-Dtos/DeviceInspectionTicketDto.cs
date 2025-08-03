using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    /// <summary>
    ///  设备巡检操作票表 对象 device_inspection_ticket
    ///  author ruoyi.net
    ///  date   2025-04-07 10:31:04
    /// </summary>
    public class DeviceInspectionTicketDto : BaseDto
    {
        /// <summary>
        /// 主键ID（不自增）
        /// </summary>
        public long Id { get; set; }
                
        /// <summary>
        /// 操作票名称
        /// </summary>
        public string Name { get; set; }
                
        /// <summary>
        /// 设备ID列表（JSON格式）
        /// </summary>
        public string? DeviceIds { get; set; }
                
        /// <summary>
        /// 设备数量
        /// </summary>
        public string? Number { get; set; }
                
    }
}
