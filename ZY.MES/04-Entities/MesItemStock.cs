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
    /// 物料表
    /// </summary>
    [SugarTable("mes_item_stock","物料表")]
    public class MesItemStock : BaseEntity
    {
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "item_no",ColumnDescription = "物料编号")]
        public string ItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "item_name",ColumnDescription = "物料名称")]
        public string ItemName { get; set; } = null!;

        [SugarColumn(ColumnName = "item_model",ColumnDescription = "规格型号")]
        public string? ItemModel { get; set; }

        [SugarColumn(ColumnName = "item_count",ColumnDescription = "库存数量")]
        public decimal? ItemCount { get; set; }

        [SugarColumn(ColumnName = "item_measure",ColumnDescription = "计量单位")]
        public string? ItemMeasure { get; set; }
    }
}
