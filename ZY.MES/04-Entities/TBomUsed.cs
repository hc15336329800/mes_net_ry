using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Data.Entities;
using SqlSugar;

namespace ZY.MES._04_Entities
{
    /// <summary>
    /// 完整BOM表
    /// </summary>
    [SugarTable("t_bom_used","完整BOM表")]
    public class TBomUsed : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "bom_no",ColumnDescription = "BOM编码")]
        public string BomCode { get; set; }

        [SugarColumn(ColumnName = "parent_code",ColumnDescription = "父物料编码")]
        public string? ParentCode { get; set; }

        [SugarColumn(ColumnName = "use_item_no",ColumnDescription = "物料编码")]
        public string ItemCode { get; set; }

        [SugarColumn(ColumnName = "use_item_count",ColumnDescription = "使用数量")]
        public decimal? Quantity { get; set; }

        [SugarColumn(ColumnName = "use_item_type",ColumnDescription = "使用类型")]
        public string? Level { get; set; }
    }
}
