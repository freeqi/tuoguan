using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 消息反馈表
    /// </summary>
    public class Feedback : ISoftDelete
    {

        public string Id { get; set; }

        /// <summary>
        /// 反馈信息
        /// </summary>
        public string BackMsg { get; set; }
        /// <summary>
        /// 反馈人ID
        /// </summary>
        public string BackUserId { get; set; }
        [ForeignKey(nameof(BackUserId))]
        public virtual Employee user { get; set; }
        /// <summary>
        /// 反馈时间
        /// </summary>
        public DateTime? BackTime { get; set; }

        public bool IsClose { get; set; }

        public bool IsDelete { get; set; }
        public virtual List<FeedbackReply> feedbackReplys { get; set; } = new List<FeedbackReply>();
    }
    /// <summary>
    ///  反馈回复表
    /// </summary>
    public class FeedbackReply

    {

        public string Id { get; set; }
        public string FeedbackId { get; set; }
        [ForeignKey("FeedbackId")]
        public virtual Feedback feedback { get; set; }
        /// <summary>
        /// 回复内容信息
        /// </summary>
        public string ReplyMsg { get; set; }
        /// <summary>
        /// 回复人ID
        /// </summary>
        public string ReplyUserId { get; set; }
        [ForeignKey(nameof(ReplyUserId))]
        public virtual Employee user { get; set; }
        // public sstr

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ReplyTime { get; set; }

    }

}
