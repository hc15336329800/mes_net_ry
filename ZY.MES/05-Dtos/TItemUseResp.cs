using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Data.Dtos;

namespace ZY.MES._05_Dtos
{
    public class TItemUseResp : BaseDto
    {
        public string? Id { get; set; }
        public string ItemNo { get; set; } = null!;
        public string UseItemNo { get; set; } = null!;
        public decimal? UseItemCount { get; set; }
        /// <summary>
        /// 用料类型,如01表示装配件
        /// </summary>
        public string? UseItemType { get; set; }
    }
}
