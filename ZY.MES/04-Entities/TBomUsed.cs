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

        [SugarColumn(ColumnName = "item_no",ColumnDescription = "物料编号")]
        public string ItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "bom_no",ColumnDescription = "BOM编码")]
        public string BomNo { get; set; } = null!;

        [SugarColumn(ColumnName = "parent_code",ColumnDescription = "父物料编码")]
        public string? ParentCode { get; set; }

        [SugarColumn(ColumnName = "use_item_no",ColumnDescription = "使用物料编号")]
        public string UseItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "fixed_used",ColumnDescription = "使用数量")]
        public decimal? FixedUsed { get; set; }

        [SugarColumn(ColumnName = "use_item_type",ColumnDescription = "使用类型")]
        public string? UseItemType { get; set; }
    }
}
