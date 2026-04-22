using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class ReturnRequest
    {
        public string Id { get; set; }
        public string ReturnNo { get; set; }
        public int? ItemType { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? TotalCost { get; set; }
        public string AuditConditionId { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Advice { get; set; }
        public string Auditor { get; set; }

        [ForeignKey("Auditor")]
        public virtual Employee employee { get; set; }
        public string SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }
        public int? IsERP { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual CenterDialysis center { get; set; }
        public DateTime? CollectData { get; set; }

        /// <summary>
        /// …Û∫À◊¥Ã¨
        /// </summary>
        public string GroupAuditStatus { get; set; }
        /// <summary>
        /// …Û∫À ±º‰
        /// </summary>
        public DateTime? GroupAuditDate { get; set; }
        /// <summary>
        /// …Û∫À“‚º˚
        /// </summary>
        public string GroupAdvice { get; set; }
        /// <summary>
        /// …Û∫À»À
        /// </summary>
        public string GroupAuditor { get; set; }

        /// <summary>
        /// ÕÀªı◊¥Ã¨
        /// </summary>
        public int? ReturnState { get; set; }
        public int? IsSettlement { get; set; }

        public DateTime? SettlementDate { get; set; }

        public string SettlementPeople { get; set; }

        public string ReturnAuditor { get; set; }
        public DateTime? ReturnDate { get; set; }

        public DateTime? CheckedDate { get; set; }
        public string CheckedPeople { get; set; }
        public int? IsChecked { get; set; }

        public virtual List<ReturnDetails> listDetails { get; set; }

    }
}
