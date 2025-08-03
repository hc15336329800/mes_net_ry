using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    /// <summary>
    ///  保养记录表 对象 device_maint
    ///  author ruoyi.net
    ///  date   2025-04-01 16:06:17
    /// </summary>
    [SugarTable("device_maint", "保养记录表")]
    public class DeviceMaint : UserBaseEntity
    {
        /// <summary>
        /// 主键 (id)
        /// </summary>
        [SugarColumn(ColumnName = "id", ColumnDescription = "自增主键", IsPrimaryKey = true)]
        public long Id { get; set; }
                
 

        /// <summary>
        /// 保养内容 (content)
        /// </summary>
        [SugarColumn(ColumnName = "content", ColumnDescription = "保养内容")]
        public string? Content { get; set; }


        /// <summary>
        /// 备注 (remark)
        /// </summary>
        [SugarColumn(ColumnName = "remark",ColumnDescription = "备注")]
        public string? Remark { get; set; }


        /// <summary>
        /// 图片
        /// </summary>
       [SugarColumn(ColumnName = "image_url",ColumnDescription = "图片")]

        public string? ImageUrl { get; set; }


    }
}
