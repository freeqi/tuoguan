using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 高血压记录
    /// </summary>
    public class HypertensiveRecord
    {
        public string Id { get; set; }
        public DateTime? RecRegistrationDate { get; set; }
        public decimal? SystolicPressure { get; set; }
        public decimal? DiastolicPressure { get; set; }
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
