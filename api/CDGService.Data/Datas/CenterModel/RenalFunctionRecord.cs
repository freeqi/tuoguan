using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class RenalFunctionRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public decimal? SerumCreatinineBeforeDialysis { get; set; }
        public decimal? SerumCreatinineAfterDialysis { get; set; }
        public string SerumUricAcidBeforeDialysis { get; set; }
        public decimal? SerumUricAcidAfterDialysis { get; set; }
        public string UreaBeforeDialysis { get; set; }
        public decimal? UreaAfterDialysis { get; set; }
        public decimal? SerumMicroglobulinBeforeDialysis { get; set; }
        public decimal? SerumMicroglobulinAfterDialysis { get; set; }
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
