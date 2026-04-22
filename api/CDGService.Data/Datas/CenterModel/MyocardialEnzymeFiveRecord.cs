using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class MyocardialEnzymeFiveRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public string SerumAsparticAcidTransferase { get; set; }
        public decimal? CreatineKinase { get; set; }
        public decimal? LactateDehydrogenase { get; set; }
        public decimal? CreatineKinaseIsozyme { get; set; }
        public decimal? HydroxybutyrateDehydrogenase { get; set; }
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
