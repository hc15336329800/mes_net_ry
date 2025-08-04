using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using ZY.MES._03_Repositories;
using ZY.MES._04_Entities;
using ZY.MES._05_Dtos;
using SqlSugar;
using Microsoft.AspNetCore.Mvc;
using RuoYi.Framework;

namespace ZY.MES._02_Services
{
    /// <summary>
    /// 完整BOM服务
    /// </summary>
    public class TBomUsedService : BaseService<TBomUsed,TBomUsedDto>
    {
        private readonly ILogger<TBomUsedService> _logger;
        private readonly TBomUsedRepository _repository;
        private readonly MesItemStockRepository _itemRepository;

        public TBomUsedService(ILogger<TBomUsedService> logger,TBomUsedRepository repository,MesItemStockRepository itemRepository)
        {
            _logger = logger;
            _repository = repository;
            _itemRepository = itemRepository;

            BaseRepo = repository;
        }

        /// <summary>
        /// 构建指定BOM的用料树
        /// </summary>
        /// <param name="bomNo">BOM编号</param>
        /// <returns>根节点集合</returns>
        public async Task<List<UseItemTreeResp>> GetItemUseTreeAsync(string bomNo)
        {
            if(string.IsNullOrWhiteSpace(bomNo))
            {
                throw new ArgumentException("bomNo is required",nameof(bomNo));
            }

            // 查询指定 BOM 的用料清单
            var usedList = await _repository.Repo.AsQueryable()
                .Where(x => x.BomNo == bomNo)
                .Select(x => new TBomUsedDto
                {
                    Id = x.Id,
                    ItemNo = x.ItemNo,
                    BomNo = x.BomNo,
                    ParentCode = x.ParentCode,
                    UseItemNo = x.UseItemNo,
                    FixedUsed = x.FixedUsed,
                    UseItemType = x.UseItemType
                })
                .ToListAsync();

            if(usedList == null || usedList.Count == 0)
            {
                return new List<UseItemTreeResp>();
            }

            // 查询物料名称映射
            var useItemNos = usedList.Select(u => u.UseItemNo).ToHashSet();

            var itemInfos = await _itemRepository.Repo.AsQueryable()
                .In(x => x.ItemNo,useItemNos.ToArray())
                .Select(x => new { x.ItemNo,x.ItemName })
                .ToListAsync();

            var itemNameMap = itemInfos.ToDictionary(x => x.ItemNo,x => x.ItemName);


            // 构建中间节点映射
            var map = new Dictionary<string,UseItemTreeResp>();
            foreach(var bom in usedList)
            {
                 var node = new UseItemTreeResp
                {
                     UsedId = bom.Id,
                     ItemNo = bom.UseItemNo,
                     ParentCode = bom.ParentCode,
                     FixedUsed = bom.FixedUsed,
                     BomNo = bom.BomNo,
                     ItemType = bom.UseItemType,
                     ItemName = itemNameMap.TryGetValue(bom.UseItemNo,out var name) ? name : null,
                     Children = new List<UseItemTreeResp>()
                 };

                map[bom.UseItemNo] = node;
            }

            // 构建树形结构并确定根节点
            var roots = new List<UseItemTreeResp>();
            foreach(var bom in usedList)
            {
                var current = map[bom.UseItemNo];
                var parentCode = bom.ParentCode;

                if(string.IsNullOrWhiteSpace(parentCode)
                       || parentCode == bom.UseItemNo
                       || !map.ContainsKey(parentCode))
                {
                    roots.Add(current);

                }
                else
                {
                    map[parentCode].Children.Add(current);
                }

            }

            return roots;

        }



    }
}
