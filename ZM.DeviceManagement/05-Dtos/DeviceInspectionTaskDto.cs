using RuoYi.Data.Dtos;
using SqlSugar;

namespace ZM.Device.Dtos
{
    /// <summary>
    /// 巡检任务 DTO
    /// </summary>
    public class DeviceInspectionTaskDto : BaseDto
    {
        

        public long RelatedTaskId { get; set; } // 新增  任务单号

        public long Id { get; set; }
        public string TaskName { get; set; }
        public long? TicketId { get; set; }

        public long? DeviceId { get; set; }  // 设备id  ,维修抢修专用
        public string? DeviceName { get; set; }  // 设备名称  ,维修抢修专用


        public string? Leader { get; set; }
        public string? Executor { get; set; }
        public string? TaskType { get; set; }
        [SugarColumn(ColumnName = "task_status")] // ✨这个一定要加！
        public string? TaskStatus { get; set; }
        public int? CheckInDeviation { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string? DeviceIds { get; set; }

        public string? ImageUrl { get; set; } //图片后增


        public long DevId { get; set; } // 记录表视图使用
        public String? DevName { get; set; } // 记录表视图使用

        public string? Remark { get; set; } // 🔥 必须显式添加！！

        [SugarColumn(IsIgnore = true)]
        public string LeaderId { get; set; }  // 新增：用于存储原始用户ID

        [SugarColumn(IsIgnore = true)]
        public string ExecutorId { get; set; } // 新增：用于存储原始用户ID


    }
}
