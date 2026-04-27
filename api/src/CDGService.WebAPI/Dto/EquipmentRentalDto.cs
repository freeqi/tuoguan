using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 设备租用输入DTO
    /// </summary>
    public class EquipmentRentalInput
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string RentalNo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime EstimatedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Creator { get; set; }
        public List<EquipmentRentalDetailInput> Details { get; set; }
    }

    /// <summary>
    /// 设备租用输出DTO
    /// </summary>
    public class EquipmentRentalOutput
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string RentalNo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime EstimatedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Remark { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public List<EquipmentRentalDetailOutput> Details { get; set; }
    }

    /// <summary>
    /// 设备租用详情输入DTO
    /// </summary>
    public class EquipmentRentalDetailInput
    {
        public string Id { get; set; }
        public string RentalId { get; set; }
        public string EquipmentId { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 设备租用详情输出DTO
    /// </summary>
    public class EquipmentRentalDetailOutput
    {
        public string Id { get; set; }
        public string RentalId { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentModel { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 设备租用延长输入DTO
    /// </summary>
    public class EquipmentRentalExtensionInput
    {
        public string Id { get; set; }
        public string RentalId { get; set; }
        public DateTime OriginalReturnDate { get; set; }
        public DateTime NewReturnDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }

    /// <summary>
    /// 设备租用延长输出DTO
    /// </summary>
    public class EquipmentRentalExtensionOutput
    {
        public string Id { get; set; }
        public string RentalId { get; set; }
        public string RentalNo { get; set; }
        public DateTime OriginalReturnDate { get; set; }
        public DateTime NewReturnDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Applicant { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
