using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 耗材入库记录输入DTO
    /// </summary>
    public class ConsumableInboundInput
    {
        public string Id { get; set; }
        public string InboundNo { get; set; }
        public string TenantId { get; set; }
        public string SupplierId { get; set; }
        public DateTime InboundDate { get; set; }
        public string Operator { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public List<ConsumableInboundDetailInput> Details { get; set; }
    }

    /// <summary>
    /// 耗材入库记录输出DTO
    /// </summary>
    public class ConsumableInboundOutput
    {
        public string Id { get; set; }
        public string InboundNo { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public DateTime InboundDate { get; set; }
        public string Operator { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public List<ConsumableInboundDetailOutput> Details { get; set; }
    }

    /// <summary>
    /// 耗材入库详情输入DTO
    /// </summary>
    public class ConsumableInboundDetailInput
    {
        public string Id { get; set; }
        public string InboundId { get; set; }
        public string MedicalItemId { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 耗材入库详情输出DTO
    /// </summary>
    public class ConsumableInboundDetailOutput
    {
        public string Id { get; set; }
        public string InboundId { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public string Specifications { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 耗材出库记录输入DTO
    /// </summary>
    public class ConsumableOutboundInput
    {
        public string Id { get; set; }
        public string OutboundNo { get; set; }
        public string TenantId { get; set; }
        public string DepartmentId { get; set; }
        public DateTime OutboundDate { get; set; }
        public string Operator { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public List<ConsumableOutboundDetailInput> Details { get; set; }
    }

    /// <summary>
    /// 耗材出库记录输出DTO
    /// </summary>
    public class ConsumableOutboundOutput
    {
        public string Id { get; set; }
        public string OutboundNo { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime OutboundDate { get; set; }
        public string Operator { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public List<ConsumableOutboundDetailOutput> Details { get; set; }
    }

    /// <summary>
    /// 耗材出库详情输入DTO
    /// </summary>
    public class ConsumableOutboundDetailInput
    {
        public string Id { get; set; }
        public string OutboundId { get; set; }
        public string MedicalItemId { get; set; }
        public string BatchNo { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 耗材出库详情输出DTO
    /// </summary>
    public class ConsumableOutboundDetailOutput
    {
        public string Id { get; set; }
        public string OutboundId { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public string Specifications { get; set; }
        public string BatchNo { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 耗材库存预警输入DTO
    /// </summary>
    public class ConsumableInventoryWarningInput
    {
        public string Id { get; set; }
        public string MedicalItemId { get; set; }
        public string TenantId { get; set; }
        public int WarningType { get; set; }
        public int WarningThreshold { get; set; }
        public int CurrentStock { get; set; }
        public string WarningMessage { get; set; }
        public int ProcessingStatus { get; set; }
        public string Handler { get; set; }
        public DateTime? ProcessingTime { get; set; }
        public string ProcessingResult { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 耗材库存预警输出DTO
    /// </summary>
    public class ConsumableInventoryWarningOutput
    {
        public string Id { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public string Specifications { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public int WarningType { get; set; }
        public string WarningTypeText { get; set; }
        public int WarningThreshold { get; set; }
        public int CurrentStock { get; set; }
        public string WarningMessage { get; set; }
        public int ProcessingStatus { get; set; }
        public string ProcessingStatusText { get; set; }
        public string Handler { get; set; }
        public DateTime? ProcessingTime { get; set; }
        public string ProcessingResult { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 耗材库存查询输入DTO
    /// </summary>
    public class ConsumableInventoryQueryInput
    {
        public string TenantId { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public string BatchNo { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 耗材库存查询输出DTO
    /// </summary>
    public class ConsumableInventoryOutput
    {
        public string Id { get; set; }
        public string MedicalItemId { get; set; }
        public string ConsumableName { get; set; }
        public string Specifications { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int StockQuantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastInboundDate { get; set; }
        public DateTime? LastOutboundDate { get; set; }
    }
}
