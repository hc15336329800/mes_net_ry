using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    /// <summary>
    ///  保养记录表 对象 device_maint
    ///  author ruoyi.net
    ///  date   2025-04-01 16:06:17
    /// </summary>
    public class DeviceMaintDto : BaseDto
    {


        /// <summary>
        ///  所属设备id
        /// </summary>
        public long DeviceId { get; set; }


        /// <summary>
        ///  主键
        /// </summary>
        public long Id { get; set; }
                
 
                
        /// <summary>
        /// 保养内容
        /// </summary>
        public string? Content { get; set; }

 

        /// <summary>
        /// 备注 (remark)
        /// </summary>
        public string? Remark { get; set; }

     /// <summary>
        /// 图片
        /// </summary>
        public string? ImageUrl { get; set; }

    }
}
