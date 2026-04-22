using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 门诊开药明细/门诊划价明细/开立医嘱明细/护士划价明细
    /// TableName--PrescriptionDetail
    /// </summary>
    public class PrescriptionDetail
    {
        public string Id { get; set; }
        public string PrescriptionId { get; set; }
        [ForeignKey("PrescriptionId")]
        public virtual Prescription prescriptions { get; set; }
        public string MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual MedicalItemRecord medicalItemRecord { get; set; }
        public int? MaterialCategory { get; set; }
        public int? MedicalAdviceType { get; set; }
        public string MedicalContent { get; set; }
        public decimal? MaterialTotal { get; set; }
        public string MaterialTotalUnit { get; set; }
        public decimal? SingleDose { get; set; }
        public decimal? MinMeasuringQty { get; set; }
        public string MinMeasuringUnit { get; set; }
        public decimal? PrescribingQty { get; set; }
        public string PrescribingUnit { get; set; }
        public string FrequencyId { get; set; }
        public string UsageId { get; set; }
        [ForeignKey("UsageId")]
        public virtual UseWay useWay { get; set; }
        public string Treatment { get; set; }
        public string SupplierId { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? DiscountStatus { get; set; }
        public decimal? ReceivablePrice { get; set; }
        public string ChargeStatus { get; set; }
        public string PrescriptionMedicalStaff { get; set; }
        [ForeignKey("PrescriptionMedicalStaff")]
        public virtual Employee employee { get; set; }
        public DateTime? PrescriptionDate { get; set; }
        public string NurseId { get; set; }
        [ForeignKey("NurseId")]
        public virtual Employee Nurseemp { get; set; }

        public int? PerformStatus { get; set; }
        public DateTime? PerformDate { get; set; }
        public string CancelPerformReason { get; set; }
        public int? NurseMustStatus { get; set; }
        public int? OutboundStatus { get; set; }
        public string OutboundId { get; set; }
        [ForeignKey("OutboundId")]
        public virtual MaterialOutboundDetail materialOutboundDetail { get; set; }
        public string WeekMedication { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }


}
