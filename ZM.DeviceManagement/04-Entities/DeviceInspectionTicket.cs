using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    /// <summary>
    ///  设备巡检操作票表 对象 device_inspection_ticket
    ///  author ruoyi.net
    ///  date   2025-04-07 10:31:04
    /// </summary>
    [SugarTable("device_inspection_ticket", "设备巡检操作票表")]
    public class DeviceInspectionTicket : BaseEntity
    {
        /// <summary>
        /// 主键ID（不自增） (id)
        /// </summary>
        [SugarColumn(ColumnName = "id", ColumnDescription = "主键ID（不自增）", IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; }
                
        /// <summary>
        /// 操作票名称 (name)
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDescription = "操作票名称")]
        public string Name { get; set; }
                
        /// <summary>
        /// 设备ID列表（JSON格式） (device_ids)
        /// </summary>
        [SugarColumn(ColumnName = "device_ids", ColumnDescription = "设备ID列表（JSON格式）")]
        public string? DeviceIds { get; set; }
                
        /// <summary>
        /// 设备数量 (number)
        /// </summary>
        [SugarColumn(ColumnName = "number", ColumnDescription = "设备数量")]
        public string? Number { get; set; }
                
    }
}
