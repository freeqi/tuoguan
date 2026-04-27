using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 设备监控记录输入DTO
    /// </summary>
    public class EquipmentMonitoringInput
    {
        public string Id { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public DateTime MonitoringTime { get; set; }
        public string Status { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? Pressure { get; set; }
        public decimal? Voltage { get; set; }
        public decimal? Current { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 设备监控记录输出DTO
    /// </summary>
    public class EquipmentMonitoringOutput
    {
        public string Id { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime MonitoringTime { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? Pressure { get; set; }
        public decimal? Voltage { get; set; }
        public decimal? Current { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 设备故障预警输入DTO
    /// </summary>
    public class EquipmentFaultWarningInput
    {
        public string Id { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public DateTime WarningTime { get; set; }
        public int WarningLevel { get; set; }
        public string WarningType { get; set; }
        public string WarningContent { get; set; }
        public int ProcessingStatus { get; set; }
        public string Handler { get; set; }
        public DateTime? ProcessingTime { get; set; }
        public string ProcessingResult { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 设备故障预警输出DTO
    /// </summary>
    public class EquipmentFaultWarningOutput
    {
        public string Id { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime WarningTime { get; set; }
        public int WarningLevel { get; set; }
        public string WarningLevelText { get; set; }
        public string WarningType { get; set; }
        public string WarningContent { get; set; }
        public int ProcessingStatus { get; set; }
        public string ProcessingStatusText { get; set; }
        public string Handler { get; set; }
        public DateTime? ProcessingTime { get; set; }
        public string ProcessingResult { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 设备故障记录输入DTO
    /// </summary>
    public class EquipmentFaultRecordInput
    {
        public string Id { get; set; }
        public string FaultNo { get; set; }
        public string EquipmentId { get; set; }
        public string TenantId { get; set; }
        public DateTime FaultOccurTime { get; set; }
        public DateTime? FaultResolveTime { get; set; }
        public string FaultType { get; set; }
        public string FaultDescription { get; set; }
        public string FaultReason { get; set; }
        public string HandlingMethod { get; set; }
        public string Handler { get; set; }
        public int FaultStatus { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
    }

    /// <summary>
    /// 设备故障记录输出DTO
    /// </summary>
    public class EquipmentFaultRecordOutput
    {
        public string Id { get; set; }
        public string FaultNo { get; set; }
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public DateTime FaultOccurTime { get; set; }
        public DateTime? FaultResolveTime { get; set; }
        public string FaultType { get; set; }
        public string FaultDescription { get; set; }
        public string FaultReason { get; set; }
        public string HandlingMethod { get; set; }
        public string Handler { get; set; }
        public int FaultStatus { get; set; }
        public string FaultStatusText { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
