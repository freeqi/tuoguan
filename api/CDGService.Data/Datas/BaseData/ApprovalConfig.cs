using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class ApprovalConfig
    {
        public string Id { get; set; }

        /// <summary>
        /// 档案类型 1 药品 2 耗材  3 固定资产 4 低值易耗品 1,2,3,4 逗号隔开
        /// </summary>
        public string  MedicalItemType { get; set; }

        /// <summary>
        /// 审批类型  1 采购申请单 2 采购订单
        /// </summary>
        public int ApprovalType { get; set; }
        /// <summary>
        /// 人员ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 人员姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 人员姓名备注（职位）
        /// </summary>
        public string UserRemark { get; set; }
        /// <summary>
        /// 审核等级
        /// </summary>
        public int PurchSubmitLevel { get; set; }
        /// <summary>
        ///  有权审核条件-金额（以上）
        /// </summary>
        public decimal? MinAmount { get; set; }
        /// <summary>
        ///  有权审核条件-金额（以下）
        /// </summary>
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// 备注、描述
        /// </summary>
        public string Describe { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }

    }
}
