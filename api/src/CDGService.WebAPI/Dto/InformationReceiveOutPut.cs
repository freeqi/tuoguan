using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{

    #region 消息中心
    public class InformationReceiveOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string title { get; set; }
        public int GroupType { get; set; }
        public int userType { get; set; }
        public bool expand { get; set; } = false;
        public List<InformationReceiveOutPut> children { get; set; } = new List<InformationReceiveOutPut>();
    }

    public class InformationOutPut
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
        /// 发布人
        /// </summary>
        public string UserName { get; set; }
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
        /// 调价物品明细
        /// </summary>
        public NoticeMedicalOutPut[] noticeMedicalOutPuts { get; set; }

        public UserMsgOutPut[] UsersMsgs { get; set; }
        public int? DataState { get; set; }

        /// <summary>
        /// 是否审核 0未审核 1已审核 2 已推送
        /// </summary>
        public int? ReviewPush { get; set; }
    }
    public class UserMsgOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        /// 接收人ID
        /// </summary>
        public string ReceiveUserId { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 1指定机构、2指定职位、3指定人员
        /// </summary>
        public int ReceiveUserType { get; set; }

        /// <summary>
        /// 是否查看
        /// </summary>
        public bool IsRead { get; set; }
        /// <summary>
        /// 是否回复、反馈
        /// </summary>
        public bool IsFeedback { get; set; }

        /// <summary>
        /// 反馈内容
        /// </summary>
        public string FeedbackContent { get; set; }

        public string BackUserId { get; set; }
        public string BackUserName { get; set; }
        public DateTime? BackTime { get; set; }
        public bool IsSend { get; set; }

    }
    public class InformationInput
    {

        /// <summary>
        /// 消息分类ID 1通知公告 2 调价公告
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
      //  public DateTime? SendTime { get; set; }
        /// <summary>
        /// 是否需要反馈
        /// </summary>
        public bool IsNeedBack { get; set; }
        /// <summary>
        /// 接收人集合
        /// </summary>
        public List<UserGroups> UserIds { get; set; }

        /// <summary>
        /// 调价物品明细
        /// </summary>
        public NoticeMedicalInput[] noticeMedicalInputs { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime OutTime { get; set; }
    }

    public class InventoryInformationInputs
    {
        public List<InventoryInformationInput> inputs { get; set; }
        /// <summary>
        /// 处理截止时间
        /// </summary>
        public DateTime OutTime { get; set; }

    }
    /// <summary>
    /// 库存处发布公告
    /// </summary>
    public class InventoryInformationInput
    {
        /// <summary>
        ///中心ID
        /// </summary>
        public string CenterId { get; set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        public string MedicalId { get; set; }
        public string MedicalName { get; set; }
        /// <summary>
        /// 成本价
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
        /// 规格
        /// </summary>
        public string specifications { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string manufacturer { get; set; }

    }





    public class UserGroups
    {
        /// <summary>
        /// ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 类别 1机构 2 职位 3 人员
        /// </summary>
        public int GroupType { get; set; }
    }


    public class InformationQueryInput
    {
        /// <summary>
        /// 全部或者自己
        /// </summary>
        public int AllOrSelf { get; set; }

        public string id { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 消息类型 1通知公告 2
        /// </summary>
        public int Type { get; set; }
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        /// <summary>
        /// 数据状态（1有效、2已关闭、3已推送）
        /// </summary>
        public int DataState { get; set; }

        /// <summary>
        /// 是否审核 0未审核 1已审核(同意) 2 已审核（拒绝） 
        /// </summary>
        public int ReviewPush { get; set; }
    }

    /// <summary>
    /// 通知反馈
    /// </summary>
    public class InformationFeedback
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] UsersMsgId { get; set; }
        public string CenterId { get; set; }
        /// <summary>
        /// 反馈人id
        /// </summary>
        public string BackUserId { get; set; }
        /// <summary>
        /// 反馈时间
        /// </summary>
        public DateTime? BackTime { get; set; }
        /// <summary>
        /// 反馈内容
        /// </summary>
        public string FeedbackContent { get; set; }

        public bool? IsRead { get; set; }
    }


    /// <summary>
    /// 调价公告中，调价明细单
    /// </summary>
    public class NoticeMedicalInput
    {
        public string Id { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticesId { get; set; }


        /// <summary>
        /// 物品ID
        /// </summary>
        public string MedicalId { get; set; }
        /// <summary>
        /// 成本价
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
    /// <summary>
    /// 调价公告中，调价明细单
    /// </summary>
    public class NoticeMedicalOutPut
    {
        public string Id { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticesId { get; set; }


        /// <summary>
        /// 物品ID
        /// </summary>
        public string MedicalId { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string MedicalName { get; set; }
        /// <summary>
        /// 原售价
        /// </summary>
        public decimal? UpSalePrice { get; set; }

        /// <summary>
        /// 当前售价 -非空
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? PurchasingPrice { get; set; }
        /// <summary>
        /// 限价
        /// </summary>
        public decimal? SocialSecurityPrice { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? DisposeTime { get; set; }
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
    #endregion

    #region  反馈
    public class FeedbackQueryInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 0全部1已解决2未解决
        /// </summary>
        public int type { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }


    }

    public class FeedbackOutPut
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
        public string BackUserName { get; set; }
        /// <summary>
        /// 反馈时间
        /// </summary>
        public DateTime? BackTime { get; set; }
        /// <summary>
        /// 是否关闭（结贴）
        /// </summary>
        public bool IsClose { get; set; }
    }

    public class BackReplyInput
    {
        public string FeedbackId { get; set; }
        /// <summary>
        /// 回复内容信息
        /// </summary>
        public string ReplyMsg { get; set; }

    }


    public class FeedbackReplysOutPut
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
        public string BackUserName { get; set; }
        /// <summary>
        /// 反馈时间
        /// </summary>
        public string BackTime { get; set; }
        /// <summary>
        /// 是否关闭（结贴）
        /// </summary>
        public bool IsClose { get; set; }
        /// <summary>
        /// 回复列表
        /// </summary>
        public FeedbackReplyOutPut[] feedbackReplyOutPuts { get; set; }
    }

    /// <summary>
    ///  反馈回复表
    /// </summary>
    public class FeedbackReplyOutPut
    {

        public string Id { get; set; }
        public string FeedbackId { get; set; }
        /// <summary>
        /// 回复内容信息
        /// </summary>
        public string ReplyMsg { get; set; }
        /// <summary>
        /// 回复人ID
        /// </summary>
        public string ReplyUserId { get; set; }
        public string UserName { get; set; }

        // public sstr

        /// <summary>
        /// 回复时间
        /// </summary>
        public string ReplyTime { get; set; }

    }
    #endregion
}
