using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class ObserveRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public string NurseId { get; set; }
        public DateTime? ObserveTime { get; set; }
        public decimal? BloodFlow { get; set; }
        public decimal? SystolicPressure { get; set; }
        public decimal? DiastolicPressure { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Pulse { get; set; }
        public decimal? Heart { get; set; }
        public decimal? Breath { get; set; }
        public decimal? VenousPressure { get; set; }
        public decimal? ArterialPressure { get; set; }
        public decimal? BranePressure { get; set; }
        public decimal? Conductance { get; set; }
        public decimal? ReplaceFluid { get; set; }
        public decimal? UltraFiltration { get; set; }
        public decimal? UltraFilRate { get; set; }
        public string Illness { get; set; }
        public string IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string DialysisId { get; set; }
        [ForeignKey("DialysisId")]
        public virtual HistoryDialysisRecords historyDialysisRecords { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
