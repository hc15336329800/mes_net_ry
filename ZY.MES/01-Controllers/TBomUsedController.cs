using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Framework;
using SqlSugar;
using ZY.MES._02_Services;
using ZY.MES._05_Dtos;

namespace ZY.MES._01_Controllers
{
    /// <summary>
    /// 完整BOM接口
    /// </summary>
    [ApiDescriptionSettings("zy/mes/bomUsed")]
    [Route("zy/mes/bomUsed")]
    [AllowAnonymous] //匿名访问
    public class TBomUsedController : ControllerBase
    {
        private readonly ILogger<TBomUsedController> _logger;
        private readonly TBomUsedService _service;

        public TBomUsedController(ILogger<TBomUsedController> logger,TBomUsedService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 构建物料用料树
        /// </summary>
        /// <param name="bomNo">BOM编号</param>
        [HttpGet("item_use_tree")]
        public async Task<AjaxResult> ItemUseTree([FromQuery] string bomNo)
        {
            var data = await _service.GetItemUseTreeAsync(bomNo);
            return AjaxResult.Success(data);
        }


 
        /// 重新构建完整BOM
        /// </summary>
        /// <param name="uses">前端传入的一级用料数组</param>
        /// <remarks>
        /// 请求示例:
        /// <code>
        /// POST /zy/mes/bomUsed/load
        /// [
        ///   { "id": 1, "itemNo": "A", "useItemNo": "B", "useItemCount": 2, "useItemType": "01" },
        ///   { "id": 2, "itemNo": "B", "useItemNo": "C", "useItemCount": 1, "useItemType": "00" }
        /// ]
        /// </code>
        /// </remarks>
        [HttpPost("load")]
        public async Task<AjaxResult> LoadBom([FromBody] List<MesItemUseDto> uses)
        {
            await _service.LoadBomDataAsync(uses);
            return AjaxResult.Success();
        }


        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<TBomUsedDto>> GetPageList([FromQuery] TBomUsedDto dto)
        {
            return await _service.GetDtoPagedListAsync(dto);
        }

        [HttpGet("{id}")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _service.FirstOrDefaultAsync(x => x.Id == id.ToString());
            return data != null ? AjaxResult.Success(data) : AjaxResult.Error("not found");
        }

        [HttpPost("add")]
        public async Task<AjaxResult> Add([FromBody] TBomUsedDto dto)
        {
            bool ok = await _service.InsertAsync(dto);
            return ok ? AjaxResult.Success() : AjaxResult.Error();
        }

        [HttpPost("update")]
        public async Task<AjaxResult> Update([FromBody] TBomUsedDto dto)
        {
            int rows = await _service.UpdateAsync(dto);
            return rows > 0 ? AjaxResult.Success() : AjaxResult.Error();
        }

        [HttpPost("delete")]
        public async Task<AjaxResult> Delete([FromBody] long id)
        {
            int rows = await _service.DeleteAsync(id);
            return rows > 0 ? AjaxResult.Success() : AjaxResult.Error();
        }

        /// <summary>
        /// 删除指定节点及其所有子节点
        /// </summary>
        /// <param name="id">待删除节点ID</param>
        [HttpPost("delete_bom")]
        public async Task<AjaxResult> DeleteBom([FromBody] string id)
        {
            int rows = await _service.DeleteBomAsync(id);
            return rows > 0 ? AjaxResult.Success() : AjaxResult.Error();
        }
    }
}
