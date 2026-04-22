using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 门诊开药/门诊划价/开立医嘱/护士划价
    /// TableName--Prescription
    /// </summary>
    public class Prescription
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public string PrescriptionNo { get; set; }
        public int? PrescriptionType { get; set; }
        public decimal? ReceivablePrice { get; set; }
        public decimal? ActualPrice { get; set; }
        public int? MedicalAdviceType { get; set; }
        public int? ChargeType { get; set; }
        public string ChargeStatus { get; set; }
        public string ChargePerson { get; set; }
        public DateTime? ChargeDate { get; set; }
        public string DialysisId { get; set; }

        /// <summary>
        /// 透析记录单
        /// </summary>
        [ForeignKey("DialysisId")]
        public virtual HistoryDialysisRecords historyDialysisRecords { get; set; }
        public string PrescriptionMedicalStaff { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

        /// <summary>
        /// 处方明细
        /// </summary>
        public virtual List<PrescriptionDetail> PrescriptionDetailsP { get; set; }
    }
}
