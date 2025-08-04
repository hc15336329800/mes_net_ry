using System;
using RuoYi.Data.Dtos;

namespace ZY.MES._05_Dtos
{
    /// <summary>
    /// 物料库存数据传输对象
    /// </summary>
    public class MesItemStockDto : BaseDto
    {
        public long Id { get; set; }

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
        /// 最小库存量
        /// </summary>
        public decimal? MinStock { get; set; }

        /// <summary>
        /// 最大库存量
        /// </summary>
        public decimal? MaxStock { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public long? WarehouseId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string? WarehouseName { get; set; }

        /// <summary>
        /// 库位编码
        /// </summary>
        public string? LocationCode { get; set; }

        /// <summary>
        /// 物料分类
        /// </summary>
        public string? ItemCategory { get; set; }

        /// <summary>
        /// 物料状态(0-正常,1-停用)
        /// </summary>
        public string? ItemStatus { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}