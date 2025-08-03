using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Data.Dtos;

namespace RuoYi.Device.Entities
{
    /// <summary>
    /// 设备管理与设备类型中间表 DTO
    /// </summary>
    public class DeviceManagementTypeDto : BaseDto
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public long DeviceId { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public long DeviceTypeId { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public long TenantId { get; set; }
    }
}
