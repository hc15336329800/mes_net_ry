using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using SqlSugar;

namespace ZM.Device.Entities
{
     public class DeviceManagementDto : BaseDto
    {

        /// <summary>
        /// 新增临时字段：保养倒计时
        /// </summary>
        public int? MaintenanceCountdown { get; set; }



        /// <summary>
        /// 上次保养时间 后增字段
        /// </summary>
        public DateTime? LastMaintenanceTime { get; set; }

        /// <summary>
        ///  主键
        /// </summary>
        public long Id { get; set; }

   

        /// <summary>
        /// 设备名称，必填
        /// </summary>
         public string Label { get; set; }

        /// <summary>
        /// 设备类型，可为空
        /// </summary>
         public string? DeviceType { get; set; }

        /// <summary>
        /// 型号规格，可为空
        /// </summary>
         public string? Model { get; set; }

        /// <summary>
        /// 额定容量，单位L，可为空
        /// </summary>
         public decimal? Capacity { get; set; }

        /// <summary>
        /// 设备数量，必填
        /// </summary>
         public int Quantity { get; set; }

        /// <summary>
        /// 设备重量，单位KG，可为空
        /// </summary>
         public decimal? Weight { get; set; }

        /// <summary>
        /// 生产厂家，可为空
        /// </summary>
         public string? Manufacturer { get; set; }

        /// <summary>
        /// 安装时间，可为空
        /// </summary>
         public DateTime? InstallDate { get; set; }

        /// <summary>
        /// 额定电流，单位A，可为空
        /// </summary>
         public decimal? RatedCurrent { get; set; }

        /// <summary>
        /// 额定电压，单位V，可为空
        /// </summary>
         public decimal? RatedVoltage { get; set; }

        /// <summary>
        /// 设备当前状态，如运行、待机、故障等，可为空
        /// </summary>
         public string? Status { get; set; }

        /// <summary>
        /// 温控要求，单位℃
        /// </summary>
         public decimal? TempControl { get; set; }

        /// <summary>
        /// 维护周期，单位天，可为空
        /// </summary>
         public int? MaintenanceCycle { get; set; }

        /// <summary>
        /// 原保期（保修到期时间），可为空
        /// </summary>
         public DateTime? WarrantyPeriod { get; set; }

        /// <summary>
        /// 设备所属工序ID，必填（存储工序树中的ID或自定义标识）
        /// </summary>
         public int ProcessId { get; set; }

        ///// <summary>
        ///// 记录创建时间
        ///// </summary>
        // public DateTime? CreateTime { get; set; }

        ///// <summary>
        ///// 记录更新时间
        ///// </summary>
        // public DateTime? UpdateTime { get; set; }
    }
}
