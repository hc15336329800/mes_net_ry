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

namespace ZY.MES._02_Services
{

    /// <summary>
    /// 物料服务
    /// </summary>
    public class TItemStockService : BaseService<TItemStock,TItemStockDto>
    {
        private readonly ILogger<TItemStockService> _logger;
        private readonly TItemStockRepository _repository;

        public TItemStockService(ILogger<TItemStockService> logger,TItemStockRepository repository)
        {
            _logger = logger;
            _repository = repository;
            BaseRepo = repository;
        }


        /// <summary>
        /// 根据主键获取库存信息
        /// </summary>
        public async Task<TItemStockDto?> GetAsync(String id)
        {
            return await _repository
                .DtoQueryable(new TItemStockDto())
                .Where(x => x.Id == id)
                .FirstAsync();
        }
    }
}
