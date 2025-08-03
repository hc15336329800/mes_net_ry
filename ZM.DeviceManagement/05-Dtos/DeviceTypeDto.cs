using RuoYi.Data.Dtos;
using SqlSugar;

namespace RuoYi.Device.Entities
{
    //设备分类表
    public class DeviceTypeDto :BaseDto
    {
        /** ID */
         public long Id { get; set; }

        /** 父ID */
         public long ParentId { get; set; }

        /** 祖级列表 */
        public string? Ancestors { get; set; }

        /** 名称 */
         public string? DeptName { get; set; }

        /** 显示顺序 */
         public int? OrderNum { get; set; }

        /** 负责人 */
        public string? Leader { get; set; }

        /** 联系电话 */
         public string? Phone { get; set; }

        /** 邮箱 */
         public string? Email { get; set; }

        /** 部门状态:0正常,1停用 */
         public string? Status { get; set; }

        /** 删除标志（0代表存在 2代表删除） */
         public string? DelFlag { get; set; }

        /** 租户ID（新增字段） */
        public long? TenantId { get; set; }


        /** 父部门名称 */
        //public string? ParentName { get; set; }

        /** 子部门 */
        public List<DeviceTypeDto> Children { get; set; }  

 
    }
}
