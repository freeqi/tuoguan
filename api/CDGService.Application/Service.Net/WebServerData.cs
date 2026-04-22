using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Helper;
using CDGService.Data.Store;
using Newtonsoft.Json;

namespace CDGService.Application.Service.Net
{
    public class WebServerData : IDataReceive
    {
        private readonly WebDataUrl _webUrls;
        private readonly IServiceProvider _sp;
        private readonly HttpClient _client;
        private readonly IUnitOfWork _unitOfWork;
        private List<CenterUser> _MyCenterUsers;
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private List<CenterDialysis> ListcenterDialyses;
        private IRepository<CenterPublishLog> CenterPublishLogStore => _unitOfWork.GetStore<CenterPublishLog>();
        private IRepository<UsersMsg> UsersMsgStore => _unitOfWork.GetStore<UsersMsg>();
        public WebServerData(WebDataUrl webUrl, IServiceProvider sp, IUnitOfWork unitOfWork, Microsoft.Extensions.Options.IOptions<List<CenterUser>> _CenterUsers)
        {
            _webUrls = webUrl;
            _sp = sp;
            _unitOfWork = unitOfWork;
            _MyCenterUsers = _CenterUsers.Value;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            /*
                

             */
            ListcenterDialyses = CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && t.CenterUrl != "").ToList();

        }
        /// <summary>
        /// 将特定 token 加入 header
        /// </summary>
        /// <param name="token"></param>
        public bool AddToken(string token, string ClientType, string Account)
        {
            token = "e2cabdfa-9a66-4697-8609-5acfad891d5b";
            if (_client.DefaultRequestHeaders.Contains("Token"))
                _client.DefaultRequestHeaders.Remove("Token");
            _client.DefaultRequestHeaders.Add("Token", token);
            //if (_client.DefaultRequestHeaders.Contains("Account"))
            //    _client.DefaultRequestHeaders.Remove("Account");
            //_client.DefaultRequestHeaders.Add("Account", Account);
            if (_client.DefaultRequestHeaders.Contains("ClientType"))
                _client.DefaultRequestHeaders.Remove("ClientType");
            _client.DefaultRequestHeaders.Add("ClientType", "JT");

            return true;
        }
        public Task<CenterEmployee[]> GetEmployeeList()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _client.SendGetAsync<CenterEmployee[]>(_webUrls.EmployeeUrl);

                    switch (result.Code)
                    {
                        case 200:
                            return result.Data.ToArray();
                            break;
                        default:
                            throw new Exception("未能获取到数据");
                            break;
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }

        public Task<CenterLogionResult> UserLoginAsync(string username, string password)
        {
            return Task.Run(async () =>
            {

                var result = await _client.SendPostAsync<CenterLogionResult>(_webUrls.UserLoginUrl(username, password));

                switch (result.Code)
                {
                    case 200:
                        return result.Data;
                        break;
                    default:
                        return null;
                        break;
                }

            });
        }
        #region  报废、滞销

        //报废审核
        public Task<bool> GroupUpdateMatState(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;

                        AddToken("", "PC", "");

                        string jsonData = JsonConvert.SerializeObject(pas);

                        var result = await _client.SendPutContentAsync<object>(_webUrls.GroupUpdateMatAuditUrl, jsonData);
                        if (result != null && result.Code == 200)
                            return true;
                        else
                            return false;

                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return true;
            });


        }

        //滞销审核
        public Task<bool> GroupUpdateMaterialsWarningState(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //var data = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    //_webUrls.BaseUrl = data.CenterUrl;//"http://localhost:60184/api/v1";//http://192.168.10.231:9003/api/v1
                    //{"GroupAuditStatus":"","GroupAuditDate":"","GroupAdvice":"","GroupAuditor":"","Id":""}
                    // _webUrls.ApprovalPurchaseRequestUrl;
                    //  var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;

                        AddToken("", "PC", "");

                        string jsonData = JsonConvert.SerializeObject(pas);

                        var result = await _client.SendPutContentAsync<object>(_webUrls.GroupUpdateMaterialsWarningAuditUrl, jsonData);
                        if (result != null && result.Code == 200)
                            return true;
                        else
                            return false;

                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return true;
            });


        }

        public Task<bool> MaterialPendingApproval(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;

                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            await _client.SendPutContentAsync<object>(_webUrls.MaterialPendingApprovalUrl, jsonData);
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });

        }

        public Task<bool> WarningPendingApproval(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;

                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            await _client.SendPutContentAsync<object>(_webUrls.WarningPendingApprovalUrl, jsonData);
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });

        }



        //报废同步
        /// <summary>
        /// 报废同步
        /// </summary>
        /// <returns></returns>
        public Task<MaterialApplyApprove[]> GetMaterialApplyApproveAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<MaterialApplyApprove> purchaseRequests = new List<MaterialApplyApprove>();

                    //string UserName = "super_admin".Encrypt();
                    //string Pwd = "123456".Encrypt();
                    //if (item.CenterUrl + "" == "")
                    //{
                    //    continue;
                    //}
                    //_webUrls.BaseUrl = item.CenterUrl;

                    var result = await _client.SendGetAsync<MaterialApplyApprove[]>(_webUrls.GetGetMaterialApplyApproveUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                // result.Data.ToList().ForEach(t => t.CenterId = item.Id);
                                purchaseRequests.InsertRange(0, result.Data);
                                break;
                            default:
                                break;
                        }

                    return purchaseRequests.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }
        public Task<MaterialApplyApproveDetail[]> GetMaterialApplyApproveDetailAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<MaterialApplyApproveDetail> purchaseRequests = new List<MaterialApplyApproveDetail>();

                    //string UserName = "super_admin".Encrypt();
                    //string Pwd = "123456".Encrypt();
                    //if (item.CenterUrl + "" == "")
                    //{
                    //    continue;
                    //}
                    //_webUrls.BaseUrl = item.CenterUrl;

                    var result = await _client.SendGetAsync<MaterialApplyApproveDetail[]>(_webUrls.GetMaterialApplyApproveDetailUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                // result.Data.ToList().ForEach(t => t.CenterId = item.Id);
                                purchaseRequests.InsertRange(0, result.Data);
                                break;
                            default:
                                break;
                        }

                    return purchaseRequests.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }

        //滞销同步

        public Task<MaterialsWarningApplyList[]> GetMaterialsWarningApplyListAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<MaterialsWarningApplyList> purchaseRequests = new List<MaterialsWarningApplyList>();

                    //string UserName = "super_admin".Encrypt();
                    //string Pwd = "123456".Encrypt();
                    //if (item.CenterUrl + "" == "")
                    //{
                    //    continue;
                    //}
                    //_webUrls.BaseUrl = item.CenterUrl;

                    var result = await _client.SendGetAsync<MaterialsWarningApplyList[]>(_webUrls.GetMaterialsWarningApplyListUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                // result.Data.ToList().ForEach(t => t.CenterId = item.Id);
                                purchaseRequests.InsertRange(0, result.Data);
                                break;
                            default:
                                break;
                        }

                    return purchaseRequests.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }
        public Task<MaterialsWarningApplyDetailList[]> GetMaterialsWarningApplyDetailListAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<MaterialsWarningApplyDetailList> purchaseRequests = new List<MaterialsWarningApplyDetailList>();

                    //string UserName = "super_admin".Encrypt();
                    //string Pwd = "123456".Encrypt();
                    //if (item.CenterUrl + "" == "")
                    //{
                    //    continue;
                    //}
                    //_webUrls.BaseUrl = item.CenterUrl;

                    var result = await _client.SendGetAsync<MaterialsWarningApplyDetailList[]>(_webUrls.GetMaterialsWarningApplyDetailListUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                // result.Data.ToList().ForEach(t => t.CenterId = item.Id);
                                purchaseRequests.InsertRange(0, result.Data);
                                break;
                            default:
                                break;
                        }

                    return purchaseRequests.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }

        #endregion





        /// <summary>
        /// 采购申请单
        /// </summary>
        /// <returns></returns>
        public Task<PurchaseRequest[]> GetPurchasManagerAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<PurchaseRequest> purchaseRequests = new List<PurchaseRequest>();

                    //string UserName = "super_admin".Encrypt();
                    //string Pwd = "123456".Encrypt();
                    //if (item.CenterUrl + "" == "")
                    //{
                    //    continue;
                    //}
                    //_webUrls.BaseUrl = item.CenterUrl;

                    var result = await _client.SendGetAsync<PurchaseRequest[]>(_webUrls.PurchasManagerUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                // result.Data.ToList().ForEach(t => t.CenterId = item.Id);
                                purchaseRequests.InsertRange(0, result.Data);
                                break;
                            default:
                                break;
                        }

                    return purchaseRequests.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }

        /// <summary>
        /// 采购申请单明细
        /// </summary>
        /// <returns></returns>
        public Task<PurchaseDetail[]> GetPurchaseDetailDataAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<PurchaseDetail> ListPurchaseDetail = new List<PurchaseDetail>();


                    var result = await _client.SendGetAsync<PurchaseDetail[]>(_webUrls.PurchaseDetailUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                ListPurchaseDetail.InsertRange(0, result.Data);
                                break;
                        }

                    return ListPurchaseDetail.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }


        public Task<Supplier[]> GetSupplierDataAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<Supplier> ListSupplier = new List<Supplier>();


                    var result = await _client.SendGetAsync<Supplier[]>(_webUrls.SupplierUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                ListSupplier.InsertRange(0, result.Data);
                                break;
                        }

                    return ListSupplier.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }



        public Task<bool> GetSIBaseDataAsync(string HisCode)
        {

            return Task.Run(async () =>
            {
                try
                {


                    var result = await _client.SendGetAsync<bool>(_webUrls.SIBaseDataUrl + HisCode);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                return true;
                                break;
                        }


                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }
                return false;

            });

        }






        public Task<MedicalItemRecord[]> GetMedicalItemRecordDataAsync()
        {

            return Task.Run(async () =>
            {
                try
                {
                    List<MedicalItemRecord> ListMedicalItemRecord = new List<MedicalItemRecord>();


                    var result = await _client.SendGetAsync<MedicalItemRecord[]>(_webUrls.MedicalItemRecordsUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                ListMedicalItemRecord.InsertRange(0, result.Data);
                                break;
                        }

                    return ListMedicalItemRecord.ToArray();
                }
                catch (Exception exp)
                {

                    throw new Exception(exp.Message, exp);
                }

            });
        }


        /// <summary>
        /// 采购审批
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<bool> UpdatePurchasState(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //var data = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    //_webUrls.BaseUrl = data.CenterUrl;//"http://localhost:60184/api/v1";//http://192.168.10.231:9003/api/v1
                    //{"GroupAuditStatus":"","GroupAuditDate":"","GroupAdvice":"","GroupAuditor":"","Id":""}
                    // _webUrls.ApprovalPurchaseRequestUrl;
                    //  var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;

                        AddToken("", "PC", "");

                        string jsonData = JsonConvert.SerializeObject(pas);

                        var result = await _client.SendPutContentAsync<object>(_webUrls.ApprovalPurchaseRequestUrl, jsonData);
                        if (result != null && result.Code == 200)
                            return true;
                        else
                            return false;

                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return true;
            });


        }

        /// <summary>
        /// 采购单审批调整物品价格
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        public Task<bool> UpdatePurchasPriceState(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //var data = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    //_webUrls.BaseUrl = data.CenterUrl;//"http://localhost:60184/api/v1";//http://192.168.10.231:9003/api/v1
                    //{"GroupAuditStatus":"","GroupAuditDate":"","GroupAdvice":"","GroupAuditor":"","Id":""}
                    // _webUrls.ApprovalPurchaseRequestUrl; 

                    //  var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;

                        AddToken("", "", "");
                        string jsonData = JsonConvert.SerializeObject(pas);

                        var data = await _client.SendPutContentAsync<object>(_webUrls.PurchasePriceUrl, jsonData);
                        if (data != null && data.Code == 200)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        //删除采购明细
        public Task<bool> DelPurchaseDetails(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {

                    //   var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        //登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        AddToken("", "", "");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);
                        string jsonData = JsonConvert.SerializeObject(pas);
                        await _client.SendDelContentAsync<object>(_webUrls.DelPurchaseDetailsUrl, jsonData);

                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        /// <summary>
        /// 添加采购明细
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<bool> AddPurchaseDetails(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {

                    // var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        //登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        //AddToken("", data.ClientType, data.userName + "|");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);

                        //if (data != null)
                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            var restul = await _client.SendPOSTContentAsync<object>(_webUrls.AddPurchaseDetailsUrl, jsonData);
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }

        /// <summary>
        /// 关闭中心端采购明细
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<ServiceMessage<object>> ClosePurchaseDetails(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {

                    // var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        ////登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        //AddToken("", data.ClientType, data.userName + "|");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);

                        //if (data != null)
                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            var rest = await _client.SendPutContentAsync<object>(_webUrls.OrderDetailCloseUrl, jsonData);
                            return rest;
                        }
                    }
                    else
                    {
                        return new ServiceMessage<object>() { Code = 500 };
                    }
                }
                catch (Exception ex)
                {
                    return new ServiceMessage<object>() { Code = 500 };
                }

            });

        }
        /// <summary>
        /// 推送审批流程至中心端
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<bool> pushPurchaseApprovesl(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {

                    // var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        ////登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        //AddToken("", data.ClientType, data.userName + "|");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);

                        //if (data != null)
                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            await _client.SendPutContentAsync<object>(_webUrls.PurchaseApprovesUrl, jsonData);
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });

        }

        /// <summary>
        /// 推送审批流程至中心端
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<bool> dzpushPurchaseApprovesl(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {

                    // var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        ////登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        //AddToken("", data.ClientType, data.userName + "|");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);

                        //if (data != null)
                        {
                            AddToken("", "PC", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            await _client.SendPutContentAsync<object>(_webUrls.dzPurchaseApprovesUrl, jsonData);
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });

        }

        /// <summary>
        /// 推送审批流程至中心端
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<bool> PushPurchaseDetail(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {

                    // var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        ////登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        //AddToken("", data.ClientType, data.userName + "|");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);

                        //if (data != null)
                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            await _client.SendPutContentAsync<object>(_webUrls.PushPurchaseDetailUrl, jsonData);
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });

        }

        /// <summary>
        /// 关闭中心端采购申请单
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<bool> ClosePurchase(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    // var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        ////登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        //AddToken("", data.ClientType, data.userName + "|");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);

                        //if (data != null)
                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);

                            await _client.SendPutContentAsync<object>(_webUrls.OrderCloseUrl, jsonData);
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }

        /// <summary>
        /// 审核退货
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public Task<bool> ReturnGoods(string CenterId, dynamic pas = null)
        {
            return Task.Run(async () =>
            {
                try
                {

                    // var data = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                    var center = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                    if (center != null)
                    {
                        _webUrls.BaseUrl = center.CenterUrl;
                        ////登录
                        //string UserName = data.userName.Encrypt();
                        //string Pwd = data.userPwd.Encrypt();
                        //AddToken("", data.ClientType, data.userName + "|");
                        //var LoginData = await UserLoginAsync(UserName, Pwd);

                        //if (data != null)
                        {
                            AddToken("", "", "");

                            string jsonData = JsonConvert.SerializeObject(pas);
                            var rest = await _client.SendPutContentAsync<object>(_webUrls.ReturnGoodsUrl, jsonData);
                            if (rest != null && rest.Code == 200) return true;
                            else return false;
                        }
                    }
                    else
                    {
                        // throw new Exception("同步中心端审核出错");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Client"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public Task<T[]> GetCollectDataAsync<T>(string Url)
        {
            return Task.Run(async () =>
            {
                try
                {
                    List<T> ListData = new List<T>();
                    var result = await _client.SendGetAsync<T[]>(Url);
                    switch (result.Code)
                    {
                        case 200:
                            ListData.InsertRange(0, result.Data);
                            break;
                    }
                    return ListData.ToArray();
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
            });

        }

        /// <summary>
        /// 推送中心端发布信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SendPublishVer(SysPublishInfo sysPublishInfo)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                try
                {
                    var centers = sysPublishInfo.UpdateCenter.Split(',');
                    foreach (var item in centers)
                    {

                        var center = ListcenterDialyses.FirstOrDefault(t => t.Id == item);
                        if (center != null)
                        {
                            _webUrls.BaseUrl = center.CenterUrl;

                            AddToken("", "", "");
                            SysPublishInfo sysPublish = new SysPublishInfo() { DataState = sysPublishInfo.DataState, DevTagsVersion = sysPublishInfo.DevTagsVersion, Founder = sysPublishInfo.Founder, FounderDate = sysPublishInfo.FounderDate, Id = sysPublishInfo.Id, isPush = sysPublishInfo.isPush, Modifier = sysPublishInfo.Modifier, ModifierDate = sysPublishInfo.ModifierDate, PublishDate = sysPublishInfo.PublishDate, Remark = sysPublishInfo.Remark, SysCHSName = sysPublishInfo.SysCHSName, SysType = sysPublishInfo.SysType, SysUSName = sysPublishInfo.SysUSName, SysVersion = sysPublishInfo.SysVersion, UpdateInfo = sysPublishInfo.UpdateInfo };
                            string jsonData = JsonConvert.SerializeObject(sysPublish);

                            var rest = await _client.SendPOSTContentAsync<object>(_webUrls.CenterPublishVerUrl, jsonData);
                            if (rest != null && rest.Code == 200)
                            {
                                sysPublishInfo.CenterPublishLogs.First(t => t.CenterId == item).isPush = true;
                                sysPublishInfo.CenterPublishLogs.First(t => t.CenterId == item).PublishTime = DateTime.Now;
                            }
                            else { Flag = false; }

                        }
                    }
                    CenterPublishLogStore.Update(sysPublishInfo.CenterPublishLogs);
                    _unitOfWork.SaveChanges();
                    return Flag;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                // return true;
            });

        }
        /// <summary>
        /// 推送公告
        /// </summary>
        /// <param name="centerNotices"></param>
        /// <returns></returns>
        public Task<bool> SendPublishNotices(CenterNotices centerNotices)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                try
                {

                    string jsonData1 = "";
                    if (centerNotices.noticeMedicals != null && centerNotices.noticeMedicals.Count > 0)
                    {
                        List<NoticeMedical> noticeMedicals = new List<NoticeMedical>();
                        foreach (var item in centerNotices.noticeMedicals)
                        {
                            noticeMedicals.Add(new NoticeMedical()
                            {
                                DataState = item.DataState,
                                DisposeTime = item.DisposeTime,
                                Founder = item.Founder,
                                FounderDate = item.FounderDate,
                                Id = item.Id,
                                MedicalId = item.MedicalId,
                                Modifier = item.Modifier,
                                ModifierDate = item.ModifierDate,
                                NoticesId = item.NoticesId,
                                OutTime = item.OutTime,
                                PurchasingPrice = item.PurchasingPrice,
                                Remark = item.Remark,
                                SalePrice = item.SalePrice,
                                UpSalePrice = item.UpSalePrice,
                                IsNeedBack = item.IsNeedBack,
                                IsRead = item.IsRead
                            });

                        }
                        jsonData1 = JsonConvert.SerializeObject(noticeMedicals);
                    }
                    //
                    Information information = new Information() { Adjunct = centerNotices.information.Adjunct, DataState = centerNotices.information.DataState == 3 ? 1 : 2, Founder = centerNotices.information.Founder, FounderDate = centerNotices.information.FounderDate, Id = centerNotices.information.Id, IsDelete = centerNotices.information.IsDelete, IsNeedBack = centerNotices.information.IsNeedBack, Modifier = centerNotices.information.Modifier, ModifierDate = centerNotices.information.ModifierDate, MsgContent = centerNotices.information.MsgContent, MsgTitle = centerNotices.information.MsgTitle, MsgTypeId = centerNotices.information.MsgTypeId, SendTime = centerNotices.information.SendTime, IsRead = centerNotices.information.IsRead };
                    string jsonData = JsonConvert.SerializeObject(information);

                    if (centerNotices.usersMsgs != null && centerNotices.usersMsgs.Count > 0)
                    {
                        foreach (var item in centerNotices.usersMsgs)
                        {
                            var center = ListcenterDialyses.FirstOrDefault(t => t.Id == item.ReceiveUserId);
                            if (center != null)
                            {
                                _webUrls.BaseUrl = center.CenterUrl;
                                AddToken("", "", "");
                                ServiceMessage<object> rest1 = new ServiceMessage<object>() { Code = 200 };
                                var rest = await _client.SendPOSTContentAsync<object>(_webUrls.CenterInformationUrl, jsonData);
                                if (jsonData1 != "")
                                    rest1 = await _client.SendPOSTContentAsync<object>(_webUrls.CenterNoticeMedicalUrl, jsonData1);

                                string jsonData2 = JsonConvert.SerializeObject(new UsersMsg() { BackTime = item.BackTime, BackUserId = item.BackUserId, FeedbackContent = item.FeedbackContent, Id = item.Id, IsFeedback = item.IsFeedback, IsRead = item.IsRead, IsSend = item.IsSend, MsgId = item.MsgId, ReceiveUserId = item.ReceiveUserId, ReceiveUserType = item.ReceiveUserType });
                                var rest2 = await _client.SendPOSTContentAsync<object>(_webUrls.CenterUsersMsgUrl, jsonData2);
                                if (rest.Code == 200 && rest1.Code == 200 && rest2.Code == 200)
                                {
                                    item.IsSend = true;
                                    UsersMsgStore.Update(item);
                                    _unitOfWork.SaveChanges();
                                    Flag = true;
                                }
                            }
                        }
                    }
                    return Flag;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            });

        }

        #region  手动更新目录

        public Task<bool> GetYPMLAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _client.SendGetAsync<bool>(_webUrls.YPUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                return true;
                                break;
                        }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return false;
            });
        }
        //
        public Task<bool> GetHCMLAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _client.SendGetAsync<bool>(_webUrls.ConsumablescUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                return true;
                                break;
                        }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return false;
            });
        }

        public Task<bool> GetFWMLAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _client.SendGetAsync<bool>(_webUrls.FWXUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                return true;
                                break;
                        }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return false;
            });
        }

        public Task<bool> GetXJAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var result = await _client.SendGetAsync<bool>(_webUrls.XJUrl);
                    if (result != null)
                        switch (result.Code)
                        {
                            case 200:
                                return true;
                                break;
                        }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return false;
            });
        }




        public Task<string> GetChronicSpecialAsync(string data)
        {
            return Task.Run(async () =>
            {
                ChronicSpecialDiseaseDrugRecordResponseModel restult = new ChronicSpecialDiseaseDrugRecordResponseModel();
                try
                {

                    var result = await _client.HISSendPOSTContentAsync(_webUrls.QueryChronicUrl, data);
                    return result;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }
        #endregion

    }
}

