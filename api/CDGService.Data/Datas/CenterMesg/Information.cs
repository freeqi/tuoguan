using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 消息公告表
    /// </summary>
    public class Information : ISoftDelete
    {
        public string Id { get; set; }

        /// <summary>
        /// 消息分类ID
        /// </summary>
        public int MsgTypeId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string MsgTitle { get; set; }
        /// <summary>
        /// 详细内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string Adjunct { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SendTime { get; set; }
        /// <summary>
        /// 是否需要反馈
        /// </summary>
        public bool IsNeedBack { get; set; }
        /// <summary>
        /// 是否阅读
        /// </summary>
        public int IsRead { get; set; }
        public string Founder { get; set; }
        [ForeignKey(nameof(Founder))]
        public virtual User user { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 1有效 2 关闭 3已推送
        /// </summary>
        public int? DataState { get; set; }

        public bool IsDelete { get; set; }

        /// <summary>
        /// 是否审核 0未审核 1已审核(同意) 2 已审核（拒绝） 
        /// </summary>
        public int? ReviewPush { get; set; }


        public string ReviewUser { get; set; }

        public DateTime? ReviewTime { get; set; }

        public virtual List<NoticeMedical> NoticeMedicals { get; set; }

        public virtual List<UsersMsg> UsersMsgs { get; set; }

    }
}
