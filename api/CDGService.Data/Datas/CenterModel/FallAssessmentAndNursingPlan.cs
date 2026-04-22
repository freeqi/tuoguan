using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    public class FallAssessmentAndNursingPlan
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public string CerebralDysfunction { get; set; }
        public decimal? CerebralDysfunctionScore { get; set; }
        public string useDrugs { get; set; }
        public decimal? useDrugsScore { get; set; }
        public string LowPosturalBlood { get; set; }
        public decimal? LowPosturalBloodScore { get; set; }
        public string FallsDropOfBed { get; set; }
        public decimal? FallsDropOfBedScore { get; set; }
        public string Vertigo { get; set; }
        public decimal? VertigoScore { get; set; }
        public string MovementDisorder { get; set; }
        public decimal? MovementDisorderScore { get; set; }
        public string AbnormalDischarge { get; set; }
        public decimal? AbnormalDischargeScore { get; set; }
        public string Age { get; set; }
        public decimal? AgeScore { get; set; }
        public string SensoryDisorder { get; set; }
        public decimal? SensoryDisorderScore { get; set; }
        public string DrugOrAlcoholAbuse { get; set; }
        public decimal? DrugOrAlcoholAbuseScore { get; set; }
        public decimal? EvaluationScore { get; set; }
        public string AssessNurse { get; set; }
        public DateTime? AssessTime { get; set; }
        public string Pager { get; set; }
        public string SafeEnvironment { get; set; }
        public string DrugMission { get; set; }
        public string WarningLabels { get; set; }
        public string MattersNeedingAttention { get; set; }
        public string FamilyMembers { get; set; }
        public string Placard { get; set; }
        public string ProtectiveConstrain { get; set; }
        public string Other { get; set; }
        public string IsFiled { get; set; }
        public DateTime? FiledDate { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
