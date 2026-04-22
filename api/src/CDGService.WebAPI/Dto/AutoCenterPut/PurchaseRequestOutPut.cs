using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    #region Output

    public class PurchaseOutPut
    {
        public PurchaseRequestOutPut purchaseRequest { get; set; }
        public PurchaseDetailOutPut[] purchaseDetails { get; set; }
        public ApprovalProcessOutPut[] approvalProcessOutPuts { get; set; }
    }

    public class BatchPurchaseOutPut
    {
        public PurchaseRequestOutPut[] purchaseRequest { get; set; }
        public PurchaseDetailOutPut[] purchaseDetails { get; set; }
        //public ApprovalProcessOutPut[] approvalProcessOutPuts { get; set; }
    }
    public class PurchaseToOrder
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrdreNO { get; set; }
        /// <summary>
        /// 类型 1 药品 2 耗材 3 其他
        /// </summary>
        public int? MedicaItemType { get; set; }
        public string supplierId { get; set; }
    }


    /// <summary>
    /// 采购申请单
    /// </summary>
    public class PurchaseRequestOutPut
    {

        public int no { get; set; }
        public string Id { get; set; }

        public string CenterId { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 订单信息
        /// </summary>
        public List<PurchaseToOrder> purchaseToOrder { get; set; } = new List<PurchaseToOrder>();

        /// <summary>
        /// 订单生成状态，0未生成 1部分 2 全部
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 申请单号
        /// </summary>
        public string PurchaseNo { get; set; }
        /// <summary>
        /// 物品种类数
        /// </summary>
        public int? ItemType { get; set; }
        /// <summary>
        /// 采购总数
        /// </summary>
        public decimal? TotalQty { get; set; }
        public decimal? ApprovalTotalQty { get; set; }
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
        public string AuditDates { get; set; }
        /// <summary>
        /// 中心端审核意见
        /// </summary>
        public string Advice { get; set; }
        /// <summary>
        /// 中心端审核人
        /// </summary>
        public string Auditor { get; set; }
        public int? PurchSubmitLevel { get; set; }
        /// <summary>
        /// 集团端审核状态
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

        public int? MedicaItemTypeCount { get; set; }
        /// <summary>
        /// 物品类别 药品、耗材、低值、固定
        /// </summary>
        public string Catalogue { get; set; }
        public string CatalogueName { get; set; }
        //public string Founder { get; set; }
        //public DateTime? FounderDate { get; set; }
        //public string Modifier { get; set; }
        //public DateTime? ModifierDate { get; set; }
        //public int? DataState { get; set; }
        /// <summary>
        /// 是否合并 1为合并，其他为未合并
        /// </summary>
        public bool IsMerge { get; set; }
        /// <summary>
        /// 是否为集团端合并的单子
        /// </summary>
        public bool IsGroupAdd { get; set; }

    }

    public class PurchaseDetailOutPut
    {
        public int? no { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 采购申请单Id
        /// </summary>
        public string ApplyId { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string CenterName { get; set; }
        public string CenterId { get; set; }
        public bool IsUpdate { get; set; }
        /// <summary>
        /// 药品耗材ID
        /// </summary>
        public string MedicalItemId { get; set; }

        //public string MedicalCode { get; set; }
        /// <summary>
        /// 双控价
        /// </summary>
        public decimal? DualPrice { get; set; }
        /// <summary>
        /// 医保现价
        /// </summary>
        public decimal? SocialSecurityPrice { get; set; } = 0;
        public MedicalItemRecordOutPut medicalItemRecordOutPut { get; set; }
        /// <summary>
        /// 成本价
        /// </summary>
        public decimal? InPrice { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal? SalePrice { get; set; }
        /// <summary>
        /// 建议价
        /// </summary>
        public decimal? AdvicePrice { get; set; }
        /// <summary>
        /// 实际到货量
        /// </summary>
        public decimal? ActualQty { get; set; }
        /// <summary>
        /// 应采购数量
        /// </summary>
        public decimal? InQty { get; set; }
        /// <summary>
        /// 建议总价
        /// </summary>
        public decimal? InSumMoney { get; set; }
        /// <summary>
        /// 采购包装规格
        /// </summary>
        public string ProcurementPackage { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        public string ProcurementUnit { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }
        /// <summary>
        /// 审批数量
        /// </summary>
        public decimal? ApprovalQty { get; set; }
        /// <summary>
        /// 供货商名称
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        public decimal? CurrentInventory { get; set; }
        /// <summary>
        /// 月均用量
        /// </summary>
        public decimal? MonthAverage { get; set; }

        public string OrderNo { get; set; }
        /// <summary>
        /// 规格/技术参数
        /// </summary>
        public string SpecModelsOrMainTechParams { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        public string AppReason { get; set; }
        // public string Remark { get; set; }

        public int? DataState { get; set; }

        public decimal? coefficient { get; set; }
        /// <summary>
        /// 是否为集团端添加
        /// </summary>
        public bool IsGroupAdd { get; set; }

    }

    public class ApprovalProcessOutPut
    {
        /// <summary>
        /// 审核人
        /// </summary>
        public string ApprovalName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 步骤
        /// </summary>
        public int current { get; set; }
        public string Time { get; set; }
    }



    #endregion

    #region  Input

    public class PurchaseRequestQueryInput
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CenterId { get; set; }
        /// <summary>
        /// 集团审核状态
        /// </summary>
        public string ApprovalState { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 物品类别 1 药品、2耗材、3其他
        /// 用于合并采购单时筛选用
        /// </summary>
        public int? MedicalItemType { get; set; }
        /// <summary>
        /// 物品类别 药品、耗材、低值、固定
        /// </summary>
        public string Catalogue { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string remarks { get; set; }
        /// <summary>
        /// 订单生成状态，0未生成 1部分 2 全部已生成
        /// </summary>
        public int OrderType { get; set; }
    }

    public class ApprovalPurchaseRequestInput
    {
        //  {"GroupAuditStatus":"","GroupAuditDate":"","GroupAdvice":"","GroupAuditor":"","Id":""}
        /// <summary>
        /// 申请单ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 审核意见 内容
        /// </summary>
        public string GroupAdvice { get; set; }
        /// <summary>
        /// 意见状态 3已同意，4已拒绝  5 回退  6 上级待审 7 暂缓
        /// </summary>
        public string GroupAuditStatus { get; set; }


    }

    public class PurchaseDetaiPriceInput
    {
        /// <summary>
        /// 申请单ID
        /// </summary>
        public string PurchaseRequestId { get; set; }
        /// <summary>
        /// 明细ID
        /// </summary>
        public string ItemsId { get; set; }
        /// <summary>
        /// 成本价
        /// </summary>
        public decimal InPrice { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }
        /// <summary>
        /// 审批数量
        /// </summary>
        public decimal? ApprovalQty { get; set; }

        public string ProcurementPackage { get; set; }
        public string Remark { get; set; }
    }


    public class PurchaseDetailInPut
    {

        public string Id { get; set; }
        /// <summary>
        /// 采购申请单Id
        /// </summary>
        public string ApplyId { get; set; }

        /// <summary>
        /// 药品耗材ID
        /// </summary>
        public string MedicalItemId { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string ProcurementPackage { get; set; }
        /// <summary>
        /// 包装单位
        /// </summary>
        public string ProcurementUnit { get; set; }
        /// <summary>
        /// 成本价
        /// </summary>
        public decimal? InPrice { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 应采购数量
        /// </summary>
        public decimal? InQty { get; set; }

        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }

    /// <summary>
    /// 拆分
    /// </summary>
    public class PurchaseDetailSplit
    {
        public string detailId { get; set; }

        public decimal detailNum { get; set; }
    }



    public class PurchaseDetailDelInPut
    {
        public string Id { get; set; }
        public string Remark { get; set; }
    }
    public class PurchaseDetailQueryInPut
    {
        /// <summary>
        /// 申请单ID数组
        /// </summary>
        public string[] ApplyId { get; set; }
        /// <summary>
        /// 物品类型1 药品 2 耗材 3 其他
        /// </summary>
        public int MedicaItemType { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }
    }

    /// <summary>
    /// 订单生成参数
    /// </summary>
    public class OrderInput
    {
        /// <summary>
        /// 申请单ID数组
        /// </summary>
        public string[] ApplyId { get; set; }
        /// <summary>
        /// 物品类型1 药品 2 耗材 3 其他 0全部
        /// </summary>
        public int MedicalItemType { get; set; }
        /// <summary>
        /// 物品类别 药品、耗材、低值、固定
        /// </summary>
        public string Catalogue { get; set; }
        /// <summary>
        /// 手动输入表单/订单编号（B018）
        /// </summary>
        public string OrderArtificialNo { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 备注（2019年05月份（下旬））
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyMain { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }

        public string[] detailId { get; set; }

    }

    public class PurchaseOrderInPut
    {
        public string Id { get; set; }

        /// <summary>
        /// 手动输入表单/订单编号（B018）
        /// </summary>
        public string OrderArtificialNo { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 备注（2019年05月份（下旬））
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyMain { get; set; }

    }


    public class OrderQueryInput
    {

        /// <summary>
        /// 物品类型1 药品 2 耗材 3 其他
        /// </summary>
        public int MedicalItemType { get; set; }
        public string SupplierId { get; set; }
        public string CenterId { get; set; }
        ///// <summary>
        ///// 手动输入表单/订单编号（B018）
        ///// </summary>
        //public string OrderArtificialNo { get; set; }

        /// <summary>
        /// 申请开始时间
        /// </summary>
        public DateTime? BeginApplyDate { get; set; }

        /// <summary>
        /// 申请结束时间
        /// </summary>
        public DateTime? EndApplyDate { get; set; }

        /// <summary>
        /// 备注（2019年05月份（下旬））
        /// </summary>
        public string Remarks { get; set; }
        ///// <summary>
        ///// 申请人
        ///// </summary>
        //public string ApplyMain { get; set; }

        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public int? OrderState { get; set; }

        public string Catalogue { get; set; }
    }


    public class OrderQueryOutPut
    {
        public int no { get; set; }

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
        public int MedicalItemType { get; set; }
        public string MedicalItemTypeName { get; set; }


        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

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

        public int? DataState { get; set; }

        public bool IsPushCenter { get; set; }
        /// <summary>
        /// 集团端审核状态
        /// 2未审批，3已同意，4已拒绝,6上级待审
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

        public string centerName { get; set; }
    }

    /// <summary>
    /// 订单明细
    /// </summary>
    public class OrderDetailData
    {
        public int no { get; set; }
        public string Id { get; set; }
        public string CenterId { get; set; }
        /// <summary>
        /// 机构
        /// </summary>
        public string CenterName { get; set; }
        public string SupplierId { get; set; }

        public string SupplierName { get; set; }
        public string MedicalId { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string MedicalName { get; set; }

        public string MedicalItemCode { get; set; }
        public string Ncode { get; set; }
        /// <summary>
        /// 件比
        /// </summary>
        public string MedicalThan { get; set; }
        /// <summary>
        /// 包装规格
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }

        public string SiSpecifications { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Uint { get; set; }
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
        /// 急缓程度
        /// </summary>
        public string HurrySlowly { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 双控价
        /// </summary>
        public decimal? DualPrice { get; set; }
        /// <summary>
        /// 医保现价
        /// </summary>
        public decimal? SocialSecurityPrice { get; set; }

        /// <summary>
        /// 单位换算后的医保限价
        /// </summary>
        public decimal? si_SecurityPrice { get; set; }

        //public string Founder { get; set; }
        //public DateTime? FounderDate { get; set; }
        //public string Modifier { get; set; }
        //public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        //public bool IsDelete { get; set; }
        /// <summary>
        /// 是否对照
        /// </summary>
        public bool? IsHIS { get; set; }
        public bool? isEdit { get; set; } = false;
        public string sFlag { get; set; }

        /// <summary>
        /// 销售价是否可0  ， ture = 是
        /// </summary>
        public bool IsSellFree { get; set; }


        /// <summary>
        /// 公司定价
        /// </summary>
        public string AgreementPrice { get; set; }
    }

    public class OrderDetailOutPut
    {
        public OrderDetailData[] orderDetailData { get; set; }
        public OrderQueryOutPut orderData { get; set; }

        public OrderApproveOutPut[] orderApproveOutPuts { get; set; }
    }
    public class OrderDetailUpdateInPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 月均用量
        /// </summary>
        public decimal? MonthAverage { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        public decimal? CurrentInventory { get; set; }

        /// <summary>
        /// 当前采购价
        /// </summary>
        public decimal? PurchPrice { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>

        public decimal? SalePrice { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierId { get; set; }
        /// <summary>
        /// 固定资产规格
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 急缓程度
        /// </summary>
        public string HurrySlowly { get; set; }

        public string remark { get; set; }

        public decimal? SumPrice { get; set; }
    }

    public class OrderCloseInput
    {
        public string OrderId { get; set; }

        public string Remark { get; set; }
    }

    public class OrderDetailCloseInput
    {
        public string OrderDetailId { get; set; }

        public string Remark { get; set; }
    }

    public class ReturnQuerInput
    {
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }
        public string Id { get; set; }

        public DateTime? beginTime { get; set; }

        public DateTime? endTime { get; set; }

        public string CenterId { get; set; }
        public string Remarks { get; set; }
        public string AuditStatus { get; set; }
    }
    public class ReturnRequestOutPut
    {
        public string Id { get; set; }
        public string ReturnNo { get; set; }
        public int? ItemType { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? TotalCost { get; set; }
        public string Auditor { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }

        public string Remark { get; set; }
        public DateTime? AuditDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }

        public string CenterName { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public string GroupAuditStatus { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? GroupAuditDate { get; set; }
        /// <summary>
        /// 审核意见
        /// </summary>
        public string GroupAdvice { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string GroupAuditor { get; set; }
    }

    public class OrderApproveOutPut
    {

        /// <summary>
        /// 审核人
        /// </summary>
        public string ApprovalName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 步骤
        /// </summary>
        public int current { get; set; }
        public string Time { get; set; }
    }

    public class ReturnDetailsOutPut
    {

        public string Id { get; set; }
        public string ReturnId { get; set; }
        public string MedicalItemId { get; set; }
        public string MedicalItemName { get; set; }

        public string specifications { get; set; }

        public string ReturnUnit { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? InQty { get; set; }
        public string ProcurementUnit { get; set; }
        public decimal? SalesReturnQty { get; set; }
        public string SalesReturnUnit { get; set; }
        public decimal? InSumMoney { get; set; }

        public string manufacturer { get; set; }
        public string Remark { get; set; }

        public int? DataState { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? YSalesReturnQty { get; set; }

    }

    public class ReturnDetailsInPut
    {
        public string Id { get; set; }
        public int SalesReturnQty { get; set; }

        public string Remark { get; set; }

    }


    public class ReturnExamineInPut
    {

        public string Id { get; set; }

        /// <summary>
        /// 审批结果 2同意，3拒绝
        /// </summary>
        public string GroupAuditStatus { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string GroupAdvice { get; set; }


    }

    public class MergePurchaseBreakInput
    {
        /// <summary>
        /// 合并后的采购单ID
        /// </summary>
        public string MergePurchaseId { get; set; }
        /// <summary>
        /// 需要拆出来的采购单ID
        /// </summary>
        public string ApplyId { get; set; }
    }
    public class batchPurchModel
    {
        public string[] OrderId { get; set; }
    }


    //固定资产报废估价入参
    public class FixedAssetsScrapInput
    {
        public string Id { get; set; }
        public string Remark { get; set; }
        public decimal? Appraise { get; set; }
    }


    #endregion
}
