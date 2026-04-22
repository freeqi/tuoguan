using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 门诊血液净化治疗病历首页-子项目诊断
    /// </summary>
    public class FirstPageItemCurrentDiagnosis
    {

        public string Id { get; set; }
        /// <summary>
        /// 病例首页Id
        /// </summary>
        public string  MedicalHistoryFirstPageId { get; set; }
        /// <summary>
        /// 目前诊断
        /// </summary>
        public string CurrentDiagnosis { get; set; }
        /// <summary>
        /// 诊断日期
        /// </summary>
        public DateTime? CurrentDiagnosisDate { get; set; }

        public string DoctorSign { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
        public string SortNo { get; set; }
        public string ICDCode { get; set; }
    }

    /// <summary>
    /// 血管通路病历记录
    /// </summary>
    public class VascularAccessRecord
    {
        public string Id { get; set; }
        /// <summary>
        /// 记录登记时间
        /// </summary>
        public DateTime? RecRegistrationDate { get; set; }
        public string VascularPathwayType { get; set; }
        public string Position { get; set; }
        public string LeftOrRight { get; set; }
        public string NewHospital { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 病历记录类型
        /// （数据字典表id）1 过敏; 2 传染病；3 高血压；4 遗传史；5 血管通路；6 跌倒；
        /// 7 既往史；8 并发症；9 吸毒；10 肿瘤；11 痛风；12 用药； 13 肾移植
        /// </summary>
        public string RecTypeId { get; set; }

        public string PatientId { get; set; }
        public string IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }


    }

    /// <summary>
    /// 传染病史登记
    /// </summary>
    public class InfectiousDiseaseRegister
    {

        public string Id { get; set; }
        /// <summary>
        /// 记录登记时间
        /// </summary>
        public DateTime? RecRegistrationDate { get; set; }
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
        public string  RecTypeId { get; set; }
        /// <summary>
        /// 患者ID
        /// </summary>
        public string  PatientId { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public string  IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态 1启用、2停用、3删除
        /// </summary>
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }

    /// <summary>
    /// 过敏（史）登记
    /// </summary>
    public class AllergyRegister
    {
        public string Id { get; set; }
        /// <summary>
        /// 记录登记时间
        /// </summary>
        public DateTime? RecRegistrationDate { get; set; }
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
        public string RecTypeId { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public string  PatientId { get; set; }
        public string  IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// 1启用、2停用、3删除
        /// </summary>
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }


    }

    /// <summary>
    /// 肿瘤登记
    /// </summary>
    public class TumorRegister
    {

        public string Id { get; set; }
        public DateTime? RecRegistrationDate { get; set; }
        public string DiagnosticSite { get; set; }
        public string PathologicalType { get; set; }
        public string DiagnosticBasis { get; set; }
        public string Remark { get; set; }
        public string RecTypeId { get; set; }
        public string PatientId { get; set; }
        public string IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }

}
