using RuoYi.Data.Entities;
using SqlSugar;

namespace RuoYi.Device.Entities
{
    [SugarTable("device_type","设备分类表")]
    public class DeviceType : UserBaseEntity
    {
        /** ID */
        [SugarColumn(ColumnName = "id",ColumnDescription = "ID",IsPrimaryKey = true,IsIdentity = true)]
        public long Id { get; set; }

        /** 父部门ID */
        [SugarColumn(ColumnName = "parent_id",ColumnDescription = "父ID")]
        public long ParentId { get; set; }

        /** 祖级列表 */
        [SugarColumn(ColumnName = "ancestors",ColumnDescription = "祖级列表")]
        public string? Ancestors { get; set; }

        /** 部门名称 */
        [SugarColumn(ColumnName = "dept_name",ColumnDescription = "名称")]
        public string? DeptName { get; set; }

        /** 显示顺序 */
        [SugarColumn(ColumnName = "order_num",ColumnDescription = "显示顺序")]
        public int? OrderNum { get; set; }

        /** 负责人 */
        [SugarColumn(ColumnName = "leader",ColumnDescription = "负责人")]
        public string? Leader { get; set; }

        /** 联系电话 */
        [SugarColumn(ColumnName = "phone",ColumnDescription = "联系电话")]
        public string? Phone { get; set; }

        /** 邮箱 */
        [SugarColumn(ColumnName = "email",ColumnDescription = "邮箱")]
        public string? Email { get; set; }

        /** 部门状态:0正常,1停用 */
        [SugarColumn(ColumnName = "status",ColumnDescription = "部门状态:0正常,1停用")]
        public string? Status { get; set; }

        /** 删除标志（0代表存在 2代表删除） */
        [SugarColumn(ColumnName = "del_flag",ColumnDescription = "删除标志（0代表存在 2代表删除）")]
        public string? DelFlag { get; set; }

        /** 租户ID（新增字段） */
        [SugarColumn(ColumnName = "tenant_id",ColumnDescription = "租户ID（集团ID）")]
        public long? TenantId { get; set; }


        /** 父部门名称 */
        //[SugarColumn(IsIgnore = true)]
        //public string? ParentName { get; set; }

        /** 子部门 */
        [SugarColumn(IsIgnore = true)]
        public List<DeviceType> Children { get; set; } = new List<DeviceType>();

    }
}
