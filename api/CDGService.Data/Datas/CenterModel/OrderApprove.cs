using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
   public class OrderApprove
    {

        public string Id { get; set; }

        /// <summary>
        /// 申请单ID
        /// </summary>
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual PurchaseOrder purchaseOrder { get; set; }
        /// <summary>
        /// 审核状态 3同意4拒绝
        /// </summary>
        public int AuditStatus { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 审核意见
        /// </summary>
        public string Advice { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }
        [ForeignKey(nameof(Auditor))]
        public virtual User user { get; set; }

        /// <summary>
        /// 审核等级
        /// </summary>
        public int AuditorLevel { get; set; }

        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }
    }

    public class GroupAuditorList
    {
       
        /// <summary>
        /// 集团端审核时间
        /// </summary>
        public string GroupAuditDate { get; set; }
        /// <summary>
        /// 集团端审核意见
        /// </summary>
        public string GroupAdvice { get; set; }
        /// <summary>
        /// 集团端审核人
        /// </summary>
        public string GroupAuditor { get; set; }
    }


}
