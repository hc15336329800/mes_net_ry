using Microsoft.AspNetCore.Http;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using RuoYi.Data;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    /// <summary>
    ///  巡检记录表 Repository
    ///  author zgr.net
    ///  date   2025-04-09 14:10:45
    /// </summary>
    public class DeviceInspectionRecordRepository : BaseRepository<DeviceInspectionRecord, DeviceInspectionRecordDto>
    {
        public DeviceInspectionRecordRepository(ISqlSugarRepository<DeviceInspectionRecord> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceInspectionRecord> Queryable(DeviceInspectionRecordDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
            ;
        }

        public override ISugarQueryable<DeviceInspectionRecordDto> DtoQueryable(DeviceInspectionRecordDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
                .Select((t) => new DeviceInspectionRecordDto
                {
                     Id = t.Id 
                }, true);
        }

 



    }
}