using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 设备维保记录输入DTO
    /// </summary>
    public class EquipmentMaintenanceInput
    {
        public string Id { get; set; }
        public string MaintenanceNo { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public int MaintenanceType { get; set; }
        public string MaintenanceContent { get; set; }
        public string ServiceProvider { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public decimal MaintenanceCost { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public List<EquipmentMaintenanceDetailInput> Details { get; set; }
    }

    /// <summary>
    /// 设备维保记录输出DTO
    /// </summary>
    public class EquipmentMaintenanceOutput
    {
        public string Id { get; set; }
        public string MaintenanceNo { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public int MaintenanceType { get; set; }
        public string MaintenanceTypeText { get; set; }
        public string MaintenanceContent { get; set; }
        public string ServiceProvider { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public decimal MaintenanceCost { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public List<EquipmentMaintenanceDetailOutput> Details { get; set; }
    }

    /// <summary>
    /// 设备维保记录详情输入DTO
    /// </summary>
    public class EquipmentMaintenanceDetailInput
    {
        public string Id { get; set; }
        public string MaintenanceId { get; set; }
        public string PartName { get; set; }
        public string PartModel { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 设备维保记录详情输出DTO
    /// </summary>
    public class EquipmentMaintenanceDetailOutput
    {
        public string Id { get; set; }
        public string MaintenanceId { get; set; }
        public string PartName { get; set; }
        public string PartModel { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 设备维保延长申请输入DTO
    /// </summary>
    public class EquipmentMaintenanceExtensionInput
    {
        public string Id { get; set; }
        public string ExtensionNo { get; set; }
        public string MaintenanceId { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime? OriginalEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public string ExtensionReason { get; set; }
        public int Status { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 设备维保延长申请输出DTO
    /// </summary>
    public class EquipmentMaintenanceExtensionOutput
    {
        public string Id { get; set; }
        public string ExtensionNo { get; set; }
        public string MaintenanceId { get; set; }
        public string MaintenanceNo { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime? OriginalEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public string ExtensionReason { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
