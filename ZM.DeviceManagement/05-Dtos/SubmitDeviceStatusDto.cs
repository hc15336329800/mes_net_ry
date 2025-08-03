using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZM.Device._05_Dtos
{

    /// <summary>
    /// submitDeviceStatus接口的参数
    /// </summary>
    public class SubmitDeviceStatusDto
    {
        public long TaskId { get; set; } //任务id
        public long Id { get; set; }  //id
        public string DevStatus { get; set; } // 合格或者完成标志 （"false"/"true"）
        public bool IsCompleted { get; set; } //巡检任务使用
        public string Remark { get; set; } //描述/备注
        public string? ImageUrl { get; set; } //图片地址
    }
}
