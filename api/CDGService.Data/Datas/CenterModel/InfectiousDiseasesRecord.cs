using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class InfectiousDiseasesRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public string HBsAg { get; set; }
        public string HBsAb { get; set; }
        public string HBeAg { get; set; }
        public string HBeAb { get; set; }
        public string HBcAb { get; set; }
        public string HepatitisCAntibody { get; set; }
        public string HIVAntibody { get; set; }
        public string SyphilisAntibody { get; set; }
        public string TRUST { get; set; }
        public string DetectionHospital { get; set; }
        public string Registrar { get; set; }
        public string Remark { get; set; }
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
