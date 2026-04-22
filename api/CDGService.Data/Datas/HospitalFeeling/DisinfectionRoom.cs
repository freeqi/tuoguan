using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class DisinfectionRoom
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }

        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis CenterDialysis { get; set; }
        /// <summary>
        /// 分区ID
        /// </summary>
        public string PartitionId { get; set; }

        [ForeignKey(nameof(PartitionId))]
        public virtual SystemDictionary Partition { get; set; }
        /// <summary>
        /// 消毒时间
        /// </summary>
        public DateTime? DisinfectionTime { get; set; }
        /// <summary>
        /// 是否合格
        /// </summary>
        public bool IsQualified { get; set; }
        /// <summary>
        /// 消毒人
        /// </summary>
        public string DisinfectionUser { get; set; }
        /// <summary>
        /// 不合格说明
        /// </summary>
        public string UnqualifiedDescribe { get; set; }
        /// <summary>
        /// 不合格后续处理
        /// </summary>
        public string UnqualifiedDeal { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
    }
}
