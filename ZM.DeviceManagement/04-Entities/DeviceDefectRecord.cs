using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    /// <summary>
    ///  缺陷/维修/抢修 记录表 对象 device_defect_record
    ///  author zgr
    ///  date   2025-04-10 10:16:07
    /// </summary>
    [SugarTable("device_defect_record", "设备缺陷记录表")]
    public class DeviceDefectRecord : UserBaseEntity
    {



        /// <summary>
        /// 主键ID (id)
        /// </summary>
        [SugarColumn(ColumnName = "id", ColumnDescription = "主键ID", IsPrimaryKey = true)]
        public long Id { get; set; }


        /// <summary>
        /// 设备ID (device_id)
        /// </summary>
        [SugarColumn(ColumnName = "device_id", ColumnDescription = "设备ID")]
        public long DeviceId { get; set; }

        /// <summary>
        /// 关联任务ID (task_id)
        /// </summary>
        [SugarColumn(ColumnName = "task_id", ColumnDescription = "关联任务ID")]
        public long TaskId { get; set; }
                
        /// <summary>
        /// 设备路径（厂房/车间/工序） (device_path)
        /// </summary>
        [SugarColumn(ColumnName = "device_path", ColumnDescription = "设备路径（厂房/车间/工序）")]
        public string? DevicePath { get; set; }
                
        /// <summary>
        /// 设备名称 (device_name)
        /// </summary>
        [SugarColumn(ColumnName = "device_name", ColumnDescription = "设备名称")]
        public string DeviceName { get; set; }
                
        /// <summary>
        /// 缺陷名称 (defect_name)
        /// </summary>
        [SugarColumn(ColumnName = "defect_name", ColumnDescription = "缺陷名称")]
        public string DefectName { get; set; }
                
        /// <summary>
        /// 缺陷描述 (defect_desc)
        /// </summary>
        [SugarColumn(ColumnName = "defect_desc", ColumnDescription = "缺陷描述")]
        public string? DefectDesc { get; set; }
                
        /// <summary>
        /// 缺陷状态（已处理/未处理） (defect_status)
        /// </summary>
        [SugarColumn(ColumnName = "defect_status", ColumnDescription = "缺陷状态（已处理/未处理）")]
        public string? DefectStatus { get; set; }
                
        /// <summary>
        /// 缺陷类别 (defect_category)
        /// </summary>
        [SugarColumn(ColumnName = "defect_category", ColumnDescription = "缺陷类别")]
        public string? DefectCategory { get; set; }
                
        /// <summary>
        /// 严重等级 (severity_level)
        /// </summary>
        [SugarColumn(ColumnName = "severity_level", ColumnDescription = "严重等级")]
        public string? SeverityLevel { get; set; }
                
        /// <summary>
        /// 处理建议 (suggestion)
        /// </summary>
        [SugarColumn(ColumnName = "suggestion", ColumnDescription = "处理建议")]
        public string? Suggestion { get; set; }
                
        /// <summary>
        /// 发现时间 (discovery_time)
        /// </summary>
        [SugarColumn(ColumnName = "discovery_time", ColumnDescription = "发现时间")]
        public DateTime? DiscoveryTime { get; set; }
                
        /// <summary>
        /// 消缺时间 (fix_time)
        /// </summary>
        [SugarColumn(ColumnName = "fix_time", ColumnDescription = "消缺时间")]
        public DateTime? FixTime { get; set; }
                
        /// <summary>
        /// 消缺人员 (fix_person)
        /// </summary>
        [SugarColumn(ColumnName = "fix_person", ColumnDescription = "消缺人员")]
        public string? FixPerson { get; set; }
                
        /// <summary>
        /// 消缺期限 (fix_deadline)
        /// </summary>
        [SugarColumn(ColumnName = "fix_deadline", ColumnDescription = "消缺期限")]
        public DateTime? FixDeadline { get; set; }
                
        /// <summary>
        /// 缺陷图片路径 (image_url)
        /// </summary>
        [SugarColumn(ColumnName = "image_url", ColumnDescription = "缺陷图片路径")]
        public string? ImageUrl { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

    }
}
