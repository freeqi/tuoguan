using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 仪表盘统计数据输出DTO
    /// </summary>
    public class DashboardStatisticsOutput
    {
        /// <summary>
        /// 设备统计
        /// </summary>
        public EquipmentStatisticsDto EquipmentStatistics { get; set; }

        /// <summary>
        /// 耗材统计
        /// </summary>
        public ConsumableStatisticsDto ConsumableStatistics { get; set; }

        /// <summary>
        /// 故障统计
        /// </summary>
        public FaultStatisticsDto FaultStatistics { get; set; }

        /// <summary>
        /// 库存预警
        /// </summary>
        public List<InventoryWarningDto> InventoryWarnings { get; set; }

        /// <summary>
        /// 最近设备故障
        /// </summary>
        public List<RecentFaultDto> RecentFaults { get; set; }

        /// <summary>
        /// 最近耗材入库
        /// </summary>
        public List<RecentInboundDto> RecentInbounds { get; set; }

        /// <summary>
        /// 最近耗材出库
        /// </summary>
        public List<RecentOutboundDto> RecentOutbounds { get; set; }
    }

    /// <summary>
    /// 设备统计DTO
    /// </summary>
    public class EquipmentStatisticsDto
    {
        /// <summary>
        /// 总设备数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 运行中设备数
        /// </summary>
        public int RunningCount { get; set; }

        /// <summary>
        /// 待机设备数
        /// </summary>
        public int IdleCount { get; set; }

        /// <summary>
        /// 故障设备数
        /// </summary>
        public int FaultCount { get; set; }

        /// <summary>
        /// 维护中设备数
        /// </summary>
        public int MaintenanceCount { get; set; }
    }

    /// <summary>
    /// 耗材统计DTO
    /// </summary>
    public class ConsumableStatisticsDto
    {
        /// <summary>
        /// 总耗材种类数
        /// </summary>
        public int TotalTypes { get; set; }

        /// <summary>
        /// 库存总量
        /// </summary>
        public int TotalStock { get; set; }

        /// <summary>
        /// 预警数量
        /// </summary>
        public int WarningCount { get; set; }

        /// <summary>
        /// 本月入库量
        /// </summary>
        public int MonthInbound { get; set; }

        /// <summary>
        /// 本月出库量
        /// </summary>
        public int MonthOutbound { get; set; }
    }

    /// <summary>
    /// 故障统计DTO
    /// </summary>
    public class FaultStatisticsDto
    {
        /// <summary>
        /// 本月故障数
        /// </summary>
        public int MonthFaultCount { get; set; }

        /// <summary>
        /// 本月已解决故障数
        /// </summary>
        public int MonthResolvedCount { get; set; }

        /// <summary>
        /// 故障解决率
        /// </summary>
        public decimal ResolutionRate { get; set; }

        /// <summary>
        /// 故障类型分布
        /// </summary>
        public List<FaultTypeDistributionDto> FaultTypeDistribution { get; set; }
    }

    /// <summary>
    /// 故障类型分布DTO
    /// </summary>
    public class FaultTypeDistributionDto
    {
        /// <summary>
        /// 故障类型
        /// </summary>
        public string FaultType { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// 库存预警DTO
    /// </summary>
    public class InventoryWarningDto
    {
        /// <summary>
        /// 预警ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 耗材名称
        /// </summary>
        public string ConsumableName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }

        /// <summary>
        /// 预警类型
        /// </summary>
        public string WarningType { get; set; }

        /// <summary>
        /// 当前库存
        /// </summary>
        public int CurrentStock { get; set; }

        /// <summary>
        /// 预警阈值
        /// </summary>
        public int WarningThreshold { get; set; }

        /// <summary>
        /// 预警时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 最近故障DTO
    /// </summary>
    public class RecentFaultDto
    {
        /// <summary>
        /// 故障ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

        /// <summary>
        /// 故障类型
        /// </summary>
        public string FaultType { get; set; }

        /// <summary>
        /// 故障描述
        /// </summary>
        public string FaultDescription { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime FaultOccurTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// 最近入库DTO
    /// </summary>
    public class RecentInboundDto
    {
        /// <summary>
        /// 入库ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string InboundNo { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime InboundDate { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public int TotalQuantity { get; set; }
    }

    /// <summary>
    /// 最近出库DTO
    /// </summary>
    public class RecentOutboundDto
    {
        /// <summary>
        /// 出库ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundNo { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime OutboundDate { get; set; }

        /// <summary>
        /// 出库数量
        /// </summary>
        public int TotalQuantity { get; set; }
    }
}
