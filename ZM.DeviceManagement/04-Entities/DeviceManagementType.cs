using RuoYi.Data.Entities;
using SqlSugar;

namespace RuoYi.Device.Entities
{
    /// <summary>
    /// 设备管理与设备类型中间表
    /// </summary>
    [SugarTable("device_management_type",TableDescription = "设备管理与设备类型中间表")]
    public class DeviceManagementType : UserBaseEntity
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(ColumnName = "device_id",IsPrimaryKey = true,ColumnDescription = "设备ID")]
        public long DeviceId { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        [SugarColumn(ColumnName = "devicetype_id",IsPrimaryKey = true,ColumnDescription = "设备类型ID")]
        public long DeviceTypeId { get; set; }

        /// <summary>
        /// 租户ID（可空，默认为0）
        /// </summary>
        [SugarColumn(ColumnName = "tenant_id",ColumnDescription = "租户ID",IsNullable = true)]
        public long TenantId { get; set; } = 0;
    }
}
