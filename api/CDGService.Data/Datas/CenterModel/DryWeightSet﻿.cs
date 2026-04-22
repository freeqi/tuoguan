using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    ///患者干体重调整
    /// TableName--DryWeightSet
    /// </summary>
    public class DryWeightSet
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public DateTime? DryWeightSetDate { get; set; }
        public string DryWeight { get; set; }
        public string DryWeightSetReason { get; set; }
        public string DryWeightSetDoctor { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }


    }

}
