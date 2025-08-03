using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using RuoYi.Data.Entities;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    /// <summary>
    /// 巡检任务仓储类
    /// </summary>
    public class DeviceInspectionTaskRepository : BaseRepository<DeviceInspectionTask,DeviceInspectionTaskDto>
    {
        public DeviceInspectionTaskRepository(ISqlSugarRepository<DeviceInspectionTask> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceInspectionTask> Queryable(DeviceInspectionTaskDto dto)
        {
            var par = Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.TaskName),it => it.TaskName.Contains(dto.TaskName))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.TaskStatus),it => it.TaskStatus.Contains(dto.TaskStatus))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.TaskType),it => it.TaskType == dto.TaskType);

            return par;
        }



        public override ISugarQueryable<DeviceInspectionTaskDto> DtoQueryable(DeviceInspectionTaskDto dto)
        {
            var par = Repo.AsQueryable()
                  //.WhereIF(!string.IsNullOrWhiteSpace(dto.TaskName),it => it.TaskName.Contains(dto.TaskName))
                  //.WhereIF(!string.IsNullOrWhiteSpace(dto.TaskStatus),it => it.TaskStatus.Contains(dto.TaskStatus))
                  //.WhereIF(!string.IsNullOrWhiteSpace(dto.TaskType),it => it.TaskType == dto.TaskType)
                .WhereIF(!string.IsNullOrWhiteSpace(dto.TaskName),it => it.TaskName.Contains(dto.TaskName))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.TaskType),it => it.TaskType == dto.TaskType)
                //.WhereIF(string.IsNullOrWhiteSpace(dto.TaskStatus),it => new[] { "待办","在办" }.Contains(it.TaskStatus)) //如果参数为空
                .WhereIF(!string.IsNullOrWhiteSpace(dto.TaskStatus),it => it.TaskStatus.Contains(dto.TaskStatus))
                .Select(it => new DeviceInspectionTaskDto
                {
                    Id = it.Id,
                    TaskName = it.TaskName,
                    TicketId = it.TicketId,
                    LeaderId = it.Leader, // 保留原始ID，后续转换
                    ExecutorId = it.Executor, // 保留原始ID，后续转换
                    TaskType = it.TaskType,
                    TaskStatus = it.TaskStatus,
                    CheckInDeviation = it.CheckInDeviation,
                    PlanStartTime = it.PlanStartTime,
                    PlanEndTime = it.PlanEndTime,
                    ActualEndTime = it.ActualEndTime,
                    DeviceIds = it.DeviceIds,
                    Remark = it.Remark,
                    CreateBy = it.CreateBy,
                    CreateTime = it.CreateTime,
                    UpdateBy = it.UpdateBy,
                    UpdateTime = it.UpdateTime
                });

            return par;
        }




        /// <summary>
        /// 查询任务分页列表，按创建时间降序排序 - App端
        /// </summary>
        /// <param name="dto">查询条件</param>
        /// <returns>分页结果</returns>
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListOrderedByMaintenanceCycleAppAsync(DeviceInspectionTaskDto dto)
        {
          


            // 获取基础查询对象
            var query = DtoQueryable(dto);

            // ✅ 增加筛选：只查询当前登录用户为执行人的任务
            var currentUser = SecurityUtils.GetLoginUser()?.User?.UserName;
            if(!string.IsNullOrWhiteSpace(currentUser))
            {
                query = query.Where(it => it.Executor == currentUser);
            }
 

            // 如果 TaskStatus 为空，则默认筛选待办、在办
            if(string.IsNullOrWhiteSpace(dto.TaskStatus))
            {
                query = query.Where(it => new[] { "待办","在办" }.Contains(it.TaskStatus));
            }

            // 获取分页数据
            var pagedInfo = await query
                .OrderBy(t => t.CreateTime,OrderByType.Desc)
                .ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);

            return await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);

        }


        /// <summary>
        /// 查询任务分页列表，按创建时间降序排序 - App端  （维修/抢修）
        /// 仅处理 deviceIds 中的首个设备，设置 DevId 与 DevName
        /// </summary>
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListOrderedByMaintenanceCycleAppWxAsync(DeviceInspectionTaskDto dto)
        {


            // todo:  我想增加筛选  当executor =  SecurityUtils.GetLoginUser().User.UserName;  //获取当前用户名称
     


            var query = DtoQueryable(dto);

            // ✅ 增加筛选：只查询当前登录用户为执行人的任务
            var currentUser = SecurityUtils.GetLoginUser()?.User?.UserName;
            if(!string.IsNullOrWhiteSpace(currentUser))
            {
                query = query.Where(it => it.Executor == currentUser);
            }

            // ✅ 默认状态筛选：仅查询“待办”“在办”

            if(string.IsNullOrWhiteSpace(dto.TaskStatus))
            {
                query = query.Where(it => new[] { "待办","在办" }.Contains(it.TaskStatus));
            }

            var pagedList = await query
                .OrderBy(t => t.CreateTime,OrderByType.Desc)
                .ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);

            var firstDeviceIdList = new List<long>();

            foreach(var item in pagedList.Rows)
            {
                try
                {
                    var ids = JsonConvert.DeserializeObject<List<long>>(item.DeviceIds ?? "[]");
                    if(ids != null && ids.Count > 0)
                    {
                        item.DevId = ids[0];  // ✅ DevId 是 long
                        firstDeviceIdList.Add(ids[0]);
                    }
                }
                catch
                {
                    item.DevId = 0; // 出错时默认设为0（无效ID）
                }
            }

            if(firstDeviceIdList.Count > 0)
            {
                var deviceDict = await Repo.Context.Queryable<DeviceManagement>()
                    .Where(d => firstDeviceIdList.Contains(d.Id))
                    .ToDictionaryAsync(d => d.Id,d => d.Label ?? "");

                foreach(var item in pagedList.Rows)
                {
                    if(deviceDict.TryGetValue(item.DevId.ToString(),out var name))
                    {
                        item.DevName = name.ToString();
                    }
                    else
                    {
                        item.DevName = "未知设备";
                    }
                }
            }

            return pagedList;
        }







        /// <summary>
        /// 查询分页列表，按创建时间降序排序
        /// </summary>
        /// <param name="dto">查询条件</param>
        /// <returns>分页结果</returns>
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListOrderedByMaintenanceCycleAsync(DeviceInspectionTaskDto dto)
        {
            // 第一步：获取分页数据
            var pagedInfo = await DtoQueryable(dto)
                .OrderBy(t => t.CreateTime,OrderByType.Desc)
                .ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);

            pagedInfo.Code = StatusCodes.Status200OK;

            // 第二步：从内存中取用户数据并进行转换（修正后的代码）
            var userList = (await Repo.Context.Queryable<SysUser>()
                .Where(u => u.DelFlag == "0")
                .ToDictionaryAsync(u => u.UserId.ToString(),u => u.NickName))
                .ToDictionary(k => k.Key,v => v.Value?.ToString() ?? "");

            // 转换Leader、Executor为姓名
            foreach(var item in pagedInfo.Rows)
            {
                if(!string.IsNullOrEmpty(item.LeaderId) && userList.ContainsKey(item.LeaderId))
                {
                    item.Leader = userList[item.LeaderId];
                }
                else
                {
                    item.Leader = item.LeaderId;
                }

                if(!string.IsNullOrEmpty(item.ExecutorId) && userList.ContainsKey(item.ExecutorId))
                {
                    item.Executor = userList[item.ExecutorId];
                }
                else
                {
                    item.Executor = item.ExecutorId;
                }
            }

            return pagedInfo;
        }


 





    }
}
