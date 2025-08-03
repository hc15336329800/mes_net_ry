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
               .WhereIF(!string.IsNullOrWhiteSpace(dto.BomNo),x => x.BomNo.Contains(dto.BomNo))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.UseItemNo),x => x.UseItemNo.Contains(dto.UseItemNo));
        }

        public override ISugarQueryable<TBomUsedDto> DtoQueryable(TBomUsedDto dto)
        {
            return Repo.AsQueryable()
              .WhereIF(!string.IsNullOrWhiteSpace(dto.BomNo),x => x.BomNo.Contains(dto.BomNo))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.UseItemNo),x => x.UseItemNo.Contains(dto.UseItemNo))
                .Select(x => new TBomUsedDto
                {
                    Id = x.Id,
                    ItemNo = x.ItemNo,
                    BomNo = x.BomNo,
                    ParentCode = x.ParentCode,
                    UseItemNo = x.UseItemNo,
                    FixedUsed = x.FixedUsed,
                    UseItemType = x.UseItemType,
                    CreateBy = x.CreateBy,
                    CreateTime = x.CreateTime,
                    UpdateBy = x.UpdateBy,
                    UpdateTime = x.UpdateTime
                });
        }
    }
}
