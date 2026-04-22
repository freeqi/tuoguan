using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 入库开单表模型
    /// </summary>
    public class PutInStorageBill
    {
        public string Id { get; set; }
        public string ReversalInBoundId { get; set; }
        public string InTypeId { get; set; }
        public string InStorageNo { get; set; }
        public string PurchaseId { get; set; }
        public string GoodsNo { get; set; }
        public int? ItemTypeCount { get; set; }
        public decimal? TotalQty { get; set; }
        public string AuditPerson { get; set; }
        public string AuditConditionId { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Advice { get; set; }
        public string Remark { get; set; }
        public string FounderId { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public int? IsBegin { get; set; }
        public int? IsSettlement { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime? SettlementDate { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
        public string SettlementPeople { get; set; }

        /// <summary>
        /// 勾稽核对人
        /// </summary>
        public string  CheckedPeople { get; set; }

        public int? IsChecked { get; set; }
        public DateTime? CheckedDate { get; set; }


    }
}
