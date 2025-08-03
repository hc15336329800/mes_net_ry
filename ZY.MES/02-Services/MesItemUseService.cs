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
    /// 一级BOM服务
    /// </summary>
    public class MesItemUseService : BaseService<MesItemUse,MesItemUseDto>
    {
        private readonly ILogger<MesItemUseService> _logger;
        private readonly MesItemUseRepository _repository;

        public MesItemUseService(ILogger<MesItemUseService> logger,MesItemUseRepository repository)
        {
            _logger = logger;
            _repository = repository;
            BaseRepo = repository;
        }
    }
}
