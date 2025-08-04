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
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true,IsIdentity = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "item_no",ColumnDescription = "物料编号",Length = 50)]
        public string ItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "item_name",ColumnDescription = "物料名称",Length = 100)]
        public string ItemName { get; set; } = null!;

        [SugarColumn(ColumnName = "item_model",ColumnDescription = "规格型号",Length = 100)]
        public string? ItemModel { get; set; }

        [SugarColumn(ColumnName = "item_count",ColumnDescription = "库存数量",DecimalDigits = 2)]
        public decimal? ItemCount { get; set; }

        [SugarColumn(ColumnName = "item_measure",ColumnDescription = "计量单位",Length = 20)]
        public string? ItemMeasure { get; set; }

        [SugarColumn(ColumnName = "min_stock",ColumnDescription = "最小库存量",DecimalDigits = 2)]
        public decimal? MinStock { get; set; }

        [SugarColumn(ColumnName = "max_stock",ColumnDescription = "最大库存量",DecimalDigits = 2)]
        public decimal? MaxStock { get; set; }

        [SugarColumn(ColumnName = "warehouse_id",ColumnDescription = "仓库ID")]
        public long? WarehouseId { get; set; }

        [SugarColumn(ColumnName = "warehouse_name",ColumnDescription = "仓库名称",Length = 50)]
        public string? WarehouseName { get; set; }

        [SugarColumn(ColumnName = "location_code",ColumnDescription = "库位编码",Length = 50)]
        public string? LocationCode { get; set; }

        [SugarColumn(ColumnName = "item_category",ColumnDescription = "物料分类",Length = 50)]
        public string? ItemCategory { get; set; }

        [SugarColumn(ColumnName = "item_status",ColumnDescription = "物料状态(0-正常,1-停用)",DefaultValue = "0")]
        public string? ItemStatus { get; set; }

        [SugarColumn(ColumnName = "remark",ColumnDescription = "备注",Length = 500)]
        public string? Remark { get; set; }

        [SugarColumn(ColumnName = "created_by",ColumnDescription = "创建人",Length = 50)]
        public string? CreatedBy { get; set; }

        [SugarColumn(ColumnName = "created_time",ColumnDescription = "创建时间")]
        public DateTime? CreatedTime { get; set; }

        [SugarColumn(ColumnName = "updated_by",ColumnDescription = "更新人",Length = 50)]
        public string? UpdatedBy { get; set; }

        [SugarColumn(ColumnName = "updated_time",ColumnDescription = "更新时间")]
        public DateTime? UpdatedTime { get; set; }
    }
}