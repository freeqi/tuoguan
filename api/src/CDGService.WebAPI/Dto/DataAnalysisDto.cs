using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 设备故障统计输入DTO
    /// </summary>
    public class EquipmentFaultStatisticsInput
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipmentType { get; set; }

        /// <summary>
        /// 故障类型
        /// </summary>
        public string FaultType { get; set; }
    }

    /// <summary>
    /// 设备故障统计输出DTO
    /// </summary>
    public class EquipmentFaultStatisticsOutput
    {
        /// <summary>
        /// 总故障数
        /// </summary>
        public int TotalFaultCount { get; set; }

        /// <summary>
        /// 已解决故障数
        /// </summary>
        public int ResolvedCount { get; set; }

        /// <summary>
        /// 未解决故障数
        /// </summary>
        public int UnresolvedCount { get; set; }

        /// <summary>
        /// 故障解决率
        /// </summary>
        public decimal ResolutionRate { get; set; }

        /// <summary>
        /// 平均解决时间（小时）
        /// </summary>
        public decimal AverageResolutionTime { get; set; }

        /// <summary>
        /// 故障类型分布
        /// </summary>
        public List<FaultTypeDistributionDto> FaultTypeDistribution { get; set; }

        /// <summary>
        /// 设备故障分布
        /// </summary>
        public List<EquipmentFaultDistributionDto> EquipmentFaultDistribution { get; set; }

        /// <summary>
        /// 故障趋势
        /// </summary>
        public List<FaultTrendDto> FaultTrend { get; set; }
    }

    /// <summary>
    /// 设备故障分布DTO
    /// </summary>
    public class EquipmentFaultDistributionDto
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipmentType { get; set; }

        /// <summary>
        /// 故障数
        /// </summary>
        public int FaultCount { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// 故障趋势DTO
    /// </summary>
    public class FaultTrendDto
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 故障数
        /// </summary>
        public int FaultCount { get; set; }

        /// <summary>
        /// 已解决数
        /// </summary>
        public int ResolvedCount { get; set; }
    }

    /// <summary>
    /// 耗材进销存统计输入DTO
    /// </summary>
    public class ConsumableInventoryStatisticsInput
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 耗材类型
        /// </summary>
        public string ConsumableType { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }
    }

    /// <summary>
    /// 耗材进销存统计输出DTO
    /// </summary>
    public class ConsumableInventoryStatisticsOutput
    {
        /// <summary>
        /// 总入库量
        /// </summary>
        public int TotalInboundQuantity { get; set; }

        /// <summary>
        /// 总入库金额
        /// </summary>
        public decimal TotalInboundAmount { get; set; }

        /// <summary>
        /// 总出库量
        /// </summary>
        public int TotalOutboundQuantity { get; set; }

        /// <summary>
        /// 总出库金额
        /// </summary>
        public decimal TotalOutboundAmount { get; set; }

        /// <summary>
        /// 库存总量
        /// </summary>
        public int TotalStockQuantity { get; set; }

        /// <summary>
        /// 库存总金额
        /// </summary>
        public decimal TotalStockAmount { get; set; }

        /// <summary>
        /// 耗材类型分布
        /// </summary>
        public List<ConsumableTypeDistributionDto> ConsumableTypeDistribution { get; set; }

        /// <summary>
        /// 供应商分布
        /// </summary>
        public List<SupplierDistributionDto> SupplierDistribution { get; set; }

        /// <summary>
        /// 进销存趋势
        /// </summary>
        public List<InventoryTrendDto> InventoryTrend { get; set; }
    }

    /// <summary>
    /// 耗材类型分布DTO
    /// </summary>
    public class ConsumableTypeDistributionDto
    {
        /// <summary>
        /// 耗材类型
        /// </summary>
        public string ConsumableType { get; set; }

        /// <summary>
        /// 入库量
        /// </summary>
        public int InboundQuantity { get; set; }

        /// <summary>
        /// 出库量
        /// </summary>
        public int OutboundQuantity { get; set; }

        /// <summary>
        /// 库存量
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// 供应商分布DTO
    /// </summary>
    public class SupplierDistributionDto
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 入库量
        /// </summary>
        public int InboundQuantity { get; set; }

        /// <summary>
        /// 入库金额
        /// </summary>
        public decimal InboundAmount { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// 进销存趋势DTO
    /// </summary>
    public class InventoryTrendDto
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 入库量
        /// </summary>
        public int InboundQuantity { get; set; }

        /// <summary>
        /// 出库量
        /// </summary>
        public int OutboundQuantity { get; set; }

        /// <summary>
        /// 库存变化
        /// </summary>
        public int StockChange { get; set; }
    }
}
