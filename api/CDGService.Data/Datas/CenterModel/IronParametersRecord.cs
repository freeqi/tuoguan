using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class IronParametersRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public decimal? SerumIronIon { get; set; }
        public string Ferritin { get; set; }
        public decimal? TotalIronBindingForce { get; set; }
        public decimal? Transferrin { get; set; }
        public decimal? TransferrinSaturation { get; set; }
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
