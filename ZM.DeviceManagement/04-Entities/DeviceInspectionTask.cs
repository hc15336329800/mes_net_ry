using SqlSugar;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    /// <summary>
    /// 巡检任务表实体
    /// </summary>
    [SugarTable("device_inspection_task",TableDescription = "设备巡检任务表")]
    public class DeviceInspectionTask : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id",IsPrimaryKey = true,ColumnDescription = "主键")]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "task_name",ColumnDescription = "任务名称")]
        public string TaskName { get; set; }

        [SugarColumn(ColumnName = "ticket_id",ColumnDescription = "任务名称")]
        public long TicketId { get; set; }

        [SugarColumn(ColumnName = "leader",ColumnDescription = "负责人")]
        public string? Leader { get; set; }

        [SugarColumn(ColumnName = "executor",ColumnDescription = "执行人")]
        public string? Executor { get; set; }

        [SugarColumn(ColumnName = "task_type",ColumnDescription = "任务类型")]
        public string? TaskType { get; set; }

        [SugarColumn(ColumnName = "task_status",ColumnDescription = "任务状态")]
        public string? TaskStatus { get; set; }

        [SugarColumn(ColumnName = "check_in_deviation",ColumnDescription = "签到偏差距离（m）")]
        public int? CheckInDeviation { get; set; }

        [SugarColumn(ColumnName = "plan_start_time",ColumnDescription = "计划开始时间")]
        public DateTime? PlanStartTime { get; set; }

        [SugarColumn(ColumnName = "plan_end_time",ColumnDescription = "计划完成时间")]
        public DateTime? PlanEndTime { get; set; }

        [SugarColumn(ColumnName = "actual_end_time",ColumnDescription = "实际完成时间")]
        public DateTime? ActualEndTime { get; set; }

        [SugarColumn(ColumnName = "device_ids",ColumnDescription = "设备ID列表（JSON）")]
        public string? DeviceIds { get; set; }

        [SugarColumn(ColumnName = "remark",ColumnDescription = "备注")]
        public string? Remark { get; set; }

 

    }
}
