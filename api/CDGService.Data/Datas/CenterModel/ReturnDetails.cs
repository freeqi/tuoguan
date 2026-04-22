using CDGService.Data.Datas;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class ReturnDetails
    {
        public string Id { get; set; }
        public string ReturnId { get; set; }
        [ForeignKey("ReturnId")]
        public virtual ReturnRequest GetRequest { get; set; }
        public string MedicalItemId { get; set; }

        [ForeignKey("MedicalItemId")]
        public virtual MedicalItemRecord MedicalItem { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? InQty { get; set; }
        public string ProcurementUnit { get; set; }
        public decimal? SalesReturnQty { get; set; }
        /// <summary>
        /// Ô­ÊýÁ¿
        /// </summary>
        public decimal? YSalesReturnQty { get; set; }
        public string SalesReturnUnit { get; set; }
        [ForeignKey("SalesReturnUnit")]
        public virtual MedicalUnit Unit { get; set; }
        public decimal? InSumMoney { get; set; }
        public int? IsERP { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public decimal? SalePrice { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
         

    }
}
