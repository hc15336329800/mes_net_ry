using RuoYi.Common.Data;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;

namespace RuoYi.Common.Utils
{
    public class PageUtils
    {


        /// <summary>
        /// 从请求中获取分页参数并构建PageDomain对象  升级版兼容pageIndex字段
        /// </summary>
        /// <returns>包含分页信息的PageDomain对象</returns>
        public static PageDomain GetPageDomain( )
        {
            // 获取当前HTTP请求上下文
            var request = App.HttpContext.Request;

            // 获取页码参数，优先使用pageNum，如果没有则使用pageIndex，默认值为1
            // 注意：pageNum和pageIndex都支持，但pageNum优先级更高
            var pageNum = !string.IsNullOrEmpty(request?.Query["pageNum"])
                ? Convert.ToInt32(request?.Query["pageNum"])
                : (!string.IsNullOrEmpty(request?.Query["pageIndex"])
                    ? Convert.ToInt32(request?.Query["pageIndex"])
                    : 1);

            // 获取每页记录数参数，默认值为10
            var pageSize = !string.IsNullOrEmpty(request?.Query["pageSize"])
                ? Convert.ToInt32(request?.Query["pageSize"])
                : 10;

            // 获取排序字段参数
            var orderByColumn = request?.Query["orderByColumn"].ToString();

            // 获取排序方式参数（asc/desc）
            var isAsc = request?.Query["isAsc"];

            // 构建排序字符串（格式：字段名 排序方式）
            var orderBy = !string.IsNullOrEmpty(orderByColumn)
                ? $"{orderByColumn.ToUnderScoreCase()} {isAsc}"
                : "";

            // 构建并返回PageDomain对象
            return new PageDomain
            {
                // 页码（确保大于0）
                PageNum = pageNum > 0 ? pageNum : 1,
                // 每页记录数（确保大于0）
                PageSize = pageSize > 0 ? pageSize : 10,
                // 原始排序字段
                OrderByColumn = orderByColumn,
                // 转换为大驼峰命名的属性名
                PropertyName = orderByColumn.ToUpperCamelCase(),
                // 完整的排序字符串
                OrderBy = orderBy,
                // 排序方式
                IsAsc = isAsc
            };
        }


        // 原版 备用
        //public static PageDomain GetPageDomain()
        //{
        //    var request = App.HttpContext.Request;
        //    var pageNum = !string.IsNullOrEmpty(request?.Query["pageNum"]) ? Convert.ToInt32(request?.Query["pageNum"]) : 1;
        //    var pageSize = !string.IsNullOrEmpty(request?.Query["pageSize"]) ? Convert.ToInt32(request?.Query["pageSize"]) : 10;
        //    var orderByColumn = request?.Query["orderByColumn"].ToString();
        //    var isAsc = request?.Query["isAsc"];

        //    var orderBy = !string.IsNullOrEmpty(orderByColumn) ? $"{orderByColumn.ToUnderScoreCase()} {isAsc}" : "";

        //    return new PageDomain
        //    {
        //        PageNum = pageNum > 0 ? pageNum : 1,
        //        PageSize = pageSize > 0 ? pageSize : 10,
        //        OrderByColumn = orderByColumn,
        //        PropertyName = orderByColumn.ToUpperCamelCase(),
        //        OrderBy = orderBy,
        //        IsAsc = isAsc
        //    };
        //}
    }
}
