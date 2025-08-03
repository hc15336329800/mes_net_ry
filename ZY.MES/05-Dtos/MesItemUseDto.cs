using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Data.Dtos;

namespace ZY.MES._05_Dtos
{
    public class MesItemUseDto : BaseDto
    {
        public long Id { get; set; }
        public string ParentCode { get; set; }
        public string ItemCode { get; set; }
        public decimal? Quantity { get; set; }
    }
}
