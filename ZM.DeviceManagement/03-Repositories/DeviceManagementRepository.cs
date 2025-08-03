using Microsoft.AspNetCore.Http;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using SqlSugar;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    /// <summary>
    /// 设备管理
    /// </summary>
    public class DeviceManagementRepository : BaseRepository<DeviceManagement,DeviceManagementDto>
    {
        public DeviceManagementRepository(ISqlSugarRepository<DeviceManagement> sqlSugarRepository)
        {
            // 注意：Repo 只能针对 DeviceManagement 表进行标准操作（如 Insert, Update, Queryable）但是 不能在 Repo 上做通用型的 SqlQueryable<T>() ！
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceManagement> Queryable(DeviceManagementDto dto)
        {
            return Repo.AsQueryable();
            //.Includes(t => t.SubTable)
            //.WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
            // .OrderBy(t => t.Id,OrderByType.Desc); // ✅ 按 ID 倒序排序
            
        }


        /// <summary>
        /// 新增一个临时的字段保养倒计时
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public override ISugarQueryable<DeviceManagementDto> DtoQueryable(DeviceManagementDto dto)
        {
            return Repo.AsQueryable()
            //.LeftJoin<SubTable>((t, s) => t.Id == s.Id)
            //.WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
            // 模糊查询设备名称（非空才过滤）
            .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label))
                .Select((t) => new DeviceManagementDto
                {
                    // 主键及基础信息
                    Id = t.Id,
                    Label = t.Label,
                    DeviceType = t.DeviceType,
                    Model = t.Model,
                    Capacity = t.Capacity,
                    Quantity = t.Quantity,
                    Weight = t.Weight,
                    Manufacturer = t.Manufacturer,
                    InstallDate = t.InstallDate,
                    RatedCurrent = t.RatedCurrent,
                    RatedVoltage = t.RatedVoltage,
                    Status = t.Status,
                    TempControl = t.TempControl,
                    MaintenanceCycle = t.MaintenanceCycle,
                    WarrantyPeriod = t.WarrantyPeriod,
                    ProcessId = t.ProcessId,
                    LastMaintenanceTime = t.LastMaintenanceTime,
                    Remark = t.Remark,
                    // 系统字段
                    CreateBy = t.CreateBy,
                    CreateTime = t.CreateTime,
                    UpdateBy = t.UpdateBy,
                    UpdateTime = t.UpdateTime,
                    // 计算倒计时：先用安装时间加上保养周期得到下次保养日期，然后用当前日期与之相减获得倒计时天数
                    // 计算倒计时：先用上次保养时间加上保养周期得到下次保养日期，然后用当前日期与之相减获得倒计时天数
                    // 计算在数据库端执行！开销非常小，一般不会影响查询速度。
                    MaintenanceCountdown = t.LastMaintenanceTime != null && t.MaintenanceCycle != null
                        ? SqlSugar.SqlFunc.DateDiff(
                            SqlSugar.DateType.Day,
                            SqlSugar.SqlFunc.GetDate(),
                            SqlSugar.SqlFunc.DateAdd(t.LastMaintenanceTime.Value,t.MaintenanceCycle.Value,SqlSugar.DateType.Day)
                          )
                        : 0,

                })
               //.OrderBy(t => t.Id,OrderByType.Desc); // ✅ 按 ID 倒序排序
            ;
        }




       


        /// <summary>
        /// 查询分页列表 -- 设备保养 -- 按计算表达式排序(临时计算的字段)
        /// OrderBy(t => t.MaintenanceCycle)	改为按计算表达式排序	确保按实际倒计时天数排序
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleAsync(DeviceManagementDto dto)
        {
            var query = DtoQueryable(dto)
                .OrderBy(t => SqlFunc.DateDiff(
                    DateType.Day,
                    SqlFunc.GetDate(),
                    SqlFunc.DateAdd(t.LastMaintenanceTime.Value,t.MaintenanceCycle.Value,DateType.Day)
                ));

            return await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);
        }






        /// <summary>
        /// 查询分页列表  ID倒序
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleIDAsync(DeviceManagementDto dto)
        {
            // 在已有查询条件基础上，按 maintenance_cycle 升序排序
             var query = DtoQueryable(dto)
            //.WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label)) //模糊查询
            .OrderBy(t => t.Id,OrderByType.Desc); //倒序

            // PageUtils:从管道获取分页信息！
            var pagedInfo = await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);
            pagedInfo.Code = StatusCodes.Status200OK;
            return pagedInfo;
        }



        //////////////////////////////自定义SQL查询中间表////////////////////////////////


        /// <summary>
        /// 查询设备分类下的所有设备 （自定义SQL-涉及中间表）
        /// </summary>
        /// <param name="deviceTypeId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //public async Task<SqlSugarPagedList<DeviceManagement>> GetPagedListByDeviceTypeAsync(long deviceTypeId,int pageIndex,int pageSize)
        //{
        //    // 从中间表查询 device_id 列表
        //    var sql = @"SELECT device_id FROM device_management_type 
        //        WHERE devicetype_id = @DeviceTypeId";
        //    var parameters = new List<SugarParameter>
        //        {
        //            new SugarParameter("@DeviceTypeId", deviceTypeId)
        //        };

        //    // 直接以 long 类型查询（数据库中 device_id 为 bigint，对应实体 long）
        //    List<long> deviceIds = await Repo.Ado.SqlQueryAsync<long>(sql,parameters.ToArray());
        //    if(deviceIds == null || !deviceIds.Any())
        //    {
        //        return new SqlSugarPagedList<DeviceManagement>()
        //        {
        //            PageIndex = pageIndex,
        //            PageSize = pageSize,
        //            Total = 0,
        //            Rows = new List<DeviceManagement>(),
        //            Code = 0,
        //            HasPrevPages = false,
        //            HasNextPages = false
        //        };
        //    }

        //    // 构造 IN 子句字符串，例如 "100,101"
        //    var idListStr = string.Join(",",deviceIds);

        //    // 构造查询条件：直接写动态 where 子句
        //    var query = Repo.AsQueryable().Where($"id  IN ({idListStr})");

        //    var pagedResult = await query.ToPagedListAsync(pageIndex,pageSize);
        //    return pagedResult;
        //}


        public async Task<SqlSugarPagedList<DeviceManagement>> GetPagedListByDeviceTypeAsync(long deviceTypeId,int pageIndex,int pageSize)
        {
            // 从中间表查询 device_id 列表
            var sql = @"SELECT device_id FROM device_management_type WHERE devicetype_id = @DeviceTypeId";
            var parameters = new List<SugarParameter>
    {
        new SugarParameter("@DeviceTypeId", deviceTypeId)
    };

            // 查询结果直接映射为long类型列表
            List<long> deviceIds = await Repo.Ado.SqlQueryAsync<long>(sql,parameters.ToArray());

            if(deviceIds == null || !deviceIds.Any())
            {
                return new SqlSugarPagedList<DeviceManagement>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Total = 0,
                    Rows = new List<DeviceManagement>(),
                    Code = StatusCodes.Status200OK,
                    HasPrevPages = false,
                    HasNextPages = false
                };
            }

            // 改为使用 SqlSugar 内置的 Contains 进行安全的 IN 查询
            var query = Repo.AsQueryable().Where(t => deviceIds.Contains(t.Id));

            var pagedResult = await query.ToPagedListAsync(pageIndex,pageSize);
            pagedResult.Code = StatusCodes.Status200OK;

            return pagedResult;
        }




    }
}
