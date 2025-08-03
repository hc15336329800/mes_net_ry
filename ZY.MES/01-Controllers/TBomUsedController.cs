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
    /// 完整BOM接口
    /// </summary>
    [ApiDescriptionSettings("zy/mes/bomUsed")]
    [Route("zy/mes/bomUsed")]
    public class TBomUsedController : ControllerBase
    {
        private readonly ILogger<TBomUsedController> _logger;
        private readonly TBomUsedService _service;

        public TBomUsedController(ILogger<TBomUsedController> logger,TBomUsedService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<TBomUsedDto>> GetPageList([FromQuery] TBomUsedDto dto)
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
    }
}
