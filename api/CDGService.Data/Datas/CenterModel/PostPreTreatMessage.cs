using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class PostPreTreatMessage
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public int? TreatHour { get; set; }
        public int? TreatMin { get; set; }
        public string PostSummary { get; set; }
        public string Post_cll { get; set; }
        public string PostGrume { get; set; }
        [ForeignKey(nameof(PostGrume))]
        public virtual SystemDictionary pPostGrume { get; set; }
        public string DialyserGrume { get; set; }
        [ForeignKey(nameof(DialyserGrume))]
        public virtual SystemDictionary pDialyserGrume { get; set; }
        public string PostBloodAccess { get; set; }
        [ForeignKey(nameof(PostBloodAccess))]
        public virtual SystemDictionary pPostBloodAccess { get; set; }


        public string PostWeight { get; set; }
        public string PostTemperature { get; set; }
        public int? PostPulse { get; set; }
        public int? PostHeart { get; set; }
        public string PostSystolicPressure { get; set; }
        public string PostDiastolicPressure { get; set; }
        public string NurseId { get; set; }
        public DateTime? LeftTime { get; set; }
        public string PunctureAssess { get; set; }
        [ForeignKey(nameof(PunctureAssess))]
        public virtual SystemDictionary pPunctureAssess { get; set; }

        public string PatientComplaint { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public decimal? PostVenousPressure { get; set; }
        public decimal? PostArterialPressure { get; set; }
        public decimal? PostBranePressure { get; set; }
        public decimal? PostConductance { get; set; }
        public decimal? PostBloodFlow { get; set; }
        public decimal? PostUltraFiltration { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
