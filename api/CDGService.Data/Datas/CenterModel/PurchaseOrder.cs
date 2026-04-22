using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 采购订单 --根据各透析中心的采购申请单总汇生成
    /// </summary>

    public class PurchaseOrder : ISoftDelete
    {
        public string Id { get; set; }
        /// <summary>
        /// 系统自动生成订单编号
        /// 规则：OD-201905150001(D药品U耗材E其他)
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 手动输入表单/订单编号（B018）
        /// </summary>
        public string OrderArtificialNo { get; set; }
        /// <summary>
        /// 订单类型（1药品，2耗材 3 其他）
        /// </summary>
        public int?  MedicalItemType { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 物品种类数
        /// </summary>
        public int? ItemType { get; set; }
        /// <summary>
        /// 采购总数
        /// </summary>
        public decimal? TotalQty { get; set; }
        /// <summary>
        ///采购总价
        /// </summary>
        public decimal? ActualPrice { get; set; }
        /// <summary>
        /// 备注（2019年05月份（下旬））
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyMain { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 1 未到货 2 已关闭 3 部分到货 4 已收货 5未审批， 6上级待审 7 未定价 8未推送 9已拒绝 （关闭订单，从申请-审批-采购-收货-入库已闭环）      
        /// </summary>
        public int? DataState { get; set; }

        public bool IsDelete { get; set; }

        /// <summary>
        /// 是否推送至中心端
        /// </summary>
        public bool? IsPushCenter { get; set; }


        public string SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier supplier { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// 集团端审核状态
        /// 1未提交，2未审批，3已同意，4已拒绝,6上级待审
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
        [ForeignKey(nameof(CenterId))]
        public CenterDialysis center { get; set; }
        public string Catalogue { get; set; }
        [ForeignKey(nameof(Catalogue))]
        public WarehouseCatalog warehouseCatalog { get; set; }

        public virtual List<OrderApprove>  OrderApproves { get; set; }

        /// <summary>
        /// 申请单信息
        /// </summary>
        public virtual List<PurchaseRequestsOrder> PurchaseRequestsOrders { get; set; } = new List<PurchaseRequestsOrder>();
        /// <summary>
        /// 是否为有价耗材
        /// </summary>
        public bool IsGeneral { get; set; }
    }

    /// <summary>
    /// 采购申请单-订单关系
    /// </summary>
    public class PurchaseRequestsOrder : ISoftDelete
    {
        public string Id { get; set; }
        /// <summary>
        /// 申请单ID
        /// </summary>
        public string ApplyId { get; set; }
        [ForeignKey(nameof(ApplyId))]
        public virtual PurchaseRequest FPurchaseRequest { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual PurchaseOrder  purchaseOrder { get; set; }
        public string OrderNo { get; set; }
        public int?  MedicalItemType { get; set; }
        public bool IsDelete { get; set; }
        public string SupplierId { get; set; }

    }

    /// <summary>
    /// 订单明细
    /// </summary>
    public class OrderDetail : ISoftDelete
    {
        public string Id { get; set; }
        public string CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual CenterDialysis Center { get; set; }
        public string OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual PurchaseOrder purchaseOrder { get; set; }
        public string MedicalId { get; set; }
        [ForeignKey("MedicalId")]
        public virtual MedicalItemRecord Medical { get; set; }
        /// <summary>
        /// 月均用量
        /// </summary>
        public decimal? MonthAverage { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        public decimal? CurrentInventory { get; set; }
        /// <summary>
        /// 上一次采购价
        /// </summary>
        public decimal? UpPurchPrice { get; set; }
        /// <summary>
        /// 当前采购价
        /// </summary>
        public decimal? PurchPrice { get; set; }
        /// <summary>
        /// 采购数量
        /// </summary>
        public decimal? PurchasePuantity { get; set; }

        /// <summary>
        /// 申请单明细id集合(,隔开)
        /// </summary>
        public string PurchDetailIds { get; set; }
        /// <summary>
        /// 采购总额
        /// </summary>
        public decimal? SumPrice { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        public decimal? SalePrice { get; set; }
        /// <summary>
        /// 实际到货数量
        /// </summary>
        public decimal? ActualQty { get; set; }
        /// <summary>
        /// 固定资产规格
        /// </summary>
        public string HurrySlowly { get; set; }

        public string SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier supplier { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        ///   1 未到货 2 已关闭 3 部分到货 4 已收货 关闭订单，从申请-审批-采购-收货-入库已闭环）
        /// </summary>
        public int? DataState { get; set; }
        public bool IsDelete { get; set; }

        public bool? isEdit { get; set; }
    }

}
