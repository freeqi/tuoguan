using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class WaterFuel : ISoftDelete
    {
        public string Id { get; set; }
        public string MainId { get; set; }
        public string CenterId { get; set; }
        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis centerDialysis { get; set; }
        /// <summary>
        /// 1租金（门诊部）、2租金（宿舍）、3物管、4水电、5工资、6社保、7福利、
        /// 8爱心基金、9折旧费、10装修/无形资产摊销、11车辆费用（含维修保养）、12 其他
        /// </summary>
        public BusinessTypeEnum ItemType { get; set; }



        // public string ItemCardNum { get; set; }
        /// <summary>
        /// 缴费时间（）
        /// </summary>
        public DateTime? Month { get; set; } 
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? AmountPriec { get; set; }
        
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }

        public bool IsDelete { get; set; }
    }
}
