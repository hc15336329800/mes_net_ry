using RuoYi.Data.Entities;
using SqlSugar;

namespace ZM.Device.Entities
{
    [SugarTable("device_management","设备管理表")]
    public class DeviceManagement : UserBaseEntity
    {

        /// <summary>
        /// 上次保养时间 后增字段
        /// </summary>
        [SugarColumn(ColumnName = "last_maintenance_time",ColumnDescription = "上次保养时间，可为空")]
        public DateTime? LastMaintenanceTime { get; set; }


        /// <summary>
        /// 主键 (id)
        /// </summary>
        [SugarColumn(ColumnName = "id",ColumnDescription = "自增主键",IsPrimaryKey = true)]
        public long Id { get; set; }


         /// <summary>
        /// 设备名称， 
        /// </summary>
        [SugarColumn(ColumnName = "remark",ColumnDescription = "保养内容")]
        public string? Remark { get; set; }

        /// <summary>
        /// 设备名称，必填
        /// </summary>
        [SugarColumn(ColumnName = "label",ColumnDescription = "设备名称，必填")]
        public string Label { get; set; }

        /// <summary>
        /// 设备类型，可为空
        /// </summary>
        [SugarColumn(ColumnName = "device_type",ColumnDescription = "设备类型，可为空")]
        public string? DeviceType { get; set; }

        /// <summary>
        /// 型号规格，可为空
        /// </summary>
        [SugarColumn(ColumnName = "model",ColumnDescription = "型号规格，可为空")]
        public string? Model { get; set; }

        /// <summary>
        /// 额定容量，单位L，可为空
        /// </summary>
        [SugarColumn(ColumnName = "capacity",ColumnDescription = "额定容量（单位L），可为空")]
        public decimal? Capacity { get; set; }

        /// <summary>
        /// 设备数量，必填
        /// </summary>
        [SugarColumn(ColumnName = "quantity",ColumnDescription = "设备数量，必填")]
        public int Quantity { get; set; }

        /// <summary>
        /// 设备重量，单位KG，可为空
        /// </summary>
        [SugarColumn(ColumnName = "weight",ColumnDescription = "设备重量（单位KG），可为空")]
        public decimal? Weight { get; set; }

        /// <summary>
        /// 生产厂家，可为空
        /// </summary>
        [SugarColumn(ColumnName = "manufacturer",ColumnDescription = "生产厂家，可为空")]
        public string? Manufacturer { get; set; }

        /// <summary>
        /// 安装时间，可为空
        /// </summary>
        [SugarColumn(ColumnName = "install_date",ColumnDescription = "安装时间，可为空")]
        public DateTime? InstallDate { get; set; }

        /// <summary>
        /// 额定电流，单位A，可为空
        /// </summary>
        [SugarColumn(ColumnName = "rated_current",ColumnDescription = "额定电流（单位A），可为空")]
        public decimal? RatedCurrent { get; set; }

        /// <summary>
        /// 额定电压，单位V，可为空
        /// </summary>
        [SugarColumn(ColumnName = "rated_voltage",ColumnDescription = "额定电压（单位V），可为空")]
        public decimal? RatedVoltage { get; set; }

        /// <summary>
        /// 设备当前状态，如运行、待机、故障等，可为空
        /// </summary>
        [SugarColumn(ColumnName = "status",ColumnDescription = "设备当前状态（如运行、待机、故障），可为空")]
        public string? Status { get; set; }

        /// <summary>
        /// 温控要求，单位℃
        /// </summary>
        [SugarColumn(ColumnName = "temp_control",ColumnDescription = "温控要求（单位℃），可为空")]
        public decimal? TempControl { get; set; }

        /// <summary>
        /// 维护周期，单位天，可为空
        /// </summary>
        [SugarColumn(ColumnName = "maintenance_cycle",ColumnDescription = "维护周期（单位天），可为空")]
        public int? MaintenanceCycle { get; set; }

        /// <summary>
        /// 原保期（保修到期时间），可为空
        /// </summary>
        [SugarColumn(ColumnName = "warranty_period",ColumnDescription = "原保期（保修到期时间），可为空")]
        public DateTime? WarrantyPeriod { get; set; }

        /// <summary>
        /// 设备所属工序ID，必填（存储工序树中的ID或自定义标识）
        /// </summary>
        [SugarColumn(ColumnName = "process_id",ColumnDescription = "设备所属工序ID，必填")]
        public int ProcessId { get; set; }

        ///// <summary>
        ///// 记录创建时间
        ///// </summary>
        //[SugarColumn(ColumnName = "create_time",ColumnDescription = "记录创建时间",IsOnlyIgnoreInsert = true)]
        //public DateTime? CreateTime { get; set; }

        ///// <summary>
        ///// 记录更新时间
        ///// </summary>
        //[SugarColumn(ColumnName = "update_time",ColumnDescription = "记录更新时间",IsOnlyIgnoreInsert = true)]
        //public DateTime? UpdateTime { get; set; }
    }
}
