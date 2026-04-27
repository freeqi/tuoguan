using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 租户输入DTO
    /// </summary>
    public class TenantInput
    {
        public string Id { get; set; }
        public string TenantName { get; set; }
        public string TenantCode { get; set; }
        public string ContactPhone { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public string Modifier { get; set; }
    }

    /// <summary>
    /// 租户输出DTO
    /// </summary>
    public class TenantOutput
    {
        public string Id { get; set; }
        public string TenantName { get; set; }
        public string TenantCode { get; set; }
        public string ContactPhone { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
    }

    /// <summary>
    /// 用户租户关联输入DTO
    /// </summary>
    public class UserTenantInput
    {
        public string UserId { get; set; }
        public string TenantId { get; set; }
    }

    /// <summary>
    /// 用户租户关联输出DTO
    /// </summary>
    public class UserTenantOutput
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string TenantId { get; set; }
    }
}
