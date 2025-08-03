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
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ParentCode),x => x.ParentCode.Contains(dto.ParentCode))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemCode),x => x.ItemCode.Contains(dto.ItemCode));
        }

        public override ISugarQueryable<MesItemUseDto> DtoQueryable(MesItemUseDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ParentCode),x => x.ParentCode.Contains(dto.ParentCode))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemCode),x => x.ItemCode.Contains(dto.ItemCode))
                .Select(x => new MesItemUseDto
                {
                    Id = x.Id,
                    ParentCode = x.ParentCode,
                    ItemCode = x.ItemCode,
                    Quantity = x.Quantity,
                    CreateBy = x.CreateBy,
                    CreateTime = x.CreateTime,
                    UpdateBy = x.UpdateBy,
                    UpdateTime = x.UpdateTime
                });
        }
    }
}
