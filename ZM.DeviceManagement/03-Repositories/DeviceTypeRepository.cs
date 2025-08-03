using RuoYi.Common.Data;
using RuoYi.Data;
using RuoYi.Device.Entities;
using SqlSugar;

namespace ZM.Device.Repositories
{
    public class DeviceTypeRepository : BaseRepository<DeviceType,DeviceTypeDto>
    {
        public DeviceTypeRepository(ISqlSugarRepository<DeviceType> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }


        /// <summary>
        /// 所有的查询带的字段都要写在这里面才生效
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public override ISugarQueryable<DeviceType> Queryable(DeviceTypeDto dto)
        {
            return Repo.AsQueryable()
            .Where(t => t.Status == "0")    //通用筛选条件
            .Where(t => t.DelFlag == "0")   //通用筛选条件
            .WhereIF(!string.IsNullOrEmpty(dto.DeptName),t => t.DeptName == dto.DeptName) // 根据 DeptName 筛选
            .WhereIF(dto.ParentId > 0,t => t.ParentId == dto.ParentId); // 根据 ParentId 筛选
            //.Includes(t => t.SubTable)
            //.WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
            ;
        }

        public override ISugarQueryable<DeviceTypeDto> DtoQueryable(DeviceTypeDto dto)
        {
            return Repo.AsQueryable()
                .Where(t => t.Status == "0")   // 通用筛选条件
                .Where(t => t.DelFlag == "0")   // 通用筛选条件
                 //.LeftJoin<SubTable>((t, s) => t.Id == s.Id)
                 //.WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
                .Select((t) => new DeviceTypeDto
                {
                    Id = t.Id,
                    ParentId = t.ParentId,
                    DeptName = t.DeptName,
                    OrderNum = t.OrderNum,
                    Status = t.Status,
                    DelFlag = t.DelFlag,
                    //Children = t.Children,
                    Ancestors = t.Ancestors

                    // 通用
                    //CreateBy = t.CreateBy,
                    //CreateTime = t.CreateTime,
                    //UpdateBy = t.UpdateBy,
                    //UpdateTime = t.UpdateTime,

                });
        }



        /// <summary>
        /// 删除信息
        /// </summary>
        public async Task<int> DeleteDeptByIdAsync(long Id)
        {
            return await base.Updateable()
                  .SetColumns(col => col.DelFlag == DelFlag.Yes)
                  .Where(col => col.Id == Id)
                  .ExecuteCommandAsync();
        }


        ////////////////////////////////////////处理节点下的所有子元素////////////////////////////////////////////////////


        /// <summary>
        /// 根据ID查询所有子部门  -- 解决字符集不匹配的问题！！！utf8mb4_general_ci  和 utf8mb4_0900_ai_ci
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public async Task<List<DeviceType>> GetChildrenDeptByIdAsync(long deptId)
        {

            //var queryable = Repo.AsQueryable()
            //    .Where(d => SqlFunc.SplitIn(d.Ancestors,deptId.ToString()));
            //return await queryable.ToListAsync();

            // 使用 LIKE 查询，包含三种可能性以及精确匹配 ancestors 的逻辑
            string sql = @"
        SELECT * 
        FROM device_type 
        WHERE 
            ancestors LIKE CONCAT('%,', @Id, ',%') OR 
            ancestors LIKE CONCAT(@Id, ',%') OR 
            ancestors LIKE CONCAT('%,', @Id) OR 
            ancestors = @Id";

            // 执行 SQL 查询并返回结果
            var list = await Repo.Ado.SqlQueryAsync<DeviceType>(sql,new { Id = deptId });

            return list;
        }



        ////////////////////////////////////////校验工具////////////////////////////////////////////////////



        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasChildByDeptIdAsync(long parentDeptId)
        {
            var query = new DeviceTypeDto { DelFlag = DelFlag.No,ParentId = parentDeptId };
            return await base.AnyAsync(query);//是否符合条件的数据
        }


 


        /// <summary>
        /// 当前节点是否存在设备
        /// 检查指定设备类型ID是否在中间表中已被使用
        /// </summary>
        /// <param name="deviceTypeId">设备类型ID</param>
        /// <returns>存在返回true，否则返回false</returns>
        public async Task<bool> CheckDeviceTypeExistInManagementTypeAsync(long deviceTypeId)
        {
            // 原生SQL，根据实际表名、字段名替换
            var sql = @"SELECT COUNT(1) 
                FROM device_management_type 
                WHERE devicetype_id = @deviceTypeId";
             
            // 构建参数，避免SQL注入
            var parameters = new List<SugarParameter>
            {
                new SugarParameter("@deviceTypeId", deviceTypeId)
            };

            // 执行查询，获取计数
            // SqlSugar 库自带的异步方法，用于执行原生 SQL 并返回单个结果。
            // < int > 代表返回的类型
            int count = await Repo.Ado.SqlQuerySingleAsync<int>(sql,parameters.ToArray());

            // 大于0表示存在
            return count > 0;
 
        }



    }
}
