using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuoYi.Common.Utils;
using RuoYi.Framework;
using SqlSugar;
using ZM.Device._05_Dtos;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using ZM.Device.Services;


namespace ZM.Device.Controllers
{
    /// <summary>
    /// 巡检任务 控制器
    /// </summary>
    [ApiDescriptionSettings("zm/deviceInspectionTask")]
    [Route("zm/deviceInspectionTask")]
    [AllowAnonymous] //匿名访问
    public class DeviceInspectionTaskController : ControllerBase
    {
        private readonly ILogger<DeviceInspectionTaskController> _logger;
        private readonly DeviceInspectionTaskService _service;

        private readonly DeviceInspectionTaskRepository _deviceInspectionTaskRepository;//直调SQL   任务
        private readonly DeviceManagementRepository _deviceManagementRepository;//直调SQL    设备
        private readonly DeviceInspectionRecordRepository _deviceInspectionRecordRepository;//直调SQL    巡检记录
        private readonly DeviceDefectRecordRepository _deviceDefectRecordRepository;//直调SQL    缺陷记录
 




        public DeviceInspectionTaskController(ILogger<DeviceInspectionTaskController> logger,DeviceInspectionTaskService service,DeviceInspectionTaskRepository deviceInspectionTaskRepository,
DeviceManagementRepository deviceManagementRepository,DeviceInspectionRecordRepository deviceInspectionRecordRepository,DeviceDefectRecordRepository deviceDefectRecordRepository)
        {
            _logger = logger;
            _service = service;
            _deviceInspectionTaskRepository = deviceInspectionTaskRepository;
            _deviceManagementRepository = deviceManagementRepository;

            _deviceInspectionRecordRepository = deviceInspectionRecordRepository;
            _deviceDefectRecordRepository = deviceDefectRecordRepository;


        }




        #region APP端




        // ====================================== APP 维修、检修结果提交 ======================================





        /// <summary>
        /// 提交单个维修结果   todo: 提交维修记录  1-根据参数task_id，修改字段remark，photos，维修记录表的状态，消缺时间。2- 反写务管理表的状态，和实际完成时间
        /// </summary>
        [HttpPost("submitDeviceWxStatus")]
        public async Task<AjaxResult> SubmitDeviceWxStatus([FromBody] SubmitDeviceStatusDto dto)
        {
            // 直接调用 Service 层方法
            return await _service.SubmitDeviceWxStatusAsync(dto);
        }


        /// <summary> 
        /// 提交单个抢修结果  备用误删！
        /// </summary>
        [HttpPost("submitDeviceQxStatus")]
        public async Task<AjaxResult> SubmitDeviceQxStatus([FromBody] SubmitDeviceStatusDto dto)
        {
            // 直接调用 Service 层方法
            return await _service.SubmitDeviceQxStatusAsync(dto);
        }



        // ====================================== APP 巡检合格/不合格结果提交 ======================================



        /// <summary>
        /// 提交单个设备巡检结果
        /// </summary>
        [HttpPost("submitDeviceStatus")]
        public async Task<AjaxResult> SubmitDeviceStatus([FromBody] SubmitDeviceStatusDto dto)
        {
            // 直接调用 Service 层方法
            return await _service.SubmitDeviceStatusAsync(dto);
        }


        /// <summary>
        /// 获取任务下的已完成设备列表（返回 finished_device_ids 列）
        /// </summary>
        [HttpGet("getFinishedDevices")]
        public async Task<AjaxResult> GetFinishedDevices(long taskId)
        {
            // 参数校验
            if(taskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");

            // 查询 device_inspection_record 表中的 finished_device_ids 字段
            //var sql = @"SELECT finished_device_ids FROM device_inspection_record WHERE id = @taskId";


            var sql = @"   SELECT finished_device_ids
                                FROM device_inspection_record
                                WHERE task_id = @taskId
                                ORDER BY create_time DESC  
                                LIMIT 1 ";

            var finishedJson = await _deviceInspectionRecordRepository
                .Repo.Context.Ado.GetStringAsync(sql,new { taskId });

            // 反序列化（默认为空数组）
            var finishedList = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(finishedJson ?? "[]");

            return AjaxResult.Success(finishedList);
        }


        /// <summary>
        /// 查询指定任务  的巡检缺陷详情  - APP
        /// </summary>
        [HttpGet("getDefectDetail")]
        public async Task<AjaxResult> GetDefectDetail(long taskId,long deviceId)
        {
            if(taskId <= 0 || deviceId <= 0)
                return AjaxResult.Error("参数无效");

            // ✅ 使用实体查询
            var data = await _deviceDefectRecordRepository
                .Repo.AsQueryable()
                .Where(d => d.TaskId == taskId && d.DeviceId == deviceId)
                .FirstAsync();

            return AjaxResult.Success(data);
        }



        /// <summary>
        /// 查询指定任务 + 设备的详细信息 - APP
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="deviceId">设备id</param>
        /// <returns></returns>
        [HttpGet("getDefectDetailDevice")]
        public async Task<AjaxResult> getDefectDetailDevice(long taskId,long deviceId)
        {
            if(taskId <= 0 || deviceId <= 0)
                return AjaxResult.Error("参数无效");

            // 查询任务信息并映射为 DTO
            var taskDto = await _deviceDefectRecordRepository
                .Repo.AsQueryable()
                .Where(d => d.TaskId == taskId && d.DeviceId == deviceId)
                .Select(t => new DeviceDefectRecordDto
                {
                    Id = t.Id,
                    DefectDesc = t.DefectDesc,//缺陷描述
                    DefectName = t.DefectName, //维修名称
                    DeviceName = t.DeviceName, //设备名称
                    Remark = t.Remark, //填写结果
                    ImageUrl = t.ImageUrl,

                })
                .FirstAsync();

            // 查询设备信息并映射为 DTO
            var deviceDto = await _deviceManagementRepository
                .Repo.AsQueryable()
                .Where(d => d.Id == deviceId)
                .Select(d => new DeviceManagementDto
                {
                    Id = d.Id,
                    Model = d.Model,
                    Label = d.Label, //设备名称
                    RatedVoltage = d.RatedVoltage,
                    RatedCurrent = d.RatedCurrent,

                    // 可继续补充字段
                })
                .FirstAsync();

            // 组合结果
            var result = new DeviceTaskDetailDto
            {
                TaskInfo = taskDto,
                DeviceInfo = deviceDto
            };


            //todo: 家判定  当TaskInfo和DeviceInfo  不为null时候才返回，否则报错查不到数据


            return AjaxResult.Success(result);
        }




        //========================================================================================================================================================


        #endregion





        #region PC端



        /// <summary>
        /// 上传维修/巡检图片（保存到服务器本地）
        /// </summary>
        [HttpPost("uploadImage")]
        [RequestSizeLimit(3 * 1024 * 1024)] // 最大上传3MB
        public async Task<AjaxResult> UploadImage(IFormFile file)
        {
            if(file == null || file.Length == 0)
                return AjaxResult.Error("上传文件不能为空");

            // 校验后缀名
            var ext = Path.GetExtension(file.FileName).ToLower();
            if(ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                return AjaxResult.Error("仅支持 JPG/PNG 格式");

            // 保存路径：/wwwroot/uploads/yyyyMMdd/
            var dateDir = DateTime.Now.ToString("yyyyMMdd");
            var folder = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads",dateDir);
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // 生成唯一文件名
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder,fileName);

            // 保存文件
            using(var stream = new FileStream(fullPath,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("📦 图片已保存到本地路径：{0}",fullPath);


            // 拼接访问 URL
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var accessPath = $"/uploads/{dateDir}/{fileName}";
            var url = baseUrl + accessPath;



             return AjaxResult.Success(data: url);

        }




        /// <summary>
        /// 根据任务ID查询任务下的所有设备列表
        /// </summary>
        [HttpGet("getDevicesByTaskId")]
        public async Task<AjaxResult> GetDevicesByTaskId(long taskId)
        {
            // ===================== 任务绑定设备ID列表（JSON）读取 =====================
            // SQL语句：从 device_inspection_task 表中获取 JSON 格式的 device_ids 字段
            var sql = @"SELECT device_ids FROM device_inspection_task WHERE id = @taskId";

            // 执行 SQL 并读取 JSON 字符串（如："[1,2,3]"）
            var deviceIdJson = await _deviceInspectionTaskRepository
                .Repo.Context.Ado.GetStringAsync(sql,new { taskId });

            // JSON 反序列化为 long 类型的设备 ID 列表
            var idList = JsonConvert.DeserializeObject<List<long>>(deviceIdJson ?? "[]");

            // ===================== 查询设备信息列表（从设备表） =====================
            // 使用设备仓储 _deviceManagementRepository 查询设备表中的信息
            var deviceList = await _deviceManagementRepository
                .Repo.Context
                .Queryable<DeviceManagement>()
                .Where(it => idList.Contains(it.Id))
                .Select(it => new DeviceManagementDto
                {
                    Id = it.Id,                         // 设备ID
                    Label = it.Label,                   // 设备名称
                    Model = it.Model,                   // 型号
                    RatedCurrent = it.RatedCurrent,     // 额定电流
                    RatedVoltage = it.RatedVoltage      // 额定电压
                })
                .ToListAsync();

            // ===================== 返回统一结果 =====================
            return AjaxResult.Success(deviceList);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetPageList([FromQuery] DeviceInspectionTaskDto dto)
        {
            //return await _service.GetDtoPagedListAsync(dto);
            return await _service.GetDtoPagedListDescAsync(dto);

        }


        /// <summary>
        /// 任务分页列表 - 手机巡检待办/在办 
        /// </summary>
        [HttpGet("pageListApp")]
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetPageListApp([FromQuery] DeviceInspectionTaskDto dto)
        {
            //return await _service.GetDtoPagedListAsync(dto);


            return await _service.GetDtoPagedListDescAsyncApp(dto);

        }



        /// <summary>
        /// 任务分页列表 - 手机巡检待办/在办 (维修/抢修专用)
        /// 仅处理 deviceIds 中的首个设备，设置 DevId 与 DevName
        /// </summary>
        [HttpGet("pageListWxApp")]
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetPageListWxApp([FromQuery] DeviceInspectionTaskDto dto)
        {
            //return await _service.GetDtoPagedListAsync(dto);


            return await _service.GetDtoPagedListDescAsyncWxApp(dto);

        }


        /// <summary>
        /// 获取分页列表 - 手机维修抢修的待办/在办  备用
        /// </summary>
        [HttpGet("pageListAppWx")]
        public async Task<SqlSugarPagedList<DeviceDefectRecordDto>> GetPageListAppWx([FromQuery] DeviceDefectRecordDto dto)
        {
            var query = _deviceDefectRecordRepository
                .Repo.AsQueryable()
                .Where(d => new[] { "待办","在办" }.Contains(d.DefectStatus)); // 状态固定写死

            // 补充：仅限维修类型（由前端传入）
            if(!string.IsNullOrWhiteSpace(dto.DefectCategory))
            {
                query = query.Where(d => d.DefectCategory == dto.DefectCategory);
            }

            // 分页 + 映射为 DTO
            return await query
                .OrderBy(d => d.CreateTime,OrderByType.Desc)
                .Select(d => new DeviceDefectRecordDto
                {
                    Id = d.Id,
                    TaskId = d.TaskId,
                    DeviceId = d.DeviceId,//设备id
                    DeviceName = d.DeviceName,//设备名称



                    DefectName = d.DefectName,
                    DefectStatus = d.DefectStatus,
                    DefectCategory = d.DefectCategory,
                    Remark = d.Remark,
                    CreateTime = d.CreateTime
                })
                .ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);
        }




        /// <summary>
        /// 新增任务   -- 此处需要判断
        /// </summary>
        [HttpPost("add")]
        public async Task<AjaxResult> Add([FromBody] DeviceInspectionTaskDto dto)
        {


            return await _service.AddWithRecordAsync(dto);


        }


        /// <summary>
        /// 下发消缺任务 - 缺陷页面专用 ！
        /// </summary>
        [HttpPost("addQx")]
        public async Task<AjaxResult> AddQx([FromBody] DeviceInspectionTaskDto dto)
        {


            return await _service.AddWithRecordQxAsync(dto);


        }

        [HttpPost("add1")]
        public async Task<IActionResult> Add( )
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            Console.WriteLine("【原始请求体】：" + rawBody);

            return Ok(new { msg = "收到原始数据",body = rawBody });
        }





        /// <summary>
        /// 修改
        /// </summary>
        [HttpPost("update")]
        public async Task<AjaxResult> Update([FromBody] DeviceInspectionTaskDto dto)
        {
            var rows = await _service.UpdateAsync(dto);
            return rows > 0 ? AjaxResult.Success("更新成功") : AjaxResult.Error("更新失败");
        }

        /// <summary>
        /// 删除 （支持批量）
        /// </summary>
        [HttpPost("delete")]
        public async Task<AjaxResult> Delete([FromBody] List<long> ids)
        {
            if(ids == null || !ids.Any()) return AjaxResult.Error("未指定删除数据");

            await _service.DeleteAsync(ids);
            return AjaxResult.Success("删除成功");
        }




        #endregion


    }
}

/// <summary>
/// getDefectDetailDevice的返回临时dto
/// </summary>
public class DeviceTaskDetailDto
{
    public DeviceDefectRecordDto? TaskInfo { get; set; }
    public DeviceManagementDto? DeviceInfo { get; set; }
}
