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
    public class MesItemStock : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "item_code",ColumnDescription = "物料编码")]
        public string ItemCode { get; set; }

        [SugarColumn(ColumnName = "item_name",ColumnDescription = "物料名称")]
        public string ItemName { get; set; }

        [SugarColumn(ColumnName = "spec",ColumnDescription = "规格")]
        public string? Spec { get; set; }

        [SugarColumn(ColumnName = "quantity",ColumnDescription = "库存数量")]
        public decimal? Quantity { get; set; }

        [SugarColumn(ColumnName = "unit",ColumnDescription = "计量单位")]
        public string? Unit { get; set; }
    }
}
