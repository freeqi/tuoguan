using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 设备耗材模板输入DTO
    /// </summary>
    public class EquipmentConsumableTemplateInput
    {
        public string Id { get; set; }
        public string TemplateName { get; set; }
        public string EquipmentTypeId { get; set; }
        public string EquipmentModelId { get; set; }
        public int TemplateType { get; set; }
        public int Status { get; set; }
        public string CenterId { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public string Modifier { get; set; }
        public List<EquipmentConsumableTemplateDetailInput> Details { get; set; }
    }

    /// <summary>
    /// 设备耗材模板输出DTO
    /// </summary>
    public class EquipmentConsumableTemplateOutput
    {
        public string Id { get; set; }
        public string TemplateName { get; set; }
        public string EquipmentTypeId { get; set; }
        public string EquipmentTypeName { get; set; }
        public string EquipmentModelId { get; set; }
        public string EquipmentModelName { get; set; }
        public int TemplateType { get; set; }
        public string TemplateTypeText { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string CenterId { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public List<EquipmentConsumableTemplateDetailOutput> Details { get; set; }
    }

    /// <summary>
    /// 设备耗材模板详情输入DTO
    /// </summary>
    public class EquipmentConsumableTemplateDetailInput
    {
        public string Id { get; set; }
        public string TemplateId { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
    }

    /// <summary>
    /// 设备耗材模板详情输出DTO
    /// </summary>
    public class EquipmentConsumableTemplateDetailOutput
    {
        public string Id { get; set; }
        public string TemplateId { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public string Specifications { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
    }

    /// <summary>
    /// 耗材采购申请输入DTO
    /// </summary>
    public class ConsumablePurchaseRequestInput
    {
        public string Id { get; set; }
        public string RequestNo { get; set; }
        public string TenantId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? EstimatedArrivalDate { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Creator { get; set; }
        public List<ConsumablePurchaseRequestDetailInput> Details { get; set; }
    }

    /// <summary>
    /// 耗材采购申请输出DTO
    /// </summary>
    public class ConsumablePurchaseRequestOutput
    {
        public string Id { get; set; }
        public string RequestNo { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? EstimatedArrivalDate { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Remark { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public List<ConsumablePurchaseRequestDetailOutput> Details { get; set; }
    }

    /// <summary>
    /// 耗材采购申请详情输入DTO
    /// </summary>
    public class ConsumablePurchaseRequestDetailInput
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public string MedicalItemId { get; set; }
        public int PurchaseQuantity { get; set; }
        public string Unit { get; set; }
        public decimal? EstimatedUnitPrice { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 耗材采购申请详情输出DTO
    /// </summary>
    public class ConsumablePurchaseRequestDetailOutput
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public string Specifications { get; set; }
        public int PurchaseQuantity { get; set; }
        public string Unit { get; set; }
        public decimal? EstimatedUnitPrice { get; set; }
        public decimal? EstimatedTotalPrice { get; set; }
        public string Remark { get; set; }
    }
}
