
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class RoutineBloodRecord 
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        
        public decimal? Hemoglobin { get; set; }
        public decimal? PlateletCount { get; set; }
        public decimal? EosinophilPercentage { get; set; }
        public decimal? EosinophilAbsoluteValue { get; set; }
        public decimal? BasophilsPercentage { get; set; }
        public decimal? HemoglobinConcentration { get; set; }
        public decimal? HemoglobinContent { get; set; }
        public decimal? ErythrocyteVolume { get; set; }
        public decimal? LymphocyteAbsolute { get; set; }
        public decimal? LymphocytesPercentage { get; set; }
        public decimal? Hematocrit { get; set; }
        public decimal? ErythrocyteCount { get; set; }
        public decimal? ErythrocyteWidth { get; set; }
        public decimal? ErythrocyteWidthVariation { get; set; }
        public decimal? MonocytesAbsolute { get; set; }
        public decimal? MonocytesPercentage { get; set; }
        public decimal? MacroplateletRatio { get; set; }
        public decimal? LeukocyteCount { get; set; }
        public decimal? NeutrophilAbsolute { get; set; }
        public decimal? NeutrophilPercentage { get; set; }
        public decimal? PlateletHyperplasia { get; set; }
        public decimal? MeanPlateletVolume { get; set; }
        public decimal? PlateletWidth { get; set; }
        public decimal? EosinophilsAbsolute { get; set; }
        public string CheckResultImage { get; set; }
        public string Institutions { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
