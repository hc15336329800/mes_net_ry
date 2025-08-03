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
    public class MesItemUse : BaseEntity
    {
        [SugarColumn(ColumnName = "id",ColumnDescription = "主键",IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "item_no",ColumnDescription = "物料编号")]
        public string ItemNo { get; set; } = null!;
        [SugarColumn(ColumnName = "use_item_no",ColumnDescription = "使用物料编号")]
        public string UseItemNo { get; set; } = null!;

        [SugarColumn(ColumnName = "use_item_count",ColumnDescription = "使用数量")]
        public decimal? UseItemCount { get; set; }
    }
}
