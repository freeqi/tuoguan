﻿using AutoMapper;
using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Utils;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Extenstions;
using CDGService.Data.Helper;
using CDGService.Application.DataCollect;
using System.Data.SqlClient;
using CDGService.Store;
using static CDGService.WebAPI.DataCore.DataManager;

namespace CDGService.WebAPI.DataCore
{ /// <summary>
  /// 中心端对接
  /// </summary>
    public class CenterDockingManger : XmlSql
    {
        private readonly IGetUserInfo _getUserInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IDataReceive _dataReceive;

        private readonly CollentStartup _purchasManagerService;
        private readonly MaterialsStatisticaManager _materialsStatisticaManager;
        private readonly DataManager _dataManager;

        private IRepository<Employee> EmployeeStore => _unitOfWork.GetStore<Employee>();
        private IRepository<Log> LogStore => _unitOfWork.GetStore<Log>();

        private IRepository<PurchaseRequest> PurchaseRequestStore => _unitOfWork.GetStore<PurchaseRequest>();

        private IRepository<PurchaseDetail> PurchaseDetailStore => _unitOfWork.GetStore<PurchaseDetail>();
        private IRepository<GroupMergePurchase> GroupMergePurchaseStore => _unitOfWork.GetStore<GroupMergePurchase>();

        private IRepository<MedicalDrugExtension> MedicalDrugExtensionStore => _unitOfWork.GetStore<MedicalDrugExtension>();

        private IRepository<MedicalItemRecord> MedicalItemRecordStore => _unitOfWork.GetStore<MedicalItemRecord>();
        //
        private IRepository<PurchaseApprove> PurchaseApproveStore => _unitOfWork.GetStore<PurchaseApprove>();
        private IRepository<PurchaseOrder> PurchaseOrderStore => _unitOfWork.GetStore<PurchaseOrder>();
        private IRepository<PurchaseRequestsOrder> PurchaseRequestsOrderStore => _unitOfWork.GetStore<PurchaseRequestsOrder>();
        private IRepository<OrderDetail> OrderDetailStore => _unitOfWork.GetStore<OrderDetail>();
        private IRepository<OrderApprove> OrderApproveStore => _unitOfWork.GetStore<OrderApprove>();

        private IRepository<ReturnRequest> ReturnRequestStore => _unitOfWork.GetStore<ReturnRequest>();
        private IRepository<ReturnDetails> ReturnDetailsStore => _unitOfWork.GetStore<ReturnDetails>();
        private IRepository<WarehouseCatalog> CatalogueItemStore => _unitOfWork.GetStore<WarehouseCatalog>();
        private List<ApprovalProcess> _approvalProcesses = new List<ApprovalProcess>();
        // private IRepository<PurchaseRequestsOrder> PurchaseRequestsOrderStore => _unitOfWork.GetStore<PurchaseRequestsOrder>();
        //   private List<PurchaseRequestsOrder> _PurchaseRequestsOrder = new List<PurchaseRequestsOrder>();

        private List<PurchApproval> _PurchApproval = new List<PurchApproval>();
        private List<OrderPricingRole> _orderPricingRole = new List<OrderPricingRole>();
        private List<PurchSubmit> _purchSubmit = new List<PurchSubmit>();
        private IRepository<SI_YPMLS> SI_YPMLSStore => _unitOfWork.GetStore<SI_YPMLS>();
        private IRepository<ApprovalConfig> ApprovalConfigStore => _unitOfWork.GetStore<ApprovalConfig>();
        private IRepository<SI_CheckPriceInfo> SI_CheckPriceInfoStore => _unitOfWork.GetStore<SI_CheckPriceInfo>();
        private IRepository<SI_YP> SI_YPStore => _unitOfWork.GetStore<SI_YP>();
        private IRepository<MaterialApplyApprove> MaterialApplyApproveStore => _unitOfWork.GetStore<MaterialApplyApprove>();

        private IRepository<MaterialApplyApproveDetail> MaterialApplyApproveDetailStore => _unitOfWork.GetStore<MaterialApplyApproveDetail>();

        private IRepository<MaterialsWarningApplyList> MaterialsWarningApplyListStore => _unitOfWork.GetStore<MaterialsWarningApplyList>();
        private readonly IPurchasManagerService _purchasManagerServices;
        /// <summary>
        /// 中心端对接
        /// </summary>
        public CenterDockingManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IDataReceive dataReceive, CollentStartup purchasManagerService, Microsoft.Extensions.Options.IOptions<List<ApprovalProcess>> approvalProcess, MaterialsStatisticaManager materialsStatisticaManager, Microsoft.Extensions.Options.IOptions<List<PurchApproval>> purchApproval, Microsoft.Extensions.Options.IOptions<List<OrderPricingRole>> orderPricingRole, Microsoft.Extensions.Options.IOptions<List<PurchSubmit>> purchSubmit, DataManager dataManager, IPurchasManagerService purchasManagerServices)
        {
            _materialsStatisticaManager = materialsStatisticaManager;
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _dataReceive = dataReceive;
            _purchasManagerService = purchasManagerService;
            _approvalProcesses = approvalProcess.Value;
            _PurchApproval = purchApproval.Value;
            _purchSubmit = purchSubmit.Value;
            _dataManager = dataManager;
            _purchasManagerServices = purchasManagerServices;
        }
        /// <summary>
        /// 库存
        /// </summary>
        public static List<GoodsInStockdModel> listStockData = new List<GoodsInStockdModel>();
        public static List<MonthGoodsInStockdModel> listMonthGoodsInStockdData = new List<MonthGoodsInStockdModel>();
        private static DateTime UpdateTime;
        private static DateTime UpdateTime1;
        /// <summary>
        /// 获取库存
        /// </summary>
        public Task GetGoodsInStockdInfo()
        {
            return Task.Run(async () =>
             {
                 if (listStockData == null || listStockData.Count <= 0 || UpdateTime.Day != DateTime.Now.Day)
                 {
                     var datas = await _materialsStatisticaManager.GetGoodsInStockdInfo(new MaterialsInPut() { CenterId = "", MedicalItemName = "", MedicalItemType = "", SupplierId = "" });
                     listStockData = datas.ToList();
                     UpdateTime = DateTime.Now;
                 }

             });

        }

        private Task GetMonthUsageStatistics(DateTime EndTime)
        {
            return Task.Run(async () =>
             {
                 EndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                 DateTime BeginTime = new DateTime(EndTime.AddMonths(-1).Year, EndTime.AddMonths(-1).Month, 1, 0, 0, 0);
                 if (listMonthGoodsInStockdData == null || listMonthGoodsInStockdData.Count <= 0 || (UpdateTime1.Day != DateTime.Now.Day))
                 {
                     var datas = await _materialsStatisticaManager.GetMonthUsageStatistics(new MaterialsConfluenceInPut() { BeginTime = BeginTime, EndTime = EndTime, CenterId = "", MedicalItemName = "", MedicalItemType = "0", SupplierId = "" });
                     listMonthGoodsInStockdData = datas.ToList();
                     UpdateTime1 = DateTime.Now;
                 }

             });

        }

        public Task<int> GetGoodsInStockdCY()
        {
            return Task.Run(async () =>
            {
                int count = 0;
                await GetGoodsInStockdInfo();
                count = listStockData.Where(t => t.SalePrice > t.SocialSecurityPrice).Count();
                return count;

            });
        }


        #region 采购申请单

        /// <summary>
        /// 立即重新获取采购申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<PurchaseRequestOutPut[]>> CollectGetPurchaseRequestQueryableAsync(PurchaseRequestQueryInput input)
        {
            return Task.Run(async () =>
            {
                //bool IsDel = false;
                //PurchaseRequestOutPut[] result = null;
                //int count = 0;
                try
                {
                    //重新获取数据
                    if (input == null)
                        input = new PurchaseRequestQueryInput() { PageSize = 10, PageNum = 1, CenterId = "0" };
                    if (input.CenterId + "" == "")
                        input.CenterId = "0";
                    if (input.CenterId == "0")
                        throw new Exception("请选择指定的透析中心进行刷新");
                    await _purchasManagerService.ExecuteGetPurchasManagerData(input.CenterId);
                    await _purchasManagerService.ExecuteGetPurchaseDetailData(input.CenterId);
                    var data = await NewsGetPurchaseRequestQueryableAsync(input);

                    return data;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }

        //PurchaseRequest 
        /// <summary>
        /// 获取采购申请单列表[]
        /// </summary>
        public Task<PageData<PurchaseRequestOutPut[]>> GetPurchaseRequestQueryableAsync(PurchaseRequestQueryInput input)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                PurchaseRequestOutPut[] result = null;
                int count = 0;
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore

                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();

                List<string> Appids = new List<string>();
                try
                {
                    if (input == null) input = new PurchaseRequestQueryInput();
                    Expression<Func<PurchaseRequest, bool>> predicate = t => t.DataState == 1;
                    if (!string.IsNullOrEmpty(input.CenterId) && input.CenterId != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);

                    if (input.BeginTime.HasValue && input.EndTime.HasValue)
                        predicate = predicate.And(t => t.AuditDate >= input.BeginTime && t.AuditDate <= input.EndTime);
                    if (input.ApprovalState + "" != "" && input.ApprovalState != "0")
                    {

                        switch (input.ApprovalState)
                        {
                            case "2": //未审批
                                predicate = predicate.And(t => t.GroupAuditStatus == input.ApprovalState || t.GroupAuditStatus == "6");
                                break;
                            case "6"://上级待审
                                predicate = predicate.And(t => t.GroupAuditStatus == input.ApprovalState || t.GroupAuditStatus == "6");
                                break;
                            default:
                                predicate = predicate.And(t => t.GroupAuditStatus == input.ApprovalState);
                                break;
                        }
                    }


                    if (RoleData != null && RoleData.IsApproval)
                        predicate = predicate.And(t => t.ActualPrice >= RoleData.MinAmount);

                    if (input.MedicalItemType > 0)
                    {
                        List<string> appid = await PurchaseRequestsOrderStore.Entities.Where(t => t.IsDelete == IsDel && t.MedicalItemType == input.MedicalItemType).Select(t => t.ApplyId).ToListAsync();

                        predicate = predicate.And(t => !appid.Contains(t.Id));
                        predicate = predicate.And(t => t.PurchaseRequestsOrders.Count() < t.MedicaItemTypeCount);
                        // predicate = predicate.And(t => t.PurchaseDsOrders.);
                    }

                    if (input.remarks + "" != "")
                    {
                        // predicate = predicate.And(t => t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));
                        Appids = await PurchaseDetailStore.Entities.Where(t => t.medicalItemRecord.MedicalItemName.Contains(input.remarks)).Select(t => t.ApplyId).ToListAsync();
                        if (Appids != null && Appids.Count > 0)
                            predicate = predicate.And(t => Appids.Contains(t.Id) || t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));

                        else
                            predicate = predicate.And(t => t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));
                    }


                    var Sum = PurchaseRequestStore.Entities.Include(t => t.centerDialysis).Include(t => t.ListPurchaseApproves).Include(t => t.PurchaseDsOrders).Include(t => t.PurchaseRequestsOrders).Where(predicate).OrderByDescending(t => t.AuditDate);
                    //是否审核到
                    if (RoleData != null && RoleData.level > 2)
                        Sum = Sum.Where(t => t.ListPurchaseApproves.Where(y => y.AuditorLevel == RoleData.level - 1 && y.AuditStatus == 3).FirstOrDefault() != null).OrderByDescending(t => t.AuditDate);
                    int NoIndex = 0;
                    List<PurchaseRequest> purchaseRequestsData = new List<PurchaseRequest>();
                    {
                        purchaseRequestsData = await Sum.ToListAsync();
                        //foreach (var item in purchaseRequestsData)
                        //{
                        //    if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                        //        await UpdatePu(item.Id);
                        //}
                        result = AutoMapperHelper.Map<PurchaseRequestOutPut[]>(purchaseRequestsData);
                        count = result.Length;
                    }

                    foreach (var item in result)
                    {
                        //var temp = purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                        //if (temp == null || temp.Count <= 0)
                        //{
                        //    item.OrderType = 0;
                        //}
                        //else if (temp != null && temp.Count == purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                        //{
                        //    item.OrderType = 2;
                        //}
                        //else if (temp != null && temp.Count < purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                        //{
                        //    item.OrderType = 1;
                        //}
                        if (RoleData != null && RoleData.level >= 1)
                        {
                            if (item.GroupAuditStatus != "4")
                            {
                                var statedata = Sum.Where(t => t.Id == item.Id).FirstOrDefault().ListPurchaseApproves.Where(t => t.AuditorLevel == RoleData.level && t.DataState == 1).FirstOrDefault();
                                if (statedata != null && statedata.AuditStatus == 5)
                                    item.GroupAuditStatus = "2";
                                else if (item.GroupAuditStatus == "6")
                                {
                                    if (item.GroupAuditStatus == "6" && statedata != null && statedata.AuditStatus == 3)
                                        item.GroupAuditStatus = "6";
                                    else if (item.GroupAuditStatus == "6" && statedata == null)
                                        item.GroupAuditStatus = "2";
                                }

                            }
                            //item.GroupAuditStatus+"" == ""?  "2" :

                        }
                        item.ActualPrice = item.ActualPrice?.ToRounds();
                        item.no = NoIndex + 1;
                        NoIndex++;
                    }
                    if (input.ApprovalState != "0" && input.ApprovalState + "" != "")
                        result = result.Where(t => t.GroupAuditStatus == input.ApprovalState).ToArray();

                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        result = PaginatedList<PurchaseRequestOutPut>.CreateAsync(result.ToList(), input.PageNum, input.PageSize).Result.ToArray();
                        foreach (var item in result)
                        {
                            if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                                await UpdatePu(item.Id);
                        }


                        // result = AutoMapperHelper.Map<CenterDialysisOutPut[]>(Dialysis);
                        //foreach (var item in purchaseRequestsData)
                        //{
                        //    if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                        //        await UpdatePu(item.Id);
                        //}
                        //  count = Sum.Count();

                        // result = AutoMapperHelper.Map<PurchaseRequestOutPut[]>(purchaseRequestsData);

                    }
                    // else


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<PurchaseRequestOutPut[]>(result, count);
            });
        }

        /// <summary>
        /// 获取采购申请单明细列表[]
        /// </summary>
        public Task<PurchaseOutPut> GetPurchaseDetailQueryableAsync(string PurchaseRId)
        {

            return Task.Run(async () =>
            {

                PurchaseDetailOutPut[] result = null;
                PurchaseOutPut purchaseOutPut = new PurchaseOutPut();

                try
                {
                    var Sum = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).ThenInclude(t => t.centerDialysis).
                    Include(t => t.supplier).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.SpecificationsUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.PackageUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.doseUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.supplier).
                     Include(t => t.medicalItemRecord).ThenInclude(t => t.ProcurementUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.MedicalDrugExtensions).
                    Include(t => t.purchaseOrder).
                    Where(t => PurchaseRId == (t.ApplyId) && t.DataState == 1).OrderByDescending(t => t.medicalItemRecord.MedicalItemName).ToArrayAsync();

                    decimal? price = 0; //ListDetaildata.Sum(t => t.InPrice);
                    decimal? SalesTotalPrice = 0;
                    decimal? TotalQty = 0;
                    result = AutoMapperHelper.Map<PurchaseDetailOutPut[]>(Sum);
                    int NoIndex = 1;
                    foreach (var item in result)
                    {
                        if (item.InPrice.HasValue)
                            price += (item.InPrice) * (item.ApprovalQty);
                        if (item.SalePrice.HasValue)
                            SalesTotalPrice += (item.SalePrice) * (item.ApprovalQty);
                        TotalQty += item.ApprovalQty;
                        item.no = NoIndex;
                        NoIndex++;
                    }
                    var Requestdata = await PurchaseRequestStore.Entities.Include(t => t.PurchaseRequestsOrders).Where(t => t.Id == PurchaseRId).FirstOrDefaultAsync();
                    Requestdata.ActualPrice = price;
                    Requestdata.SalesTotalPrice = SalesTotalPrice;
                    Requestdata.ItemType = Sum.Length;
                    Requestdata.ApprovalTotalQty = TotalQty;
                    PurchaseRequestStore.Update(Requestdata);
                    _unitOfWork.SaveChanges();



                    var data = AutoMapperHelper.Map<PurchaseRequestOutPut>(Requestdata);

                    //var temp = Sum.Where(t => (t.OrderId != null && t.OrderId != "") && t.DataState == 1).ToList();
                    //if (temp == null || temp.Count <= 0)
                    //{
                    //    data.OrderType = 0; 
                    //}
                    //else if (temp != null && temp.Count == Sum.Count())
                    //{
                    //    data.OrderType = 2;
                    //}
                    //else if (temp != null && temp.Count < Sum.Count())
                    //{
                    //    data.OrderType = 1;
                    //}
                    purchaseOutPut.purchaseRequest = data;
                    purchaseOutPut.purchaseDetails = result;
                    purchaseOutPut.approvalProcessOutPuts = getapprovalProcessOutPuts(Requestdata).Result.ToArray();
                    //ApprovalProcessOutPut 
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return purchaseOutPut;
            });
        }

        /// <summary>
        /// 获取多采购单的采购明细
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<BatchPurchaseOutPut> GetPurchaseDetailQueryableAsync(PurchaseDetailQueryInPut inPut)
        {

            return Task.Run(async () =>
            {

                PurchaseDetailOutPut[] result = null;
                BatchPurchaseOutPut purchaseOutPut = new BatchPurchaseOutPut();

                try
                {
                    Expression<Func<PurchaseDetail, bool>> predicate = t => t.DataState == 1;
                    predicate = predicate.And(t => t.OrderId == null || t.OrderId == "");
                    if (inPut.ApplyId != null && inPut.ApplyId.Length > 0)
                        predicate = predicate.And(t => inPut.ApplyId.Contains(t.ApplyId));
                    //if (inPut.MedicaItemType > 0)
                    //{
                    //    switch (inPut.MedicaItemType)
                    //    {
                    //        case 1:
                    //            predicate = predicate.And(t => inPut.MedicaItemType == t.medicalItemRecord.MedicalItemType);
                    //            break;
                    //        case 2:
                    //            predicate = predicate.And(t => inPut.MedicaItemType == t.medicalItemRecord.MedicalItemType);
                    //            break;
                    //        case 3:
                    //            List<int> types = new List<int>() { 3, 4, 5 };
                    //            predicate = predicate.And(t => types.Contains(t.medicalItemRecord.MedicalItemType));
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    //if (inPut.SupplierId + "" != "" && inPut.SupplierId + "" != "0")
                    //{
                    //    predicate = predicate.And(t => t.SupplierId == inPut.SupplierId);
                    //}

                    var Sum = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).ThenInclude(t => t.centerDialysis).
                    Include(t => t.supplier).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.SpecificationsUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.PackageUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.doseUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.supplier).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.MedicalDrugExtensions).
                    Where(predicate).OrderByDescending(t => t.purchaseRequest.centerDialysis.ShortName).ToArrayAsync();

                    //decimal? price = 0; //ListDetaildata.Sum(t => t.InPrice);
                    //decimal? SalesTotalPrice = 0;
                    //decimal? TotalQty = 0;
                    //foreach (var item in Sum)
                    //{
                    //    if (item.InPrice.HasValue)
                    //        price += (item.InPrice) * (item.InQty);
                    //    if (item.SalePrice.HasValue)
                    //        SalesTotalPrice += (item.SalePrice) * (item.InQty);
                    //    TotalQty += item.InQty;
                    //}
                    var Requestdata = await PurchaseRequestStore.Entities.Where(t => inPut.ApplyId.Contains(t.Id) && t.DataState == 1).ToListAsync();

                    //Requestdata.ActualPrice = price;
                    //Requestdata.SalesTotalPrice = SalesTotalPrice;
                    //Requestdata.ItemType = Sum.Length;
                    //Requestdata.TotalQty = TotalQty;
                    //PurchaseRequestStore.Update(Requestdata);
                    //_unitOfWork.SaveChanges();

                    result = AutoMapperHelper.Map<PurchaseDetailOutPut[]>(Sum);
                    int NoIndex = 1;
                    foreach (var item in result)
                    {
                        item.no = NoIndex;
                        NoIndex++;

                    }
                    var data = AutoMapperHelper.Map<PurchaseRequestOutPut[]>(Requestdata);

                    purchaseOutPut.purchaseRequest = data;
                    purchaseOutPut.purchaseDetails = result;

                    //ApprovalProcessOutPut
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return purchaseOutPut;
            });
        }

        /// <summary>
        /// 获取审核流程
        /// </summary>
        /// <returns></returns>
        private Task<List<ApprovalProcessOutPut>> getapprovalProcessOutPuts(PurchaseRequest purchaseRequest)
        {
            return Task.Run(async () =>
            {

                List<string> YLFZcat = new List<string>() { null, "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8 cc1c671ec03e1d5", "e07940c4e798455e95680f986a8182fe", "187b752e52414196b763f91fae53a825", "c1fe3b7a2e3a495d958062fa81c9aaf6" };

                List<string> FGFZcat = new List<string>() { null, "97277431fc754297853d9d20cb423b50", "d0913b0979b1422e8545ad78496c35bb", "1db44420ccd1462c98eb65afe83cd569", "c1fe3b7a2e3a495d958062fa81c9aaf6", };




                // await UpdatePu("");
                List<ApprovalProcessOutPut> approvalProcessOutPuts = new List<ApprovalProcessOutPut>();
                var a_approvalProcesses = _purchSubmit.CompareDistinct(t => t.level).ToList();

                if (YLFZcat.Contains(purchaseRequest.Catalogue))
                {
                    a_approvalProcesses.Add(new PurchSubmit() { level = 3, Role = "医疗副总" });
                }
                else
                {
                    a_approvalProcesses.Add(new PurchSubmit() { level = 3, Role = "行政副总" });
                }

                var data = await PurchaseApproveStore.Entities.Include(t => t.user).ThenInclude(t => t.Employee).Where(t => t.ApplyId == purchaseRequest.Id && t.DataState == 1).OrderBy(t => t.AuditDate).ToListAsync();
                int level = a_approvalProcesses.Max(t => t.level);
                int Maxlevel = 0;
                int current = 0;
                string state = "";
                if (data != null && data.Count > 0)
                {
                    //data = data.CompareDistinct(t => t.AuditorLevel).ToList();
                    Maxlevel = data.Max(t => t.AuditorLevel);
                    //approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                    //{
                    //    ApprovalName = "",
                    //    content = "采购员调价并初审",
                    //    title = "已完成",

                    //});
                    foreach (var item in data)
                    {
                        state = item.AuditStatus == 3 ? "同意" : item.AuditStatus == 4 ? "拒绝" : item.AuditStatus == 7 ? "暂缓" : "回退";
                        if (state == "回退" || state == "拒绝")
                            state += $"，备注：{item.Advice}";
                        approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                        {
                            ApprovalName = "",
                            content = $"({item.user.Employee.Name})审查，结果： {state}",
                            title = item.AuditStatus == 4 ? "已拒绝" : "已完成",
                            Time = item.AuditDate.Value.ToString("yyyy-MM-dd"),
                        });
                        current++;
                    }
                    if (data.Last().AuditStatus == 5)
                        Maxlevel = 0;
                    else
                        Maxlevel = data.Last().AuditorLevel;
                    if (data.Last().AuditStatus != 4)
                    {

                        current++;
                        foreach (var item in a_approvalProcesses)
                        {
                            if ((Maxlevel < level && item.level > Maxlevel && item.level <= level) && purchaseRequest.GroupAuditStatus == "6")
                                approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                                {
                                    ApprovalName = "",
                                    content = "等待" + item.Role + "审查",
                                    title = "待进行",
                                });
                        }
                    }
                }
                else
                {
                    //approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                    //{
                    //    ApprovalName = "",
                    //    content = "采购员调价并初审",
                    //    title = "待完成",
                    //    current = 0,
                    //});

                    foreach (var item in a_approvalProcesses)
                    {
                        if (Maxlevel < level && item.level <= level)
                            approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                            {
                                ApprovalName = "",
                                content = "等待" + item.Role + "审查",
                                title = "待进行",
                                current = 0,
                            });
                    }
                }
                //if (level == Maxlevel || state == "拒绝")
                //{
                //    current++;
                //    approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                //    {
                //        ApprovalName = "",
                //        content = "审批结束",
                //        title = "已完成",

                //    });
                //}
                //else
                //{
                //    approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                //    {
                //        ApprovalName = "",
                //        content = "已完成",
                //        title = "待进行",

                //    });
                //}
                approvalProcessOutPuts.ForEach(t => t.current = current);
                return approvalProcessOutPuts;
            });
        }

        //审核采购申请单

        public Task<bool> ApprovalPurchaseRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {

                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var data = await PurchaseRequestStore.GetFirstOrDefaultAsync(t => t.Id == Input.Id);
                    if (data == null)
                        return true;
                    //PurchaseApproveStore
                    var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                    if (RoleData == null)
                        throw new Exception("您暂时没有审核采购单的权限");
                    if (RoleData.level == 1 && Input.GroupAuditStatus == "5")
                        throw new Exception("当前状态无法回退");

                    PurchaseApprove purchaseApprove = new PurchaseApprove()
                    {
                        Advice = Input.GroupAdvice,
                        ApplyId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditorLevel = RoleData.level,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        ModifierDate = DateTime.Now

                    };

                    //回退 
                    if (Input.GroupAuditStatus == "5")
                    {
                        var approves = await PurchaseApproveStore.Entities.Where(t => t.ApplyId == Input.Id).ToListAsync();
                        approves.ForEach(t => t.DataState = 2);
                        PurchaseApproveStore.Update(approves);
                        _unitOfWork.SaveChanges();
                    }
                    PurchaseApproveStore.Insert(purchaseApprove);
                    _unitOfWork.SaveChanges();
                    if (Input.GroupAuditStatus == "5")//回退，从开始位置重新审批
                    {
                        data.GroupAuditStatus = "2";
                        data.GroupAdvice = Input.GroupAdvice;
                        data.GroupAuditDate = DateTime.Now;
                        data.GroupAuditor = userId;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.PurchSubmitLevel = 1;
                        PurchaseRequestStore.Update(data);
                        _unitOfWork.SaveChanges();
                        return true;
                    }
                    data.GroupAuditStatus = "6";
                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;

                    if (((data.ActualPrice < RoleData.MaxAmount && data.ActualPrice >= RoleData.MinAmount && RoleData.level >= 3) || Input.GroupAuditStatus == "4"))// 是否审核到此等级本单结束(至少要经过医保部)
                    {
                        data.GroupAuditStatus = Input.GroupAuditStatus;

                        //对接中心端审核状态
                        if (data.GroupAuditStatus == "4") //拒绝 ,直接对接中心端， 如果同意，则走订单
                        {
                            await _dataReceive.UpdatePurchasState(data.CenterId, new
                            {
                                GroupAuditStatus = data.GroupAuditStatus,
                                GroupAuditDate = data.GroupAuditDate,
                                GroupAdvice = data.GroupAdvice,
                                GroupAuditor = name,
                                Modifier = user.UserName,
                                ModifierDate = data.ModifierDate,
                                ApprovalTotalQty = data.ApprovalTotalQty,
                                Id = data.Id
                            });
                        }
                        //修改中心端价格
                        //if (data.GroupAuditStatus == "4") //拒绝，不提交价格
                        //    return true;
                        //await AddUpdatePurchas(Input.Id);
                        //var PurchaseDetaiList = await PurchaseDetailStore.Entities.Where(t => t.ApplyId == Input.Id && t.DataState == 1).ToListAsync();
                        //foreach (var item in PurchaseDetaiList)
                        //{
                        //    await _dataReceive.UpdatePurchasPriceState(data.CenterId, new
                        //    {
                        //        InPrice = item.InPrice.HasValue ? item.InPrice : 0,
                        //        Id = item.Id,
                        //        SalePrice = item.SalePrice.HasValue ? item.SalePrice : 0,
                        //        SupplierId = item.SupplierId,
                        //        ApprovalQty = item.ApprovalQty,
                        //        Modifier = user.UserName,
                        //    });
                        //}

                    }
                    PurchaseRequestStore.Update(data);
                    _unitOfWork.SaveChanges();
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
        /// <param name="Input"></param>
        /// <returns></returns>
        public Task<bool> UpdatePurchasPriceState(PurchaseDetaiPriceInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (Input != null && Input.ItemsId != "")
                    {
                        //var data = await PurchaseRequestStore.GetFirstOrDefaultAsync(t => t.Id == Input.First().PurchaseRequestId);
                        //if (data == null)
                        //    return false;
                        string userId = await _getUserInfo.GetCurrentUserIdAsync();
                        var Detaildata = await PurchaseDetailStore.Entities.Include(t => t.medicalItemRecord).Where(t => t.Id == Input.ItemsId).FirstOrDefaultAsync();

                        Detaildata.InPrice = Input.InPrice.ToRounds(6);
                        Detaildata.AdvicePrice = Detaildata.InPrice;
                        Detaildata.SalePrice = Input.SalePrice.ToFloorRound(2);
                        Detaildata.SupplierId = Input.SupplierId == "" || Input.SupplierId == "0" ? "" : Input.SupplierId;
                        Detaildata.ModifierDate = DateTime.Now;
                        Detaildata.Modifier = userId;
                        if (Input.ApprovalQty > 0)
                            Detaildata.ApprovalQty = Input.ApprovalQty;
                        if (Detaildata.SupplierId != "")
                            Detaildata.medicalItemRecord.SupplierId = Detaildata.SupplierId;
                        if (Detaildata.medicalItemRecord.MedicalItemType == 3)
                        {//固定资产
                            Detaildata.ProcurementPackage = Input.ProcurementPackage;
                        }
                        Detaildata.Remark = Input.Remark + "" == "" ? "手动调整供应商，价格" : Input.Remark;
                        PurchaseDetailStore.Update(Detaildata);

                        // MedicalItemRecordStore

                        _unitOfWork.SaveChanges();
                        //修改实际采购总价
                        var Requestdata = await PurchaseRequestStore.GetFirstOrDefaultAsync(t => t.Id == Detaildata.ApplyId);
                        var ListDetaildata = await PurchaseDetailStore.Entities.Where(t => t.ApplyId == Detaildata.ApplyId).ToListAsync();
                        decimal? price = 0; //ListDetaildata.Sum(t => t.InPrice);
                        decimal? SalesTotalPrice = 0;
                        decimal? TotalQty = 0;
                        foreach (var item in ListDetaildata)
                        {
                            if (item.InPrice.HasValue)
                                price += (item.InPrice) * (item.ApprovalQty ?? 0);
                            if (item.SalePrice.HasValue)
                                SalesTotalPrice += (item.SalePrice) * (item.ApprovalQty);
                            SalesTotalPrice += (item.SalePrice) * (item.ApprovalQty);
                            TotalQty += item.ApprovalQty;
                        }
                        Requestdata.ActualPrice = price.Value.ToFloorRound(2);
                        Requestdata.SalesTotalPrice = SalesTotalPrice == null ? 0 : SalesTotalPrice.Value.ToFloorRound();
                        Requestdata.ApprovalTotalQty = TotalQty;
                        PurchaseRequestStore.Update(Requestdata);
                        _unitOfWork.SaveChanges();
                        // 修改档案价格 
                        List<string> centerIds = new List<string>() { "0", Requestdata.CenterId };

                        var _MedicalDrugExtensiones = await MedicalDrugExtensionStore.Entities.Where(t => t.MedicalId == Detaildata.MedicalItemId && t.IsCurrentUse && centerIds.Contains(t.CenterId)).ToListAsync();
                        //统一价
                        var AllPrice = _MedicalDrugExtensiones.FirstOrDefault(t => t.CenterId == "0");
                        //机构指定价
                        var CenterPrice = _MedicalDrugExtensiones.FirstOrDefault(t => t.CenterId == Requestdata.CenterId);
                        //当前价格与统一价一样，则不需新增机构指定价   当前价与指定价一致也不需要新增
                        if (!((AllPrice != null && Input.InPrice == AllPrice.PurchasingPrice && Input.SalePrice == AllPrice.RetailPrice)
                            ||
                            (CenterPrice != null && Input.InPrice == CenterPrice.PurchasingPrice && Input.SalePrice == CenterPrice.RetailPrice && CenterPrice.DataState == 1)))
                        {
                            if (CenterPrice != null)
                            {
                                CenterPrice.IsCurrentUse = false;
                                MedicalDrugExtensionStore.Update(CenterPrice);
                                _unitOfWork.SaveChanges();
                            }


                            // var newMedicalDrugExtensione = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.MedicalId == Detaildata.MedicalItemId);

                            MedicalDrugExtension medicalDrugExtension = new MedicalDrugExtension();
                            // if (newMedicalDrugExtensione != null)
                            //    EntityHelper.CoptyProperty(newMedicalDrugExtensione, medicalDrugExtension);
                            medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                            medicalDrugExtension.DataState = 1;
                            medicalDrugExtension.IsCurrentUse = true;
                            //medicalDrugExtension.MedicalId = Input.ItemsId;
                            medicalDrugExtension.FounderDate = DateTime.Now;
                            medicalDrugExtension.ModifierDate = DateTime.Now;
                            medicalDrugExtension.Founder = userId;
                            medicalDrugExtension.Modifier = userId;
                            medicalDrugExtension.PurchasingPrice = Input.InPrice;
                            medicalDrugExtension.RetailPrice = Input.SalePrice;
                            medicalDrugExtension.CenterId = Requestdata.CenterId;
                            medicalDrugExtension.MedicalId = Detaildata.MedicalItemId;
                            //  medicalDrugExtension.ReferencePrice = newMedicalDrugExtensione.ReferencePrice;

                            MedicalDrugExtensionStore.Insert(medicalDrugExtension);
                            _unitOfWork.SaveChanges();

                        }

                        //当前价格与统一价一样，则将指定价改为禁用

                        if ((AllPrice != null && Input.InPrice == AllPrice.PurchasingPrice && Input.SalePrice == AllPrice.RetailPrice) && CenterPrice != null)
                        {
                            CenterPrice.DataState = 2;

                            MedicalDrugExtensionStore.Update(CenterPrice);
                            _unitOfWork.SaveChanges();
                        }

                        //if (_MedicalDrugExtensione != null
                        //&& _MedicalDrugExtensione.PurchasingPrice == Input.InPrice
                        //&& _MedicalDrugExtensione.RetailPrice == Input.SalePrice)
                        //{

                        //}
                        //else
                        //{
                        //    if (_MedicalDrugExtensione != null)
                        //    {
                        //        _MedicalDrugExtensione.IsCurrentUse = false;
                        //        MedicalDrugExtensionStore.Update(_MedicalDrugExtensione);
                        //        _unitOfWork.SaveChanges();
                        //    }

                        //}

                        //修改中心端{"InPrice":"","Id":"","SalePrice":""}多个id以逗号隔开

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return true;
            });
        }

        /// <summary>
        /// 删除、新增中心端采购明细
        /// </summary>
        /// <param name="PurchaseRequestId">采购单ID</param>
        /// <returns></returns>
        private Task<bool> AddUpdatePurchas(string PurchaseRequestId)
        {
            return Task.Run(async () =>
            {
                string userName = _getUserInfo.GetUserAsync().Result.UserName;
                var data = await PurchaseRequestStore.GetFirstOrDefaultAsync(t => t.Id == PurchaseRequestId);
                var DetailData = await PurchaseDetailStore.Entities.Where(t => t.ApplyId == PurchaseRequestId && (t.IsGroupAdd == true || t.DataState == 3)).ToListAsync();
                //DelPurchaseDetails
                foreach (var item in DetailData)
                {
                    //{"InSumMoney":"","InQty": , "Id":"采购详情id",	"ApplyId": "采购申请id"}
                    //if (item.DataState == 3 && !item.IsGroupAdd)
                    //    await _dataReceive.DelPurchaseDetails(data.CenterId, new
                    //    {
                    //        InSumMoney = (item.AdvicePrice.HasValue && item.InQty.HasValue) ? item.AdvicePrice * item.InQty : 0,
                    //        InQty = item.InQty,
                    //        Id = item.Id,
                    //        ApplyId = item.ApplyId,
                    //        Modifier = userName,
                    //    });
                    if (item.DataState == 1 && item.IsGroupAdd && item.ApprovalQty > 0)
                    {
                        await _dataReceive.AddPurchaseDetails(data.CenterId, new
                        {
                            Id = item.Id,
                            ApplyId = item.ApplyId,
                            MedicalItemId = item.MedicalItemId,
                            InPrice = item.InPrice,
                            SalePrice = item.SalePrice,
                            AdvicePrice = item.AdvicePrice,
                            ActualQty = item.ActualQty,
                            InQty = item.InQty,
                            InSumMoney = item.InSumMoney,
                            SupplierId = item.SupplierId,
                            ProcurementPackage = item.ProcurementPackage,
                            ProcurementUnit = item.ProcurementUnit,
                            Remark = item.Remark,
                            FounderDate = item.FounderDate,
                            ModifierDate = item.ModifierDate,
                            DataState = item.DataState,
                            ApprovalQty = item.ApprovalQty,
                            Founder = userName,
                            Modifier = userName,
                            IsClosed = 2,

                        });
                    }
                }

                return true;
            });
        }

        /// <summary>
        /// 拒绝采购明细项 - 用于申请物品与实际采购物品不符合
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<bool> DelPurchasDetilAsync(PurchaseDetailDelInPut inPut)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                try
                {
                    var Detaildata = await PurchaseDetailStore.Entities.Where(t => t.Id == inPut.Id).FirstOrDefaultAsync();
                    if (Detaildata != null)
                    {
                        Detaildata.DataState = 2;
                        Detaildata.Modifier = _getUserInfo.GetCurrentUserIdAsync().Result;
                        Detaildata.ModifierDate = DateTime.Now;
                        Detaildata.Remark = inPut.Remark;
                        //Detaildata.IsGroupAdd = true;  

                        /* 
                         * 
                         *
                         */
                        PurchaseDetailStore.Update(Detaildata);
                        _unitOfWork.SaveChanges();
                        await UpdatePu(Detaildata.ApplyId);

                        Flag = true;
                    }
                    else
                        throw new Exception($"未能获取到ID:{inPut.Id}的信息");

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return Flag;


            });

        }
        //添加采购明细 PurchaseDetailInPut
        public Task<bool> AddPurchaseDetailAsync(PurchaseDetailInPut input, bool IsIsSplit = false)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {
                    if (input == null)
                        throw new Exception(MessageFormater.PrameterNeedProvider("明细信息"));
                    if (string.IsNullOrEmpty(input.ApplyId))
                        throw new Exception(MessageFormater.PrameterNeedProvider("采购单ID"));
                    if (string.IsNullOrEmpty(input.MedicalItemId))
                        throw new Exception(MessageFormater.PrameterNeedProvider("物品档案ID"));

                    var appData = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Where(t => t.Id == input.ApplyId).FirstOrDefaultAsync();
                    if (appData == null)
                        throw new Exception("未能获取到采购申请信息，无法添加采购明细");
                    if (IsIsSplit == false && appData != null && appData.PurchaseDsOrders.Where(t => t.MedicalItemId == input.MedicalItemId && t.DataState == 1).Count() > 0)
                        throw new Exception($"明细中以存在该物品，请勿重复添加");
                    var medData = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.Id == input.MedicalItemId);
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    PurchaseDetail data = null;
                    data = AutoMapperHelper.Map<PurchaseDetail>(input);
                    data.ApprovalQty = input.InQty;
                    data.AdvicePrice = data.InPrice;
                    data.InSumMoney = data.InQty * data.InPrice;
                    data.Founder = userId;
                    data.FounderDate = DateTime.Now;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.DataState = 1;
                    data.Id = Guid.NewGuid().tostring32();
                    data.IsGroupAdd = true;
                    data.CenterId = appData.CenterId;
                    data.CollectTime = DateTime.Now;
                    data.IsClosed = 2;
                    if (medData.IsSplited)
                    {
                        data.ProcurementUnit = medData.SpecificationsUnit;
                        data.ProcurementPackage = medData.Specifications;
                    }
                    else
                    {
                        data.ProcurementUnit = medData.PackageUnit;
                        data.ProcurementPackage = medData.Packaging;
                    }

                    data.SupplierId = appData.PurchaseDsOrders.Where(t => t.SupplierId + "" != "").FirstOrDefault() != null ? appData.PurchaseDsOrders.Where(t => t.SupplierId + "" != "").FirstOrDefault().SupplierId : "";
                    PurchaseDetailStore.Insert(data);
                    _unitOfWork.SaveChanges();
                    await UpdatePu(input.ApplyId);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return Flag;


            });
        }

        //修改采购申请单物品类型数
        private Task<bool> UpdatePu(string Id)
        {
            return Task.Run(async () =>
             {
                 List<PurchaseDetail> lit = new List<PurchaseDetail>();

                 bool Flag = true;
                 try
                 {
                     var data = await PurchaseRequestStore.Entities.Where(t => t.Id == Id).ToListAsync();
                     foreach (var items in data)
                     {
                         var Details = await PurchaseDetailStore.Entities.Include(t => t.medicalItemRecord).Where(t => t.ApplyId == items.Id && t.DataState != 3).ToListAsync();
                         if (Details == null || Details.Count <= 0)
                         {
                             items.MedicaItemTypeCount = 0;
                             PurchaseRequestStore.Update(items);
                             continue;
                         }
                         lit = Details;
                         Details.RemoveAll(t => t.medicalItemRecord == null);
                         var gData = Details.GroupBy(t => t.medicalItemRecord.MedicalItemType);
                         List<int> type = new List<int>();
                         int count = 0;
                         foreach (var item in gData)
                         {
                             type.Add(item.Key);
                         }
                         if (type.Contains(1))
                             count += 1;
                         if (type.Contains(2))
                             count += 1;
                         if (type.Contains(3) || type.Contains(4) || type.Contains(5))
                             count += 1;
                         items.MedicaItemTypeCount = count;
                         PurchaseRequestStore.Update(items);
                     }
                     _unitOfWork.SaveChanges();

                 }
                 catch (Exception ex)
                 {

                     throw new Exception(ex.Message, ex);
                 }
                 return Flag;

             });
        }

        //拆分采购明细 PurchaseDetailSplit   
        public Task<bool> PurchaseDetailSplitAsync(PurchaseDetailSplit input)
        {

            return Task.Run(async () =>
            {

                var Detaildata = await PurchaseDetailStore.Entities.Where(t => t.Id == input.detailId).FirstOrDefaultAsync();
                if (Detaildata != null)
                {
                    decimal? Num = Detaildata.ApprovalQty - input.detailNum;
                    if (Num <= 0)
                        throw new Exception($"拆分数量应小于采购总量");
                    Detaildata.ApprovalQty = input.detailNum;
                    PurchaseDetailStore.Update(Detaildata);
                    _unitOfWork.SaveChanges();
                    PurchaseDetailInPut data = new PurchaseDetailInPut()
                    {
                        ApplyId = Detaildata.ApplyId,
                        InPrice = Detaildata.InPrice,
                        InQty = Num,
                        MedicalItemId = Detaildata.MedicalItemId,
                        ProcurementPackage = Detaildata.ProcurementPackage,
                        ProcurementUnit = Detaildata.ProcurementUnit,
                        Remark = "拆分生成",
                        SalePrice = Detaildata.SalePrice,
                        SupplierId = Detaildata.SupplierId,
                    };
                    // AutoMapperHelper.Map<PurchaseDetailInPut>(Detaildata);

                    var flag = await AddPurchaseDetailAsync(data, true);
                    return flag;

                }
                else
                {
                    throw new Exception($"未能获取的明细数据");
                }


            });
        }


        #endregion


        #region 采购订单

        // OrderInput 采
        //GetGoodsInStockdInfo 
        /// <summary>
        /// 采购申请单生成采购需求计划（订单）
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<bool> PurchasToOrder(OrderInput inPut)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {
                    if (inPut == null)//|| (inPut.SupplierId + "" == "" || inPut.SupplierId + "0" == "0")
                        throw new Exception("请提供有效参数");
                    var OldData = await PurchaseRequestsOrderStore.Entities.Where(t => t.MedicalItemType == inPut.MedicalItemType && t.SupplierId == inPut.SupplierId && inPut.ApplyId.Contains(t.ApplyId) && t.IsDelete == false).ToListAsync();
                    if (OldData != null && OldData.Count > 0)
                        throw new Exception("已存在该类型的生成订单，请勿重复生成！");
                    Expression<Func<PurchaseDetail, bool>> predicate = t => t.DataState == 1;
                    if (inPut.ApplyId != null && inPut.ApplyId.Length > 0)
                        predicate = predicate.And(t => inPut.ApplyId.Contains(t.ApplyId));

                    if (inPut.SupplierId + "" != "" && inPut.SupplierId + "" != "0")
                        predicate = predicate.And(t => t.SupplierId == inPut.SupplierId);

                    string OrderNo = "";

                    var NowTime = DateTime.Now;
                    int maxNo = 1;
                    var list = await PurchaseOrderStore.Entities.Where(t => t.FounderDate > new DateTime(NowTime.Year, NowTime.Month, NowTime.Day, 0, 0, 0)).Select(t => t.OrderNo).ToListAsync();
                    foreach (var item in list)
                    {
                        if (item.Length > 10 && Convert.ToInt32(item.Substring(item.Length - 4, 4)) >= maxNo)
                            maxNo = Convert.ToInt32(item.Substring(item.Length - 4, 4)) + 1;
                    }
                    switch (inPut.MedicalItemType)
                    {
                        case 1:
                            predicate = predicate.And(t => inPut.MedicalItemType == t.medicalItemRecord.MedicalItemType);
                            OrderNo = $"OD-{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}";
                            break;
                        case 2:
                            predicate = predicate.And(t => inPut.MedicalItemType == t.medicalItemRecord.MedicalItemType);
                            OrderNo = $"OU-{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}";
                            break;
                        case 3:
                            List<int> types = new List<int>() { 3, 4, 5 };
                            predicate = predicate.And(t => types.Contains(t.medicalItemRecord.MedicalItemType));
                            OrderNo = $"OE-{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}";
                            break;
                        default:
                            OrderNo = $"OA-{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}";
                            break;
                    }

                    var Sum = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).Where(predicate).ToListAsync();
                    if (Sum == null || Sum.Count <= 0)
                        throw new Exception("未能获取到该合并类型的采购明细数据 ！");
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var OrderDetailData = Sum.GroupBy(t => new { t.purchaseRequest.CenterId, t.MedicalItemId, t.InPrice, t.SalePrice });
                    PurchaseOrder data = null;
                    data = AutoMapperHelper.Map<PurchaseOrder>(inPut);
                    data.Id = Guid.NewGuid().tostring32();
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.Founder = userId;
                    data.FounderDate = DateTime.Now;
                    data.OrderNo = OrderNo;
                    data.DataState = 1;
                    data.IsDelete = false;
                    data.IsPushCenter = false;
                    data.ActualPrice = Sum.Sum(t => t.InPrice * t.ApprovalQty);
                    data.TotalQty = Sum.Sum(t => t.ApprovalQty);
                    data.SupplierId = data.SupplierId == "0" ? Sum.FirstOrDefault().SupplierId : data.SupplierId;
                    data.ItemType = OrderDetailData.Count();
                    PurchaseOrderStore.Insert(data); //插入采购订单
                    //插入采购单关联订单 多对多  
                    //
                    List<PurchaseRequestsOrder> requestsOrders = new List<PurchaseRequestsOrder>();
                    var Pdata = Sum.Select(t => t.ApplyId).CompareDistinct(t => t);  //await  PurchaseRequestStore.Entities.Where(t => inPut.ApplyId.Contains(t.Id)).Select(t => t.Id).ToListAsync();
                    foreach (var item in Pdata)
                    {
                        requestsOrders.Add(new PurchaseRequestsOrder() { ApplyId = item, MedicalItemType = data.MedicalItemType, IsDelete = false, OrderNo = data.OrderNo, OrderId = data.Id, Id = Guid.NewGuid().tostring32(), SupplierId = data.SupplierId });
                    }
                    PurchaseRequestsOrderStore.Insert(requestsOrders);

                    //当前库存数据
                    // Dictionary<string, GoodsInStockdModel[]> pairs = new Dictionary<string, GoodsInStockdModel[]>();


                    //获取库存   

                    List<OrderDetail> details = new List<OrderDetail>();

                    foreach (var item in OrderDetailData)
                    {
                        //if (!pairs.ContainsKey(item.FirstOrDefault().purchaseRequest.CenterId))
                        //{
                        //    var GoodsInStockd = await _materialsStatisticaManager.GetGoodsInStockdInfo(new MaterialsInPut() { CenterId = item.FirstOrDefault().purchaseRequest.CenterId, MedicalItemType = inPut.MedicalItemType });
                        //    pairs[item.FirstOrDefault().purchaseRequest.CenterId] = GoodsInStockd;
                        //}
                        ////  string PurchDetailIds = a.ToString();
                        //decimal? CurrentInventory = pairs[item.FirstOrDefault().purchaseRequest.CenterId].Where(t => t.MedicalItemId == item.FirstOrDefault().MedicalItemId).Sum(t => t.InQty);

                        details.Add(new OrderDetail()
                        {
                            OrderId = data.Id,
                            MedicalId = item.First().MedicalItemId,
                            // CurrentInventory = 100,
                            DataState = 1,
                            Founder = userId,
                            FounderDate = DateTime.Now,
                            Id = Guid.NewGuid().tostring32(),
                            IsDelete = false,
                            Modifier = userId,
                            HurrySlowly = item.First().ProcurementPackage,
                            ModifierDate = DateTime.Now,
                            // MonthAverage = 100,
                            PurchPrice = item.First().InPrice,
                            SalePrice = item.First().SalePrice,
                            UpPurchPrice = item.First().InPrice,
                            SupplierId = item.First().SupplierId,
                            PurchasePuantity = item.Sum(t => t.ApprovalQty),
                            PurchDetailIds = string.Join(",", item.Select(t => t.Id).ToArray()),
                            SumPrice = item.Sum(t => t.ApprovalQty) * item.First().InPrice,
                            CenterId = item.First().purchaseRequest.CenterId,
                            CurrentInventory = item.Sum(t => t.CurrentInventory), //CurrentInventory.HasValue ? CurrentInventory : 0,//当前库存
                            MonthAverage = item.Sum(t => t.MonthAverage),//月均用量  
                        });

                    }
                    OrderDetailStore.Insert(details);
                    Sum.ForEach(t => t.OrderId = data.Id);
                    //修改采购明细关联订单
                    PurchaseDetailStore.Update(Sum);
                    await _unitOfWork.SaveChangesAsync();
                    //插入订单明细流水


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return Flag;

            });
        }
        //   private 
        //删除订单
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public Task<bool> PurchasOrderDel(string OrderId)
        {
            return Task.Run(async () =>
             {
                 bool Flag = true;
                 try
                 {
                     //删除订单
                     var data = await PurchaseOrderStore.GetFirstOrDefaultAsync(t => OrderId == t.Id);
                     if (data == null)
                         throw new Exception("未能获取到订单信息");
                     var userId = await _getUserInfo.GetCurrentUserIdAsync();
                     data.IsDelete = true;
                     data.Modifier = userId;
                     data.ModifierDate = DateTime.Now;
                     PurchaseOrderStore.Update(data);
                     // 删除订单-申请关系 
                     var poData = await PurchaseRequestsOrderStore.Entities.Where(t => t.OrderId == data.Id).ToListAsync();
                     poData.ForEach(t => t.IsDelete = true);
                     PurchaseRequestsOrderStore.Update(poData);
                     //清除明细关联订单ID
                     var DetailData = await PurchaseDetailStore.Entities.Where(t => t.OrderId == data.Id).ToListAsync();
                     DetailData.ForEach(t => t.OrderId = null);
                     PurchaseDetailStore.Update(DetailData);
                     //清除订单流水
                     var DetailDatas = await OrderDetailStore.Entities.Where(t => t.OrderId == data.Id).ToListAsync();
                     DetailDatas.ForEach(t => t.IsDelete = true);
                     OrderDetailStore.Update(DetailDatas);
                     //修改订单生成状态
                     var ids = poData.Select(t => t.ApplyId).ToList();

                     var Sum = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Include(t => t.PurchaseRequestsOrders).Where(t => ids.Contains(t.Id)).ToListAsync();
                     foreach (var item in Sum)
                     {
                         var temp = item.PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                         if (temp == null || temp.Count <= 0)
                         {
                             item.OrderType = 0;
                         }
                         else if (temp != null && temp.Count == item.PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                         {
                             item.OrderType = 2;
                         }
                         else if (temp != null && temp.Count < item.PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                         {
                             item.OrderType = 1;
                         }
                         PurchaseRequestStore.Update(item);

                     }



                     await _unitOfWork.SaveChangesAsync();
                 }
                 catch (Exception ex)
                 {
                     throw new Exception(ex.Message, ex);

                 }
                 return Flag;
             });

        }

        //修改订单

        public Task<bool> UpdatePurchasOrder(PurchaseOrderInPut inPut)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {
                    var data = await PurchaseOrderStore.GetFirstOrDefaultAsync(t => inPut.Id == t.Id);
                    if (data == null)
                        throw new Exception("未能获取到订单信息");
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    EntityHelper.CoptyProperty(inPut, data);
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    PurchaseOrderStore.Update(data);
                    _unitOfWork.SaveChanges();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return Flag;
            });
        }
        //查询订单OrderQueryInput
        public Task<PageData<OrderQueryOutPut[]>> GetOrderDataAsync(OrderQueryInput input)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                OrderQueryOutPut[] result = null;
                int count = 0;

                try
                {
                    if (input == null) input = new OrderQueryInput();
                    Expression<Func<PurchaseOrder, bool>> predicate = t => t.IsDelete == IsDel;

                    if (input.BeginApplyDate.HasValue && input.EndApplyDate.HasValue)
                        predicate = predicate.And(t => t.ApplyDate >= input.BeginApplyDate && t.ApplyDate <= input.EndApplyDate);

                    if (input.MedicalItemType > 0)
                        predicate = predicate.And(t => t.MedicalItemType == input.MedicalItemType);
                    if (input.SupplierId + "" != "" && input.SupplierId + "" != "0")
                        predicate = predicate.And(t => t.SupplierId == input.SupplierId);

                    if (input.Remarks.Trim() + "" != "")
                        predicate = predicate.And(t => t.Remarks.Contains(input.Remarks) || t.OrderNo.Contains(input.Remarks) || t.ApplyMain.Contains(input.Remarks));

                    var Sum = PurchaseOrderStore.Entities.Include(t => t.supplier).Include(t => t.warehouseCatalog).Where(predicate).OrderByDescending(t => t.ApplyDate);
                    count = 0;
                    int NoIndex = 0;
                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        var Dialysis = await PaginatedList<PurchaseOrder>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(Dialysis);

                    }
                    else
                    {
                        var datas = await Sum.ToArrayAsync();
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(datas);
                    }
                    foreach (var item in result)
                    {
                        item.no = NoIndex + 1;
                        NoIndex++;

                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return new PageData<OrderQueryOutPut[]>(result, count);
            });
        }

        //修改订单明细

        public Task<bool> UpdateOrderDetailAsync(OrderDetailUpdateInPut input)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                try
                {
                    if (input == null)
                        throw new Exception(MessageFormater.PrameterNeedProvider("明细信息"));

                    var data = await OrderDetailStore.GetFirstOrDefaultAsync(t => input.Id == t.Id);

                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    if (input.SalePrice != data.SalePrice)
                    {
                        data.isEdit = true;
                    }
                    data.HurrySlowly = input.specifications;
                    data.MonthAverage = input.MonthAverage;
                    data.CurrentInventory = input.CurrentInventory;
                    data.SalePrice = input.SalePrice.Value.ToFloorRound();
                    data.SupplierId = input.SupplierId;
                    if (input.SumPrice.HasValue && input.SumPrice > 0 && data.SumPrice != input.SumPrice)
                    {
                        data.SumPrice = input.SumPrice;
                        data.PurchPrice = (input.SumPrice / data.PurchasePuantity).Value.ToRounds(6);
                    }
                    else
                    {
                        data.PurchPrice = input.PurchPrice.Value.ToRounds(6); ;
                        data.SumPrice = (input.PurchPrice * data.PurchasePuantity).Value.ToRounds();
                    }
                    data.Remarks = input.remark;

                    //修改采购明细
                    bool flag = true;
                    foreach (var item in data.PurchDetailIds.Split(','))
                    {
                        var rest = await UpdatePurchasPriceState(new PurchaseDetaiPriceInput() { ItemsId = item, ApprovalQty = -1, InPrice = data.PurchPrice.HasValue ? data.PurchPrice.Value : 0, SalePrice = data.SalePrice.HasValue ? data.SalePrice.Value.ToFloorRound() : 0, SupplierId = data.SupplierId, ProcurementPackage = input.specifications });
                        if (rest == false)
                            flag = false;
                    }
                    if (flag)
                    {
                        OrderDetailStore.Update(data);
                        var orderData = await PurchaseOrderStore.Entities.Where(t => t.Id == data.OrderId).Include(t => t.OrderDetails).FirstOrDefaultAsync();
                        orderData.ActualPrice = orderData.OrderDetails.Sum(t => t.SumPrice);
                        if (orderData.OrderDetails.Count == 1 && orderData.SupplierId != orderData.OrderDetails.First().SupplierId)
                            orderData.SupplierId = orderData.OrderDetails.First().SupplierId;
                        PurchaseOrderStore.Update(orderData);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("数据修改失败");
                    }
                    // UpdatePurchasPriceState(new PurchaseDetaiPriceInput() {  })

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return Flag;
            });
        }

        //查询明细 OrderDetailOutPut  OrderDetail

        public Task<OrderDetailOutPut> GetOrderDetailAsync(string OrderId)
        {
            return Task.Run(async () =>
            {
                OrderDetailOutPut outPut = new OrderDetailOutPut();
                OrderDetailData[] result = null;
                OrderQueryOutPut orderData = null;
                try
                {
                    var OrderData = await PurchaseOrderStore.GetFirstOrDefaultAsync(t => t.Id == OrderId);

                    var data = await OrderDetailStore.Entities.Include(t => t.Center).Include(t => t.Medical).ThenInclude(t => t.PackageUnits).Include(t => t.Medical).ThenInclude(t => t.SpecificationsUnits).Include(t => t.purchaseOrder).Include(t => t.supplier).Where(t => OrderId == t.OrderId).OrderBy(t => t.CenterId).ToListAsync();
                    if (data == null)
                        throw new Exception("未能获取到明细信息");

                    //更新实际到货数量
                    var purchDeailData = await PurchaseDetailStore.Entities.Where(t => t.OrderId == OrderId).ToListAsync();
                    foreach (var item in data)
                    {
                        var Ids = item.PurchDetailIds.Split(',');
                        item.ActualQty = purchDeailData.Where(t => Ids.Contains(t.Id)).Sum(t => t.ActualQty);
                    }
                    OrderDetailStore.Update(data);
                    await _unitOfWork.SaveChangesAsync();
                    List<string> CenterIds
                      = data.Select(t => t.CenterId).CompareDistinct(t => t).ToList();
                    int index = 0;
                    foreach (var item in CenterIds) //统计各透析中心总价
                    {
                        data.Insert(data.Count(t => t.CenterId == item) + index, new OrderDetail()
                        {
                            CenterId = "",
                            SumPrice = data.Where(t => t.CenterId == item).Sum(t => t.SumPrice),
                        });
                        index += data.Count(t => t.CenterId == item) + 1;
                    }
                    data.Add(new OrderDetail()
                    {
                        SumPrice = data.Where(t => t.CenterId != "").Sum(t => t.SumPrice),
                        Founder = "0",
                    });
                    int NoIndex = 1;

                    result = AutoMapperHelper.Map<OrderDetailData[]>(data);

                    //更新库存
                    //  if (listStockData == null || listStockData.Count <= 0)
                    await GetGoodsInStockdInfo();
                    foreach (var item in result)
                    {
                        decimal? StockData = 0;
                        if (orderData.MedicalItemType == 2)
                            StockData = listStockData.Where(t => t.MedicalItemName == item.MedicalName && t.Specifications == item.sFlag && t.CenterId == item.CenterId).Sum(t => t.InQty);
                        else
                            StockData = listStockData.Where(t => t.MedicalItemName == item.MedicalName && t.CenterId == item.CenterId).Sum(t => t.InQty);

                        item.CurrentInventory = StockData;

                        item.no = NoIndex;
                        NoIndex++;
                    }
                    orderData = AutoMapperHelper.Map<OrderQueryOutPut>(OrderData);
                    outPut.orderDetailData = result;
                    outPut.orderData = orderData;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return outPut;
            });

        }


        /// <summary>
        /// 关闭订单（该订单失效）
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public Task<bool> CloseOrderAsync(OrderCloseInput input)
        {

            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {
                    //关闭订单     
                    var data = await PurchaseOrderStore.GetFirstOrDefaultAsync(t => input.OrderId == t.Id);
                    if (data == null)
                        throw new Exception("未能获取到订单信息");
                    //判断该订单是否存在已有到货
                    decimal? SignInCount = await OrderDetailStore.Entities.Where(t => t.OrderId == input.OrderId).SumAsync(t => t.ActualQty);
                    if (SignInCount.HasValue && SignInCount > 0)
                        throw new Exception("该订单存在已收货物品，不能手动关闭，只能关闭明细中未全部到货的项。");

                    if (data.DataState > 4)
                        throw new Exception("该订单还未推送至中心端，无法关闭");
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    data.DataState = 2;
                    data.Remarks = data.Remarks + "" + input.Remark;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    PurchaseOrderStore.Update(data);
                    await _unitOfWork.SaveChangesAsync();
                    var datade = await OrderDetailStore.Entities.Where(t => data.Id == t.OrderId).ToListAsync();
                    bool isTB = false;
                    int index = 1;
                    foreach (var item in datade)
                    {
                        if (index == datade.Count)
                            isTB = true;
                        await CloseOrderDetailAsync(new OrderDetailCloseInput() { OrderDetailId = item.Id, Remark = input.Remark }, isTB);
                        index++;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return Flag;
            });
            //处理中心端流程
        }

        //关闭明细 
        /// <summary>
        /// 关闭明细
        /// </summary>
        /// <param name="DetailId">明细ID</param> 
        /// <returns></returns>
        public Task<bool> CloseOrderDetailAsync(OrderDetailCloseInput input, bool IsTB = false)
        {

            return Task.Run(async () =>
            {
                bool Flag = false;
                try
                {
                    //关闭订单明细
                    var data = await OrderDetailStore.Entities.Include(t => t.purchaseOrder).FirstOrDefaultAsync(t => input.OrderDetailId == t.Id);
                    if (data == null)
                        throw new Exception("未能获取到订单信息");
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    //对接中心端
                    var ids = data.PurchDetailIds.Split(',');
                    var PurchaseDetailData = await PurchaseDetailStore.Entities.Where(t => ids.Contains(t.Id)).ToListAsync();
                    if (data.purchaseOrder.DataState > 4)
                    {

                        var NowOrderDList = await OrderDetailStore.Entities.Where(t => t.OrderId == data.OrderId && t.DataState != 2).ToListAsync();

                        if (NowOrderDList.Count <= 1)
                        {
                            throw new Exception("当前订单只有一条未关闭明细，不可再关闭明细，可直接拒绝该订单");

                        }

                        PurchaseDetailData.ForEach(t => t.ApprovalQty = 0); //关闭本地采购明细  
                        PurchaseDetailStore.Update(PurchaseDetailData);
                        data.DataState = 2;
                        data.Remarks = data.Remarks + "" + input.Remark;
                        data.Modifier = userId;
                        data.PurchasePuantity = 0;
                        data.ModifierDate = DateTime.Now;
                        OrderDetailStore.Update(data);
                        //更新申请单明细   
                        await _unitOfWork.SaveChangesAsync();
                        List<dynamic> ddynamics = new List<dynamic>();
                        foreach (var item in PurchaseDetailData)
                        {
                            ddynamics.Add(new { IsClosed = 1, Id = item.Id, ClosedDetail = input.Remark });
                        }
                        var dflag = await _dataReceive.ClosePurchaseDetails(data.CenterId, ddynamics);

                        return true;
                    }
                    if (data.ActualQty == data.PurchasePuantity)
                        throw new Exception("该明细已全部到货，不能手动关闭。");

                    //盘点明细是否到完或者全部到货 -- 货到完且全关闭，则申请单关闭   
                    List<dynamic> dynamics = new List<dynamic>();
                    foreach (var item in PurchaseDetailData)
                    {
                        dynamics.Add(new { IsClosed = 1, Id = item.Id, ClosedDetail = input.Remark });
                    }
                    var flag = await _dataReceive.ClosePurchaseDetails(data.CenterId, dynamics);
                    if (flag.Code == 200) //中心端关闭成功后处理集团端状态
                    {

                        data.DataState = 2;
                        data.Remarks = data.Remarks + "" + input.Remark;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        OrderDetailStore.Update(data);
                        //更新申请单明细   
                        await _unitOfWork.SaveChangesAsync();
                        //更新主表到货状态
                        var detailData = await OrderDetailStore.Entities.Where(t => t.OrderId == data.OrderId).ToListAsync();
                        var orderData = await PurchaseOrderStore.GetFirstOrDefaultAsync(t => t.Id == data.OrderId);
                        //订单状态 明细全部未收货1 
                        if (detailData.Where(t => t.DataState == 3).Count() > 0)
                            orderData.DataState = 3;
                        if (detailData.Where(t => t.DataState == 1).Count() == detailData.Count)
                            orderData.DataState = 1;
                        if (detailData.Where(t => t.DataState == 4 || t.DataState == 2).Count() == detailData.Count)
                            orderData.DataState = 4;
                        if (detailData.Where(t => t.DataState == 2).Count() == detailData.Count)
                            orderData.DataState = 2;
                        PurchaseOrderStore.Update(orderData);

                        //[{"IsClosed":"","Id":""},{"IsClosed":"","Id":""}]  

                        PurchaseDetailData.ForEach(t => t.IsClosed = 1); //关闭本地采购明细  
                        PurchaseDetailStore.Update(PurchaseDetailData);

                        await _unitOfWork.SaveChangesAsync();
                        Flag = true;

                    }
                    else
                    {
                        throw new Exception("关闭状态同步至中心端出错，请稍后再试");
                    }
                    if (IsTB)
                    {
                        // await _purchasManagerService.SynchronousOrder();//同步
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return Flag;
            });
            //处理中心端流程
        }



        //拆分订单 （根据历史订单，更改供货商后，拆分为新的订单）
        public Task<bool> SplitOrder(string OrderId)
        {
            return Task.Run(async () =>
            {
                bool flag = true;

                var OrderData = await PurchaseOrderStore.Entities.Include(t => t.OrderDetails).Include(t => t.OrderApproves).Where(t => t.Id == OrderId).FirstOrDefaultAsync();
                try
                {
                    if (OrderData != null)
                    {
                        var newdata = OrderData.OrderDetails.GroupBy(t => t.SupplierId);
                        //  bool firset = true;
                        foreach (var item in newdata)
                        {
                            if (item.Key == OrderData.SupplierId) //第一条使用原订单
                            {
                                // OrderData.SupplierId = item.First().SupplierId;

                                OrderData.ItemType = item.Count();
                                OrderData.ActualPrice = item.Sum(t => t.SumPrice);
                                OrderData.TotalQty = item.Sum(t => t.PurchasePuantity);
                                PurchaseOrderStore.Update(OrderData);
                                // firset = false;
                            }
                            else//其他的构建新订单
                            {
                                var NowTime = DateTime.Now;
                                int maxNo = 1;
                                var list = await PurchaseOrderStore.Entities.Where(t => t.ModifierDate > new DateTime(NowTime.Year, NowTime.Month, NowTime.Day, 0, 0, 0)).Select(t => t.OrderNo).ToListAsync();
                                foreach (var itemno in list)
                                {
                                    if (itemno.Length > 10 && Convert.ToInt32(itemno.Substring(itemno.Length - 4, 4)) >= maxNo)
                                        maxNo = Convert.ToInt32(itemno.Substring(itemno.Length - 4, 4)) + 1;
                                }
                                PurchaseOrder newOrder = new PurchaseOrder()
                                {
                                    CenterId = OrderData.CenterId,
                                    ActualPrice = item.Sum(t => t.SumPrice),
                                    ApplyDate = OrderData.ApplyDate,
                                    ApplyMain = OrderData.ApplyMain,
                                    DataState = OrderData.DataState,
                                    Founder = OrderData.Founder,
                                    FounderDate = OrderData.FounderDate,
                                    Id = Guid.NewGuid().tostring32(),
                                    IsDelete = false,
                                    IsPushCenter = false,
                                    ItemType = item.Count(),
                                    MedicalItemType = OrderData.MedicalItemType,
                                    Modifier = OrderData.Modifier,
                                    ModifierDate = DateTime.Now,// OrderData.ModifierDate,
                                    //OrderDetails = item.ToList(),
                                    OrderNo = $"OA-{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}",
                                    Remarks = $"{OrderData.OrderNo}根据供货商自动拆分订单",
                                    SupplierId = item.First().SupplierId,
                                    TotalQty = item.Sum(t => t.PurchasePuantity),
                                    Catalogue = OrderData.Catalogue,
                                    GroupAdvice = OrderData.GroupAdvice,
                                    GroupAuditDate = OrderData.GroupAuditDate,
                                    GroupAuditor = OrderData.GroupAuditor,
                                    GroupAuditStatus = OrderData.GroupAuditStatus,

                                };
                                PurchaseOrderStore.Insert(newOrder);
                                //  await _unitOfWork.SaveChangesAsync();
                                //添加审批流程

                                // OrderApproves
                                foreach (var itemapp in OrderData.OrderApproves)
                                {

                                    OrderApproveStore.Insert(new OrderApprove() { DataState = itemapp.DataState, Advice = itemapp.Advice, AuditDate = itemapp.AuditDate, Auditor = itemapp.Auditor, AuditorLevel = itemapp.AuditorLevel, AuditStatus = itemapp.AuditStatus, Founder = itemapp.Founder, FounderDate = itemapp.FounderDate, Id = Guid.NewGuid().tostring32(), Modifier = itemapp.Modifier, ModifierDate = itemapp.ModifierDate, OrderId = newOrder.Id });
                                }


                                //item.ToList().ForEach(t => t.OrderId = newOrder.Id);
                                List<string> PurchDelId = new List<string>();
                                foreach (var detail in item.ToList())
                                {
                                    detail.OrderId = newOrder.Id;
                                    var ids = detail.PurchDetailIds.Split(',');
                                    PurchDelId.AddRange(ids);
                                }

                                //修改采购明细关联订单号
                                var detaildata = await PurchaseDetailStore.Entities.Where(t => PurchDelId.Contains(t.Id)).ToListAsync();
                                detaildata.ForEach(t => t.OrderId = newOrder.Id);
                                PurchaseDetailStore.Update(detaildata);
                                //修改申请单-订单关联表、
                                foreach (var porder in detaildata.GroupBy(t => t.ApplyId).ToList())
                                {
                                    PurchaseRequestsOrderStore.Insert(new PurchaseRequestsOrder
                                    {
                                        ApplyId = porder.Key,
                                        Id = Guid.NewGuid().tostring32(),
                                        IsDelete = false,
                                        MedicalItemType = OrderData.MedicalItemType,
                                        OrderId = newOrder.Id,
                                        OrderNo = newOrder.OrderNo,
                                        SupplierId = newOrder.SupplierId,

                                    });

                                }

                            }
                            await _unitOfWork.SaveChangesAsync();
                        }


                    }

                }
                catch (Exception exp)
                {

                }

                return flag;


            });
        }


        /// <summary>
        /// 采购信息推送至中心端（采购单下所有明细都推送则下发至中心端）
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public Task<bool> PushCenter(string OrderId)
        {
            return Task.Run(async () =>
            {
                bool flag = true;
                var data = await PurchaseOrderStore.Entities.Include(t => t.OrderDetails).Where(t => OrderId.Contains(t.Id)).FirstOrDefaultAsync();
                if (data != null && data.OrderDetails != null && data.OrderDetails.Count > 0)
                {

                    //更新采购明细
                    List<string> PurchDelId = new List<string>();

                    // var ids = item.OrderDetails.Select(t => t.PurchDetailIds);
                    foreach (var detail in data.OrderDetails)
                    {
                        var ids = detail.PurchDetailIds.Split(',');
                        PurchDelId.AddRange(ids);
                    }

                    if (PurchDelId.Count > 0)
                    {
                        var detaildata = await PurchaseDetailStore.Entities.Where(t => PurchDelId.Contains(t.Id)).ToListAsync();
                        detaildata.ForEach(t => t.IsPushCenter = true);
                        PurchaseDetailStore.Update(detaildata);
                        await _unitOfWork.SaveChangesAsync();


                        //盘点明细是否全部推送，
                        var applyId = detaildata.Select(t => t.ApplyId).CompareDistinct(t => t);

                        var PurchData = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Where(t => applyId.Contains(t.Id)).ToListAsync();
                        foreach (var item in PurchData)
                        {
                            if (item.PurchaseDsOrders.Where(t => (t.IsPushCenter == false || t.IsPushCenter == null) && t.DataState == 1).Count() == 0)
                            {

                                await AddUpdatePurchas(item.Id);
                                //var PurchaseDetaiList = await PurchaseDetailStore.Entities.Where(t => t.ApplyId == Input.Id && t.DataState == 1).ToListAsync();
                                bool restBool = true;
                                foreach (var delitem in item.PurchaseDsOrders.Where(t => t.DataState == 1).ToList())
                                {
                                    var result = await _dataReceive.UpdatePurchasPriceState(item.CenterId, new
                                    {
                                        InPrice = delitem.InPrice.HasValue ? delitem.InPrice : 0,
                                        Id = delitem.Id,
                                        SalePrice = delitem.SalePrice.HasValue ? delitem.SalePrice : 0,
                                        SupplierId = delitem.SupplierId,
                                        ApprovalQty = delitem.ApprovalQty,
                                        Modifier = delitem.Modifier,
                                        ProcurementPackage = delitem.ProcurementPackage,

                                    });
                                    if (!result) restBool = result; //明细修改失败，则审批失败
                                }
                                //对接中心端审核状态
                                if (restBool)
                                {
                                    var purestult = await _dataReceive.UpdatePurchasState(item.CenterId, new
                                    {
                                        GroupAuditStatus = "3",
                                        GroupAuditDate = item.GroupAuditDate,
                                        GroupAdvice = item.GroupAdvice,
                                        GroupAuditor = _getUserInfo.GetCurrentUserEmpNameAsync(item.GroupAuditor).Result,
                                        Modifier = item.Modifier,
                                        ModifierDate = item.ModifierDate,
                                        ApprovalTotalQty = item.ApprovalTotalQty,
                                        Id = item.Id
                                    });

                                    if (purestult)
                                    {
                                        data.IsPushCenter = true; data.SupplierId = data.OrderDetails.FirstOrDefault().SupplierId;
                                        PurchaseOrderStore.Update(data);
                                        await _unitOfWork.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        flag = false;
                                        throw new Exception("中心服务连接异常，推送失败，请稍后再试！");
                                    }
                                }
                                else
                                {
                                    flag = false;
                                    throw new Exception("中心服务连接异常，推送失败，请稍后再试！");
                                }

                            }
                            else
                            {
                                data.IsPushCenter = true; data.SupplierId = data.OrderDetails.FirstOrDefault().SupplierId;
                                PurchaseOrderStore.Update(data);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }

                    }
                }



                return flag;


            });

        }




        public Task<bool> AddCenterOrder(PurchaseOrder order)
        {
            return Task.Run(async () =>
            {
                bool flag = true;

                var MainOrder = await PurchaseOrderStore.Entities.Where(t => t.Id == order.Id).FirstOrDefaultAsync();


                if (MainOrder != null)
                {

                    var MdeOrder = await OrderDetailStore.Entities.Where(t => t.OrderId == order.Id).ToListAsync();
                    OrderDetailStore.RemoveRange(MdeOrder.ToArray());
                    PurchaseOrderStore.Remove(MainOrder);
                    await _unitOfWork.SaveChangesAsync();
                }
                var emp = await EmployeeStore.Entities.Where(t => t.Id == order.Founder).FirstOrDefaultAsync();
                if (emp != null)
                    order.ApplyMain = emp.Name;

                PurchaseOrderStore.Insert(order); //插入采购订单
                OrderDetailStore.Insert(order.OrderDetails);
                await _unitOfWork.SaveChangesAsync();

                return flag;


            });

        }







        #endregion

        #region  退货

        /// <summary>
        /// 立即获取退货列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<ReturnRequestOutPut[]>> CollectGetRequestQueryableAsync(ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                //bool IsDel = false;
                //PurchaseRequestOutPut[] result = null;
                //int count = 0;
                try
                {

                    await _purchasManagerService.ExecuteGetReturnRequest(input.CenterId);
                    //重新获取数据
                    if (input == null || input.PageNum <= 0 || input.PageSize <= 0)
                        input = new ReturnQuerInput() { PageSize = 10, PageNum = 1 };
                    var data = await GetReturnAsync(input);
                    return data;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }


        /// <summary>
        /// 退货订单列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<ReturnRequestOutPut[]>> GetReturnAsync(ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                ReturnRequestOutPut[] returns = null;
                int count = 0;
                Expression<Func<ReturnRequest, bool>> predicate = t => t.DataState == 1 && t.AuditConditionId == "8e3549a587c04c16a187d087dd57cb57"; //筛选提交至集团端
                if (!string.IsNullOrEmpty(input.CenterId) && input.CenterId != "0")
                    predicate = predicate.And(t => t.CenterId == input.CenterId);
                if (input.beginTime.HasValue && input.endTime.HasValue)
                    predicate = predicate.And(t => t.AuditDate >= input.beginTime && t.AuditDate <= input.endTime.Value.AddHours(24 - input.endTime.Value.Hour));
                if (input.AuditStatus != "0" && input.AuditStatus != "1")
                    predicate = predicate.And(t => t.GroupAuditStatus == input.AuditStatus);
                if (input.AuditStatus == "1")
                    predicate = predicate.And(t => t.GroupAuditStatus == null || t.GroupAuditStatus + "" == "");
                var data = ReturnRequestStore.Entities.Include(t => t.Supplier).Include(t => t.center).Include(t => t.employee).Where(predicate).OrderByDescending(t => t.AuditDate);

                if (input.PageNum > 0 && input.PageSize > 0)
                {
                    var pagegdata = await PaginatedList<ReturnRequest>.CreateAsync(data, input.PageNum, input.PageSize);
                    returns = AutoMapperHelper.Map<ReturnRequestOutPut[]>(pagegdata.ToArray());
                    count = pagegdata.Count;
                }
                else
                {
                    returns = AutoMapperHelper.Map<ReturnRequestOutPut[]>(await data.ToArrayAsync());
                    count = returns.Count();
                }
                return new PageData<ReturnRequestOutPut[]>(returns, count);

            });
        }

        //退货明细

        public Task<ReturnDetailsOutPut[]> GetreturnDetailsAsync(string ReturnId)
        {

            return Task.Run(async () =>
            {
                ReturnDetailsOutPut[] returns = null;
                Expression<Func<ReturnDetails, bool>> predicate = t => t.DataState == 1;
                if (!string.IsNullOrEmpty(ReturnId) && ReturnId != "0")
                    predicate = predicate.And(t => t.ReturnId == ReturnId);

                var data = await ReturnDetailsStore.Entities.Include(t => t.MedicalItem).Include(t => t.Unit).Where(predicate).ToListAsync();
                var resutl = AutoMapperHelper.Map<List<ReturnDetailsOutPut>>(data);

                resutl.Add(new ReturnDetailsOutPut()
                {
                    MedicalItemName = "合计",
                    SalesReturnQty = resutl.Sum(t => t.SalesReturnQty),
                    InSumMoney = resutl.Sum(t => t.InSumMoney),

                });
                return resutl.ToArray();

            });
        }


        public Task<bool> UpdatereturnDetailsAsync(ReturnDetailsInPut inPut)
        {

            return Task.Run(async () =>
            {
                try
                {

                    Expression<Func<ReturnDetails, bool>> predicate = t => t.DataState == 1 && t.Id == inPut.Id;

                    var data = await ReturnDetailsStore.Entities.Where(predicate).FirstAsync();
                    if (data != null)
                    {
                        if (data.YSalesReturnQty >= inPut.SalesReturnQty)
                        {
                            data.SalesReturnQty = inPut.SalesReturnQty;
                            data.InSumMoney = inPut.SalesReturnQty * data.InPrice;
                            data.Remark = inPut.Remark;
                            ReturnDetailsStore.Update(data);
                            _unitOfWork.SaveChanges();
                            var MainReturn = await ReturnRequestStore.Entities.Include(t => t.listDetails).Where(t => t.Id == data.ReturnId).FirstAsync();
                            MainReturn.TotalQty = MainReturn.listDetails.Sum(t => t.SalesReturnQty);
                            MainReturn.TotalCost = MainReturn.listDetails.Sum(t => t.InSumMoney);
                            ReturnRequestStore.Update(MainReturn);
                            _unitOfWork.SaveChanges();
                            return true;
                        }
                        else
                        {
                            throw new Exception("修改数量不能大于申请数据");
                        }
                    }
                    else
                        throw new Exception("未能获取到数据");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            });
        }


        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<bool> ReturnExamine(ReturnExamineInPut input)
        {
            return Task.Run(async () =>
            {
                bool flag = true;
                try
                {
                    var reData = await ReturnRequestStore.Entities.Include(t => t.listDetails).Where(t => t.Id == input.Id).FirstOrDefaultAsync();
                    if (reData != null)
                    {
                        var userid = await _getUserInfo.GetCurrentUserIdAsync();
                        reData.GroupAuditStatus = input.GroupAuditStatus;
                        reData.GroupAdvice = input.GroupAdvice;
                        reData.GroupAuditDate = DateTime.Now;
                        reData.GroupAuditor = await _getUserInfo.GetCurrentUserEmpNameAsync(userid);
                        //ReturnRequestStore.Update(reData);
                        //_unitOfWork.SaveChanges();
                        List<MaterialsReturnDetailModel> materials = new List<MaterialsReturnDetailModel>();
                        foreach (var item in reData.listDetails)
                        {
                            if (item.SalesReturnQty != item.YSalesReturnQty)
                                materials.Add(new MaterialsReturnDetailModel()
                                {
                                    Id = item.Id,
                                    MedicalItemId = item.MedicalItemId,
                                    Remark = item.Remark,
                                    SalesReturnQty = item.SalesReturnQty
                                });
                        }
                        //对接中心端
                        var rest = await _dataReceive.ReturnGoods(reData.CenterId, new
                        {
                            Id = reData.Id,
                            GroupAuditStatus = reData.GroupAuditStatus,
                            GroupAuditor = reData.GroupAuditor,
                            GroupAdvice = reData.GroupAdvice,
                            TotalQty = reData.TotalQty,
                            TotalCost = reData.TotalCost,
                            DetailList = materials,
                        });
                        if (rest)
                        {
                            ReturnRequestStore.Update(reData);
                            _unitOfWork.SaveChanges();

                        }
                        else
                        {
                            throw new Exception("对接中心数据失败，请稍后再试");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return flag;
            });
        }


        #endregion

        #region 2019年9月27日采购-订单新流程


        public Task<bool> UpdateTest()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var Sum = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Include(t => t.PurchaseRequestsOrders).ToListAsync();
                    foreach (var item in Sum)
                    {
                        var temp = item.PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                        if (temp == null || temp.Count <= 0)
                        {
                            item.OrderType = 0;
                        }
                        else if (temp != null && temp.Count == item.PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                        {
                            item.OrderType = 2;
                        }
                        else if (temp != null && temp.Count < item.PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                        {
                            item.OrderType = 1;
                        }
                        PurchaseRequestStore.Update(item);

                    }
                    _unitOfWork.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });

        }



        //系统自动拆分采购单（由中心端源头控制）
        //采购申请列表（分管副总批准） 
        //列表

        /// <summary>
        /// 采购单列表  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<PurchaseRequestOutPut[]>> NewGetPurchaseRequestQueryableAsync(PurchaseRequestQueryInput input)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                PurchaseRequestOutPut[] result = null;
                int count = 0;
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore //

                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                var RoleData = _PurchApproval.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                var subRoleData = _purchSubmit.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                //行政副总 低值+固定（非专业） 医疗副总（药品、耗材、专业固定资产）
                List<string> Appids = new List<string>();
                try
                {
                    if (input == null) input = new PurchaseRequestQueryInput();
                    Expression<Func<PurchaseRequest, bool>> predicate = t => t.DataState == 1 && t.IsMerge == false;
                    if (!string.IsNullOrEmpty(input.CenterId) && input.CenterId != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);

                    if (input.BeginTime.HasValue && input.EndTime.HasValue)
                        predicate = predicate.And(t => t.AuditDate >= input.BeginTime && t.AuditDate <= input.EndTime.Value.AddDays(+1));
                    if (input.ApprovalState + "" != "" && input.ApprovalState != "0")
                    {
                        if (input.ApprovalState == "2")
                        {
                            List<string> states = new List<string>() { "2", "6" };
                            predicate = predicate.And(t => states.Contains(t.GroupAuditStatus));
                        }
                        else
                        {
                            predicate = predicate.And(t => t.GroupAuditStatus == input.ApprovalState);
                        }
                    }

                    if (RoleData != null)
                    {
                        //药品、 耗材、 专业固定
                        List<string> cat = new List<string>() { null, "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5", "e07940c4e798455e95680f986a8182fe", "187b752e52414196b763f91fae53a825", "c1fe3b7a2e3a495d958062fa81c9aaf6" };
                        switch (RoleData.RoleId)
                        {
                            case "9496d9f56d354c23a9c95cfa02c7b1f0"://医疗副总 
                                predicate = predicate.And(t => cat.Contains(t.Catalogue) && t.PurchSubmitLevel > 1);
                                break;
                            case "7358c1c008524689a1b665736a276483"://分管副总
                                cat = new List<string>() { null, "97277431fc754297853d9d20cb423b50", "d0913b0979b1422e8545ad78496c35bb", "1db44420ccd1462c98eb65afe83cd569", "c1fe3b7a2e3a495d958062fa81c9aaf6", };
                                //  cat = new List<string>() { null, "97277431fc754297853d9d20cb423b50", "d0913b0979b1422e8545ad78496c35bb", "1db44420ccd1462c98eb65afe83cd569", "c1fe3b7a2e3a495d958062fa81c9aaf6", "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5", "e07940c4e798455e95680f986a8182fe", "187b752e52414196b763f91fae53a825", "c1fe3b7a2e3a495d958062fa81c9aaf6" };
                                predicate = predicate.And(t => cat.Contains(t.Catalogue) && t.PurchSubmitLevel > 1);
                                break;
                            case "bd1e3ba5879045bb9febadb4c8880209"://高管
                                cat = new List<string>() { null, "97277431fc754297853d9d20cb423b50", "d0913b0979b1422e8545ad78496c35bb", "1db44420ccd1462c98eb65afe83cd569", "c1fe3b7a2e3a495d958062fa81c9aaf6", "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5", "e07940c4e798455e95680f986a8182fe", "187b752e52414196b763f91fae53a825", "c1fe3b7a2e3a495d958062fa81c9aaf6" };
                                predicate = predicate.And(t => cat.Contains(t.Catalogue) && t.PurchSubmitLevel > 1);
                                break;
                        }
                    }
                    if (subRoleData != null)
                    {
                        int temp = (subRoleData.level == 1 ? 0 : 1);
                        predicate = predicate.And(t => t.PurchSubmitLevel >= temp);
                    }
                    if (input.Catalogue + "" != "")
                        predicate = predicate.And(t => t.Catalogue == input.Catalogue);

                    if (input.remarks + "" != "")
                    {
                        Appids = await PurchaseDetailStore.Entities.Where(t => t.medicalItemRecord.MedicalItemName.Contains(input.remarks)).Select(t => t.ApplyId).ToListAsync();
                        if (Appids != null && Appids.Count > 0)
                            predicate = predicate.And(t => Appids.Contains(t.Id) || t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));

                        else
                            predicate = predicate.And(t => t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));
                    }
                    if (input.OrderType != 3)
                        predicate = predicate.And(t => t.OrderType == input.OrderType);
                    var Sum = PurchaseRequestStore.Entities.Include(t => t.centerDialysis).Include(t => t.PurchaseDsOrders).Include(t => t.WarehouseCatalog).Include(t => t.PurchaseRequestsOrders).Where(predicate).OrderByDescending(t => t.AuditDate);

                    int NoIndex = 0;
                    List<PurchaseRequest> purchaseRequestsData = new List<PurchaseRequest>();
                    {
                        purchaseRequestsData = await Sum.ToListAsync();
                        //foreach (var item in purchaseRequestsData)
                        //{
                        //    if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                        //        await UpdatePu(item.Id);
                        //}
                        result = AutoMapperHelper.Map<PurchaseRequestOutPut[]>(purchaseRequestsData);
                        count = result.Length;
                    }

                    //foreach (var item in result)
                    //{
                    //    var temp = purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                    //    if (temp == null || temp.Count <= 0)
                    //    {
                    //        item.orderType = 0;
                    //    }
                    //    else if (temp != null && temp.Count == purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                    //    {
                    //        item.orderType = 2;
                    //    }
                    //    else if (temp != null && temp.Count < purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                    //    {
                    //        item.orderType = 1;
                    //    }
                    //    item.no = NoIndex + 1;
                    //    NoIndex++;
                    //}

                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        result = PaginatedList<PurchaseRequestOutPut>.CreateAsync(result.ToList(), input.PageNum, input.PageSize).Result.ToArray();
                        foreach (var item in result)
                        {

                            //var temp = purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                            //if (temp == null || temp.Count <= 0)
                            //{
                            //    item.OrderType = 0;
                            //}
                            //else if (temp != null && temp.Count == purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                            //{
                            //    item.OrderType = 2;
                            //}
                            //else if (temp != null && temp.Count < purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                            //{
                            //    item.OrderType = 1;
                            //}
                            item.no = NoIndex + 1;//
                            item.ActualPrice = item.ActualPrice?.ToRounds();

                            NoIndex++;
                            if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                                await UpdatePu(item.Id);
                            if (subRoleData != null && subRoleData.level >= 1)
                            {
                                if (item.GroupAuditStatus == "6")
                                {
                                    if (item.PurchSubmitLevel >= subRoleData.level)
                                        item.GroupAuditStatus = "6";
                                    else
                                        item.GroupAuditStatus = "2";
                                }
                            }
                            else
                            {
                                if (item.GroupAuditStatus == "6")
                                    item.GroupAuditStatus = "2";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<PurchaseRequestOutPut[]>(result, count);
            });
        }


        public Task<PageData<PurchaseRequestOutPut[]>> NewsGetPurchaseRequestQueryableAsync(PurchaseRequestQueryInput input)
        {


            return Task.Run(async () =>
            {
                bool IsDel = false;
                PurchaseRequestOutPut[] result = null;
                int count = 0;
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore //
                var Approvals = await GetApprovalConfigAsync(1, user.Id);
                var Approval = Approvals.FirstOrDefault();
                //行政副总 低值+固定（非专业） 医疗副总（药品、耗材、专业固定资产） 
                List<string> Appids = new List<string>();
                // 
                try
                {

                    if (input == null) input = new PurchaseRequestQueryInput();
                    if (!input.BeginTime.HasValue)
                        input.BeginTime = DateTime.Now.AddMonths(-6);
                    if (!input.EndTime.HasValue)
                        input.EndTime = DateTime.Now.AddDays(+1);

                    //&& t.Catalogue != "97277431fc754297853d9d20cb423b50" || (t.Catalogue == "97277431fc754297853d9d20cb423b50" && t.TotalCost > 500)
                    Expression<Func<PurchaseRequest, bool>> predicate = t => t.DataState == 1 && t.IsMerge == false && t.CenterId != "d0701c7d2cb242028445fca1034fb4ec";
                    if (!string.IsNullOrEmpty(input.CenterId) && input.CenterId != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);

                    if (input.BeginTime.HasValue && input.EndTime.HasValue)
                        predicate = predicate.And(t => t.AuditDate >= input.BeginTime && t.AuditDate <= input.EndTime.Value.AddDays(+1));
                    if (input.ApprovalState + "" != "" && input.ApprovalState != "0")
                    {
                        if (input.ApprovalState == "2")
                        {
                            //

                            List<string> states = new List<string>() { "2", "6" };
                            predicate = predicate.And(t => states.Contains(t.GroupAuditStatus));

                        }
                        else
                        {
                            predicate = predicate.And(t => t.GroupAuditStatus == input.ApprovalState);
                        }
                    }

                    if (Approval != null && !string.IsNullOrWhiteSpace(Approval.MedicalItemType))
                    {
                        List<string> cat = new List<string>();
                        var mtype = Approval.MedicalItemType.Split(',');
                        foreach (var item in mtype)
                        {

                            switch (item)
                            {
                                case "1":
                                    cat.Add("9807f86a1a8846eb82e85f5a5f92e9e4");
                                    break;
                                case "2":
                                    cat.Add("c1fe3b7a2e3a495d958062fa81c9aaf6");
                                    cat.Add("187b752e52414196b763f91fae53a825");
                                    cat.Add("baf07893f9954f9e8cc1c671ec03e1d5");
                                    break;
                                case "3":
                                    cat.Add("e07940c4e798455e95680f986a8182fe");
                                    cat.Add("d0913b0979b1422e8545ad78496c35bb");
                                    cat.Add("1db44420ccd1462c98eb65afe83cd569");
                                    break;
                                case "4":
                                    cat.Add("97277431fc754297853d9d20cb423b50");

                                    break;
                                default:
                                    break;
                            }
                        }
                        predicate = predicate.And(t => cat.Contains(t.Catalogue) && t.PurchSubmitLevel >= Approval.PurchSubmitLevel - 1);

                    }

                    if (input.Catalogue + "" != "")
                        predicate = predicate.And(t => t.Catalogue == input.Catalogue);

                    if (input.remarks + "" != "")
                    {
                        Appids = await PurchaseDetailStore.Entities.Where(t => t.medicalItemRecord.MedicalItemName.Contains(input.remarks)).Select(t => t.ApplyId).ToListAsync();
                        if (Appids != null && Appids.Count > 0)
                            predicate = predicate.And(t => Appids.Contains(t.Id) || t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));

                        else
                            predicate = predicate.And(t => t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));
                    }
                    if (input.OrderType != 3)
                        predicate = predicate.And(t => t.OrderType == input.OrderType);
                    var Sum = PurchaseRequestStore.Entities.Include(t => t.centerDialysis).Include(t => t.PurchaseDsOrders).Include(t => t.WarehouseCatalog).Include(t => t.PurchaseRequestsOrders).Where(predicate).OrderByDescending(t => t.AuditDate);

                    int NoIndex = 0;

                    // 

                    List<PurchaseRequest> purchaseRequestsData = new List<PurchaseRequest>();
                    {
                        var time = Convert.ToDateTime("2023-05-05");
                        purchaseRequestsData = await Sum.ToListAsync();
                        purchaseRequestsData.RemoveAll(t => t.Catalogue == "97277431fc754297853d9d20cb423b50" && t.AuditDate < time);
                        result = AutoMapperHelper.Map<PurchaseRequestOutPut[]>(purchaseRequestsData);
                        count = result.Length;
                    }


                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        result = PaginatedList<PurchaseRequestOutPut>.CreateAsync(result.ToList(), input.PageNum, input.PageSize).Result.ToArray();
                        foreach (var item in result)
                        {
                            item.no = NoIndex + 1;// 
                            item.ActualPrice = item.ActualPrice?.ToRounds();
                            NoIndex++;
                            if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                                await UpdatePu(item.Id);
                            if (Approval != null && Approval.PurchSubmitLevel >= 1)
                            {
                                if (item.GroupAuditStatus == "6")
                                {
                                    if (item.PurchSubmitLevel >= Approval.PurchSubmitLevel)
                                        item.GroupAuditStatus = "6";
                                    else
                                    {
                                        item.GroupAuditStatus = "2";
                                        item.GroupAdvice = "";
                                        item.GroupAuditDate = null;
                                        item.GroupAuditor = "";

                                    }
                                }
                            }
                            else
                            {
                                if (item.GroupAuditStatus == "6")
                                {
                                    item.GroupAuditStatus = "2";
                                    item.GroupAdvice = "";
                                    item.GroupAuditDate = null;
                                    item.GroupAuditor = "";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<PurchaseRequestOutPut[]>(result, count);
            });
        }


        private Task<List<ApprovalProcessOutPut>> NewgetapprovalProcessOutPuts(PurchaseRequest purchaseRequest, int ApprovalType)
        {
            return Task.Run(async () =>
            {

                var ALLdata = await ApprovalConfigStore.Entities.Where(t => t.DataState == 1 && t.ApprovalType == ApprovalType).OrderBy(t => t.PurchSubmitLevel).ToListAsync();
                List<ApprovalProcessOutPut> approvalProcessOutPuts = new List<ApprovalProcessOutPut>();
                var data = await PurchaseApproveStore.Entities.Include(t => t.user).ThenInclude(t => t.Employee).Where(t => t.ApplyId == purchaseRequest.Id && t.DataState == 1).OrderBy(t => t.AuditDate).ToListAsync();
                var a_approvalProcesses = ALLdata.GroupBy(t => t.PurchSubmitLevel);
                int level = ALLdata.Max(t => t.PurchSubmitLevel);
                int Maxlevel = 0;
                int current = 0;
                string state = "";
                if (data != null && data.Count > 0)
                {

                    Maxlevel = data.Max(t => t.AuditorLevel);

                    foreach (var item in data)
                    {
                        state = item.AuditStatus == 3 ? "同意" : item.AuditStatus == 4 ? "拒绝" : item.AuditStatus == 7 ? "暂缓" : "回退";
                        if (state == "回退" || state == "拒绝")
                            state += $"，备注：{item.Advice}";
                        approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                        {
                            ApprovalName = "",
                            content = $"({item.user.Employee.Name})审查，结果： {state}",
                            title = item.AuditStatus == 4 ? "已拒绝" : "已完成",
                            Time = item.AuditDate.Value.ToString("yyyy-MM-dd"),
                        });
                        current++;
                    }
                    if (data.Last().AuditStatus == 5)
                        Maxlevel = 0;
                    else
                        Maxlevel = data.Last().AuditorLevel;
                    if (data.Last().AuditStatus != 4)
                    {

                        current++;
                        foreach (var item in a_approvalProcesses)
                        {
                            var temp = item.FirstOrDefault();
                            if ((Maxlevel < level && temp.PurchSubmitLevel > Maxlevel && temp.PurchSubmitLevel <= level) && purchaseRequest.GroupAuditStatus == "6")
                                approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                                {
                                    ApprovalName = "",
                                    content = "等待" + temp.UserRemark + "审查",
                                    title = "待进行",
                                });
                        }
                    }
                }
                else
                {

                    foreach (var item in a_approvalProcesses)
                    {
                        var temp = item.FirstOrDefault();
                        if (Maxlevel < level && temp.PurchSubmitLevel <= level)
                            approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                            {
                                ApprovalName = "",
                                content = "等待" + temp.UserRemark + "审查",
                                title = "待进行",
                                current = 0,
                            });
                    }
                }
                //if (level == Maxlevel || state == "拒绝")
                //{
                //    current++;
                //    approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                //    {
                //        ApprovalName = "",
                //        content = "审批结束",
                //        title = "已完成",

                //    });
                //}
                //else
                //{
                //    approvalProcessOutPuts.Add(new ApprovalProcessOutPut()
                //    {
                //        ApprovalName = "",
                //        content = "已完成",
                //        title = "待进行",

                //    });
                //}
                approvalProcessOutPuts.ForEach(t => t.current = current);
                return approvalProcessOutPuts;
            });
        }




        /// <summary>
        /// 获取采购申请单明细列表[]
        /// </summary>
        public Task<PurchaseOutPut> NewGetPurchaseDetailQueryableAsync(string PurchaseRId)
        {

            return Task.Run(async () =>
            {

                List<PurchaseDetailOutPut> result = new List<PurchaseDetailOutPut>();
                List<PurchaseDetailOutPut> NewResult = new List<PurchaseDetailOutPut>();
                PurchaseOutPut purchaseOutPut = new PurchaseOutPut();

                try
                {
                    var MRelevancy = await _dataManager.GetMRelevancyOutPutAsync();
                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore

                    //var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    //var subRoleData = _purchSubmit.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                    var Requestdata = await PurchaseRequestStore.Entities.Include(t => t.PurchaseRequestsOrders).Include(t => t.WarehouseCatalog).Where(t => t.Id == PurchaseRId).FirstOrDefaultAsync();//
                    List<string> PurchaseIds = new List<string>();
                    if (Requestdata == null)
                    {
                        throw new Exception("未能获取到该申请单明细");
                    }
                    if (Requestdata.IsGroupAdd)
                    {
                        //Requestdata.Id;
                        PurchaseIds = await GroupMergePurchaseStore.Entities.Where(t => t.MainPurchaseId == PurchaseRId).Select(t => t.CenterPurchaseId).ToListAsync();
                    }
                    else
                    {
                        PurchaseIds.Add(PurchaseRId);
                    }
                    var Sum = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).ThenInclude(t => t.centerDialysis).
                    Include(t => t.supplier).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.SpecificationsUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.PackageUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.doseUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.supplier).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.ProcurementUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.MedicalDrugExtensions).
                    Include(t => t.purchaseOrder).
                    Where(t => PurchaseIds.Contains(t.ApplyId)).OrderByDescending(t => t.medicalItemRecord.MedicalItemName).ToArrayAsync();

                    decimal? price = 0; //ListDetaildata.Sum(t => t.InPrice);
                    decimal? SalesTotalPrice = 0;
                    decimal? TotalQty = 0;
                    result = AutoMapperHelper.Map<PurchaseDetailOutPut[]>(Sum).OrderByDescending(t => t.coefficient).ToList();
                    int NoIndex = 1;
                    //更新库存
                    //  if (listStockData == null || listStockData.Count <= 0)

                    //(t => new { t.purchaseRequest.CenterId, t.SupplierId });
                    //--------后面下两行得打开
                    await GetGoodsInStockdInfo();
                    await GetMonthUsageStatistics(Requestdata.AuditDate.Value);//更新月均
                    foreach (var Detail in result.GroupBy(t => new { t.CenterName, t.ApplyId }))
                    {
                        NoIndex = 1;
                        string Appid = "";
                        string CenterName = "";
                        foreach (var item in Detail)
                        {
                            CenterName = item.CenterName;
                            if (item.InPrice.HasValue && item.DataState == 1)
                                price += (item.InPrice) * (item.ApprovalQty);
                            if (item.SalePrice.HasValue)
                                SalesTotalPrice += (item.SalePrice) * (item.ApprovalQty);
                            TotalQty += item.ApprovalQty;
                            item.no = NoIndex;
                            NoIndex++;
                            item.InSumMoney = ((item.InPrice) * (item.ApprovalQty)).Value.ToRounds();
                            string mName = item.medicalItemRecordOutPut.MedicalItemName + (item.medicalItemRecordOutPut.GoodsName + "" != "" ? "(" + item.medicalItemRecordOutPut.GoodsName + ")" : "");
                            decimal? StockData = 0;
                            decimal? MonthData = 0;
                            var MRelevancyData = MRelevancy.Where(t => t.MainMedId == item.MedicalItemId).FirstOrDefault();
                            List<RelevancyMedModel> MRelevancyIds = new List<RelevancyMedModel>();
                            if (MRelevancyData != null)
                            {
                                MRelevancyIds = MRelevancyData.ChildMedModel;
                            }
                            //sp = item.medicalItemRecordOutPut.MinDose;


                            var sp = item.medicalItemRecordOutPut.IsSplited == 1 ? item.medicalItemRecordOutPut.MinDose : item.medicalItemRecordOutPut.Packaging;

                            StockData = listStockData.Where(t => (t.MedicalItemName == item.medicalItemRecordOutPut.MedicalItemName && t.Specifications == sp && t.CenterId == item.CenterId && t.Manufacturer == item.medicalItemRecordOutPut.Manufacturer)).Sum(t => t.InQty);

                            if (MRelevancyIds.Count > 0)
                            {
                                foreach (var mitem in MRelevancyIds)
                                {
                                    StockData += listStockData.Where(t => (t.MedicalItemName == mitem.MedicalItemName && t.Specifications == mitem.Packaging && t.CenterId == item.CenterId && t.Manufacturer == mitem.Manufacturer)).Sum(t => t.InQty);
                                }
                            }


                            MonthData = listMonthGoodsInStockdData.Where(t => t.MedicalItemName == item.medicalItemRecordOutPut.MedicalItemName && t.Specifications == sp && t.CenterId == item.CenterId && t.Manufacturer == item.medicalItemRecordOutPut.Manufacturer).Sum(t => t.MonthInQty);
                            if (MRelevancyIds.Count > 0)
                            {
                                foreach (var mitem in MRelevancyIds)
                                {
                                    MonthData += listMonthGoodsInStockdData.Where(t => (t.MedicalItemName == mitem.MedicalItemName && t.Specifications == mitem.Packaging && t.CenterId == item.CenterId && t.Manufacturer == mitem.Manufacturer)).Sum(t => t.MonthInQty);
                                }
                            }

                            if (!item.CurrentInventory.HasValue)
                                item.CurrentInventory = 0;
                            if (!item.MonthAverage.HasValue)
                                item.MonthAverage = 0;
                            //  if (StockData > 0) 
                            item.CurrentInventory = StockData;
                            if (MonthData > 0)
                            {

                                item.MonthAverage = MonthData;
                                //var de = Sum.Where(t => t.Id == item.Id).First();
                                //de.MonthAverage = MonthData; 
                                //PurchaseDetailStore.Update(de);  

                            }
                            if (item.MonthAverage > 0)
                                item.coefficient = Math.Round(((item.ApprovalQty + item.CurrentInventory + 0) / item.MonthAverage).Value, 1);
                            else
                                item.coefficient = 1;
                            NewResult.Add(item);
                            Appid = item.ApplyId;
                        }
                        if (PurchaseIds.Count > 1)
                            NewResult.Add(new PurchaseDetailOutPut()
                            {
                                //  no = NoIndex,
                                CenterName = CenterName,
                                MedicalItemId = "",
                                medicalItemRecordOutPut = new MedicalItemRecordOutPut() { MedicalItemName = $"单号:{Sum.Where(t => t.ApplyId == Appid).First().purchaseRequest.PurchaseNo}" },
                                ApprovalQty = Detail.Sum(T => T.ApprovalQty),
                                InSumMoney = Detail.Sum(t => t.InPrice * t.ApprovalQty),
                                ApplyId = Appid,
                            });
                    }

                    Requestdata.ActualPrice = price?.ToRounds();
                    Requestdata.SalesTotalPrice = SalesTotalPrice;
                    Requestdata.ItemType = Sum.Where(t => t.DataState == 1).Count();
                    Requestdata.ApprovalTotalQty = TotalQty;
                    PurchaseRequestStore.Update(Requestdata);
                    _unitOfWork.SaveChanges();
                    var data = AutoMapperHelper.Map<PurchaseRequestOutPut>(Requestdata);

                    var aconfig = await GetApprovalConfigAsync(1, user.Id);
                    if (aconfig.Length > 0 && aconfig.FirstOrDefault().PurchSubmitLevel >= 1)
                    {
                        if (data.GroupAuditStatus == "6")
                        {
                            if (data.PurchSubmitLevel >= aconfig.FirstOrDefault().PurchSubmitLevel)
                                data.GroupAuditStatus = "6";
                            else
                            {
                                data.GroupAuditStatus = "2";
                                data.GroupAdvice = "";
                                data.GroupAuditDate = null;
                                data.GroupAuditor = "";
                            }
                        }
                    }
                    else
                    {
                        if (data.GroupAuditStatus == "6")
                        {
                            data.GroupAuditStatus = "2";
                            data.GroupAdvice = "";
                            data.GroupAuditDate = null;
                            data.GroupAuditor = "";
                        }
                    }
                    purchaseOutPut.purchaseRequest = data;
                    purchaseOutPut.purchaseDetails = NewResult.ToArray();

                    purchaseOutPut.approvalProcessOutPuts = NewgetapprovalProcessOutPuts(Requestdata, 1).Result.ToArray();
                    //ApprovalProcessOutPut 
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return purchaseOutPut;
            });
        }

        // 
        public Task<PageData<PurchaseRequestOutPut[]>> flNewGetPurchaseRequestQueryableAsync(PurchaseRequestQueryInput input)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                PurchaseRequestOutPut[] result = null;
                int count = 0;
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore 
                List<string> Appids = new List<string>();

                try
                {
                    if (input == null) input = new PurchaseRequestQueryInput();
                    Expression<Func<PurchaseRequest, bool>> predicate = t => t.DataState == 1 && t.IsMerge == false;
                    if (!string.IsNullOrEmpty(input.CenterId) && input.CenterId != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);

                    if (input.BeginTime.HasValue && input.EndTime.HasValue)
                        predicate = predicate.And(t => t.AuditDate >= input.BeginTime && t.AuditDate <= input.EndTime.Value.AddDays(+1));
                    if (input.ApprovalState + "" != "" && input.ApprovalState != "0")
                    {
                        if (input.ApprovalState == "2")
                        {
                            List<string> states = new List<string>() { "2", "6" };
                            predicate = predicate.And(t => states.Contains(t.GroupAuditStatus));
                        }
                        else
                        {
                            predicate = predicate.And(t => t.GroupAuditStatus == input.ApprovalState);
                        }
                    }


                    if (input.Catalogue + "" != "")
                        predicate = predicate.And(t => t.Catalogue == input.Catalogue);

                    if (input.remarks + "" != "")
                    {
                        Appids = await PurchaseDetailStore.Entities.Where(t => t.medicalItemRecord.MedicalItemName.Contains(input.remarks)).Select(t => t.ApplyId).ToListAsync();
                        if (Appids != null && Appids.Count > 0)
                            predicate = predicate.And(t => Appids.Contains(t.Id) || t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));

                        else
                            predicate = predicate.And(t => t.Remark.Contains(input.remarks) || t.PurchaseNo.Contains(input.remarks));
                    }
                    if (input.OrderType != 3)
                        predicate = predicate.And(t => t.OrderType == input.OrderType);
                    var Sum = PurchaseRequestStore.Entities.Include(t => t.centerDialysis).Include(t => t.PurchaseDsOrders).Include(t => t.WarehouseCatalog).Include(t => t.PurchaseRequestsOrders).Where(predicate).OrderByDescending(t => t.AuditDate);

                    int NoIndex = 0;
                    List<PurchaseRequest> purchaseRequestsData = new List<PurchaseRequest>();
                    {
                        purchaseRequestsData = await Sum.ToListAsync();
                        //foreach (var item in purchaseRequestsData)
                        //{
                        //    if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                        //        await UpdatePu(item.Id);
                        //} 
                        result = AutoMapperHelper.Map<PurchaseRequestOutPut[]>(purchaseRequestsData);
                        count = result.Length;
                    }

                    //foreach (var item in result)
                    //{
                    //    var temp = purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                    //    if (temp == null || temp.Count <= 0)
                    //    {
                    //        item.orderType = 0;
                    //    }
                    //    else if (temp != null && temp.Count == purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                    //    {
                    //        item.orderType = 2;
                    //    }
                    //    else if (temp != null && temp.Count < purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                    //    {
                    //        item.orderType = 1;
                    //    }
                    //    item.no = NoIndex + 1;
                    //    NoIndex++;
                    //}

                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        result = PaginatedList<PurchaseRequestOutPut>.CreateAsync(result.ToList(), input.PageNum, input.PageSize).Result.ToArray();
                        foreach (var item in result)
                        {

                            //var temp = purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                            //if (temp == null || temp.Count <= 0)
                            //{
                            //    item.OrderType = 0;
                            //}
                            //else if (temp != null && temp.Count == purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                            //{
                            //    item.OrderType = 2;
                            //}
                            //else if (temp != null && temp.Count < purchaseRequestsData.Where(t => t.Id == item.Id).First().PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                            //{
                            //    item.OrderType = 1;
                            //}
                            item.no = NoIndex + 1;//
                            item.ActualPrice = item.ActualPrice?.ToRounds();
                            NoIndex++;
                            if (!item.MedicaItemTypeCount.HasValue || item.MedicaItemTypeCount <= 0)
                                await UpdatePu(item.Id);

                            if (item.GroupAuditStatus == "6")
                                item.GroupAuditStatus = "2";

                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<PurchaseRequestOutPut[]>(result, count);
            });
        }
        public Task<PurchaseOutPut> flNewGetPurchaseDetailQueryableAsync(string PurchaseRId)
        {

            return Task.Run(async () =>
            {

                List<PurchaseDetailOutPut> result = new List<PurchaseDetailOutPut>();
                List<PurchaseDetailOutPut> NewResult = new List<PurchaseDetailOutPut>();
                PurchaseOutPut purchaseOutPut = new PurchaseOutPut();

                try
                {
                    var Requestdata = await PurchaseRequestStore.Entities.Include(t => t.PurchaseRequestsOrders).Include(t => t.WarehouseCatalog).Where(t => t.Id == PurchaseRId).FirstOrDefaultAsync();//
                    List<string> PurchaseIds = new List<string>();
                    if (Requestdata == null)
                    {
                        throw new Exception("未能获取到该申请单明细");
                    }
                    if (Requestdata.IsGroupAdd)
                    {
                        //Requestdata.Id;
                        PurchaseIds = await GroupMergePurchaseStore.Entities.Where(t => t.MainPurchaseId == PurchaseRId).Select(t => t.CenterPurchaseId).ToListAsync();
                    }
                    else
                    {
                        PurchaseIds.Add(PurchaseRId);
                    }
                    var Sum = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).ThenInclude(t => t.centerDialysis).
                    Include(t => t.supplier).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.SpecificationsUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.PackageUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.doseUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.supplier).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.ProcurementUnits).
                    Include(t => t.medicalItemRecord).ThenInclude(t => t.MedicalDrugExtensions).
                    Include(t => t.purchaseOrder).
                    Where(t => PurchaseIds.Contains(t.ApplyId)).OrderByDescending(t => t.medicalItemRecord.MedicalItemName).ToArrayAsync();

                    decimal? price = 0; //ListDetaildata.Sum(t => t.InPrice);
                    decimal? SalesTotalPrice = 0;
                    decimal? TotalQty = 0;
                    result = AutoMapperHelper.Map<PurchaseDetailOutPut[]>(Sum).OrderByDescending(t => t.coefficient).ToList();
                    int NoIndex = 1;
                    //更新库存
                    //  if (listStockData == null || listStockData.Count <= 0)
                    await GetGoodsInStockdInfo();
                    //(t => new { t.purchaseRequest.CenterId, t.SupplierId });
                    foreach (var Detail in result.GroupBy(t => new { t.CenterName, t.ApplyId }))
                    {
                        NoIndex = 1;
                        string Appid = "";
                        string CenterName = "";
                        foreach (var item in Detail)
                        {
                            CenterName = item.CenterName;
                            if (item.InPrice.HasValue && item.DataState == 1)
                                price += (item.InPrice) * (item.ApprovalQty);
                            if (item.SalePrice.HasValue)
                                SalesTotalPrice += (item.SalePrice) * (item.ApprovalQty);
                            TotalQty += item.ApprovalQty;
                            item.no = NoIndex;
                            NoIndex++;
                            item.InSumMoney = ((item.InPrice) * (item.ApprovalQty)).Value.ToRounds();
                            string mName = item.medicalItemRecordOutPut.MedicalItemName + (item.medicalItemRecordOutPut.GoodsName + "" != "" ? "(" + item.medicalItemRecordOutPut.GoodsName + ")" : "");
                            decimal? StockData = 0;

                            StockData = listStockData.Where(t => t.MedicalItemName == item.medicalItemRecordOutPut.MedicalItemName && t.Specifications == item.medicalItemRecordOutPut.MinDose && t.CenterId == item.CenterId && t.Manufacturer == item.medicalItemRecordOutPut.Manufacturer).Sum(t => t.InQty);
                            //else if (item.medicalItemRecordOutPut.MedicalItemType == 2)
                            //    StockData = listStockData.Where(t => t.MedicalItemName == item.medicalItemRecordOutPut.MedicalItemName && t.CenterId == item.CenterId).Sum(t => t.InQty);
                            if (StockData > 0)
                                item.CurrentInventory = StockData;


                            NewResult.Add(item);
                            Appid = item.ApplyId;
                        }
                        if (PurchaseIds.Count > 1)
                            NewResult.Add(new PurchaseDetailOutPut()
                            {
                                //  no = NoIndex,
                                CenterName = CenterName,
                                MedicalItemId = "",
                                medicalItemRecordOutPut = new MedicalItemRecordOutPut() { MedicalItemName = $"单号:{Sum.Where(t => t.ApplyId == Appid).First().purchaseRequest.PurchaseNo}" },
                                ApprovalQty = Detail.Sum(T => T.ApprovalQty),
                                InSumMoney = Detail.Sum(t => t.InPrice * t.ApprovalQty),
                                ApplyId = Appid,
                            });
                    }

                    Requestdata.ActualPrice = price?.ToRounds();
                    Requestdata.SalesTotalPrice = SalesTotalPrice;
                    Requestdata.ItemType = Sum.Where(t => t.DataState == 1).Count();
                    Requestdata.ApprovalTotalQty = TotalQty;
                    PurchaseRequestStore.Update(Requestdata);
                    _unitOfWork.SaveChanges();
                    var data = AutoMapperHelper.Map<PurchaseRequestOutPut>(Requestdata);


                    purchaseOutPut.purchaseRequest = data;
                    purchaseOutPut.purchaseDetails = NewResult.ToArray();
                    purchaseOutPut.approvalProcessOutPuts = new List<ApprovalProcessOutPut>().ToArray();
                    //ApprovalProcessOutPut 
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return purchaseOutPut;
            });
        }


        public Task<bool> flNewApprovalPurchaseRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var data = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Where(t => t.Id == Input.Id).FirstOrDefaultAsync();
                    if (data == null)
                        throw new Exception("未能获取到当前ID的采购单");
                    if (data.PurchaseDsOrders.Where(t => t.SupplierId + "" == "" && t.DataState == 1).Count() > 0)
                        throw new Exception("请完善采购单明细数据，存在供货商为空的数据");
                    //PurchaseApproveStore


                    //if (RoleData == null)
                    //    throw new Exception("您暂时没有审核采购单的权限");

                    string PendingApprovalValue = "";
                    PurchaseApprove purchaseApprove = new PurchaseApprove()
                    {
                        Advice = Input.GroupAdvice,
                        ApplyId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        ModifierDate = DateTime.Now

                    };
                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    if (Input.GroupAuditStatus == "5")
                    {
                        data.GroupAuditStatus = "2"; data.PurchSubmitLevel = 1;
                    }
                    else
                    {
                        data.GroupAuditStatus = Input.GroupAuditStatus;
                    }
                    PurchaseRequestStore.Update(data);
                    _unitOfWork.SaveChanges();
                    purchaseApprove.AuditorLevel = 2;
                    PendingApprovalValue = $"{name}已审核,等待制作订单，继续进行订单审批";


                    //回退 
                    if (Input.GroupAuditStatus == "5")
                    {
                        var approves = await PurchaseApproveStore.Entities.Where(t => t.ApplyId == Input.Id).ToListAsync();
                        approves.ForEach(t => t.DataState = 2);
                        PurchaseApproveStore.Update(approves);
                        _unitOfWork.SaveChanges();
                        PendingApprovalValue = $"{name}已审核（回退）,等待采购专员重新处理";
                    }

                    //对接中心端审核状态 
                    if (data.GroupAuditStatus == "4") //拒绝 ,直接对接中心端， 如果同意，则走订单
                    {
                        await _dataReceive.UpdatePurchasState(data.CenterId, new
                        {
                            GroupAuditStatus = data.GroupAuditStatus,
                            GroupAuditDate = data.GroupAuditDate,
                            GroupAdvice = data.GroupAdvice,
                            GroupAuditor = name,
                            Modifier = user.UserName,
                            ModifierDate = data.ModifierDate,
                            ApprovalTotalQty = data.ApprovalTotalQty,
                            Id = data.Id
                        });
                        PendingApprovalValue = $"{name}已拒绝该采购单";
                    }

                    PurchaseApproveStore.Insert(purchaseApprove);
                    _unitOfWork.SaveChanges();
                    //await _dataReceive.pushPurchaseApprovesl(data.CenterId, new
                    //{
                    //    PendingApproval = PendingApprovalValue,
                    //    Id = Input.Id
                    //});

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }


        //采购单审批
        public Task<bool> NewApprovalPurchaseRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var data = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Where(t => t.Id == Input.Id).FirstOrDefaultAsync();
                    if (data == null)
                        throw new Exception("未能获取到当前ID的采购单");
                    if (data.PurchaseDsOrders.Where(t => t.SupplierId + "" == "" && t.DataState == 1 && data.Catalogue != "97277431fc754297853d9d20cb423b50").Count() > 0)
                        throw new Exception("请完善采购单明细数据，存在供货商为空的数据");


                    var aconfig = await GetApprovalConfigAsync(1); //审核流程 


                    var subData = aconfig.Where(t => t.UserId == userId).FirstOrDefault(); //当前审核人
                    if (subData == null)
                    {
                        throw new Exception("您暂时没有处理采购单的权限！");
                    }
                    string PendingApprovalValue = "";
                    PurchaseApprove purchaseApprove = new PurchaseApprove()
                    {
                        Advice = Input.GroupAdvice,
                        ApplyId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        AuditorLevel = subData.PurchSubmitLevel,
                        ModifierDate = DateTime.Now

                    };
                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;

                    switch (Input.GroupAuditStatus)
                    {
                        case "5":
                            var approves = await PurchaseApproveStore.Entities.Where(t => t.ApplyId == Input.Id).ToListAsync();
                            approves.ForEach(t => t.DataState = 2);
                            PurchaseApproveStore.Update(approves);
                            _unitOfWork.SaveChanges();
                            data.GroupAuditStatus = "2";
                            data.PurchSubmitLevel = 1;
                            PendingApprovalValue = $"{aconfig.FirstOrDefault().UserRemark}：{aconfig.FirstOrDefault().UserName}已审核（回退）,等待采购专员重新处理";
                            break;
                        case "7":
                            data.GroupAuditStatus = "7";

                            break;

                        default:
                            if (subData.PurchSubmitLevel == aconfig.Max(t => t.PurchSubmitLevel))
                            {
                                data.GroupAuditStatus = Input.GroupAuditStatus;
                                PendingApprovalValue = $"{subData.UserRemark}：{subData.UserName}已审核,等待采购专员制作订单，继续进行订单审批";
                                if (data.Catalogue == "97277431fc754297853d9d20cb423b50")
                                {
                                    PendingApprovalValue = $"采购申请单审批完成，请自行生成采购订单";
                                    List<dynamic> dynamics = new List<dynamic>();
                                    foreach (var item in data.PurchaseDsOrders)
                                    {
                                        dynamics.Add(new { IsPurchase = 1, Id = item.Id });
                                    }
                                    await _dataReceive.PushPurchaseDetail(data.CenterId, dynamics);//

                                    foreach (var delitem in data.PurchaseDsOrders)
                                    {
                                        var result = await _dataReceive.UpdatePurchasPriceState(data.CenterId, new
                                        {

                                            Id = delitem.Id,

                                            ApprovalQty = delitem.DataState == 1 ? delitem.ApprovalQty : 0,
                                            Modifier = delitem.Modifier,
                                            Remark = delitem.Remark,

                                        });
                                    }


                                    await _dataReceive.UpdatePurchasState(data.CenterId, new
                                    {
                                        GroupAuditStatus = data.GroupAuditStatus,
                                        GroupAuditDate = data.GroupAuditDate,
                                        GroupAdvice = data.GroupAdvice,
                                        GroupAuditor = name,
                                        Modifier = user.UserName,
                                        ModifierDate = data.ModifierDate,
                                        ApprovalTotalQty = data.ApprovalTotalQty,
                                        Id = data.Id
                                    });
                                }
                            }
                            else
                            {
                                data.GroupAuditStatus = Input.GroupAuditStatus == "3" ? "6" : Input.GroupAuditStatus;
                                PendingApprovalValue = $"{subData.UserRemark}:{subData.UserName}已审核,等待:{aconfig.Where(t => t.PurchSubmitLevel == subData.PurchSubmitLevel + 1).FirstOrDefault().UserName}审核";

                            }
                            data.PurchSubmitLevel = subData.PurchSubmitLevel;
                            break;
                    }
                    //对接中心端审核状态
                    if (data.GroupAuditStatus == "4") //拒绝 ,直接对接中心端， 如果同意，则走订单
                    {
                        await _dataReceive.UpdatePurchasState(data.CenterId, new
                        {
                            GroupAuditStatus = data.GroupAuditStatus,
                            GroupAuditDate = data.GroupAuditDate,
                            GroupAdvice = data.GroupAdvice,
                            GroupAuditor = name,
                            Modifier = user.UserName,
                            ModifierDate = data.ModifierDate,
                            ApprovalTotalQty = data.ApprovalTotalQty,
                            Id = data.Id
                        });
                        PendingApprovalValue = $"{name}已拒绝该采购单";
                    }
                    PurchaseRequestStore.Update(data);
                    PurchaseApproveStore.Insert(purchaseApprove);
                    _unitOfWork.SaveChanges();
                    await _dataReceive.pushPurchaseApprovesl(data.CenterId, new
                    {
                        PendingApproval = PendingApprovalValue,
                        Id = Input.Id
                    });

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }




        //采购工程师编制采购订单(采购单生成订单)可以使用原流程
        public Task<bool> NewPurchasToOrder(OrderInput inPut)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {
                    /*
                       //配置跨域
            config.EnableCors(new EnableCorsAttribute("*", "*", "*")
            {
                SupportsCredentials = true
            });

                     */
                    if (inPut == null)//|| (inPut.SupplierId + "" == "" || inPut.SupplierId + "0" == "0")
                        throw new Exception("请提供有效参数");
                    //if (inPut.SupplierId + "" == "")
                    //    throw new Exception("请提供供应商");
                    //if (inPut.Catalogue + "" == "")
                    //    throw new Exception("请提供物品类型");
                    //var requests = await PurchaseRequestStore.Entities.Where(t => inPut.ApplyId.Contains(t.Id)).ToListAsync();
                    //var gruopRequ = requests.Where(t => t.IsGroupAdd == true).Select(t => t.Id).ToList();
                    //var oldqeid = requests.Where(t => t.IsGroupAdd == false).Select(t => t.Id).ToList();
                    //List<string> appid = new List<string>();
                    //if (oldqeid != null && oldqeid.Count > 0)
                    //    appid.AddRange(appid);
                    //if (gruopRequ != null && gruopRequ.Count > 0)
                    //{
                    //    var groupData = await GroupMergePurchaseStore.Entities.Where(t => gruopRequ.Contains(t.MainPurchaseId)).Select(t => t.CenterPurchaseId).ToListAsync();

                    //    appid = inPut.ApplyId.ToList();
                    //    appid.RemoveAll(t => gruopRequ.Contains(t));
                    //    appid.AddRange(groupData);
                    //    inPut.ApplyId = appid.ToArray();
                    //}
                    Expression<Func<PurchaseDetail, bool>> predicate = t => t.DataState == 1 && t.OrderId == null;
                    if (inPut.ApplyId != null && inPut.ApplyId.Length > 0)
                        predicate = predicate.And(t => inPut.ApplyId.Contains(t.ApplyId));

                    if (inPut.SupplierId + "" != "" && inPut.SupplierId + "" != "0")
                        predicate = predicate.And(t => t.SupplierId == inPut.SupplierId);

                    if (inPut.detailId != null && inPut.detailId.Length > 0)
                        predicate = predicate.And(t => inPut.detailId.Contains(t.Id));
                    //else
                    //{
                    //    throw new Exception("请提供生成订单的采购明细");
                    //}

                    var Sum = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).Include(t => t.medicalItemRecord).Where(predicate).ToListAsync();
                    if (Sum == null || Sum.Count <= 0)
                        throw new Exception("未能获取到该合并类型的采购明细数据 ！");

                    var caData = await CatalogueItemStore.Entities.Where(t => t.IsDelete == false && t.Id == Sum.First().purchaseRequest.Catalogue).FirstOrDefaultAsync();
                    if (caData == null)
                        throw new Exception("不存在当前提供的物品类型");
                    //var OldData = await PurchaseRequestsOrderStore.Entities.Where(t => t.MedicalItemType == caData.CataIndex && t.SupplierId == inPut.SupplierId && inPut.ApplyId.Contains(t.ApplyId) && t.IsDelete == false).ToListAsync();
                    //if (OldData != null && OldData.Count > 0)
                    //    throw new Exception("已存在该类型的生成订单，请勿重复生成！");  
                    string OrderNo = "";
                    var NowTime = DateTime.Now;
                    int maxNo = 1;

                    var list = await PurchaseOrderStore.Entities.Where(t => t.FounderDate > new DateTime(NowTime.Year, NowTime.Month, NowTime.Day, 0, 0, 0)).Select(t => t.OrderNo).ToListAsync();

                    foreach (var item in list)
                    {
                        if (item.Length > 10 && Convert.ToInt32(item.Substring(item.Length - 4, 4)) >= maxNo)
                            maxNo = Convert.ToInt32(item.Substring(item.Length - 4, 4)) + 1;
                    }
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    Dictionary<string, List<PurchaseDetail>> dicdetails = new Dictionary<string, List<PurchaseDetail>>();
                    var GroupDetailData = Sum.GroupBy(t => new { t.purchaseRequest.CenterId, t.SupplierId });

                    foreach (var item in GroupDetailData)
                    {
                        var caGrupdata = item.ToList();
                        if (caData.Id == "baf07893f9954f9e8cc1c671ec03e1d5")//有价 普通拆分
                        {
                            var GroupDetailDatas = caGrupdata.GroupBy(t => new { t.medicalItemRecord.WareHouseIds });
                            foreach (var items in GroupDetailDatas)
                            {
                                dicdetails.Add(items.Key.WareHouseIds, items.ToList());
                            }
                        }
                        else
                        {
                            dicdetails.Add(item.Key.SupplierId, item.ToList());

                        }

                    }

                    foreach (var Mainitem in dicdetails)//根据供应商、机构
                    {

                        if (caData.FirstLetter + "" != "")
                            OrderNo = $"{caData.FirstLetter}-{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}";
                        else
                            OrderNo = $"OO-{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}";
                        var detailData = Mainitem.Value.GroupBy(t => new { t.MedicalItemId, t.InPrice });

                        PurchaseOrder data = null;
                        data = AutoMapperHelper.Map<PurchaseOrder>(inPut);
                        data.Id = Guid.NewGuid().tostring32();
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.OrderNo = OrderNo;
                        data.Catalogue = caData.Id;// inPut.Catalogue; 
                        data.IsGeneral = Mainitem.Value.First().medicalItemRecord.WareHouseIds == "c1fe3b7a2e3a495d958062fa81c9aaf6" ? false : true;

                        data.MedicalItemType = caData.CataIndex;
                        data.DataState = 5;
                        data.IsDelete = false;

                        data.IsPushCenter = false;
                        data.ActualPrice = Mainitem.Value.Sum(t => t.InPrice * t.ApprovalQty);
                        data.TotalQty = Mainitem.Value.Sum(t => t.ApprovalQty);
                        data.SupplierId = inPut.SupplierId + "" == "" ? Mainitem.Value.First().SupplierId : inPut.SupplierId;//data.SupplierId == "0" ? Sum.FirstOrDefault().SupplierId : data.SupplierId;
                        data.ItemType = detailData.Count();
                        data.GroupAuditStatus = "2";
                        data.CenterId = Mainitem.Value.FirstOrDefault().purchaseRequest.CenterId;
                        PurchaseOrderStore.Insert(data); //插入采购订单 

                        maxNo = maxNo + 1;

                        //插入采购单关联订单 多对多 
                        List<PurchaseRequestsOrder> requestsOrders = new List<PurchaseRequestsOrder>();
                        var Pdata = Sum.Select(t => t.ApplyId).CompareDistinct(t => t);  //await  PurchaseRequestStore.Entities.Where(t => inPut.ApplyId.Contains(t.Id)).Select(t => t.Id).ToListAsync();
                        foreach (var item in Pdata)
                        {
                            if (Mainitem.Value.Where(t => t.ApplyId == item).Count() > 0)
                                requestsOrders.Add(new PurchaseRequestsOrder() { ApplyId = item, MedicalItemType = data.MedicalItemType, IsDelete = false, OrderNo = data.OrderNo, OrderId = data.Id, Id = Guid.NewGuid().tostring32(), SupplierId = data.SupplierId });
                        }
                        PurchaseRequestsOrderStore.Insert(requestsOrders);

                        //当前库存数据
                        // Dictionary<string, GoodsInStockdModel[]> pairs = new Dictionary<string, GoodsInStockdModel[]>();


                        //获取库存
                        List<string> YpHcId = new List<string>() { "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5" };


                        List<OrderDetail> details = new List<OrderDetail>();

                        foreach (var item in detailData)
                        {

                            details.Add(new OrderDetail()
                            {
                                OrderId = data.Id,
                                MedicalId = item.First().MedicalItemId,
                                // CurrentInventory = 100,
                                DataState = 1,
                                Founder = userId,
                                FounderDate = DateTime.Now,
                                Id = Guid.NewGuid().tostring32(),
                                IsDelete = false,
                                Modifier = userId,
                                HurrySlowly = item.First().ProcurementPackage,
                                ModifierDate = DateTime.Now,
                                // MonthAverage = 100,
                                PurchPrice = item.First().InPrice,


                                SalePrice = item.First().SalePrice.HasValue && (data.Catalogue == "9807f86a1a8846eb82e85f5a5f92e9e4" || (data.Catalogue == "baf07893f9954f9e8cc1c671ec03e1d5" && data.IsGeneral)) ? item.First().SalePrice.Value.ToFloorRound() : 0,


                                UpPurchPrice = item.First().InPrice,
                                SupplierId = item.First().SupplierId,
                                PurchasePuantity = item.Sum(t => t.ApprovalQty),
                                PurchDetailIds = string.Join(",", item.Select(t => t.Id).ToArray()),
                                SumPrice = item.Sum(t => t.ApprovalQty) * item.First().InPrice,
                                CenterId = item.First().purchaseRequest.CenterId,
                                CurrentInventory = item.Sum(t => t.CurrentInventory), //CurrentInventory.HasValue ? CurrentInventory : 0,//当前库存
                                MonthAverage = item.Sum(t => t.MonthAverage),//月均用量
                            });
                            foreach (var items in item)
                            {
                                items.SalePrice = item.First().SalePrice.HasValue && (data.Catalogue == "9807f86a1a8846eb82e85f5a5f92e9e4" || (data.Catalogue == "baf07893f9954f9e8cc1c671ec03e1d5" && data.IsGeneral)) ? item.First().SalePrice.Value.ToFloorRound() : 0;
                            }

                        }
                        OrderDetailStore.Insert(details);
                        Mainitem.Value.ToList().ForEach(t => t.OrderId = data.Id);
                        //修改采购明细关联订单
                        PurchaseDetailStore.Update(Sum);
                        await _unitOfWork.SaveChangesAsync();
                        //插入订单明细流水
                        //更新采购申请单生成订单状态
                        var RequestData = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Include(t => t.PurchaseRequestsOrders).Where(t => inPut.ApplyId.Contains(t.Id)).ToListAsync();
                        foreach (var item in RequestData)
                        {

                            var temp = item.PurchaseDsOrders.Where(t => (t.OrderId != null && t.OrderId != "" && t.DataState == 1) && t.DataState == 1).ToList();
                            if (temp == null || temp.Count <= 0)
                            {
                                item.OrderType = 0;
                            }
                            else if (temp != null && temp.Count == item.PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                            {
                                item.OrderType = 2;
                            }
                            else if (temp != null && temp.Count < item.PurchaseDsOrders.Where(t => t.DataState == 1).Count())
                            {
                                item.OrderType = 1;
                            }
                            PurchaseRequestStore.Update(item);

                        }
                        _unitOfWork.SaveChanges();


                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);

                }
                return Flag;

            });
        }
        //订单列表
        public Task<PageData<OrderQueryOutPut[]>> NewGetOrderDataAsync(OrderQueryInput input)
        {

            return Task.Run(async () =>
            {

                bool IsDel = false;
                OrderQueryOutPut[] result = null;
                int count = 0;
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();

                List<string> YpHcId = new List<string>() { "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5" };
                try
                {
                    if (input == null) input = new OrderQueryInput();
                    Expression<Func<PurchaseOrder, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrWhiteSpace(input.CenterId) && input.CenterId + "" != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);
                    if (input.BeginApplyDate.HasValue && input.EndApplyDate.HasValue)
                        predicate = predicate.And(t => t.ApplyDate >= input.BeginApplyDate && t.ApplyDate <= input.EndApplyDate.Value.AddDays(+1));

                    if (input.Catalogue + "" != "")
                        predicate = predicate.And(t => t.Catalogue == input.Catalogue);
                    if (input.SupplierId + "" != "" && input.SupplierId + "" != "0")
                        predicate = predicate.And(t => t.SupplierId == input.SupplierId);
                    if (input.OrderState.HasValue && input.OrderState + 0 > 0)
                    {
                        switch (input.OrderState)
                        {
                            case 5: //未审批
                                predicate = predicate.And(t => t.DataState == input.OrderState || t.GroupAuditStatus == "6");
                                break;
                            case 6://上级待审
                                predicate = predicate.And(t => t.DataState == input.OrderState || t.GroupAuditStatus == "6");
                                break;
                            default:
                                predicate = predicate.And(t => t.DataState == input.OrderState);
                                break;
                        }
                    }
                    if (input.Remarks.Trim() + "" != "")
                    {
                        var Appids = await OrderDetailStore.Entities.Where(t => t.Medical.MedicalItemName.Contains(input.Remarks)).Select(t => t.OrderId).ToListAsync();
                        if (Appids != null && Appids.Count > 0)
                            predicate = predicate.And(t => Appids.Contains(t.Id));
                        else
                            predicate = predicate.And(t => t.Remarks.Contains(input.Remarks) || t.OrderNo.Contains(input.Remarks) || t.ApplyMain.Contains(input.Remarks));
                    }
                    //医保 只显示药品耗材
                    var prroid = _orderPricingRole.Where(t => roleIds.Contains(t.RoleId)).ToList();
                    if (prroid != null && prroid.Count > 0)
                        predicate = predicate.And(t => YpHcId.Contains(t.Catalogue) && t.DataState == 7);
                    var Sum = await PurchaseOrderStore.Entities.Include(t => t.center).Include(t => t.supplier).Include(t => t.OrderApproves).Include(t => t.warehouseCatalog).Where(predicate).OrderByDescending(t => t.ApplyDate).ToListAsync();
                    if (RoleData != null && RoleData.level > 1)
                        Sum = Sum.Where(t => (t.OrderApproves.Where(y => y.AuditorLevel == RoleData.level - 1 && y.AuditStatus == 3 && y.DataState == 1).FirstOrDefault() != null && t.DataState == 6) || (t.DataState != 5 && t.DataState != 6)).OrderByDescending(t => t.FounderDate).ToList();
                    count = 0;
                    int NoIndex = 0;
                    count = Sum.Count();
                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        var Dialysis = await PaginatedList<PurchaseOrder>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(Dialysis);
                    }
                    else
                    {
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(Sum);
                    }
                    foreach (var item in result)
                    {
                        if (RoleData != null && RoleData.level >= 1)
                        {
                            if (item.GroupAuditStatus == "2" || item.GroupAuditStatus == "6")
                            {
                                var statedata = Sum.Where(t => t.Id == item.Id).FirstOrDefault().OrderApproves.Where(t => t.AuditorLevel == RoleData.level && t.DataState == 1).OrderByDescending(t => t.FounderDate).FirstOrDefault();
                                if (statedata == null)
                                {
                                    item.GroupAuditStatus = "2";
                                    item.DataState = 5;
                                }
                                if (statedata != null && statedata.AuditStatus == 5)
                                {
                                    item.GroupAuditStatus = "2";
                                    item.DataState = 5;
                                }
                                else if (item.GroupAuditStatus == "6")
                                {
                                    if (item.GroupAuditStatus == "6" && statedata != null && statedata.AuditStatus == 3)
                                    {
                                        item.GroupAuditStatus = "6";
                                        item.DataState = 6;
                                    }
                                    else if (item.GroupAuditStatus == "6" && statedata == null)
                                    {//
                                        item.GroupAuditStatus = "2";
                                        item.DataState = 5;
                                    }
                                }
                            }
                            //item.GroupAuditStatus+"" == ""?  "2" : 
                        }
                        item.no = NoIndex + 1;
                        NoIndex++;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return new PageData<OrderQueryOutPut[]>(result, count);
            });
        }


        public Task<PageData<OrderQueryOutPut[]>> NewsGetOrderDataAsync(OrderQueryInput input)
        {

            return Task.Run(async () =>
            {

                bool IsDel = false;
                OrderQueryOutPut[] result = null;
                int count = 0;
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                //var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                var Approvals = await GetApprovalConfigAsync(2, user.Id);
                var Approval = Approvals.FirstOrDefault();
                for (int i = 1; i < Approvals.Length; i++)
                {

                    Approval.MedicalItemType += "," + Approvals[i].MedicalItemType;
                }
                List<string> YpHcId = new List<string>() { "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5" };
                try
                {
                    if (!input.BeginApplyDate.HasValue)
                        input.BeginApplyDate = DateTime.Now.AddMonths(-3);
                    if (!input.EndApplyDate.HasValue)
                        input.EndApplyDate = DateTime.Now.AddDays(+1);


                    if (input == null) input = new OrderQueryInput();
                    Expression<Func<PurchaseOrder, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrWhiteSpace(input.CenterId) && input.CenterId + "" != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);
                    if (input.BeginApplyDate.HasValue && input.EndApplyDate.HasValue)
                        predicate = predicate.And(t => t.ApplyDate >= input.BeginApplyDate && t.ApplyDate <= input.EndApplyDate.Value.AddDays(+1));

                    if (input.Catalogue + "" != "")
                        predicate = predicate.And(t => t.Catalogue == input.Catalogue);
                    if (input.SupplierId + "" != "" && input.SupplierId + "" != "0")
                        predicate = predicate.And(t => t.SupplierId == input.SupplierId);
                    if (input.OrderState.HasValue && input.OrderState + 0 > 0)
                    {
                        switch (input.OrderState)
                        {
                            case 5: //未审批
                                predicate = predicate.And(t => t.DataState == input.OrderState || t.GroupAuditStatus == "6");
                                break;
                            case 6://上级待审
                                predicate = predicate.And(t => t.DataState == input.OrderState || t.GroupAuditStatus == "6");
                                break;
                            default:
                                predicate = predicate.And(t => t.DataState == input.OrderState);
                                break;
                        }
                    }
                    if (input.Remarks.Trim() + "" != "")
                    {
                        var Appids = await OrderDetailStore.Entities.Where(t => t.Medical.MedicalItemName.Contains(input.Remarks)).Select(t => t.OrderId).ToListAsync();
                        if (Appids != null && Appids.Count > 0)
                            predicate = predicate.And(t => Appids.Contains(t.Id));
                        else
                            predicate = predicate.And(t => t.Remarks.Contains(input.Remarks) || t.OrderNo.Contains(input.Remarks) || t.ApplyMain.Contains(input.Remarks));
                    }
                    if (Approval != null && !string.IsNullOrWhiteSpace(Approval.MedicalItemType))
                    {
                        List<string> cat = new List<string>();
                        var mtype = Approval.MedicalItemType.Split(',');
                        foreach (var item in mtype)
                        {

                            switch (item)
                            {
                                case "1":
                                    cat.Add("9807f86a1a8846eb82e85f5a5f92e9e4");
                                    break;
                                case "2":
                                    cat.Add("c1fe3b7a2e3a495d958062fa81c9aaf6");
                                    cat.Add("187b752e52414196b763f91fae53a825");
                                    cat.Add("baf07893f9954f9e8cc1c671ec03e1d5");
                                    break;
                                case "3":
                                    cat.Add("e07940c4e798455e95680f986a8182fe");
                                    cat.Add("d0913b0979b1422e8545ad78496c35bb");
                                    cat.Add("1db44420ccd1462c98eb65afe83cd569");
                                    break;
                                case "4":
                                    cat.Add("97277431fc754297853d9d20cb423b50");

                                    break;
                                default:
                                    break;
                            }
                        }
                        predicate = predicate.And(t => cat.Contains(t.Catalogue));

                    }






                    //医保 只显示药品耗材
                    var prroid = _orderPricingRole.Where(t => roleIds.Contains(t.RoleId)).ToList();
                    if (prroid != null && prroid.Count > 0)
                        predicate = predicate.And(t => YpHcId.Contains(t.Catalogue) && t.DataState == 7);
                    var Sum = await PurchaseOrderStore.Entities.Include(t => t.center).Include(t => t.supplier).Include(t => t.OrderApproves).Include(t => t.warehouseCatalog).Where(predicate).OrderByDescending(t => t.ApplyDate).ToListAsync();
                    if (Approval != null && Approval.PurchSubmitLevel > 1)
                        Sum = Sum.Where(t => (t.OrderApproves.Where(y => y.AuditorLevel == Approval.PurchSubmitLevel - 1 && y.AuditStatus == 3 && y.DataState == 1).FirstOrDefault() != null && t.DataState == 6) || (t.DataState != 5 && t.DataState != 6)).OrderByDescending(t => t.FounderDate).ToList();
                    count = 0;
                    int NoIndex = 0;
                    count = Sum.Count();
                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        var Dialysis = await PaginatedList<PurchaseOrder>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(Dialysis);
                    }
                    else
                    {
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(Sum);
                    }
                    foreach (var item in result)
                    {
                        if (Approval != null && Approval.PurchSubmitLevel >= 1)
                        {
                            if (item.GroupAuditStatus == "2" || item.GroupAuditStatus == "6")
                            {
                                var statedata = Sum.Where(t => t.Id == item.Id).FirstOrDefault().OrderApproves.Where(t => t.AuditorLevel == Approval.PurchSubmitLevel && t.DataState == 1).OrderByDescending(t => t.FounderDate).FirstOrDefault();
                                if (statedata == null)
                                {
                                    item.GroupAuditStatus = "2";
                                    item.DataState = 5;
                                }
                                if (statedata != null && statedata.AuditStatus == 5)
                                {
                                    item.GroupAuditStatus = "2";
                                    item.DataState = 5;
                                }
                                else if (item.GroupAuditStatus == "6")
                                {
                                    if (item.GroupAuditStatus == "6" && statedata != null && statedata.AuditStatus == 3)
                                    {
                                        item.GroupAuditStatus = "6";
                                        item.DataState = 6;
                                    }
                                    else if (item.GroupAuditStatus == "6" && statedata == null)
                                    {//
                                        item.GroupAuditStatus = "2";
                                        item.DataState = 5;
                                    }
                                }
                            }
                            //item.GroupAuditStatus+"" == ""?  "2" : 
                        }
                        item.no = NoIndex + 1;
                        NoIndex++;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return new PageData<OrderQueryOutPut[]>(result, count);
            });
        }


        public Task<PageData<OrderQueryOutPut[]>> flNewGetOrderDataAsync(OrderQueryInput input)
        {

            return Task.Run(async () =>
            {


                bool IsDel = false;
                OrderQueryOutPut[] result = null;
                int count = 0;
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore



                try
                {
                    if (input == null) input = new OrderQueryInput();
                    Expression<Func<PurchaseOrder, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrWhiteSpace(input.CenterId) && input.CenterId + "" != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);
                    if (input.BeginApplyDate.HasValue && input.EndApplyDate.HasValue)
                        predicate = predicate.And(t => t.ApplyDate >= input.BeginApplyDate && t.ApplyDate <= input.EndApplyDate.Value.AddDays(+1));

                    if (input.Catalogue + "" != "")
                        predicate = predicate.And(t => t.Catalogue == input.Catalogue);
                    if (input.SupplierId + "" != "" && input.SupplierId + "" != "0")
                        predicate = predicate.And(t => t.SupplierId == input.SupplierId);
                    if (input.OrderState.HasValue && input.OrderState + 0 > 0)
                    {
                        switch (input.OrderState)
                        {
                            case 5: //未审批
                                predicate = predicate.And(t => t.DataState == input.OrderState || t.GroupAuditStatus == "6");
                                break;
                            case 6://上级待审
                                predicate = predicate.And(t => t.DataState == input.OrderState || t.GroupAuditStatus == "6");
                                break;
                            default:
                                predicate = predicate.And(t => t.DataState == input.OrderState);
                                break;
                        }
                    }
                    if (input.Remarks.Trim() + "" != "")
                    {
                        var Appids = await OrderDetailStore.Entities.Where(t => t.Medical.MedicalItemName.Contains(input.Remarks)).Select(t => t.OrderId).ToListAsync();
                        if (Appids != null && Appids.Count > 0)
                            predicate = predicate.And(t => Appids.Contains(t.Id));
                        else
                            predicate = predicate.And(t => t.Remarks.Contains(input.Remarks) || t.OrderNo.Contains(input.Remarks) || t.ApplyMain.Contains(input.Remarks));
                    }
                    var Sum = await PurchaseOrderStore.Entities.Include(t => t.center).Include(t => t.supplier).Include(t => t.OrderApproves).Include(t => t.warehouseCatalog).Where(predicate).OrderByDescending(t => t.ApplyDate).ToListAsync();

                    count = 0;
                    int NoIndex = 0;
                    count = Sum.Count();
                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        NoIndex = (input.PageNum - 1) * input.PageSize;
                        var Dialysis = await PaginatedList<PurchaseOrder>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(Dialysis);
                    }
                    else
                    {
                        result = AutoMapperHelper.Map<OrderQueryOutPut[]>(Sum);
                    }
                    foreach (var item in result)
                    {

                        item.no = NoIndex + 1;
                        NoIndex++;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return new PageData<OrderQueryOutPut[]>(result, count);
            });
        }

        //采购订单明细(包括订单、明细、审批信息)       
        public Task<OrderDetailOutPut> NewGetOrderDetailAsync(string OrderId)
        {
            return Task.Run(async () =>
            {
                OrderDetailOutPut outPut = new OrderDetailOutPut();
                OrderDetailData[] result = null;
                OrderQueryOutPut orderData = null;

                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                var MRelevancy = await _dataManager.GetMRelevancyOutPutAsync();
                try
                {

                    var OrderData = await PurchaseOrderStore.Entities.Include(t => t.OrderApproves).Include(t => t.warehouseCatalog).FirstOrDefaultAsync(t => t.Id == OrderId);

                    var Maiindata = (OrderDetailStore.Entities.Include(t => t.Center).Include(t => t.Medical).ThenInclude(t => t.PackageUnits).Include(t => t.Medical).ThenInclude(t => t.SpecificationsUnits).Include(t => t.Medical).ThenInclude(t => t.MedicalDrugExtensions).Include(t => t.purchaseOrder).Include(t => t.supplier).Where(t => OrderId == t.OrderId)).ToList();
                    //((cp.hilist_lmtpric_type = '901' or cp.hilist_lmtpric_type = '902' or cp.hilist_lmtpric_type is null) and cp.enddate is null)
                    //var querb = from q in Maiindata
                    //            join ac in SI_CheckPriceInfoStore.Entities on q.Medical.NationItemCode equals ac.hilist_code into ud

                    //            from ud1 in ud.DefaultIfEmpty()
                    //            where (ud1.hilist_lmtpric_type == "901" || ud1.hilist_lmtpric_type == null)
                    //            select new { q, ud1 };
                    /*
                      .Join(SI_YPMLSStore.Entities.Select(a => new SI_YPMLS()
                    {   

                        YPLSH = a.YPLSH,
                        YLBZDJ = a.YLBZDJ,

                    }), a => a.Medical.HiCenterCode, b => b.YPLSH, (a, b) => (new { detail = a, HiData = b }))
                     */
                    if (Maiindata == null)
                        throw new Exception("未能获取到明细信息");

                    //var datayy = await querb.ToListAsync();
                    // var data = datayy.Select(a => { var detail = a.q; detail.Medical.SI_CheckPriceInfoS = a.ud1; return detail; }).ToList().GroupBy(t => t.Id);

                    List<OrderDetail> newData = new List<OrderDetail>();
                    var listid = Maiindata.Select(t => t.Medical.NationItemCode).ToList();
                    var price = SI_CheckPriceInfoStore.Entities.Where(t => listid.Contains(t.hilist_code) && (t.hilist_lmtpric_type == "901" || t.hilist_lmtpric_type == null) && t.enddate == null).ToList();

                    var SIData = SI_YPStore.Entities.Where(t => listid.Contains(t.YLMLBM)).ToList();
                    foreach (var item in Maiindata)
                    {
                        item.Medical.SI_CheckPriceInfoS = price.Where(t => t.hilist_code == item.Medical.NationItemCode).FirstOrDefault();
                        //if (item.Count() == 1)
                        newData.Add(item);
                        //else
                        //    newData.Add(item.OrderByDescending(t => t.Medical.SI_CheckPriceInfoS.begndate).First());
                    }
                    List<string> CenterIds
                      = newData.Select(t => t.CenterId).CompareDistinct(t => t).ToList();
                    int index = 0;
                    foreach (var item in CenterIds) //统计各透析中心总价
                    {
                        newData.Insert(newData.Count(t => t.CenterId == item) + index, new OrderDetail()
                        {
                            CenterId = "",
                            SumPrice = newData.Where(t => t.CenterId == item).Sum(t => t.SumPrice),
                        });
                        index += newData.Count(t => t.CenterId == item) + 1;
                    }
                    newData.Add(new OrderDetail()
                    {
                        SumPrice = newData.Where(t => t.CenterId != "").Sum(t => t.SumPrice),
                        Founder = "0",
                    });
                    int NoIndex = 1;

                    result = AutoMapperHelper.Map<OrderDetailData[]>(newData);

                    await GetGoodsInStockdInfo();
                    await GetMonthUsageStatistics(DateTime.Now);//更新月均
                    foreach (var item in result)
                    {
                        item.AgreementPrice = "-";
                        if (item.Id != null)
                        {
                            //公司定价
                            var pri = Maiindata.Where(t => t.Id == item.Id).FirstOrDefault().Medical.MedicalDrugExtensions.Where(t => t.IsCurrentUse == true && t.CenterId == "0").FirstOrDefault();
                            item.AgreementPrice = pri != null && pri.AgreementPrice > 0 ? pri.AgreementPrice.ToFloorRound() + "" : "-";
                        }
                        var med = newData.Where(t => t.Id == item.Id).First();
                        if (med.Medical != null)
                        {
                            //sp = med.Medical.Specifications; 
                            //    if (med.Medical.MedicalItemType == 1)
                            var sp = med.Medical.IsSplited ? med.Medical.Specifications : med.Medical.Packaging;


                            var MRelevancyData = MRelevancy.Where(t => t.MainMedId == med.MedicalId).FirstOrDefault();
                            List<RelevancyMedModel> MRelevancyIds = new List<RelevancyMedModel>();
                            if (MRelevancyData != null)
                            {
                                MRelevancyIds = MRelevancyData.ChildMedModel;
                            }


                            decimal? MonthData = 0;
                            decimal? StockData = 0;
                            var temp = SIData.Where(t => t.YLMLBM == item.Ncode).FirstOrDefault();
                            if (temp != null)
                            {
                                item.SiSpecifications = $"{temp.YPGG}*{temp.ZXBZSL}{temp.ZXZJDW}/{temp.ZXBZDW}";
                                if (item.Uint != temp.ZXBZDW && item.SocialSecurityPrice > 0)
                                {
                                    item.si_SecurityPrice = (item.SocialSecurityPrice / Convert.ToInt32(temp.ZXBZSL)).Value.ToFloorRound();
                                }
                                else
                                {
                                    item.si_SecurityPrice = item.SocialSecurityPrice;
                                }

                            }
                            StockData = listStockData.Where(t => (t.MedicalItemName == med.Medical.MedicalItemName && t.Specifications == sp && t.CenterId == item.CenterId && t.Manufacturer == med.Medical.Manufacturer)).Sum(t => t.InQty);

                            if (MRelevancyIds.Count > 0)
                            {
                                foreach (var mitem in MRelevancyIds)
                                {
                                    StockData += listStockData.Where(t => (t.MedicalItemName == mitem.MedicalItemName && t.Specifications == mitem.Packaging && t.CenterId == item.CenterId && t.Manufacturer == mitem.Manufacturer)).Sum(t => t.InQty);
                                }
                            }


                            MonthData = listMonthGoodsInStockdData.Where(t => t.MedicalItemName == med.Medical.MedicalItemName && t.Specifications == sp && t.CenterId == item.CenterId && t.Manufacturer == med.Medical.Manufacturer).Sum(t => t.MonthInQty);
                            if (MRelevancyIds.Count > 0)
                            {
                                foreach (var mitem in MRelevancyIds)
                                {
                                    MonthData += listMonthGoodsInStockdData.Where(t => (t.MedicalItemName == mitem.MedicalItemName && t.Specifications == mitem.Packaging && t.CenterId == item.CenterId && t.Manufacturer == mitem.Manufacturer)).Sum(t => t.MonthInQty);
                                }
                            }



                            item.CurrentInventory = StockData;
                            item.MonthAverage = MonthData;
                            item.MedicalItemCode = med.Medical.MedicalItemCode;
                            item.Packaging = med.Medical.Packaging;
                        }
                        item.no = NoIndex;
                        NoIndex++;//2102722 2102829 
                        if (!string.IsNullOrWhiteSpace(item.SiSpecifications))
                            item.SiSpecifications = item.SiSpecifications.Replace(',', '，');

                    }
                    orderData = AutoMapperHelper.Map<OrderQueryOutPut>(OrderData);

                    if (RoleData != null && RoleData.level >= 1)
                    {

                        if (orderData.GroupAuditStatus == "2" || orderData.GroupAuditStatus == "6")
                        {
                            var statedata = OrderData.OrderApproves.Where(t => t.AuditorLevel == RoleData.level && t.DataState == 1).FirstOrDefault();
                            if (statedata == null)
                            {
                                orderData.GroupAuditStatus = "2";
                                orderData.DataState = 5;
                            }
                            if (statedata != null && statedata.AuditStatus == 5)
                            {
                                orderData.GroupAuditStatus = "2";
                                orderData.DataState = 5;
                            }
                            else if (orderData.GroupAuditStatus == "6")
                            {
                                if (orderData.GroupAuditStatus == "6" && statedata != null && statedata.AuditStatus == 3)
                                {
                                    orderData.GroupAuditStatus = "6";
                                    orderData.DataState = 6;
                                }
                                else if (orderData.GroupAuditStatus == "6" && statedata == null)
                                {
                                    orderData.GroupAuditStatus = "2";
                                    orderData.DataState = 5;
                                }
                            }
                        }
                        //item.GroupAuditStatus+"" == ""?  "2" : 
                    }
                    outPut.orderDetailData = result;
                    outPut.orderData = orderData;
                    outPut.orderApproveOutPuts = orderData.DataState <= 4 ? new OrderApproveOutPut[0] : getOrderProcessOutPuts(OrderData).Result.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return outPut;
            });

        }


        public Task<OrderDetailOutPut> flNewGetOrderDetailAsync(string OrderId)
        {
            return Task.Run(async () =>
            {
                OrderDetailOutPut outPut = new OrderDetailOutPut();
                OrderDetailData[] result = null;
                OrderQueryOutPut orderData = null;


                try
                {

                    var OrderData = await PurchaseOrderStore.Entities.Include(t => t.OrderApproves).Include(t => t.warehouseCatalog).FirstOrDefaultAsync(t => t.Id == OrderId);

                    var Maiindata = (OrderDetailStore.Entities.Include(t => t.Center).Include(t => t.Medical).ThenInclude(t => t.PackageUnits).Include(t => t.Medical).ThenInclude(t => t.SpecificationsUnits).Include(t => t.Medical).ThenInclude(t => t.MedicalDrugExtensions).Include(t => t.purchaseOrder).Include(t => t.supplier).Where(t => OrderId == t.OrderId)).ToList();
                    //((cp.hilist_lmtpric_type = '901' or cp.hilist_lmtpric_type = '902' or cp.hilist_lmtpric_type is null) and cp.enddate is null)
                    //var querb = from q in Maiindata
                    //            join ac in SI_CheckPriceInfoStore.Entities on q.Medical.NationItemCode equals ac.hilist_code into ud

                    //            from ud1 in ud.DefaultIfEmpty()
                    //            where (ud1.hilist_lmtpric_type == "901" || ud1.hilist_lmtpric_type == null)
                    //            select new { q, ud1 };
                    /*
                      .Join(SI_YPMLSStore.Entities.Select(a => new SI_YPMLS()
                    {
                        YPLSH = a.YPLSH,
                        YLBZDJ = a.YLBZDJ,

                    }), a => a.Medical.HiCenterCode, b => b.YPLSH, (a, b) => (new { detail = a, HiData = b }))
                     */
                    if (Maiindata == null)
                        throw new Exception("未能获取到明细信息");

                    //var datayy = await querb.ToListAsync();
                    // var data = datayy.Select(a => { var detail = a.q; detail.Medical.SI_CheckPriceInfoS = a.ud1; return detail; }).ToList().GroupBy(t => t.Id);

                    List<OrderDetail> newData = new List<OrderDetail>();
                    var listid = Maiindata.Select(t => t.Medical.NationItemCode).ToList();
                    var price = SI_CheckPriceInfoStore.Entities.Where(t => listid.Contains(t.hilist_code) && (t.hilist_lmtpric_type == "901" || t.hilist_lmtpric_type == null) && t.enddate == null).ToList();
                    foreach (var item in Maiindata)
                    {
                        item.Medical.SI_CheckPriceInfoS = price.Where(t => t.hilist_code == item.Medical.NationItemCode).FirstOrDefault();
                        //if (item.Count() == 1)
                        newData.Add(item);
                        //else
                        //    newData.Add(item.OrderByDescending(t => t.Medical.SI_CheckPriceInfoS.begndate).First());
                    }


                    List<string> CenterIds
                      = newData.Select(t => t.CenterId).CompareDistinct(t => t).ToList();
                    int index = 0;
                    foreach (var item in CenterIds) //统计各透析中心总价
                    {
                        newData.Insert(newData.Count(t => t.CenterId == item) + index, new OrderDetail()
                        {
                            CenterId = "",
                            SumPrice = newData.Where(t => t.CenterId == item).Sum(t => t.SumPrice),
                        });
                        index += newData.Count(t => t.CenterId == item) + 1;
                    }
                    newData.Add(new OrderDetail()
                    {
                        SumPrice = newData.Where(t => t.CenterId != "").Sum(t => t.SumPrice),
                        Founder = "0",
                    });
                    int NoIndex = 1;

                    result = AutoMapperHelper.Map<OrderDetailData[]>(newData);
                    //更新库存
                    //  if (listStockData == null || listStockData.Count <= 0)
                    await GetGoodsInStockdInfo();
                    foreach (var item in result)
                    {
                        decimal? StockData = 0;

                        StockData = listStockData.Where(t => t.MedicalItemName == item.MedicalName && t.Specifications == item.Specifications && t.Manufacturer == item.Manufacturer && t.CenterId == item.CenterId).Sum(t => t.InQty);
                        //else if (OrderData.MedicalItemType == 2)
                        //    StockData = listStockData.Where(t => t.MedicalItemName == item.MedicalName && t.CenterId == item.CenterId).Sum(t => t.InQty);

                        item.CurrentInventory = StockData;
                        item.no = NoIndex;
                        NoIndex++;//2102722 2102829 
                    }
                    orderData = AutoMapperHelper.Map<OrderQueryOutPut>(OrderData);

                    outPut.orderDetailData = result;
                    outPut.orderData = orderData;
                    outPut.orderApproveOutPuts = new OrderApproveOutPut[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return outPut;
            });

        }

        //订单审批流程（药品、耗材需走医保部） 将采购审批流程移动到订单流程中   
        private Task<List<OrderApproveOutPut>> getOrderProcessOutPuts(PurchaseOrder purchaseOrder)
        {
            return Task.Run(async () =>
            {


                // await UpdatePu("");
                List<OrderApproveOutPut> approvalProcessOutPuts = new List<OrderApproveOutPut>();
                var ALLdata = await ApprovalConfigStore.Entities.Where(t => t.DataState == 1 && t.ApprovalType == 2).OrderBy(t => t.PurchSubmitLevel).ToListAsync();
                var a_approvalProcesses = ALLdata.GroupBy(t => t.PurchSubmitLevel);
                var data = await OrderApproveStore.Entities.Include(t => t.user).ThenInclude(t => t.Employee).Where(t => t.OrderId == purchaseOrder.Id && t.DataState == 1).OrderBy(t => t.AuditDate).ToListAsync();
                var Mtype = purchaseOrder.MedicalItemType;
                if (Mtype > 4) Mtype = 3;
                int level = ALLdata.Where(t => t.MinAmount <= purchaseOrder.ActualPrice && t.MaxAmount > purchaseOrder.ActualPrice && t.MedicalItemType.Contains(Mtype + "")).Last().PurchSubmitLevel;
                int Maxlevel = 0;
                int current = 0;
                string state = "";
                List<OrderApprove> NewData = new List<OrderApprove>();
                if (data != null && data.Count > 0)
                {
                    NewData = data.Where(t => t.AuditStatus != 5).CompareDistinct(t => t.AuditorLevel).ToList();
                    if (data.Where(t => t.AuditStatus == 5).ToList() != null)
                    {
                        NewData.AddRange(data.Where(t => t.AuditStatus == 5).ToList());
                    }
                    data = NewData.OrderBy(t => t.AuditDate).ToList();
                    Maxlevel = data.Max(t => t.AuditorLevel);
                    approvalProcessOutPuts.Add(new OrderApproveOutPut()
                    {
                        ApprovalName = "",
                        content = "采购员制作采购订单",
                        title = "已完成",

                    });
                    foreach (var item in data)
                    {
                        state = item.AuditStatus == 3 ? "同意" : item.AuditStatus == 4 ? "拒绝" : "回退";
                        if (state == "回退" || state == "拒绝")
                            state += $"，备注：{item.Advice}";
                        //string content = "";
                        //if (a_approvalProcesses.Where(t => t.level == item.AuditorLevel).First().Role == "0b063b50906445719da07631a92aa791")
                        //    content = "";
                        //else

                        approvalProcessOutPuts.Add(new OrderApproveOutPut()
                        {
                            ApprovalName = "",
                            content = $"({item.user.Employee.Name})审批，审核结果： {state}",
                            title = item.AuditStatus == 4 ? "已拒绝" : "已完成",
                            Time = item.AuditDate.Value.ToString("yyyy-MM-dd"),
                        });
                        current++;
                    }
                    if (data.Last().AuditStatus == 5)
                        Maxlevel = 0;
                    else
                        Maxlevel = data.Last().AuditorLevel;
                    if (data.Last().AuditStatus != 4)
                    {

                        current++;
                        foreach (var item in a_approvalProcesses)
                        {
                            var temp = item.FirstOrDefault();
                            if ((Maxlevel < level && temp.PurchSubmitLevel > Maxlevel && temp.PurchSubmitLevel <= level) && (purchaseOrder.GroupAuditStatus == "6" || purchaseOrder.GroupAuditStatus == "2"))
                                approvalProcessOutPuts.Add(new OrderApproveOutPut()
                                {
                                    ApprovalName = "",
                                    content = "等待" + temp.UserRemark + "审批",
                                    title = "待进行",
                                });
                        }
                    }
                }
                else
                {
                    foreach (var item in a_approvalProcesses)
                    {
                        var temp = item.FirstOrDefault();

                        if (Maxlevel < level && temp.PurchSubmitLevel <= level)
                            approvalProcessOutPuts.Add(new OrderApproveOutPut()
                            {
                                ApprovalName = "",
                                content = "等待" + temp.UserRemark + "审批",
                                title = "待进行",
                                current = 0,
                            });
                    }
                }

                approvalProcessOutPuts.ForEach(t => t.current = current);
                return approvalProcessOutPuts;
            });
        }


        //订单审批
        public Task<bool> OrderPurchaseRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {



                    List<string> YpHcId = new List<string>() { "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5" };
                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var data = await PurchaseOrderStore.Entities.Include(t => t.PurchaseRequestsOrders).Include(t => t.OrderApproves).Where(t => t.Id == Input.Id).FirstOrDefaultAsync();
                    if (data == null)
                        return true;

                    if (data.MedicalItemType == 3)//低值特殊处理
                    {
                        var flat = await DZOrderPurchaseRequestAsync(Input);
                        return flat;
                    }
                    //PurchaseApproveStore
                    //var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    //var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();

                    var Approvals = await GetApprovalConfigAsync(2);

                    int? mType = data.MedicalItemType == 5 ? 3 : data.MedicalItemType == 6 ? 3 : data.MedicalItemType == 7 ? 3 : data.MedicalItemType;

                    var Approval = Approvals.Where(t => t.UserId == userId && t.MedicalItemType.Contains(mType + "")).FirstOrDefault();
                    if (Approval == null)
                        throw new Exception("您暂时没有审核采购单的权限");
                    if (Approval.PurchSubmitLevel == 1 && Input.GroupAuditStatus == "5")
                        throw new Exception("当前状态无法回退");
                    OrderApprove purchaseApprove = new OrderApprove()
                    {
                        Advice = Input.GroupAdvice,
                        OrderId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditorLevel = Approval.PurchSubmitLevel,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        ModifierDate = DateTime.Now

                    };

                    //回退 
                    if (Input.GroupAuditStatus == "5")
                    {
                        var approves = await OrderApproveStore.Entities.Where(t => t.OrderId == Input.Id).ToListAsync();
                        approves.ForEach(t => t.DataState = 2);
                        OrderApproveStore.Update(approves);
                        _unitOfWork.SaveChanges();
                    }
                    OrderApproveStore.Insert(purchaseApprove);
                    _unitOfWork.SaveChanges();
                    if (Input.GroupAuditStatus == "5")//回退，从开始位置重新审批
                    {
                        data.GroupAuditStatus = "2";
                        data.GroupAdvice = Input.GroupAdvice;
                        data.GroupAuditDate = DateTime.Now;
                        data.GroupAuditor = userId;
                        data.Modifier = userId;
                        data.DataState = 5;

                        data.ModifierDate = DateTime.Now;
                        PurchaseOrderStore.Update(data);
                        _unitOfWork.SaveChanges();
                        return true;
                    }

                    data.GroupAuditStatus = Input.GroupAuditStatus != "4" ? "6" : "4";
                    data.DataState = 6;
                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    bool cresrult = true;

                    if (((data.ActualPrice < Approval.MaxAmount && data.ActualPrice >= Approval.MinAmount) || Input.GroupAuditStatus == "4"))
                    {
                        data.GroupAuditStatus = Input.GroupAuditStatus;
                        if (YpHcId.Contains(data.Catalogue) && data.IsGeneral)
                            data.DataState = 7; //全部同意， 药品耗材自动进入未定价状态 其他进入未推送状态
                        else
                            data.DataState = 8;
                        //对接中心端审核状态 
                        if (data.GroupAuditStatus == "4") //拒绝 ,直接对接中心端， 如果同意，继续根据权限审批，药品耗材走医保定价
                        {
                            data.DataState = 9;//拒绝 
                            var ResultData = await PurchaseRequestsOrderStore.Entities.Where(t => t.OrderId == Input.Id).ToListAsync();
                            //验证该采购单下所有订单是否全拒绝-是则采购单拒绝，否则修改明细为拒绝

                            foreach (var item in ResultData)
                            {
                                var purchData = await PurchaseRequestsOrderStore.Entities.Include(t => t.purchaseOrder).Include(t => t.FPurchaseRequest).Where(t => t.ApplyId == item.ApplyId).ToListAsync();

                                if (purchData.First().FPurchaseRequest.OrderType == 2 && purchData.Where(t => t.purchaseOrder.DataState == 9).Count() == purchData.Count)
                                {
                                    //全拒绝，订单拒绝
                                    cresrult = await _dataReceive.UpdatePurchasState(data.CenterId, new
                                    {
                                        GroupAuditStatus = data.GroupAuditStatus,
                                        GroupAuditDate = data.GroupAuditDate,
                                        GroupAdvice = data.GroupAdvice,
                                        GroupAuditor = name,
                                        Modifier = user.UserName,
                                        ModifierDate = data.ModifierDate,
                                        ApprovalTotalQty = data.TotalQty,
                                        Id = item.ApplyId, //采购单ID
                                    });
                                    if (!cresrult)
                                        break;
                                }
                                else
                                {
                                    //修改明细为拒绝
                                    var del = await OrderDetailStore.Entities.Where(t => t.OrderId == Input.Id).ToListAsync();
                                    foreach (var d in del)
                                    {
                                        var ids = d.PurchDetailIds.Split(',');
                                        var det = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).Where(t => ids.Contains(t.Id)).ToListAsync();
                                        foreach (var items in det)
                                        {
                                            items.DataState = 2;
                                            PurchaseDetailStore.Update(items);
                                            //对接中心
                                            if (items.IsGroupAdd && items.DataState == 2)
                                                continue;
                                            cresrult = await _dataReceive.UpdatePurchasPriceState(items.purchaseRequest.CenterId, new
                                            {
                                                InPrice = items.InPrice.HasValue ? items.InPrice : 0,
                                                Id = items.Id,
                                                SalePrice = items.SalePrice.HasValue ? items.SalePrice : 0,
                                                SupplierId = items.SupplierId,
                                                ApprovalQty = items.DataState == 1 ? items.ApprovalQty : 0,
                                                Modifier = items.Modifier,
                                                Remark = items.Remark != "手动调整供应商，价格" ? items.Remark : "",
                                                ProcurementPackage = items.ProcurementPackage,
                                            });
                                            if (cresrult == false)
                                                break;
                                        }
                                    }

                                }
                            }
                        }

                    }
                    if (cresrult)
                    {
                        PurchaseOrderStore.Update(data);
                        _unitOfWork.SaveChanges();
                    }
                    else throw new Exception("中心服务连接异常，对接中心审批结果失败，请稍后再试！");
                    string PendingApprovalValue = "";
                    switch (data.DataState)
                    {
                        case 8:
                            PendingApprovalValue = $"订单已审批完成";
                            break;

                        case 7:
                            PendingApprovalValue = $"订单已审批完成，等待医保部定价";
                            break;

                        case 1:
                            PendingApprovalValue = $"订单已审批完成";
                            break;
                        case 9:
                            PendingApprovalValue = $"订单已拒绝";
                            break;
                        default:
                            PendingApprovalValue = $"订单已由{Approval.UserRemark}:{Approval.UserName}审批完成，等待{Approvals.Where(t => Approval.PurchSubmitLevel + 1 == t.PurchSubmitLevel).FirstOrDefault().UserName}审批";
                            break;
                    }

                    foreach (var item in data.PurchaseRequestsOrders)
                    {
                        await _dataReceive.pushPurchaseApprovesl(data.CenterId, new
                        {
                            PendingApproval = PendingApprovalValue,
                            Id = item.ApplyId,
                        });
                    }


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }



        //低值易耗订单审批

        public Task<bool> DZOrderPurchaseRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {
                    List<string> YpHcId = new List<string>() { "9807f86a1a8846eb82e85f5a5f92e9e4", "baf07893f9954f9e8cc1c671ec03e1d5" };
                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var data = await PurchaseOrderStore.Entities.Include(t => t.PurchaseRequestsOrders).Include(t => t.OrderApproves).Where(t => t.Id == Input.Id).FirstOrDefaultAsync();
                    if (data == null)
                        return true;
                    //PurchaseApproveStore
                    //var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    //var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();

                    var Approvals = await GetApprovalConfigAsync(2);

                    int? mType = data.MedicalItemType == 5 ? 3 : data.MedicalItemType == 6 ? 3 : data.MedicalItemType == 7 ? 3 : data.MedicalItemType;

                    var Approval = Approvals.Where(t => t.UserId == userId && t.MedicalItemType.Contains(mType + "")).FirstOrDefault();
                    if (Approval == null)
                        throw new Exception("您暂时没有审核采购单的权限");
                    //if (Approval.PurchSubmitLevel == 1 && Input.GroupAuditStatus == "5")
                    //    throw new Exception("当前状态无法回退");
                    OrderApprove purchaseApprove = new OrderApprove()
                    {
                        Advice = Input.GroupAdvice,
                        OrderId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditorLevel = Approval.PurchSubmitLevel,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        ModifierDate = DateTime.Now

                    };

                    //回退 
                    if (Input.GroupAuditStatus == "5")
                    {
                        var approves = await OrderApproveStore.Entities.Where(t => t.OrderId == Input.Id).ToListAsync();
                        approves.ForEach(t => t.DataState = 2);
                        OrderApproveStore.Update(approves);
                        _unitOfWork.SaveChanges();
                    }
                    OrderApproveStore.Insert(purchaseApprove);
                    _unitOfWork.SaveChanges();
                    if (Input.GroupAuditStatus == "5")//回退，从开始位置重新审批
                    {
                        data.GroupAuditStatus = "1";
                        data.GroupAdvice = Input.GroupAdvice;
                        data.GroupAuditDate = DateTime.Now;
                        data.GroupAuditor = userId;
                        data.Modifier = userId;
                        data.DataState = 5;

                        data.ModifierDate = DateTime.Now;
                        PurchaseOrderStore.Update(data);
                        _unitOfWork.SaveChanges();

                        await _dataReceive.dzpushPurchaseApprovesl(data.CenterId, new
                        {
                            GroupAuditStatus = "1",
                            GroupAuditDate = DateTime.Now,
                            GroupAdvice = Input.GroupAdvice,
                            GroupAuditor = name,
                            Id = Input.Id

                        });
                        return true;
                    }

                    data.GroupAuditStatus = Input.GroupAuditStatus != "4" ? "6" : "4";
                    data.DataState = 6;
                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    bool cresrult = true;

                    if (((data.ActualPrice < Approval.MaxAmount && data.ActualPrice >= Approval.MinAmount) || Input.GroupAuditStatus == "4"))
                    {
                        data.GroupAuditStatus = Input.GroupAuditStatus;

                        data.DataState = 1;
                        //对接中心端审核状态 
                        if (data.GroupAuditStatus == "4") //拒绝 ,直接对接中心端， 如果同意，继续根据权限审批，药品耗材走医保定价
                        {
                            data.DataState = 9;//拒绝  
                        }
                        cresrult = await _dataReceive.dzpushPurchaseApprovesl(data.CenterId, new
                        {
                            GroupAuditStatus = data.GroupAuditStatus,
                            GroupAuditDate = DateTime.Now,
                            GroupAdvice = Input.GroupAdvice,
                            GroupAuditor = name,
                            Id = Input.Id

                        });


                    }
                    if (cresrult)
                    {
                        PurchaseOrderStore.Update(data);
                        _unitOfWork.SaveChanges();
                    }
                    else throw new Exception("中心服务连接异常，对接中心审批结果失败，请稍后再试！");


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }






        public Task<bool> flOrderPurchaseRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {

                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var data = await PurchaseOrderStore.Entities.Include(t => t.PurchaseRequestsOrders).Include(t => t.OrderApproves).Where(t => t.Id == Input.Id).FirstOrDefaultAsync();
                    if (data == null)
                        return true;


                    OrderApprove purchaseApprove = new OrderApprove()
                    {
                        Advice = Input.GroupAdvice,
                        OrderId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditorLevel = 2,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        ModifierDate = DateTime.Now

                    };

                    //回退 
                    if (Input.GroupAuditStatus == "5")
                    {
                        var approves = await OrderApproveStore.Entities.Where(t => t.OrderId == Input.Id).ToListAsync();
                        approves.ForEach(t => t.DataState = 2);
                        OrderApproveStore.Update(approves);
                        _unitOfWork.SaveChanges();
                    }
                    OrderApproveStore.Insert(purchaseApprove);
                    _unitOfWork.SaveChanges();
                    if (Input.GroupAuditStatus == "5")//回退，从开始位置重新审批
                    {
                        data.GroupAuditStatus = "2";
                        data.GroupAdvice = Input.GroupAdvice;
                        data.GroupAuditDate = DateTime.Now;
                        data.GroupAuditor = userId;
                        data.Modifier = userId;
                        data.DataState = 5;

                        data.ModifierDate = DateTime.Now;
                        PurchaseOrderStore.Update(data);
                        _unitOfWork.SaveChanges();
                        return true;
                    }

                    // data.GroupAuditStatus = Input.GroupAuditStatus != "4" ? "6" : "4";
                    // data.DataState = 6;
                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    bool cresrult = true;
                    data.GroupAuditStatus = Input.GroupAuditStatus;


                    data.DataState = 8;
                    //对接中心端审核状态 
                    if (data.GroupAuditStatus == "4") //拒绝 ,直接对接中心端， 如果同意，继续根据权限审批，药品耗材走医保定价
                    {
                        data.DataState = 9;//拒绝 
                        var ResultData = await PurchaseRequestsOrderStore.Entities.Where(t => t.OrderId == Input.Id).ToListAsync();
                        //验证该采购单下所有订单是否全拒绝-是则采购单拒绝，否则修改明细为拒绝

                        foreach (var item in ResultData)
                        {
                            var purchData = await PurchaseRequestsOrderStore.Entities.Include(t => t.purchaseOrder).Include(t => t.FPurchaseRequest).Where(t => t.ApplyId == item.ApplyId).ToListAsync();

                            if (purchData.First().FPurchaseRequest.OrderType == 2 && purchData.Where(t => t.purchaseOrder.DataState == 9).Count() == purchData.Count)
                            {
                                //全拒绝，订单拒绝
                                cresrult = await _dataReceive.UpdatePurchasState(data.CenterId, new
                                {
                                    GroupAuditStatus = data.GroupAuditStatus,
                                    GroupAuditDate = data.GroupAuditDate,
                                    GroupAdvice = data.GroupAdvice,
                                    GroupAuditor = name,
                                    Modifier = user.UserName,
                                    ModifierDate = data.ModifierDate,
                                    ApprovalTotalQty = data.TotalQty,
                                    Id = item.ApplyId, //采购单ID
                                });
                                if (!cresrult)
                                    break;
                            }
                            else
                            {
                                //修改明细为拒绝
                                var del = await OrderDetailStore.Entities.Where(t => t.OrderId == Input.Id).ToListAsync();
                                foreach (var d in del)
                                {
                                    var ids = d.PurchDetailIds.Split(',');
                                    var det = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).Where(t => ids.Contains(t.Id)).ToListAsync();
                                    foreach (var items in det)
                                    {
                                        items.DataState = 2;
                                        PurchaseDetailStore.Update(items);
                                        //对接中心
                                        if (items.IsGroupAdd && items.DataState == 2)
                                            continue;
                                        cresrult = await _dataReceive.UpdatePurchasPriceState(items.purchaseRequest.CenterId, new
                                        {
                                            InPrice = items.InPrice.HasValue ? items.InPrice : 0,
                                            Id = items.Id,
                                            SalePrice = items.SalePrice.HasValue ? items.SalePrice : 0,
                                            SupplierId = items.SupplierId,
                                            ApprovalQty = items.DataState == 1 ? items.ApprovalQty : 0,
                                            Modifier = items.Modifier,
                                            Remark = items.Remark != "手动调整供应商，价格" ? items.Remark : "",
                                            ProcurementPackage = items.ProcurementPackage,
                                        });
                                        if (cresrult == false)
                                            break;
                                    }
                                }

                            }
                        }
                    }


                    if (cresrult)
                    {
                        PurchaseOrderStore.Update(data);
                        _unitOfWork.SaveChanges();
                    }
                    else throw new Exception("中心服务连接异常，对接中心审批结果失败，请稍后再试！");
                    string PendingApprovalValue = "";
                    switch (data.DataState)
                    {
                        case 8:
                            PendingApprovalValue = $"订单已审批完成";
                            break;

                        case 7:
                            PendingApprovalValue = $"订单已审批完成，等待医保部定价";
                            break;

                        case 1:
                            PendingApprovalValue = $"订单已审批完成";
                            break;
                        case 9:
                            PendingApprovalValue = $"订单已拒绝";
                            break;
                        default:

                            break;
                    }

                    //foreach (var item in data.PurchaseRequestsOrders)
                    //{
                    //    await _dataReceive.pushPurchaseApprovesl(data.CenterId, new
                    //    {
                    //        PendingApproval = PendingApprovalValue,
                    //        Id = item.ApplyId,
                    //    });
                    //}


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }

        /// <summary>
        /// 推送订单
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public Task<bool> NewPushCenter(string OrderId)
        {
            return Task.Run(async () =>
            {
                bool flag = true;
                var data = await PurchaseOrderStore.Entities.Include(t => t.OrderDetails).Where(t => OrderId.Contains(t.Id)).FirstOrDefaultAsync();
                if (data != null && data.OrderDetails != null && data.OrderDetails.Count > 0)
                {

                    //更新采购明细
                    List<string> PurchDelId = new List<string>();

                    // var ids = item.OrderDetails.Select(t => t.PurchDetailIds);
                    foreach (var detail in data.OrderDetails)
                    {
                        var ids = detail.PurchDetailIds.Split(',');
                        PurchDelId.AddRange(ids);
                    }

                    if (PurchDelId.Count > 0)
                    {
                        var detaildata = await PurchaseDetailStore.Entities.Where(t => PurchDelId.Contains(t.Id)).ToListAsync();
                        detaildata.ForEach(t => t.IsPushCenter = true);
                        PurchaseDetailStore.Update(detaildata);
                        await _unitOfWork.SaveChangesAsync();


                        //盘点明细是否全部推送，
                        var applyId = detaildata.Select(t => t.ApplyId).CompareDistinct(t => t);

                        var PurchData = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Where(t => applyId.Contains(t.Id)).ToListAsync();
                        foreach (var item in PurchData)
                        {
                            if (item.PurchaseDsOrders.Where(t => (t.IsPushCenter == false || t.IsPushCenter == null) && t.DataState == 1).Count() == 0)
                            {

                                await AddUpdatePurchas(item.Id);
                                //var PurchaseDetaiList = await PurchaseDetailStore.Entities.Where(t => t.ApplyId == Input.Id && t.DataState == 1).ToListAsync();
                                bool restBool = true;
                                foreach (var delitem in item.PurchaseDsOrders.ToList())
                                {
                                    if (delitem.IsGroupAdd && delitem.DataState == 2)
                                        continue;
                                    var result = await _dataReceive.UpdatePurchasPriceState(item.CenterId, new
                                    {
                                        InPrice = delitem.InPrice.HasValue ? delitem.InPrice : 0,
                                        Id = delitem.Id,
                                        SalePrice = delitem.SalePrice.HasValue ? delitem.SalePrice : 0,
                                        SupplierId = delitem.SupplierId,
                                        ApprovalQty = delitem.DataState == 1 ? delitem.ApprovalQty : 0,
                                        Modifier = delitem.Modifier,
                                        Remark = item.Remark != "手动调整供应商，价格" ? item.Remark : "",
                                        ProcurementPackage = delitem.ProcurementPackage,

                                    });
                                    if (!result) restBool = result; //明细修改失败，则审批失败
                                }
                                //对接中心端审核状态
                                if (restBool)
                                {
                                    var purestult = await _dataReceive.UpdatePurchasState(item.CenterId, new
                                    {
                                        GroupAuditStatus = "3",
                                        GroupAuditDate = item.GroupAuditDate,
                                        GroupAdvice = item.GroupAdvice,
                                        GroupAuditor = _getUserInfo.GetCurrentUserEmpNameAsync(item.GroupAuditor).Result,
                                        Modifier = item.Modifier,
                                        ModifierDate = item.ModifierDate,
                                        ApprovalTotalQty = item.ApprovalTotalQty,
                                        Id = item.Id
                                    });

                                    if (purestult)
                                    {
                                        data.IsPushCenter = true; data.DataState = 1;
                                        data.SupplierId = data.OrderDetails.FirstOrDefault().SupplierId;
                                        PurchaseOrderStore.Update(data);
                                        await _unitOfWork.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        flag = false;
                                        throw new Exception("中心服务连接异常，推送失败，请稍后再试！");
                                    }
                                }
                                else
                                {
                                    flag = false;
                                    throw new Exception("中心服务连接异常，推送失败，请稍后再试！");
                                }

                            }
                            else
                            {
                                data.IsPushCenter = true;
                                data.SupplierId = data.OrderDetails.FirstOrDefault().SupplierId;
                                data.DataState = 1;
                                PurchaseOrderStore.Update(data);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }

                    }


                    /*
                     
                  List<dynamic> dynamics = new List<dynamic>();
                        //foreach (var item in PurchaseDetailData)
                        //{
                        //    dynamics.Add(new { IsClosed = 1, Id = item.Id });

                        //}
                        await _dataReceive.pushPurchaseApprovesl(data.CenterId, dynamics);
                 */
                }



                return flag;


            });

        }
        /// <summary>
        /// 推送部分订单
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public Task<bool> PartOrderPushCenter(string OrderId)
        {
            return Task.Run(async () =>
            {
                bool flag = true;
                var data = await PurchaseOrderStore.Entities.Include(t => t.OrderDetails).Where(t => OrderId == t.Id).FirstOrDefaultAsync();
                if (data != null && data.OrderDetails != null && data.OrderDetails.Count > 0)
                {

                    //更新采购明细
                    List<string> PurchDelId = new List<string>();

                    // var ids = item.OrderDetails.Select(t => t.PurchDetailIds);
                    foreach (var detail in data.OrderDetails)
                    {
                        var ids = detail.PurchDetailIds.Split(',');
                        PurchDelId.AddRange(ids);
                    }
                    bool restBool = true;
                    List<dynamic> dynamics = new List<dynamic>();
                    if (PurchDelId.Count > 0)
                    {
                        //获取该订单中的采购明细
                        var detaildata = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).Where(t => PurchDelId.Contains(t.Id)).ToListAsync();
                        var appids = detaildata.Select(t => t.ApplyId).ToList().CompareDistinct(t => t).ToList();
                        var deleteData = await PurchaseDetailStore.Entities.Include(t => t.purchaseRequest).Where(t => appids.Contains(t.ApplyId) && t.DataState == 2).ToListAsync();

                        foreach (var item in appids)
                        {
                            await AddUpdatePurchas(item);
                        }

                        foreach (var item in detaildata) //买了的
                        {
                            if (item.IsGroupAdd && item.DataState == 2)
                                continue;

                            var result = await _dataReceive.UpdatePurchasPriceState(item.purchaseRequest.CenterId, new
                            {
                                InPrice = item.InPrice.HasValue ? item.InPrice : 0,
                                Id = item.Id,
                                SalePrice = item.SalePrice.HasValue ? item.SalePrice.Value.ToFloorRound() : 0,
                                SupplierId = item.SupplierId,
                                ApprovalQty = item.DataState == 1 ? item.ApprovalQty : 0,
                                Modifier = item.Modifier,
                                Remark = item.Remark != "手动调整供应商，价格" ? item.Remark : "",
                                ProcurementPackage = item.ProcurementPackage,

                            });
                            if (!result) restBool = result; //明细修改失败，则审批失败
                            else
                            {
                                dynamics.Add(new { IsPurchase = 1, Id = item.Id });
                            }

                        }

                        foreach (var item in deleteData)//拒绝的
                        {
                            if (item.IsGroupAdd && item.DataState == 2)
                                continue;
                            //   await AddUpdatePurchas(item.purchaseRequest.Id);
                            var result = await _dataReceive.UpdatePurchasPriceState(item.purchaseRequest.CenterId, new
                            {
                                InPrice = item.InPrice.HasValue ? item.InPrice : 0,
                                Id = item.Id,
                                SalePrice = item.SalePrice.HasValue ? item.SalePrice.Value.ToFloorRound() : 0,
                                SupplierId = item.SupplierId,
                                ApprovalQty = item.DataState == 1 ? item.ApprovalQty : 0,
                                Modifier = item.Modifier,
                                Remark = item.Remark != "手动调整供应商，价格" ? item.Remark : "",
                                ProcurementPackage = item.ProcurementPackage,

                            });
                            if (!result) restBool = result; //明细修改失败，则审批失败
                            else
                            {
                                dynamics.Add(new { IsPurchase = 1, Id = item.Id });
                            }

                        }
                        //对接中心端审核状态
                        if (restBool)
                        {
                            var item = detaildata.First().purchaseRequest;//修改中心端审批状态
                            var purestult = await _dataReceive.UpdatePurchasState(item.CenterId, new
                            {
                                GroupAuditStatus = "3",
                                GroupAuditDate = item.GroupAuditDate,
                                GroupAdvice = item.GroupAdvice,
                                GroupAuditor = _getUserInfo.GetCurrentUserEmpNameAsync(item.GroupAuditor).Result,
                                Modifier = item.Modifier,
                                ModifierDate = item.ModifierDate,
                                ApprovalTotalQty = item.ApprovalTotalQty,
                                Id = item.Id
                            });

                            //推送审批流程消息
                            await _dataReceive.pushPurchaseApprovesl(item.CenterId, new
                            {
                                PendingApproval = "订单审批完成",
                                Id = item.Id
                            });


                            var centerresult = await _dataReceive.PushPurchaseDetail(data.CenterId, dynamics);//推送部分订单

                            if (purestult && centerresult)
                            {
                                data.IsPushCenter = true;
                                data.DataState = 1;
                                data.SupplierId = data.OrderDetails.FirstOrDefault().SupplierId;
                                PurchaseOrderStore.Update(data);
                                detaildata.ForEach(t => t.IsPushCenter = true);
                                PurchaseDetailStore.Update(detaildata);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                flag = false;
                                throw new Exception("中心服务连接异常，推送失败，请稍后再试！");
                            }
                        }
                        else
                        {
                            flag = false;
                            throw new Exception("中心服务连接异常，推送失败，请稍后再试！");
                        }

                    }


                    /*
                     
                  List<dynamic> dynamics = new List<dynamic>();
                        //foreach (var item in PurchaseDetailData)
                        //{
                        //    dynamics.Add(new { IsClosed = 1, Id = item.Id });

                        //}
                        await _dataReceive.pushPurchaseApprovesl(data.CenterId, dynamics);
                 */
                }



                return flag;


            });

        }


        //药品耗材定价（审批完成未推送之前）
        public Task<bool> OrderPricingAsync(string OrderId)
        {
            return Task.Run(async () =>
            {
                bool flag = true;
                var data = await PurchaseOrderStore.Entities.Include(t => t.PurchaseRequestsOrders).Where(t => OrderId == t.Id).FirstOrDefaultAsync();
                if (data == null)
                    throw new Exception("未能获取到该订单！");
                if (data.DataState != 7 || data.GroupAuditStatus != "3")
                    throw new Exception("该订单暂未到达定价流程！");
                data.DataState = 8;
                PurchaseOrderStore.Update(data);
                await _unitOfWork.SaveChangesAsync();
                foreach (var item in data.PurchaseRequestsOrders)
                {
                    await _dataReceive.pushPurchaseApprovesl(data.CenterId, new
                    {
                        PendingApproval = "医保部已定价，等待推送",
                        Id = item.ApplyId,
                    });
                }

                return flag;
            });

        }

        #endregion

        #region 2020年1月14日14:18:15 未审批采购单合并后再审批

        //合并条件：（未审批且同一类型） ，合并后可新增，修改单子明细，（然后继续走审批流程）

        /// <summary>
        /// 合并未审批的申请单，进行统一审批，生成订单
        /// </summary>
        /// <param name="CenterPurchaseId"></param>
        /// <returns></returns>
        public Task<PurchaseOutPut> GroupMergePurchaseAsync(string[] CenterPurchaseId)
        {
            return Task.Run(async () =>
            {
                // public Task<PurchaseOutPut> NewGetPurchaseDetailQueryableAsync(string PurchaseRId)
                bool flag = true;

                if (CenterPurchaseId == null || CenterPurchaseId.Length <= 0)
                {
                    throw new Exception("需要合并的申请单不能为空！");
                }
                var PurchaseData = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Where(t => CenterPurchaseId.Contains(t.Id)).ToListAsync();

                if (PurchaseData != null && PurchaseData.Where(t => t.IsMerge == true || t.IsGroupAdd == true).Count() > 0)
                    throw new Exception("存在已合并的申请单");
                if (PurchaseData != null && PurchaseData.GroupBy(t => t.Catalogue).Count() > 1)
                    throw new Exception("请合并同一种类的采购单");
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                var NowTime = DateTime.Now;
                int maxNo = 1;
                var list = await PurchaseRequestStore.Entities.Where(t => t.FounderDate > new DateTime(NowTime.Year, NowTime.Month, NowTime.Day, 0, 0, 0) && t.IsGroupAdd == true).Select(t => t.PurchaseNo).ToListAsync();
                foreach (var item in list)
                {
                    if (item.Length > 10 && Convert.ToInt32(item.Substring(item.Length - 4, 4)) >= maxNo)
                        maxNo = Convert.ToInt32(item.Substring(item.Length - 4, 4)) + 1;
                }
                //复合条件，进行合并   
                PurchaseRequest purchaseRequest = new PurchaseRequest()
                {
                    Id = Guid.NewGuid().tostring32(),
                    ActualPrice = PurchaseData.Sum(t => t.PurchaseDsOrders.Sum(k => k.InPrice * k.ApprovalQty)),
                    Advice = PurchaseData.First().Advice,
                    ApprovalTotalQty = PurchaseData.Sum(t => t.PurchaseDsOrders.Sum(k => k.ApprovalQty)),
                    AuditConditionId = PurchaseData.First().AuditConditionId,
                    AuditDate = DateTime.Now,
                    Auditor = name,
                    Catalogue = PurchaseData.First().Catalogue,
                    GroupAuditStatus = "2",
                    CenterId = "0",
                    DataState = 1,
                    Founder = userId,
                    FounderDate = DateTime.Now,
                    IsClosed = 2,
                    IsERP = 0,
                    IsMerge = false,
                    IsGroupAdd = true,
                    ItemType = PurchaseData.Sum(t => t.ItemType),
                    MedicaItemTypeCount = 1,
                    Modifier = userId,
                    ModifierDate = DateTime.Now,
                    PurchaseNo = $"JTHB{NowTime.ToString("yyyyMMdd")}{maxNo.ToString().PadLeft(4, '0')}",
                    PurchSubmitLevel = 1,
                    Remark = "合并采购单",
                    TotalQty = PurchaseData.Sum(t => t.PurchaseDsOrders.Sum(k => k.InQty)),
                    SalesTotalPrice = PurchaseData.Sum(t => t.PurchaseDsOrders.Sum(k => k.SalePrice * k.ApprovalQty)),
                    TotalCost = PurchaseData.Sum(t => t.PurchaseDsOrders.Sum(k => k.InPrice * k.InQty)),
                };
                PurchaseRequestStore.Insert(purchaseRequest);
                PurchaseData.ForEach(t => t.IsMerge = true);
                PurchaseRequestStore.Update(PurchaseData);
                //GroupMergePurchaseStore
                foreach (var item in PurchaseData)
                {
                    GroupMergePurchaseStore.Insert(new GroupMergePurchase()
                    {

                        CenterPurchaseId = item.Id,
                        Id = Guid.NewGuid().tostring32(),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        MainPurchaseId = purchaseRequest.Id,
                        Modifier = userId,
                        ModifierDate = DateTime.Now,
                        Remark = ""
                    });

                }

                await _unitOfWork.SaveChangesAsync();
                var resuleData = await NewGetPurchaseDetailQueryableAsync(purchaseRequest.Id);
                return resuleData;
            });
        }


        //
        /// <summary>
        /// 拆分已合并申请单 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<bool> MergePurchaseBreakAsync(MergePurchaseBreakInput input)
        {
            return Task.Run(async () =>
            {
                bool flag = true;
                var MainData = await PurchaseRequestStore.Entities.Where(t => t.Id == input.MergePurchaseId).FirstOrDefaultAsync();
                var purchData = await PurchaseRequestStore.Entities.Where(t => t.Id == input.ApplyId).FirstOrDefaultAsync();

                if (!purchData.IsMerge)
                    throw new Exception("该采购单暂未合并");
                purchData.IsMerge = false;
                PurchaseRequestStore.Update(purchData);
                var mergeData = await GroupMergePurchaseStore.Entities.Where(t => t.MainPurchaseId == input.MergePurchaseId).ToListAsync();
                if (mergeData != null && mergeData.Count == 2) //合并里面只有两条，删除合并单
                {
                    GroupMergePurchaseStore.Remove(mergeData);
                    PurchaseRequestStore.Remove(MainData);
                    string id = mergeData.Where(t => t.CenterPurchaseId != input.ApplyId).First().CenterPurchaseId;
                    var tempData = await PurchaseRequestStore.Entities.Where(t => t.Id == id).FirstOrDefaultAsync();
                    tempData.IsMerge = false;
                    PurchaseRequestStore.Update(tempData);
                }
                else
                {
                    var data = mergeData.Where(t => t.CenterPurchaseId == input.ApplyId).First();
                    GroupMergePurchaseStore.Remove(data);
                }

                _unitOfWork.SaveChanges();
                return flag;
            });
        }


        /// <summary>
        /// 新采购单并入合并单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<bool> MergePurchaseAddAsync(MergePurchaseBreakInput input)
        {
            return Task.Run(async () =>
            {
                bool flag = true;

                var purchData = await PurchaseRequestStore.Entities.Where(t => t.Id == input.ApplyId).FirstOrDefaultAsync();

                if (purchData.IsMerge)
                    throw new Exception("该采购单已经合并");
                purchData.IsMerge = true;
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                var mergeData = await GroupMergePurchaseStore.Entities.Where(t => t.MainPurchaseId == input.MergePurchaseId && t.CenterPurchaseId == input.ApplyId).FirstOrDefaultAsync();
                if (mergeData == null)
                {
                    mergeData = new GroupMergePurchase()
                    {
                        CenterPurchaseId = input.ApplyId,
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Id = Guid.NewGuid().tostring32(),
                        MainPurchaseId = input.MergePurchaseId,
                        Modifier = userId,
                        ModifierDate = DateTime.Now,
                        Remark = $"{DateTime.Now.ToShortDateString()}新并入",
                    };

                    GroupMergePurchaseStore.Insert(mergeData);
                }
                PurchaseRequestStore.Update(purchData);
                _unitOfWork.SaveChanges();


                return flag;
            });
        }


        #endregion



        #region 2022年3月 医保目录改版后新查询接口



        #endregion




        #region 2022年5月  新增审批权限配置  ApprovalConfig

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>

        public Task<bool> AddApprovalConfig(ApprovalConfig[] approval)
        {
            return Task.Run(async () =>
            {



                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                if (approval == null)
                    throw new Exception("请提供有效参数");
                //if (string.IsNullOrWhiteSpace(approval.UserId))
                //    throw new Exception("用户ID非空");
                //if (approval.MedicalItemType < 1)
                //    throw new Exception("请提供物品类型 ");
                //if (approval.PurchSubmitLevel < 1)
                //    throw new Exception("审批等级必须大于1 ");
                //var data = await ApprovalConfigStore.GetFirstOrDefaultAsync(t => t.UserId == approval.UserId && t.DataState == 1);
                //if (data != null)
                //    throw new Exception("当前审批流程已存在该用户");
                var orderData = approval.OrderBy(t => t.PurchSubmitLevel).ToList();
                var oldData = await GetApprovalConfigAsync(approval[0].ApprovalType);
                foreach (var item in oldData)
                {
                    await DelApprovalConfig(item.Id);
                }
                int Livel = 1;
                ApprovalConfig tempdata = new ApprovalConfig();

                foreach (var item in orderData)
                {
                    if (item.PurchSubmitLevel > tempdata.PurchSubmitLevel)
                    {
                        Livel++;
                        tempdata = item;
                    }
                    ApprovalConfig config = new ApprovalConfig();
                    string userName = await _getUserInfo.GetCurrentUserEmpNameAsync(item.UserId);
                    config.Id = Guid.NewGuid().tostring32();
                    config.ModifierDate = DateTime.Now;
                    config.FounderDate = DateTime.Now;
                    config.Founder = userId;
                    config.Modifier = userId;
                    config.DataState = 1;
                    config.UserName = userName;
                    config.PurchSubmitLevel = Livel;
                    if (item.ApprovalType == 2)
                        config.PurchSubmitLevel = Livel - 1;
                    config.ApprovalType = item.ApprovalType;
                    config.Describe = item.Describe;
                    config.MaxAmount = item.MaxAmount;

                    config.MedicalItemType = item.MedicalItemType;
                    config.MinAmount = item.MinAmount;
                    config.UserId = item.UserId;
                    config.UserRemark = item.UserRemark;
                    ApprovalConfigStore.Insert(config);

                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>

        public Task<bool> UpdateApprovalConfig(ApprovalConfig approval)
        {
            return Task.Run(async () =>
            {
                string userName = await _getUserInfo.GetCurrentUserEmpNameAsync(approval.UserId);
                //var data = await ApprovalConfigStore.GetFirstOrDefaultAsync(t => t.Id == PurchaseRequestId);
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                if (approval == null)
                    throw new Exception("请提供有效参数");
                if (string.IsNullOrWhiteSpace(approval.UserId))
                    throw new Exception("用户ID非空");

                if (approval.PurchSubmitLevel < 1)
                    throw new Exception("审批等级必须大于1 ");
                var data = await ApprovalConfigStore.GetFirstOrDefaultAsync(t => t.Id == approval.Id);

                data.ModifierDate = DateTime.Now;
                data.Modifier = userId;
                data.UserId = approval.UserId;
                data.ApprovalType = approval.ApprovalType;
                data.Describe = approval.Describe;
                data.MaxAmount = approval.MaxAmount;
                data.MinAmount = approval.MinAmount;
                data.MedicalItemType = approval.MedicalItemType;
                data.PurchSubmitLevel = approval.PurchSubmitLevel;
                data.UserName = userName;
                data.UserRemark = approval.UserRemark;
                ApprovalConfigStore.Update(data);
                await _unitOfWork.SaveChangesAsync();
                return true;
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        public Task<bool> DelApprovalConfig(string Id)
        {
            return Task.Run(async () =>
            {


                var userId = await _getUserInfo.GetCurrentUserIdAsync();

                var data = await ApprovalConfigStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                data.DataState = 3;
                data.Modifier = userId;
                data.ModifierDate = DateTime.Now;
                ApprovalConfigStore.Update(data);
                await _unitOfWork.SaveChangesAsync();
                return true;
            });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="ApprovalType">1 申请单 2订单</param>
        /// <returns></returns>

        public Task<ApprovalConfig[]> GetApprovalConfigAsync(int ApprovalType, string UserId = "")
        {
            return Task.Run(async () =>
            {
                if (UserId == "")
                {
                    var data = await ApprovalConfigStore.Entities.Where(t => t.DataState == 1 && t.ApprovalType == ApprovalType).OrderBy(t => t.PurchSubmitLevel).ToListAsync();

                    return data.ToArray();
                }
                else
                {
                    var data = await ApprovalConfigStore.Entities.Where(t => t.DataState == 1 && t.ApprovalType == ApprovalType && t.UserId == UserId).OrderBy(t => t.PurchSubmitLevel).ToListAsync();

                    return data.ToArray();

                }
            });
        }

        /// <summary>
        ///预览
        /// </summary>
        /// <param name="ApprovalType">1 申请单 2订单</param>
        /// <returns></returns>

        public Task<ApprovalProcessOutPut[]> GetApprovalConfigPreviewAsync(int ApprovalType)
        {
            return Task.Run(async () =>
            {
                var data = await ApprovalConfigStore.Entities.Where(t => t.DataState == 1 && t.ApprovalType == ApprovalType).OrderBy(t => t.PurchSubmitLevel).ToListAsync();
                List<ApprovalProcessOutPut> list = new List<ApprovalProcessOutPut>();
                foreach (var item in data)
                {
                    list.Add(new ApprovalProcessOutPut()
                    {
                        ApprovalName = "",
                        content = $"等待{item.UserRemark}:{item.UserName}审核",
                        current = 0,
                        title = "待进行",
                    });
                }
                return list.ToArray();
            });
        }



        #endregion




        #region 2022年12月08日13:52:17  报废出库申请审批

        //获取报废申请列表

        public Task<PageData<MaterialApplyApproveOutPut[]>> GetMapproveAsync(ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                //var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                var Approvals = await GetApprovalConfigAsync(3, user.Id);
                var Approval = Approvals.FirstOrDefault();

                if (!input.beginTime.HasValue)
                    input.beginTime = DateTime.Now.AddYears(-1);
                if (!input.endTime.HasValue)
                    input.endTime = DateTime.Now.AddDays(+1);
                for (int i = 1; i < Approvals.Length; i++)
                {
                    Approval.MedicalItemType += "," + Approvals[i].MedicalItemType;
                }
                if (input.beginTime < Convert.ToDateTime("2022-12-30"))
                {
                    input.beginTime = Convert.ToDateTime("2022-12-30");
                }
                int count = 0;
                var sqlParameterList = new List<SqlParameter>();
                string sqlwhere = $@" where rma.AuditConditionId = '9000bced463f42fbadef5abc554a5145' And rma.OutboundType = '01f769807f9a4736be41db800606447e' And rma.AuditDate>'{input.beginTime}' And rma.AuditDate<'{input.endTime.Value.AddDays(1)}' ";
                if (input.AuditStatus + "" != "" && input.AuditStatus + "" != "0")
                {
                    if (input.AuditStatus == "2")
                    {
                        sqlwhere += $"and (GroupAuditStatus = '{input.AuditStatus}'  or GroupAuditStatus IS NULL)";
                    }
                    else
                        sqlwhere += $"and GroupAuditStatus = '{input.AuditStatus}'";
                }

                if (Approval != null && Approval.PurchSubmitLevel != 1)
                    sqlwhere += $"and ( AuditorLevel >= {Approval.PurchSubmitLevel - 1} OR GroupAuditStatus = '4' OR GroupAuditStatus = '3' )";
                if (input.CenterId + "" != "0" && input.CenterId + "" != "")
                    sqlwhere += $"and rma.CenterId = '{input.CenterId}'";

                if (input.Id + "" != "")
                    sqlwhere += $"and rma.Id = '{input.Id}'";

                if (input.Remarks + "" != "")
                    sqlwhere += $"and rma.PurchaseNo like '%{input.Remarks}%'";

                if (Approval != null && !string.IsNullOrWhiteSpace(Approval.MedicalItemType))
                {
                    List<string> cat = new List<string>();
                    var mtype = Approval.MedicalItemType.Split(',');
                    foreach (var item in mtype)
                    {

                        switch (item)
                        {
                            case "1":
                                cat.Add("dc8169e93f3b4ae4a29ec51e945be1e7");
                                cat.Add("f4be23d39b0d44998b93a5a277d6c84e");
                                break;
                            case "2":
                                cat.Add("3ee1747b95dc45609049c7988e6989fc");
                                cat.Add("f378ef013f4b4df8ad5be1a5fd2abb3e");

                                break;
                            case "3":
                                cat.Add("2543420c3132403098b85cad056c577e");
                                cat.Add("5fe8074b1e044520b9bdc680ebd978ba");

                                break;
                            case "4":
                                cat.Add("7747bc35fa0d43cdba559a990701841b");

                                break;
                            default:
                                break;
                        }
                    }

                    if (cat.Count > 0)
                    {
                        sqlwhere += $"and rma.WarehouseId  in  (";
                        foreach (var item in cat)
                        {
                            sqlwhere += $" '{item}',";
                        }
                        sqlwhere = sqlwhere.Remove(sqlwhere.Length - 1, 1);
                        sqlwhere += ")";
                    }

                }
                string QueryCountSQL = $"select count(*) as count    FROM dbo.MaterialApplyApprove as rma  {sqlwhere}";

                string QuerySQL = $@" SELECT emps.Name as AuditorName  ,rma.*,ware.Name As WarehouseName, cd.ShortName as CenterName, ey.Name as GroupApprovalName 
      FROM dbo.MaterialApplyApprove as rma
      LEFT JOIN dbo.WareHouseCatalog AS  ware ON ware.Id = rma.WarehouseId   
      LEFT JOIN dbo.Employees AS emps ON emps.Id = rma.Auditor 
 left join CenterDialysiss as cd on cd.Id = rma.CenterId
 left join Users as ep on ep.Id = rma.GroupAuditor
	  left join Employees as ey on ey.Id = ep.EmployeeId
	   {sqlwhere}
      order by   rma.AuditDate desc
	  offset {((input.PageNum - 1) * input.PageSize)} rows
      fetch next {input.PageSize} rows only;";
                var dtcount = MsSqlHelper.GetSingleObj().GetDataTable(QueryCountSQL, sqlParameterList.ToArray());
                count = GetTupleByList<PageCount>(dtcount).Item2.First().count;

                var matTb = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());
                var matData = GetTupleByList<MaterialApplyApproveOutPut>(matTb).Item2;
                foreach (var item in matData)
                {
                    if (Approval != null && Approval.PurchSubmitLevel >= 1)
                    {
                        if (item.GroupAuditStatus == "6" && (Approval.PurchSubmitLevel - 1 == item.AuditorLevel && item.TotalCost > Approval.MinAmount))
                            item.GroupAuditStatus = "2";
                    }

                }
                return new PageData<MaterialApplyApproveOutPut[]>(matData.ToArray(), count);

            });
        }


        //获取明细

        public Task<mapDetailOutPut> GetmappDetailsAsync(string AppId)
        {

            return Task.Run(async () =>
            {
                mapDetailOutPut detailOutPut = new mapDetailOutPut();

                var MainData = await GetMapproveAsync(new ReturnQuerInput() { Id = AppId, beginTime = DateTime.Now.AddYears(-10), endTime = DateTime.Now, PageNum = 1, PageSize = 10, CenterId = "0" });
                detailOutPut.approveOutPut = MainData.Result.First();

                var sqlParameterList = new List<SqlParameter>();
                string QuerySQL = $@"  select mat.*,med.MedicalItemName , med.Packaging,med.Specifications ,mu.SpeUnitCHS  , med.Manufacturer  ,sp.Name as SuppName from MaterialApplyApproveDetail  as mat 
  left join  MedicalItemRecords as med on mat.MaterialId = med.Id
  left join  MedicalUnits mu on mat.MaterialUnit = mu.Id
 left join Suppliers sp on sp.Id = mat.SupplierName
  where mat.MAApplyId = '{AppId}' and mat.DataState = 1;";
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());
                var DeData = GetTupleByList<MaterialApplyApproveDetailOutPut>(dt).Item2;
                foreach (var item in DeData)
                {
                    item.CostTotalPrice = item.UnitPrice * item.MaterialQuantity;
                }
                DeData.Add(new MaterialApplyApproveDetailOutPut() { MedicalItemName = "合计", CostTotalPrice = DeData.Sum(t => t.CostTotalPrice), MaterialQuantity = DeData.Sum(t => t.MaterialQuantity) });
                detailOutPut.MaterialApplyApproveDetailOutPuts = DeData.ToArray();

                detailOutPut.approveOutPut.TotalCost = DeData.Sum(t => t.UnitPrice * t.MaterialQuantity);


                detailOutPut.orderApproveOutPuts = detailOutPut.approveOutPut.GroupAuditStatus == "4" || detailOutPut.approveOutPut.GroupAuditStatus == "3" ? new OrderApproveOutPut[0] : getmappProcessOutPuts(detailOutPut.approveOutPut).Result.ToArray();
                //审批流程getmappProcessOutPuts


                return detailOutPut;
            });
        }

        //审核流程
        private Task<List<OrderApproveOutPut>> getmappProcessOutPuts(MaterialApplyApproveOutPut purchaseOrder)
        {
            return Task.Run(async () =>
            {
                var Mtype = 1;
                switch (purchaseOrder.WarehouseId)
                {
                    case "f4be23d39b0d44998b93a5a277d6c84e": //药房
                        Mtype = 1;
                        break;

                    case "f378ef013f4b4df8ad5be1a5fd2abb3e":
                        Mtype = 2;
                        break;

                    case "dc8169e93f3b4ae4a29ec51e945be1e7":
                        Mtype = 1;
                        break;

                    case "7747bc35fa0d43cdba559a990701841b":
                        Mtype = 4;
                        break;

                    case "3ee1747b95dc45609049c7988e6989fc":
                        Mtype = 2;
                        break;

                    case "2543420c3132403098b85cad056c577e":
                        Mtype = 3;
                        break;
                    case "5fe8074b1e044520b9bdc680ebd978ba":
                        Mtype = 4;
                        break;
                    default:
                        break;
                }
                // await UpdatePu("");
                List<OrderApproveOutPut> approvalProcessOutPuts = new List<OrderApproveOutPut>();
                var ALLdata = await ApprovalConfigStore.Entities.Where(t => t.DataState == 1 && t.ApprovalType == 3 && t.MedicalItemType.Contains(Mtype + "")).OrderBy(t => t.PurchSubmitLevel).ToListAsync();
                var a_approvalProcesses = ALLdata.GroupBy(t => t.PurchSubmitLevel);
                var data = await OrderApproveStore.Entities.Include(t => t.user).ThenInclude(t => t.Employee).Where(t => t.OrderId == purchaseOrder.Id && t.DataState == 1).OrderBy(t => t.AuditDate).ToListAsync();

                int level = ALLdata.Where(t => t.MinAmount <= purchaseOrder.TotalCost && t.MaxAmount > purchaseOrder.TotalCost && t.MedicalItemType.Contains(Mtype + "")).Last().PurchSubmitLevel;
                int Maxlevel = 0;
                int current = 0;
                string state = "";
                List<OrderApprove> NewData = new List<OrderApprove>();
                if (data != null && data.Count > 0)
                {
                    NewData = data.Where(t => t.AuditStatus != 5).CompareDistinct(t => t.AuditorLevel).ToList();
                    if (data.Where(t => t.AuditStatus == 5).ToList() != null)
                    {
                        NewData.AddRange(data.Where(t => t.AuditStatus == 5).ToList());
                    }
                    data = NewData.OrderBy(t => t.AuditDate).ToList();
                    Maxlevel = data.Max(t => t.AuditorLevel);

                    foreach (var item in data)
                    {
                        state = item.AuditStatus == 3 ? "同意" : item.AuditStatus == 4 ? "拒绝" : "回退";
                        if (state == "回退" || state == "拒绝")
                            state += $"，备注：{item.Advice}";
                        //string content = "";
                        //if (a_approvalProcesses.Where(t => t.level == item.AuditorLevel).First().Role == "0b063b50906445719da07631a92aa791")
                        //    content = "";
                        //else

                        approvalProcessOutPuts.Add(new OrderApproveOutPut()
                        {
                            ApprovalName = "",
                            content = $"({item.user.Employee.Name})审批，审核结果： {state}",
                            title = item.AuditStatus == 4 ? "已拒绝" : "已完成",
                            Time = item.AuditDate.Value.ToString("yyyy-MM-dd"),
                        });
                        current++;
                    }
                    if (data.Last().AuditStatus == 5)
                        Maxlevel = 0;
                    else
                        Maxlevel = data.Last().AuditorLevel;
                    if (data.Last().AuditStatus != 4)
                    {

                        current++;
                        foreach (var item in a_approvalProcesses)
                        {
                            var temp = item.FirstOrDefault();
                            if ((Maxlevel < level && temp.PurchSubmitLevel > Maxlevel && temp.PurchSubmitLevel <= level) && (purchaseOrder.GroupAuditStatus == "6" || purchaseOrder.GroupAuditStatus == "2"))
                                approvalProcessOutPuts.Add(new OrderApproveOutPut()
                                {
                                    ApprovalName = "",
                                    content = "等待" + temp.UserRemark + "（"+temp.UserName+"）审批",
                                    title = "待进行",
                                });
                        }
                    }
                }
                else
                {
                    foreach (var item in a_approvalProcesses)
                    {
                        var temp = item.FirstOrDefault();

                        if (Maxlevel < level && temp.PurchSubmitLevel <= level)
                            approvalProcessOutPuts.Add(new OrderApproveOutPut()
                            {
                                ApprovalName = "",
                                content = "等待" + temp.UserRemark + "（" + temp.UserName + "）审批",
                                title = "待进行",
                                current = 0,
                            });
                    }
                }

                approvalProcessOutPuts.ForEach(t => t.current = current);
                return approvalProcessOutPuts;
            });
        }


        //审核

        //报废审批
        public Task<bool> MaterialApplyRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //
                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var data = await MaterialApplyApproveStore.Entities.Include(t => t.materialApplyApproveDetails).Where(t => t.Id == Input.Id).FirstOrDefaultAsync();

                    if (data == null)
                        return true;
                    //PurchaseApproveStore
                    //var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    //var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                    data.TotalCost = data.materialApplyApproveDetails.Sum(t => t.UnitPrice * t.MaterialQuantity);

                    var Approvals = await GetApprovalConfigAsync(3);
                    var maxLev = Approvals.Max(t => t.PurchSubmitLevel);
                    int? Mtype = 1;
                    switch (data.WarehouseId)
                    {
                        case "f4be23d39b0d44998b93a5a277d6c84e": //药房
                            Mtype = 1;
                            break;

                        case "f378ef013f4b4df8ad5be1a5fd2abb3e":
                            Mtype = 2;
                            break;

                        case "dc8169e93f3b4ae4a29ec51e945be1e7":
                            Mtype = 1;
                            break;

                        case "7747bc35fa0d43cdba559a990701841b":
                            Mtype = 4;
                            break;

                        case "3ee1747b95dc45609049c7988e6989fc":
                            Mtype = 2;
                            break;

                        case "2543420c3132403098b85cad056c577e":
                            Mtype = 3;
                            break;
                        case "5fe8074b1e044520b9bdc680ebd978ba":
                            Mtype = 4;
                            break;
                        default:
                            break;
                    }

                    var Approval = Approvals.Where(t => t.UserId == userId && t.MedicalItemType.Contains(Mtype + "")).FirstOrDefault();
                    if (Approval == null)
                        throw new Exception("您暂时没有审核采购单的权限");

                    if (Approval.PurchSubmitLevel < data.AuditorLevel)
                        throw new Exception("您暂时没有审核采购单的权限");

                    if (data.AuditorLevel < 1 && Mtype == 3)
                    {
                        throw new Exception("当前未估值净价，无法进行报废审批");
                    }
                    OrderApprove purchaseApprove = new OrderApprove()
                    {
                        Advice = Input.GroupAdvice,
                        OrderId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditorLevel = Approval.PurchSubmitLevel,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        ModifierDate = DateTime.Now

                    };

                    data.GroupAuditStatus = Input.GroupAuditStatus != "4" ? "6" : "4";
                    data.DataState = 1;
                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.AuditorLevel = Approval.PurchSubmitLevel;
                    bool cresrult = true;
                    var ApproveDatas = await OrderApproveStore.Entities.Include(t => t.user).ThenInclude(t => t.Employee).Where(t => t.OrderId == data.Id).OrderBy(t => t.AuditDate).ToListAsync();

                    List<GroupAuditorList> auditorLists = new List<GroupAuditorList>();

                    foreach (var item in ApproveDatas)
                    {
                        auditorLists.Add(new GroupAuditorList() { GroupAdvice = item.Advice, GroupAuditDate = item.AuditDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), GroupAuditor = item.user.Employee.Name });
                    }
                    auditorLists.Add(new GroupAuditorList() { GroupAdvice = Input.GroupAdvice, GroupAuditDate = purchaseApprove.AuditDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), GroupAuditor = name });

                    bool isUpdate = false;
                    if (Mtype != 3)
                    {
                        isUpdate = ((data.TotalCost < Approval.MaxAmount && data.TotalCost >= Approval.MinAmount) || Input.GroupAuditStatus == "4") && data.AuditorLevel >= 3;
                    }
                    else
                    {
                        isUpdate = ((data.AppraiseCost < Approval.MaxAmount && data.AppraiseCost >= Approval.MinAmount) || Input.GroupAuditStatus == "4") && data.AuditorLevel >= 3;
                    }

                    if (isUpdate)
                    {
                        data.GroupAuditStatus = Input.GroupAuditStatus;


                        //对接中心端审核状态 
                        //if (data.GroupAuditStatus == "4") //拒绝 ,直接对接中心端， 如果同意，继续根据权限审批，药品耗材走医保定价
                        //{


                        //}

                        cresrult = await _dataReceive.GroupUpdateMatState(data.CenterId, new
                        {
                            GroupAuditStatus = data.GroupAuditStatus,
                            GroupAuditDate = data.GroupAuditDate,
                            GroupAdvice = data.GroupAdvice,
                            GroupAuditor = name,
                            GroupAuditorName = JsonHelper.ObjectToJson(auditorLists),
                            Id = data.Id, //采购单ID
                        });

                    }
                    if (data.GroupAuditStatus == "4")
                    {

                        cresrult = await _dataReceive.GroupUpdateMatState(data.CenterId, new
                        {
                            GroupAuditStatus = data.GroupAuditStatus,
                            GroupAuditDate = data.GroupAuditDate,
                            GroupAdvice = data.GroupAdvice,
                            GroupAuditorName = JsonHelper.ObjectToJson(auditorLists),
                            GroupAuditor = name,

                            Id = data.Id, //采购单ID
                        });
                    }

                    if (cresrult)
                    {
                        MaterialApplyApproveStore.Update(data);
                        OrderApproveStore.Insert(purchaseApprove);

                        _unitOfWork.SaveChanges();
                        string PendingApprovalValue = "审批完成";
                        var MainData = await GetMapproveAsync(new ReturnQuerInput() { Id = data.Id, beginTime = DateTime.Now.AddYears(-10), endTime = DateTime.Now, PageNum = 1, PageSize = 10, CenterId = "0" });
                        if (data.GroupAuditStatus == "6")
                        {


                            var UpName = Approvals.Where(t => t.PurchSubmitLevel == data.AuditorLevel + 1 && t.MedicalItemType.Contains(Mtype + "")).FirstOrDefault().UserName;
                            //else
                            //{
                            //    UpName = Approvals.Where(t => t.PurchSubmitLevel == 3 && t.MedicalItemType.Contains(Mtype + "") && t.MinAmount>data.TotalCost && t.MaxAmount <= data.TotalCost).FirstOrDefault().UserName;
                            //}
                            PendingApprovalValue = $"{name}已审核，等待{UpName}审批";


                        }
                        var mappProcess = await getmappProcessOutPuts(MainData.Result.FirstOrDefault());
                        await _dataReceive.MaterialPendingApproval(data.CenterId, new
                        {
                            PendingApproval = PendingApprovalValue,
                            GroupAuditorName = JsonHelper.ObjectToJson(mappProcess),
                            Id = Input.Id
                        });
                    }
                    else throw new Exception("中心服务连接异常，对接中心审批结果失败，请稍后再试！");



                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        public Task<PageData<MaterialApplyApproveOutPut[]>> CollectGetMaterialApplyApproveAsync(ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                //bool IsDel = false;
                //PurchaseRequestOutPut[] result = null;
                //int count = 0;
                try
                {
                    //重新获取数据
                    if (input == null)
                        input = new ReturnQuerInput() { PageSize = 10, PageNum = 1, CenterId = "0" };
                    if (input.CenterId + "" == "")
                        input.CenterId = "0";
                    if (input.CenterId == "0")
                        throw new Exception("请选择指定的透析中心进行刷新");
                    await _purchasManagerServices.SyncMaterialApplyApproveData(input.CenterId);
                    await _purchasManagerServices.SyncMaterialApplyApproveDetailData(input.CenterId);
                    var data = await GetMapproveAsync(input);

                    return data;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }



        public Task<bool> ReturnMapproveAsync(string MapId)
        {
            return Task.Run(async () =>
            {

                try
                {
                    var data = await MaterialApplyApproveStore.Entities.Where(t => t.Id == MapId).FirstOrDefaultAsync();

                    if (data != null)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete OrderApproves where OrderId='{MapId}'");
                        data.AuditorLevel = 0;
                        data.Auditor = null;
                        data.AuditDate = null;
                        data.AuditConditionId = "";
                        data.Advice = null;
                        data.GroupAdvice = null;
                        data.GroupAuditDate = null;
                        data.GroupAuditor = null;
                        data.GroupAuditStatus = null;
                        MaterialApplyApproveStore.Update(data);

                        await _unitOfWork.SaveChangesAsync();
                    }



                    return true;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }




        //固定资产报废估值

        public Task<bool> FixedAssetsScrapValueAsync(FixedAssetsScrapInput input)
        {
            return Task.Run(async () =>
            {

                try
                {
                    var data = await MaterialApplyApproveDetailStore.Entities.Include(t => t.materialApply).Where(t => t.Id == input.Id).FirstOrDefaultAsync();

                    if (data != null)
                    {
                        var MainData = await MaterialApplyApproveStore.Entities.Include(t => t.materialApplyApproveDetails).Where(t => t.Id == data.MAApplyId).FirstOrDefaultAsync();

                        data.Appraise = input.Appraise;
                        MaterialApplyApproveDetailStore.Update(data);
                        MainData.AppraiseCost = MainData.materialApplyApproveDetails.Sum(t => t.Appraise);
                        MaterialApplyApproveStore.Update(MainData);
                        await _unitOfWork.SaveChangesAsync();
                    }



                    return true;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }


        //确定估值
        public Task<bool> AscertainFixedAssetsScrapAsync(FixedAssetsScrapInput input)
        {
            return Task.Run(async () =>
            {

                try
                {
                    var data = await MaterialApplyApproveStore.Entities.Include(t => t.materialApplyApproveDetails).Where(t => t.Id == input.Id).FirstOrDefaultAsync();

                    if (data != null)
                    {
                        data.AppraiseCost = data.materialApplyApproveDetails.Sum(t => t.Appraise);
                        data.AuditorLevel = 1;
                        MaterialApplyApproveStore.Update(data);

                        await _unitOfWork.SaveChangesAsync();
                    }



                    return true;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }


        #endregion



        #region  滞销

        public Task<PageData<MMaterialsWarningApplyListOutPut[]>> CollectGetMaterialsWarningApplyListAsync(ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                //bool IsDel = false;
                //PurchaseRequestOutPut[] result = null;
                //int count = 0;
                try
                {
                    //重新获取数据
                    if (input == null)
                        input = new ReturnQuerInput() { PageSize = 10, PageNum = 1, CenterId = "0" };
                    if (input.CenterId + "" == "")
                        input.CenterId = "0";
                    if (input.CenterId == "0")
                        throw new Exception("请选择指定的透析中心进行刷新");
                    await _purchasManagerServices.SyncMaterialsWarningApplyListStoreData(input.CenterId);
                    await _purchasManagerServices.SyncMaterialsWarningApplyDetailListData(input.CenterId);
                    var data = await GetMMaterialsWarningApplyListAsync(input);

                    return data;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

            });

        }
        public Task<PageData<MMaterialsWarningApplyListOutPut[]>> GetMMaterialsWarningApplyListAsync(ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                //var RoleData = _approvalProcesses.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                var Approvals = await GetApprovalConfigAsync(3, user.Id);
                var Approval = Approvals.FirstOrDefault();
                for (int i = 1; i < Approvals.Length; i++)
                {

                    Approval.MedicalItemType += "," + Approvals[i].MedicalItemType;
                }

                if (!input.beginTime.HasValue)
                {
                    input.beginTime = DateTime.Now.AddYears(-1);
                }
                if (!input.endTime.HasValue)
                {
                    input.endTime = DateTime.Now.AddDays(1);
                }

                if (input.beginTime < Convert.ToDateTime("2022-12-28"))
                {
                    input.beginTime = Convert.ToDateTime("2022-12-28");
                }
                int count = 0;
                var sqlParameterList = new List<SqlParameter>();
                string sqlwhere = $@" where rma.AuditConditionId = '9000bced463f42fbadef5abc554a5145'   And rma.AuditDate>'{input.beginTime}' And rma.AuditDate<'{input.endTime.Value.AddDays(+1)}' ";
                if (input.AuditStatus + "" != "" && input.AuditStatus + "" != "0" && user.RoleUsers.Where(t => t.RoleId == "7358c1c008524689a1b665736a276483").FirstOrDefault() == null)
                {
                    if (input.AuditStatus == "2")
                    {
                        sqlwhere += $"and (GroupAuditStatus = '{input.AuditStatus}'  or GroupAuditStatus IS NULL)";
                    }

                    if (input.AuditStatus == "0")
                    {
                        //  
                        // sqlwhere += $"and (GroupAuditStatus = '{input.AuditStatus}'  or GroupAuditStatus IS NULL)";
                    }
                    else
                        sqlwhere += $"and GroupAuditStatus = '{input.AuditStatus}'";
                }

                if (Approval != null && Approval.PurchSubmitLevel != 1)
                    sqlwhere += $"and ( AuditorLevel >= {Approval.PurchSubmitLevel - 1} OR GroupAuditStatus = '4' OR GroupAuditStatus = '3')";

                if (input.CenterId + "" != "0" && input.CenterId + "" != "")
                    sqlwhere += $"and rma.CenterId = '{input.CenterId}'";

                if (input.Id + "" != "")
                    sqlwhere += $"and rma.Id = '{input.Id}'";

                string QueryCountSQL = $"select count(*) as count    FROM dbo.MaterialsWarningApplyList as rma  {sqlwhere}";

                string QuerySQL = $@" SELECT emps.Name as AuditorName  ,rma.*,ware.Name As WarehouseName, cd.ShortName as CenterName, ey.Name as GroupApprovalName 
      FROM dbo.MaterialsWarningApplyList as rma
      LEFT JOIN dbo.WareHouseCatalog AS  ware ON ware.Id = rma.WarehouseId   
      LEFT JOIN dbo.Employees AS emps ON emps.Id = rma.Auditor 
 left join CenterDialysiss as cd on cd.Id = rma.CenterId
 left join Users as ep on ep.Id = rma.GroupAuditor
	  left join Employees as ey on ey.Id = ep.EmployeeId
	   {sqlwhere}
      order by   rma.AuditDate desc
	  offset {((input.PageNum - 1) * input.PageSize)} rows
      fetch next {input.PageSize} rows only;";
                var dtcount = MsSqlHelper.GetSingleObj().GetDataTable(QueryCountSQL, sqlParameterList.ToArray());
                count = GetTupleByList<PageCount>(dtcount).Item2.First().count;

                var matTb = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());
                var matData = GetTupleByList<MMaterialsWarningApplyListOutPut>(matTb).Item2;

                foreach (var item in matData)
                {
                    if (Approval != null && Approval.PurchSubmitLevel >= 1)
                    {
                        if (item.GroupAuditStatus == "6" && Approval.PurchSubmitLevel - 1 == item.AuditorLevel)
                            item.GroupAuditStatus = "2";

                    }
                }
                return new PageData<MMaterialsWarningApplyListOutPut[]>(matData.ToArray(), count);

            });
        }



        public Task<MaterialsWarningOutPut> GetMaterialsWarningDetailsAsync(string AppId)
        {

            return Task.Run(async () =>
            {
                MaterialsWarningOutPut detailOutPut = new MaterialsWarningOutPut();

                var MainData = await GetMMaterialsWarningApplyListAsync(new ReturnQuerInput() { Id = AppId, beginTime = DateTime.Now.AddYears(-10), AuditStatus = "0", endTime = DateTime.Now, PageNum = 1, PageSize = 10, CenterId = "0" });
                detailOutPut.applyListOutPut = MainData.Result.First();

                var sqlParameterList = new List<SqlParameter>();
                string QuerySQL = $@"    select mat.*, (mat.InPrice*mat.Amount) as SumPrice ,med.MedicalItemName , med.Packaging  , med.Manufacturer    from MaterialsWarningApplyDetailList  as mat 
  left join  MedicalItemRecords as med on mat.MedicalItemId = med.Id 
  where mat.MaterialsWarningApplyListId = '{AppId}'  and mat.DataState = 1;";
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());
                var DeData = GetTupleByList<MaterialsWarningApplyDetailOutPut>(dt).Item2;

                DeData.Add(new MaterialsWarningApplyDetailOutPut() { MedicalItemName = "合计", SumPrice = DeData.Sum(t => t.SumPrice), Amount = DeData.Sum(t => t.Amount) });
                detailOutPut.detailOutPuts = DeData.ToArray();




                //   detailOutPut.orderApproveOutPuts = detailOutPut.approveOutPut.GroupAuditStatus == "2" || detailOutPut.approveOutPut.GroupAuditStatus == "3" ? new OrderApproveOutPut[0] : getmappProcessOutPuts(detailOutPut.approveOutPut).Result.ToArray();
                //审批流程getmappProcessOutPuts
                //  var pro = await getmappProcessOutPuts(new MaterialApplyApproveOutPut() { GroupAuditStatus = detailOutPut.applyListOutPut.GroupAuditStatus, TotalCost = DeData.Sum(t => t.SumPrice), Id = detailOutPut.applyListOutPut.Id });

                detailOutPut.orderApproveOutPuts = detailOutPut.applyListOutPut.GroupAuditStatus == "3" ? new OrderApproveOutPut[0] : getmappProcessOutPuts(new MaterialApplyApproveOutPut() { Id = detailOutPut.applyListOutPut.Id, WarehouseId = detailOutPut.applyListOutPut.WarehouseId, TotalCost = DeData.Where(t => t.MedicalItemName == "合计").First().SumPrice, GroupAuditStatus = detailOutPut.applyListOutPut.GroupAuditStatus }).Result.ToArray();

                //  detailOutPut.orderApproveOutPuts = new OrderApproveOutPut[0];//pro.ToArray();
                return detailOutPut;
            });
        }

        //审核

        public Task<bool> MaterialsWarningRequestAsync(ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                try
                {

                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();

                    var data = await MaterialsWarningApplyListStore.Entities.Include(t => t.applyDetailLists).Where(t => t.Id == Input.Id).FirstOrDefaultAsync();



                    if (data == null)
                        return true;

                    var Approvals = await GetApprovalConfigAsync(3);
                    var maxLev = 3; //Approvals.Max(t => t.PurchSubmitLevel);
                    int? Mtype = 1;
                    switch (data.WarehouseId)
                    {
                        case "f4be23d39b0d44998b93a5a277d6c84e": //药房
                            Mtype = 1;
                            break;

                        case "f378ef013f4b4df8ad5be1a5fd2abb3e":
                            Mtype = 2;
                            break;

                        case "dc8169e93f3b4ae4a29ec51e945be1e7":
                            Mtype = 1;
                            break;

                        case "7747bc35fa0d43cdba559a990701841b":
                            Mtype = 4;
                            break;

                        case "3ee1747b95dc45609049c7988e6989fc":
                            Mtype = 2;
                            break;

                        case "2543420c3132403098b85cad056c577e":
                            Mtype = 3;
                            break;
                        case "5fe8074b1e044520b9bdc680ebd978ba":
                            Mtype = 4;
                            break;
                        default:
                            break;
                    }

                    var Approval = Approvals.Where(t => t.UserId == userId && t.MedicalItemType.Contains(Mtype + "")).FirstOrDefault();
                    if (Approval == null)
                        throw new Exception("您暂时没有审核采购单的权限");

                    if (Approval.PurchSubmitLevel <= data.AuditorLevel)
                        throw new Exception("您暂时没有审核采购单的权限");
                    OrderApprove purchaseApprove = new OrderApprove()
                    {
                        Advice = Input.GroupAdvice,
                        OrderId = data.Id,
                        Id = Guid.NewGuid().tostring32(),
                        AuditDate = DateTime.Now,
                        Auditor = userId,
                        AuditorLevel = Approval.PurchSubmitLevel,
                        AuditStatus = Convert.ToInt32(Input.GroupAuditStatus),
                        DataState = 1,
                        Founder = userId,
                        FounderDate = DateTime.Now,
                        Modifier = userId,
                        ModifierDate = DateTime.Now

                    };


                    OrderApproveStore.Insert(purchaseApprove);
                    _unitOfWork.SaveChanges();


                    data.GroupAuditStatus = Input.GroupAuditStatus != "4" ? "6" : "4";

                    data.GroupAdvice = Input.GroupAdvice;
                    data.GroupAuditDate = DateTime.Now;
                    data.GroupAuditor = userId;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.AuditorLevel = Approval.PurchSubmitLevel;
                    bool cresrult = true;

                    if ((Input.GroupAuditStatus == "4"))
                    {
                        data.GroupAuditStatus = Input.GroupAuditStatus;

                        //对接中心端审核状态 
                        cresrult = await _dataReceive.GroupUpdateMaterialsWarningState(data.CenterId, new
                        {
                            GroupAuditStatus = data.GroupAuditStatus,
                            GroupAuditDate = data.GroupAuditDate,
                            GroupAdvice = data.GroupAdvice,
                            GroupAuditor = name,
                            Id = data.Id, //采购单ID
                        });

                    }

                    if (data.AuditorLevel == maxLev)
                    {
                        data.GroupAuditStatus = Input.GroupAuditStatus;
                        //对接中心端审核状态 
                        cresrult = await _dataReceive.GroupUpdateMaterialsWarningState(data.CenterId, new
                        {
                            GroupAuditStatus = data.GroupAuditStatus,
                            GroupAuditDate = data.GroupAuditDate,
                            GroupAdvice = data.GroupAdvice,
                            GroupAuditor = name,
                            Id = data.Id, //采购单ID
                        });
                    }
                    if (cresrult)
                    {
                        MaterialsWarningApplyListStore.Update(data);
                        _unitOfWork.SaveChanges();
                        string PendingApprovalValue = "审批完成";
                        if (data.GroupAuditStatus == "6")
                        {
                            var UpName = Approvals.Where(t => t.PurchSubmitLevel == data.AuditorLevel + 1 && t.MedicalItemType.Contains(Mtype + "")).FirstOrDefault().UserName;
                            PendingApprovalValue = $"{name}已审核，等待{UpName}审批";
                        }
                        await _dataReceive.WarningPendingApproval(data.CenterId, new
                        {
                            PendingApproval = PendingApprovalValue,
                            Id = Input.Id
                        });
                    }
                    else throw new Exception("中心服务连接异常，对接中心审批结果失败，请稍后再试！");

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }




        #endregion



        /// <summary>
        /// 更新物品价格，修复历史价格就没停的掉。导致同样的档案数据出现了多条
        /// </summary>
        /// <returns></returns>
        public Task<bool> UpdateMedicalDrugExtensions()
        {

            return Task.Run(() =>
            {
                bool Flag = true;
                try
                {

                    List<string> ListSql = new List<string>();
                    string QuerySql = @"SELECT a.* FROM MedicalDrugExtensions a,
(
SELECT  MedicalId,CenterId FROM MedicalDrugExtensions where IsCurrentUse  =1  
GROUP BY MedicalId,CenterId
HAVING COUNT(1)>1
) AS b
WHERE  a.MedicalId  =b.MedicalId AND a.CenterId=b.CenterId  and a.IsCurrentUse =1  order by  MedicalId";

                    var MdTable = MsSqlHelper.GetSingleObj().GetDataTable(QuerySql);
                    if (MdTable != null && MdTable.Rows.Count > 0)
                    {
                        var md = GetTupleByList<MedicalDrugExtension>(MdTable).Item2;

                        var gopmd = md.GroupBy(t => new { t.CenterId, t.MedicalId });
                        foreach (var item in gopmd)
                        {
                            var temp = item.ToList().OrderBy(t => t.FounderDate).ToList();
                            for (int i = 0; i < temp.Count - 1; i++)
                            {
                                string UpSQL = $"update MedicalDrugExtensions set IsCurrentUse = 0 where id = '{temp[i].Id}'";
                                ListSql.Add(UpSQL);
                            }
                        }

                    }

                    if (ListSql.Count > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecSqlTran(ListSql);
                    }

                    CDGService.WebAPI.Extenstions.LogHelper.WriteCommLog($"档案价格更新完毕.");
                }
                catch (Exception ex)
                {
                    Flag = false;
                    CDGService.WebAPI.Extenstions.LogHelper.WriteCommLog($"档案价格更新错误：{ex + ""}");
                }
                return Flag;

            });
        }

    }

    public class MaterialsReturnDetailModel
    {
        public string Id { get; set; }

        public decimal? SalesReturnQty { get; set; }
        public string Remark { get; set; }
        public string MedicalItemId { get; set; }
    }
}
