using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    /// <summary>
    ///  设备信息与设备保养中间表 对象 device_management_maint
    ///  author ruoyi.net
    ///  date   2025-04-01 16:11:48
    /// </summary>
    public class DeviceManagementMaintDto : BaseDto
    {
        /// <summary>
        /// 设备CodeID
        /// </summary>
        public long DeviceId { get; set; }
                
        /// <summary>
        /// 保养CodeID
        /// </summary>
        public long MaintId { get; set; }
                
        /// <summary>
        /// 所属集团ID
        /// </summary>
        public long? TenantId { get; set; }
                
    }
}
