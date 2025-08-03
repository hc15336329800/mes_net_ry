using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Common.Data;
using RuoYi.Device.Entities;
using SqlSugar;

namespace ZM.Device.Repositories
{
    /// <summary>
    /// 设别和类型中间表
    /// </summary>
    public class DeviceManagementTypeRepository : BaseRepository<DeviceManagementType,DeviceManagementTypeDto>
    {
        public DeviceManagementTypeRepository(ISqlSugarRepository<DeviceManagementType> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceManagementType> Queryable(DeviceManagementTypeDto dto)
        {
            return Repo.AsQueryable();
        }

        public override ISugarQueryable<DeviceManagementTypeDto> DtoQueryable(DeviceManagementTypeDto dto)
        {
            return Repo.AsQueryable().Select(t => new DeviceManagementTypeDto
            {
                DeviceId = t.DeviceId,
                DeviceTypeId = t.DeviceTypeId,
                TenantId = t.TenantId
            });
        }

    }
}
