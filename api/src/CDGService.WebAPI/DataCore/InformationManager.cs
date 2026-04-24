﻿﻿using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Data.Helper;
using CDGService.WebAPI.Datas;
using System.Linq.Expressions;
using CDGService.Utils;
using CDGService.Application.DataCollect;

namespace CDGService.WebAPI.DataCore
{
    public class InformationManager
    {
        private readonly Data.DocumentSetting _documentSetting;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly CollentStartup _purchasManagerService;
        private readonly IMapper _mapper;
        public InformationManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<Data.DocumentSetting> documentSetting, CollentStartup purchasManagerService, IMapper mapper)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _documentSetting = documentSetting.Value;
            _purchasManagerService = purchasManagerService;
            _mapper = mapper;
        }

        //InformationReceiveOutPut
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<Employee> EmployeeStore => _unitOfWork.GetStore<Employee>();

        private IRepository<SystemDictionary> SystemDictionaryStore => _unitOfWork.GetStore<SystemDictionary>();
        private IRepository<Information> InformationStore => _unitOfWork.GetStore<Information>();
        private IRepository<NoticeMedical> NoticeMedicalStore => _unitOfWork.GetStore<NoticeMedical>();
        private IRepository<MedicalItemRecord> MedicalItemRecordStore => _unitOfWork.GetStore<MedicalItemRecord>();
        private IRepository<UsersMsg> UsersMsgStore => _unitOfWork.GetStore<UsersMsg>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<Feedback> FeedbackStore => _unitOfWork.GetStore<Feedback>();
        private IRepository<FeedbackReply> FeedbackReplyStore => _unitOfWork.GetStore<FeedbackReply>();
        private IRepository<SI_YPMLS> SI_YPMLSStore => _unitOfWork.GetStore<SI_YPMLS>();

        private IRepository<SI_CheckPriceInfo> SI_CheckPriceInfoStore => _unitOfWork.GetStore<SI_CheckPriceInfo>();

        public Task<InformationReceiveOutPut[]> GetInformationAsync(int selType)
        {
            return Task.Run(async () =>
            {
                // string 
                string workState = "e81a52d7e88e49a28dd536548ca8fe8a";//离职状态
                List<InformationReceiveOutPut> outPuts = new List<InformationReceiveOutPut>();
                var emp = await EmployeeStore.Entities.Where(t => t.IsDelete == false && t.DataState
                == 1 && t.WorkingState != workState).ToListAsync();
                switch (selType)
                {
                    case 1://按照机构分组   
                        var cen = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.DataState
              == 1 &&  t.ShortName != "九龙坡透析中心").ToListAsync();
                        foreach (var item in cen)
                        {
                            var cenUser = emp.Where(t => t.CenterDialysisId == item.Id);
                            List<InformationReceiveOutPut> users = new List<InformationReceiveOutPut>();
                            foreach (var items in cenUser)
                            {
                                users.Add(new InformationReceiveOutPut() { Id = items.Id, title = items.Name, GroupType = 1, userType = 3 });
                            }
                            if (users.Count > 0)
                                outPuts.Add(new InformationReceiveOutPut()
                                {
                                    Id = item.Id,
                                    title = item.ShortName,
                                    GroupType = 1,
                                    userType = 1,
                                    children = users,
                                });
                        }
                        List<InformationReceiveOutPut> cusers = new List<InformationReceiveOutPut>();
                        var cenUsers = emp.Where(t => t.CenterDialysisId == "" || t.CenterDialysisId == null);
                        foreach (var items in cenUsers)
                        {
                            cusers.Add(new InformationReceiveOutPut() { Id = items.Id, title = items.Name, GroupType = 1, userType = 3 });
                        }
                        outPuts.Insert(0, new InformationReceiveOutPut()
                        {
                            Id = "0",
                            title = "集团总部",
                            GroupType = 1,
                            userType = 1,
                            children = cusers,
                        });
                        break;
                    case 2://职务分组
                        string TypeId = "cb30fac65abb48f1ba5b276d1fa76730";// order by ShowSortNo

                        var pid = await SystemDictionaryStore.Entities.Where(t => t.TypeId == TypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                        foreach (var item in pid)
                        {
                            var cenUser = emp.Where(t => t.positionId == item.Id);
                            List<InformationReceiveOutPut> users = new List<InformationReceiveOutPut>();
                            foreach (var items in cenUser)
                            {
                                users.Add(new InformationReceiveOutPut() { Id = items.Id, title = items.Name, GroupType = 2, userType = 3 });
                            }
                            if (users.Count > 0)

                                outPuts.Add(new InformationReceiveOutPut()
                                {
                                    Id = item.Id,
                                    title = item.Name,
                                    GroupType = 2,
                                    userType = 2,
                                    children = users,
                                });
                        }

                        break;
                    default:
                        break;
                }


                return outPuts.ToArray();

            });
        }

        /// <summary>
        /// 添加公告发布
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<bool> SendMesgAsync(InformationInput input)
        {
            return Task.Run(async () =>
            {
                //(调价公告)消息重复发送，集团端应该给出提示。不能重复发送
                if (input.MsgContent == "<p><br></p>")
                    throw new Exception($"通知内容不能为空");
                if (input.MsgTypeId == 2)
                {
                    if (input.noticeMedicalInputs.Length == 0)
                        throw new Exception($"调价公告，调价明细非空");
                    //筛去重复物品  透析中心
                    List<string> medicId = input.noticeMedicalInputs.Select(t => t.MedicalId).ToList();
                    //找出存在且未过调价期的物品
                    var MedicData = await NoticeMedicalStore.Entities.Include(t => t.information).Where(t => medicId.Contains(t.MedicalId) && t.OutTime > DateTime.Now && t.DataState == 1 && t.information.ReviewPush != 2).ToListAsync();

                    List<NoticeMedical> noticeMedicals = new List<NoticeMedical>();
                    foreach (var item in input.noticeMedicalInputs)
                    {
                        var mdata = MedicData.Where(t => t.MedicalId == item.MedicalId && t.UpSalePrice == item.UpSalePrice).FirstOrDefault();
                        if (mdata != null)
                            noticeMedicals.Add(mdata);
                    }        

                    if (noticeMedicals != null && noticeMedicals.Count > 0)
                    {

                        List<string> noticeId = noticeMedicals.Select(t => t.NoticesId).CompareDistinct(t => t).ToList();
                        var UsermsgData = await UsersMsgStore.Entities.Where(t => noticeId.Contains(t.MsgId)).ToListAsync();
                        List<string> Userids = input.UserIds.Select(t => t.UserId).ToList();
                        if (UsermsgData != null && UsermsgData.FindAll(t => Userids.Contains(t.ReceiveUserId)).Count > 0)
                        {
                            var str = noticeMedicals.Select(t => t.MedicalId).ToList();

                            var med = await MedicalItemRecordStore.Entities.Where(t => str.Contains(t.Id)).ToListAsync();
                            string MedName = "";
                            foreach (var item in med)
                            {

                                MedName += item.MedicalItemName + ";";
                            }

                            throw new Exception($"存在同一物品在调价有效期内，请勿重复发布！物品明细为：{MedName}");
                        }
                    }
                }
                bool Flag = true;
                string userId = await _getUserInfo.GetCurrentUserIdAsync();
                var data = _mapper.Map<Information>(input);
                data.Id = Guid.NewGuid().tostring32();
                data.Founder = userId;
                data.FounderDate = DateTime.Now;
                data.Modifier = userId;
                data.ModifierDate = DateTime.Now;
                //data.SendTime = DateTime.Now;
                data.DataState = 1;
                data.IsDelete = false;
                data.IsNeedBack = true;
                data.IsRead = 2;
                data.ReviewPush = 0;
                InformationStore.Insert(data);
                //人员  
                List<UsersMsg> usersMsgs = new List<UsersMsg>();
                if (input.UserIds != null && input.UserIds.Count > 0)
                {

                    foreach (var item in input.UserIds)
                    {
                        usersMsgs.Add(new UsersMsg()
                        {
                            Id = Guid.NewGuid().tostring32(),
                            IsFeedback = true,
                            IsRead = false,
                            MsgId = data.Id,
                            ReceiveUserType = item.GroupType,
                            ReceiveUserId = item.UserId,//TODO 需根据组ID找到人员ID
                            IsSend = false,
                        });
                    }
                    UsersMsgStore.Insert(usersMsgs);

                }
                List<NoticeMedical> medicals = new List<NoticeMedical>();
                //调价目录 
                if (input.noticeMedicalInputs != null && input.noticeMedicalInputs.Length > 0)
                {
                    var noticeMedical = _mapper.Map<NoticeMedical[]>(input.noticeMedicalInputs);
                    foreach (var item in noticeMedical)
                    {
                        if (item.SalePrice == item.UpSalePrice)
                            continue;
                        item.Founder = userId;
                        item.DataState = 1;
                        item.FounderDate = DateTime.Now;
                        item.Modifier = userId;
                        item.ModifierDate = DateTime.Now;
                        item.Id = Guid.NewGuid().tostring32();
                        item.NoticesId = data.Id;
                        item.OutTime = input.OutTime.AddDays(+1).AddMinutes(-1);
                        item.IsRead = 2;
                        item.IsNeedBack = true;
                        NoticeMedicalStore.Insert(item);
                        medicals.Add(item);
                    }
                }
                _unitOfWork.SaveChanges();

                // await _purchasManagerService.SendPublishNotices(new CenterNotices() { information = data, noticeMedicals = medicals, usersMsgs = usersMsgs });
                return Flag;

            });
        }


        //审核 
        public Task<bool> MesgAudit(string InfoId, int State)
        {
            return Task.Run(async () =>
            {

                bool Flag = true;
                var data = await InformationStore.GetFirstOrDefaultAsync(t => t.Id == InfoId);

                if (data != null)
                {
                    string userId = await _getUserInfo.GetCurrentUserIdAsync();
                    data.ReviewPush = State;
                    data.ReviewUser = userId;
                    data.ReviewTime = DateTime.Now;
                    InformationStore.Update(data);
                    var result = _unitOfWork.SaveChanges();
                    Flag = result > 0;
                    if (result > 0 && State == 1) //2022年4月2日  审批同意后自动推送 
                        await MesgSendAsync(InfoId);
                }

                return Flag;

            });
        }

        //推送   
        public Task<bool> MesgSendAsync(string InfoId)
        {
            return Task.Run(async () =>
            {

                bool Flag = true;
                var data = await InformationStore.Entities.Include(t => t.NoticeMedicals).Include(t => t.UsersMsgs).Where(t => t.Id == InfoId).FirstOrDefaultAsync();
                if (data != null)
                {
                    string userId = await _getUserInfo.GetCurrentUserIdAsync();
                    data.SendTime = DateTime.Now;
                    data.DataState = 3;
                    InformationStore.Update(data);
                    _unitOfWork.SaveChanges();
                    await _purchasManagerService.SendPublishNotices(new CenterNotices() { information = data, noticeMedicals = data.NoticeMedicals, usersMsgs = data.UsersMsgs.Where(t => t.IsSend == false).ToList() });
                }
                return Flag;

            });
        }


        public Task<bool> MesgSendsAsync(string[] InfoId)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                foreach (var item in InfoId)
                {
                    var data = await InformationStore.Entities.Include(t => t.NoticeMedicals).Include(t => t.UsersMsgs).Where(t => t.Id == item).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        string userId = await _getUserInfo.GetCurrentUserIdAsync();
                        data.SendTime = DateTime.Now;
                        data.DataState = 3;
                        InformationStore.Update(data);
                        _unitOfWork.SaveChanges();
                        await _purchasManagerService.SendPublishNotices(new CenterNotices() { information = data, noticeMedicals = data.NoticeMedicals, usersMsgs = data.UsersMsgs.Where(t => t.IsSend == false).ToList() });

                    }
                }

                return Flag;

            });
        }
        /// <summary>
        /// 库存处发布调价公告
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Task<bool> InventoryToSendMesgAsync(InventoryInformationInputs inputs)
        {

            bool Falg = false;
            return Task.Run(async () =>
            {
                try
                {
                    if (inputs == null || inputs.inputs.Count <= 0)
                        throw new Exception($"调价明细非空");

                    inputs.inputs.RemoveAll(t => t.MedicalId + "" == "" || t.CenterId + "" == "");
                    inputs.inputs.RemoveAll(t => t.SalePrice == t.UpSalePrice); //异常原价和现价相等的
                    var medData = inputs.inputs.GroupBy(t => new { t.CenterId });
                    string userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var cender = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToListAsync();
                    foreach (var item in medData)
                    {
                        var cen = cender.Where(t => t.Id == item.Key.CenterId).FirstOrDefault();
                        string mName = "";
                        if (item.Count() == 1)
                            mName = $"【{item.FirstOrDefault().MedicalName}】";
                        InformationInput informationInput = new InformationInput()
                        {

                            IsNeedBack = true,

                            MsgTitle = $"{cen.ShortName}{mName}售价异常公告",
                            MsgTypeId = 2,
                            OutTime = inputs.OutTime,
                            UserIds = new List<UserGroups>()
                        {
                                new UserGroups() { GroupType = 1, UserId = item.First().CenterId }  },
                        };
                        List<NoticeMedicalInput> noticeMedicals = new List<NoticeMedicalInput>();
                        int i = 1;
                        string MsgContent = "<ul>";
                        /*
                         <ul><li>
1.物品<span style="font-style: italic;">柴胡注射液  (2ml/支)  (河南润弘制药股份有限公司) 2ml*10支/盒 [CHZSY]</span>&nbsp; &nbsp;<span style="font-weight: bold;">成本价为</span><span style="font-weight:bold;">：1.33元</span>，<span style="font-weight: bold;">原销售价格</span><span style="font-weight: bold;">：2.8元</span>，<span style="font-weight: bold;">调整为：3 元</span>。</li>
<li>
2.物品<span style="font-style: italic;">非布司他片  (40mg*7片/盒)  (江苏万邦生化医药集团有限责任公司) 40mg*7片/盒 [FBSTP]</span>&nbsp; &nbsp;<span style="font-weight: bold;">成本价为</span><span style="font-weight:bold;">：6.9元</span>，<span style="font-weight: bold;">原销售价格</span><span style="font-weight: bold;">：8元</span>，<span style="font-weight: bold;">调整为：7.5 元</span>。
</li>
</ul>
<h3 style="text-align: right;">调价截止日期：2019-12-13</h3> 
                         */
                        foreach (var detail in item)
                        {
                            if (noticeMedicals.FindAll(t => t.MedicalId == detail.MedicalId && t.UpSalePrice == detail.UpSalePrice && t.SalePrice == detail.SalePrice && t.PurchasingPrice == detail.PurchasingPrice).Count > 0)
                                continue;

                            MsgContent += $" <li>{i}物品<span style=\"font-style: italic; \">{detail.MedicalName}  ({detail.manufacturer}) {detail.specifications} </span>&nbsp; &nbsp;<span style=\"font-weight: bold; \">成本价为</span><span style=\"font-weight:bold; \">：{detail.PurchasingPrice}元</span>，<span style=\"font-weight: bold; \">原销售价格</span><span style=\"font-weight: bold; \">：{detail.UpSalePrice}元</span>，<span style=\"font-weight: bold; \">调整为：{detail.SalePrice} 元</span>。</li>";
                            noticeMedicals.Add(new NoticeMedicalInput()
                            {
                                DataState = 1,
                                Founder = userId,
                                FounderDate = DateTime.Now,
                                MedicalId = detail.MedicalId,
                                Modifier = userId,
                                ModifierDate = DateTime.Now,
                                PurchasingPrice = detail.PurchasingPrice,
                                SalePrice = detail.SalePrice,
                                UpSalePrice = detail.UpSalePrice,

                            }); 
                            i++;
                        }
                        MsgContent += $" </ul><h3 style = \"text-align: right;\" > 调价截止日期：{inputs.OutTime.ToString("yyyy-MM-dd HH:mm:ss")} </h3>";
                        informationInput.noticeMedicalInputs = noticeMedicals.ToArray();
                        informationInput.MsgContent = MsgContent;
                        await SendMesgAsync(informationInput);
                    }

                    return Falg;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            });
        }


        /// <summary>
        /// 关闭公告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> CloseMsgAsync(string id)
        {
            return Task.Run(async () =>
            {
                var data = await InformationStore.Entities.Include(t => t.NoticeMedicals).Include(t => t.UsersMsgs).Include(t => t.user).ThenInclude(t => t.Employee).Where(t => t.Id == id).FirstOrDefaultAsync();
                int DataState = data.DataState.Value;
                if (data.UsersMsgs.Where(t => t.BackUserId + "" != "").Count() > 0)
                {
                    throw new Exception($"该公告已有中心提供反馈，不可关闭");
                }
                if (data.NoticeMedicals != null && data.NoticeMedicals.FindAll(t => t.DisposeTime.HasValue).Count > 0)
                    throw new Exception($"该公告已有物品执行了调价，不可关闭");
                data.DataState = 2;
                //data.UsersMsgs.ForEach(t=>t.data)
                data.NoticeMedicals.ForEach(t => t.DataState = 2);
                InformationStore.Update(data);
                _unitOfWork.SaveChanges();
                if (DataState == 3)//已推送的才对接中心端关闭
                    await _purchasManagerService.SendPublishNotices(new CenterNotices() { information = data, noticeMedicals = data.NoticeMedicals, usersMsgs = data.UsersMsgs });
                return true;
            });

        }


        //公告列表 InformationQueryInput  InformationOutPut

        /// <summary>
        /// 获取公告列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>                                                                                                        

        public Task<PageData<InformationOutPut[]>> GetMesgAsync(InformationQueryInput input)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                InformationOutPut[] result = null;

                int count = 0;
                try
                {

                    Expression<Func<Information, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrEmpty(input.Remark))
                        predicate = predicate.And(t => t.MsgTitle.Contains(input.Remark));
                    //if (input.Type > 0)
                    //    predicate = predicate.And(t => t.MsgTypeId == input.Type);
                    if (input.id + "" != "")
                        predicate = predicate.And(t => t.Id == input.id);

                    if (input.AllOrSelf > 0)
                    {
                        string userid = await _getUserInfo.GetCurrentUserIdAsync();
                        predicate = predicate.And(t => t.Founder == userid);
                    }

                    if (input.DataState > 0)
                    {
                        predicate = predicate.And(t => t.DataState == input.DataState);

                    }
                    if (input.ReviewPush != 3)
                    {
                        predicate = predicate.And(t => t.ReviewPush == input.ReviewPush);

                    }
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).ToListAsync();
                    var EmpData = await EmployeeStore.Entities.ToListAsync();
                    var data = InformationStore.Entities.Include(t => t.NoticeMedicals).Include(t => t.UsersMsgs).Include(t => t.user).ThenInclude(t => t.Employee).Where(predicate).OrderByDescending(t => t.FounderDate);

                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        var msgData = await PaginatedList<Information>.CreateAsync(data, input.PageNum, input.PageSize);
                        result = _mapper.Map<InformationOutPut[]>(msgData);
                        count = msgData.Count;
                    }
                    else
                    {
                        result = _mapper.Map<InformationOutPut[]>(data.ToArrayAsync().Result);

                        count = result.Length;
                    }
                    foreach (var item in result)
                    {
                        foreach (var userMsg in item.UsersMsgs)
                        {
                            if (userMsg.BackUserId + "" != "")
                                userMsg.BackUserName = EmpData.FirstOrDefault(t => t.Id == userMsg.BackUserId) != null ? EmpData.FirstOrDefault(t => t.Id == userMsg.BackUserId).Name : "";
                            userMsg.CenterName = centerData.FirstOrDefault(t => t.Id == userMsg.ReceiveUserId)?.ShortName;

                        }
                        foreach (var med in item.noticeMedicalOutPuts)
                        {
                            var medData = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.Id == med.MedicalId);
                            if (medData.MedicalItemType == 1)
                            {
                                var YpData = await SI_CheckPriceInfoStore.Entities.Where(t => t.hilist_code == medData.NationItemCode && t.hilist_lmtpric_type == "901" && t.enddate == null).FirstOrDefaultAsync();
                                med.SocialSecurityPrice = YpData != null ? YpData.hilist_pric_uplmt_amt : 0;
                            }
                            med.MedicalName = medData.MedicalItemName;
                        }


                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return new PageData<InformationOutPut[]>(result, count);
            });
        }

        //消息反馈
        //反馈列表FeedbackQueryInput  FeedbackOutPut

        public Task<PageData<FeedbackOutPut[]>> GetFeedbackMesgAsync(FeedbackQueryInput input)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                FeedbackOutPut[] result = null;

                int count = 0;
                try
                {
                    Expression<Func<Feedback, bool>> predicate = t => t.IsDelete == IsDel;

                    if (input.type == 1)
                        predicate = predicate.And(t => t.IsClose == true);
                    if (input.type == 2)
                        predicate = predicate.And(t => t.IsClose == false);
                    if (input.Id + "" != "")
                        predicate = predicate.And(t => t.Id == input.Id);


                    var data = FeedbackStore.Entities.Include(t => t.user).Where(predicate);
                    count = data.Count();
                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        var msgData = await PaginatedList<Feedback>.CreateAsync(data, input.PageNum, input.PageSize);
                        result = _mapper.Map<FeedbackOutPut[]>(msgData);
                    }
                    else
                        result = _mapper.Map<FeedbackOutPut[]>(data.ToArrayAsync().Result);

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return new PageData<FeedbackOutPut[]>(result, count);
            });
        }

        //回复反馈信息 BackInput  

        public Task<bool> BackAddAsync(BackReplyInput input)
        {
            return Task.Run(async () =>
            {
                var user = await _getUserInfo.GetUserAsync();
                FeedbackReply reply = new FeedbackReply()
                {
                    FeedbackId = input.FeedbackId,
                    Id = Guid.NewGuid().tostring32(),
                    ReplyMsg = input.ReplyMsg,
                    ReplyUserId = user.EmployeeId,
                    ReplyTime = DateTime.Now,
                };
                FeedbackReplyStore.Insert(reply);
                _unitOfWork.SaveChanges();
                return true;
            });
        }

        //详情 

        public Task<FeedbackReplysOutPut> GetBackDetailAsync(string input)
        {
            return Task.Run(async () =>
            {
                FeedbackReplysOutPut outPut = null;

                var data = await FeedbackStore.Entities.Include(t => t.user).Include(t => t.feedbackReplys).FirstOrDefaultAsync(t => t.Id == input);

                outPut = _mapper.Map<FeedbackReplysOutPut>(data);
                return outPut;
            });
        }

        //关闭
        public Task<bool> BackUpdaeStateAsync(string Id)
        {
            return Task.Run(async () =>
            {
                var user = await _getUserInfo.GetUserAsync();
                var data = await FeedbackStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                data.IsClose = true;
                FeedbackStore.Update(data);
                _unitOfWork.SaveChanges();
                return true;
            });
        }




        //接受反馈

        /// <summary>
        /// 接收中心端反馈信息
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public Task<bool> InformationFeedbackasync(InformationFeedback feedback)
        {
            return Task.Run(async () =>
            {
                try
                {
                     
                    if (feedback.UsersMsgId == null || feedback.UsersMsgId.Length == 0)
                        throw new Exception($"UsersMsgId[]非空");
                    var MsgData = await UsersMsgStore.Entities.Where(t => (feedback.UsersMsgId.Contains(t.MsgId) || feedback.UsersMsgId.Contains(t.Id)) && t.ReceiveUserId == feedback.CenterId).ToListAsync();
                    if (MsgData != null)
                    {
                        MsgData.ForEach(t =>
                        {
                            t.FeedbackContent = feedback.FeedbackContent; t.BackTime = feedback.BackTime; t.BackUserId = feedback.BackUserId;
                            if (feedback.IsRead.HasValue)
                                t.IsRead = feedback.IsRead.Value;
                        });
                        //MsgData.FeedbackContent
                        // = feedback.FeedbackContent;
                        //MsgData.BackTime = feedback.BackTime;
                        //MsgData.BackUserId = feedback.BackUserId;
                        UsersMsgStore.Update(MsgData);
                        //
                        //List<string> noticeId = MsgData.Select(t => t.MsgId).ToList();
                        //var infromation = await InformationStore.Entities.Include(t => t.NoticeMedicals).Where(t => noticeId.Contains(t.Id)).ToListAsync();
                        //foreach (var item in infromation)
                        //{
                        //    item.IsRead = feedback.IsRead;
                        //    if (item.NoticeMedicals != null && item.NoticeMedicals.Count > 0)
                        //   item.NoticeMedicals.ForEach(t => t.IsRead = feedback.IsRead);
                        //    InformationStore.Update(infromation);
                        //}
                        _unitOfWork.SaveChanges();
                        return true;
                    }
                    else
                    {
                        throw new Exception($"未能获取到当前ID的接收明细");
                    }


                }
                catch (Exception exp)
                {
                    throw new Exception($"内部错误：{exp.Message}");
                }


            });
        }

    }
}
