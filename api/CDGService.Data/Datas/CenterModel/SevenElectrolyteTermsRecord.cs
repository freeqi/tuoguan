using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class SevenElectrolyteTermsRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public decimal? InorganicPhosphorus { get; set; }
        public decimal? Magnesium { get; set; }
        public decimal? Potassium { get; set; }
        public decimal? Chlorine { get; set; }
        public decimal? Sodium { get; set; }
        public decimal? Calcium { get; set; }
        public decimal? CarbonDioxide { get; set; }
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
