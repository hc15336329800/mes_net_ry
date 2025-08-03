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
    /// 一级BOM仓储
    /// </summary>
    public class MesItemUseRepository : BaseRepository<MesItemUse,MesItemUseDto>
    {
        public MesItemUseRepository(ISqlSugarRepository<MesItemUse> repo)
        {
            Repo = repo;
        }

        public override ISugarQueryable<MesItemUse> Queryable(MesItemUseDto dto)
        {
            return Repo.AsQueryable()
               .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemNo),x => x.ItemNo.Contains(dto.ItemNo))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.UseItemNo),x => x.UseItemNo.Contains(dto.UseItemNo));
        }

        public override ISugarQueryable<MesItemUseDto> DtoQueryable(MesItemUseDto dto)
        {
            return Repo.AsQueryable()
               .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemNo),x => x.ItemNo.Contains(dto.ItemNo))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.UseItemNo),x => x.UseItemNo.Contains(dto.UseItemNo))
                .Select(x => new MesItemUseDto
                {
                    Id = x.Id,
                    ItemNo = x.ItemNo,
                    UseItemNo = x.UseItemNo,
                    UseItemCount = x.UseItemCount,
              
                });
        }
    }
}
