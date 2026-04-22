using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 调价公告中，调价明细单
    /// </summary>
    public class NoticeMedical
    {
        public string Id { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticesId { get; set; }

        [ForeignKey(nameof(NoticesId))]
        public virtual Information information { get; set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        public string MedicalId { get; set; }



        /// <summary>
        /// 成本
        /// </summary>
        public decimal? PurchasingPrice { get; set; }
        /// <summary>
        /// 原售价
        /// </summary>
        public decimal? UpSalePrice { get; set; }

        /// <summary>
        /// 当前售价 -非空
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? DisposeTime { get; set; }

        public DateTime? OutTime { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>
        public int IsRead { get; set; }
        /// <summary>
        /// 是否需要反馈
        /// </summary>
        public bool IsNeedBack { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }

    }


    public class CenterNotices
    {
        public List<NoticeMedical> noticeMedicals{ get; set; }
        public Information information { get; set; }

        public List<UsersMsg> usersMsgs { get; set; }
    }
}
