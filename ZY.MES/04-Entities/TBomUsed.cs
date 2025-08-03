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
    public class TBomUsed : BaseEntity
    {
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "item_no",ColumnDescription = "物料编码 父节点")]
        public string ItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "bom_no",ColumnDescription = "bom编码")]
        public string BomNo { get; set; } = null!;

        [SugarColumn(ColumnName = "use_item_no",ColumnDescription = "物理编码 子节点")]
        public string UseItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "use_item_count",ColumnDescription = "用料数量")]
        public decimal UseItemCount { get; set; } = 0.000m;

        [SugarColumn(ColumnName = "use_item_type",ColumnDescription = "物料类型,00:物料，01:BOM")]
        public string? UseItemType { get; set; }

        [SugarColumn(ColumnName = "parent_code",ColumnDescription = "父编码")]
        public string? ParentCode { get; set; }

        [SugarColumn(ColumnName = "item_nos",ColumnDescription = "物料长编码")]
        public string? ItemNos { get; set; }

        [SugarColumn(ColumnName = "used_id",ColumnDescription = "用料表ID")]
        public string? UsedId { get; set; }

        [SugarColumn(ColumnName = "fixed_used",ColumnDescription = "固定用量")]
        public decimal? FixedUsed { get; set; }

        [SugarColumn(ColumnName = "created_by",ColumnDescription = "创建人")]
        public string? CreatedBy { get; set; }

        [SugarColumn(ColumnName = "created_time",ColumnDescription = "创建时间")]
        public DateTime? CreatedTime { get; set; }

        [SugarColumn(ColumnName = "updated_by",ColumnDescription = "更新人")]
        public string? UpdatedBy { get; set; }

        [SugarColumn(ColumnName = "updated_time",ColumnDescription = "更新时间")]
        public DateTime? UpdatedTime { get; set; }
    }
}
