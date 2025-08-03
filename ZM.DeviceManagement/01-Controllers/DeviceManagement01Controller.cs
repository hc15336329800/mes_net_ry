using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Framework;
using SqlSugar;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    /// <summary>
    /// 设备管理01  分页查询/新增/修改/删除模板合集
    /// 注意：这个为直调仓储层写法实例，事务和部门标注不会生效！！
    /// </summary>
    [ApiDescriptionSettings("zm.deviceManagementTest")]
    [Route("zm/deviceManagementTest")]
    [AllowAnonymous] //匿名访问
    public class DeviceManagementController01 : ControllerBase
    {
        private readonly ILogger<DeviceManagementController> _logger;
        private readonly DeviceManagementService _deviceManagementService;
        private readonly DeviceManagement01Repository _deviceManagement01Repository;//直调repo    设备


        public DeviceManagementController01(ILogger<DeviceManagementController> logger,
            DeviceManagement01Repository deviceManagement01Repository,
            DeviceManagementService deviceManagementService)
        {
            _logger = logger;
            _deviceManagementService = deviceManagementService;
            _deviceManagement01Repository = deviceManagement01Repository;

        }


        ////////////////////////// ApiFox软件测试 - GET查询接口参数 //////////////////////////////////
        ///
        /// 接口：http://localhost:5000/zm/deviceManagementTest/pageListByBaseDto
        /// 参数1：pageNum  ： 1   ：索引编号 
        /// 参数2：pageSize ： 10  ：每页大小 
        /// 参数3：params[queryType]  ： "SpecialType"  ： 动态标注，可空则内部默认值
        ///
        ///////////////////////////////////////////////////////////////////////////////////////////////




        /////////////////////////////////////////一、查询模板合集 ///////////////////////////////////////////////

        #region ********** ① 【分页查询接口合集】 **********

        /// <summary>
        /// 🔵 分页查询 - Base封装版（返回实体列表 DeviceManagement）
        /// </summary>
        [HttpGet("pageListByBaseEntity")]
        public async Task<SqlSugarPagedList<DeviceManagement>> PageListByBaseEntity([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetPageByBaseEntityAsync(dto);
        }

        /// <summary>
        /// 🔵 分页查询 - Base封装版（返回DTO列表 DeviceManagementDto）
        /// </summary>
        [HttpGet("pageListByBaseDto")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> PageListByBaseDto([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetPageByBaseDtoAsync(dto);
        }

        /// <summary>
        /// 🔵 分页查询 - Context链式版（返回实体列表 DeviceManagement）
        /// </summary>
        [HttpGet("pageListByContextEntity")]
        public async Task<SqlSugarPagedList<DeviceManagement>> PageListByContextEntity([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetPageByContextEntityAsync(dto);
        }

        /// <summary>
        /// 🔵 分页查询 - Context链式版（返回DTO列表 DeviceManagementDto）
        /// </summary>
        [HttpGet("pageListByContextDto")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> PageListByContextDto([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetPageByContextDtoAsync(dto);
        }

        /// <summary>
        /// 🔵 分页查询 - Ado原生SQL版（返回实体列表 DeviceManagement）
        /// </summary>
        [HttpGet("pageListByAdoEntity")]
        public async Task<SqlSugarPagedList<DeviceManagement>> PageListByAdoEntity([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetPageByAdoEntityAsync(dto);
        }

        /// <summary>
        /// 🔵 分页查询 - Ado原生SQL版（返回DTO列表 DeviceManagementDto）
        /// </summary>
        [HttpGet("pageListByAdoDto")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> PageListByAdoDto([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetPageByAdoDtoAsync(dto);
        }

        #endregion


        #region ********** ② 【不分页查询接口合集】 **********

        /// <summary>
        /// 🔵 不分页查询 - Base封装版（返回实体列表 DeviceManagement）
        /// </summary>
        [HttpGet("listByBaseEntity")]
        public async Task<List<DeviceManagement>> ListByBaseEntity([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetEntityListByBaseAsync(dto);
        }

        /// <summary>
        /// 🔵 不分页查询 - Base封装版（返回DTO列表 DeviceManagementDto）
        /// </summary>
        [HttpGet("listByBaseDto")]
        public async Task<List<DeviceManagementDto>> ListByBaseDto([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetDtoListByBaseAsync(dto);
        }

        /// <summary>
        /// 🔵 不分页查询 - Context链式版（返回实体列表 DeviceManagement）
        /// </summary>
        [HttpGet("listByContextEntity")]
        public async Task<List<DeviceManagement>> ListByContextEntity([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetEntityListByContextAsync(dto);
        }

        /// <summary>
        /// 🔵 不分页查询 - Context链式版（返回DTO列表 DeviceManagementDto）
        /// </summary>
        [HttpGet("listByContextDto")]
        public async Task<List<DeviceManagementDto>> ListByContextDto([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetDtoListByContextAsync(dto);
        }

        /// <summary>
        /// 🔵 不分页查询 - Ado原生SQL版（返回实体列表 DeviceManagement）
        /// </summary>
        [HttpGet("listByAdoEntity")]
        public async Task<List<DeviceManagement>> ListByAdoEntity([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetEntityListByAdoAsync(dto);
        }

        /// <summary>
        /// 🔵 不分页查询 - Ado原生SQL版（返回DTO列表 DeviceManagementDto）
        /// </summary>
        [HttpGet("listByAdoDto")]
        public async Task<List<DeviceManagementDto>> ListByAdoDto([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagement01Repository.GetDtoListByAdoAsync(dto);
        }

        #endregion



        /////////////////////////////////////////二、新增模板合集 ///////////////////////////////////////////////

        #region ********** ① 【单条新增接口合集】 **********

        /// <summary>
        /// 🔵 单条新增 - Base封装版
        /// </summary>
        [HttpPost("addByBase")]
        public async Task<AjaxResult> AddDeviceByBase([FromBody] DeviceManagementDto dto)
        {
            var success = await _deviceManagement01Repository.AddDeviceByBaseAsync(dto);
            return success ? AjaxResult.Success("新增成功") : AjaxResult.Error("新增失败");
        }

        /// <summary>
        /// 🔵 单条新增 - Context链式版
        /// </summary>
        [HttpPost("addByContext")]
        public async Task<AjaxResult> AddDeviceByContext([FromBody] DeviceManagementDto dto)
        {
            var success = await _deviceManagement01Repository.AddDeviceByContextAsync(dto);
            return success ? AjaxResult.Success("新增成功") : AjaxResult.Error("新增失败");
        }

        /// <summary>
        /// 🔵 单条新增 - Ado原生SQL版
        /// </summary>
        [HttpPost("addByAdo")]
        public async Task<AjaxResult> AddDeviceByAdo([FromBody] DeviceManagementDto dto)
        {
            var success = await _deviceManagement01Repository.AddDeviceByAdoAsync(dto);
            return success ? AjaxResult.Success("新增成功") : AjaxResult.Error("新增失败");
        }

        #endregion


        #region ********** ② 【批量新增接口合集】 **********

        /// <summary>
        /// 🔵 批量新增 - Base封装版
        /// </summary>
        [HttpPost("addBatchByBase")]
        public async Task<AjaxResult> AddDevicesByBase([FromBody] List<DeviceManagementDto> dtoList)
        {
            var success = await _deviceManagement01Repository.AddDevicesByBaseAsync(dtoList);
            return success ? AjaxResult.Success("批量新增成功") : AjaxResult.Error("批量新增失败");
        }

        /// <summary>
        /// 🔵 批量新增 - Context链式版
        /// </summary>
        [HttpPost("addBatchByContext")]
        public async Task<AjaxResult> AddDevicesByContext([FromBody] List<DeviceManagementDto> dtoList)
        {
            var success = await _deviceManagement01Repository.AddDevicesByContextAsync(dtoList);
            return success ? AjaxResult.Success("批量新增成功") : AjaxResult.Error("批量新增失败");
        }

        /// <summary>
        /// 🔵 批量新增 - Ado原生SQL版
        /// </summary>
        [HttpPost("addBatchByAdo")]
        public async Task<AjaxResult> AddDevicesByAdo([FromBody] List<DeviceManagementDto> dtoList)
        {
            var success = await _deviceManagement01Repository.AddDevicesByAdoAsync(dtoList);
            return success ? AjaxResult.Success("批量新增成功") : AjaxResult.Error("批量新增失败");
        }

        #endregion



        /////////////////////////////////////////三、修改模板合集 ///////////////////////////////////////////////

        #region ********** ⑤ 【单条修改接口合集】 **********

        /// <summary>
        /// 🔵 单条修改 - Base封装版
        /// </summary>
        [HttpPost("updateByBase")]
        public async Task<AjaxResult> UpdateDeviceByBase([FromBody] DeviceManagementDto dto)
        {
           var  success = await _deviceManagement01Repository.UpdateDeviceByBaseAsync(dto);
            return success  ? AjaxResult.Success("修改成功") : AjaxResult.Error("修改失败");

        }

        /// <summary>
        /// 🔵 单条修改 - Context链式版
        /// </summary>
        [HttpPost("updateByContext")]
        public async Task<AjaxResult> UpdateDeviceByContext([FromBody] DeviceManagementDto dto)
        {
            var success = await _deviceManagement01Repository.UpdateDeviceByContextAsync(dto);
            return success ? AjaxResult.Success("修改成功") : AjaxResult.Error("修改失败");

        }

        /// <summary>
        /// 🔵 单条修改 - Ado原生SQL版
        /// </summary>
        [HttpPost("updateByAdo")]
        public async Task<AjaxResult> UpdateDeviceByAdo([FromBody] DeviceManagementDto dto)
        {
            var success = await _deviceManagement01Repository.UpdateDeviceByAdoAsync(dto);
            return success ? AjaxResult.Success("修改成功") : AjaxResult.Error("修改失败");

        }

        #endregion

        #region ********** ⑥ 【批量修改接口合集】 **********

        /// <summary>
        /// 🔵 批量修改 - Base封装版
        /// </summary>
        [HttpPost("updateBatchByBase")]
        public async Task<bool> UpdateDevicesByBase([FromBody] List<DeviceManagementDto> dtoList)
        {
            return await _deviceManagement01Repository.UpdateDevicesByBaseAsync(dtoList);
        }

        /// <summary>
        /// 🔵 批量修改 - Context链式版
        /// </summary>
        [HttpPost("updateBatchByContext")]
        public async Task<bool> UpdateDevicesByContext([FromBody] List<DeviceManagementDto> dtoList)
        {
            return await _deviceManagement01Repository.UpdateDevicesByContextAsync(dtoList);
        }

        /// <summary>
        /// 🔵 批量修改 - Ado原生SQL版
        /// </summary>
        [HttpPost("updateBatchByAdo")]
        public async Task<bool> UpdateDevicesByAdo([FromBody] List<DeviceManagementDto> dtoList)
        {
            return await _deviceManagement01Repository.UpdateDevicesByAdoAsync(dtoList);
        }

        #endregion


        /////////////////////////////////////////四、删除模板合集 ///////////////////////////////////////////////

        #region ********** ① 【单条删除接口合集】 **********

        /// <summary>
        /// 🔵 单条删除 - Base封装版
        /// </summary>
        [HttpGet("deleteByBase")]
        public async Task<AjaxResult> DeleteDeviceByBase([FromQuery] long id)
        {
            var success = await _deviceManagement01Repository.DeleteDeviceByBaseAsync(id);
            return success ? AjaxResult.Success("删除成功") : AjaxResult.Error("删除失败");
        }

        /// <summary>
        /// 🔵 单条删除 - Context链式版
        /// </summary>
        [HttpGet("deleteByContext")]
        public async Task<AjaxResult> DeleteDeviceByContext([FromQuery] long id)
        {
            var success = await _deviceManagement01Repository.DeleteDeviceByContextAsync(id);
            return success ? AjaxResult.Success("删除成功") : AjaxResult.Error("删除失败");
        }

        /// <summary>
        /// 🔵 单条删除 - Ado原生SQL版
        /// </summary>
        [HttpGet("deleteByAdo")]
        public async Task<AjaxResult> DeleteDeviceByAdo([FromQuery] long id)
        {
            var success = await _deviceManagement01Repository.DeleteDeviceByAdoAsync(id);
            return success ? AjaxResult.Success("删除成功") : AjaxResult.Error("删除失败");
        }

        #endregion

        #region ********** ② 【批量删除接口合集】 **********

        /// <summary>
        /// 🔵 批量删除 - Base封装版
        /// </summary>
        [HttpGet("deleteBatchByBase")]
        public async Task<AjaxResult> DeleteDevicesByBase([FromQuery] List<long> ids)
        {
            var success = await _deviceManagement01Repository.DeleteDevicesByBaseAsync(ids);
            return success ? AjaxResult.Success("批量删除成功") : AjaxResult.Error("批量删除失败");
        }

        /// <summary>
        /// 🔵 批量删除 - Context链式版
        /// </summary>
        [HttpGet("deleteBatchByContext")]
        public async Task<AjaxResult> DeleteDevicesByContext([FromQuery] List<long> ids)
        {
            var success = await _deviceManagement01Repository.DeleteDevicesByContextAsync(ids);
            return success ? AjaxResult.Success("批量删除成功") : AjaxResult.Error("批量删除失败");
        }

        /// <summary>
        /// 🔵 批量删除 - Ado原生SQL版
        /// </summary>
        [HttpGet("deleteBatchByAdo")]
        public async Task<AjaxResult> DeleteDevicesByAdo([FromQuery] List<long> ids)
        {
            var success = await _deviceManagement01Repository.DeleteDevicesByAdoAsync(ids);
            return success ? AjaxResult.Success("批量删除成功") : AjaxResult.Error("批量删除失败");
        }

        #endregion







        /////////////////////////////////////////原来的代码  无需改动///////////////////////////////////////////////


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
