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
            BaseRepo = repository;
        }

        /// <summary>
        /// 构建指定物料的用料树
        /// </summary>
        /// <param name="itemNo">根物料编号</param>
        /// <returns>用料树</returns>
        public async Task<UseItemTreeResp> GetItemUseTreeAsync(string itemNo)
        {
            if(string.IsNullOrWhiteSpace(itemNo))
            {
                throw new ArgumentException("itemNo is required",nameof(itemNo));
            }

            // 一次性查询所有用料关系
            var usedList = await _repository.Repo.AsQueryable().ToListAsync();

            if(usedList == null || usedList.Count == 0)
            {
                throw new Exception("no data");
            }

            // 收集涉及到的所有物料编号
            var codes = usedList.Select(u => u.ItemCode)
                .Concat(usedList.Select(u => u.ParentCode ?? string.Empty))
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToList();

            var itemInfos = await _itemRepository.Repo.AsQueryable()
                .In(x => x.ItemCode,codes)
                .ToListAsync();

            var itemDict = itemInfos.ToDictionary(x => x.ItemCode,x => x);
            var lookup = usedList.GroupBy(u => u.ParentCode)
                .ToDictionary(g => g.Key,g => g.ToList());

            var visited = new HashSet<string>();

            UseItemTreeResp BuildNode(string code)
            {
                visited.Add(code);
                var node = new UseItemTreeResp
                {
                    ItemNo = code,
                    ParentCode = code,
                    ItemName = itemDict.TryGetValue(code,out var item) ? item.ItemName : null,
                    FixedUsed = 1,
                    BomNo = lookup.TryGetValue(code,out var firstChild) && firstChild.Any() ? firstChild.First().BomCode : null
                };

                if(lookup.TryGetValue(code,out var children))
                {
                    foreach(var child in children)
                    {
                        if(visited.Contains(child.ItemCode))
                            continue;

                        var childNode = BuildNode(child.ItemCode);
                        childNode.ParentCode = child.ParentCode;
                        childNode.FixedUsed = child.Quantity;
                        childNode.UsedId = child.Id;
                        childNode.BomNo = child.BomCode;
                        node.Children.Add(childNode);
                    }
                }

                visited.Remove(code);
                return node;
            }

            var root = BuildNode(itemNo);
            if(root.Children.Count == 0 && !lookup.ContainsKey(itemNo))
            {
                throw new Exception("tree build failed");
            }
            return root;
        }



    }
}
