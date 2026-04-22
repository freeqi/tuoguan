using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CDGService.Data.Datas
{
    public class MaterialOutboundDetail
    {
        public string Id { get; set; }
        public string MaterialOutboundId { get; set; }
        [ForeignKey("MaterialOutboundId")]
        public virtual MaterialOutbound  materialOutbound { get; set; }

        public string MaterialId { get; set; }
        public decimal? MaterialQuantity { get; set; }
        public string MaterialUnit { get; set; }
        public string BatchNo { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? CostTotalPrice { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? SalesTotalPrice { get; set; }

        public string SupplierName { get; set; }

        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

        public virtual ICollection<BatchNoSecondaryStoreroomDetail> batchNoSecondaryStoreroomDetails { get; set; }
        public virtual PrescriptionDetail  PrescriptionDetail { get; set; }
    }

}
