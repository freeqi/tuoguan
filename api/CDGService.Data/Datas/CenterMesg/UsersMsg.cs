using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 公告接受人员
    /// </summary>
    public class UsersMsg
    {
        public string Id { get; set; }
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MsgId { get; set; }

        [ForeignKey(nameof(MsgId))]
        public virtual Information Informations { get; set; }
        /// <summary>
        /// 接收人(机构)ID
        /// </summary>
        public string ReceiveUserId { get; set; }
        /// <summary>
        /// 1指定机构、2指定职位、3指定人员
        /// </summary>
        public int ReceiveUserType { get; set; }

        /// <summary>
        /// 是否查看
        /// </summary>
        public bool IsRead { get; set; }
        /// <summary>
        /// 是否需要回复、反馈
        /// </summary>
        public bool IsFeedback { get; set; }

        /// <summary>
        /// 反馈内容
        /// </summary>
        public string FeedbackContent { get; set; }

        public string BackUserId { get; set; }

        public DateTime? BackTime { get; set; }
        public bool IsSend { get; set; }

    }
}
