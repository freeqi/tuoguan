using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 门诊血液净化治疗病历首页
    /// </summary>
    public class MedicalHistoryFirstPage
    {

        public string Id { get; set; }
        /// <summary>
        /// 病理诊断
        /// </summary>
        public string PathologicalDiagnosis { get; set; }
        /// <summary>
        /// 病理诊断日期
        /// </summary>
        public DateTime? DiagnosisDate { get; set; }
        /// <summary>
        /// 治疗方式
        /// 共6个选项（HD、HDF、HP、CVVH、SUCF、其他）,可多选，格式形如"HD,HP,SUCF"
        /// </summary>
        public string TreatmentMode { get; set; }
        /// <summary>
        /// 治疗频次
        /// 共6个选项（3次/W、2.5次/W、2次/W、1.5次/W、1次/W、其他），可多选
        /// </summary>
        public string TreatmentFrequency { get; set; }
        /// <summary>
        /// 抗凝剂
        /// 共4个选项（肝素、低分子肝素、无肝素、其他），可多选
        /// </summary>
        public string Anticoagulants { get; set; }
        /// <summary>
        /// 原发疾病
        /// 原发性肾小球疾病/糖尿病/ 高血压病/囊性肾病/慢性间质肾炎/SLE/血管炎肾损害/其他
        /// </summary>
        public string PrimaryDisease { get; set; }
        /// <summary>
        /// 原发疾病其它说明
        /// </summary>
        public string PrimaryDiseaseOther { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public string  PatientId { get; set; }
        /// <summary>
        /// 医生编号
        /// </summary>
        public string DoctorId { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int? DataState { get; set; }

        /// <summary>
        /// 首次肾脏替代治疗备注
        /// </summary>
        public string FirstRenalReplaceTherapyRemark { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
