using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDGService.Data.Helper;
using System.Data.SqlClient;
using CDGService.Store;
using System.Reflection;
using CDGService.Utils;

namespace CDGService.Application.Service.Net
{
    public class PurchasManagerService : XmlSql, IPurchasManagerService
    {
        private readonly ILogger<PurchasManagerService> _logger;
        private readonly IDataReceive _dataService;
        private static readonly object _lockObj = new object();
        private static readonly object SaveObject = new object();
        private readonly DictionaryCode _dictionaryCode;
        private readonly IUnitOfWork _unitOfWork;
        private readonly WebDataUrl _webUrls;

        #region 
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<SystemDictionary> SystemDictionaryStore => _unitOfWork.GetStore<SystemDictionary>();
        public IRepository<PurchaseRequest> PurchaseRequestStore => _unitOfWork.GetStore<PurchaseRequest>();
        public IRepository<MaterialApplyApprove> MaterialApplyApproveStore => _unitOfWork.GetStore<MaterialApplyApprove>();
        public IRepository<MaterialApplyApproveDetail> MaterialApplyApproveDetailStore => _unitOfWork.GetStore<MaterialApplyApproveDetail>();
        private IRepository<PurchaseDetail> PurchaseDetailStore => _unitOfWork.GetStore<PurchaseDetail>();

        private IRepository<MaterialsWarningApplyList> MaterialsWarningApplyListStore => _unitOfWork.GetStore<MaterialsWarningApplyList>();
        private IRepository<MaterialsWarningApplyDetailList> MaterialsWarningApplyDetailListStore => _unitOfWork.GetStore<MaterialsWarningApplyDetailList>();
        // private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        //  private IRepository<EquipmentInfo> EquipmentInfoStore => _unitOfWork.GetStore<EquipmentInfo>();
        private IRepository<MedicalDrugExtension> MedicalDrugExtensionStore => _unitOfWork.GetStore<MedicalDrugExtension>();
        private IRepository<MedicalItemRecord> MedicalItemRecordStroe => _unitOfWork.GetStore<MedicalItemRecord>();
        private IRepository<Supplier> SupplierStroe => _unitOfWork.GetStore<Supplier>();
        //  private IRepository<BatchNoSecondaryStoreroom> BatchNoSecondaryStore => _unitOfWork.GetStore<BatchNoSecondaryStoreroom>();
        //  private IRepository<BatchNoSecondaryStoreroomDetail> BatchNoSecondarydetailStore => _unitOfWork.GetStore<BatchNoSecondaryStoreroomDetail>();
        //  private IRepository<MaterialOutbound> MaterialOutboundStore => _unitOfWork.GetStore<MaterialOutbound>();
        //  private IRepository<MaterialOutboundDetail> MaterialOutboundDetaillStore => _unitOfWork.GetStore<MaterialOutboundDetail>();
        private IRepository<Prescription> PrescriptionStore => _unitOfWork.GetStore<Prescription>();
        // private IRepository<PrescriptionDetail> PrescriptionDetailStore => _unitOfWork.GetStore<PrescriptionDetail>();
        // private IRepository<PrescriptionDetailRefund> PrescriptionDetailRefundStore => _unitOfWork.GetStore<PrescriptionDetailRefund>();
        private IRepository<PutInStorageBill> PutInStorageBillStore => _unitOfWork.GetStore<PutInStorageBill>();

        //private IRepository<PutInStorageBillDetails> PutInStorageBillDetailsStore => _unitOfWork.GetStore<PutInStorageBillDetails>();
        //private IRepository<PutInStorageBatchNoDetails> PutInStorageBatchNoDetailsStore => _unitOfWork.GetStore<PutInStorageBatchNoDetails>();
        //private IRepository<PutOutFlow> PutOutFlowlStore => _unitOfWork.GetStore<PutOutFlow>();
        private IRepository<ReturnDetails> ReturnDetailsStore => _unitOfWork.GetStore<ReturnDetails>();
        private IRepository<ReturnRequest> ReturnRequestStore => _unitOfWork.GetStore<ReturnRequest>();
        //private IRepository<MaintenanceRecord> MaintenanceRecordStore => _unitOfWork.GetStore<MaintenanceRecord>();
        //private IRepository<BiochemicalTest> BiochemicalTestStore => _unitOfWork.GetStore<BiochemicalTest>();
        //private IRepository<EquipmentKeepRecord> EquipmentKeepRecordStore => _unitOfWork.GetStore<EquipmentKeepRecord>();
        //private IRepository<MedicalHistoryFirstPage> MedicalHistoryFirstPageStore => _unitOfWork.GetStore<MedicalHistoryFirstPage>();
        //private IRepository<FirstPageItemCurrentDiagnosis> FirstPageItemCurrentDiagnosisStore => _unitOfWork.GetStore<FirstPageItemCurrentDiagnosis>();
        //private IRepository<VascularAccessRecord> VascularAccessRecordStore => _unitOfWork.GetStore<VascularAccessRecord>();
        //private IRepository<VascularMaintainRecord> VascularMaintaidRecordStore => _unitOfWork.GetStore<VascularMaintainRecord>();
        //private IRepository<InfectiousDiseaseRegister> InfectiousDiseaseRegisterStore => _unitOfWork.GetStore<InfectiousDiseaseRegister>();
        //private IRepository<AllergyRegister> AllergyRegisterStore => _unitOfWork.GetStore<AllergyRegister>();
        //private IRepository<TumorRegister> TumorRegisterStore => _unitOfWork.GetStore<TumorRegister>();
        //private IRepository<OutpatientMedicalRecord> OutpatientMedicalRecordStore => _unitOfWork.GetStore<OutpatientMedicalRecord>();
        private IRepository<OutpatientDetailsLog> OutpatientLogStore => _unitOfWork.GetStore<OutpatientDetailsLog>();
        //private IRepository<StageSummary> StageSummaryStore => _unitOfWork.GetStore<StageSummary>();
        //private IRepository<DryWeightSet> DryWeightSetStore => _unitOfWork.GetStore<DryWeightSet>();

        //private IRepository<RoutineBloodRecord> RoutineBloodRecordStore => _unitOfWork.GetStore<RoutineBloodRecord>();
        //private IRepository<SevenElectrolyteTermsRecord> SevenElectrolyteTermsRecordStore => _unitOfWork.GetStore<SevenElectrolyteTermsRecord>();
        //private IRepository<MineralAndBoneMetabolismRecord> MineralAndBoneMetabolismRecordStore => _unitOfWork.GetStore<MineralAndBoneMetabolismRecord>();
        //private IRepository<RenalFunctionRecord> RenalFunctionRecordStore => _unitOfWork.GetStore<RenalFunctionRecord>();
        //private IRepository<LiverFunctionBloodFatBoodSugarRecord> LiverFunctionBloodFatBoodSugarRecordStore => _unitOfWork.GetStore<LiverFunctionBloodFatBoodSugarRecord>();
        //private IRepository<InfectiousDiseasesRecord> InfectiousDiseasesRecordStore => _unitOfWork.GetStore<InfectiousDiseasesRecord>();
        //private IRepository<IronParametersRecord> IronParametersRecordStore => _unitOfWork.GetStore<IronParametersRecord>();
        //private IRepository<MyocardialEnzymeFiveRecord> MyocardialEnzymeFiveRecordStore => _unitOfWork.GetStore<MyocardialEnzymeFiveRecord>();
        //private IRepository<HypertensionFourRecord> HypertensionFourRecordStore => _unitOfWork.GetStore<HypertensionFourRecord>();
        //private IRepository<AnemiaThreeRecord> AnemiaThreeRecordStore => _unitOfWork.GetStore<AnemiaThreeRecord>();
        //private IRepository<CoagulationFiveRecord> CoagulationFiveRecordStore => _unitOfWork.GetStore<CoagulationFiveRecord>();
        //private IRepository<InspectionOtherRecord> InspectionOtherRecordStore => _unitOfWork.GetStore<InspectionOtherRecord>();
        //private IRepository<HypertensiveRecord> HypertensiveRecordStore => _unitOfWork.GetStore<HypertensiveRecord>();
        //private IRepository<ComplicationsHistoryRecord> ComplicationsHistoryRecordStore => _unitOfWork.GetStore<ComplicationsHistoryRecord>();
        //private IRepository<GeneticHistoryRecord> GeneticHistoryRecordStore => _unitOfWork.GetStore<GeneticHistoryRecord>();
        //private IRepository<FallHistoryRecord> FallHistoryRecordStore => _unitOfWork.GetStore<FallHistoryRecord>();
        //private IRepository<PastHistoryRecord> PastHistoryRecordStore => _unitOfWork.GetStore<PastHistoryRecord>();
        //private IRepository<DrugTakingRecord> DrugTakingRecordStore => _unitOfWork.GetStore<DrugTakingRecord>();
        //private IRepository<GoutRecord> GoutRecordStore => _unitOfWork.GetStore<GoutRecord>();
        //private IRepository<DrugAbuseRecord> DrugAbuseRecordStore => _unitOfWork.GetStore<DrugAbuseRecord>();
        //private IRepository<KidneyTransplantRecord> KidneyTransplantRecordStore => _unitOfWork.GetStore<KidneyTransplantRecord>();

        //private IRepository<BalanceDetails> BalanceDetailsStore => _unitOfWork.GetStore<BalanceDetails>();
        //private IRepository<BalanceMain> BalanceMainStore => _unitOfWork.GetStore<BalanceMain>();
        //private IRepository<BalancePayType> BalancePayTypeStore => _unitOfWork.GetStore<BalancePayType>();
        //private IRepository<DayBalanceDetail> DayBalanceDetailStore => _unitOfWork.GetStore<DayBalanceDetail>();
        //private IRepository<DayBalanceMain> DayBalanceMainStore => _unitOfWork.GetStore<DayBalanceMain>();
        //private IRepository<SI_CFMX> SI_CFMXStore => _unitOfWork.GetStore<SI_CFMX>();
        //private IRepository<SI_JSMX> SI_JSMXStore => _unitOfWork.GetStore<SI_JSMX>();
        //private IRepository<SI_JZDJMX> SI_JZDJMXStore => _unitOfWork.GetStore<SI_JZDJMX>();
        //private IRepository<SICompare> SICompareStore => _unitOfWork.GetStore<SICompare>();

        //   private IRepository<HistoryDialysisRecords> HistoryDialysisRecordsStore => _unitOfWork.GetStore<HistoryDialysisRecords>();
        //   private IRepository<PatientCycleScheduling> PatientCycleSchedulingStore => _unitOfWork.GetStore<PatientCycleScheduling>();
        //  private IRepository<CurrentDialysisProgram> CurrentDialysisProgramStore => _unitOfWork.GetStore<CurrentDialysisProgram>();
        //   private IRepository<PreTreatMessage> PreTreatMessageStore => _unitOfWork.GetStore<PreTreatMessage>();
        //   private IRepository<DialysisAdequacyRecord> DialysisAdequacyRecordStore => _unitOfWork.GetStore<DialysisAdequacyRecord>();
        //   private IRepository<ObserveRecord> ObserveRecordStore => _unitOfWork.GetStore<ObserveRecord>();
        //   private IRepository<PostPreTreatMessage> PostPreTreatMessageStore => _unitOfWork.GetStore<PostPreTreatMessage>();
        //    private IRepository<FirstOutpatientRecord> FirstOutpatientRecordStore => _unitOfWork.GetStore<FirstOutpatientRecord>();
        //   private IRepository<MaterialTypeWarehouseConfig> MaterialTypeWarehouseConfigStore => _unitOfWork.GetStore<MaterialTypeWarehouseConfig>();
        //   private IRepository<AssetSecondaryStoreList> AssetSecondaryStoreListStore => _unitOfWork.GetStore<AssetSecondaryStoreList>();
        //   private IRepository<WareHouseCatalogs> WareHouseCatalogsStore => _unitOfWork.GetStore<WareHouseCatalogs>();
        //   private IRepository<TransferRecord> TransferRecordStore => _unitOfWork.GetStore<TransferRecord>();

        //   private IRepository<NursingAssessmentRecord> NursingAssessmentRecordStore => _unitOfWork.GetStore<NursingAssessmentRecord>();
        //   private IRepository<FallAssessmentAndNursingPlan> FallAssessmentAndNursingPlanStore => _unitOfWork.GetStore<FallAssessmentAndNursingPlan>();
        // private IRepository<PatientNutritionAssessmentRecord> PatientNutritionAssessmentRecordStore => _unitOfWork.GetStore<PatientNutritionAssessmentRecord>();

        // private IRepository<PatientHealthEducationRecord> PatientHealthEducationRecordStore => _unitOfWork.GetStore<PatientHealthEducationRecord>();


        private IRepository<SI_YPMLS> SI_YPMLStore => _unitOfWork.GetStore<SI_YPMLS>();
        private IRepository<SI_ZLXMS> SI_ZLXMStore => _unitOfWork.GetStore<SI_ZLXMS>();

        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public PurchasManagerService(ILogger<PurchasManagerService> logger, IDataReceive dataService,
            IUnitOfWork unitOfWork, IOptions<Data.DictionaryCode> dictionaryCode, WebDataUrl webUrl)
        {
            _logger = logger;
            _dataService = dataService;
            _unitOfWork = unitOfWork;
            _dictionaryCode = dictionaryCode.Value;
            _webUrls = webUrl;
        }


        /// <summary>
        ///物品库存信息
        /// </summary>
        /// <returns></returns>
        public Task<GoodsInStockdModel[]> GetGoodsInStockdInfo(string CenterId)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                //if (!string.IsNullOrWhiteSpace(model.MedicalItemName))
                //{
                //    sqlWhere += "  and a.MedicalItemName like  @MedicalItemName";
                //    sqlParameterList.Add(new SqlParameter("@MedicalItemName", $"%{model.MedicalItemName}%"));
                //}
                //if (model.MedicalItemType.HasValue)
                //{
                //    sqlWhere += "  and a.MedicalItemType=@MedicalItemType";
                //    sqlParameterList.Add(new SqlParameter("@MedicalItemType", model.MedicalItemType));
                //}
                if (!string.IsNullOrWhiteSpace(CenterId) && CenterId != "0")
                {
                    sqlWhere += "  and a.CenterId=@CenterId";
                    sqlParameterList.Add(new SqlParameter("@CenterId", CenterId));
                }
                sqlWhere += "  order by a.MedicalItemName asc";
                var xmlSqlParameter = GetXmlSqlParameter("MaterialsStatisticaManager", "SqlGetGoodsInStockdInfo", "获取物品库存信息");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = sql + sqlWhere;
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<GoodsInStockdModel>(dt);

                var result_List = new List<GoodsInStockdModel>();
                if (tuple.Item1)
                {
                    var data = tuple.Item2.GroupBy(a => new { a.CenterId, a.MedicalItemId, a.Specifications, a.Manufacturer });
                    foreach (var item in data)
                    {
                        var sumQty = item.Sum(a => a.InQty);
                        var _data = item.First();
                        //  var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");
                        _data.MedicalItemName = _data.MedicalItemName;// + name;

                        _data.InQty = sumQty;
                        //_data.Specifications = _data.IsSplited.Value ? _data.Specifications : _data.Packaging;
                        //_data.UnitName = _data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName;
                        result_List.Add(_data);
                    }
                }
                return result_List.ToArray();
            });
        }
        /// <summary>
        /// 用量
        /// </summary>
        /// <param name="CenterId"></param>
        /// <returns></returns>
        public Task<GoodsInStockdModel[]> GetUsageStatistics(string CenterId)
        {
            return Task.Run(() =>
            {
                var result_List = new List<GoodsInStockdModel>();
                try
                {
                    var sqlParameterList = new List<SqlParameter>();
                    string sqlWhere = string.Empty;
                    //if (!string.IsNullOrWhiteSpace(model.MedicalItemName))
                    //{
                    //    sqlWhere += "  and mir.MedicalItemName like  @MedicalItemName";
                    //    sqlParameterList.Add(new SqlParameter("@MedicalItemName", $"%{model.MedicalItemName}%"));
                    //}
                    //if (!string.IsNullOrWhiteSpace(model.MedicalItemType) && model.MedicalItemType != "0")
                    //{
                    //    sqlWhere += " and mir.MedicalItemType=@MedicalItemType";
                    //    sqlParameterList.Add(new SqlParameter("@MedicalItemType", model.MedicalItemType));
                    //}
                    if (!string.IsNullOrWhiteSpace(CenterId) && CenterId != "0")
                    {
                        sqlWhere += " AND outbound.CenterId=@CenterId";
                        sqlParameterList.Add(new SqlParameter("@CenterId", CenterId));
                    }
                    sqlWhere += "  order by mir.MedicalItemName asc";
                    var xmlSqlParameter = GetXmlSqlParameter("MaterialsStatisticaManager", "SqlGetItemUsageStatistics", "物品用量统计");
                    string sql = GetQuerySql(xmlSqlParameter);
                    DateTime bgtime = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-3);
                    DateTime endtime = DateTime.Now.AddDays(1 - DateTime.Now.Day);
                    sql = string.Format(sql, bgtime, endtime) + sqlWhere;
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                    var tuple = GetTupleByList<GoodsInStockdModel>(dt);


                    if (tuple.Item1)
                    {
                        var data = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.MedicalItemName, a.Specifications, a.Manufacturer });
                        foreach (var item in data)
                        {
                            var sumQty = item.Sum(a => a.InQty);
                            var TotalInPrice = item.Sum(a => a.InQty * a.InPrice);
                            var TotalSalePrice = item.Sum(a => a.InQty * a.SalePrice);
                            var _data = item.First();
                            // var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");

                            _data.MedicalItemName = _data.MedicalItemName;// + name;

                            _data.TotalInQty = sumQty;
                            _data.TotalInPrice = TotalInPrice;
                            _data.TotalSalePrice = TotalSalePrice;
                            //_data.Specifications = _data.IsSplited.Value ? _data.Specifications : _data.Packaging;
                            //_data.UnitName = _data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName;
                            result_List.Add(_data);
                        }
                    }

                }
                catch (Exception exp)
                {


                }
                return result_List.ToArray();

            });
        }


        //低值易耗品用量

        /// <summary>
        /// 低值易耗品用量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<GoodsInStockdModel[]> GetOtherOutboundDetailByDateAsync(string CenterId)
        {
            return Task.Run(() =>
           {

               var sqlwhere = string.Empty;


               sqlwhere += $" and  mir.MedicalItemType in (4)";

               var xmlSqlParameter = GetXmlSqlParameter("MaterialsStatisticaManager", "SqlGetOtherOutboundDetailByDate", "时间段查询其它出库明细信息");
               string sql = GetQuerySql(xmlSqlParameter);

               var list = new List<GoodsInStockdModel>();
               var CenterDialysiss = new List<CenterDialysis>();
               DateTime bgtime = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-3);
               DateTime endtime = DateTime.Now.AddDays(1 - DateTime.Now.Day);

               sqlwhere += $" AND center.Id=@CenterId";

               sql = string.Format(sql, bgtime, endtime).Replace("@sqlWhere", sqlwhere);
               var sqlParameterList = new List<SqlParameter>();
               sqlParameterList.Add(new SqlParameter("@CenterId", CenterId));
               var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
               var tuple = GetTupleByList<GoodsInStockdModel>(dt);
               if (tuple.Item1)
               {
                   list = tuple.Item2;
               }
               var result_list = new List<GoodsInStockdModel>();

               if (list.Count > 0)
               {
                   var data = list.GroupBy(a => new { a.CenterId, a.MedicalItemName, a.Specifications, a.Manufacturer });

                   foreach (var item in data)
                   {
                       var outboundModel = item.First();
                       outboundModel.ItemQty = item.Where(t => t.OutboundType != "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty) - item.Where(t => t.OutboundType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty);
                       result_list.Add(outboundModel);
                   }
               }
               //  return new PageData<OtherOutboundDetailModel[]>(result_list.ToArray(), count);
               return result_list.ToArray();
           });
        }

        /// <summary>
        /// 报废
        /// </summary>
        public Task<bool> SyncMaterialApplyApproveData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var CenterData = CenterDialysisStore.Entities.FirstOrDefault(t => t.Id == centerId);
                    if (CenterData == null || CenterData.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                        return false;
                    _webUrls.BaseUrl = CenterData.CenterUrl;

                    _dataService.AddToken("", "PC", "");

                    var data = await _dataService.GetMaterialApplyApproveAsync();
                    if (data != null && data.Count() > 0)
                    {
                        //var ListId = data.Select(t => t.Id);
                        //   var repeatData = await PurchaseRequestStore.Entities.Select(t => t.Id).ToListAsync();

                        //   var NewData = data.ToList().Where(t => !repeatData.Contains(t.Id)).ToList();
                        data = data.Where(t => t.AuditDate >= DateTime.Now.AddMonths(-12)).ToArray();
                        foreach (var item in data)
                        {
                            var OldData = await MaterialApplyApproveStore.GetFirstOrDefaultAsync(t => t.Id == item.Id);
                            if (OldData == null)
                            {
                                item.CenterId = centerId;

                                item.AuditorLevel = 0;

                                MaterialApplyApproveStore.Insert(item);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                OldData.AuditConditionId = item.AuditConditionId;
                                OldData.AuditDate = item.AuditDate;
                                OldData.Auditor = item.Auditor;
                                OldData.Advice = item.Advice;
                                OldData.ItemType = item.ItemType;
                                OldData.TotalQty = item.TotalQty;
                                if (item.AuditConditionId == "246adb016cbf42bb978fc10d91c24ee5")
                                {
                                    OldData.GroupAdvice = "";
                                    OldData.GroupAuditDate = null;
                                    OldData.GroupAuditor = null;
                                    OldData.GroupAuditStatus = null;
                                    OldData.AuditorLevel = 0;
                                    MsSqlHelper.GetSingleObj().ExecNonQuery($"delete OrderApproves where OrderId='{OldData.Id}'");

                                }
                                MaterialApplyApproveStore.Update(OldData);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步采购申请数据:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });

        }
        /// <summary>
        /// 报废明细
        /// </summary>
        public Task<bool> SyncMaterialApplyApproveDetailData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var CenterData = CenterDialysisStore.Entities.FirstOrDefault(t => t.Id == centerId);
                    if (CenterData == null || CenterData.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                        return false;
                    _webUrls.BaseUrl = CenterData.CenterUrl;

                    _dataService.AddToken("", "PC", "");
                    var data = await _dataService.GetMaterialApplyApproveDetailAsync();
                    if (data != null && data.Count() > 0)
                    {
                        // MsSqlHelper.GetSingleObj().ExecNonQuery($"delete MaterialApplyApproveDetail where CenterId='{centerId}'");
                        //var ListId = data.Select(t => t.Id);
                        //   var repeatData = await PurchaseRequestStore.Entities.Select(t => t.Id).ToListAsync();

                        //   var NewData = data.ToList().Where(t => !repeatData.Contains(t.Id)).ToList();
                        var newDAta = data.Where(t => t.ModifierDate >= DateTime.Now.AddMonths(-6)).ToArray();
                        foreach (var item in newDAta)
                        {
                            var OldData = await MaterialApplyApproveDetailStore.GetFirstOrDefaultAsync(t => t.Id == item.Id);
                            if (OldData == null)
                            {
                                item.CenterId = centerId;
                                MaterialApplyApproveDetailStore.Insert(item);
                            }

                        }
                        await _unitOfWork.SaveChangesAsync();

                    }

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步采购申请数据:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });

        }



        /// <summary>
        /// 滞销
        /// </summary>
        public Task<bool> SyncMaterialsWarningApplyListStoreData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var CenterData = CenterDialysisStore.Entities.FirstOrDefault(t => t.Id == centerId);
                    if (CenterData == null || CenterData.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                        return false;
                    _webUrls.BaseUrl = CenterData.CenterUrl;

                    _dataService.AddToken("", "PC", "");
                    var data = await _dataService.GetMaterialsWarningApplyListAsync();
                    if (data != null && data.Count() > 0)
                    {
                        //var ListId = data.Select(t => t.Id);
                        //   var repeatData = await PurchaseRequestStore.Entities.Select(t => t.Id).ToListAsync();

                        //   var NewData = data.ToList().Where(t => !repeatData.Contains(t.Id)).ToList();
                        data = data.Where(t => t.AuditDate >= DateTime.Now.AddMonths(-12)).ToArray();
                        foreach (var item in data)
                        {
                            var OldData = await MaterialsWarningApplyListStore.GetFirstOrDefaultAsync(t => t.Id == item.Id);
                            if (OldData == null)
                            {
                                item.CenterId = centerId;
                                item.AuditorLevel = 0;
                                MaterialsWarningApplyListStore.Insert(item);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                OldData.AuditConditionId = item.AuditConditionId;
                                OldData.AuditDate = item.AuditDate;
                                OldData.Auditor = item.Auditor;
                                OldData.Advice = item.Advice;
                                MaterialsWarningApplyListStore.Update(OldData);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步采购申请数据:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });

        }
        /// <summary>
        /// 滞销明细
        /// </summary>
        public Task<bool> SyncMaterialsWarningApplyDetailListData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var CenterData = CenterDialysisStore.Entities.FirstOrDefault(t => t.Id == centerId);
                    if (CenterData == null || CenterData.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                        return false;
                    _webUrls.BaseUrl = CenterData.CenterUrl;

                    _dataService.AddToken("", "PC", "");
                    var data = await _dataService.GetMaterialsWarningApplyDetailListAsync();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete MaterialsWarningApplyDetailList where CenterId='{centerId}'");
                        //var ListId = data.Select(t => t.Id);
                        //   var repeatData = await PurchaseRequestStore.Entities.Select(t => t.Id).ToListAsync();

                        //   var NewData = data.ToList().Where(t => !repeatData.Contains(t.Id)).ToList();

                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            MaterialsWarningApplyDetailListStore.Insert(item);
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步采购申请数据:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });

        }



        /// <summary>
        /// 申请单
        /// </summary>
        public Task<bool> SyncPurchasManagerData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await _dataService.GetPurchasManagerAsync();
                    if (data != null && data.Count() > 0)
                    {
                        //var ListId = data.Select(t => t.Id);
                        //   var repeatData = await PurchaseRequestStore.Entities.Select(t => t.Id).ToListAsync();

                        //   var NewData = data.ToList().Where(t => !repeatData.Contains(t.Id)).ToList();
                        data = data.Where(t => t.AuditDate >= DateTime.Now.AddDays(-30)).ToArray();
                        foreach (var item in data)
                        {
                            var OldData = await PurchaseRequestStore.GetFirstOrDefaultAsync(t => t.Id == item.Id);
                            if (OldData == null)
                            {
                                item.CenterId = centerId;
                                item.ActualPrice = 0;
                                item.SalesTotalPrice = 0;
                                item.IsClosed = 2;
                                item.PurchSubmitLevel = 1;
                                item.IsMerge = false;
                                item.IsGroupAdd = false;
                                item.OrderType = 0;
                                PurchaseRequestStore.Insert(item);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                OldData.PutInStorageMark = item.PutInStorageMark;
                                PurchaseRequestStore.Update(OldData);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步采购申请数据:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });

        }
        /// <summary>
        /// 申请单明细
        /// </summary>
        public Task<bool> SyncPurchaseDetailData()
        {
            return Task.Run(async () =>
            {
                bool flag = true;
                try
                {
                    var data = await _dataService.GetPurchaseDetailDataAsync();
                    if (data != null && data.Count() > 0)
                    {
                        //  if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 18)
                        data = data.Where(t => t.ModifierDate >= DateTime.Now.AddDays(-30)).ToArray();
                        //else
                        //    data = data.Where(t => t.ModifierDate >= DateTime.Now.AddDays(-60)).ToArray();
                        // var   PurchaseDetailStore = _unitOfWork.GetStore<PurchaseDetail>();
                        //库存
                        Dictionary<string, GoodsInStockdModel[]> pairs = new Dictionary<string, GoodsInStockdModel[]>();
                        //用量
                        Dictionary<string, GoodsInStockdModel[]> pairs1 = new Dictionary<string, GoodsInStockdModel[]>();
                        //低值用量
                        Dictionary<string, GoodsInStockdModel[]> pairs2 = new Dictionary<string, GoodsInStockdModel[]>();
                        var MedData = await MedicalItemRecordStroe.Entities.Where(t => t.IsDelete == false).ToListAsync();
                        foreach (var item in data)
                        {
                            var OldData = await PurchaseDetailStore.Entities.Include(t => t.medicalItemRecord).Where(t => t.Id == item.Id).FirstOrDefaultAsync();
                            var purchasData = await PurchaseRequestStore.GetFirstOrDefaultAsync(t => t.Id == item.ApplyId);
                            decimal? CurrentInventory = 0;
                            //月均用量
                            decimal? MonthAverage = 0;
                            List<string> centerIds = new List<string>();
                            centerIds.Add("0");
                            if (purchasData != null && purchasData.CenterId + "" != "")
                            {
                                centerIds.Add(purchasData.CenterId);
                                if (!pairs.ContainsKey(purchasData.CenterId))
                                {
                                    var GoodsInStockd = await GetGoodsInStockdInfo(purchasData.CenterId);
                                    if (GoodsInStockd != null || GoodsInStockd.Length > 0)
                                        pairs[purchasData.CenterId] = GoodsInStockd;
                                }
                                if (!pairs1.ContainsKey(purchasData.CenterId))
                                {
                                    var GoodsInStockd1 = await GetUsageStatistics(purchasData.CenterId);
                                    if (GoodsInStockd1 != null || GoodsInStockd1.Length > 0)
                                        pairs1[purchasData.CenterId] = GoodsInStockd1;
                                }
                                if (!pairs2.ContainsKey(purchasData.CenterId))
                                {
                                    var GoodsInStockd2 = await GetOtherOutboundDetailByDateAsync(purchasData.CenterId);
                                    if (GoodsInStockd2 != null || GoodsInStockd2.Length > 0)
                                        pairs2[purchasData.CenterId] = GoodsInStockd2;
                                }
                                var medicalData = MedData.Where(t => t.Id == item.MedicalItemId).FirstOrDefault();
                                var mName = medicalData?.MedicalItemName;
                                if (pairs.ContainsKey(purchasData.CenterId))
                                {

                                    if (mName + "" == "")
                                        CurrentInventory = 0;
                                    else
                                    {
                                        // if (medicalData.MedicalItemType == 1)
                                        CurrentInventory = pairs[purchasData.CenterId].Where(t => t.MedicalItemName == mName && t.Specifications == medicalData.Specifications && t.Manufacturer == medicalData.Manufacturer).Sum(t => t.InQty);
                                        //else
                                        //    CurrentInventory = pairs[purchasData.CenterId].Where(t => t.MedicalItemName == mName && t.Specifications == item.ProcurementPackage).Sum(t => t.InQty);
                                    }
                                }

                                if (pairs1.ContainsKey(purchasData.CenterId))
                                {
                                    if (mName + "" == "")
                                        MonthAverage = 0;
                                    else
                                    {
                                        if (medicalData.MedicalItemType == 4)
                                            MonthAverage = Math.Round(pairs2[purchasData.CenterId].Where(t => t.MedicalItemName == mName && t.Specifications == medicalData.Specifications && t.Manufacturer == medicalData.Manufacturer).Sum(t => t.ItemQty).Value / 3, 0);
                                        else

                                            MonthAverage = Math.Round(pairs1[purchasData.CenterId].Where(t => t.MedicalItemName == mName && t.Specifications == medicalData.Specifications && t.Manufacturer == medicalData.Manufacturer).Sum(t => t.TotalInQty).Value / 3, 0); //获取前3月的用量，求平均  已改为6个月，已经改为上月用量 又改为3月平均
                                                                                                                                                                                                                                                                                //else
                                                                                                                                                                                                                                                                                //    MonthAverage = Math.Round(pairs1[purchasData.CenterId].Where(t => t.MedicalItemName == mName && t.Specifications == item.ProcurementPackage).Sum(t => t.TotalInQty).Value, 0);
                                    }

                                }

                            }
                            var sup = await MedicalItemRecordStroe.GetFirstOrDefaultAsync(t => t.Id == item.MedicalItemId);
                            if (OldData == null)
                            {

                                //  string PurchDetailIds = a.ToString(); 

                                var _MedicalDrugExtensione = await MedicalDrugExtensionStore.Entities.Where(t => t.MedicalId == item.MedicalItemId && t.IsCurrentUse && centerIds.Contains(t.CenterId) && t.DataState == 1).ToListAsync();
                                if (_MedicalDrugExtensione != null && _MedicalDrugExtensione.Count > 0)
                                {

                                    if (_MedicalDrugExtensione.Where(t => t.CenterId != "0").FirstOrDefault() != null && sup.MedicalItemType != 3)
                                    {
                                        item.InPrice = _MedicalDrugExtensione.Where(t => t.CenterId != "0").FirstOrDefault().PurchasingPrice;
                                        item.SalePrice = _MedicalDrugExtensione.Where(t => t.CenterId != "0").FirstOrDefault().RetailPrice.ToFloorRound(2);
                                        // item.SupplierId = _me
                                    }
                                    else if (sup.MedicalItemType != 3)
                                    {
                                        item.InPrice = _MedicalDrugExtensione.Where(t => t.CenterId == "0").FirstOrDefault().PurchasingPrice;
                                        item.SalePrice = _MedicalDrugExtensione.Where(t => t.CenterId == "0").FirstOrDefault().RetailPrice.ToFloorRound(2);
                                    }

                                    if (sup.MedicalItemType == 2)
                                    {
                                        item.SalePrice = _MedicalDrugExtensione.Where(t => t.CenterId == "0").FirstOrDefault().RetailPrice.ToFloorRound(2);
                                        if (_MedicalDrugExtensione.Where(t => t.CenterId == "0").FirstOrDefault().AgreementPrice > 0)
                                            item.SalePrice = _MedicalDrugExtensione.Where(t => t.CenterId == "0").FirstOrDefault().AgreementPrice.ToFloorRound(2);
                                    }

                                }

                                if (item.AdvicePrice > 0 && item.InPrice == 0)
                                    item.InPrice = item.AdvicePrice;

                                item.IsGroupAdd = false;
                                if (sup.MedicalItemType == 1 || sup.MedicalItemType == 2)
                                    item.SupplierId = sup != null ? sup.SupplierId : "";
                                item.IsClosed = 2;
                                item.ApprovalQty = item.InQty;
                                item.CollectTime = DateTime.Now;
                                item.CurrentInventory = CurrentInventory;
                                item.MonthAverage = MonthAverage;
                                item.CenterId = purchasData.CenterId;
                                PurchaseDetailStore.Insert(item);
                            }
                            if (OldData != null && OldData.Id + "" != "")
                            {
                                OldData.MonthAverage = MonthAverage;
                                OldData.CurrentInventory = CurrentInventory;
                                OldData.ActualQty = item.ActualQty;
                                if (OldData.ActualQty >= OldData.ApprovalQty)
                                    OldData.IsClosed = 1;//实际到货量>=审批量， 该明细自动关闭
                                else
                                    OldData.IsClosed = 2;
                                if (sup.MedicalItemType == 4)
                                {
                                    OldData.SupplierId = item.SupplierId;

                                    OldData.InPrice = item.InPrice;
                                }

                                PurchaseDetailStore.Update(OldData);

                            }
                            await _unitOfWork.SaveChangesAsync();
                        }
                        flag = true;
                        //var ListId = data.Select(t => t.Id);
                        //var repeatData = await PurchaseDetailStore.Entities.Select(t => t.Id).Where(t => ListId.Contains(t)).ToListAsync();

                        //var NewData = data.ToList().Where(t => !repeatData.Contains(t.Id)).ToList();
                        //if (NewData != null && NewData.Count > 0)
                        //{

                        //    foreach (var item in NewData)
                        //    {
                        //        var _MedicalDrugExtensione = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.MedicalId == item.MedicalItemId && t.IsCurrentUse);
                        //        if (_MedicalDrugExtensione != null)
                        //        {
                        //            item.InPrice = item.InPrice.HasValue && item.InPrice > 0 ? item.InPrice : _MedicalDrugExtensione.PurchasingPrice;
                        //            item.SalePrice = item.SalePrice.HasValue && item.SalePrice > 0 ? item.SalePrice : _MedicalDrugExtensione.RetailPrice;
                        //        }
                        //        PurchaseDetailStore.Insert(item);
                        //        _unitOfWork.SaveChanges();
                        //    }

                        //    flag = true;
                        //}

                    }


                }
                catch (Exception ex)
                {
                    flag = false;
                    CDGService.Utils.FileHelper.WriteLog("", $"同步采购申请明细数据:{ex}");
                    // throw new Exception("同步采购申请明细数据:" + ex.Message, ex);
                    //_logger.LogError(ex, "同步采购申请明细数据:" + ex.Message);
                }
                return flag;
            });
        }

        public Task<bool> SyncMedicalItemRecordsData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await _dataService.GetMedicalItemRecordDataAsync();
                    if (data != null && data.Count() > 0)
                    {
                        data.ToList().ForEach(t => { t.CenterId = centerId; t.MayCharges = 0; });

                        MedicalItemRecordStroe.Remove(t => t.IsCenterAdd == 1 && t.CenterId == centerId);

                        await _unitOfWork.SaveChangesAsync();
                        MedicalItemRecordStroe.Insert(data);
                        await _unitOfWork.SaveChangesAsync();
                    }

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步中心端档案:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });
        }

        public Task<bool> SyncSuppliersData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await _dataService.GetSupplierDataAsync();
                    if (data != null && data.Count() > 0)
                    {
                        data.ToList().ForEach(t => t.CenterId = centerId);
                        SupplierStroe.Remove(t => t.IsCenterAdd == 1 && t.CenterId == centerId);

                        await _unitOfWork.SaveChangesAsync();
                        SupplierStroe.Insert(data);
                        await _unitOfWork.SaveChangesAsync();
                    }

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步中心端档案:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });

        }

        public Task<bool> SyncHisData(string HisCode)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await _dataService.GetSIBaseDataAsync(HisCode);
                    return data;

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步中心端档案:{ex}");
                    return false;
                    // throw new Exception("同步采购申请数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请数据:" + ex.Message);
                }
                return true;
            });

        }
        /// <summary>
        ///  患者
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPatientData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {

                    var data = await _dataService.GetCollectDataAsync<Patient>(_webUrls.PatientRequestUrl);
                    string SexTypeId = _dictionaryCode.CenterSexTypeId;//性别// "bc26726c62364272acd4df12c61200c5";

                    //医保
                    string YiBaoTypeId = _dictionaryCode.SIInsuredTypeID;// "10";
                    var sex = await SystemDictionaryStore.Entities.Where(t => t.TypeId == SexTypeId).ToListAsync();
                    var sYiBao = await SystemDictionaryStore.Entities.Where(t => t.TypeId == YiBaoTypeId).ToListAsync();

                    var list = new List<Patient>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete Patients where CenterDialysisId='{centerId}'");
                        foreach (var item in data.ToList())
                        {
                            if (item.Sex + "" != "")
                                item.Sex = sex.FirstOrDefault(k => k.Id == item.Sex).Name;
                            else
                                item.Sex = sex.FirstOrDefault().Name;
                            item.CenterId = centerId;

                            if (item.SIInsuredType + "" == "")
                            {
                                item.SIInsuredType = sYiBao.FirstOrDefault(t => t.Name == "无").Id;
                            }
                            else
                            {
                                item.SIInsuredType = (item.SIInsuredType == _dictionaryCode.SIInsuredID) ? "职工医保" : "居民医保";
                                item.SIInsuredType = sYiBao.FirstOrDefault(t => item.SIInsuredType.Contains(t.Name)).Id;

                            }
                            //PatientStore.Insert(item);
                            //await _unitOfWork.SaveChangesAsync();
                            list.Add(item);
                        }
                        // var ListId = data.Select(t => t.Id);
                        //  var repeatData = await PatientStore.Entities.Select(t => t.Id).Where(t => ListId.Contains(t)).ToListAsync();
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "Patients");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步患者信息数据:{ex}");
                    return false;

                    // throw new Exception("同步患者信息数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请明细数据:" + ex.Message);
                }
                return true;
            });
        }

        /// <summary>
        /// 设备
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncEquipmentData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await _dataService.GetCollectDataAsync<EquipmentInfo>(_webUrls.EquipmentUrl);
                    //  string SexTypeId = _dictionaryCode.CenterSexTypeId;//性别// "bc26726c62364272acd4df12c61200c5";
                    var list = new List<EquipmentInfo>();
                    if (data != null && data.Count() > 0)
                    {
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.Model = item.Model.Replace("－", "-");//替换不标准字符
                            //EquipmentInfoStore.Remove(t => ListId.Contains(t.Id));//移除存在的，
                            //EquipmentInfoStore.Insert(item);
                            //await _unitOfWork.SaveChangesAsync();
                            list.Add(item);
                        }
                        //var ListId = data.Select(t => t.Id);
                        //var repeatData = await PatientStore.Entities.Select(t => t.Id).Where(t => ListId.Contains(t)).ToListAsync();
                        if (list.Count > 0)
                        {

                            MsSqlHelper.GetSingleObj().ExecNonQuery($"delete EquipmentInfos where CenterId='{centerId}'");
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "EquipmentInfos");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步设备信息数据:{ex}");
                    return false;
                    // throw new Exception("同步设备信息数据:" + ex.Message, ex);
                    //  _logger.LogError(ex, "同步采购申请明细数据:" + ex.Message);
                }
                return true;
            });
        }

        /*
         4078 GetEquipmentSupplier  设备供应商
4079 GetEquipmentManufacturers 设备生产厂家
4080 GetEquipmentType  设备类型
4081 GetEquipmentModel  设备型号
4082 GetEquipmentBedConfigur  床位信息配置
             */


        /// <summary>
        /// 二级库批次库存
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncBatchNoSecondaryStoreroomData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<BatchNoSecondaryStoreroom>(_webUrls.BatchUrl);
                    var list = new List<BatchNoSecondaryStoreroom>();
                    if (data != null && data.Count() > 0)
                    {

                        //var datas = await BatchNoSecondaryStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    BatchNoSecondaryStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete BatchNoSecondaryStoreroom where CenterId='{centerId}'");
                        foreach (var item in data)
                        {

                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "BatchNoSecondaryStoreroom");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步一级库批次库存数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 二级库出入流水
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncBatchNoSecondaryStoreroomDetailData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<BatchNoSecondaryStoreroomDetail>(_webUrls.BatchDetailUrl);
                    var list = new List<BatchNoSecondaryStoreroomDetail>();
                    if (data != null && data.Count() > 0)
                    {
                        //var datas = await BatchNoSecondarydetailStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    BatchNoSecondarydetailStore.Remove(datas); await _unitOfWork.SaveChangesAsync();
                        //}
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete BatchNoSecondaryStoreroomDetail where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "BatchNoSecondaryStoreroomDetail");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步二级库出入流水数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 出库信息 
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncMaterialOutboundData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<MaterialOutbound>(_webUrls.OutboundUrl);
                    var list = new List<MaterialOutbound>();
                    if (data != null && data.Count() > 0)
                    {
                        //var datas = await MaterialOutboundStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    MaterialOutboundStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}   
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete MaterialOutbound where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "MaterialOutbound");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步出库信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 出库明细信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncMaterialOutboundDetailData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<MaterialOutboundDetail>(_webUrls.OutboundDetailUrl);
                    var list = new List<MaterialOutboundDetail>();
                    if (data != null && data.Count() > 0)
                    {
                        //var datas = await MaterialOutboundDetaillStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    MaterialOutboundDetaillStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete MaterialOutboundDetail where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "MaterialOutboundDetail");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步出库明细信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 处方单信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPrescriptionData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<Prescription>(_webUrls.PrescriptionUrl);
                    var list = new List<Prescription>();
                    if (data != null && data.Count() > 0)
                    {
                        var datas = await PrescriptionStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    PrescriptionStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete Prescription where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "Prescription");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步处方单信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 处方明细信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPrescriptionDetailData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PrescriptionDetail>(_webUrls.PrescriptionDetailUrl);
                    var list = new List<PrescriptionDetail>();
                    if (data != null && data.Count() > 0)
                    {
                        //var datas = await PrescriptionDetailStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    PrescriptionDetailStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete   PrescriptionDetail where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PrescriptionDetail");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步处方明细信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 退费信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPrescriptionDetailRefundData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PrescriptionDetailRefund>(_webUrls.RefundUrl);
                    var list = new List<PrescriptionDetailRefund>();
                    if (data != null && data.Count() > 0)
                    {
                        //    var datas = await PrescriptionDetailRefundStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //    if (datas.Count > 0)
                        //    {
                        //        PrescriptionDetailRefundStore.Remove(datas);
                        //        await _unitOfWork.SaveChangesAsync();
                        //    }
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete PrescriptionDetailRefund where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PrescriptionDetailRefund");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步退费信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 一级库批次库存
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPutInStorageBatchNoDetailsData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PutInStorageBatchNoDetails>(_webUrls.PutInStorageBatchUrl);
                    var list = new List<PutInStorageBatchNoDetails>();
                    if (data != null && data.Count() > 0)
                    {
                        //var datas = await PutInStorageBatchNoDetailsStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    PutInStorageBatchNoDetailsStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}\

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete PutInStorageBatchNoDetails where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PutInStorageBatchNoDetails");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步一级库出入流水数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 入库单信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPutInStorageBillData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //var now = DateTime.Now;
                    //var datas = await PutInStorageBillStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                    //if (datas.Count > 0)
                    //{
                    //    PutInStorageBillStore.Remove(datas);
                    //}
                    //var data = await _dataService.GetCollectDataAsync<PutInStorageBill>(_webUrls.PutInStorageUrl);
                    //var list = new List<PutInStorageBill>();
                    //if (data != null && data.Count() > 0)
                    //{
                    //    foreach (var item in data)
                    //    {
                    //        item.CenterId = centerId;
                    //        item.CollectData = now;
                    //        item.IsSettlement = 0;
                    //        list.Add(item);
                    //    }
                    //    if (list.Count > 0)
                    //    {
                    //        await _unitOfWork.SaveChangesAsync();
                    //        var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                    //        int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PutInStorageBill");
                    //        if (count <= 0)
                    //        {
                    //            return false;
                    //        }
                    //    }
                    //}
                    var now = DateTime.Now;
                    var datas = await PutInStorageBillStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                    var data = await _dataService.GetCollectDataAsync<PutInStorageBill>(_webUrls.PutInStorageUrl);
                    var list = new List<PutInStorageBill>();

                    if (data != null && data.Count() > 0)
                    {
                        foreach (var item in data)
                        {

                            if (item.Id == "536fe08068a041cf97f2ddcdedf09a4c")
                            {

                            }
                            item.CenterId = centerId;
                            item.CollectData = now;
                            var t = datas.FindAll(a => a.Id == item.Id);
                            if (t.Count <= 0)
                            {
                                item.IsSettlement = 0;
                                item.IsChecked = item.IsChecked.HasValue ? item.IsChecked : 0;
                                list.Add(item);

                            }
                            else
                            {

                                item.IsSettlement = t.First().IsSettlement;
                                item.SettlementDate = t.First().SettlementDate;
                                item.SettlementPeople = t.First().SettlementPeople;
                                item.IsChecked = t.First().IsChecked;
                                item.CheckedDate = t.First().CheckedDate;
                                item.CheckedPeople = t.First().CheckedPeople;
                                CoptyPropertys(item, t.First());
                                //PutInStorageBillStore.Update(t.First());
                                //await _unitOfWork.SaveChangesAsync();
                                list.Add(item);
                            }
                        }

                    }
                    if (list.Count > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete PutInStorageBill where CenterId='{centerId}'");
                        var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                        int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PutInStorageBill");
                        if (count <= 0)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步入库单信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 入库单明细信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPutInStorageBillDetailsData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PutInStorageBillDetails>(_webUrls.PutInStorageDetailUrl);
                    var list = new List<PutInStorageBillDetails>();
                    if (data != null && data.Count() > 0)
                    {
                        //var datas = await PutInStorageBillDetailsStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    PutInStorageBillDetailsStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete PutInStorageBillDetails where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PutInStorageBillDetails");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步入库单明细数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 一级库出入流水
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPutOutFlowlStoreData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PutOutFlow>(_webUrls.PutOutFlowUrl);
                    var list = new List<PutOutFlow>();
                    if (data != null && data.Count() > 0)
                    {
                        //var datas = await PutOutFlowlStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    PutOutFlowlStore.Remove(datas);
                        //    await _unitOfWork.SaveChangesAsync();
                        //}

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete PutOutFlow where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PutOutFlow");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步一级库出入流水数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 退货信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncReturnRequestData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //var now = DateTime.Now;
                    //var datas = await ReturnRequestStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                    ////if (datas.Count > 0)
                    ////{
                    ////    ReturnRequestStore.Remove(datas); ;
                    ////}
                    //var data = await _dataService.GetCollectDataAsync<ReturnRequest>(_webUrls.ReturnUrl);
                    //var list = new List<ReturnRequest>();
                    //if (data != null && data.Count() > 0)
                    //{
                    //    foreach (var item in data)
                    //    {
                    //        item.CenterId = centerId;
                    //        item.CollectData = now;
                    //        list.Add(item);
                    //    }
                    //    if (list.Count > 0)
                    //    {
                    //        await _unitOfWork.SaveChangesAsync();
                    //        var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                    //        int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "ReturnRequest");
                    //        if (count <= 0)
                    //        {
                    //            return false; 
                    //        }
                    //    }
                    //}
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<ReturnRequest>(_webUrls.ReturnUrl);
                    var list = new List<ReturnRequest>();
                    if (data != null && data.Count() > 0)
                    {
                        var datas = await ReturnRequestStore.Entities.Where(t => t.CenterId == centerId || t.CenterId == null).ToListAsync();
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            var t = datas.FindAll(a => a.Id == item.Id);
                            if (t.Count <= 0)
                            {
                                item.IsSettlement = 0;
                                item.IsChecked = 0;
                                list.Add(item);
                            }
                            else
                            {
                                item.IsSettlement = t.First().IsSettlement;
                                item.SettlementDate = t.First().SettlementDate;
                                item.SettlementPeople = t.First().SettlementPeople;
                                item.IsChecked = t.First().IsChecked;
                                item.CheckedDate = t.First().CheckedDate;
                                item.CheckedPeople = t.First().CheckedPeople;
                                // CoptyPropertys(item, t.First());
                                // ReturnRequestStore.Remove(t.First());
                                //  ReturnRequestStore.Update(t.First());

                                //   await _unitOfWork.SaveChangesAsync();
                                list.Add(item);
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete ReturnRequest where CenterId='{centerId}'");
                        var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                        int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "ReturnRequest");
                        if (count <= 0)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步退货数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 退货明细信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncReturnDetailsData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<ReturnDetails>(_webUrls.ReturnDetailUrl);
                    var list = new List<ReturnDetails>();
                    if (data != null && data.Count() > 0)
                    {
                        var datas = await ReturnDetailsStore.Entities.Where(t => t.CenterId == centerId).ToListAsync();
                        //if (datas.Count > 0)
                        //{
                        //    ReturnDetailsStore.Remove(datas); ;
                        //    await _unitOfWork.SaveChangesAsync(); 
                        //}  
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.YSalesReturnQty = item.SalesReturnQty;
                            var t = datas.FindAll(a => a.Id == item.Id);
                            if (t.Count > 0)
                            {
                                var OldData = t.First();
                                item.YSalesReturnQty = OldData.YSalesReturnQty;
                                if (OldData.YSalesReturnQty != OldData.SalesReturnQty)
                                    item.SalesReturnQty = OldData.SalesReturnQty;
                            }

                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            MsSqlHelper.GetSingleObj().ExecNonQuery($"delete ReturnDetails where CenterId='{centerId}'");
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "ReturnDetails");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步退货明细数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 维修记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncMaintenanceRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<MaintenanceRecord>(_webUrls.MaintenanceUrl);
                    var list = new List<MaintenanceRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete MaintenanceRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "MaintenanceRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步设备维修记录水数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 生化检测
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncBiochemicalTestData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<BiochemicalTest>(_webUrls.DetectUrl);
                    var list = new List<BiochemicalTest>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete BiochemicalTests where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "BiochemicalTests");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步生化检测数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 维保信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncEquipmentKeepRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<EquipmentKeepRecord>(_webUrls.MaintUrl);
                    var list = new List<EquipmentKeepRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete EquipmentKeepRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "EquipmentKeepRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步设备维保数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 门诊血液净化治疗病历首页
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncMedicalHistoryFirstPageData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<MedicalHistoryFirstPage>(_webUrls.MedicalUrl);
                    var list = new List<MedicalHistoryFirstPage>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete MedicalHistoryFirstPages where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "MedicalHistoryFirstPages");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步门诊血液净化治疗病历首页数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 门诊血液净化治疗病历首页子项主要诊断
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncFirstPageItemCurrentDiagnosisData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<FirstPageItemCurrentDiagnosis>(_webUrls.DiagnoseUrl);
                    var list = new List<FirstPageItemCurrentDiagnosis>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete FirstPageItemCurrentDiagnosiss where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "FirstPageItemCurrentDiagnosiss");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步主要诊断数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 血管通路病史记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncVascularAccessRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<VascularAccessRecord>(_webUrls.VascularUrl);
                    var list = new List<VascularAccessRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete VascularAccessRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "VascularAccessRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步血管通路数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 病历首页相关血管通路维修记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncVascularMaintaidRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<VascularMaintainRecord>(_webUrls.VascularMaintaidUrl);
                    var list = new List<VascularMaintainRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete VascularMaintainRecord where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "VascularMaintainRecord");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步血管通路维修记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 传染史
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncInfectiousDiseaseRegisterData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<InfectiousDiseaseRegister>(_webUrls.InfectiousUrl);
                    var list = new List<InfectiousDiseaseRegister>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete InfectiousDiseaseRegisters where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "InfectiousDiseaseRegisters");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步传染史数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 过敏史
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncAllergyRegisterData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<AllergyRegister>(_webUrls.AllergyUrl);
                    var list = new List<AllergyRegister>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete AllergyRegisters where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "AllergyRegisters");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步过敏史数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 肿瘤
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncTumorRegisterData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<TumorRegister>(_webUrls.TumourUrl);
                    var list = new List<TumorRegister>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete TumorRegisters where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "TumorRegisters");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步肿瘤数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 门诊病历
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncOutpatientMedicalRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<OutpatientMedicalRecord>(_webUrls.OutpatientUrl);
                    var list = new List<OutpatientMedicalRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete OutpatientMedicalRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "OutpatientMedicalRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步门诊病历数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 门诊日志
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncOutpatientLogData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<OutpatientLogModel>(_webUrls.OutpatientLogUrl);
                    var reult_list = new List<OutpatientDetailsLog>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  OutpatientDetailsLogs where CenterId='{centerId}'");

                        foreach (var item in data)
                        {
                            var model = new OutpatientDetailsLog();
                            model.Id = Guid.NewGuid().tostring32();
                            model.CenterId = centerId;
                            model.CollectData = now;
                            model.BloodPressure = item.PreSystolicPressure + "/" + item.PreDiastolicPressure;
                            model.MorbidityDate = item.MorbidityDate;
                            model.Diagnosis = item.Diagnose;
                            model.PatientId = item.PatientId;
                            model.DiagnosisDate = item.VisitDate;
                            //model.ReportDate = "未提供";
                            //model.ReportUser = "未提供";
                            model.SubsequentVisit = item.IsDiagnoseAgain;
                            model.Symptoms = item.MainSymptomsAndSigns;
                            //model.Remark ="";
                            //model.Founder = usId;
                            model.FounderDate = now;
                            //model.Modifier = usId;
                            model.ModifierDate = now;
                            model.DataState = 1;
                            reult_list.Add(model);
                        }
                        OutpatientLogStore.Insert(reult_list);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步门诊日志数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 治疗小结
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncStageSummaryData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<StageSummary>(_webUrls.StageSummaryUrl);
                    var list = new List<StageSummary>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  StageSummarys where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "StageSummarys");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步治疗小结数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 干体重
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynDryWeightSetData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<DryWeightSet>(_webUrls.DryWeightSetUrl);
                    var list = new List<DryWeightSet>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  DryWeightSet where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "DryWeightSet");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步干体重数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        #region 检验结果
        /// 血常规
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynRoutineBloodRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<RoutineBloodRecord>(_webUrls.RoutineBloodUrl);
                    var list = new List<RoutineBloodRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  RoutineBloodRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "RoutineBloodRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步血常规数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 电解质七项
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynSevenElectrolyteTermsRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SevenElectrolyteTermsRecord>(_webUrls.SevenElectrolyteTermsUrl);
                    var list = new List<SevenElectrolyteTermsRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  SevenElectrolyteTermsRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SevenElectrolyteTermsRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步电解质七项数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 矿物质及骨代谢
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynMineralAndBoneMetabolismRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<MineralAndBoneMetabolismRecord>(_webUrls.MineralAndBoneMetabolismUrl);
                    var list = new List<MineralAndBoneMetabolismRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  MineralAndBoneMetabolismRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "MineralAndBoneMetabolismRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步矿物质及骨代谢数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 肾功能
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynRenalFunctionRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<RenalFunctionRecord>(_webUrls.RenalFunctionRecordUrl);
                    var list = new List<RenalFunctionRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  RenalFunctionRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "RenalFunctionRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步肾功能数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        ///  肝功，血脂血糖
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynLiverFunctionBloodFatBoodSugarRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<LiverFunctionBloodFatBoodSugarRecord>(_webUrls.LiverFunctionBloodFatBoodSugarUrl);
                    var list = new List<LiverFunctionBloodFatBoodSugarRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  LiverFunctionBloodFatBoodSugarRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "LiverFunctionBloodFatBoodSugarRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步 肝功，血脂血糖数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 传染病监测
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynInfectiousDiseasesRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<InfectiousDiseasesRecord>(_webUrls.InfectiousDiseasesUrl);
                    var list = new List<InfectiousDiseasesRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  InfectiousDiseasesRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "InfectiousDiseasesRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步传染病监测数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 铁参数
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynIronParametersRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<IronParametersRecord>(_webUrls.IronParametersUrl);
                    var list = new List<IronParametersRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  IronParametersRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "IronParametersRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步铁参数数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 心肌酶5项
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynMyocardialEnzymeFiveRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<MyocardialEnzymeFiveRecord>(_webUrls.MyocardialEnzymeFiveUrl);
                    var list = new List<MyocardialEnzymeFiveRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  MyocardialEnzymeFiveRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "MyocardialEnzymeFiveRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步心肌酶5项数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 高血压4项
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynHypertensionFourRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<HypertensionFourRecord>(_webUrls.HypertensionFourUrl);
                    var list = new List<HypertensionFourRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  HypertensionFourRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "HypertensionFourRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步高血压4项数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 贫血3项
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynAnemiaThreeRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<AnemiaThreeRecord>(_webUrls.AnemiaThreeUrl);
                    var list = new List<AnemiaThreeRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  AnemiaThreeRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "AnemiaThreeRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步贫血3项数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 凝血5项
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynCoagulationFiveRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<CoagulationFiveRecord>(_webUrls.CoagulationFiveUrl);
                    var list = new List<CoagulationFiveRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  CoagulationFiveRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "CoagulationFiveRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步凝血5项数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 其它
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynInspectionOtherRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<InspectionOtherRecord>(_webUrls.InspectionOtheUrl);
                    var list = new List<InspectionOtherRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  InspectionOtherRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "InspectionOtherRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步其它数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        #endregion
        #region 病史记录
        /// 高血压
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynHypertensiveRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<HypertensiveRecord>(_webUrls.HypertensiveUrl);
                    var list = new List<HypertensiveRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  HypertensiveRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "HypertensiveRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步高血压数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 并发症
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynComplicationsHistoryRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<ComplicationsHistoryRecord>(_webUrls.ComplicationsHistoryUrl);
                    var list = new List<ComplicationsHistoryRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  ComplicationsHistoryRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "ComplicationsHistoryRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步并发症数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 遗传史
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynGeneticHistoryRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<GeneticHistoryRecord>(_webUrls.GeneticHistorymUrl);
                    var list = new List<GeneticHistoryRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  GeneticHistoryRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "GeneticHistoryRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步遗传史数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 跌倒史
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynFallHistoryRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<FallHistoryRecord>(_webUrls.FallHistoryUrl);
                    var list = new List<FallHistoryRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  FallHistoryRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "FallHistoryRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步跌倒史数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// </summary>
        ///  既往史
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynPastHistoryRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PastHistoryRecord>(_webUrls.PastHistoryUrl);
                    var list = new List<PastHistoryRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  PastHistoryRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PastHistoryRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步既往史数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 犯罪吸毒史
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynDrugTakingRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<DrugTakingRecord>(_webUrls.DrugTakingUrl);
                    var list = new List<DrugTakingRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  DrugTakingRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "DrugTakingRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步犯罪吸毒史数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 痛风史
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynGoutRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<GoutRecord>(_webUrls.GoutUrl);
                    var list = new List<GoutRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  GoutRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "GoutRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步痛风史数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 药物滥用
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynDrugAbuseRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<DrugAbuseRecord>(_webUrls.DrugAbuseUrl);
                    var list = new List<DrugAbuseRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  DrugAbuseRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "DrugAbuseRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步药物滥用数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 肾移植
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynKidneyTransplantRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<KidneyTransplantRecord>(_webUrls.KidneyTransplantUrl);
                    var list = new List<KidneyTransplantRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  KidneyTransplantRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "KidneyTransplantRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步肾移植数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        #endregion

        #region 医保相关数据
        /// <summary>
        /// 结算明细表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynBalanceDetailsData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<BalanceDetails>(_webUrls.BalanceDetailsUrl);
                    var list = new List<BalanceDetails>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  BalanceDetailss where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "BalanceDetailss");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步结算明细表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 费用结算
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynBalanceMainData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<BalanceMain>(_webUrls.BalanceMainUrl);
                    var list = new List<BalanceMain>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  BalanceMains where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "BalanceMains");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步费用结算数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 费用结算支付方式表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynBalancePayTypeData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<BalancePayType>(_webUrls.BalancePayTypeUrl);
                    var list = new List<BalancePayType>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  BalancePayType where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "BalancePayType");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步费用结算支付方式表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 日结明细表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynDayBalanceDetailData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<DayBalanceDetail>(_webUrls.DayBalanceDetailUrl);
                    var list = new List<DayBalanceDetail>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  DayBalanceDetail where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "DayBalanceDetail");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步日结明细表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 日结主表
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        public Task<bool> SynDayBalanceMainData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<DayBalanceMain>(_webUrls.DayBalanceMainUrl);
                    var list = new List<DayBalanceMain>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  DayBalanceMain where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "DayBalanceMain");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步日结主表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 上传医保的处方明细记录中间表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynSI_CFMXData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SI_CFMX>(_webUrls.SI_CFMXUrl);
                    var list = new List<SI_CFMX>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  SI_CFMX where CenterId='{centerId}'");
                        foreach (var item in data)
                        {

                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SI_CFMX");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步上传医保的处方明细记录中间表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 医保的结算明细中间表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynSI_JSMXData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SI_JSMX>(_webUrls.SI_JSMXUrl);
                    var list = new List<SI_JSMX>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  SI_JSMX where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SI_JSMX");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步医保的结算明细中间表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 医保的就诊登记明细中间表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynSI_JZDJMXData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SI_JZDJMX>(_webUrls.SI_JZDJMXUrl);
                    var list = new List<SI_JZDJMX>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  SI_JZDJMX where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            item.Id = Guid.NewGuid().tostring32();
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SI_JZDJMX");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步医保的就诊登记明细中间表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 医保对照中间表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynSICompareData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SICompare>(_webUrls.SICompareUrl);
                    var list = new List<SICompare>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  SICompare where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            var medData = await MedicalItemRecordStroe.Entities.Include(t => t.MedicalDrugExtensions).FirstAsync(t => t.MedicalItemCode == item.MedicalItemCode);
                            if (medData != null && medData.HiCenterCode + "" == "")
                            {
                                switch (medData.MedicalItemType)
                                {

                                    case 1: //药品
                                        var SiData = await SI_YPMLStore.Entities.Where(T => T.YPLSH == item.CenterItemCode).FirstOrDefaultAsync();
                                        medData.HiCenterCode = item.CenterItemCode;
                                        medData.HiLevel = item.CenterItemClass + "" != "" ? Convert.ToInt32(item.CenterItemClass) : 3;
                                        medData.MedicalDrugExtensions.ForEach(t => t.SocialSecurityPrice = (SiData.YLBZDJ.HasValue ? SiData.YLBZDJ.Value : 0));
                                        break;
                                    default: //诊疗项目+耗材
                                        var HcData = await SI_ZLXMStore.Entities.Where(T => T.XMLSH == item.CenterItemCode).FirstOrDefaultAsync();
                                        medData.HiCenterCode = item.CenterItemCode;
                                        medData.HiLevel = item.CenterItemClass + "" != "" ? Convert.ToInt32(item.CenterItemClass) : 3;
                                        medData.MedicalDrugExtensions.ForEach(t => t.SocialSecurityPrice = (HcData.YLBZJ.HasValue ? HcData.YLBZJ.Value : 0));
                                        break;
                                }

                                MedicalItemRecordStroe.Update(medData);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SICompare");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步医保对照中间表数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        #endregion
        /// 透析记录单
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynHistoryDialysisRecordsData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<HistoryDialysisRecords>(_webUrls.HistoryDialysisUrl);
                    var list = new List<HistoryDialysisRecords>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  HistoryDialysisRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "HistoryDialysisRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步透析记录单数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 患者签到
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynPatientCycleSchedulingData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PatientCycleScheduling>(_webUrls.CycleSchedulingUrl);
                    var list = new List<PatientCycleScheduling>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  PatientCycleScheduling where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PatientCycleScheduling");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步患者签到数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// 当前透析方案
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynCurrentDialysisProgramData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<CurrentDialysisProgram>(_webUrls.DialysisProgramUrl);
                    var list = new List<CurrentDialysisProgram>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  CurrentDialysisPrograms where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "CurrentDialysisPrograms");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步当前透析方案数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        ///  上机信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynPreTreatMessageData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PreTreatMessage>(_webUrls.PreTreatMessageUrl);
                    var list = new List<PreTreatMessage>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  PreTreatMessages where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PreTreatMessages");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步上机信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 透析充分性记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SynDialysisAdequacyRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<DialysisAdequacyRecord>(_webUrls.DialysisAdequacyRecordUrl);
                    var list = new List<DialysisAdequacyRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  DialysisAdequacyRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "DialysisAdequacyRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步透析充分性记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 观察记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncObserveRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<ObserveRecord>(_webUrls.ObserveRecordUrl);
                    var list = new List<ObserveRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  ObserveRecord where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "ObserveRecord");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步观察记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 下机信息
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPostPreTreatMessageData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PostPreTreatMessage>(_webUrls.PostPreTreatMessageUrl);
                    var list = new List<PostPreTreatMessage>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  PostPreTreatMessage where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PostPreTreatMessage");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步下机信息数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 专用病历（首次门诊病历）GetMaterialTypeWarehouseConfigInfo
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncFirstOutpatientRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<FirstOutpatientRecord>(_webUrls.FirstOutpatientUrl);
                    var list = new List<FirstOutpatientRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  FirstOutpatientRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "FirstOutpatientRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步专用病历（首次门诊病历）数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 物品库房关联表
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncMaterialTypeWarehouseConfigData()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<MaterialTypeWarehouseConfig>(_webUrls.ConfigUrl);
                    var list = new List<MaterialTypeWarehouseConfig>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  MaterialTypeWarehouseConfig ");
                        foreach (var item in data)
                        {
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "MaterialTypeWarehouseConfig");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步物品库房关联表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 库房类别
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncWareHouseCatalogsData()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<WareHouseCatalogs>(_webUrls.WareHouseCatalogUrl);
                    var list = new List<WareHouseCatalogs>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  WareHouseCatalogs  ");
                        foreach (var item in data)
                        {
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "WareHouseCatalogs");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步库房类别数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 备查库
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncAssetSecondaryStoreListData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<AssetSecondaryStoreList>(_webUrls.AssetSecondaryStoreUrl);
                    var list = new List<AssetSecondaryStoreList>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  AssetSecondaryStoreList where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "AssetSecondaryStoreList");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步备查库数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 转归记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncTransferRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<TransferRecord>(_webUrls.TransferRecordUrl);
                    var list = new List<TransferRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  TransferRecord where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "TransferRecord");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步转归记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 护理评估
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncNursingAssessmentRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<NursingAssessmentRecord>(_webUrls.NursingAssessmentUrl);
                    var list = new List<NursingAssessmentRecord>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  NursingAssessmentRecord where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "NursingAssessmentRecord");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步护理评估记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 跌倒评估
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncFallAssessmentAndNursingPlanData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<FallAssessmentAndNursingPlan>(_webUrls.FallAssessmentUrl);
                    var list = new List<FallAssessmentAndNursingPlan>();
                    if (data != null && data.Count() > 0)
                    {

                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  FallAssessmentAndNursingPlan where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "FallAssessmentAndNursingPlan");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步跌倒评估记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 营养评估
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPatientNutritionAssessmentRecordData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PatientNutritionAssessmentRecord>(_webUrls.NutritionAssessmentUrl);
                    var list = new List<PatientNutritionAssessmentRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  PatientNutritionAssessmentRecord where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PatientNutritionAssessmentRecord");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步营养评估记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 健康宣教
        /// </summary>
        /// <returns></returns>
        public Task<bool> SyncPatientHealthEducationRecordsData(string centerId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<PatientHealthEducationRecord>(_webUrls.HealthEducationtUrl);
                    var list = new List<PatientHealthEducationRecord>();
                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete  PatientHealthEducationRecords where CenterId='{centerId}'");
                        foreach (var item in data)
                        {
                            item.CenterId = centerId;
                            item.CollectData = now;
                            list.Add(item);
                        }
                        if (list.Count > 0)
                        {

                            var InsertDataTable = DataTypeConvertHelper.ListToDataTable(list);
                            int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "PatientHealthEducationRecords");
                            if (count <= 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步健康宣教记录数据:{ex}");
                    return false;
                }
                return true;
            });
        }




        /// <summary>
        /// 推送中心端发布记录
        /// </summary>
        /// <param name="sysPublishInfo"></param>
        /// <returns></returns>

        public Task<bool> SyncSendSysPublishInfo(SysPublishInfo sysPublishInfo)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                try
                {

                    var now = DateTime.Now;

                    Flag = await _dataService.SendPublishVer(sysPublishInfo);

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"推送发布信息异常:{ex}");
                    return false;
                }
                return true;
            });
        }
        //
        //   public Task<bool> SendPublishNotices(CenterNotices centerNotices)
        public Task<bool> SendPublishNotices(CenterNotices centerNotices)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                try
                {

                    var now = DateTime.Now;

                    Flag = await _dataService.SendPublishNotices(centerNotices);

                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"推送发布信息异常:{ex}");
                    return false;
                }
                return true;
            });
        }

        public Task<bool> GetYPMLAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SI_YPMLS>(_webUrls.GetYPMLUrl);

                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete SI_YPML");
                        var InsertDataTable = DataTypeConvertHelper.ListToDataTable(data.ToList());
                        int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SI_YPML");
                        if (count <= 0)
                        {
                            return false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步医保药品表数据:{ex}");
                    return false;
                }
                return true;
            });
        }

        public Task<bool> GetZLXMAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SI_ZLXMS>(_webUrls.GetZLXMUrl);

                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete SI_ZLXM");
                        var InsertDataTable = DataTypeConvertHelper.ListToDataTable(data.ToList());
                        int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SI_ZLXM");
                        if (count <= 0)
                        {
                            return false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步医保诊疗项目表数据:{ex}");
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 病种目录  17562  
        /// </summary>
        /// <returns></returns>
        public Task<bool> GetSI_BZMLsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now;

                    var data = await _dataService.GetCollectDataAsync<SI_BZML>(_webUrls.GetBZMLUrl);

                    if (data != null && data.Count() > 0)
                    {
                        MsSqlHelper.GetSingleObj().ExecNonQuery($"delete SI_BZML");
                        var InsertDataTable = DataTypeConvertHelper.ListToDataTable(data.ToList());
                        int count = MsSqlHelper.GetSingleObj().InsertBulkToDB(InsertDataTable, "SI_BZML");
                        if (count <= 0)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步病种目录表数据:{ex}");
                    return false;
                }
                return true;
            });

        }
        private void CoptyPropertys<P, T>(P src, T target)
        {
            try
            {
                var srcTypes = src.GetType();//获得类型  
                var typed = typeof(T);
                foreach (PropertyInfo sp in srcTypes.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo dp in typed.GetProperties())
                    {

                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType)//判断属性名是否相同  
                        {

                            var data = sp.GetValue(src, null);
                            //   if (data + "" != "" && data + "" != "0")
                            dp.SetValue(target, sp.GetValue(src, null), null);//获得s对象属性的值复制给d对象的属性  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }

    /// <summary>
    /// 物品库存信息
    /// </summary>
    public class GoodsInStockdModel
    {
        public string Id { get; set; }
        public string MedicalItemId { get; set; }
        public string MedicalItemName { get; set; }
        public string Packaging { get; set; }
        public string PackageUnit { get; set; }
        public string PackageUnitName { get; set; }
        public string Specifications { get; set; }
        public string SpecificationsUnit { get; set; }
        public string SpecificationsUnitName { get; set; }
        public string BatchNo { get; set; }
        public decimal? TakeInventory { get; set; }
        public decimal? InQty { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public string Manufacturer { get; set; }
        public string SupplierName { get; set; }
        /// 根据是否可拆分展示包装单位或规格单位
        /// </summary>
        public string MedicalItemSpecifications { get; set; }
        public string MedicalItemUnit { get; set; }
        public string MedicalItemUnitName { get; set; }
        /// <summary>
        /// 是否警示
        /// </summary>
        public bool? IsWarning { get; set; }
        public int? MinInventory { get; set; }
        public bool? IsSplited { get; set; }
        public string SupplierId { get; set; }
        public string UnitName { get; set; }
        public string GoodsName { get; set; }
        public string Brand { get; set; }
        public string ItemTypeName { get; set; }

        public decimal? TotalInPrice { get; set; }
        public decimal? TotalSalePrice { get; set; }
        public decimal? TotalInQty { get; set; }
        public string CenterId { get; set; }
        public string DialysisName { get; set; }

        public decimal? ItemQty { get; set; }
        public string OutboundType { get; set; }
    }



}
