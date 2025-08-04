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
        public string? Id { get; set; }
        public string ItemNo { get; set; } = null!;
        public string UseItemNo { get; set; } = null!;
        public decimal? UseItemCount { get; set; }
    }
}
