using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 通风记录
    /// </summary>
    public class GoutRecord
    {
        public string Id { get; set; }
        public DateTime? RecRegistrationDate { get; set; }
        public string GoutParts { get; set; }
        public string GoutDescription { get; set; }
        public string Diagnosis { get; set; }
        public string DiagnosisDoctor { get; set; }
        public string Remark { get; set; }
        public string RecTypeId { get; set; }
        public string PatientId { get; set; }
        public string IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
