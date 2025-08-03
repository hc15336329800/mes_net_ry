using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    /// <summary>
    ///  巡检记录表 对象 device_inspection_record
    ///  author zgr.net
    ///  date   2025-04-09 14:10:45
    /// </summary>
    public class DeviceInspectionRecordDto : BaseDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
                
        /// <summary>
        /// 任务ID，关联 device_inspection_task(id)
        /// </summary>
        public long TaskId { get; set; }
                
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
                
        /// <summary>
        /// 负责人
        /// </summary>
        public string? Leader { get; set; }
                
        /// <summary>
        /// 执行人
        /// </summary>
        public string? Executor { get; set; }
                
        /// <summary>
        /// 任务类型（巡检、维修、抢修）
        /// </summary>
        public string? TaskType { get; set; }
                
        /// <summary>
        /// 任务状态（待办、在办、办毕）
        /// </summary>
        public string? TaskStatus { get; set; }
                
        /// <summary>
        /// 签到偏差距离（单位m）
        /// </summary>
        public int? CheckInDeviation { get; set; }
                
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime? TaskStartTime { get; set; }
                
        /// <summary>
        /// 任务结束时间
        /// </summary>
        public DateTime? TaskEndTime { get; set; }
                
        /// <summary>
        /// 计划完成时间
        /// </summary>
        public DateTime? PlanEndTime { get; set; }
                
        /// <summary>
        /// 巡检设备数量
        /// </summary>
        public int? DeviceCount { get; set; }
                
        /// <summary>
        /// 缺陷总个数
        /// </summary>
        public int? DefectTotal { get; set; }
                
        /// <summary>
        /// 缺陷已处理数量
        /// </summary>
        public int? DefectProcessed { get; set; }
                
        /// <summary>
        /// 巡检设备ID列表（JSON存储）
        /// </summary>
        public string? InspectedDeviceIds { get; set; }
                
        /// <summary>
        /// 已巡检设备ID列表（JSON存储）
        /// </summary>
        public string? FinishedDeviceIds { get; set; }
                
    }
}
