using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 透析方案表【DialysisProgram】模型
    /// </summary>
    public class DialysisProgram 
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public string FrequencyId { get; set; }
        public Byte? TreatHour { get; set; }
        public Byte? TreatMin { get; set; }
        public decimal? BloodFlow { get; set; }
        public decimal? FlowDialy { get; set; }
        public string BloodAccess { get; set; }
        public string Anticoagulants { get; set; }
        public decimal? AnticoagulantsFirstDose﻿ { get; set; }
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
        public decimal? KeepDose﻿ { get; set; }
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
        public Byte? IsSetDefaultScheme﻿ { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
