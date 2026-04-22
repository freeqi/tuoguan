using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{

    public class PurchaseDetail
    {

        public string Id { get; set; }
        /// <summary>
        /// 采购申请单Id
        /// </summary>
        public string ApplyId { get; set; }

        [ForeignKey(nameof(ApplyId))]
        public virtual PurchaseRequest purchaseRequest { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual PurchaseOrder  purchaseOrder { get; set; }
        /// <summary>
        /// 药品耗材ID
        /// </summary>
        public string MedicalItemId { get; set; }
        [ForeignKey(nameof(MedicalItemId))]
        public virtual MedicalItemRecord medicalItemRecord { get; set; }
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



        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier supplier { get; set; }
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
        /// 是否为集团端添加
        /// </summary>
        public bool IsGroupAdd { get; set; }
        /// <summary>
        /// 1关 2 开
        /// </summary>
        public int? IsClosed { get; set; }

        public DateTime CollectTime { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        public decimal? CurrentInventory { get; set; }
        /// <summary>
        /// 月均用量
        /// </summary>
        public decimal? MonthAverage { get; set; }

        /// <summary>
        /// 固定资产：规格型号或主要技术参数
        /// </summary>
        public string SpecModelsOrMainTechParams { get; set; }

        /// <summary>
        /// 固定资产：申请理由
        /// </summary>
        public string AppReason { get; set; }

        public bool? IsPushCenter { get; set; }
        public string CenterId { get; set; }
    }

}
