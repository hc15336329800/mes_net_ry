using RuoYi.Common.Data;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    /// <summary>
    ///  设备信息与设备保养中间表 Repository
    ///  author ruoyi.net
    ///  date   2025-04-01 16:11:48
    /// </summary>
    public class DeviceManagementMaintRepository : BaseRepository<DeviceManagementMaint, DeviceManagementMaintDto>
    {
        public DeviceManagementMaintRepository(ISqlSugarRepository<DeviceManagementMaint> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceManagementMaint> Queryable(DeviceManagementMaintDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.DeviceId > 0, (t) => t.DeviceId == dto.DeviceId)
            ;
        }

        public override ISugarQueryable<DeviceManagementMaintDto> DtoQueryable(DeviceManagementMaintDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.DeviceId > 0, (t) => t.DeviceId == dto.DeviceId)
                .Select((t) => new DeviceManagementMaintDto
                {
                     DeviceId = t.DeviceId 
                }, true);
        }
    }
}