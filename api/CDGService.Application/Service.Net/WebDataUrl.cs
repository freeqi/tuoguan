using CDGService.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Application.Service.Net
{
    public class WebDataUrl
    {
        public string BaseUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                _baseUrl = value;
                if (!_baseUrl.EndsWith(@"/"))
                    _baseUrl += "/";
            }
        }
        private string _baseUrl = "";//http://192.168.10.231:9003/api/v1/
                                     //http://192.168.10.231:9003/api/v1/
                                     /// <summary>
                                     /// 登录
                                     /// </summary>
                                     /// <param name="account"></param>
                                     /// <param name="pwd"></param>
                                     /// <returns></returns>
        public string UserLoginUrl(string account, string pwd) => $"{_baseUrl}Account/LoginToken/{account}/{pwd}";

        /// <summary>
        /// 获取所有员工信息
        /// </summary>
        public string EmployeeUrl => $"{_baseUrl}UserInfo/4001";
        //PatientInfo/4001


        public string PatientUrl => $"{_baseUrl}PatientInfo/4001";
        //PurchasManager/4004
        /// <summary>
        /// 采购申请单
        /// </summary>
        public string PurchasManagerUrl => $"{_baseUrl}MaterialDataManager/4059";

        /// <summary>
        /// 采购申请单明细
        /// </summary>
        public string PurchaseDetailUrl => $"{_baseUrl}MaterialDataManager/4060";

        //PurchasManager/3004
        /// <summary>
        /// 审核
        /// </summary>
        public string ApprovalPurchaseRequestUrl => $"{_baseUrl}MaterialDataManager/3002";
        /// <summary>
        /// 审核填写物品采购、销售价格
        /// </summary>
        public string PurchasePriceUrl => $"{_baseUrl}MaterialDataManager/3001";
        ///api/v1/PurchasManager/2002	删除采购详情	{"InSumMoney":"","InQty": , "Id":"采购详情id",	"ApplyId": "采购申请id"}

        /// <summary>
        /// 删除采购明细{"InSumMoney":"","InQty": , "Id":"采购详情id",	"ApplyId": "采购申请id"}
        /// </summary>
        public string DelPurchaseDetailsUrl => $"{_baseUrl}MaterialDataManager/2001";
        ///api/v1/PurchasManager/1002
        /// <summary>
        /// 添加采购明 细详情表结构 不传总价（InSumMoney    ）
        /// </summary>
        public string AddPurchaseDetailsUrl => $"{_baseUrl}MaterialDataManager/1001";
        /*
         /api/v1/PurchasManager/3005	
/api/v1/PurchasManager/3006	采购明细闭环	[{"IsClosed":"","Id":""},{"IsClosed":"","Id":""}] 
             */
        /// <summary>
        /// 采购单闭环	[{"IsClosed":"","Id":""},{"IsClosed":"","Id":""}]
        /// </summary>
        public string OrderCloseUrl => $"{_baseUrl}MaterialDataManager/3003";
        /// <summary>
        /// 采购明细闭环	[{"IsClosed":"","Id":""},{"IsClosed":"","Id":""}]
        /// </summary>
        public string OrderDetailCloseUrl => $"{_baseUrl}MaterialDataManager/3004";
        /// <summary>
        /// 推送审批流程至中心端
        /// </summary>
        public string PurchaseApprovesUrl => $"{_baseUrl}MaterialDataManager/3006";
        public string dzPurchaseApprovesUrl => $"{_baseUrl}PurchasManager/3010";

        /// <summary>
        /// 申请单审批部分推送
        /// </summary>
        public string PushPurchaseDetailUrl => $"{_baseUrl}MaterialDataManager/3007";




        /// <summary>
        /// 退货审批
        /// </summary>
        public string ReturnGoodsUrl => $"{_baseUrl}MaterialDataManager/3005";

        /// <summary>
        /// 患者基本信息接口
        /// </summary>
        public string PatientRequestUrl => $"{_baseUrl}MaterialDataManager/4061";
        /// <summary>
        /// 设备基本信息接口
        /// </summary>
        public string EquipmentUrl => $"{_baseUrl}MaterialDataManager/4062";

        //字典类型

        //字典

        #region 财务报表
        /// <summary>
        /// 二级库批次库存信息接口
        /// </summary>
        public string BatchUrl => $"{_baseUrl}MaterialDataManager/4001";

        /// <summary>
        /// 二级库批次流水信息接口
        /// </summary>
        public string BatchDetailUrl => $"{_baseUrl}MaterialDataManager/4002";

        /// <summary>
        /// 一级库批次库存
        /// </summary>
        public string PutInStorageBatchUrl => $"{_baseUrl}MaterialDataManager/4003";

        /// <summary>
        /// 出库信息
        /// </summary>
        public string OutboundUrl => $"{_baseUrl}MaterialDataManager/4004";

        /// <summary>
        /// 出库明细
        /// </summary>
        public string OutboundDetailUrl => $"{_baseUrl}MaterialDataManager/4005";

        /// <summary>
        /// 处方单信息
        /// </summary>
        public string PrescriptionUrl => $"{_baseUrl}MaterialDataManager/4006";

        /// <summary>
        /// 处方明细
        /// </summary>
        public string PrescriptionDetailUrl => $"{_baseUrl}MaterialDataManager/4007";

        /// <summary>
        /// 退费明细
        /// </summary>
        public string RefundUrl => $"{_baseUrl}MaterialDataManager/4008";

        /// <summary>
        /// 入库信息
        /// </summary>
        public string PutInStorageUrl => $"{_baseUrl}MaterialDataManager/4009";

        /// <summary>
        /// 入库明细信息
        /// </summary>
        public string PutInStorageDetailUrl => $"{_baseUrl}MaterialDataManager/4010";

        /// <summary>
        /// 一级库流水
        /// </summary>
        public string PutOutFlowUrl => $"{_baseUrl}MaterialDataManager/4011";

        /// <summary>
        /// 退货
        /// </summary>
        public string ReturnUrl => $"{_baseUrl}MaterialDataManager/4012";

        /// <summary>
        ///退货明细
        /// </summary>
        public string ReturnDetailUrl => $"{_baseUrl}MaterialDataManager/4013";
        #endregion

        #region 设备维修记录
        /// <summary>
        /// 设备维修记录
        /// </summary>
        public string MaintenanceUrl => $"{_baseUrl}MaterialDataManager/4014";
        /// <summary>
        /// 生化检测
        /// </summary>
        public string DetectUrl => $"{_baseUrl}MaterialDataManager/4015";
        /// <summary>
        /// 设备维保记录
        /// </summary>
        public string MaintUrl => $"{_baseUrl}MaterialDataManager/4016";
        #endregion

        #region 门诊病历  病历首页
        /// <summary>
        /// 病历首页
        /// </summary>
        public string MedicalUrl => $"{_baseUrl}MaterialDataManager/4017";
        /// <summary>
        /// 主要诊断
        /// </summary>
        public string DiagnoseUrl => $"{_baseUrl}MaterialDataManager/4018";
        /// <summary>
        /// 血管通路
        /// </summary>
        public string VascularUrl => $"{_baseUrl}MaterialDataManager/4019";
        /// <summary>
        /// 病历首页相关血管通路维修记录
        /// </summary>
        public string VascularMaintaidUrl => $"{_baseUrl}MaterialDataManager/4066";
        /// <summary>
        /// 传染病史
        /// </summary>
        public string InfectiousUrl => $"{_baseUrl}MaterialDataManager/4020";
        /// <summary>
        /// 过敏史
        /// </summary>
        public string AllergyUrl => $"{_baseUrl}MaterialDataManager/4021";
        /// <summary>
        /// 肿瘤
        /// </summary>
        public string TumourUrl => $"{_baseUrl}MaterialDataManager/4022";
        /// <summary>
        /// 门诊病历
        /// </summary>
        public string OutpatientUrl => $"{_baseUrl}MaterialDataManager/4023";
        #endregion

        #region 门诊日志
        /// <summary>
        /// 门诊日志
        /// </summary>
        public string OutpatientLogUrl => $"{_baseUrl}MaterialDataManager/4063";
        #endregion

        #region 阶段小结
        /// <summary>
        /// 阶段小结
        /// </summary>
        public string StageSummaryUrl => $"{_baseUrl}MaterialDataManager/4024";
        #endregion
        #region 干体重
        /// <summary>
        /// 干体重
        /// </summary>
        public string DryWeightSetUrl => $"{_baseUrl}MaterialDataManager/4025";
        #endregion
        #region 检验结果
        /// <summary>
        /// 血常规
        /// </summary>
        public string RoutineBloodUrl => $"{_baseUrl}MaterialDataManager/4026";
        /// <summary>
        /// 电解质七项
        /// </summary>
        public string SevenElectrolyteTermsUrl => $"{_baseUrl}MaterialDataManager/4027";
        /// <summary>
        /// 矿物质及骨代谢
        /// </summary>
        public string MineralAndBoneMetabolismUrl => $"{_baseUrl}MaterialDataManager/4028";
        /// <summary>
        /// 肾功能
        /// </summary>
        public string RenalFunctionRecordUrl => $"{_baseUrl}MaterialDataManager/4029";
        /// <summary>
        /// 肝功，血脂血糖
        /// </summary>
        public string LiverFunctionBloodFatBoodSugarUrl => $"{_baseUrl}MaterialDataManager/4030";
        /// <summary>
        /// 传染病监测
        /// </summary>
        public string InfectiousDiseasesUrl => $"{_baseUrl}MaterialDataManager/4031";
        /// <summary>
        /// 铁参数测定
        /// </summary>
        public string IronParametersUrl => $"{_baseUrl}MaterialDataManager/4032";
        /// <summary>
        /// 心肌酶5项
        /// </summary>
        public string MyocardialEnzymeFiveUrl => $"{_baseUrl}MaterialDataManager/4033";
        /// <summary>
        /// 高血压4项
        /// </summary>
        public string HypertensionFourUrl => $"{_baseUrl}MaterialDataManager/4034";
        /// <summary>
        /// 贫血3项
        /// </summary>
        public string AnemiaThreeUrl => $"{_baseUrl}MaterialDataManager/4035";
        /// <summary>
        /// 凝血5项
        /// </summary>
        public string CoagulationFiveUrl => $"{_baseUrl}MaterialDataManager/4036";
        /// <summary>
        /// 其它
        /// </summary>
        public string InspectionOtheUrl => $"{_baseUrl}MaterialDataManager/4037";
        #endregion

        #region 病史记录
        /// <summary>
        /// 高血压
        /// </summary>
        public string HypertensiveUrl => $"{_baseUrl}MaterialDataManager/4038";
        /// <summary>
        /// 并发症
        /// </summary>
        public string ComplicationsHistoryUrl => $"{_baseUrl}MaterialDataManager/4039";
        /// <summary>
        /// 遗传史
        /// </summary>
        public string GeneticHistorymUrl => $"{_baseUrl}MaterialDataManager/4040";
        /// <summary>
        /// 跌倒史
        /// </summary>
        public string FallHistoryUrl => $"{_baseUrl}MaterialDataManager/4041";
        /// <summary>
        /// 既往史
        /// </summary>
        public string PastHistoryUrl => $"{_baseUrl}MaterialDataManager/4042";
        /// <summary>
        /// 犯罪吸毒记录
        /// </summary>
        public string DrugTakingUrl => $"{_baseUrl}MaterialDataManager/4043";
        /// <summary>
        /// 痛风记录
        /// </summary>
        /// </summary>
        public string GoutUrl => $"{_baseUrl}MaterialDataManager/4044";
        /// <summary>
        /// 药物滥用
        /// </summary>
        public string DrugAbuseUrl => $"{_baseUrl}MaterialDataManager/4045";
        /// <summary>
        /// 肾移植
        /// </summary>
        public string KidneyTransplantUrl => $"{_baseUrl}MaterialDataManager/4046";

        #endregion
        #region 医保相关数据
        /// <summary>
        /// 结算明细表
        /// </summary>
        public string BalanceDetailsUrl => $"{_baseUrl}MaterialDataManager/4047";
        /// <summary>
        /// 费用结算主表
        /// </summary>
        public string BalanceMainUrl => $"{_baseUrl}MaterialDataManager/4048";
        /// <summary>
        /// 费用结算支付方式表
        /// </summary>
        public string BalancePayTypeUrl => $"{_baseUrl}MaterialDataManager/4049";
        /// <summary>
        /// 日结明细表
        /// </summary>
        public string DayBalanceDetailUrl => $"{_baseUrl}MaterialDataManager/4050";
        /// <summary>
        /// 日结主表
        /// </summary>
        /// </summary>
        public string DayBalanceMainUrl => $"{_baseUrl}MaterialDataManager/4051";
        /// <summary>
        /// 上传医保的处方明细记录中间表
        /// </summary>
        public string SI_CFMXUrl => $"{_baseUrl}MaterialDataManager/4052";
        /// <summary>
        /// 医保的结算明细中间表
        /// </summary>
        public string SI_JSMXUrl => $"{_baseUrl}MaterialDataManager/4053";
        /// <summary>
        /// 医保的就诊登记明细中间表
        /// </summary>
        public string SI_JZDJMXUrl => $"{_baseUrl}MaterialDataManager/4054";
        /// <summary>
        /// 医保对照中间表 
        /// </summary>
        public string SICompareUrl => $"{_baseUrl}MaterialDataManager/4055";
        /// <summary>
        /// 透析记录单 
        /// </summary>
        public string HistoryDialysisUrl => $"{_baseUrl}MaterialDataManager/4056";
        /// <summary>
        /// 患者签到 
        /// </summary>
        public string CycleSchedulingUrl => $"{_baseUrl}MaterialDataManager/4057";
        /// <summary>
        /// 当前透析方案
        /// </summary>
        public string DialysisProgramUrl => $"{_baseUrl}MaterialDataManager/4058";

        #endregion
        /// <summary>
        /// 上机信息
        /// </summary>
        public string PreTreatMessageUrl => $"{_baseUrl}MaterialDataManager/4064";
        /// <summary>
        /// 透析充分性记录
        /// </summary>
        public string DialysisAdequacyRecordUrl => $"{_baseUrl}MaterialDataManager/4065";

        /// <summary>
        /// 观察记录
        /// </summary>
        public string ObserveRecordUrl => $"{_baseUrl}MaterialDataManager/4067";
        /// <summary>
        /// 下机信息
        /// </summary>
        public string PostPreTreatMessageUrl => $"{_baseUrl}MaterialDataManager/4068";
        /// <summary>
        /// 专用病历（首次门诊病历）
        /// </summary>
        public string FirstOutpatientUrl => $"{_baseUrl}MaterialDataManager/4069";
        /// <summary>
        /// 物品库房关联表
        /// </summary>
        public string ConfigUrl => $"{_baseUrl}MaterialDataManager/4070";
        /// <summary>
        /// 库房类别
        /// </summary>
        public string WareHouseCatalogUrl => $"{_baseUrl}MaterialDataManager/4071";
        /// <summary>
        /// 备查库
        /// </summary>
        public string AssetSecondaryStoreUrl => $"{_baseUrl}MaterialDataManager/4072";
        /// <summary>
        /// 转归记录
        /// </summary>
        public string TransferRecordUrl => $"{_baseUrl}MaterialDataManager/4073";
        /// <summary>
        /// 护理评估记录
        /// </summary>
        public string NursingAssessmentUrl => $"{_baseUrl}MaterialDataManager/4074";
        /// <summary>
        /// 跌倒评估
        /// </summary>
        public string FallAssessmentUrl => $"{_baseUrl}MaterialDataManager/4075";
        /// <summary>
        /// 营养评估
        /// </summary>
        public string NutritionAssessmentUrl => $"{_baseUrl}MaterialDataManager/4076";
        /// <summary>
        /// 健康宣教
        /// </summary>
        public string HealthEducationtUrl => $"{_baseUrl}MaterialDataManager/4077";

        /// <summary>
        /// 中心端推送
        /// </summary>
        public string CenterPublishVerUrl => $"{_baseUrl}GroupSysPublishManage/1001";

        //通知公告
        /// <summary>
        /// 【集团端】使用【全字段-单条】保存【通知表：Informations】
        /// </summary>
        public string CenterInformationUrl => $"{_baseUrl}GroupAdjustPriceInfoManage/1003";
        /// <summary>
        /// 【集团端】使用【全字段-多条】保存【调价明细表：NoticeMedicals】
        /// </summary>
        public string CenterNoticeMedicalUrl => $"{_baseUrl}GroupAdjustPriceInfoManage/1002";
        /// <summary>
        /// 【集团端】使用【全字段-单条】保存【公告接受人员表：UsersMsg】
        /// </summary>
        public string CenterUsersMsgUrl => $"{_baseUrl}GroupAdjustPriceInfoManage/1001";


        /*
         4078 GetEquipmentSupplier  设备供应商
        4079 GetEquipmentManufacturers 设备生产厂家
        4080 GetEquipmentType  设备类型
        4081 GetEquipmentModel  设备型号
        4082 GetEquipmentBedConfigur  床位信息配置
                 */
        /// <summary>
        /// 设备供应商
        /// </summary>
        public string GetEquipmentSupplierUrl => $"{_baseUrl}GetEquipmentSupplier/4078";
        /// <summary>
        /// 设备厂家
        /// </summary>
        public string GetEquipmentManufacturersUrl => $"{_baseUrl}GetEquipmentManufacturers/4079";
        /// <summary>
        /// 设备类型
        /// </summary>
        public string GetEquipmentTypeUrl => $"{_baseUrl}GetEquipmentType/4080";
        /// <summary>
        /// 设备型号
        /// </summary>
        public string GetEquipmentModelUrl => $"{_baseUrl}GetEquipmentModel/4081";
        /// <summary>
        /// 床位信息配置
        /// </summary>
        public string GetEquipmentBedConfigurUrl => $"{_baseUrl}GetEquipmentBedConfigur/4082";

        //83 YPML  84ZLXM
        /// <summary>
        /// 医保药品
        /// </summary>
        public string GetYPMLUrl => $"{_baseUrl}MaterialDataManager/4083";
        /// <summary>
        /// 医保诊疗项目
        /// </summary>
        public string GetZLXMUrl => $"{_baseUrl}MaterialDataManager/4084";
        /// <summary>
        /// 医保诊疗项目
        /// </summary>
        public string GetBZMLUrl => $"{_baseUrl}MaterialDataManager/4085";




        public string GetMaterialsWarningApplyListUrl => $"{_baseUrl}MaterialDataManager/4086";
        public string GetMaterialsWarningApplyDetailListUrl => $"{_baseUrl}MaterialDataManager/4087";

        public string GetGetMaterialApplyApproveUrl => $"{_baseUrl}MaterialDataManager/4088";
        public string GetMaterialApplyApproveDetailUrl => $"{_baseUrl}MaterialDataManager/4089";
        public string GroupUpdateMatAuditUrl => $"{_baseUrl}MaterialDataManager/3008";

        public string GroupUpdateMaterialsWarningAuditUrl => $"{_baseUrl}MaterialDataManager/3009";
        public string MaterialPendingApprovalUrl => $"{_baseUrl}MaterialDataManager/3010";
        public string WarningPendingApprovalUrl => $"{_baseUrl}MaterialDataManager/3011";
        /// <summary>
        /// 档案
        /// </summary>
        public string MedicalItemRecordsUrl => $"{_baseUrl}MaterialDataManager/4090";
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierUrl => $"{_baseUrl}MaterialDataManager/4091";


        public string SIBaseDataUrl => $"http://172.16.1.111:20231/SI_Directory/UpDic/";      
       /// <summary>
       /// 药品
       /// </summary>
        public string YPUrl => $"http://172.16.1.111:20231/SI_Directory/DownYPDirectory";
        /// <summary>
        /// 耗材
        /// </summary>
        public string ConsumablescUrl => $"http://172.16.1.111:20231/SI_Directory/DownConsumablescDirectory";
        /// <summary>
        /// 服务项目
        /// </summary>
        public string FWXUrl => $"http://172.16.1.111:20231/SI_Directory/DownFWXMDirectory";

        /// <summary>
        /// 限价一类
        /// </summary>
        public string XJUrl => $"http://172.16.1.111:20231/SI_Directory/SIPriceLimitFile";

        public string QueryChronicUrl => $"http://172.16.1.111:20231/SI_Directory/QueryChronic";
    }
}
