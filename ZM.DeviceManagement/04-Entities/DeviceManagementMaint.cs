using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    /// <summary>
    ///  设备信息与设备保养中间表 对象 device_management_maint
    ///  author ruoyi.net
    ///  date   2025-04-01 16:11:48
    /// </summary>
    [SugarTable("device_management_maint", "设备信息与设备保养中间表")]
    public class DeviceManagementMaint : BaseEntity
    {

        // 注意： 加上IsIdentity = true是错误的！！！
        [SugarColumn(ColumnName = "device_id",ColumnDescription = "设备CodeID",IsPrimaryKey = true)]
        public long DeviceId { get; set; }

        [SugarColumn(ColumnName = "maint_id",ColumnDescription = "保养CodeID",IsPrimaryKey = true)]
        public long MaintId { get; set; }

        [SugarColumn(ColumnName = "tenant_id",ColumnDescription = "所属集团ID")]
        public long? TenantId { get; set; }


    }
}
