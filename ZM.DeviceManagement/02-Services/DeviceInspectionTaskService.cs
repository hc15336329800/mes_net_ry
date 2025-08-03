using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using RuoYi.Common.Utils; // 用于 NextId
using Newtonsoft.Json;
using RuoYi.Framework.Interceptors;
using RuoYi.Framework;
using ZM.Device.Tool;
using ZM.Device._05_Dtos;
using UAParser.Objects;
using Newtonsoft.Json.Linq;
using RuoYi.Data.Entities;

namespace ZM.Device.Services
{
    /// <summary>
    /// 巡检任务业务服务类
    /// </summary>
    public class DeviceInspectionTaskService : BaseService<DeviceInspectionTask,DeviceInspectionTaskDto>
    {
        private readonly ILogger<DeviceInspectionTaskService> _logger;
        private readonly DeviceInspectionTaskRepository _repo;
        private readonly DeviceInspectionRecordRepository _deviceInspectionRecordRepository;
        private readonly DeviceInspectionTicketRepository _ticketRepo;
        private readonly DeviceDefectRecordRepository _defectRepo;
        private readonly DeviceInspectionRecordRepository _recordRepo;
        private readonly DeviceManagementRepository _deviceManagementRepo;



        public DeviceInspectionTaskService(
            ILogger<DeviceInspectionTaskService> logger,
            DeviceInspectionTaskRepository repo,
            DeviceInspectionRecordRepository deviceInspectionRecordRepository,
                DeviceInspectionTicketRepository ticketRepo,   // 注入操作票仓储
                 DeviceDefectRecordRepository defectRepo,
                 DeviceInspectionRecordRepository recordRepo,
            DeviceManagementRepository deviceManagementRepo

        )
        {
            _logger = logger;
            _repo = repo;
            _deviceInspectionRecordRepository = deviceInspectionRecordRepository;
            _ticketRepo = ticketRepo;                      // 保存引用
            _recordRepo = recordRepo;
            _defectRepo = defectRepo;
            _deviceManagementRepo = deviceManagementRepo;
            BaseRepo = repo;
        }

        /// <summary>
        /// 分页查询-按 maintenance_cycle 升序排序的逻辑
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListDescAsync(DeviceInspectionTaskDto dto)
        {
            return await _repo.GetDtoPagedListOrderedByMaintenanceCycleAsync(dto);
        }

        /// <summary>
        /// 分页查询- 待办+在办 - app
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListDescAsyncApp(DeviceInspectionTaskDto dto)
        {

             
            dto.TaskStatus = null;  //设置null. 内部自动待办,在办
            return await  _repo.GetDtoPagedListOrderedByMaintenanceCycleAppAsync(dto);
        }



        /// <summary>
        /// 分页查询- 待办+在办 - app 维修
        /// 仅处理 deviceIds 中的首个设备，设置 DevId 与 DevName
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListDescAsyncWxApp(DeviceInspectionTaskDto dto)
        {


            dto.TaskStatus = null;  //设置null. 内部自动待办,在办
            return await _repo.GetDtoPagedListOrderedByMaintenanceCycleAppWxAsync(dto);
        }




        /// <summary>
        /// 新增任务及关联记录（事务操作） 原来的正常
        /// </summary>
        //[Transactional]
        //public virtual async Task<AjaxResult> AddWithRecordAsync(DeviceInspectionTaskDto dto)
        //{
        //    if(dto == null)
        //        return AjaxResult.Error("参数不可为空");


        //    // —— 0. 校验任务类型 —— 
        //    var allowedTypes = new[] { "巡检","维修","抢修" };
        //    if(!allowedTypes.Contains(dto.TaskType))
        //        return AjaxResult.Error($"无效的任务类型：{dto.TaskType}");

        //    // —— 1. 查询操作票，获取 JSON 设备列表 —— 
        //    var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == dto.TicketId);
        //    var deviceIdsJson = ticket?.DeviceIds ?? "[]";

        //    // —— 2. 新增任务 —— 
        //    dto.Id = NextId.Id13();
        //    dto.TaskStatus = "待办";
        //    dto.DeviceIds = deviceIdsJson;  // 待巡检设备列表
        //    var taskInserted = await _repo.InsertAsync(dto);
        //    if(!taskInserted)
        //        return AjaxResult.Error("新增任务失败");


        //    // —— 3. 构造并新增记录 —— 
        //    if(dto.TaskType == "巡检")
        //    {
        //        var record = new DeviceInspectionRecord
        //        {
        //            Id = NextId.Id13(),
        //            TaskId = dto.Id,
        //            TaskName = $"{dto.TaskName}",
        //            InspectedDeviceIds = deviceIdsJson,
        //            FinishedDeviceIds = "[]",
        //            DeviceCount = JsonConvert.DeserializeObject<List<long>>(deviceIdsJson).Count,
        //            DefectTotal = 0,
        //            DefectProcessed = 0,
        //            Leader = dto.Leader,
        //            Executor = dto.Executor,
        //            TaskType = dto.TaskType,
        //            TaskStatus = "待办",
        //            TaskStartTime = DateTime.Now
        //         };
        //        var recordInserted = await _deviceInspectionRecordRepository.InsertAsync(record);
        //        if(!recordInserted)
        //            return AjaxResult.Error("新增记录失败");
        //    }
        //    else if(dto.TaskType == "维修")
        //    {

        //        // 提取第一个维修设备
        //        long did = 0;
        //        try
        //        {
        //            var jarr = JArray.Parse(dto.DeviceIds);
        //            did = (jarr.Count > 0)
        //                ? jarr[0].ToObject<long>()
        //                : 0L;
        //        }
        //        catch(JsonException ex)
        //        {
        //            return AjaxResult.Error("DeviceIds 格式不合法");
        //        }

        //        // 根据 did 查询设备名称（使用仓储已有方法）
        //        string deviceName = "未知设备";
        //        if(did > 0)
        //        {
        //            var device = await _deviceManagementRepo.FirstOrDefaultAsync(dm => dm.Id == did);
        //            if(device != null && !string.IsNullOrEmpty(device.Label))
        //            {
        //                deviceName = device.Label;
        //            }
        //        }


        //        var record = new DeviceDefectRecord
        //        {
        //            Id = NextId.Id13(),
        //            TaskId = dto.Id,
        //            //DeviceId =  dto.DeviceId, //设备id  
        //            DeviceId = did, // 解析失败时返回 0
        //            DeviceName = deviceName,// did.ToString(),//设备名称
        //            DefectName = dto.TaskName, //缺陷名称 改为 任务名称
        //            DefectDesc = dto.Remark,  //描述
        //            DefectStatus = "待办",
        //            DefectCategory = "维修",
        //            DiscoveryTime = DateTime.Now, //发现时间
        //            SeverityLevel = "一般", //严重等级
        //            //ImageUrl = "",
        //            //FixPerson = dto.Executor,//消缺人员

        //        };
        //        var recordInserted = await _defectRepo.InsertAsync(record);
        //        if(!recordInserted)
        //            return AjaxResult.Error("新增记录失败");
        //    }
        //    else if(dto.TaskType == "抢修")
        //    {

        //        // 提取第一个维修设备
        //        long did = 0;
        //        try
        //        {
        //            var jarr = JArray.Parse(dto.DeviceIds);
        //            did = (jarr.Count > 0)
        //                ? jarr[0].ToObject<long>()
        //                : 0L;
        //        }
        //        catch(JsonException ex)
        //        {
        //            return AjaxResult.Error("DeviceIds 格式不合法");
        //        }

        //        // 根据 did 查询设备名称（使用仓储已有方法）
        //        string deviceName = "未知设备";
        //        if(did > 0)
        //        {
        //            var device = await _deviceManagementRepo.FirstOrDefaultAsync(dm => dm.Id == did);
        //            if(device != null && !string.IsNullOrEmpty(device.Label))
        //            {
        //                deviceName = device.Label;
        //            }
        //        }



        //        var record = new DeviceDefectRecord
        //        {
        //            Id = NextId.Id13(),
        //            TaskId = dto.Id,
        //            //DeviceId =  dto.DeviceId, //设备id  
        //            DeviceId = did, // 解析失败时返回 0
        //            DeviceName =deviceName, // did.ToString(),//设备名称
        //            DefectName = dto.TaskName, //缺陷名称 改为 任务名称
        //            DefectDesc = dto.Remark,  //描述
        //            DefectStatus = "待办",
        //            DefectCategory = "抢修",
        //            DiscoveryTime = DateTime.Now, //发现时间
        //            SeverityLevel = "一般", //严重等级
        //            //ImageUrl = "",
        //            FixPerson = dto.Executor,//消缺人员

        //        };
        //        var recordInserted = await _defectRepo.InsertAsync(record);
        //        if(!recordInserted)
        //            return AjaxResult.Error("新增记录失败");
        //    }


        //    return AjaxResult.Success("新增成功");
        //}



        /// <summary>
        /// 新增任务及关联记录（事务操作）
        /// </summary>
        [Transactional]
        public virtual async Task<AjaxResult> AddWithRecordAsync(DeviceInspectionTaskDto dto)
        {
            if(dto == null)
                return AjaxResult.Error("参数不可为空");

            // —— 0. 校验任务类型 —— 
            var allowedTypes = new[] { "巡检","维修","抢修" };
            if(!allowedTypes.Contains(dto.TaskType))
                return AjaxResult.Error($"无效的任务类型：{dto.TaskType}");

            // —— 1. 构建 DeviceIds —— 
            string deviceIdsJson = "[]";

            if(dto.TaskType == "巡检")
            {
                var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == dto.TicketId);
                if(ticket == null)
                    return AjaxResult.Error("操作票不存在");

                deviceIdsJson = ticket.DeviceIds ?? "[]";
            }
            else // 维修 or 抢修
            {
                if(dto.DeviceId <= 0 || string.IsNullOrWhiteSpace(dto.DeviceName.ToString()))
                    return AjaxResult.Error("维修/抢修任务必须指定设备及名称");

                long deviceIdStr = dto.DeviceId ?? 0;
                deviceIdsJson = JsonConvert.SerializeObject(new List<long> { deviceIdStr });
            }

            // —— 2. 新增任务 —— 
            dto.Id = NextId.Id13();
            dto.TaskStatus = "待办";
            dto.DeviceIds = deviceIdsJson;

            var taskInserted = await _repo.InsertAsync(dto);
            if(!taskInserted)
                return AjaxResult.Error("新增任务失败");

            // —— 3. 新增记录 —— 
            if(dto.TaskType == "巡检")
            {
                var record = new DeviceInspectionRecord
                {
                    Id = NextId.Id13(),
                    TaskId = dto.Id,
                    TaskName = dto.TaskName,
                    InspectedDeviceIds = deviceIdsJson,
                    FinishedDeviceIds = "[]",
                    DeviceCount = JsonConvert.DeserializeObject<List<long>>(deviceIdsJson).Count,
                    DefectTotal = 0,
                    DefectProcessed = 0,
                    Leader = dto.Leader,
                    Executor = dto.Executor,
                    TaskType = dto.TaskType,
                    TaskStatus = "待办",
                    TaskStartTime = DateTime.Now
                };

                var recordInserted = await _deviceInspectionRecordRepository.InsertAsync(record);
                if(!recordInserted)
                    return AjaxResult.Error("新增记录失败");
            }
            else // 维修 or 抢修
            {
                var record = new DeviceDefectRecord
                {
                    Id = NextId.Id13(),
                    TaskId = dto.Id,
                    DeviceId = dto.DeviceId ?? 0,
                    DeviceName = dto.DeviceName.ToString(),
                    DefectName = dto.TaskName,
                    DefectDesc = dto.Remark,
                    DefectStatus = "待办",
                    DefectCategory = dto.TaskType,
                    DiscoveryTime = DateTime.Now,
                    SeverityLevel = "一般",
                    FixPerson = dto.Executor
                };

                var recordInserted = await _defectRepo.InsertAsync(record);
                if(!recordInserted)
                    return AjaxResult.Error("新增记录失败");
            }

            return AjaxResult.Success("新增成功");
        }




        /// <summary>
        /// 缺陷页面：新增任务及关联记录（事务操作）
        /// </summary>
        [Transactional]
        public virtual async Task<AjaxResult> AddWithRecordQxAsync(DeviceInspectionTaskDto dto)
        {
           

            if(dto == null)
                return AjaxResult.Error("参数不可为空");

            if(string.IsNullOrWhiteSpace(dto.DeviceIds))
                return AjaxResult.Error("设备ID不能为空");


             // ✅ 安全解析 JSON 字符串为 List<long>
            List<long> Dids;
            try
            {
                Dids = JsonConvert.DeserializeObject<List<long>>(dto.DeviceIds);
            }
            catch(Exception ex)
            {
                return AjaxResult.Error("设备ID格式非法，需为数字数组字符串，如：[123456]");
            }

            if(Dids == null || !Dids.Any())
                return AjaxResult.Error("设备ID为空，解析失败");

            long deviceIdss = Dids.First(); // 仅取第一个设备参与维修记录



            // ✅ 0. 判断 relatedTaskId 对应缺陷是否已提交
            if(dto.RelatedTaskId > 0)
            {
                var existDefect = await _defectRepo.Repo.AsQueryable()
                    .Where(d => d.TaskId == dto.RelatedTaskId && d.DeviceId == deviceIdss && d.DefectCategory == "缺陷")
                    .FirstAsync();

                if(existDefect != null && !string.IsNullOrWhiteSpace(existDefect.DevicePath))
                {
                    return AjaxResult.Error("该缺陷已提交维修任务，不能重复操作");
                }
            }




            //转化
            var ids = JsonConvert.DeserializeObject<List<long>>(dto.DeviceIds ?? "[]");
            long did = ids.FirstOrDefault();



            // —— 2. 新增维修任务 —— 
            dto.Id = NextId.Id13();
            dto.TicketId = 0;
            dto.TaskType = "维修";
            dto.TaskStatus = "待办";
            dto.DeviceIds = dto.DeviceIds;  // 待巡检设备列表   因为是维修直接

            dto.Leader = dto.Leader; //负责人
            dto.Executor = dto.Executor; //执行人

            var taskInserted = await _repo.InsertAsync(dto);
            if(!taskInserted)
                return AjaxResult.Error("新增任务失败");



            // —— 3. 构造并新增维修记录 —— 
 
                // 根据 did 查询设备名称（使用仓储已有方法）
                string deviceName = "未知设备";
                if(did > 0)
                {
                    var device = await _deviceManagementRepo.FirstOrDefaultAsync(dm => dm.Id == did);
                    if(device != null && !string.IsNullOrEmpty(device.Label))
                    {
                        deviceName = device.Label;
                    }
                }


                var record = new DeviceDefectRecord
                {
                    Id = NextId.Id13(),
                    TaskId = dto.Id,
                    //DeviceId =  dto.DeviceId, //设备id  
                    DeviceId = did, // 解析失败时返回 0
                    DeviceName = deviceName,// did.ToString(),//设备名称
                    DefectName = dto.TaskName, //缺陷名称 改为 任务名称
                    DefectDesc = dto.Remark,  //描述
                    DefectStatus = "待办",
                    DefectCategory = "维修",
                    DiscoveryTime = DateTime.Now, //发现时间
                    SeverityLevel = "一般", //严重等级
                    ImageUrl = dto.ImageUrl,

                    
                    FixPerson = dto.Executor,//执行人  (消缺人员)


                    //FixPerson = dto.Leader,//消缺人员
                    CreateBy = SecurityUtils.GetLoginUser().User.UserName  //获取当前用户名称
 
                };
                var recordInserted = await _defectRepo.InsertAsync(record);
                if(!recordInserted)
                    return AjaxResult.Error("新增记录失败");


 
 
            // ✅ 更新原“缺陷”记录 device_path = dto.RelatedTaskId
            if(dto.RelatedTaskId > 0)
            {
                var updatedRows = await _defectRepo.Repo.Context.Updateable<DeviceDefectRecord>()
                    .SetColumns(d => new DeviceDefectRecord
                    {
                        DevicePath = dto.Id.ToString(),  // 补丁！
                        //FixPerson = dto.Leader,//消缺人员
                        UpdateBy = SecurityUtils.GetLoginUser().User.UserName,  //获取当前用户名称
                        UpdateTime = DateTime.Now
                    })
                    .Where(d => d.TaskId == dto.RelatedTaskId && d.DefectCategory == "缺陷")
                    .ExecuteCommandAsync();

                if(updatedRows == 0)
                {
                    _logger.LogWarning($"未找到原缺陷记录进行更新，RelatedTaskId={dto.RelatedTaskId}");
                }
            }



            return AjaxResult.Success("新增成功");
        }


        /// <summary>
        /// App提交单个设备巡检结果
        /// 涉及到3张表（device_inspection_record、device_inspection_task、device_defect_record）
        /// </summary>
        [Transactional]  // 方法需 virtual 以支持事务拦截
        public virtual async Task<AjaxResult> SubmitDeviceStatusAsync(SubmitDeviceStatusDto dto)
        {
            // 参数校验
            if(dto.TaskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");
            if(dto.Id <= 0)
                return AjaxResult.Error("参数 id 不可为空或无效");
            if(string.IsNullOrWhiteSpace(dto.DevStatus))
                return AjaxResult.Error("参数 devStatus 不可为空");
            if(dto.DevStatus == "false" && string.IsNullOrWhiteSpace(dto.Remark))
                return AjaxResult.Error("参数 remark 不可为空");

            // 读取当前巡检记录
            var record = await _recordRepo.Repo.Context
                .Queryable<DeviceInspectionRecord>()
                .Where(r => r.TaskId == dto.TaskId)
                .FirstAsync();
            if(record == null)
                return AjaxResult.Error("未找到对应的巡检记录");

            // 反序列化 finishedDeviceIds
            var finishedJson = record.FinishedDeviceIds ?? "[]";
            var finishedList = JsonConvert
                .DeserializeObject<List<Dictionary<string,string>>>(finishedJson)
                ?? new List<Dictionary<string,string>>();

            // 查找 & 更新单设备状态
            var key = dto.Id.ToString();
            var entry = finishedList.FirstOrDefault(m => m.ContainsKey(key));
            if(entry != null)
                entry[key] = dto.DevStatus.ToLower();
            else
                finishedList.Add(new Dictionary<string,string> { { key,dto.DevStatus.ToLower() } });

            // 持久化 finishedDeviceIds
            record.FinishedDeviceIds = JsonConvert.SerializeObject(finishedList);
            await _recordRepo.UpdateAsync(record);



            //==================== 新增/删除 设备巡检缺陷（合格/不合格 ）====================

            if(dto.DevStatus == "false")
            {
                string deviceName = "未知设备"; // 默认值
                var device = await _deviceManagementRepo.FirstOrDefaultAsync(d => d.Id == dto.Id);
                if(device != null && !string.IsNullOrWhiteSpace(device.Label))
                {
                    deviceName = device.Label;
                }

                var defect = new DeviceDefectRecord
                {

                    Id = NextId.Id13(),
                    TaskId = dto.TaskId,
                    DeviceId = dto.Id,
                    DeviceName = deviceName, // ✅ 真实的设备名称
                    DefectName = record.TaskName,
                    DefectDesc = dto.Remark,
                    ImageUrl = dto.ImageUrl,
                    DiscoveryTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    CreateBy = SecurityUtils.GetUsername(),
                    DefectStatus = "待办",
                    DefectCategory = "缺陷"

                };
                await _defectRepo.InsertAsync(defect);
            }
            else
            {
                await _defectRepo.Repo.DeleteAsync(d =>
                    d.TaskId == dto.TaskId && d.DeviceId == dto.Id);
            }



            //==================== 更新任务状态（完成/未完成） ====================

            if(dto.IsCompleted) // ✅ 仅在当设备全部巡检完成时
            {
               

                record.TaskStatus = "办毕"; //反写完成状态
                record.TaskEndTime = DateTime.Now;//反写完成时间
                await _deviceInspectionRecordRepository.UpdateAsync(record);//更新巡检表 上面有id


                //todo:  我想设置下这个字段 DeviceInspectionTask.ActualEndTime = DateTime.Now;
                await _repo.Repo.Context//更新任务表
                    .Updateable<DeviceInspectionTask>()
                    .SetColumns(t => new DeviceInspectionTask {
                        TaskStatus = record.TaskStatus, //反写完成状态
                         ActualEndTime = DateTime.Now //反写完成时间
                    })
                    .Where(t => t.Id == dto.TaskId)
                    .ExecuteCommandAsync();
            }
            else
            {

                record.TaskStatus = "在办";
                await _deviceInspectionRecordRepository.UpdateAsync(record);//更新巡检表 上面有id

                await _repo.Repo.Context//更新任务表
                    .Updateable<DeviceInspectionTask>()
                    .SetColumns(t => new DeviceInspectionTask { TaskStatus = record.TaskStatus })
                    .Where(t => t.Id == dto.TaskId)
                    .ExecuteCommandAsync();
            }





            return AjaxResult.Success("设备状态更新成功");
        }




        //开始手机端的维修提交

        /// <summary>
        /// App提交单个维修结果
        /// </summary>
        /// <summary>
        /// App提交单个维修结果（更新 defect 表 + 反写状态）
        /// </summary>
        [Transactional]
        public virtual async Task<AjaxResult> SubmitDeviceWxStatusAsync(SubmitDeviceStatusDto dto)
        {
            // devStatus	"true"   备用字段是否完成


            // === 参数校验 ===
            if(dto.TaskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");

            if(dto.Id <= 0)
                return AjaxResult.Error("参数 id 不可为空或无效");

            if(string.IsNullOrWhiteSpace(dto.DevStatus))
                return AjaxResult.Error("参数 devStatus 不可为空");

            // === 查询已有的缺陷记录（维修类型）===
            var defect = await _defectRepo.Repo.AsQueryable()
                .Where(d => d.TaskId == dto.TaskId && d.DefectCategory == "维修")
                .FirstAsync();

            if(defect == null)
                return AjaxResult.Error("未找到维修记录");

            // === 更新维修记录表 ===
            defect.Remark = dto.Remark;
            defect.ImageUrl = dto.ImageUrl;
            defect.FixTime = DateTime.Now;
            defect.DefectStatus = "办毕";
            defect.FixPerson = SecurityUtils.GetLoginUser().User.UserName;//消缺人员

            defect.UpdateTime = DateTime.Now;
            defect.UpdateBy = SecurityUtils.GetLoginUser().User.UserName; 


            await _defectRepo.UpdateAsync(defect);



            // === 反写缺陷记录表 ===   

            var defectQx = await _defectRepo.Repo.AsQueryable()
              .Where(d => d.DevicePath == dto.TaskId.ToString() && d.DefectCategory == "缺陷")
              .FirstAsync();

            if(defectQx != null) { 
                defectQx.DefectStatus = "办毕";
                defectQx.UpdateTime = DateTime.Now;
                defectQx.UpdateBy = SecurityUtils.GetLoginUser().User.UserName;  //获取当前用户名称

                defectQx.FixPerson = SecurityUtils.GetLoginUser().User.UserName;//处理人员
                defectQx.FixTime = DateTime.Now; //处理时间

                await _defectRepo.UpdateAsync(defectQx);
            
            }
                 
 

            // === 更新任务表状态 ===
            await _repo.Repo.Context.Updateable<DeviceInspectionTask>()
                    .SetColumns(t => new DeviceInspectionTask
                    {
                        TaskStatus = "办毕",
                        ActualEndTime = DateTime.Now,
                        UpdateBy = SecurityUtils.GetUsername(),
                        UpdateTime = DateTime.Now
                    })
                    .Where(t => t.Id == dto.TaskId)
                    .ExecuteCommandAsync();


            return AjaxResult.Success("维修记录提交成功");
        }





        /// <summary>
        /// App提交单个抢修结果
        /// </summary>
        [Transactional]
        public virtual async Task<AjaxResult> SubmitDeviceQxStatusAsync(SubmitDeviceStatusDto dto)
        {
            // devStatus	"true"   备用字段是否完成


            // === 参数校验 ===
            if(dto.TaskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");

            if(dto.Id <= 0)
                return AjaxResult.Error("参数 id 不可为空或无效");

            if(string.IsNullOrWhiteSpace(dto.DevStatus))
                return AjaxResult.Error("参数 devStatus 不可为空");

            


            // === 反写抢修记录表 ===   

            var defectQx = await _defectRepo.Repo.AsQueryable()
              .Where(d => d.TaskId == dto.TaskId && d.DefectCategory == "抢修" )
              .FirstAsync();

            if(defectQx != null)
            {
                defectQx.ImageUrl = dto.ImageUrl;//图片
                defectQx.Remark = dto.Remark; //描述
                defectQx.DefectStatus = "办毕";
                defectQx.UpdateTime = DateTime.Now;
                defectQx.UpdateBy = SecurityUtils.GetLoginUser().User.UserName;  //获取当前用户名称

                defectQx.FixPerson = SecurityUtils.GetLoginUser().User.UserName;//处理人员
                defectQx.FixTime = DateTime.Now; //处理时间

                await _defectRepo.UpdateAsync(defectQx);

            }



            // === 更新任务表状态 ===
            await _repo.Repo.Context.Updateable<DeviceInspectionTask>()
                    .SetColumns(t => new DeviceInspectionTask
                    {
                        TaskStatus = "办毕",
                        ActualEndTime = DateTime.Now,
                        UpdateBy = SecurityUtils.GetUsername(),
                        UpdateTime = DateTime.Now
                    })
                    .Where(t => t.Id == dto.TaskId)
                    .ExecuteCommandAsync();


            return AjaxResult.Success("维修记录提交成功");
        }


        

    }
}
