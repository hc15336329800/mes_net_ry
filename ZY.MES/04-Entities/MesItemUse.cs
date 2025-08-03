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
    /// 一级BOM表
    /// </summary>
    [SugarTable("mes_item_use","一级BOM表")]
    public class MesItemUse : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "parent_code",ColumnDescription = "父物料编码")]
        public string ParentCode { get; set; }

        [SugarColumn(ColumnName = "item_code",ColumnDescription = "物料编码")]
        public string ItemCode { get; set; }

        [SugarColumn(ColumnName = "quantity",ColumnDescription = "使用数量")]
        public decimal? Quantity { get; set; }
    }
}
