using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    public class PrescriptionDetailRefund
    {
        public string Id { get; set; }
        public string PrescriptionDetailId { get; set; }
        public decimal? ApplyRefundQty { get; set; }
        public string ApplyRefundUnit { get; set; }
        public decimal? ReturnWarehouseQty { get; set; }
        public string ReturnWarehouseId { get; set; }
        public string RefundStatus { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

        public int? RefundType { get; set; }

    }
}
