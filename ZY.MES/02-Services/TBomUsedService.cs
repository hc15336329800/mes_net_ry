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
    /// 完整BOM服务
    /// </summary>
    public class TBomUsedService : BaseService<TBomUsed,TBomUsedDto>
    {
        private readonly ILogger<TBomUsedService> _logger;
        private readonly TBomUsedRepository _repository;

        public TBomUsedService(ILogger<TBomUsedService> logger,TBomUsedRepository repository)
        {
            _logger = logger;
            _repository = repository;
            BaseRepo = repository;
        }
    }
}
