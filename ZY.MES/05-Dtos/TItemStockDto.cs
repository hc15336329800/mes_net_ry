using System;
using RuoYi.Data.Dtos;

namespace ZY.MES._05_Dtos
{
    /// <summary>
    /// 物料库存数据传输对象
    /// </summary>
    /// <summary>
    /// 物料库存表 数据传输对象 (DTO)
    /// </summary>
    public class TItemStockDto : BaseDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public String Id { get; set; } = null!;

        /// <summary>
        /// 物料编号
        /// </summary>
        public string ItemNo { get; set; } = null!;

        /// <summary>
        /// 物料名称
        /// </summary>
        public string ItemName { get; set; } = null!;

        /// <summary>
        /// 规格型号
        /// </summary>
        public string? ItemModel { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal? ItemCount { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string? ItemMeasure { get; set; }

        /// <summary>
        /// 辅助库存数量
        /// </summary>
        public decimal? ItemCountAssist { get; set; }

        /// <summary>
        /// 辅助计量单位
        /// </summary>
        public string? ItemMeasureAssist { get; set; }

        /// <summary>
        /// BOM编号
        /// </summary>
        public string? BomNo { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public string? ItemType { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// ERP库存数量
        /// </summary>
        public decimal? ErpCount { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// 有效标识
        /// </summary>
        public string? IsValid { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        public int? UniId { get; set; }




        /// <summary>
        /// 创建人
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedTime { get; set; }
    }
}