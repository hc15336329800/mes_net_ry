using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Common.Data;
using SqlSugar;
using ZY.MES._04_Entities;
using ZY.MES._05_Dtos;

namespace ZY.MES._03_Repositories
{
    /// <summary>
    /// 物料表仓储
    /// </summary>
    public class MesItemStockRepository : BaseRepository<MesItemStock,MesItemStockDto>
    {
        public MesItemStockRepository(ISqlSugarRepository<MesItemStock> repo)
        {
            Repo = repo;
        }

        public override ISugarQueryable<MesItemStock> Queryable(MesItemStockDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemNo),x => x.ItemNo.Contains(dto.ItemNo))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemName),x => x.ItemName.Contains(dto.ItemName))
                .WhereIF(dto.ItemCount.HasValue,x => x.ItemCount == dto.ItemCount)
                .WhereIF(dto.CreatedTime.HasValue,x => x.CreatedTime >= dto.CreatedTime)
                .WhereIF(dto.UpdatedTime.HasValue,x => x.UpdatedTime <= dto.UpdatedTime);
        }

        public override ISugarQueryable<MesItemStockDto> DtoQueryable(MesItemStockDto dto)
        {
            // 第一步：使用 Repo.AsQueryable 创建查询基础
            var query = Repo.AsQueryable();


 
            // 第二步：根据 dto.ItemNo 添加查询条件
            if(!string.IsNullOrWhiteSpace(dto.ItemNo))
            {
                query = query.Where(x => x.ItemNo.Contains(dto.ItemNo));
            }
 
            if(!string.IsNullOrWhiteSpace(dto.BomNo))
            {
                query = query.Where(x => x.BomNo.Contains(dto.BomNo));
            }

            // 第三步：根据 dto.ItemName 添加查询条件
            if(!string.IsNullOrWhiteSpace(dto.ItemName))
            {
                query = query.Where(x => x.ItemName.Contains(dto.ItemName));
            }

            //  根据 itemType  添加查询条件
            query = query.Where(x => x.ItemType == dto.ItemType);

            // 打印出生成的 SQL 查询语句和查询参数
            var sqlQuery = query.ToSql();
            Console.WriteLine("Generated SQL Query: ");
            Console.WriteLine(sqlQuery.Key);  // 输出生成的 SQL 查询语句（Key 是 SQL 查询）

            Console.WriteLine("Parameters: ");
            foreach(var param in sqlQuery.Value)  // Value 是参数列表
            {
                Console.WriteLine($"{param.ParameterName}: {param.Value}");  // 输出 SQL 查询参数
            }

            // 第四步：选择需要的字段并映射到 MesItemStockDto
            var result = query.Select(x => new MesItemStockDto
            {
                Id = x.Id,
                ItemNo = x.ItemNo,
                ItemName = x.ItemName,
                ItemModel = x.ItemModel,
                ItemCount = x.ItemCount,
                ItemMeasure = x.ItemMeasure,
                ItemCountAssist = x.ItemCountAssist,
                ItemMeasureAssist = x.ItemMeasureAssist,
                BomNo = x.BomNo,
                ItemType = x.ItemType,
                Location = x.Location,
                Remark = x.Remark,
                CreateBy = x.CreatedBy, // 注意字段
                CreateTime = x.CreatedTime,
                UpdateBy = x.UpdatedBy,
                UpdateTime = x.UpdatedTime,
                ErpCount = x.ErpCount,
                NetWeight = x.NetWeight,
                IsValid = x.IsValid,
                UniId = x.UniId
            },true);

            // 最后，返回查询结果
            return result;
        }




    }
}
