using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{
    /// <summary>
    /// 订单审批权限
    /// </summary>
    public class ApprovalProcess
    {
        /*
          "Role": "采购专员",
      "describe": "填写预算价格等采购信息",
      "IsApproval": false,
      "amount": 0
         */
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public int level { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 是否有权审批
        /// </summary>
        public bool IsApproval { get; set; }
        /// <summary>
        /// 有权审核条件-金额（以上）
        /// </summary>
        public decimal? MinAmount { get; set; }
        /// <summary>
        /// 有权审核条件-金额（以下）
        /// </summary>
        public decimal? MaxAmount { get; set; }
    }

    /// <summary>
    /// 采购提交
    /// </summary>
    public class PurchSubmit
    {
        /*
          "Role": "采购专员",
      "describe": "填写预算价格等采购信息",
      "IsApproval": false,
      "amount": 0
         */
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public int level { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 是否有权审批
        /// </summary>
        public bool IsApproval { get; set; }
        /// <summary>
        /// 有权审核条件-金额（以上）
        /// </summary>
        public decimal? MinAmount { get; set; }
        /// <summary>
        /// 有权审核条件-金额（以下）
        /// </summary>
        public decimal? MaxAmount { get; set; }
    }
    /// <summary>
    /// 采购审批
    /// </summary>
    public class  PurchApproval
    {
        public string Role { get; set; }
        public string UserName { get; set; }
        public int level { get; set;}
        public string RoleId { get; set; }
       
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
    }
    /// <summary>
    /// 药品定价
    /// </summary>
    public class OrderPricingRole
    {
        public string Role { get; set; }
        public string RoleId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
    }
}
