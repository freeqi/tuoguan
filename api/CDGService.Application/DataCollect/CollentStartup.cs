﻿﻿using CDGService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDGService.Data.Helper;
using System.Timers;
using CDGService.Data.Store;
using CDGService.Application.Service.Net;
using System.IO;
using Microsoft.EntityFrameworkCore;
using CDGService.Data.Datas;
using System.Linq.Expressions;
using CDGService.Utils;

namespace CDGService.Application.DataCollect
{

    public class CollentStartup
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataReceive _dataService;
        private readonly IPurchasManagerService _purchasManagerService;
        private readonly Object O = new object();
        private List<CenterUser> _MyCenterUsers;
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<PurchaseRequest> PurchaseRequestStore => _unitOfWork.GetStore<PurchaseRequest>();

        private IRepository<PurchaseDetail> PurchaseDetailStore => _unitOfWork.GetStore<PurchaseDetail>();

        private IRepository<OrderDetail> OrderDetailStore => _unitOfWork.GetStore<OrderDetail>();

        private IRepository<DialysisProgram> DialysisProgramStore => _unitOfWork.GetStore<DialysisProgram>();
        private IRepository<PurchaseOrder> PurchaseOrderStore => _unitOfWork.GetStore<PurchaseOrder>();
        private List<CenterDialysis> ListcenterDialyses;
        private readonly WebDataUrl _webUrls;
        //private static string filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Common",
        //  $"CDGServiceWebApi{DateTime.Now.ToString("yyyyMMdd")}.log");
        public CollentStartup(IDataReceive dataService, IUnitOfWork unitOfWork, IPurchasManagerService purchasManagerService, Microsoft.Extensions.Options.IOptions<List<CenterUser>> _CenterUsers, WebDataUrl webUrls)
        {
            _dataService = dataService;
            _purchasManagerService = purchasManagerService;

            _MyCenterUsers = _CenterUsers.Value;
            _unitOfWork = unitOfWork;
            _webUrls = webUrls;
            ListcenterDialyses = CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && t.CenterUrl != "").ToList();

        }

        #region 数据采集定时器
        /// <summary>
        /// 采购申请单
        /// </summary>
        private static System.Timers.Timer timerPurchasManager = null;
        /// <summary>
        /// 采购申请单明细
        /// </summary>
        private static System.Timers.Timer timerPurchaseDetail = null;
        /// <summary>
        /// 患者信息
        /// </summary>
        private static System.Timers.Timer timerPatient = null;

        private static System.Timers.Timer timerEquipment = null;
        public delegate Task<bool> Predicate();
        //初始化Timer
        /// <summary>
        /// 初始化Timer
        /// </summary>
        //private static Func<string, double, Predicate, Timer> getInitTimer = (dataType, intervalMinute, doAction) =>
        //{
        //    var timer = new Timer(60000 * intervalMinute);
        //    timer.Elapsed += (sender, e) =>
        //    {
        //        try
        //        {
        //            var Flag = doAction();
        //            if (!Flag.Result)
        //                CDGService.Utils.FileHelper.WriteLog("", $"{dataType}同步数据失败，等待下次同步");
        //            else
        //            {
        //                CDGService.Utils.FileHelper.WriteLog(filename, $"{DateTime.Now}{dataType}完成同步数据");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            CDGService.Utils.FileHelper.WriteLog("", $"{dataType}同步数据失败，等待下次同步:{ex}");
        //            //throw new Exception(ex.Message, ex);
        //        }

        //    };
        //    timer.AutoReset = true;
        //    timer.Start();
        //    return timer;
        //};

        /// <summary>
        /// 开始同步数据
        /// </summary>
        public Task StartSyncCollentData()
        {
            return Task.Run(async () =>
            {
                string filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Common",
         $"CDGServiceWebApi{DateTime.Now.ToString("yyyyMMdd")}.log");


                var Flag = false;

                //  Flag = await SynchronousOrder();
                try
                {
                    ListcenterDialyses = DateTime.Now.Day % 2 == 0 ? ListcenterDialyses.OrderBy(t => t.SortNnm).ToList() : ListcenterDialyses.OrderByDescending(t => t.SortNnm).ToList();
                    //  ListcenterDialyses = ListcenterDialyses.Where(t => t.ShortName.Contains("铜梁透析中心")).ToList();

                    foreach (var item in ListcenterDialyses)
                    {

                        // var CenterData = _MyCenterUsers.FirstOrDefault(t => t.centerKey == item.Id);
                        if (item.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                            continue;
                        _webUrls.BaseUrl = item.CenterUrl;

                        var flag = await Login(item.Id);


                        //  int aa = 0;
                        //if (aa == 0)
                        //{
                        //    Flag = await _purchasManagerService.SyncPutInStorageBillDetailsData(item.Id);
                        //    if (Flag)
                        //        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "入库明细完成同步数据");
                        //    Flag = await _purchasManagerService.SyncPutInStorageBillData(item.Id);
                        //    if (Flag)
                        //        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "入库单完成同步数据");
                        //}
                        //else
                        {


                            if (flag)
                            {
                                if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 19)//早上8点到晚上6点之间不用采集下面的
                                {
                                    Flag = await _purchasManagerService.SyncPurchasManagerData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "采购申请单完成同步数据");

                                    Flag = await _purchasManagerService.SyncPurchaseDetailData();
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, $"采购申请单明细完成同步数据");
                                    //continue

                                    //  continue;
                                }
                                else
                                {


                                    Flag = await _purchasManagerService.SyncBatchNoSecondaryStoreroomData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "二级批次库存完成同步数据");
                                    Flag = await _purchasManagerService.SyncBatchNoSecondaryStoreroomDetailData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "二级库出入流水完成同步数据");
                                    Flag = await _purchasManagerService.SyncMaterialOutboundDetailData(item.Id);

                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "出库明细完成同步数据");
                                    Flag = await _purchasManagerService.SyncMaterialOutboundData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "出库完成同步数据");
                                    Flag = await _purchasManagerService.SyncPrescriptionDetailData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "处方明细完成同步数据");
                                    Flag = await _purchasManagerService.SyncPrescriptionData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "处方单完成同步数据");

                                    Flag = await _purchasManagerService.SyncPrescriptionDetailRefundData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "退费明细完成同步数据");
                                    Flag = await _purchasManagerService.SyncPutInStorageBatchNoDetailsData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "一级库批次库存完成同步数据");
                                    Flag = await _purchasManagerService.SyncPutInStorageBillDetailsData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "入库明细完成同步数据");
                                    Flag = await _purchasManagerService.SyncPutInStorageBillData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "入库单完成同步数据");
                                    Flag = await _purchasManagerService.SyncPutOutFlowlStoreData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "一级库出入流水完成同步数据");
                                    Flag = await _purchasManagerService.SyncReturnDetailsData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "退货明细完成同步数据");
                                    Flag = await _purchasManagerService.SyncReturnRequestData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "退货单完成同步数据");

                                    Flag = await _purchasManagerService.SyncAssetSecondaryStoreListData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "备查库完成同步数据");

                                    continue;

                                    Flag = await _purchasManagerService.SyncMaintenanceRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "设备维修记录完成同步数据");
                                    Flag = await _purchasManagerService.SyncPatientData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "患者完成同步数据");
                                    Flag = await _purchasManagerService.SyncEquipmentData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "设备完成同步数据");

                                    Flag = await _purchasManagerService.SyncBiochemicalTestData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "生化检测完成同步数据");
                                    Flag = await _purchasManagerService.SyncEquipmentKeepRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "设备维保同步数据");
                                    Flag = await _purchasManagerService.SyncMedicalHistoryFirstPageData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病历首页单完成同步数据");
                                    Flag = await _purchasManagerService.SyncFirstPageItemCurrentDiagnosisData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "主要诊断完成同步数据");
                                    Flag = await _purchasManagerService.SyncAllergyRegisterData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "过敏史完成同步数据");

                                    Flag = await _purchasManagerService.SyncInfectiousDiseaseRegisterData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "传染史完成同步数据");
                                    Flag = await _purchasManagerService.SyncVascularAccessRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "血管通路完成同步数据");
                                    Flag = await _purchasManagerService.SyncTumorRegisterData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "肿瘤完成同步数据");
                                    Flag = await _purchasManagerService.SyncOutpatientMedicalRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "门诊病历完成同步数据");
                                    Flag = await _purchasManagerService.SyncOutpatientLogData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "门诊日志完成同步数据");
                                    Flag = await _purchasManagerService.SyncStageSummaryData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "阶段小结完成同步数据");
                                    Flag = await _purchasManagerService.SynDryWeightSetData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "干体重完成同步数据");

                                    Flag = await _purchasManagerService.SynRoutineBloodRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "血常规完成同步数据");
                                    Flag = await _purchasManagerService.SynSevenElectrolyteTermsRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "电解质七项完成同步数据");
                                    Flag = await _purchasManagerService.SynMineralAndBoneMetabolismRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "矿物质及骨代谢完成同步数据");
                                    Flag = await _purchasManagerService.SynRenalFunctionRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "肾功能完成同步数据");
                                    Flag = await _purchasManagerService.SynLiverFunctionBloodFatBoodSugarRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "肝功血脂血糖完成同步数据");
                                    Flag = await _purchasManagerService.SynInfectiousDiseasesRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "传染病监测完成同步数据");
                                    Flag = await _purchasManagerService.SynIronParametersRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "铁参数完成同步数据");
                                    Flag = await _purchasManagerService.SynMyocardialEnzymeFiveRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "心肌酶5项完成同步数据");
                                    Flag = await _purchasManagerService.SynHypertensionFourRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "高血压4项完成同步数据");
                                    Flag = await _purchasManagerService.SynAnemiaThreeRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "贫血3项完成同步数据");
                                    Flag = await _purchasManagerService.SynCoagulationFiveRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "凝血5项完成同步数据");
                                    Flag = await _purchasManagerService.SynInspectionOtherRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "检验结果-其它完成同步数据");
                                    Flag = await _purchasManagerService.SynHypertensiveRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-高血压完成同步数据");
                                    Flag = await _purchasManagerService.SynComplicationsHistoryRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-并发症完成同步数据");
                                    Flag = await _purchasManagerService.SynGeneticHistoryRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-遗传史完成同步数据");
                                    Flag = await _purchasManagerService.SynFallHistoryRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-跌倒史完成同步数据");
                                    Flag = await _purchasManagerService.SynPastHistoryRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-既往史完成同步数据");
                                    Flag = await _purchasManagerService.SynDrugTakingRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-犯罪吸毒史完成同步数据");
                                    Flag = await _purchasManagerService.SynGoutRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-痛风史完成同步数据");
                                    Flag = await _purchasManagerService.SynDrugAbuseRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-药物滥用完成同步数据");
                                    Flag = await _purchasManagerService.SynKidneyTransplantRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "病史记录-肾移植完成同步数据");

                                    Flag = await _purchasManagerService.SynBalanceDetailsData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "结算明细表完成同步数据");
                                    Flag = await _purchasManagerService.SynBalanceMainData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "费用结算表完成同步数据");
                                    Flag = await _purchasManagerService.SynBalancePayTypeData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "费用结算支付方式表完成同步数据");

                                    Flag = await _purchasManagerService.SynSI_CFMXData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "上传医保的处方明细记录中间表完成同步数据");
                                    Flag = await _purchasManagerService.SynSI_JSMXData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "医保的结算明细中间表完成同步数据");
                                    Flag = await _purchasManagerService.SynSI_JZDJMXData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "医保的就诊登记明细中间表完成同步数据");


                                    Flag = await _purchasManagerService.SynCurrentDialysisProgramData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "当前透析方案完成同步数据");
                                    Flag = await _purchasManagerService.SynPatientCycleSchedulingData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "患者签到完成同步数据");
                                    Flag = await _purchasManagerService.SynPreTreatMessageData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "上机信息完成同步数据");
                                    Flag = await _purchasManagerService.SynDialysisAdequacyRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "透析充分性记录完成同步数据");
                                    Flag = await _purchasManagerService.SynHistoryDialysisRecordsData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "透析记录单完成同步数据");
                                    Flag = await _purchasManagerService.SyncVascularMaintaidRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "血管通路维修记录完成同步数据");
                                    Flag = await _purchasManagerService.SyncObserveRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "观察记录完成同步数据");
                                    Flag = await _purchasManagerService.SyncPostPreTreatMessageData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "下机信息完成同步数据");
                                    Flag = await _purchasManagerService.SyncFirstOutpatientRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "首次专用病历（首次门诊病历）完成同步数据");

                                    Flag = await _purchasManagerService.SyncTransferRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "转归记录完成同步数据");

                                    Flag = await _purchasManagerService.SyncNursingAssessmentRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "护理评估记录完成同步数据");
                                    Flag = await _purchasManagerService.SyncFallAssessmentAndNursingPlanData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "跌倒评估记录完成同步数据");
                                    Flag = await _purchasManagerService.SyncPatientNutritionAssessmentRecordData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "营养评估记录完成同步数据");
                                    Flag = await _purchasManagerService.SyncPatientHealthEducationRecordsData(item.Id);
                                    if (Flag)
                                        CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "健康宣教完成同步数据");
                                }

                                //暂时不同步

                                //Flag = await _purchasManagerService.SynDayBalanceDetailData(item.Id);
                                //if (Flag)
                                //    CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "日结明细表完成同步数据");
                                //Flag = await _purchasManagerService.SynDayBalanceMainData(item.Id);
                                //if (Flag)
                                //    CDGService.Utils.FileHelper.WriteLog(filename, item.DialysisName + "日结主表完成同步数据");

                            }
                        }
                    }
                    //医保目录
                    var week = DateTime.Now.DayOfWeek;
                    if (week == DayOfWeek.Saturday)
                    {

                        _webUrls.BaseUrl = ListcenterDialyses.Where(t => t.Id == "abc3d60b474c4fef946a66887348c41a").FirstOrDefault().CenterUrl;
                        await Login("abc3d60b474c4fef946a66887348c41a");
                        //  Flag = await _purchasManagerService.GetYPMLAsync();
                        //  if (Flag)
                        //    CDGService.Utils.FileHelper.WriteLog(filename, "医保药品目录同步完成");
                        //Flag = await _purchasManagerService.GetZLXMAsync();
                        //if (Flag)
                        //    CDGService.Utils.FileHelper.WriteLog(filename, "医保诊疗项目目录同步完成");
                        //Flag = await _purchasManagerService.GetSI_BZMLsync();
                        //if (Flag)
                        //    CDGService.Utils.FileHelper.WriteLog(filename, "医保病种目录目录同步完成");
                        Flag = await _purchasManagerService.SyncMaterialTypeWarehouseConfigData();
                        if (Flag)
                            CDGService.Utils.FileHelper.WriteLog(filename, "物品库房关联表完成同步数据");
                        Flag = await _purchasManagerService.SyncWareHouseCatalogsData();
                        if (Flag)
                            CDGService.Utils.FileHelper.WriteLog(filename, "库房类别完成同步数据");
                        // Flag = await _purchasManagerService.SynSICompareData("abc3d60b474c4fef946a66887348c41a");//只同步康美的医保对照
                        //if (Flag)
                        //    CDGService.Utils.FileHelper.WriteLog(filename, "医保对照中间表完成同步数据");
                    }
                    Flag = await SynchronousOrder();
                    if (Flag)
                        CDGService.Utils.FileHelper.WriteLog(filename, $"订单到货状态完成");
                    Flag = await ChronousPurchaseAsync();
                    if (Flag)
                        CDGService.Utils.FileHelper.WriteLog(filename, $"更新采购单闭环状态完成");

                }
                catch (Exception exp)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"同步数据失败，等待下次同步:{exp}");
                }
                finally
                {
                    CDGService.Utils.FileHelper.WriteLog(filename, $"==================同步数据完成:{DateTime.Now}===========================");
                    // System.Threading.Thread.Sleep(1000 * 60 * 60 * 1);//每天同步一次
                    // System.Threading.Thread.Sleep(1000 * 60 * 5);//测试 5分钟采集一次
                }

            });


            ////每天执行一次采购申请单
            //timerPurchasManager = getInitTimer("采购申请单", 2, ExecuteGetPurchasManagerData);
            //// ExecuteGetPurchaseDetailData
            //timerPurchaseDetail = getInitTimer("采购申请单明细", 5, ExecuteGetPurchaseDetailData);
            ////患者 1天同步一次
            //timerPatient = getInitTimer("患者信息", 4, ExecuteGetPatientData);
            //// 设备 3天同步一次
            //timerEquipment = getInitTimer("设备信息", 6, ExecuteGetEquipmentData);


        }

        //两端同步采购单到货情况 
        public Task<bool> SynchronousOrder()
        {
            return Task.Run(async () =>
             {
                 bool Flag = true;
                 try
                 {
                     //采购申请单
                     // var PurchData = await PurchaseRequestStore.Entities.Where(t => t.GroupAuditStatus == "3").ToListAsync();
                     #region  -- 2020年7月7日17:05:00 服务器压力大，暂停同步
                     //foreach (var item in PurchData)
                     //{
                     //    var DetailData = await PurchaseDetailStore.Entities.Where(t => t.ApplyId == item.Id && t.IsClosed == 2 && t.DataState == 1).ToListAsync();
                     //    if (DetailData == null || DetailData.Count <= 0)
                     //        item.IsClosed = 1;
                     //    else
                     //        item.IsClosed = 2;//处理闭环后冲销，中心端取消闭环
                     //    PurchaseRequestStore.Update(item); 
                     //}
                     //await _unitOfWork.SaveChangesAsync();

                     //var ClosePurchData = await PurchaseRequestStore.Entities.Where(t => t.IsClosed == 1).ToListAsync();
                     ////同步中心端
                     //List<dynamic> dynamics = new List<dynamic>();
                     //foreach (var PurchDatas in ClosePurchData.GroupBy(t => t.CenterId))
                     //{
                     //    foreach (var item in PurchDatas)
                     //    {
                     //        dynamics.Add(new { IsClosed = item.IsClosed, Id = item.Id });
                     //    }

                     //    await _dataService.ClosePurchase(PurchDatas.Key, dynamics);
                     //}
                     #endregion

                     //更新需求订单状态:
                     /*
                      1 未到货 2 已关闭 3 部分到货 4 已收货
                      */
                     // 
                     List<int?> dataState = new List<int?>() { 1, 3 };
                     var OrderData = await PurchaseOrderStore.Entities.Where(t => t.IsDelete == false && dataState.Contains(t.DataState)).ToListAsync();

                     foreach (var item in OrderData)//订单
                     {
                         //采购明细
                         var OrderDetailData = await OrderDetailStore.Entities.Where(t => t.OrderId == item.Id).ToListAsync();

                         var purchDeailData = await PurchaseDetailStore.Entities.Where(t => t.OrderId == item.Id).ToListAsync();
                         //更新实际到货数量 
                         foreach (var DetailData in OrderDetailData)
                         {
                             var Ids = DetailData.PurchDetailIds.Split(',');
                             DetailData.ActualQty = purchDeailData.Where(t => Ids.Contains(t.Id)).Sum(t => t.ActualQty);
                             if (DetailData.ActualQty == 0 && DetailData.DataState != 2) //订单明细状态
                                 DetailData.DataState = 1;
                             if (DetailData.ActualQty > 0 && DetailData.DataState != 2)
                                 DetailData.DataState = 3;
                             if (DetailData.ActualQty >= DetailData.PurchasePuantity && DetailData.DataState != 2)
                                 DetailData.DataState = 4;
                             OrderDetailStore.Update(DetailData);
                             await _unitOfWork.SaveChangesAsync();
                         }
                         //订单状态 明细全部未收货1 
                         if (OrderDetailData.Where(t => t.DataState == 3).Count() > 0 || (OrderDetailData.Where(t => t.DataState == 1).Count() > 0 && OrderDetailData.Where(t => t.DataState == 4).Count() > 0))
                             item.DataState = 3;
                         //if (OrderDetailData.Where(t => t.DataState == 1).Count() == OrderDetailData.Count)
                         //    item.DataState = 1;
                         if (OrderDetailData.Where(t => t.DataState == 4 || t.DataState == 2).Count() == OrderDetailData.Count)
                             item.DataState = 4;
                         if (OrderDetailData.Where(t => t.DataState == 2).Count() == OrderDetailData.Count)
                             item.DataState = 2;
                         PurchaseOrderStore.Update(item);
                         await _unitOfWork.SaveChangesAsync();

                     }
                 }
                 catch (Exception ex)
                 {
                     Flag = false;
                     CDGService.Utils.FileHelper.WriteLog("", $"采购申请单明细：{ex} ");
                 }
                 return Flag;

             });
        }

        /// <summary>
        /// 更新采购闭环
        /// 每周六更新一次(半年)，每月第一个周六更新ALL
        /// </summary>
        /// <returns></returns>
        public Task<bool> ChronousPurchaseAsync()
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                try
                {

                    var beg = DateTime.Now;

                    var week = DateTime.Now.DayOfWeek;
                    if (week == DayOfWeek.Saturday)
                    {

                        foreach (var item in ListcenterDialyses)
                        {


                            Expression<Func<PurchaseRequest, bool>> predicate = t => t.DataState == 1 && t.GroupAuditStatus == "3" && t.IsClosed == 2 && t.CenterId == item.Id;
                            if (DateTime.Now.Day >= 6)
                            {
                                predicate = predicate.And(t => t.FounderDate >= DateTime.Now.AddMonths(-6));
                            }
                            var PurchaseData = await PurchaseRequestStore.Entities.Include(t => t.PurchaseDsOrders).Where(predicate).ToListAsync();

                            foreach (var Purchase in PurchaseData)
                            {
                                List<dynamic> dynamics = new List<dynamic>();
                                foreach (var Detail in Purchase.PurchaseDsOrders)
                                {
                                    if (Detail.ApprovalQty <= Detail.ActualQty && Detail.IsClosed == 2)
                                    {
                                        Detail.IsClosed = 1;
                                        dynamics.Add(new { IsClosed = 1, Id = Detail.Id, ClosedDetail = "入库完成，自动关闭" });
                                    }
                                }
                                if (dynamics.Count > 0) //关闭明细
                                {
                                    var result = await _dataService.ClosePurchaseDetails(Purchase.CenterId, dynamics);
                                    if (result.Code == 1024) //网络连接不上
                                        break;
                                    if (result.Code == 200)
                                    {
                                        PurchaseDetailStore.Update(Purchase.PurchaseDsOrders);
                                        await _unitOfWork.SaveChangesAsync();
                                    }

                                }
                                if (Purchase.PurchaseDsOrders.Where(t => t.IsClosed == 1).Count() == Purchase.PurchaseDsOrders.Count)
                                {
                                    Purchase.IsClosed = 1;

                                    List<dynamic> dynamics1 = new List<dynamic>();
                                    dynamics1.Add(new { IsClosed = 1, Id = Purchase.Id });
                                    await _dataService.ClosePurchase(Purchase.CenterId, dynamics1);

                                }
                                PurchaseRequestStore.Update(Purchase);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            var end = DateTime.Now;
                            Console.WriteLine((end - beg).ToString());
                        }
                    }
                }
                catch (Exception exp)
                {
                    CDGService.Utils.FileHelper.WriteLog("", $"更新采购单关闭功能错误:{exp}");
                }

                return Flag;
            });
        }
        /// <summary>
        /// 同步采购申请单
        /// </summary>
        public Task<bool> ExecuteGetPurchasManagerData(string CenterId = "0")
        {

            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    if (CenterId + "" != "0")
                    {
                        var CenterData = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                        if (CenterData == null || CenterData.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                            return false;
                        _webUrls.BaseUrl = CenterData.CenterUrl;

                        var flag = await Login(CenterData.Id);
                        if (flag)
                            result = await _purchasManagerService.SyncPurchasManagerData(CenterData.Id);
                    }

                    else
                    {
                        foreach (var item in ListcenterDialyses)
                        {

                            // var CenterData = _MyCenterUsers.FirstOrDefault(t => t.centerKey == item.Id);
                            if (item.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                                continue;
                            _webUrls.BaseUrl = item.CenterUrl;

                            var flag = await Login(item.Id);
                            if (flag)
                                result = await _purchasManagerService.SyncPurchasManagerData(item.Id);
                        }
                    }

                }
                catch (Exception exp)
                {
                    result = false; // 
                    CDGService.Utils.FileHelper.WriteLog("", $"采购申请单：{exp} ");
                }
                return result;
            });

        }
        //SyncPurchaseDetailData
        /// <summary>
        /// 同步采购明细
        /// </summary>
        public Task<bool> ExecuteGetPurchaseDetailData(string CenterId = "0")
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    if (CenterId + "" != "0")
                    {
                        var CenterData = ListcenterDialyses.FirstOrDefault(t => t.Id == CenterId);
                        if (CenterData == null || CenterData.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                            return false;
                        _webUrls.BaseUrl = CenterData.CenterUrl;

                        var flag = await Login(CenterData.Id);
                        if (flag)
                        {
                            result = await _purchasManagerService.SyncPurchaseDetailData();
                            //同步档案，供应商
                            await _purchasManagerService.SyncMedicalItemRecordsData(CenterData.Id);
                            await _purchasManagerService.SyncSuppliersData(CenterData.Id);

                        }

                    }

                    else
                    {

                        foreach (var item in ListcenterDialyses)
                        {

                            //var CenterData = _MyCenterUsers.FirstOrDefault(t => t.centerKey == item.Id);
                            if (item.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                                continue;
                            _webUrls.BaseUrl = item.CenterUrl;

                            var flag = await Login(item.Id);
                            if (flag)
                                result = await _purchasManagerService.SyncPurchaseDetailData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    CDGService.Utils.FileHelper.WriteLog("", $"采购申请单明细：{ex} ");
                }

                return result;
            });
        }
        /// <summary>
        /// 立即获取退货
        /// </summary>
        /// <returns></returns>
        public Task<bool> ExecuteGetReturnRequest(string CenterId)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {

                    if (CenterId != "" && CenterId != "0")
                    {
                        var item = ListcenterDialyses.Where(t => t.Id == CenterId).First();
                        if (item.CenterUrl + "" != "") // 未配置用户或采集地址不存在
                        {
                            _webUrls.BaseUrl = item.CenterUrl;

                            var flag = await Login(item.Id);
                            //  var CenterData = _MyCenterUsers.FirstOrDefault(t => t.centerKey == item.Id);
                            if (flag)
                            {
                                await _purchasManagerService.SyncReturnDetailsData(item.Id);
                                await _purchasManagerService.SyncReturnRequestData(item.Id);

                            }
                        }
                    }
                    else
                    {
                        foreach (var item in ListcenterDialyses)
                        {
                            if (item.CenterUrl + "" == "") // 未配置用户或采集地址不存在
                                continue;
                            _webUrls.BaseUrl = item.CenterUrl;

                            var flag = await Login(item.Id);
                            //  var CenterData = _MyCenterUsers.FirstOrDefault(t => t.centerKey == item.Id);
                            if (flag)
                            {
                                await _purchasManagerService.SyncReturnDetailsData(item.Id);
                                await _purchasManagerService.SyncReturnRequestData(item.Id);


                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    CDGService.Utils.FileHelper.WriteLog("", $"采购申请单明细：{ex} ");
                }

                return result;
            });
        }



        public Task<bool> SyncSendSysPublishInfo(SysPublishInfo sysPublishInfo)
        {
            return Task.Run(async () =>
            {

                return await _purchasManagerService.SyncSendSysPublishInfo(sysPublishInfo);

            });

        }

        public Task<bool> SendPublishNotices(CenterNotices centerNotices)
        {
            return Task.Run(async () =>
            {

                return await _purchasManagerService.SendPublishNotices(centerNotices);

            });

        }
        #endregion


        #region 采购申请单自动分类


        #endregion


        private Task<bool> Login(string CenterId)
        {
            return Task.Run(async () =>
            {
                // var CenterData = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                _dataService.AddToken("", "PC", "");
                return true;
                ////   var dataaa = _MyCenterUsers;
                //var CenterData = _MyCenterUsers.FirstOrDefault(t => t.centerKey == CenterId);
                //string UserName = CenterData.userName.Encrypt();
                //string Pwd = CenterData.userPwd.Encrypt();
                //_dataService.AddToken("", CenterData.ClientType, CenterData.userName + "|");
                //var data = await _dataService.UserLoginAsync(UserName, Pwd);

                //if (data != null)
                //{
                //_dataService.AddToken(data.Token, CenterData.ClientType, CenterData.userName + "|" + data.JSON.account.employeeId);
                //    //StartSyncCollentData();
                //    return true;
                //}
                //else
                //    return false;
            });
        }
    }
}
