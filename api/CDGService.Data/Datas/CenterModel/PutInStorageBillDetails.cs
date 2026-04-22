using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 物品入库明细表
    /// </summary>
    public class PutInStorageBillDetails
    {
        public string Id { get; set; }
        public string PutInStorageBillId { get; set; }
        public string PurchaseDetailsId { get; set; }
        public string MedicalItemId { get; set; }
        public string BatchNo { get; set; }
        public string SupplierId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? InQty { get; set; }
        public int? IsImportProcurement { get; set; }
        public string ProcurementPackage { get; set; }
        public string ProcurementUnit { get; set; }
        public decimal? InSumMoney { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

        public string BatchId { get; set; }
        public string NoticeMedicalsId { get; set; }
        public decimal? SaleSumMoney { get; set; }
        /// <summary>
        /// 1按单价统计2按总价统计
        /// </summary>
        public int? StatisticType { get; set; }

    }
}
