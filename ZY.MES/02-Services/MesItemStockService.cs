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
    public class MesItemStockService : BaseService<MesItemStock,MesItemStockDto>
    {
        private readonly ILogger<MesItemStockService> _logger;
        private readonly MesItemStockRepository _repository;

        public MesItemStockService(ILogger<MesItemStockService> logger,MesItemStockRepository repository)
        {
            _logger = logger;
            _repository = repository;
            BaseRepo = repository;
        }
    }
}
