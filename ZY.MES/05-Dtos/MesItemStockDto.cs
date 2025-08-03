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
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string? Spec { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
    }
}
