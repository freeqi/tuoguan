using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 租户信息
    /// </summary>
    public class Tenant : ISoftDelete
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 租户名称
        /// </summary>
        public string TenantName { get; set; }
        
        /// <summary>
        /// 租户编码
        /// </summary>
        public string TenantCode { get; set; }
        
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; set; }
        
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; }
        
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// 状态 1:启用 0:停用
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
        
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
    
    /// <summary>
    /// 用户租户关联
    /// </summary>
    public class UserTenant
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
    }
}
