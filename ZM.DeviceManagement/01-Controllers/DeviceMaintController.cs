using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Services;


namespace ZM.Device.Controllers
{
    /// <summary>
    /// 保养记录表
    /// </summary>
    [ApiDescriptionSettings("zm.deviceMaint")]
    [Route("zm/deviceMaint")]
    [AllowAnonymous] //匿名访问
    public class DeviceMaintController : ControllerBase
    {
        private readonly ILogger<DeviceMaintController> _logger;
        private readonly DeviceMaintService _deviceMaintService;
        private readonly DeviceManagementService _deviceManagementService;


        public DeviceMaintController(ILogger<DeviceMaintController> logger,
            DeviceMaintService deviceMaintService,
            DeviceManagementService deviceManagementService)
        {
            _logger = logger;
            _deviceMaintService = deviceMaintService;
            _deviceManagementService = deviceManagementService; 
        }


        /////////////////////////////////////新增的额业务/////////////////////////////////////


        /// <summary>
        /// 根据设备ID查询设备信息 + 保养记录分页列表（组合）
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        [HttpGet("getDetailById")]
        public async Task<AjaxResult> GetDetailById([FromQuery] long deviceId)
        {
            if(deviceId <= 0)
                return AjaxResult.Error("参数无效");

            // 查询设备信息（单条）
            var device = await _deviceManagementService.FirstOrDefaultAsync(d => d.Id == deviceId);
            if(device == null)
                return AjaxResult.Error("设备不存在");

            // 查询保养记录分页（默认第1页，10条，可改造为支持传参）
            var pageList = await _deviceMaintService.GetDeviceMaintPagedListById(deviceId);

            return AjaxResult.Success(new
            {
                DeviceInfo = device,
                MaintList = pageList
            });
        }



        /// <summary>
        /// 根据设备ID查询设备信息
        /// </summary>
        /// <param name="id">设备ID</param>
        [HttpGet("getById")]
        public async Task<AjaxResult> GetById([FromQuery] long id)
        {
            if(id <= 0)
                return AjaxResult.Error("参数无效");

            var result = await _deviceManagementService.FirstOrDefaultAsync(d => d.Id == id);
            return result != null ? AjaxResult.Success(result) : AjaxResult.Error("设备不存在");
        }


        /// <summary>
        /// 返回保养列表  <-- 中间表 <-- 设备ID    apifox通过
        /// </summary>
        [HttpGet("listbyid")]
        public async Task<SqlSugarPagedList<DeviceMaintDto>> GetDeviceMaintPagedListById(string par)
        {
            long id =  long.Parse(par);
            return await _deviceMaintService.GetDeviceMaintPagedListById(id);
        }


        /// <summary>
        /// 新增保养记录（并绑定设备）   apifox通过
        /// </summary>
        [HttpPost("add")]
        public async Task<AjaxResult> AddWithRelation([FromBody] DeviceMaintDto dto)
        {
            var success = await _deviceMaintService.AddWithRelationAsync(dto);
            return success ? AjaxResult.Success("新增成功") : AjaxResult.Error("新增失败");
        }

        /// <summary>
        /// 修改保养记录（仅限当天创建的记录）    apifox通过
        /// </summary>
        [HttpPost("edit")]
        public async Task<AjaxResult> EditWithDateLimit([FromBody] DeviceMaintDto dto)
        {
            var result = await _deviceMaintService.UpdateWithDateLimitAsync(dto);
            return result;
        }


        /// <summary>
        /// 删除保养记录（含中间表）
        /// 注意： 使用get方式适用于单个删除最合适
        /// </summary>
        [HttpGet("delete")]
        public async Task<AjaxResult> DeleteWithRelation(  long id)
        {
            var success = await _deviceMaintService.DeleteWithRelationAsync(id);
            return success ? AjaxResult.Success("删除成功") : AjaxResult.Error("删除失败");
        }




        /////////////////////////////////////生成器自动生成的代码/////////////////////////////////////

        /// <summary>
        /// 查询保养记录表列表
        /// </summary>
        [HttpGet("list")]
        public async Task<SqlSugarPagedList<DeviceMaintDto>> GetDeviceMaintPagedList([FromQuery] DeviceMaintDto dto)
        {
            return await _deviceMaintService.GetDtoPagedListAsync(dto);
        }


        /// <summary>
        /// 获取 保养记录表 详细信息
        /// </summary>
        [HttpGet("")]
        [HttpGet("{id}")]
        [AppAuthorize("zM.Device:deviceMaint:query")]
        public async Task<AjaxResult> Get(int id)
        {
            var data = await _deviceMaintService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 新增 保养记录表
        /// </summary>
        //[HttpPost("")]
        //[AppAuthorize("zM.Device:deviceMaint:add")]
        //[TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        //[RuoYi.System.Log(Title = "保养记录表", BusinessType = BusinessType.INSERT)]
        //public async Task<AjaxResult> Add([FromBody] DeviceMaintDto dto)
        //{
        //    var data = await _deviceMaintService.InsertAsync(dto);
        //    return AjaxResult.Success(data);
        //}

        /// <summary>
        /// 修改 保养记录表
        /// </summary>
        [HttpPut("")]
        [AppAuthorize("zM.Device:deviceMaint:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [RuoYi.System.Log(Title = "保养记录表", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] DeviceMaintDto dto)
        {
            var data = await _deviceMaintService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 删除 保养记录表
        /// </summary>
        [HttpDelete("{ids}")]
        [AppAuthorize("zM.Device:deviceMaint:remove")]
        [RuoYi.System.Log(Title = "保养记录表", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceMaintService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }


    }
}