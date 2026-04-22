using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 门诊病历
    /// </summary>
    public class OutpatientMedicalRecord
    {
        public string Id { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string PatientId { get; set; }
        public string DoctorSign { get; set; }
        public DateTime? TreatmentDate { get; set; }
        public string PrincipleAction { get; set; }
        public decimal? CurrentDryWeight { get; set; }
        public decimal? WeightAfterSecondaryDialysis { get; set; }
        public decimal? DryWeightBeforeDialysis { get; set; }
        public decimal? ClothingChange { get; set; }
        public decimal? WeightGain { get; set; }
        public string BleedingTendency { get; set; }
        public string NauseaVomitingBlood { get; set; }
        public string ChestTightnessShortnessOfBreath { get; set; }
        public string SkinItch { get; set; }
        public decimal? UrineVolume { get; set; }
        public int? DefecationFrequency { get; set; }
        public string PresentHistoryOther { get; set; }
        public string Complication { get; set; }
        public string PreviousHistory { get; set; }
        public decimal? T { get; set; }
        public int? P { get; set; }
        public int? R { get; set; }
        public decimal? SystolicPressure { get; set; }
        public decimal? DiastolicPressure { get; set; }
        public string ConsciousState { get; set; }
        public string QAQuestion { get; set; }
        public string Posture { get; set; }
        public string JugularVein { get; set; }
        public int? HeartRate { get; set; }
        public string RhythmHeart { get; set; }
        public string RespiratoryState { get; set; }
        public string LowerExtremityEdema { get; set; }
        public string PositiveSignsOther { get; set; }
        public string AccessoryExamination { get; set; }
        public string Diagnose { get; set; }
        public string OtherRecords { get; set; }
        public string TreatmentPlanAdjustmentRecords { get; set; }
        public string SupplementaryDiagnosticRecords { get; set; }
        public string MedicalInstitutionsCode { get; set; }
        public string OtherRecordsDoctorSign { get; set; }
        public string ConsultationType { get; set; }
        public DateTime? ConsultationAppDate { get; set; }
        public string ConsultationDescription { get; set; }
        public string BeInvitedPhysician { get; set; }
        public string ApplyConsultingPhysician { get; set; }
        public string ConsultationOpinions { get; set; }
        public DateTime? ConsultationDate { get; set; }
        public string ConsultingPhysician { get; set; }
        public string JoinMedicalInstitutionName { get; set; }
        public string ClinicalDiagnosis { get; set; }
        public string RescueRecord { get; set; }
        public string Recorder { get; set; }
        public string JoinRescuePersonOrTitle { get; set; }
        public DateTime? RescueStartDate { get; set; }
        public DateTime? RescueEndDate { get; set; }
        public string RescueEffectAndFamilyComm { get; set; }
        public string IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string DialysisType { get; set; }
        public string Dialyzer { get; set; }
        public string DialysisPerfusion { get; set; }
        public int? TreatHour { get; set; }
        public int? TreatMin { get; set; }
        public decimal? UltraFilRate { get; set; }
        public string Anticoagulants { get; set; }
        public decimal? AnticoagulantsFirstDose { get; set; }
        public decimal? AnticoagulantsBolus { get; set; }
        public string AnticoagulantsUnitId { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }


    }
}
