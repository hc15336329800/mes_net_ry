using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using ZM.Device.Services;
using ZM.Device.Tool;

namespace ZM.Device.Controllers
{
    /// <summary>
    /// 巡检记录表
    /// </summary>
    [ApiDescriptionSettings("zm/record")]
    [Route("zm/record")]
    [AllowAnonymous] //匿名访问
    public class DeviceInspectionRecordController : ControllerBase
    {
        private readonly ILogger<DeviceInspectionRecordController> _logger;
        private readonly DeviceInspectionRecordService _deviceInspectionRecordService;
        private readonly DeviceInspectionRecordRepository _deviceInspectionRecordRepository; 
        private readonly DeviceInspectionTaskRepository _deviceInspectionTaskRepository;//直调SQL
        private readonly DeviceManagementRepository _deviceManagementRepository;//直调SQL




        public DeviceInspectionRecordController(ILogger<DeviceInspectionRecordController> logger,
            DeviceInspectionRecordService deviceInspectionRecordService,
             DeviceInspectionRecordRepository deviceInspectionRecordRepository,
             DeviceInspectionTaskRepository deviceInspectionTaskRepository,
DeviceManagementRepository deviceManagementRepository


             ) 
        {
            _logger = logger;
            _deviceInspectionRecordService = deviceInspectionRecordService;
            _deviceInspectionRecordRepository = deviceInspectionRecordRepository;
            _deviceInspectionTaskRepository = deviceInspectionTaskRepository;
            _deviceManagementRepository = deviceManagementRepository;


        }
 

        /// <summary>
        /// 查询巡检记录表列表
        /// </summary>
        [HttpGet("list")]
        //[AppAuthorize("device:record:list")]
        public async Task<SqlSugarPagedList<DeviceInspectionRecordDto>> GetDeviceInspectionRecordPagedList([FromQuery] DeviceInspectionRecordDto dto)
        {
           return await _deviceInspectionRecordService.GetDtoPagedListAsync(dto);
        }

        /// <summary>
        /// 获取 巡检记录表 详细信息
        /// </summary>
        [HttpGet("info/{id}")]
        //[AppAuthorize("device:record:query")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _deviceInspectionRecordService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }



        /// <summary>
        /// 获取指定巡检记录的设备列表（InspectedDeviceIds）+ 已完成设备ID列表（FinishedDeviceIds）
        /// </summary>
        [HttpGet("getDeviceIds/{id}")]
        public async Task<AjaxResult> GetDeviceIds(long id)
        {
           

            // 查巡检记录
            var record = await _deviceInspectionRecordRepository.Repo.AsQueryable()
                .Where(r => r.Id == id)
                .Select(r => new { r.FinishedDeviceIds,r.InspectedDeviceIds,r.TaskId })
                .FirstAsync();

            if(record == null)
            {
                return AjaxResult.Error("未找到对应巡检记录");
            }



            string tataskId = record.TaskId.ToString(); //任务id
            List<string> finishedDeviceIds = new();
            List<string> inspectedDeviceIds = new();

            try
            {
                // 解析 FinishedDeviceIds（Json数组对象 List<Dict> 格式）
                if(!string.IsNullOrWhiteSpace(record.FinishedDeviceIds))
                {
                    var finishedList = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(record.FinishedDeviceIds);
                    if(finishedList != null)
                    {
                        finishedDeviceIds = finishedList.SelectMany(d => d.Keys).ToList();
                    }
                }

                // 解析 InspectedDeviceIds（纯字符串数组 List<string>）
                if(!string.IsNullOrWhiteSpace(record.InspectedDeviceIds))
                {
                    inspectedDeviceIds = JsonConvert.DeserializeObject<List<string>>(record.InspectedDeviceIds) ?? new List<string>();
                }
            }
            catch(Exception ex)
            {
                return AjaxResult.Error("设备ID解析失败: " + ex.Message);
            }

            // 根据 InspectedDeviceIds 查设备表
            List<DeviceManagement> devices = new();

            if(inspectedDeviceIds.Any())
            {
                devices = await _deviceManagementRepository.Repo.AsQueryable()
                    //.In(inspectedDeviceIds.Select(id => long.Parse(id)).ToArray(),d => d.Id)
                    .In(d => d.Id,inspectedDeviceIds.Select(id => long.Parse(id)).ToArray())

                    .Select(d => new DeviceManagement
                    {
                        //taskId = d.TaskId, // 任务ID
                        Id = d.Id, //设备ID
                        Label = d.Label, //设备名称
   
                        // 按需选择要返回的字段，避免一次性查太多
                    })
                    .ToListAsync();
            }

            var result = new
            {
                TataskId = tataskId,
                Devices = devices,
                FinishedDeviceIds = finishedDeviceIds
            };

            return AjaxResult.Success(result);
        }





        /// <summary>
        /// 新增 巡检记录表
        /// </summary>
        [HttpPost("add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        //[RuoYi.System.Log(Title = "巡检记录表", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] DeviceInspectionRecordDto dto)
        {
            dto.Id = NextId.Id13(); // 手动设置id，调试有值！

            var data = await _deviceInspectionRecordService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 修改 巡检记录表
        /// </summary>
        [HttpPost("update")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        //[RuoYi.System.Log(Title = "巡检记录表", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] DeviceInspectionRecordDto dto)
        {
            var data = await _deviceInspectionRecordService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 删除 巡检记录表
        /// </summary>
        [HttpPost("delete/{ids}")]
        //[AppAuthorize("device:record:remove")]
        //[RuoYi.System.Log(Title = "巡检记录表", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceInspectionRecordService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }

        ///// <summary>
        ///// 导入 巡检记录表
        ///// </summary>
        //[HttpPost("import")]
        //[AppAuthorize("device:record:import")]
        //[RuoYi.System.Log(Title = "巡检记录表", BusinessType = BusinessType.IMPORT)]
        //public async Task Import([Required] IFormFile file)
        //{
        //    var stream = new MemoryStream();
        //    file.CopyTo(stream);
        //    var list = await ExcelUtils.ImportAsync<DeviceInspectionRecordDto>(stream);
        //    await _deviceInspectionRecordService.ImportDtoBatchAsync(list);
        //}

        ///// <summary>
        ///// 导出 巡检记录表
        ///// </summary>
        //[HttpPost("export")]
        //[AppAuthorize("device:record:export")]
        //[RuoYi.System.Log(Title = "巡检记录表", BusinessType = BusinessType.EXPORT)]
        //public async Task Export(DeviceInspectionRecordDto dto)
        //{
        //    var list = await _deviceInspectionRecordService.GetDtoListAsync(dto);
        //    await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        //}
    }
}