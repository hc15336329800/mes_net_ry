using RuoYi.Common.Data;
using RuoYi.Data;
 
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    /// <summary>
    ///  设备巡检操作票表 Repository
    ///  author ruoyi.net
    ///  date   2025-04-07 10:31:04
    /// </summary>
    public class DeviceInspectionTicketRepository : BaseRepository<DeviceInspectionTicket, DeviceInspectionTicketDto>
    {
        public DeviceInspectionTicketRepository(ISqlSugarRepository<DeviceInspectionTicket> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceInspectionTicket> Queryable(DeviceInspectionTicketDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
            ;
        }

        public override ISugarQueryable<DeviceInspectionTicketDto> DtoQueryable(DeviceInspectionTicketDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
                .Select((t) => new DeviceInspectionTicketDto
                {
                     Id = t.Id 
                }, true);
        }
    }
}