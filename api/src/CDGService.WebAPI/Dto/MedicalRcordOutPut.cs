using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 门诊开药/门诊划价/开立医嘱/护士划价
    /// </summary>
    public class PrescriptionViewModel
    {
        public string Id { get; set; }
        public string PatientId { get; set; }

        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string PrescriptionNo { get; set; }
        public int? PrescriptionType { get; set; }
        public int? MedicalAdviceType { get; set; }
        public decimal? ReceivablePrice { get; set; }
        public decimal? ActualPrice { get; set; }
        public string ChargeStatus { get; set; }
        public string ChargePerson { get; set; }
        public string DialysisId { get; set; }
        public string ChargePersonName { get; set; }
        public DateTime? ChargeDate { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public int? ChargeType { get; set; }
        public string Sex { get; set; }
        //public List<PrescriptionDetailViewModel> PrescriptionDetailList { get; set; }
    }
    public class PrescriptionDetailViewModel
    {
        public string Id { get; set; }
        public string PrescriptionId { get; set; }
        public string PrescriptionNo { get; set; }
        public string MaterialId { get; set; }
        public string MaterialName { get; set; }
        public int? MaterialCategory { get; set; }
        public int? MedicalAdviceType { get; set; }
        public string MedicalContent { get; set; }
        public decimal? MaterialTotal { get; set; }
        public string MaterialTotalUnit { get; set; }
        public string MaterialTotalUnitName { get; set; }
        public int? MedicalItemType { get; set; }
        public decimal? SingleDose { get; set; }
        public string MinMeasuringUnit { get; set; }
        public string MinMeasuringUnitName { get; set; }
        public string FrequencyId { get; set; }
        public string FrequencyName { get; set; }
        public string UsageId { get; set; }
        public string UsageName { get; set; }
        public string Treatment { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? ReceivablePrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int NurseMustStatus { get; set; }
        public string ChargeStatus { get; set; }
        public string PrescriptionMedicalStaff { get; set; }
        public string PrescriptionMedicalStaffName { get; set; }
        public DateTime? PrescriptionDate { get; set; }
        public string NurseId { get; set; }
        public string NurseName { get; set; }
        public int? PerformStatus { get; set; }
        public DateTime? PerformDate { get; set; }
        public string CancelPerformReason { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public decimal? PrescribingQty { get; set; }
        public string PrescribingUnit { get; set; }
        public string PrescribingUnitName { get; set; }
        public string DoseUnitName { get; set; }
        public string Packaging { get; set; }
        public int? PackageSpecifications { get; set; }
        public decimal? DoseMin { get; set; }
        public string Specifications { get; set; }
        public int? SpecificationsQuantity { get; set; }
        public decimal? MinMeasuringQty { get; set; }

        public string CatalogueName { get; set; }
        public string IsVisible { get; set; }
        public string MedicalItemName { get; set; }

        public string PrescriptionType { get; set; }
        public string SpeUnitCHS { get; set; }
        /// <summary>
        /// 已退数量
        /// </summary>
        public decimal? RefundTotalQty { get; set; }
        /// <summary>
        /// 退费后的总数量
        /// </summary>
        public decimal? RTotalQty { get; set; }
        /// <summary>
        /// 退费后的总价
        /// </summary>
        public decimal? RTotalPrice { get; set; }
        public decimal? ApplyTotalQty { get; set; }
        public string RefundId { get; set; }
        public bool? IsSplited { get; set; }
        public decimal? ReturnWarehouseQty { get; set; }
        public string FeeType { get; set; }
        public string FeeTypeId { get; set; }
        public string GoodsName { get; set; }
        public string PackageUnitName { get; set; }
        public string SpecificationsUnitName { get; set; }
        public int? OutboundStatus { get; set; }
        public string OutboundId { get; set; }
        public decimal? SecondaryInventory { get; set; }
        public string WeekMedication { get; set; }
        public string Manufacturer { get; set; }

        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public int? DiscountStatus { get; set; }
        public decimal? RefundPrice { get; set; }
    }
    /// <summary>
    /// 收费清单
    /// </summary>
    public class FeesforListingModel
    {
        public string Id { get; set; }
        public string PatientName { get; set; }
        public string Sex { get; set; }

        public DateTime? Birthday { get; set; }
        public int? BalanceType { get; set; }
        /// <summary>
        /// 居民医保
        /// </summary>
        public string SIInsuredType { get; set; }
        public int? BalanceState { get; set; }
        public DateTime? BalanceDate { get; set; }
        public decimal? SumPrice { get; set; }
        public string RecipelNos { get; set; }
        public string BalanceNo { get; set; }
        public decimal? HealthCareMoney { get; set; }
        public decimal? LargeMoney { get; set; }
        public decimal? RliefMoney { get; set; }
        public decimal? AccountMoney { get; set; }
        public decimal? CashMoney { get; set; }
        public decimal? CBMoney { get; set; }
       
    }


    /// <summary>
    /// 收费清单明细
    /// </summary>
    public class FeesforListingDetailModel
    {
        public string Id { get; set; }
        public string PatientName { get; set; }
        public string Sex { get; set; }

        public DateTime? Birthday { get; set; }
        public int? BalanceType { get; set; }
        /// <summary>
        /// 居民医保
        /// </summary>
        public string SIInsuredType { get; set; }
        public int? BalanceState { get; set; }
        public DateTime? BalanceDate { get; set; }
        public decimal? SumPrice { get; set; }
        public string RecipelNos { get; set; }
        public string BalanceNo { get; set; }
       
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string PatientNo { get; set; }
        public string CategoryName { get; set; }
        public DateTime? PrescriptionDetailFounderDate { get; set; }
        public string Specifications { get; set; }
        public string BalancePerson { get; set; }
    }
    public class PrescriptionInput
    {
        public string BalanceNo { get; set; }
        public string PrescriptionId { get; set; }
        public string CenterId { get; set; }
        public string PatientId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int? PageIndex { get; set; }
    }
}
