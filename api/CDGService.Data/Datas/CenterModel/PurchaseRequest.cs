using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class PurchaseRequest
    {

        public string Id { get; set; }

        public string CenterId { get; set; }

        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis centerDialysis { get; set; }
        /// <summary>
        /// 申请单号
        /// </summary>
        public string PurchaseNo { get; set; }
        /// <summary>
        /// 物品类别总数 
        /// </summary>
        public int? MedicaItemTypeCount { get; set; }

        /// <summary>
        /// 物品种类数
        /// </summary>
        public int? ItemType { get; set; }
        /// <summary>
        /// 采购总数
        /// </summary>
        public decimal? TotalQty { get; set; }
        /// <summary>
        /// 合计建议总价
        /// </summary>
        public decimal? TotalCost { get; set; }
        /// <summary>
        ///实际采购总价
        /// </summary>
        public decimal? ActualPrice { get; set; }

        /// <summary>
        /// 销售总价（预期，包含人为浪费的）
        /// </summary>
        public decimal? SalesTotalPrice { get; set; }
        /// <summary>
        /// 入库标识
        /// </summary>
        public int? PutInStorageMark { get; set; }
        /// <summary>
        /// 中心端审核状态
        /// </summary>
        public string AuditConditionId { get; set; }
        /// <summary>
        /// 中心端审核时间
        /// </summary>
        public DateTime? AuditDate { get; set; }
        /// <summary>
        /// 中心端审核意见
        /// </summary>
        public string Advice { get; set; }
        /// <summary>
        /// 中心端审核人
        /// </summary>
        public string Auditor { get; set; }
        /// <summary>
        /// 集团端审核状态
        /// 1未提交，2未审批，3已同意，4已拒绝, 5 回退（变2） 6上级待审 7 暂缓
        /// </summary>
        public string GroupAuditStatus { get; set; }
        /// <summary>
        /// 集团端审核时间
        /// </summary>
        public DateTime? GroupAuditDate { get; set; }
        /// <summary>
        /// 集团端审核意见
        /// </summary>
        public string GroupAdvice { get; set; }
        /// <summary>
        /// 集团端审核人
        /// </summary>
        public string GroupAuditor { get; set; }
        public int? IsERP { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        /// <summary>
        /// 1关 2 开
        /// </summary>
        public int? IsClosed { get; set; }
        /// <summary>
        /// 审批总量
        /// </summary>
        public decimal? ApprovalTotalQty { get; set; }

        /// <summary>
        /// 物品类别
        /// </summary>
        public string Catalogue { get; set; }
        [ForeignKey(nameof(Catalogue))]
        public virtual WarehouseCatalog WarehouseCatalog { get; set; }


        public int? PurchSubmitLevel { get; set; }

        /// <summary>
        /// 是否合并 1为合并，其他为未合并
        /// </summary>
        public bool IsMerge { get; set; }

        public bool IsGroupAdd { get; set; }

        /// <summary>
        /// 订单生成状态，0未生成 1部分 2 全部
        /// </summary>
        public int? OrderType { get; set; }

        /// <summary>
        /// 审批
        /// </summary>
        public virtual List<PurchaseApprove> ListPurchaseApproves { get; set; } = new List<PurchaseApprove>();
        /// <summary>
        /// 订单信息
        /// </summary>
        public virtual List<PurchaseRequestsOrder> PurchaseRequestsOrders { get; set; } = new List<PurchaseRequestsOrder>();

        public virtual List<PurchaseDetail> PurchaseDsOrders { get; set; } = new List<PurchaseDetail>();
    }

    /// <summary>
    /// 集团端合并的申请单流水
    /// </summary>
    public class GroupMergePurchase
    {
        public string Id { get; set; }
        /// <summary>
        /// 集团端生成的新申请单ID
        /// </summary>
        public string MainPurchaseId { get; set; }
        /// <summary>
        /// 原中心端申请的ID
        /// </summary>
        public string CenterPurchaseId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }

    }
}
