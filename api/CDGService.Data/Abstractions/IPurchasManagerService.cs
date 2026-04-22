using CDGService.Data.Datas;
using CDGService.Data.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{
    /// <summary>
    ///采购申请
    /// </summary>
    public interface IPurchasManagerService
    {
        // IRepository<PurchaseDetail> PurchaseDetailStore { get;  set; }
        /// <summary>
        /// 同步采购申请数据
        /// </summary>
        Task<bool> SyncPurchasManagerData(string centerId);

        /// <summary>
        /// 采购明细
        /// </summary>
        Task<bool> SyncPurchaseDetailData();

        Task<bool> SyncMedicalItemRecordsData(string centerId);

        Task<bool> SyncSuppliersData(string centerId);

        Task<bool> SyncMaterialApplyApproveData(string centerId);
        Task<bool> SyncMaterialApplyApproveDetailData(string centerId);
        Task<bool> SyncMaterialsWarningApplyListStoreData(string centerId);
        Task<bool> SyncMaterialsWarningApplyDetailListData(string centerId);
        /// <summary>
        /// 患者信息
        /// </summary>
        Task<bool> SyncPatientData(string centerId);
        Task<bool> SyncHisData(string HisCode);

        /// <summary>
        /// 设备信息
        /// </summary>
        Task<bool> SyncEquipmentData(string centerId);

        /*
                /// <summary>
                /// 设备供应商
                /// </summary>
                /// <param name="centerId"></param>
                /// <returns></returns>
                Task<bool> SynctEquipmentSupplierData(string centerId);
                /// <summary>
                /// 设备生产厂家
                /// </summary>
                /// <param name="centerId"></param>
                /// <returns></returns>
                Task<bool> GetEquipmentManufacturers(string centerId);
                /// <summary>
                /// 设备类型
                /// </summary>
                /// <param name="centerId"></param>
                /// <returns></returns>
                Task<bool> GetEquipmentType(string centerId);
                /// <summary>
                /// 设备型号
                /// </summary>
                /// <param name="centerId"></param>
                /// <returns></returns>
                Task<bool> GetEquipmentModel(string centerId);
                /// <summary>
                ///  床位信息配置
                /// </summary>
                /// <param name="centerId"></param>
                /// <returns></returns>
                Task<bool> GetEquipmentBedConfigur(string centerId);

        */

        /// <summary>
        /// 二级库批次库存
        /// </summary>
        Task<bool> SyncBatchNoSecondaryStoreroomData(string centerId);

        /// <summary>
        /// 二级库出入流水
        /// </summary>
        Task<bool> SyncBatchNoSecondaryStoreroomDetailData(string centerId);

        /// <summary>
        /// 出库信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncMaterialOutboundData(string centerId);
        /// <summary>
        /// 出库明细信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncMaterialOutboundDetailData(string centerId);
        /// <summary>
        /// 处方单信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPrescriptionData(string centerId);
        /// <summary>
        /// 处方明细信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPrescriptionDetailData(string centerId);
        /// <summary>
        /// 退费信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPrescriptionDetailRefundData(string centerId);
        /// <summary>
        /// 一级库批次库存
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPutInStorageBatchNoDetailsData(string centerId);
        /// <summary>
        /// 入库单信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPutInStorageBillData(string centerId);
        /// <summary>
        /// 入库单明细信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPutInStorageBillDetailsData(string centerId);
        /// 一级库出入流水
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPutOutFlowlStoreData(string centerId);
        /// <summary>
        /// 退货信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncReturnRequestData(string centerId);
        /// <summary>
        /// 退货明细信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncReturnDetailsData(string centerId);

        /// <summary>
        /// 维修记录
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncMaintenanceRecordData(string centerId);
        /// <summary>
        /// 生化检测
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncBiochemicalTestData(string centerId);
        /// <summary>
        /// 维保信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncEquipmentKeepRecordData(string centerId);
        /// <summary>
        /// 门诊血液净化治疗病历首页
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncMedicalHistoryFirstPageData(string centerId);
        /// <summary>
        /// 门诊血液净化治疗病历首页子项主要诊断
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncFirstPageItemCurrentDiagnosisData(string centerId);
        /// <summary>
        /// 血管通路病史记录
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncVascularAccessRecordData(string centerId);
        /// <summary>
        /// 血管通路维修记录
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        Task<bool> SyncVascularMaintaidRecordData(string centerId);
        /// <summary>
        /// 传染史
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncInfectiousDiseaseRegisterData(string centerId);
        /// <summary>
        /// 过敏史
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncAllergyRegisterData(string centerId);
        /// <summary>
        /// 肿瘤
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncTumorRegisterData(string centerId);
        /// <summary>
        /// 门诊病历
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncOutpatientMedicalRecordData(string centerId);

        /// <summary>
        /// 门诊日志
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncOutpatientLogData(string centerId);


        /// <summary>
        /// 治疗小结
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncStageSummaryData(string centerId);
        /// <summary>
        /// 干体重
        /// </summary>
        /// <returns></returns>
        Task<bool> SynDryWeightSetData(string centerId);

        #region 检验结果
        /// 血常规
        /// </summary>
        /// <returns></returns>
        Task<bool> SynRoutineBloodRecordData(string centerId);
        /// 电解质七项
        /// </summary>
        /// <returns></returns>
        Task<bool> SynSevenElectrolyteTermsRecordData(string centerId);
        /// 矿物质及骨代谢
        /// </summary>
        /// <returns></returns>
        Task<bool> SynMineralAndBoneMetabolismRecordData(string centerId);
        /// 肾功能
        /// </summary>
        /// <returns></returns>
        Task<bool> SynRenalFunctionRecordData(string centerId);
        ///  肝功，血脂血糖
        /// </summary>
        /// <returns></returns>
        Task<bool> SynLiverFunctionBloodFatBoodSugarRecordData(string centerId);
        /// 传染病监测
        /// </summary>
        /// <returns></returns>
        Task<bool> SynInfectiousDiseasesRecordData(string centerId);
        /// 铁参数
        /// </summary>
        /// <returns></returns>
        Task<bool> SynIronParametersRecordData(string centerId);
        /// 心肌酶5项
        /// </summary>
        /// <returns></returns>
        Task<bool> SynMyocardialEnzymeFiveRecordData(string centerId);
        /// 高血压4项
        /// </summary>
        /// <returns></returns>
        Task<bool> SynHypertensionFourRecordData(string centerId);
        /// 贫血3项
        /// </summary>
        /// <returns></returns>
        Task<bool> SynAnemiaThreeRecordData(string centerId);
        /// 凝血5项
        /// </summary>
        /// <returns></returns>
        Task<bool> SynCoagulationFiveRecordData(string centerId);
        /// 其它
        /// </summary>
        /// <returns></returns>
        Task<bool> SynInspectionOtherRecordData(string centerId);
        #endregion

        #region 病史记录
        /// 高血压
        /// </summary>
        /// <returns></returns>
        Task<bool> SynHypertensiveRecordData(string centerId);
        /// 并发症
        /// </summary>
        /// <returns></returns>
        Task<bool> SynComplicationsHistoryRecordData(string centerId);
        /// 遗传史
        /// </summary>
        /// <returns></returns>
        Task<bool> SynGeneticHistoryRecordData(string centerId);
        /// 跌倒史
        /// </summary>
        /// <returns></returns>
        Task<bool> SynFallHistoryRecordData(string centerId);
        ///  既往史
        /// </summary>
        /// <returns></returns>
        Task<bool> SynPastHistoryRecordData(string centerId);
        /// 犯罪吸毒史
        /// </summary>
        /// <returns></returns>
        Task<bool> SynDrugTakingRecordData(string centerId);
        /// 痛风史
        /// </summary>
        /// <returns></returns>
        Task<bool> SynGoutRecordData(string centerId);
        /// 药物滥用
        /// </summary>
        /// <returns></returns>
        Task<bool> SynDrugAbuseRecordData(string centerId);
        /// 肾移植
        /// </summary>
        /// <returns></returns>
        Task<bool> SynKidneyTransplantRecordData(string centerId);
        #endregion
        #region 医保相关数据
        /// <summary>
        /// 结算明细表
        /// </summary>
        /// <returns></returns>
        Task<bool> SynBalanceDetailsData(string centerId);
        /// <summary>
        /// 费用结算
        /// </summary>
        /// <returns></returns>
        Task<bool> SynBalanceMainData(string centerId);
        /// <summary>
        /// 费用结算支付方式表
        /// </summary>
        /// <returns></returns>
        Task<bool> SynBalancePayTypeData(string centerId);
        /// <summary>
        /// 日结明细表
        /// </summary>
        /// <returns></returns>
        Task<bool> SynDayBalanceDetailData(string centerId);
        /// <summary>
        /// 日结主表
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        Task<bool> SynDayBalanceMainData(string centerId);
        /// <summary>
        /// 上传医保的处方明细记录中间表
        /// </summary>
        /// <returns></returns>
        Task<bool> SynSI_CFMXData(string centerId);
        /// <summary>
        /// 医保的结算明细中间表
        /// </summary>
        /// <returns></returns>
        Task<bool> SynSI_JSMXData(string centerId);
        /// 医保的就诊登记明细中间表
        /// </summary>
        /// <returns></returns>
        Task<bool> SynSI_JZDJMXData(string centerId);
        /// 医保对照中间表
        /// </summary>
        /// <returns></returns>
        Task<bool> SynSICompareData(string centerId);
        #endregion
        /// 透析记录单
        /// </summary>
        /// <returns></returns>
        Task<bool> SynHistoryDialysisRecordsData(string centerId);
        /// 患者签到
        /// </summary>
        /// <returns></returns>
        Task<bool> SynPatientCycleSchedulingData(string centerId);
        /// 当前透析方案
        /// </summary>
        /// <returns></returns>
        Task<bool> SynCurrentDialysisProgramData(string centerId);

        /// <summary>
        ///  上机信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SynPreTreatMessageData(string centerId);
        /// <summary>
        /// 透析充分性记录
        /// </summary>
        /// <returns></returns>
        Task<bool> SynDialysisAdequacyRecordData(string centerId);
        /// <summary>
        /// 观察记录
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncObserveRecordData(string centerId);
        /// <summary>
        /// 下机信息
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPostPreTreatMessageData(string centerId);
        /// <summary>
        /// 专用病历（首次门诊病历）
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncFirstOutpatientRecordData(string centerId);
        /// <summary>
        /// 物品库房关联表
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncMaterialTypeWarehouseConfigData();
        /// <summary>
        /// 库房类别
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncWareHouseCatalogsData();
        /// <summary>
        /// 备查库
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncAssetSecondaryStoreListData(string centerId);
        /// <summary>
        /// 转归记录
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncTransferRecordData(string centerId);
        /// <summary>
        /// 护理评估
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncNursingAssessmentRecordData(string centerId);
        /// <summary>
        /// 跌倒评估
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncFallAssessmentAndNursingPlanData(string centerId);
        /// <summary>
        /// 营养评估
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncPatientNutritionAssessmentRecordData(string centerId);
        /// <summary>
        /// 健康宣教
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        Task<bool> SyncPatientHealthEducationRecordsData(string centerId);

        Task<bool> SyncSendSysPublishInfo(SysPublishInfo sysPublishInfo);

        Task<bool> SendPublishNotices(CenterNotices centerNotices);



        //医保目录

        Task<bool> GetYPMLAsync();

        Task<bool> GetZLXMAsync();
        Task<bool> GetSI_BZMLsync();
    }
}
