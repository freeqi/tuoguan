using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 当前透气方案 -- 透析期间体重增长控制率
    /// </summary>
    public class CurrentDialysisProgram
    {

        public string Id { get; set; }

        public string CenterId { get; set; }

        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public string PatientCycleSchedulingId { get; set; }
        [ForeignKey("PatientCycleSchedulingId")]
        public virtual PatientCycleScheduling patientCycleScheduling { get; set; }
        public string EquipmentId { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual EquipmentInfo  Equipment { get; set; }
        public DateTime? Date { get; set; }
        public string Shift { get; set; }
        public string DialysisType { get; set; }
        public string Dialyzer { get; set; }
        public string DialysisPerfusion { get; set; }
        public int? TreatHour { get; set; }
        public int? TreatMin { get; set; }
        public decimal? BloodFlow { get; set; }
        public decimal? FlowDialy { get; set; }
        public string BloodAccess { get; set; }
        public string Anticoagulants { get; set; }
        public decimal? AnticoagulantsFirstDose { get; set; }
        public decimal? AnticoagulantsBolus { get; set; }
        public string AnticoagulantsUnitId { get; set; }
        public string FillWay { get; set; }
        public decimal? FluidFlow { get; set; }
        public decimal? FluidTotal { get; set; }
        public string BloodSpeed { get; set; }
        public decimal? SequentialDialysisDose { get; set; }
        public int? SequentialDialysisAloneTime { get; set; }
        public string IsSequentialDialysis { get; set; }
        public string IsNoEat { get; set; }
        public decimal? KeepDose { get; set; }
        public string FlowPres_k { get; set; }
        public string FlowPres_na { get; set; }
        public string FlowPres_ga { get; set; }
        public string FlowPres_hq { get; set; }
        public decimal? TxyTemperature { get; set; }
        public string Curve_cl { get; set; }
        public string Curve_na { get; set; }
        public string Curve_zh { get; set; }
        public DateTime? MakeDate { get; set; }
        public string MedPlan { get; set; }
        public string MakeDoctor { get; set; }
        public string EdemaType { get; set; }
        public string GaspType { get; set; }
        public string PrecordialDiscomfortType { get; set; }
        public string HAS_BLED { get; set; }
        public decimal? PreSystolicPressure { get; set; }
        public decimal? PreDiastolicPressure { get; set; }
        public decimal? PrePulse { get; set; }
        public string LastDialysisRemarks { get; set; }
        public string BeforeTreatments { get; set; }
        public string OtherSpecialDiscomfort { get; set; }
        public string OtherSpecialDiscomfortRemarks { get; set; }
        public string OtherTestsAndTreatments { get; set; }
        public string OtherTestsAndTreatmentsRemarks { get; set; }
        public decimal? CurrentDryWeight { get; set; }
        public decimal? PreviousAfterDialysisWeight { get; set; }
        public decimal? BeforeDialysisWeight { get; set; }
        public decimal? ClothingWeight { get; set; }
        public decimal? UltraFilRate { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public decimal? WeightGainRate { get; set; }
        public string SummaryLastTreatment { get; set; }
        public DateTime? CollectData { get; set; }

    }
}
