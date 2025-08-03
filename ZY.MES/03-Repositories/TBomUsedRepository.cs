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
    /// 完整BOM仓储
    /// </summary>
    public class TBomUsedRepository : BaseRepository<TBomUsed,TBomUsedDto>
    {
        public TBomUsedRepository(ISqlSugarRepository<TBomUsed> repo)
        {
            Repo = repo;
        }

        public override ISugarQueryable<TBomUsed> Queryable(TBomUsedDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.BomCode),x => x.BomCode.Contains(dto.BomCode))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemCode),x => x.ItemCode.Contains(dto.ItemCode));
        }

        public override ISugarQueryable<TBomUsedDto> DtoQueryable(TBomUsedDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.BomCode),x => x.BomCode.Contains(dto.BomCode))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemCode),x => x.ItemCode.Contains(dto.ItemCode))
                .Select(x => new TBomUsedDto
                {
                    Id = x.Id,
                    BomCode = x.BomCode,
                    ParentCode = x.ParentCode,
                    ItemCode = x.ItemCode,
                    Quantity = x.Quantity,
                    Level = x.Level,
                    CreateBy = x.CreateBy,
                    CreateTime = x.CreateTime,
                    UpdateBy = x.UpdateBy,
                    UpdateTime = x.UpdateTime
                });
        }
    }
}
