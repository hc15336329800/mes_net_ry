using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ZM.Device.Services;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Tool;

namespace ZM.Device.Controllers
{
    /// <summary>
    /// 设备巡检操作票表   -- 典型增删改
    /// </summary>
    [ApiDescriptionSettings("zm/deviceInspectionTicketController")]
    [Route("zm/ticket")]
    [AllowAnonymous] //匿名访问

    public class DeviceInspectionTicketController : ControllerBase
    {
        private readonly ILogger<DeviceInspectionTicketController> _logger;
        private readonly DeviceInspectionTicketService _deviceInspectionTicketService;
                
        public DeviceInspectionTicketController(ILogger<DeviceInspectionTicketController> logger,
            DeviceInspectionTicketService deviceInspectionTicketService)
        {
            _logger = logger;
            _deviceInspectionTicketService = deviceInspectionTicketService;
        }

        /// <summary>
        /// 查询设备巡检操作票表列表
        /// </summary>
        [HttpGet("list")]
        //[AppAuthorize("system:ticket:list")]
        public async Task<SqlSugarPagedList<DeviceInspectionTicketDto>> GetDeviceInspectionTicketPagedList([FromQuery] DeviceInspectionTicketDto dto)
        {
           return await _deviceInspectionTicketService.GetDtoPagedListAsync(dto);
        }

        /// <summary>
        /// 获取 设备巡检操作票表 详细信息
        /// </summary>
        [HttpGet("info/{id}")]
        //[AppAuthorize("system:ticket:query")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _deviceInspectionTicketService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 新增 设备巡检操作票表
        /// </summary>
        [HttpPost("add")]
        //[AppAuthorize("system:ticket:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]  //需要登录  先注释
        //[RuoYi.System.Log(Title = "设备巡检操作票表", BusinessType = BusinessType.INSERT)]//需要登录  先注释
        public async Task<AjaxResult> Add([FromBody] DeviceInspectionTicketDto dto)
        {
            if(dto == null)
            {
                return AjaxResult.Error("新增失败");
            }

            dto.Id  = NextId.Id13(); // 手动设置id，调试有值！
            var data = await _deviceInspectionTicketService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 修改 设备巡检操作票表
        /// </summary>
        [HttpPost("update")]
        //[AppAuthorize("system:ticket:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        //[RuoYi.System.Log(Title = "设备巡检操作票表", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] DeviceInspectionTicketDto dto)
        {
            var data = await _deviceInspectionTicketService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 删除 设备巡检操作票表
        /// </summary>
        [HttpPost("delete/{ids}")]
        //[AppAuthorize("system:ticket:remove")]
        //[RuoYi.System.Log(Title = "设备巡检操作票表", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceInspectionTicketService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }

        ///// <summary>
        ///// 导入 设备巡检操作票表
        ///// </summary>
        //[HttpPost("import")]
        ////[AppAuthorize("system:ticket:import")]
        //[RuoYi.System.Log(Title = "设备巡检操作票表", BusinessType = BusinessType.IMPORT)]
        //public async Task Import([Required] IFormFile file)
        //{
        //    var stream = new MemoryStream();
        //    file.CopyTo(stream);
        //    var list = await ExcelUtils.ImportAsync<DeviceInspectionTicketDto>(stream);
        //    await _deviceInspectionTicketService.ImportDtoBatchAsync(list);
        //}

        ///// <summary>
        ///// 导出 设备巡检操作票表
        ///// </summary>
        //[HttpPost("export")]
        ////[AppAuthorize("system:ticket:export")]
        //[RuoYi.System.Log(Title = "设备巡检操作票表", BusinessType = BusinessType.EXPORT)]
        //public async Task Export(DeviceInspectionTicketDto dto)
        //{
        //    var list = await _deviceInspectionTicketService.GetDtoListAsync(dto);
        //    await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        //}
    }
}