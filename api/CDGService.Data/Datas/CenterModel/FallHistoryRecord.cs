using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 跌倒史记录
    /// </summary>
    public class FallHistoryRecord
    {
        public string Id { get; set; }
        public DateTime? RecRegistrationDate { get; set; }
        public string FallDescribe { get; set; }
        public string FallReason { get; set; }
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
