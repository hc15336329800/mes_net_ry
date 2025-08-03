using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Device.Entities;
using RuoYi.Framework;
using SqlSugar;
using ZM.Device.Entities;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    /// <summary>
    /// 设备管理
    /// </summary>
    [ApiDescriptionSettings("zm.deviceManagement")]
    [Route("zm/deviceManagement")]
    public class DeviceManagementController : ControllerBase
    {
        private readonly ILogger<DeviceManagementController> _logger;
        private readonly DeviceManagementService _deviceManagementService;

        public DeviceManagementController(ILogger<DeviceManagementController> logger,
            DeviceManagementService deviceManagementService)
        {
            _logger = logger;
            _deviceManagementService = deviceManagementService;
        }


        /// <summary>
        /// 查询分页列表
        /// </summary>
        [HttpGet("pagelistById")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetTypePageList([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagementService.GetDtoPagedListAsync(dto);
        }


        /// <summary>
        /// 查询分页列表  ID倒序
        /// </summary>
        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> pagelistById([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagementService.GetDtoPagedListOrderedByMaintenanceCycleIdAsync(dto);
        }


        /// <summary>
        /// 查询分页列表 -- 设备保养
        /// </summary>
        [HttpGet("pagelistByMaint")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetMaintPageList([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagementService.GetDtoPagedListOrderedByMaintenanceCycleAsync(dto);
        }


        /// <summary>
        /// 查询设备分类下的所有设备 （自定义SQL-涉及中间表）
        /// </summary>
        /// <param name="deviceTypeId">设备类型ID</param>
        /// <param name="pageIndex">页码，默认从1开始</param>
        /// <param name="pageSize">页容量，默认10</param>
        [HttpGet("pagelistbytype")]
        public async Task<SqlSugarPagedList<DeviceManagement>> GetPageByDeviceType(
            [FromQuery] long deviceTypeId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 100)
        {
            return await _deviceManagementService.GetPagedListByDeviceTypeAsync(deviceTypeId,pageIndex,pageSize);
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
        /// 根据设备ID列表查询设备信息（设备ID为字符串数组）
        /// </summary>
        /// <param name="Ids">设备ID列表</param>
        /// <returns></returns>
        [HttpPost("listByIds")]
        public async Task<List<DeviceManagementDto>> GetListByDeviceIds([FromBody] List<string>  Ids)
        {
            if(Ids == null || Ids.Count == 0)
            {
                return new List<DeviceManagementDto>();
            }

            // 转换为 long 类型列表
            List<long> longDeviceIds = Ids.Select(id => Convert.ToInt64(id)).ToList();

            return await _deviceManagementService.GetListByDeviceIdsAsync(longDeviceIds);
        }



        /// <summary>
        /// 新增设备（同时新增中间表记录 devicetype_id）
        /// </summary>
        /// <param name="dto">设备信息DTO，其中 deviceType 为前端传来的设备类型ID</param>
        [HttpPost("add")]
        public async Task<AjaxResult> AddDeviceWithType([FromBody] DeviceManagementDto dto)
        {
            bool success = await _deviceManagementService.AddDeviceWithTypeAsync(dto);
            return success ? AjaxResult.Success("新增成功") : AjaxResult.Error("新增失败");
        }


         /// <summary>
        /// 修改设备（同时更新中间表记录，先删除后新增）
        /// </summary>
        /// <param name="dto">设备信息DTO，其中 deviceType 为前端传来的设备类型ID</param>
        [HttpPost("update")]
        public async Task<AjaxResult> UpdateDeviceWithType([FromBody] DeviceManagementDto dto)
        {
            bool success = await _deviceManagementService.UpdateDeviceWithTypeAsync(dto);
            return success ? AjaxResult.Success("更新成功") : AjaxResult.Error("更新失败");
        }


        /// <summary>
        /// 删除设备（同时删除中间表记录）
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        [HttpPost("delete")]
        public async Task<AjaxResult> DeleteDeviceWithType([FromBody] long deviceId)
        {
            bool success = await _deviceManagementService.DeleteDeviceWithTypeAsync(deviceId);
            return success ? AjaxResult.Success("删除成功") : AjaxResult.Error("删除失败");
        }

    }
}
