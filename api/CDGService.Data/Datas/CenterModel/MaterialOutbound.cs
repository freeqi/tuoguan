using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// tablename--MaterialOutbound
    /// </summary>
    public class MaterialOutbound
    {
        public string Id { get; set; }
        public string ImportNo{ get; set; }
        public string WarehouseId { get; set; }
        public string OutboundType { get; set; }
        public string OutboundNo { get; set; }
        public string OutboundReason { get; set; }
        public decimal? TotalCost { get; set; }
        public int? ItemType { get; set; }
        public decimal? TotalQty { get; set; }
        public string AuditConditionId { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Advice { get; set; }
        public string Auditor { get; set; }
        public int? IsERP { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string ReversalOutboundId { get; set; }
        public decimal? TotalSalesPrice { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

         
    }

}
