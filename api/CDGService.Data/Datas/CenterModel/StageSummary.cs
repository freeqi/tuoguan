using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 阶段小结-尿素清除指数（KT/V）、尿素下降率（URR）记录完成率
    /// </summary>
    public class StageSummary
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public virtual Patient patient { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PhaseInfo { get; set; }
        public string Anticoagulants { get; set; }
        public string BloodPre { get; set; }
        public string BloodIn { get; set; }
        public string BloodAfter { get; set; }
        public string BloodAccess { get; set; }
        public string NoiseSound { get; set; }
        public string NoiseSharp { get; set; }
        public string NoiseTremor { get; set; }
        public string UseTime { get; set; }
        public string UseEvaluate { get; set; }
        public string TxComplication { get; set; }
        public string ToxuriaComplication { get; set; }
        public string ToxuriaPosition { get; set; }
        public string ToxuriaSpecific { get; set; }
        public string PhysicalCheck { get; set; }
        public string AccessoryExamination { get; set; }
        public string DoctorDialysis { get; set; }
        public string DoctorTemporary { get; set; }
        public string DoctorPrescribe { get; set; }
        public string FullClinical { get; set; }
        public string FullDryweight { get; set; }
        public string FullAnemia { get; set; }
        public string FullBlood { get; set; }
        public string FullWeightgrow { get; set; }
        public string ReturnSociety { get; set; }
        public string FullUrr { get; set; }
        public string FullDyn { get; set; }
        public string FullTACurea { get; set; }
        public string Diagnosis { get; set; }
        public string TreatPlan { get; set; }
        public string DoctorId { get; set; }
        public int? DocumentState { get; set; }
        public DateTime? DocumentDateTime { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string SupplementaryDiagnosis { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }
}
