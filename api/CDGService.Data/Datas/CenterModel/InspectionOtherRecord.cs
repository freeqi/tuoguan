using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class InspectionOtherRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public decimal? GlycosylatedHemoglobin { get; set; }
        public decimal? ReactiveProtein { get; set; }
        public decimal? AntistreptococcalHemolysin_O { get; set; }
        public decimal? RheumatoidFactor { get; set; }
        public decimal? CalcitoninOriginal { get; set; }
        public decimal? Homocysteine { get; set; }
        public decimal? NatriureticPeptidePrecursor { get; set; }
        public decimal? AFP { get; set; }
        public decimal? CarcinoembryonicAntigen { get; set; }
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
