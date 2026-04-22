using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 报废
    /// </summary>
    public class MaterialApplyApprove
    {
        public string Id { get; set; }
        /// <summary>
        /// 库房Id
        /// </summary>
        public string WarehouseId { get; set; }
        public string OutboundType { get; set; }
        /// <summary>
        /// 报废申请单号
        /// </summary>
        public string PurchaseNo { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        public string PurchaseReason { get; set; }
        /// <summary>
        /// 成本总价
        /// </summary>
        public decimal? TotalCost { get; set; }

        /// <summary>
        /// // 估值总价
        /// </summary>
        public decimal? AppraiseCost { get; set; }
        public decimal? TotalSalesPrice { get; set; }
        /// <summary>
        /// 种类
        /// </summary>
        public int? ItemType { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? TotalQty { get; set; }
        public string AuditConditionId { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDate { get; set; }
        public string Advice { get; set; }
        public string Auditor { get; set; }

        /// <summary>
        /// 集团端审核状态  1未提交，2未审批， 3已同意，4已拒绝  5 回退  6 上级待审 7 暂缓
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

        public string CenterId { get; set; }
        public string IsERP { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }

        public List<MaterialApplyApproveDetail> materialApplyApproveDetails { get; set; }

        public int? AuditorLevel { get; set; }
    }

    /// <summary>
    /// 报废明细
    /// </summary>
    public class MaterialApplyApproveDetail
    {
        public string Id { get; set; }
        public string MAApplyId { get; set; }
        [ForeignKey("MAApplyId")]
        public MaterialApplyApprove materialApply { get; set; }
        public string MaterialId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? MaterialQuantity { get; set; }
        public string MaterialUnit { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? SalesTotalPrice { get; set; }
        /// <summary>
        /// 成本单价
        /// </summary>
        public decimal? UnitPrice { get; set; }
        public decimal? CostTotalPrice { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }

        public string ProductionDateStr { get; set; }

        public string QualityDateStr { get; set; }

        /// <summary>
        /// 估值
        /// </summary>
        public decimal? Appraise { get; set; }
    }


    /// <summary>
    /// 滞销
    /// </summary>
    public class MaterialsWarningApplyList
    {
        public string Id { get; set; }
        public int? ItemType { get; set; }
        public string WarehouseId { get; set; }
        public string PurchaseNo { get; set; }
        public string AuditConditionId { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Advice { get; set; }
        public string Auditor { get; set; }
        public string GroupAuditStatus { get; set; }
        public DateTime? GroupAuditDate { get; set; }
        public string GroupAdvice { get; set; }
        public string GroupAuditor { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }

        public string CenterId { get; set; }

        public int AuditorLevel { get; set; }
        public List<MaterialsWarningApplyDetailList> applyDetailLists { get; set; }
    }
    /// <summary>
    /// 滞销明细
    /// </summary>
    public class MaterialsWarningApplyDetailList
    {
        public string Id { get; set; }
        public string MaterialsWarningApplyListId { get; set; }
        [ForeignKey("MaterialsWarningApplyListId")]
        public MaterialsWarningApplyList warningApplyList { get; set; }
        public string MedicalItemId { get; set; }
        public string BatchNo { get; set; }
        public string SupplierId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }
        public decimal? Amount { get; set; }
        public string CenterId { get; set; }
    }

}
