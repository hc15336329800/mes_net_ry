using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.MES._05_Dtos
{
    /// <summary>
    /// 物料用料树响应
    /// </summary>
    public class UseItemTreeResp
    {
        /// <summary>
        /// 物料编号
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string? ItemName { get; set; }

        /// <summary>
        /// 父物料编号
        /// </summary>
        public string? ParentCode { get; set; }

        /// <summary>
        /// 用料关系主键
        /// </summary>
        public long? UsedId { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<UseItemTreeResp> Children { get; set; } = new();

        /// <summary>
        /// 固定用量
        /// </summary>
        public decimal? FixedUsed { get; set; }

        /// <summary>
        /// BOM 编号
        /// </summary>
        public string? BomNo { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public string? ItemType { get; set; }
    }
}
