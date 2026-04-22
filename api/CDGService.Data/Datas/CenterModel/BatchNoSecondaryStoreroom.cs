using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 批次库存(二级库房)表
    /// </summary>
    public class BatchNoSecondaryStoreroom
    {
        public string Id { get; set; }
        public string WarehouseId { get; set; }
        public string BatchNo { get; set; }
        public string MedicalItemId { get; set; }
        [ForeignKey("MedicalItemId")]
        public virtual MedicalItemRecord MedicalItemRecord { get; set; }
        public string SupplierId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? InQty { get; set; }
        public decimal? TakeInventory { get; set; }
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
