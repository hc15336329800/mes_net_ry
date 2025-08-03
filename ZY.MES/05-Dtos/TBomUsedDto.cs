using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Data.Dtos;

namespace ZY.MES._05_Dtos
{
    public class TBomUsedDto : BaseDto
    {
        public long Id { get; set; }
        public string ItemNo { get; set; } = null!;
        public string BomNo { get; set; } = null!;   
         public string? ParentCode { get; set; }
        public string UseItemNo { get; set; } = null!;
        public decimal? FixedUsed { get; set; }
        public string? UseItemType { get; set; }
    }
}
