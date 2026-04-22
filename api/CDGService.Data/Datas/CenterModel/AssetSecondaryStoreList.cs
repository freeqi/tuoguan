using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    public class AssetSecondaryStoreList
    {
        public string Id { get; set; }
        public string WarehouseId { get; set; }
        public string BatchNo { get; set; }
        public string MedicalItemId { get; set; }
        public string SupplierId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? InQty { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }

}
