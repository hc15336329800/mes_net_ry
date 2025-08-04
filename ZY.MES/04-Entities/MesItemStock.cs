using System;
using SqlSugar;
using RuoYi.Data.Entities;

namespace ZY.MES._04_Entities
{
    /// <summary>
    /// 物料库存表
    /// </summary>
    [SugarTable("mes_item_stock","物料库存表")]
    public class MesItemStock : BaseEntity
    {
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true)]
        public string Id { get; set; } = null!;

        [SugarColumn(ColumnName = "item_no",ColumnDescription = "物料编号",Length = 32)]
        public string ItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "item_name",ColumnDescription = "物料名称",Length = 90)]
        public string ItemName { get; set; } = null!;

        [SugarColumn(ColumnName = "item_model",ColumnDescription = "规格型号",Length = 100)]
        public string? ItemModel { get; set; }

        [SugarColumn(ColumnName = "item_count",ColumnDescription = "库存数量",DecimalDigits = 3)]
        public decimal? ItemCount { get; set; }

        [SugarColumn(ColumnName = "item_measure",ColumnDescription = "计量单位",Length = 20)]
        public string? ItemMeasure { get; set; }

        [SugarColumn(ColumnName = "item_count_assist",ColumnDescription = "辅助库存数量",DecimalDigits = 3)]
        public decimal? ItemCountAssist { get; set; }

        [SugarColumn(ColumnName = "item_measure_assist",ColumnDescription = "辅助计量单位",Length = 20)]
        public string? ItemMeasureAssist { get; set; }

        [SugarColumn(ColumnName = "bom_no",ColumnDescription = "BOM编号",Length = 50)]
        public string? BomNo { get; set; }

        [SugarColumn(ColumnName = "item_type",ColumnDescription = "物料类型",Length = 2)]
        public string? ItemType { get; set; }

        [SugarColumn(ColumnName = "location",ColumnDescription = "库位",Length = 20)]
        public string? Location { get; set; }

        [SugarColumn(ColumnName = "remark",ColumnDescription = "备注",Length = 255)]
        public string? Remark { get; set; }


        [SugarColumn(ColumnName = "erp_count",ColumnDescription = "ERP库存数量",DecimalDigits = 3)]
        public decimal? ErpCount { get; set; }

        [SugarColumn(ColumnName = "net_weight",ColumnDescription = "净重",DecimalDigits = 3)]
        public decimal? NetWeight { get; set; }

        [SugarColumn(ColumnName = "is_valid",ColumnDescription = "有效标识",Length = 2)]
        public string? IsValid { get; set; }

        [SugarColumn(ColumnName = "uni_id",ColumnDescription = "单位ID",Length = 11)]
        public int? UniId { get; set; }

        // 新添加字段
        [SugarColumn(ColumnName = "item_origin",ColumnDescription = "来源：自制或采购",Length = 10)]
        public string? ItemOrigin { get; set; }  // 添加物料来源字段





        [SugarColumn(ColumnName = "created_by",ColumnDescription = "创建人",Length = 255)]
        public string? CreatedBy { get; set; }

        [SugarColumn(ColumnName = "created_time",ColumnDescription = "创建时间")]
        public DateTime? CreatedTime { get; set; }

        [SugarColumn(ColumnName = "updated_by",ColumnDescription = "更新人",Length = 255)]
        public string? UpdatedBy { get; set; }

        [SugarColumn(ColumnName = "updated_time",ColumnDescription = "更新时间")]
        public DateTime? UpdatedTime { get; set; }
    }
}
