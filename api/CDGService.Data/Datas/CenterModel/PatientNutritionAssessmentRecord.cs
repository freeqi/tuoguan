using System;
namespace CDGService.Data.Datas
{
    public class PatientNutritionAssessmentRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public decimal ? CurrentWeight { get; set; }
        public decimal ? Weight2WeeksAgo { get; set; }
        public decimal ? Weight6MonthsAgo { get; set; }
        public string WeightChangeWithin6Months { get; set; }
        public string WeightChangeIn2Weeks { get; set; }
        public string FeedingChange { get; set; }
        public string FeedingChangeTime { get; set; }
        public string FoodType { get; set; }
        public string GastrointestinalSymptoms { get; set; }
        public string IsDoBeforeThings { get; set; }
        public string LowerEyeAndFace { get; set; }
        public string BicepsTriceps { get; set; }
        public string Tempus { get; set; }
        public string Clavicle { get; set; }
        public string Shoulder { get; set; }
        public string Scapula { get; set; }
        public string Knee { get; set; }
        public string GastrocnemiusMuscle { get; set; }
        public string Edema { get; set; }
        public string Ascites { get; set; }
        public string SGA { get; set; }
        public string NutritionAssessment { get; set; }
        public int? IsInfoDoctor { get; set; }
        public string InfoDoctorDescription { get; set; }
        public string EvaluationDoctorSignature { get; set; }
        public DateTime? EvaluationDate { get; set; }
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
