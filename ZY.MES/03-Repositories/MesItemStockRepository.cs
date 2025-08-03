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
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemCode),x => x.ItemCode.Contains(dto.ItemCode))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemName),x => x.ItemName.Contains(dto.ItemName));
        }

        public override ISugarQueryable<MesItemStockDto> DtoQueryable(MesItemStockDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemCode),x => x.ItemCode.Contains(dto.ItemCode))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemName),x => x.ItemName.Contains(dto.ItemName))
                .Select(x => new MesItemStockDto
                {
                    Id = x.Id,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    Spec = x.Spec,
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    CreateBy = x.CreateBy,
                    CreateTime = x.CreateTime,
                    UpdateBy = x.UpdateBy,
                    UpdateTime = x.UpdateTime
                });
        }
    }
}
