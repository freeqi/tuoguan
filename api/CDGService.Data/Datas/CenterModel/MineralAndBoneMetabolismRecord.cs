using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class MineralAndBoneMetabolismRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public decimal? ParathyroidHormone { get; set; }
        public string BoneAlkalinePhosphatase { get; set; }
        public decimal? Hydroxyvitamin { get; set; }
        public decimal? Calcium { get; set; }
        public decimal? InorganicPhosphorus { get; set; }
        public decimal? CalciumAndPhosphorus { get; set; }
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
