using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Framework;
using SqlSugar;
using ZY.MES._02_Services;
using ZY.MES._05_Dtos;

namespace ZY.MES._01_Controllers
{
    /// <summary>
    /// 一级BOM接口
    /// </summary>
    [ApiDescriptionSettings("zy/mes/itemUse")]
    [Route("zy/mes/itemUse")]
    public class MesItemUseController : ControllerBase
    {
        private readonly ILogger<MesItemUseController> _logger;
        private readonly MesItemUseService _service;

        public MesItemUseController(ILogger<MesItemUseController> logger,MesItemUseService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<MesItemUseDto>> GetPageList([FromQuery] MesItemUseDto dto)
        {
            return await _service.GetDtoPagedListAsync(dto);
        }

        [HttpGet("{id}")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _service.FirstOrDefaultAsync(x => x.Id == id);
            return data != null ? AjaxResult.Success(data) : AjaxResult.Error("not found");
        }

        [HttpPost("add")]
        public async Task<AjaxResult> Add([FromBody] MesItemUseDto dto)
        {
            bool ok = await _service.InsertAsync(dto);
            return ok ? AjaxResult.Success() : AjaxResult.Error();
        }

        [HttpPost("update")]
        public async Task<AjaxResult> Update([FromBody] MesItemUseDto dto)
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
