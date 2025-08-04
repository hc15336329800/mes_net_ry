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

    public class MesItemStockController : ControllerBase
    {
        private readonly ILogger<MesItemStockController> _logger;
        private readonly MesItemStockService _service;

        public MesItemStockController(ILogger<MesItemStockController> logger,MesItemStockService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<MesItemStockDto>> GetPageList([FromQuery] MesItemStockDto dto)
        {
            return await _service.GetDtoPagedListAsync(dto);
        }

        [HttpGet("{id}")]
        public async Task<AjaxResult> Get(String id)
        {
            var data = await _service.GetAsync(id);
            return data != null ? AjaxResult.Success(data) : AjaxResult.Error("not found");
        }

        [HttpPost("add")]
        public async Task<AjaxResult> Add([FromBody] MesItemStockDto dto)
        {
            bool ok = await _service.InsertAsync(dto);
            return ok ? AjaxResult.Success() : AjaxResult.Error();
        }

        [HttpPost("update")]
        public async Task<AjaxResult> Update([FromBody] MesItemStockDto dto)
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
