using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{

    /// <summary>
    /// 药品档案扩展目录-价格
    /// </summary>
    public class MedicalDrugExtension
    {
        public string Id { get; set; }

        public string MedicalId { get; set; }
        [ForeignKey("MedicalId")]
        public virtual MedicalItemRecord medicalItemRecord { get; set; }

        /// <summary>
        /// 透析中心ID 为0 则为通用价格
        /// </summary>
        public string CenterId { get; set; }

        [ForeignKey("CenterId")]
        public virtual CenterDialysis centerDialysis { get; set; }
        /// <summary>
        /// 采购价
        /// </summary>
        public decimal PurchasingPrice { get; set; }

        /// <summary>
        /// 协议价（公司定价）
        /// 
        /// </summary>
        public decimal AgreementPrice { get; set; }
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal RetailPrice { get; set; }
        /// <summary>
        /// 参考价
        /// </summary>
        public decimal ReferencePrice { get; set; }

        /// <summary>
        /// 政府指导价
        /// </summary>
        public decimal GocGuidePrice { get; set; }

        /// <summary>
        /// 社保指导价
        /// </summary>
        public decimal? SocialSecurityPrice { get; set; }
        /// <summary>
        /// 是否为当前使用
        /// </summary>
        public bool IsCurrentUse { get; set; }
        public int DataState { get; set; }
        /// <summary>
        /// 建档原因及目的
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }

        /// <summary>
        /// 采购系数上限
        /// </summary>
        public decimal? Coefficient { get; set; }
    }
}
