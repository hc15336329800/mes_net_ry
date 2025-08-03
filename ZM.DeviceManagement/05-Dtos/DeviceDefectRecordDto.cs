using RuoYi.Data.Dtos;
using SqlSugar;

namespace ZM.Device.Dtos
{
    /// <summary>
    ///  设备缺陷记录表 对象 device_defect_record
    ///  author zgr
    ///  date   2025-04-10 10:16:07
    /// </summary>
    public class DeviceDefectRecordDto : BaseDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 设备ID (device_id)
        /// </summary>
        public long DeviceId { get; set; }


        /// <summary>
        /// 关联任务ID
        /// </summary>
        public long TaskId { get; set; }
                
        /// <summary>
        /// 设备路径（厂房/车间/工序）
        /// </summary>
        public string? DevicePath { get; set; }
                
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
                
        /// <summary>
        /// 缺陷名称
        /// </summary>
        public string DefectName { get; set; }
                
        /// <summary>
        /// 缺陷描述
        /// </summary>
        public string? DefectDesc { get; set; }
                
        /// <summary>
        /// 缺陷状态（已处理/未处理）
        /// </summary>
        public string? DefectStatus { get; set; }
                
        /// <summary>
        /// 缺陷类别
        /// </summary>
        public string? DefectCategory { get; set; }
                
        /// <summary>
        /// 严重等级
        /// </summary>
        public string? SeverityLevel { get; set; }
                
        /// <summary>
        /// 处理建议
        /// </summary>
        public string? Suggestion { get; set; }
                
        /// <summary>
        /// 发现时间
        /// </summary>
        public DateTime? DiscoveryTime { get; set; }
                
        /// <summary>
        /// 消缺时间
        /// </summary>
        public DateTime? FixTime { get; set; }
                
        /// <summary>
        /// 消缺人员
        /// </summary>
        public string? FixPerson { get; set; }
                
        /// <summary>
        /// 消缺期限
        /// </summary>
        public DateTime? FixDeadline { get; set; }
                
        /// <summary>
        /// 缺陷图片路径
        /// </summary>
        public string? ImageUrl { get; set; }
                
    }
}
