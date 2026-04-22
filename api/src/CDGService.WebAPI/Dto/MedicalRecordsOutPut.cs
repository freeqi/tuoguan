using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 门诊病历首页
    /// </summary>

    public class MedicalRecordsOutPut
    {
        /// <summary>
        /// 患者基本信息
        /// </summary>
        public PatientOutPut HomePageInfo { get; set; }

        /// <summary>
        /// 门诊血液净化治疗病历首页
        /// </summary>
        public MedicalHistoryFirstOutPut medicalHistoryFirst { get; set; }
        /// <summary>
        /// 诊血液净化治疗病历首页-子项目诊断
        /// </summary>
        public FirstPageItemCurrentDiagnosisOutPut[] firstPageItemCurrentDiagnosis { get; set; }
        /// <summary>
        /// 血管通路病历记录
        /// </summary>
        public VascularAccessRecordOutPut[] vascularAccessRecord { get; set; }
        /// <summary>
        /// 传染病史登记
        /// </summary>
        public InfectiousDiseaseRegisterOutPut[] infectiousDiseaseRegister { get; set; }
        /// <summary>
        /// 过敏（史）登记
        /// </summary>
        public AllergyRegisterOutPut[] allergyRegister { get; set; }
        /// <summary>
        /// 肿瘤登记
        /// </summary>
        public TumorRegisterOutPut[] tumorRegister { get; set; }
    }

    public class MedicalHistoryFirstOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 病理诊断
        /// </summary>
        public string PathologicalDiagnosis { get; set; }
        /// <summary>
        /// 病理诊断日期
        /// </summary>
        public string DiagnosisDate { get; set; }
        /// <summary>
        /// 治疗方式
        /// 共6个选项（HD、HDF、HP、CVVH、SUCF、其他）,可多选，格式形如"HD,HP,SUCF"
        /// </summary>
        public List<string> TreatmentMode { get; set; }
        /// <summary>
        /// 治疗频次
        /// 共6个选项（3次/W、2.5次/W、2次/W、1.5次/W、1次/W、其他），可多选
        /// </summary>
        public List<string> TreatmentFrequency { get; set; }
        /// <summary>
        /// 抗凝剂
        /// 共4个选项（肝素、低分子肝素、无肝素、其他），可多选
        /// </summary>
        public List<string> Anticoagulants { get; set; }
        /// <summary>
        /// 原发疾病
        /// 原发性肾小球疾病/糖尿病/ 高血压病/囊性肾病/慢性间质肾炎/SLE/血管炎肾损害/其他
        /// </summary>
        public List<string> PrimaryDisease { get; set; }
        /// <summary>
        /// 原发疾病其它说明
        /// </summary>
        public string PrimaryDiseaseOther { get; set; }
        /// <summary>
        /// 首次肾脏替代治疗备注
        /// </summary>
        public string FirstRenalReplaceTherapyRemark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// 医生编号
        /// </summary>
        public int DoctorId { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }

    }

    /// <summary>
    /// 门诊血液净化治疗病历首页-子项目诊断
    /// </summary>
    public class FirstPageItemCurrentDiagnosisOutPut
    {

        public string Id { get; set; }
        /// <summary>
        /// 病例首页Id
        /// </summary>
        public int MedicalHistoryFirstPageId { get; set; }
        /// <summary>
        /// 目前诊断
        /// </summary>
        public string CurrentDiagnosis { get; set; }
        /// <summary>
        /// 诊断日期
        /// </summary>
        public string CurrentDiagnosisDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
    }

    /// <summary>
    /// 血管通路病历记录
    /// </summary>
    public class VascularAccessRecordOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 记录登记时间
        /// </summary>
        public string RecRegistrationDate { get; set; }
        /// <summary>
        /// 部位
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 左右
        /// </summary>
        public string LeftOrRight { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 病历记录类型
        /// （数据字典表id）1 过敏; 2 传染病；3 高血压；4 遗传史；5 血管通路；6 跌倒；
        /// 7 既往史；8 并发症；9 吸毒；10 肿瘤；11 痛风；12 用药； 13 肾移植
        /// </summary>
        public int RecTypeId { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public bool IsFiled { get; set; }
        /// <summary>
        /// 1启用、2停用、3删除
        /// </summary>
        public int DataState { get; set; }

    }

    /// <summary>
    /// 传染病史登记
    /// </summary>
    public class InfectiousDiseaseRegisterOutPut
    {

        public string Id { get; set; }
        /// <summary>
        /// 记录登记时间
        /// </summary>
        public string RecRegistrationDate { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 病历记录类型
        /// （数据字典表id）1 过敏; 2 传染病；3 高血压；4 遗传史；5 血管通路；6 跌倒；
        /// 7 既往史；8 并发症；9 吸毒；10 肿瘤；11 痛风；12 用药； 13 肾移植
        /// </summary>
        public int RecTypeId { get; set; }
        /// <summary>
        /// 患者ID
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public bool IsFiled { get; set; }

        /// <summary>
        /// 数据状态 1启用、2停用、3删除
        /// </summary>
        public int DataState { get; set; }

    }

    /// <summary>
    /// 过敏（史）登记
    /// </summary>
    public class AllergyRegisterOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 记录登记时间
        /// </summary>
        public string RecRegistrationDate { get; set; }
        /// <summary>
        /// 过敏内容
        /// </summary>
        public string AllergicContent { get; set; }
        /// <summary>
        /// 治疗经过
        /// </summary>
        public string TreatmentProcess { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 病历记录类型
        /// （数据字典表id）1 过敏; 2 传染病；3 高血压；4 遗传史；5 血管通路；6 跌倒；
        /// 7 既往史；8 并发症；9 吸毒；10 肿瘤；11 痛风；12 用药； 13 肾移植
        /// </summary>
        public int RecTypeId { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public bool IsFiled { get; set; }

        /// <summary>
        /// 数据状态
        /// 1启用、2停用、3删除
        /// </summary>
        public int DataState { get; set; }


    }

    /// <summary>
    /// 肿瘤登记
    /// </summary>
    public class TumorRegisterOutPut
    {

        public string Id { get; set; }
        /// <summary>
        /// 记录登记时间
        /// </summary>
        public string RecRegistrationDate { get; set; }
        /// <summary>
        /// 诊断部位
        /// </summary>
        public string DiagnosticSite { get; set; }
        /// <summary>
        /// 病理学类型
        /// </summary>
        public string PathologicalType { get; set; }
        /// <summary>
        /// 诊断依据
        /// </summary>
        public string DiagnosticBasis { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 病历记录类型
        /// （数据字典表id）1 过敏; 2 传染病；3 高血压；4 遗传史；5 血管通路；6 跌倒；
        /// 7 既往史；8 并发症；9 吸毒；10 肿瘤；11 痛风；12 用药； 13 肾移植
        /// </summary>
        public int RecTypeId { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public bool IsFiled { get; set; }

        /// <summary>
        /// 1启用、2停用、3删除
        /// </summary>
        public int DataState { get; set; }
    }




    public class OutpatientRecordOutPut
    {
        /// <summary>
        /// 患者基本信息
        /// </summary>
        public PatientOutPut HomePageInfo { get; set; }
        public FirstOutpatientRecordOutPut firstOutpatientRecordOutPut { get; set; }
    }
    /// <summary>
    /// 专用病历
    /// </summary>
    public class FirstOutpatientRecordOutPut
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        /// <summary>
        /// 病史陈述者
        /// </summary>
        public string Representor { get; set; }
        /// <summary>
        /// 与患者关系
        /// </summary>
        public string Relation { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public string RecordingTime { get; set; }
        /// <summary>
        /// 主诉
        /// </summary>
        public string ChiefComplaint { get; set; }
        /// <summary>
        /// 现病史
        /// </summary>
        public string PresentIllness { get; set; }
        /// <summary>
        /// 通路建立情况
        /// </summary>
        public string Passage { get; set; }

        public string Occlusion { get; set; }
        public int? OcclusionNumber { get; set; }
        public string IsIntubation { get; set; }
        public int? IntubationNumber { get; set; }
        public string FirstPassage { get; set; }
        public int? HDNumber { get; set; }
        public string IsHP { get; set; }
        public string IsHDF { get; set; }
        public string OtherTreat { get; set; }
        public string DrugsAndUse { get; set; }
        public string PhysicalPower { get; set; }
        public string Appetite { get; set; }
        public string Sleep { get; set; }
        public string IsFever { get; set; }
        public string IsBleeding { get; set; }
        public string IsNausea { get; set; }
        public string IsChestDistress { get; set; }
        public string IsItchySkin { get; set; }
        public float? UrineUtput { get; set; }
        public int? NightUrineNumber { get; set; }
        public int? ShitNumber { get; set; }
        public string MedicalHistory { get; set; }
        public string CurrentSituation { get; set; }
        public string IsDiabetes { get; set; }
        public string IsCHD { get; set; }
        public string IsHypertension { get; set; }
        public string PastHistory { get; set; }
        public string IsContagion { get; set; }
        public string ContagionSituation { get; set; }
        public string IsAllergy { get; set; }
        public string AllergySituation { get; set; }
        public string IsSurgicalTrauma { get; set; }
        public string TraumaSituation { get; set; }
        public string IsSmoking { get; set; }
        public string SmokeYears { get; set; }
        public int? SmokeBranch { get; set; }
        public string IsQuitSmoking { get; set; }
        public string IsAlcoholAbuse { get; set; }
        public string AlcoholAbuseYears { get; set; }
        public float? AmountOfAlcohol { get; set; }
        public string IsStopDrinking { get; set; }
        public string IsDrugUse { get; set; }
        public string DrugUseRemark { get; set; }
        public string DrugUseOther { get; set; }
        public string FamilyHistory { get; set; }
        public string FamilyRemak { get; set; }
        public string Signature { get; set; }
        public float? T { get; set; }
        public int? P { get; set; }
        public int? R { get; set; }
        public float? BP { get; set; }
        public string ConsciousState { get; set; }
        public string IsBodyCoopera { get; set; }
        public string IsQuestion { get; set; }
        public string Position { get; set; }
        public string IsBleeder { get; set; }
        public string IsJugularPhlebitis { get; set; }
        public string IsGoitre { get; set; }
        public int? HeartRate { get; set; }
        public string RhythmOfTheHeart { get; set; }
        public string Noise { get; set; }
        public string NoiseRmark { get; set; }
        public string HeartWorld { get; set; }
        public string Breathing { get; set; }
        public string Ale { get; set; }
        public string lungRemark { get; set; }
        public string Abdominal { get; set; }
        public string Tenderness { get; set; }
        public string TendernessRemark { get; set; }
        public string RefluxSign { get; set; }
        public string Murphy { get; set; }
        public string SerousCavityEffusion { get; set; }
        public string CavityAmount { get; set; }
        public string Parts { get; set; }
        public string Edema { get; set; }
        public string EdemaAmount { get; set; }
        public string InternalFistula { get; set; }
        public string Filling { get; set; }
        public string Mature { get; set; }
        public string Auscultation { get; set; }
        public string OtherPositiveSigns { get; set; }
        public string RoutineBloodTime { get; set; }
        public float? Hb { get; set; }
        public float? HCT { get; set; }
        public float? WBC { get; set; }
        public float? N { get; set; }
        public float? PLT { get; set; }
        public string HembiochemicalTime { get; set; }
        public float? BUN { get; set; }
        public float? Scr { get; set; }
        public float? UA { get; set; }
        public float? K { get; set; }
        public float? Na { get; set; }
        public float? Cl { get; set; }
        public float? CO2CP { get; set; }
        public float? Ca2 { get; set; }
        public float? P3 { get; set; }
        public float? iPTH { get; set; }
        public string ContagionTime { get; set; }
        public string HBsAg { get; set; }
        public string HBsAb { get; set; }
        public string HBeAg { get; set; }
        public string HBeAb { get; set; }
        public string TP { get; set; }
        public string HIV { get; set; }
        public string HCV { get; set; }
        public string DoubleRenalUtrasound { get; set; }
        public string CheckRemark { get; set; }
        public string CaseAbstract { get; set; }
        public string SupplementaryDiagnosis { get; set; }
        public string Diagnosis { get; set; }
        public string DiagnosisDoctor { get; set; }
        public string DiagnosYear { get; set; }
        public string DiagnosMonth { get; set; }
        public string DiagnosisDate { get; set; }
        public string DiagnosticBasis { get; set; }
        public string TreatmentPlan { get; set; }
        public string DiagnosisTreatDoctor { get; set; }
        public string DiagnosisTreatYear { get; set; }
        public string DiagnosisTreatMonth { get; set; }
        public string DiagnosisTreatDate { get; set; }
        public string IsFiled { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public string FounderDate { get; set; }
        public string Modifier { get; set; }
        public string ModifierDate { get; set; }
        public int? DataState { get; set; }
    }



    public class POutpatientMedicalRecordOutPut
    {
        /// <summary>
        /// 患者基本信息
        /// </summary>
        public PatientOutPut HomePageInfo { get; set; }
        public List<string> DateList { get; set; } = new List<string>();
        /// <summary>
        /// 门诊病例
        /// </summary>
        public OutpatientMedicalRecordOutPut[] outpatientMedicalRecord { get; set; }

    }

    /// <summary>
    /// 门诊病历
    /// </summary>
    public class OutpatientMedicalRecordOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 科别
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 科室
        /// </summary>
        public string Section { get; set; }
        public int  PatientId { get; set; }
        public int  DoctorId { get; set; }
        /// <summary>
        /// 就诊日期
        /// </summary>
        public string TreatmentDate { get; set; }
        /// <summary>
        /// 主诉
        /// </summary>
        public string PrincipleAction { get; set; }
        /// <summary>
        /// 目前干体重
        /// </summary>
        public double? CurrentDryWeight { get; set; }
        /// <summary>
        /// 次透后体重
        /// </summary>
        public double? WeightAfterSecondaryDialysis { get; set; }
        /// <summary>
        /// 本次透前体重
        /// </summary>
        public double? DryWeightBeforeDialysis { get; set; }
        /// <summary>
        /// 衣物变化
        /// </summary>
        public double? ClothingChange { get; set; }
        /// <summary>
        /// 体重增长
        /// </summary>
        public double? WeightGain { get; set; }
        /// <summary>
        /// 出血倾向
        /// </summary>
        public string BleedingTendency { get; set; }
        /// <summary>
        /// 恶心呕吐
        /// </summary>
        public string NauseaVomitingBlood { get; set; }
        /// <summary>
        /// 胸闷气促
        /// </summary>
        public string ChestTightnessShortnessOfBreath { get; set; }
        /// <summary>
        /// 皮肤瘙痒
        /// </summary>
        public string SkinItch { get; set; }
        /// <summary>
        /// 尿量
        /// </summary>
        public double? UrineVolume { get; set; }
        /// <summary>
        /// 大便次数
        /// </summary>
        public int? DefecationFrequency { get; set; }
        /// <summary>
        /// 现病史其他
        /// </summary>
        public string PresentHistoryOther { get; set; }
        /// <summary>
        /// 合并症
        /// </summary>
        public string Complication { get; set; }
        /// <summary>
        /// 既往史
        /// </summary>
        public string PreviousHistory { get; set; }
        /// <summary>
        /// 体温
        /// </summary>
        public double? T { get; set; }
        /// <summary>
        /// 脉搏
        /// </summary>
        public int? P { get; set; }
        /// <summary>
        /// 呼吸
        /// </summary>
        public int? R { get; set; }
        /// <summary>
        /// 收缩压
        /// </summary>
        public double? SystolicPressure { get; set; }
        /// <summary>
        /// 舒张压
        /// </summary>
        public double? DiastolicPressure { get; set; }
        /// <summary>
        /// 意识状态
        /// </summary>
        public string ConsciousState { get; set; }
        /// <summary>
        /// 问答切题
        /// </summary>
        public string QAQuestion { get; set; }
        /// <summary>
        /// 问答切题
        /// </summary>
        public string Posture { get; set; }
        /// <summary>
        /// 颈静脉怒张
        /// </summary>
        public string JugularVein { get; set; }
        /// <summary>
        /// 心率
        /// </summary>
        public int? HeartRate { get; set; }
        /// <summary>
        /// 心律
        /// </summary>
        public string RhythmHeart { get; set; }
        /// <summary>
        /// 肺部呼吸状态
        /// </summary>
        public string RespiratoryState { get; set; }
        /// <summary>
        /// 下肢水肿
        /// </summary>
        public string LowerExtremityEdema { get; set; }
        /// <summary>
        /// 其他阳性体征
        /// </summary>
        public string PositiveSignsOther { get; set; }
        /// <summary>
        /// 辅助检查
        /// </summary>
        public string AccessoryExamination { get; set; }
        /// <summary>
        /// 诊断
        /// </summary>
        public string Diagnose { get; set; }
        /// <summary>
        /// 其他记录
        /// </summary>
        public string OtherRecords { get; set; }
        /// <summary>
        /// 治疗方案调整记录
        /// </summary>
        public string TreatmentPlanAdjustmentRecords { get; set; }
        /// <summary>
        /// 补充诊断记录
        /// </summary>
        public string SupplementaryDiagnosticRecords { get; set; }
        /// <summary>
        /// 门诊其他记录医生签字
        /// </summary>
        public string OtherRecordsDoctorSign { get; set; }
        /// <summary>
        /// 医疗机构编码
        /// </summary>
        public string MedicalInstitutionsCode { get; set; }       
        public int? DataState { get; set; }

        //
        /// <summary>
        /// 透析模式
        /// </summary>
        public string DialysisType { get; set; }
        /// <summary>
        /// 透析器（滤过器）
        /// </summary>
        public string Dialyzer { get; set; }
        /// <summary>
        /// 灌流器
        /// </summary>
        public string DialysisPerfusion { get; set; }
        //透析处方
        /// <summary>
        /// 小时
        /// </summary>
        public int Hours { get; set; }
        /// <summary>
        /// 分钟
        /// </summary>
        public int Mins { get; set; }
        /// <summary>
        /// 超滤量
        /// </summary>
        public double ultrafiltration { get; set; }
        /// <summary>
        /// 抗凝剂
        /// </summary>
        public double anticoagulants { get; set; }
        /// <summary>
        /// 追加
        /// </summary>
        public double Zjanticoagulants { get; set; }
    }


}
