using RuoYi.Common.Data;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    /// <summary>
    ///  设备缺陷记录表 Repository
    ///  author zgr
    ///  date   2025-04-10 10:16:07
    /// </summary>
    public class DeviceDefectRecordRepository : BaseRepository<DeviceDefectRecord, DeviceDefectRecordDto>
    {
        public DeviceDefectRecordRepository(ISqlSugarRepository<DeviceDefectRecord> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceDefectRecord> Queryable(DeviceDefectRecordDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
            ;
        }

        //public override ISugarQueryable<DeviceDefectRecordDto> DtoQueryable(DeviceDefectRecordDto dto)
        //{
        //    return Repo.AsQueryable()
        //        .WhereIF(dto.Id > 0, (t) => t.Id == dto.Id)
        //        .Select((t) => new DeviceDefectRecordDto
        //        {
        //             Id = t.Id 
        //        }, true);
        //}
        public override ISugarQueryable<DeviceDefectRecordDto> DtoQueryable(DeviceDefectRecordDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.Id > 0,t => t.Id == dto.Id)
                .WhereIF(!string.IsNullOrEmpty(dto.DefectName),t => t.DefectName.Contains(dto.DefectName))
                .WhereIF(dto.TaskId > 0,t => t.TaskId == dto.TaskId)
                .WhereIF(!string.IsNullOrWhiteSpace(dto.DefectStatus),t => t.DefectStatus == dto.DefectStatus) // ✅ 精准匹配
                .WhereIF(!string.IsNullOrWhiteSpace(dto.DefectCategory),t => t.DefectCategory == dto.DefectCategory) // ✅ 精准匹配  维修/缺陷/抢修

                .Select(t => new DeviceDefectRecordDto
                {
                    Id = t.Id,
                    TaskId = t.TaskId,
                    DefectName = t.DefectName,
                    DefectStatus = t.DefectStatus,
                    DeviceName = t.DeviceName,
                    DevicePath = t.DevicePath,
                    DefectDesc = t.DefectDesc,
                    DefectCategory = t.DefectCategory,
                    SeverityLevel = t.SeverityLevel,
                    Suggestion = t.Suggestion,
                    DiscoveryTime = t.DiscoveryTime,
                    FixTime = t.FixTime,
                    FixPerson = t.FixPerson,
                    FixDeadline = t.FixDeadline,
                    ImageUrl = t.ImageUrl,
                    // 系统
                    CreateTime = t.CreateTime,
                    CreateBy = t.CreateBy,
                    UpdateBy = t.UpdateBy,
                    UpdateTime = t.UpdateTime

                },true);
        }


    }
}