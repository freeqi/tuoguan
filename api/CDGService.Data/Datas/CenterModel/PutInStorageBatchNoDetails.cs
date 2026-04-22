using System;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 物品入库批次库存表
    /// </summary>
    public class PutInStorageBatchNoDetails
    {
        public string Id { get; set; }
        public string BatchNo { get; set; }
        public string MedicalItemId { get; set; }
        public string SupplierId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? InQty { get; set; }
        public string ProcurementPackage { get; set; }
        public string ProcurementUnit { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }
}
