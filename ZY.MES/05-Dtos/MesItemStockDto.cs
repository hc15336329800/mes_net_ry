using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Data.Dtos;

namespace ZY.MES._05_Dtos
{
    public class MesItemStockDto : BaseDto
    {
        public long Id { get; set; }
        public string ItemNo { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string? ItemModel { get; set; }
        public decimal? ItemCount { get; set; }
        public string? ItemMeasure { get; set; }
    }
}
