using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 设备报废记录输入DTO
    /// </summary>
    public class EquipmentScrapInput
    {
        public string Id { get; set; }
        public string ScrapNo { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public DateTime ScrapDate { get; set; }
        public int ScrapReasonType { get; set; }
        public string ScrapReason { get; set; }
        public string ScrapHandler { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 设备报废记录输出DTO
    /// </summary>
    public class EquipmentScrapOutput
    {
        public string Id { get; set; }
        public string ScrapNo { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime ScrapDate { get; set; }
        public int ScrapReasonType { get; set; }
        public string ScrapReasonTypeText { get; set; }
        public string ScrapReason { get; set; }
        public string ScrapHandler { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 设备退租记录输入DTO
    /// </summary>
    public class EquipmentReturnInput
    {
        public string Id { get; set; }
        public string ReturnNo { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public string RentalId { get; set; }
        public DateTime ReturnDate { get; set; }
        public int ReturnStatus { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnHandler { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 设备退租记录输出DTO
    /// </summary>
    public class EquipmentReturnOutput
    {
        public string Id { get; set; }
        public string ReturnNo { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string RentalId { get; set; }
        public string RentalNo { get; set; }
        public DateTime ReturnDate { get; set; }
        public int ReturnStatus { get; set; }
        public string ReturnStatusText { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnHandler { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 设备巡检记录输入DTO
    /// </summary>
    public class EquipmentInspectionInput
    {
        public string Id { get; set; }
        public string InspectionNo { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public DateTime InspectionDate { get; set; }
        public int InspectionType { get; set; }
        public string Inspector { get; set; }
        public int InspectionResult { get; set; }
        public string ProblemDescription { get; set; }
        public string HandlingSuggestion { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 设备巡检记录输出DTO
    /// </summary>
    public class EquipmentInspectionOutput
    {
        public string Id { get; set; }
        public string InspectionNo { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime InspectionDate { get; set; }
        public int InspectionType { get; set; }
        public string InspectionTypeText { get; set; }
        public string Inspector { get; set; }
        public int InspectionResult { get; set; }
        public string InspectionResultText { get; set; }
        public string ProblemDescription { get; set; }
        public string HandlingSuggestion { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
