using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 批次库存(二级库存)明细表
    /// </summary>
    public class BatchNoSecondaryStoreroomDetail
    {
        public string Id { get; set; }
        public string BatchNoSecondaryStoreroomId { get; set; }
        [ForeignKey("BatchNoSecondaryStoreroomId")]
        public virtual BatchNoSecondaryStoreroom batchNoSecondaryStoreroom { get; set; }
        public string OutInBoundType { get; set; }
        public string OutInBoundDetailId { get; set; }
        [ForeignKey("OutInBoundDetailId")]
        public virtual  MaterialOutboundDetail  materialOutboundDetail { get; set; }

        public decimal? OutInBoundQty { get; set; }
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
