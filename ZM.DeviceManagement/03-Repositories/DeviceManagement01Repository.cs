using System.Text;
using Mapster;
using Microsoft.AspNetCore.Http;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using RuoYi.Device.Entities;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;



///  
/// 封装版  、链式版、原生SQL版 三种写法实例
/// 



namespace ZM.Device.Repositories
{
    /// <summary>
    /// 设备管理 01  --  单/多表查询模版
    /// </summary>
    public class DeviceManagement01Repository : BaseRepository<DeviceManagement,DeviceManagementDto>
    {
        /// <summary>
        /// 🔵 注入
        /// </summary>
        /// <param name="sqlSugarRepository"></param>
        public DeviceManagement01Repository(ISqlSugarRepository<DeviceManagement> sqlSugarRepository)
        {
            // 注意：Repo 只能针对 DeviceManagement 表进行标准操作（如 Insert, Update, Queryable）但是 不能在 Repo 上做通用型的 SqlQueryable<T>() 
            Repo = sqlSugarRepository;
        }


        ///////////////////////////////////////// 强制实现接口 ///////////////////////////////////////////////


        /// <summary>
        /// 🔵 返回 TEntity 的查询对象 （必须实现BaseRepository中的Queryable方法）
        /// 🔵 场景：需要完整原始数据、做业务处理（增删改）时使用！增删改查询，只关注是否查到，不关心数据顺序，无加 OrderBy
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public override ISugarQueryable<DeviceManagement> Queryable(DeviceManagementDto dto)
        {
            // 你可以根据 dto 的某个字段判断用哪个查询逻辑
            return dto.Params.QueryType switch
            {
                 "RunDev" => BuilQueryByStatus(dto), //筛选设备状态“运行”
                 _ => BuilQuery(dto) // 默认普通查询
            };

        }

 

        /// <summary>
        /// 🔵 必须实现BaseRepository中的DtoQueryable方法
        /// 🔵 场景：只查你前端页面需要的字段，提高查询性能！
        /// </summary>
        public override ISugarQueryable<DeviceManagementDto> DtoQueryable(DeviceManagementDto dto)
        {
            // 你可以根据 dto 的某个字段判断用哪个查询逻辑
            return dto.Params.QueryType switch
            {
                "Countdown" => BuildMaintenanceCountdownQuery(dto), //计算保养倒计时字段
                "SpecialType" => BuildSpecialTypeQuery(dto), //筛选设备状态
                "GetRunningAndFaultDeviceCountAsync" => GetRunningAndFaultDeviceCountAsync(dto), //查询数据库中 Status 是 "运行" 或 "故障" 的设备总数

                "LinkedTable" => BuildLinkedTableQuery(dto), //连表查询
                _ => BuildNormalDeviceQuery(dto) // 默认普通查询
            };
        }


        /////////////////////////////////////////零、Queryable的动态选择 ///////////////////////////////////////////////

        /// <summary>
        /// 🔵 普通单表查询逻辑
        /// </summary>
        private ISugarQueryable<DeviceManagement> BuilQuery(DeviceManagementDto dto)
        {
            return Repo.AsQueryable()
            .WhereIF(dto.Id > 0,t => t.Id == dto.Id);    // 根据 Id 查询
        }


        /// <summary>
        /// 🔵 普通单表查询逻辑
        /// </summary>
        private ISugarQueryable<DeviceManagement> BuilQueryByStatus(DeviceManagementDto dto)
        {
            return Repo.AsQueryable()
            .WhereIF(string.IsNullOrWhiteSpace(dto.Status),t => t.Status == "运行")  // 🔥 如果空，则默认正在运行的设备
            .WhereIF(dto.Id > 0,t => t.Id == dto.Id);    // 根据 Id 查询
        }



        /////////////////////////////////////////零、DtoQueryable的动态选择 ///////////////////////////////////////////////

        /// <summary>
        /// 🔵 普通单表查询逻辑
        /// </summary>
        private ISugarQueryable<DeviceManagementDto> BuildNormalDeviceQuery(DeviceManagementDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label)) // 🔵 按设备名称模糊
                .WhereIF(dto.Params?.BeginTime != null,t => t.CreateTime >= dto.Params.BeginTime) // 🔵 按创建时间区间起始
                .WhereIF(dto.Params?.EndTime != null,t => t.CreateTime <= dto.Params.EndTime)     // 🔵 按创建时间区间结束
                .WhereIF(dto.ProcessId > 0,t => t.ProcessId == dto.ProcessId) // 🔵 如果传了工序ID
                .Select(t => new DeviceManagementDto
                {
                    Id = t.Id,
                    Label = t.Label,
                    DeviceType = t.DeviceType,
                    Model = t.Model,
                    Quantity = t.Quantity,
                    Status = t.Status,
                    CreateTime = t.CreateTime,
                    UpdateTime = t.UpdateTime
                })
                .OrderBy(t => t.Id,OrderByType.Desc); // 🔵 默认按ID倒序（最新的在前）
        }


        /// <summary>
        /// 🔵 保养倒计时 查询逻辑
        /// </summary>
        private ISugarQueryable<DeviceManagementDto> BuildMaintenanceCountdownQuery(DeviceManagementDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label)) // 名称模糊
                .WhereIF(true,t => t.LastMaintenanceTime != null && t.MaintenanceCycle != null)          // 必须有上次保养时间和维护周期
                .Select(t => new DeviceManagementDto
                {
                    Id = t.Id,
                    Label = t.Label,
                    LastMaintenanceTime = t.LastMaintenanceTime,
                    MaintenanceCycle = t.MaintenanceCycle,
                    CreateTime = t.CreateTime,
                    // 🔥 计算倒计时字段：数据库端完成，不占用内存
                    MaintenanceCountdown = SqlFunc.DateDiff(
                        DateType.Day,
                        SqlFunc.GetDate(),
                        SqlFunc.DateAdd(t.LastMaintenanceTime.Value,t.MaintenanceCycle.Value,DateType.Day)
                    )
                })
                .OrderBy(t => t.MaintenanceCountdown); // ✅ 默认按倒计时升序排列（快到期的排前面）
        }


        /// <summary>
        /// 🔵 特殊类型 查询逻辑（比如筛选设备状态）
        /// </summary>
        private ISugarQueryable<DeviceManagementDto> BuildSpecialTypeQuery(DeviceManagementDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(true,t => t.Status == "运行") // 🔥 特定状态，比如只要正在运行的设备
                .Select(t => new DeviceManagementDto
                {
                    Id = t.Id,
                    Label = t.Label,
                    Status = t.Status,
                    CreateTime = t.CreateTime,
                    Manufacturer = t.Manufacturer
                })
                .OrderBy(t => t.CreateTime,OrderByType.Desc); // ✅ 默认按创建时间倒序
        }



        /// <summary>
        /// 🔵 查询数据库中 Status 是 "运行" 或 "故障" 的设备列表
        /// </summary>
        private ISugarQueryable<DeviceManagementDto> GetRunningAndFaultDeviceCountAsync(DeviceManagementDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(true,t => new[] { "运行","故障" }.Contains(t.Status)) // 🔥 IN 查询
                .Select(t => new DeviceManagementDto
                {
                    Id = t.Id,
                    Label = t.Label,
                    Status = t.Status,
                    CreateTime = t.CreateTime,
                    Manufacturer = t.Manufacturer
                })
                .OrderBy(t => t.CreateTime,OrderByType.Desc); // ✅ 默认按创建时间倒序
        }
      


        /// <summary>
        /// 🔵 链表查询逻辑：设备表 LeftJoin 设备分类  多表查询
        /// </summary>
        private ISugarQueryable<DeviceManagementDto> BuildLinkedTableQuery(DeviceManagementDto dto)
        {
            return Repo.Context.Queryable<DeviceManagement>() // 必须用 Context，因为涉及多表
                .LeftJoin<DeviceManagementType>((device,type) => device.Id == type.DeviceId) // 左连接
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),device => device.Label.Contains(dto.Label)) // 主表条件
                .WhereIF(dto.ProcessId > 0,device => device.ProcessId == dto.ProcessId) // 主表条件
                .Select((device,type) => new DeviceManagementDto
                {
                    Id = device.Id,
                    Label = device.Label,
                    DeviceType = device.DeviceType,
                    Quantity = device.Quantity,
                    ProcessId = device.ProcessId,
                    CreateTime = device.CreateTime,
                    // 🔥 把子表的分类ID带出来
                    //（如果需要的话，你DTO里可以新增字段接收）
                    //DeviceTypeId = type.DeviceTypeId
                })
                .OrderBy(device => device.CreateTime,OrderByType.Desc); // 默认按创建时间倒序
        }



        /////////////////////////////////////////零、DtoQueryable的基础查询逻辑+额外的条件 ///////////////////////////////////////////////


        ///
        /// 注意点： 
        /// 问：什么情况下，框架回自己调用 DtoQueryable()？ 那有没有特殊情况，可能“像”自动调用？
        /// 答：有一种情况，会让你以为是自动调用的，但其实本质还是手动调用：你在某些 Service 层方法里，统一调用了 GetDtoPagedListAsync()、GetDtoListAsync() 这类方法，这些方法内部封装了 DtoQueryable()
        ///


        /// <summary>
        /// 查询分页列表，按创建时间降序排序 - App端
        /// </summary>
        /// <param name="dto">查询条件</param>
        /// <returns>分页结果</returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListAppAsync(DeviceManagementDto dto)
        {
 

            // 获取基础查询对象
            var query = DtoQueryable(dto);

            // 补充：如果 DeviceType 为空，则默认筛选打磨、切割
            if(string.IsNullOrWhiteSpace(dto.DeviceType))
            {
                query = query.Where(it => new[] { "打磨","切割" }.Contains(it.DeviceType));
            }

            // 获取分页数据
            var pagedInfo = await query
                .OrderBy(t => t.CreateTime,OrderByType.Desc)
                .ToPagedListAsync(PageUtils.GetPageDomain().PageNum,PageUtils.GetPageDomain().PageSize);

            pagedInfo.Code = StatusCodes.Status200OK;

            return pagedInfo;
        }


        /////////////////////////////////////////一、分页查询模板合集（异步）///////////////////////////////////////////////

        #region  ********** ① 【Base封装版】标准 **********



        /// <summary>
        /// 🔵 返回实体分页（DeviceManagement）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagement>> GetPageByBaseEntityAsync(DeviceManagementDto dto)
        {
            return await GetPagedListAsync(dto); //BaseRepository中封装的方法
        }


        /// <summary>
        /// 🔵 返回 DTO 分页（DeviceManagementDto）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetPageByBaseDtoAsync(DeviceManagementDto dto)
        {
            return await GetDtoPagedListAsync(dto);
        }

        #endregion


        #region ********** ② 【Repo.Context链式版】灵活 **********  


        /// <summary>
        /// 🔵 返回实体分页（DeviceManagement）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagement>> GetPageByContextEntityAsync(DeviceManagementDto dto)
        {
            var query = Repo.Context.Queryable<DeviceManagement>()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label))
                .WhereIF(dto.Id > 0,t => t.Id == dto.Id);

            var pageDomain = PageUtils.GetPageDomain();
            var pagedList = await query.ToPagedListAsync(pageDomain.PageNum,pageDomain.PageSize);

            pagedList.Code = StatusCodes.Status200OK;
            return pagedList;
        }


        /// <summary>
        /// 🔵 返回 DTO 分页（自己 Select）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetPageByContextDtoAsync(DeviceManagementDto dto)
        {
            var query = Repo.Context.Queryable<DeviceManagement>()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label))
                .Select(t => new DeviceManagementDto
                {
                    Id = t.Id,
                    Label = t.Label,
                    Status = t.Status,
                    CreateTime = t.CreateTime
                });

            var pageDomain = PageUtils.GetPageDomain();
            var pagedList = await query.ToPagedListAsync(pageDomain.PageNum,pageDomain.PageSize);

            pagedList.Code = StatusCodes.Status200OK;
            return pagedList;
        }

        #endregion


        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********  


        /// <summary>
        /// 🔵 返回实体分页（DeviceManagement）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagement>> GetPageByAdoEntityAsync(DeviceManagementDto dto)
        {
            string sql = @"SELECT * FROM device_management WHERE 1=1";
            var parameters = new List<SugarParameter>();

            if(!string.IsNullOrWhiteSpace(dto.Label))
            {
                sql += " AND label LIKE @Label";
                parameters.Add(new SugarParameter("@Label","%" + dto.Label + "%"));
            }

            var query = Repo.Context.SqlQueryable<DeviceManagement>(sql)
                .AddParameters(parameters); // 🔥 参数单独加

            var pageDomain = PageUtils.GetPageDomain();
            var pagedList = await query.ToPagedListAsync(pageDomain.PageNum,pageDomain.PageSize);

            pagedList.Code = StatusCodes.Status200OK;
            return pagedList;
        }



        /// <summary>
        /// 🔵 返回 DTO 分页（只查部分字段）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetPageByAdoDtoAsync(DeviceManagementDto dto)
        {
            string sql = @"SELECT id, label, status, create_time FROM device_management WHERE 1=1";
            var parameters = new List<SugarParameter>();

            if(!string.IsNullOrWhiteSpace(dto.Label))
            {
                sql += " AND label LIKE @Label";
                parameters.Add(new SugarParameter("@Label","%" + dto.Label + "%"));
            }

            // 正确做法：只传 sql，后面 AddParameters
            var query = Repo.Context.SqlQueryable<DeviceManagement>(sql)
                .AddParameters(parameters) // 🔥 这里补充参数
                .Select(t => new DeviceManagementDto
                {
                    Id = t.Id,
                    Label = t.Label,
                    Status = t.Status,
                    CreateTime = t.CreateTime
                });

            //从管道中取
            var pageDomain = PageUtils.GetPageDomain();
            var pagedList = await query.ToPagedListAsync(pageDomain.PageNum,pageDomain.PageSize);

            pagedList.Code = StatusCodes.Status200OK;
            return pagedList;
        }

        #endregion


        /////////////////////////////////////////一、不分页查询模板合集（异步）///////////////////////////////////////////////

        #region ********** ① 【Base封装版】标准 **********  

        /// <summary>
        /// 🔵 不分页查询 - 返回实体列表（DeviceManagement）
        /// </summary>
        public async Task<List<DeviceManagement>> GetEntityListByBaseAsync(DeviceManagementDto dto)
        {
            return await GetListAsync(dto); // BaseRepository封装的方法
        }

        /// <summary>
        /// 🔵 不分页查询 - 返回 DTO 列表（DeviceManagementDto）
        /// </summary>
        public async Task<List<DeviceManagementDto>> GetDtoListByBaseAsync(DeviceManagementDto dto)
        {
            return await GetDtoListAsync(dto); // BaseRepository封装的方法
        }
        #endregion

        #region ********** ② 【Repo.Context链式版】灵活 **********
 

        /// <summary>
        /// 🔵 不分页查询 - 返回实体列表（DeviceManagement）
        /// </summary>
        public async Task<List<DeviceManagement>> GetEntityListByContextAsync(DeviceManagementDto dto)
        {
            var query = Repo.Context.Queryable<DeviceManagement>()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label))
                .WhereIF(dto.Id > 0,t => t.Id == dto.Id);

            return await query.ToListAsync();
        }

        /// <summary>
        /// 🔵 不分页查询 - 返回 DTO 列表（DeviceManagementDto）
        /// </summary>
        public async Task<List<DeviceManagementDto>> GetDtoListByContextAsync(DeviceManagementDto dto)
        {
            var query = Repo.Context.Queryable<DeviceManagement>()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.Label),t => t.Label.Contains(dto.Label))
                .Select(t => new DeviceManagementDto
                {
                    Id = t.Id,
                    Label = t.Label,
                    Status = t.Status,
                    CreateTime = t.CreateTime
                });

            return await query.ToListAsync();
        }

        #endregion

        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********

        /// <summary>
        /// 🔵 不分页查询（原生SQL Ado版） - 返回实体列表（DeviceManagement）
        /// </summary>
        public async Task<List<DeviceManagement>> GetEntityListByAdoAsync(DeviceManagementDto dto)
        {
            string sql = @"SELECT * FROM device_management WHERE 1=1";
            var parameters = new List<SugarParameter>();

            if(!string.IsNullOrWhiteSpace(dto.Label))
            {
                sql += " AND label LIKE @Label";
                parameters.Add(new SugarParameter("@Label","%" + dto.Label + "%"));
            }

            var list = await Repo.Ado.SqlQueryAsync<DeviceManagement>(sql,parameters.ToArray()); // 🔥 用 Ado查询！

            return list;
        }


        /// <summary>
        /// 🔵 不分页查询（原生SQL Ado版） - 返回 DTO 列表（DeviceManagementDto）
        /// </summary>
        public async Task<List<DeviceManagementDto>> GetDtoListByAdoAsync(DeviceManagementDto dto)
        {
            string sql = @"SELECT id, label, status, create_time FROM device_management WHERE 1=1";
            var parameters = new List<SugarParameter>();

            if(!string.IsNullOrWhiteSpace(dto.Label))
            {
                sql += " AND label LIKE @Label";
                parameters.Add(new SugarParameter("@Label","%" + dto.Label + "%"));
            }

            var list = await Repo.Ado.SqlQueryAsync<DeviceManagementDto>(sql,parameters.ToArray()); // 🔥 用 Ado查询！

            return list;
        }

        #endregion


        /////////////////////////////////////////二、单条新增模板合集（异步）///////////////////////////////////////////////

        #region ********** ① 【Base封装版】标准 **********  

        /// <summary>
        /// 🔵 【封装版】新增单条设备（DeviceManagement）
        /// </summary>
        public async Task<bool> AddDeviceByBaseAsync(DeviceManagementDto dto)
        {
            var entity = dto.Adapt<DeviceManagement>();
            return await InsertAsync(entity);
        }
 
        #endregion

        #region ********** ② 【Repo.Context链式版】灵活 **********

        /// <summary>
        /// 🔵 【链式版】新增单条设备（DeviceManagement）
        /// </summary>
        public async Task<bool> AddDeviceByContextAsync(DeviceManagementDto dto)
        {
            var entity = dto.Adapt<DeviceManagement>();
            var result = await Repo.Context.Insertable(entity).ExecuteCommandAsync();
            return result > 0;
        }

        #endregion

        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********
 
        /// <summary>
        /// 🔵 【原生SQL版】新增单条设备（DeviceManagement）
        /// </summary>
        public async Task<bool> AddDeviceByAdoAsync(DeviceManagementDto dto)
        {
            string sql = @"INSERT INTO device_management (label, device_type, model, quantity, status, create_time)
                   VALUES (@Label, @DeviceType, @Model, @Quantity, @Status, @CreateTime)";

            var parameters = new List<SugarParameter>
            {
                new SugarParameter("@Label", dto.Label),
                new SugarParameter("@DeviceType", dto.DeviceType),
                new SugarParameter("@Model", dto.Model),
                new SugarParameter("@Quantity", dto.Quantity),
                new SugarParameter("@Status", dto.Status ?? "运行"),
                new SugarParameter("@CreateTime", DateTime.Now)
            };

            var result = await Repo.Ado.ExecuteCommandAsync(sql,parameters.ToArray());
            return result > 0;
        }


        #endregion


        /////////////////////////////////////////二、批量新增模板合集（异步）///////////////////////////////////////////////

        #region ********** ① 【Base封装版】标准 **********  

        /// <summary>
        /// 🔵 【封装版】批量新增设备列表（DeviceManagement）
        /// </summary>
        public async Task<bool> AddDevicesByBaseAsync(List<DeviceManagementDto> dtoList)
        {
            var entityList = dtoList.Adapt<List<DeviceManagement>>();
             var result = await InsertBatchAsync(entityList); // 这里 result 是 int
            return result > 0; // 🔥 判断大于0才成功
        }


        #endregion

        #region ********** ② 【Repo.Context链式版】灵活 **********

        /// <summary>
        /// 🔵 【链式版】批量新增设备列表（DeviceManagement）
        /// </summary>
        public async Task<bool> AddDevicesByContextAsync(List<DeviceManagementDto> dtoList)
        {
            var entityList = dtoList.Adapt<List<DeviceManagement>>();
            var result = await Repo.Context.Insertable(entityList).ExecuteCommandAsync();
            return result > 0;
        }

        #endregion

        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********

        /// <summary>
        /// 🔵 【原生SQL版】批量新增设备列表（DeviceManagement）
        /// </summary>
        public async Task<bool> AddDevicesByAdoAsync(List<DeviceManagementDto> dtoList)
        {
            if(dtoList == null || dtoList.Count == 0)
                return false;

            var sql = new StringBuilder();
            var parameters = new List<SugarParameter>();
            int index = 0;

            foreach(var dto in dtoList)
            {
                sql.AppendLine($@"INSERT INTO device_management (label, device_type, model, quantity, status, create_time)
                          VALUES (@Label{index}, @DeviceType{index}, @Model{index}, @Quantity{index}, @Status{index}, @CreateTime{index});");

                parameters.Add(new SugarParameter($"@Label{index}",dto.Label));
                parameters.Add(new SugarParameter($"@DeviceType{index}",dto.DeviceType));
                parameters.Add(new SugarParameter($"@Model{index}",dto.Model));
                parameters.Add(new SugarParameter($"@Quantity{index}",dto.Quantity));
                parameters.Add(new SugarParameter($"@Status{index}",dto.Status ?? "运行"));
                parameters.Add(new SugarParameter($"@CreateTime{index}",DateTime.Now));

                index++;
            }

            var result = await Repo.Ado.ExecuteCommandAsync(sql.ToString(),parameters.ToArray());
            return result > 0;
        }

        #endregion


        /////////////////////////////////////////三、单条修改模板合集（异步）///////////////////////////////////////////////


        #region ********** ① 【Base封装版】标准 **********  

        /// <summary>
        /// 🔵 【封装版】修改单条设备（DeviceManagement）
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDeviceByBaseAsync(DeviceManagementDto dto)
        {
            var entity = dto.Adapt<DeviceManagement>();
            // 🔥 新增逻辑：如果CreateTime是默认值，则强制置null，防止误更新
            //entity.CreateTime = DateTime.Now;
            var result = await UpdateAsync(entity,ignoreAllNullColumns: true); // ✅ 自动只更新你传的字段
            return result > 0; // 🔥 判断大于0才成功
        }


        #endregion


        #region ********** ② 【Repo.Context链式版】灵活 **********

        /// <summary>
        /// 🔵 【链式版】修改单条设备（DeviceManagement）
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDeviceByContextAsync(DeviceManagementDto dto)
        {
            var entity = dto.Adapt<DeviceManagement>();
            entity.CreateTime = DateTime.Now;
            var result = await Repo.Context.Updateable(entity).ExecuteCommandAsync(); // 🔥 链式Updateable
            return result > 0;
        }


        #endregion


        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********

        /// <summary>
        /// 🔵 【原生SQL版】修改单条设备（DeviceManagement）
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDeviceByAdoAsync(DeviceManagementDto dto)
        {
            string sql = @"UPDATE device_management 
                   SET label = @Label, device_type = @DeviceType, model = @Model, quantity = @Quantity, status = @Status, update_time = @UpdateTime 
                   WHERE id = @Id";

            var parameters = new List<SugarParameter>
    {
        new SugarParameter("@Label", dto.Label),
        new SugarParameter("@DeviceType", dto.DeviceType),
        new SugarParameter("@Model", dto.Model),
        new SugarParameter("@Quantity", dto.Quantity),
        new SugarParameter("@Status", dto.Status ?? "运行"),
        new SugarParameter("@UpdateTime", DateTime.Now),
        new SugarParameter("@Id", dto.Id)
    };

            var result = await Repo.Ado.ExecuteCommandAsync(sql,parameters.ToArray());
            return result > 0;
        }


        #endregion


        /////////////////////////////////////////三、批量修改模板合集（异步）///////////////////////////////////////////////


        #region ********** ① 【Base封装版】标准 **********  

        /// <summary>
        /// 🔵 【封装版】批量修改设备列表（DeviceManagement）
        /// </summary>
        /// <param name="dtoList">设备信息DTO列表</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDevicesByBaseAsync(List<DeviceManagementDto> dtoList)
        {
            var entityList = dtoList.Adapt<List<DeviceManagement>>();
            var result = await UpdateBulkAsync(entityList); // 🔥 Base封装的批量更新
            return result > 0; // 🔥 判断大于0才成功
        }


        #endregion


        #region ********** ② 【Repo.Context链式版】灵活 **********

        /// <summary>
        /// 🔵 【链式版】批量修改设备列表（DeviceManagement）
        /// </summary>
        /// <param name="dtoList">设备信息DTO列表</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDevicesByContextAsync(List<DeviceManagementDto> dtoList)
        {
            var entityList = dtoList.Adapt<List<DeviceManagement>>();
            var result = await Repo.Context.Updateable(entityList).ExecuteCommandAsync(); // 🔥 链式Updateable
            return result > 0;
        }


        #endregion


        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********

        /// <summary>
        /// 🔵 【原生SQL版】批量修改设备列表（DeviceManagement）
        /// </summary>
        /// <param name="dtoList">设备信息DTO列表</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDevicesByAdoAsync(List<DeviceManagementDto> dtoList)
        {
            if(dtoList == null || dtoList.Count == 0)
                return false;

            var sql = new StringBuilder();
            var parameters = new List<SugarParameter>();
            int index = 0;

            foreach(var dto in dtoList)
            {
                sql.AppendLine($@"
            UPDATE device_management 
            SET label = @Label{index}, device_type = @DeviceType{index}, model = @Model{index}, quantity = @Quantity{index}, status = @Status{index}, update_time = @UpdateTime{index} 
            WHERE id = @Id{index};
        ");

                parameters.Add(new SugarParameter($"@Label{index}",dto.Label));
                parameters.Add(new SugarParameter($"@DeviceType{index}",dto.DeviceType));
                parameters.Add(new SugarParameter($"@Model{index}",dto.Model));
                parameters.Add(new SugarParameter($"@Quantity{index}",dto.Quantity));
                parameters.Add(new SugarParameter($"@Status{index}",dto.Status ?? "运行"));
                parameters.Add(new SugarParameter($"@UpdateTime{index}",DateTime.Now));
                parameters.Add(new SugarParameter($"@Id{index}",dto.Id));

                index++;
            }

            var result = await Repo.Ado.ExecuteCommandAsync(sql.ToString(),parameters.ToArray());
            return result > 0;
        }


        #endregion

        /////////////////////////////////////////四、单条删除模板合集（异步）///////////////////////////////////////////////


        #region ********** ① 【Base封装版】标准 **********  

        /// <summary>
        /// 🔵 【封装版】删除单条设备（DeviceManagement）
        /// </summary>
        /// <param name="id">设备主键ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDeviceByBaseAsync(long id)
        {
            var result = await Repo.DeleteAsync(id); // ✅ Repo本身提供 DeleteAsync
            return result > 0;
        }



        #endregion


        #region ********** ② 【Repo.Context链式版】灵活 **********

        /// <summary>
        /// 🔵 【链式版】删除单条设备（DeviceManagement）
        /// </summary>
        /// <param name="id">设备主键ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDeviceByContextAsync(long id)
        {
            var result = await Repo.Context.Deleteable<DeviceManagement>()
                .Where(t => t.Id == id)
                .ExecuteCommandAsync();
            return result > 0;
        }


        #endregion


        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********

        /// <summary>
        /// 🔵 【原生SQL版】删除单条设备（DeviceManagement）
        /// </summary>
        /// <param name="id">设备主键ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDeviceByAdoAsync(long id)
        {
            string sql = @"DELETE FROM device_management WHERE id = @Id";
            var parameters = new SugarParameter[]
            {
        new SugarParameter("@Id", id)
            };

            var result = await Repo.Ado.ExecuteCommandAsync(sql,parameters);
            return result > 0;
        }


        #endregion

        /////////////////////////////////////////四、批量删除模板合集（异步）///////////////////////////////////////////////


        #region ********** ① 【Base封装版】标准 **********  

        /// <summary>
        /// 🔵 【封装版】批量删除设备（DeviceManagement）
        /// </summary>
        /// <param name="ids">设备主键ID列表</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDevicesByBaseAsync(List<long> ids)
        {
            var result = await Repo.DeleteAsync(ids); // ✅ Repo也支持批量 DeleteAsync
             return result > 0;
        }



        #endregion


        #region ********** ② 【Repo.Context链式版】灵活 **********

        /// <summary>
        /// 🔵 【链式版】批量删除设备（DeviceManagement）
        /// </summary>
        /// <param name="ids">设备主键ID列表</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDevicesByContextAsync(List<long> ids)
        {
            var result = await Repo.Context.Deleteable<DeviceManagement>()
                .In(ids)
                .ExecuteCommandAsync();
            return result > 0;
        }


        #endregion


        #region ********** ③ 【Repo.Ado原生SQL版】极度自由 **********

        /// <summary>
        /// 🔵 【原生SQL版】批量删除设备（DeviceManagement）
        /// </summary>
        /// <param name="ids">设备主键ID列表</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDevicesByAdoAsync(List<long> ids)
        {
            if(ids == null || ids.Count == 0)
                return false;

            string idList = string.Join(",",ids);
            string sql = $"DELETE FROM device_management WHERE id IN ({idList})";

            var result = await Repo.Ado.ExecuteCommandAsync(sql);
            return result > 0;
        }


        #endregion
    }
}
