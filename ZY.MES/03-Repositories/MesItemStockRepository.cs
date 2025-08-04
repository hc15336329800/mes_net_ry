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
            return Repo.AsQueryable()
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemNo),x => x.ItemNo.Contains(dto.ItemNo))
                .WhereIF(!string.IsNullOrWhiteSpace(dto.ItemName),x => x.ItemName.Contains(dto.ItemName))
                .Select(x => new MesItemStockDto
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
                    CreatedBy = x.CreatedBy,
                    CreatedTime = x.CreatedTime,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedTime = x.UpdatedTime,
                    ErpCount = x.ErpCount,
                    NetWeight = x.NetWeight,
                    IsValid = x.IsValid,
                    UniId = x.UniId
                });
        }
    }
}
