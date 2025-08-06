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
    /// 物料接口
    /// </summary>
    [ApiDescriptionSettings("zy/mes/itemStock")]
    [Route("zy/mes/itemStock")]
    [AllowAnonymous] //匿名访问

    public class TItemStockController : ControllerBase
    {
        private readonly ILogger<TItemStockController> _logger;
        private readonly TItemStockService _service;

        public TItemStockController(ILogger<TItemStockController> logger,TItemStockService service)
        {
            _logger = logger;
            _service = service;
        }


        /// <summary>
        /// 查询bom
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<TItemStockDto>> GetPageList(
            [FromQuery] TItemStockDto dto,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 10)
        {
            // pageNum and pageSize are captured via PageUtils in service layer

            dto.ItemType = "01";//bom类型
            return await _service.GetDtoPagedListAsync(dto);
        }


        /// <summary>
        /// 查询物料
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("pagelistmaterial")]
        public async Task<SqlSugarPagedList<TItemStockDto>> GetPageListMaterial(
          [FromQuery] TItemStockDto dto,
          [FromQuery] int pageNum = 1,
          [FromQuery] int pageSize = 10)
        {

            dto.ItemType = "00";//物料类型
            return await _service.GetDtoPagedListAsync(dto);
        }




        /// <summary>
        /// 查询bom和物料
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("pagelistall")]
        public async Task<SqlSugarPagedList<TItemStockDto>> GetPageListAll(
            [FromQuery] TItemStockDto dto,
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 10)
        {
            // pageNum and pageSize are captured via PageUtils in service layer
            dto.ItemType = "03";//物料类型

            return await _service.GetDtoPagedListAsync(dto);
        }



        [HttpGet("{id}")]
        public async Task<AjaxResult> Get(String id)
        {
            var data = await _service.GetAsync(id);
            return data != null ? AjaxResult.Success(data) : AjaxResult.Error("not found");
        }

        [HttpPost("add")]
        public async Task<AjaxResult> Add([FromBody] TItemStockDto dto)
        {
            bool ok = await _service.InsertAsync(dto);
            return ok ? AjaxResult.Success() : AjaxResult.Error();
        }

        [HttpPost("update")]
        public async Task<AjaxResult> Update([FromBody] TItemStockDto dto)
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
    }
}
