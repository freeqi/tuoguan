using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 健康宣教
    /// </summary>
    public class PatientHealthEducationRecord
    {

        public string Id { get; set; }
        public string PublicityEduType { get; set; }
        public string AdmissionEvaluation { get; set; }
        public string PublicityEduContent { get; set; }
        public string EvaluationEffect { get; set; }
        public string EvaluationEffectAgain { get; set; }
        public string Diagnosis { get; set; }
        public string NurseSign { get; set; }
        public string PatientOrRelationSign { get; set; }
        public string PatientId { get; set; }
        public DateTime? PublicityEduDate { get; set; }
        public string GroupId { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public DateTime? VerbDate { get; set; }
        public DateTime? VerbDateAgain { get; set; }
        public string IsFiled { get; set; }


        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
