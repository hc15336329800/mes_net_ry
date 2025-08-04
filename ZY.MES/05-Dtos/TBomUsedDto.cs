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
        public string Id { get; set; }
        public string ItemNo { get; set; } = null!;
        public string BomNo { get; set; } = null!;
        public string UseItemNo { get; set; } = null!;
        public decimal UseItemCount { get; set; } = 0.000m;
        public string? UseItemType { get; set; }
        public string? ParentCode { get; set; }
        public string? ItemNos { get; set; }
        public string? UsedId { get; set; }
        public decimal? FixedUsed { get; set; }
        //public string? CreatedBy { get; set; }
        //public DateTime? CreatedTime { get; set; }
        //public string? UpdatedBy { get; set; }
        //public DateTime? UpdatedTime { get; set; }
    }
}
