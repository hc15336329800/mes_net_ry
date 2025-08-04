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
using RuoYi.Data.Tool;


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

        public TBomUsedService(
            ILogger<TBomUsedService> logger,
            TBomUsedRepository repository,
            MesItemStockRepository itemRepository)
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


        /// <summary>
        /// 根据前端传递的一级用料数据重新构建完整BOM
        /// </summary>
        /// <param name="uses">一级用料列表</param>
        public async Task LoadBomDataAsync(List<MesItemUseDto> uses)
        {
            if(uses == null || uses.Count == 0)
            {
                throw new ArgumentException("uses is required",nameof(uses));
            }

            // 推断根物料编码：父节点集合减去子节点集合
            var parentSet = uses.Select(x => x.ItemNo).ToHashSet();
            var childSet = uses.Select(x => x.UseItemNo).ToHashSet();
            var itemNo = parentSet.Except(childSet).FirstOrDefault() ?? uses[0].ItemNo;

            // 查询所需物料信息
            var allNos = parentSet.Union(childSet).Append(itemNo).Distinct().ToArray();
            var itemInfos = await _itemRepository.Repo.AsQueryable()
                .In(x => x.ItemNo,allNos)
                .Select(x => new { x.ItemNo,x.ItemType,x.BomNo })
                .ToListAsync();
            var typeMap = itemInfos.ToDictionary(x => x.ItemNo,x => x.ItemType);
            var bomNo = itemInfos.FirstOrDefault(x => x.ItemNo == itemNo)?.BomNo ?? itemNo;

            var childrenMap = uses
                .GroupBy(x => x.ItemNo)
                .ToDictionary(g => g.Key,g => g.ToList());

            var result = new List<TBomUsed>();
            var now = DateTime.Now;

            void FindChildUse(string parentItemNo,decimal parentCount,string path)
            {
                if(!childrenMap.TryGetValue(parentItemNo,out var children))
                {
                    return;
                }

                foreach(var child in children)
                {
                    // 优先使用前端传入的用料类型,否则回退到物料档案中的类型
                    var childType = child.UseItemType ?? (typeMap.TryGetValue(child.UseItemNo,out var t) ? t : null); var useCount = (child.UseItemCount ?? 0m) * parentCount;
                    var childPath = string.IsNullOrEmpty(path) ? child.UseItemNo : $"{path}|{child.UseItemNo}";

                    result.Add(new TBomUsed
                    {
                        Id = NextId.Id13().ToString(),
                        ItemNo = itemNo,
                        BomNo = bomNo,
                        UseItemNo = child.UseItemNo,
                        UseItemCount = useCount,
                        UseItemType = childType,
                        ParentCode = parentItemNo,
                        ItemNos = childPath,
                        UsedId = child.Id,
                        FixedUsed = useCount,
                        CreatedTime = now,
                        UpdatedTime = now
                    });

                    if(childType == "01")
                    {
                        FindChildUse(child.UseItemNo,useCount,childPath);
                    }
                }
            }

            FindChildUse(itemNo,1m,itemNo);

            // 添加自身依赖节点
            // 如果根节点尚未存在则补充一条自身依赖记录
            bool rootExists = await _repository.Repo.AsQueryable()
                .AnyAsync(x => x.ItemNo == itemNo && x.UseItemNo == itemNo);
            if(!rootExists)
            {
                result.Add(new TBomUsed
                {
                    Id = NextId.Id13().ToString(),
                    ItemNo = itemNo,
                    BomNo = bomNo,
                    UseItemNo = itemNo,
                    UseItemCount = 1m,
                    UseItemType = typeMap.TryGetValue(itemNo,out var rootType) ? rootType : null,
                    ParentCode = itemNo,
                    ItemNos = itemNo,
                    FixedUsed = 1m,
                    CreatedTime = now,
                    UpdatedTime = now
                });
            }

            // 同一父子组合去重并合并数量
            var deduped = result
                .GroupBy(x => new { x.ParentCode,x.UseItemNo })
                .Select(g =>
                {
                    var first = g.First();
                    first.UseItemCount = g.Sum(x => x.UseItemCount);
                    first.FixedUsed = g.Sum(x => x.FixedUsed ?? 0m);
                    return first;
                })
                .ToList();

            // 为避免重复,先删除即将插入的父子组合对应的旧记录
            foreach(var node in deduped)
            {
                await _repository.Repo.DeleteAsync(x => x.ItemNo == itemNo && x.ParentCode == node.ParentCode && x.UseItemNo == node.UseItemNo);
            }

            // 批量写入新增或更新的记录
            if(deduped.Count > 0)
            {
                await _repository.Repo.InsertAsync(deduped);
            }
        }


    }
}
