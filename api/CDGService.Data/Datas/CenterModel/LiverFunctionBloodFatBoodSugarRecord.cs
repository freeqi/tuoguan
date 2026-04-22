using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDGService.Data.Datas
{
    public class LiverFunctionBloodFatBoodSugarRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime? CheckDate { get; set; }
        public decimal? TotalSerumProtein { get; set; }
        public string SerumTotalBilirubin { get; set; }
        public decimal? LactateDehydrogenase { get; set; }
        public decimal? IndirectBilirubin { get; set; }
        public decimal? Seroglobulin { get; set; }
        public decimal? GlutamylTransferase { get; set; }
        public string SerumDirectBilirubin { get; set; }
        public string AST_ALT { get; set; }
        public string SerumAsparticAcidTransferase { get; set; }
        public decimal? SerumAlanineTransferase { get; set; }
        public decimal? SerumAlkalinePhosphatase { get; set; }
        public decimal? SerumAlbumin { get; set; }
        public decimal? SerumProalbumin { get; set; }
        public decimal? TotalCholesterol { get; set; }
        public decimal? Triglycerides { get; set; }
        public decimal? HighDensityLipoprotein { get; set; }
        public decimal? LowDensityLipoprotein { get; set; }
        public decimal? BloodSugar { get; set; }
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
