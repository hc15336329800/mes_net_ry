using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Utils;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    /// <summary>
    /// 设备缺陷记录表
    /// </summary>
    [ApiDescriptionSettings("zm/defectRecord")]
    [Route("zm/defectRecord")]
    [AllowAnonymous] //匿名访问
    public class DeviceDefectRecordController : ControllerBase
    {
        private readonly ILogger<DeviceDefectRecordController> _logger;
        private readonly DeviceDefectRecordService _deviceDefectRecordService;
                
        public DeviceDefectRecordController(ILogger<DeviceDefectRecordController> logger,
            DeviceDefectRecordService deviceDefectRecordService)
        {
            _logger = logger;
            _deviceDefectRecordService = deviceDefectRecordService;
        }

        /// <summary>
        /// 查询设备缺陷记录表列表
        /// </summary>
        [HttpGet("list")]
        //[AppAuthorize("device:record:list")]
        public async Task<SqlSugarPagedList<DeviceDefectRecordDto>> GetDeviceDefectRecordPagedList([FromQuery] DeviceDefectRecordDto dto)
        {
           return await _deviceDefectRecordService.GetDtoPagedListAsync(dto);
        }

        /// <summary>
        /// 获取 设备缺陷记录表 详细信息
        /// </summary>
        [HttpGet("info/{id}")]
        //[AppAuthorize("device:record:query")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _deviceDefectRecordService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 新增 设备缺陷记录表
        /// </summary>
        [HttpPost("add")]
        //[AppAuthorize("device:record:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        //[RuoYi.System.Log(Title = "设备缺陷记录表", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] DeviceDefectRecordDto dto)
        {
            var data = await _deviceDefectRecordService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 修改 设备缺陷记录表
        /// </summary>
        [HttpPost("update")]
        //[AppAuthorize("device:record:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        //[RuoYi.System.Log(Title = "设备缺陷记录表", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] DeviceDefectRecordDto dto)
        {
            var data = await _deviceDefectRecordService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 删除 设备缺陷记录表
        /// </summary>
        [HttpPost("delete/{ids}")]
        //[AppAuthorize("device:record:remove")]
        //[RuoYi.System.Log(Title = "设备缺陷记录表", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceDefectRecordService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 导入 设备缺陷记录表
        /// </summary>
        [HttpPost("import")]
        [AppAuthorize("device:record:import")]
        //[RuoYi.System.Log(Title = "设备缺陷记录表", BusinessType = BusinessType.IMPORT)]
        public async Task Import([Required] IFormFile file)
        {
            var stream = new MemoryStream();
            file.CopyTo(stream);
            var list = await ExcelUtils.ImportAsync<DeviceDefectRecordDto>(stream);
            await _deviceDefectRecordService.ImportDtoBatchAsync(list);
        }

        /// <summary>
        /// 导出 设备缺陷记录表
        /// </summary>
        [HttpPost("export")]
        [AppAuthorize("device:record:export")]
        //[RuoYi.System.Log(Title = "设备缺陷记录表", BusinessType = BusinessType.EXPORT)]
        public async Task Export(DeviceDefectRecordDto dto)
        {
            var list = await _deviceDefectRecordService.GetDtoListAsync(dto);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        }
    }
}