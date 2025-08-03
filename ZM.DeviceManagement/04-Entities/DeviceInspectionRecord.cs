using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    /// <summary>
    ///  巡检记录表 对象 device_inspection_record
    ///  author zgr.net
    ///  date   2025-04-09 14:10:45
    /// </summary>
    [SugarTable("device_inspection_record", "缺陷/维修/抢修记录表（统一结构）")]
    public class DeviceInspectionRecord : UserBaseEntity
    {
        /// <summary>
        /// 主键 (id)
        /// </summary>
        [SugarColumn(ColumnName = "id", ColumnDescription = "主键", IsPrimaryKey = true)]
        public long Id { get; set; }
                
        /// <summary>
        /// 任务ID，关联 device_inspection_task(id) (task_id)
        /// </summary>
        [SugarColumn(ColumnName = "task_id", ColumnDescription = "任务ID，关联 device_inspection_task(id)")]
        public long TaskId { get; set; }
                
        /// <summary>
        /// 任务名称 (task_name)
        /// </summary>
        [SugarColumn(ColumnName = "task_name", ColumnDescription = "任务名称")]
        public string TaskName { get; set; }
                
        /// <summary>
        /// 负责人 (leader)
        /// </summary>
        [SugarColumn(ColumnName = "leader", ColumnDescription = "负责人")]
        public string? Leader { get; set; }
                
        /// <summary>
        /// 执行人 (executor)
        /// </summary>
        [SugarColumn(ColumnName = "executor", ColumnDescription = "执行人")]
        public string? Executor { get; set; }
                
        /// <summary>
        /// 任务类型（巡检、维修、抢修） (task_type)
        /// </summary>
        [SugarColumn(ColumnName = "task_type", ColumnDescription = "任务类型（巡检、维修、抢修）")]
        public string? TaskType { get; set; }
                
        /// <summary>
        /// 任务状态（待办、在办、办毕） (task_status)
        /// </summary>
        [SugarColumn(ColumnName = "task_status", ColumnDescription = "任务状态（待办、在办、办毕）")]
        public string? TaskStatus { get; set; }
                
        /// <summary>
        /// 签到偏差距离（单位m） (check_in_deviation)
        /// </summary>
        [SugarColumn(ColumnName = "check_in_deviation", ColumnDescription = "签到偏差距离（单位m）")]
        public int? CheckInDeviation { get; set; }
                
        /// <summary>
        /// 任务开始时间 (task_start_time)
        /// </summary>
        [SugarColumn(ColumnName = "task_start_time", ColumnDescription = "任务开始时间")]
        public DateTime? TaskStartTime { get; set; }
                
        /// <summary>
        /// 任务结束时间 (task_end_time)
        /// </summary>
        [SugarColumn(ColumnName = "task_end_time", ColumnDescription = "任务结束时间")]
        public DateTime? TaskEndTime { get; set; }
                
        /// <summary>
        /// 计划完成时间 (plan_end_time)
        /// </summary>
        [SugarColumn(ColumnName = "plan_end_time", ColumnDescription = "计划完成时间")]
        public DateTime? PlanEndTime { get; set; }
                
        /// <summary>
        /// 巡检设备数量 (device_count)
        /// </summary>
        [SugarColumn(ColumnName = "device_count", ColumnDescription = "巡检设备数量")]
        public int? DeviceCount { get; set; }
                
        /// <summary>
        /// 缺陷总个数 (defect_total)
        /// </summary>
        [SugarColumn(ColumnName = "defect_total", ColumnDescription = "缺陷总个数")]
        public int? DefectTotal { get; set; }
                
        /// <summary>
        /// 缺陷已处理数量 (defect_processed)
        /// </summary>
        [SugarColumn(ColumnName = "defect_processed", ColumnDescription = "缺陷已处理数量")]
        public int? DefectProcessed { get; set; }
                
        /// <summary>
        /// 巡检设备ID列表（JSON存储） (inspected_device_ids)
        /// </summary>
        [SugarColumn(ColumnName = "inspected_device_ids", ColumnDescription = "巡检设备ID列表（JSON存储）")]
        public string? InspectedDeviceIds { get; set; }
                
        /// <summary>
        /// 已巡检设备ID列表（JSON存储） (finished_device_ids)
        /// </summary>
        [SugarColumn(ColumnName = "finished_device_ids", ColumnDescription = "已巡检设备ID列表（JSON存储）")]
        public string? FinishedDeviceIds { get; set; }
                
    }
}
