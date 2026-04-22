using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 肾移植记录
    /// </summary>
    public class KidneyTransplantRecord
    {
        public string Id { get; set; }
        public DateTime? RecRegistrationDate { get; set; }
        public DateTime? TransplantationDate { get; set; }
        public string Position { get; set; }
        public string Source { get; set; }
        public string AdverseReactions { get; set; }
        public string IsNecrosis { get; set; }
        public DateTime? NecrosisDate { get; set; }
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
