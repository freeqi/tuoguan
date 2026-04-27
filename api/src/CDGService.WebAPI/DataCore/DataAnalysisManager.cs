using CDGService.Data.Datas;
using CDGService.Store.IRepository;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class DataAnalysisManager
    {
        private readonly IRepository<EquipmentFaultRecord> _equipmentFaultRecordRepository;
        private readonly IRepository<MedicalEquipment> _medicalEquipmentRepository;
        private readonly IRepository<ConsumableInbound> _consumableInboundRepository;
        private readonly IRepository<ConsumableInboundDetail> _consumableInboundDetailRepository;
        private readonly IRepository<ConsumableOutbound> _consumableOutboundRepository;
        private readonly IRepository<ConsumableOutboundDetail> _consumableOutboundDetailRepository;
        private readonly IRepository<ConsumableInventory> _consumableInventoryRepository;
        private readonly IRepository<MedicalItem> _medicalItemRepository;

        public DataAnalysisManager(
            IRepository<EquipmentFaultRecord> equipmentFaultRecordRepository,
            IRepository<MedicalEquipment> medicalEquipmentRepository,
            IRepository<ConsumableInbound> consumableInboundRepository,
            IRepository<ConsumableInboundDetail> consumableInboundDetailRepository,
            IRepository<ConsumableOutbound> consumableOutboundRepository,
            IRepository<ConsumableOutboundDetail> consumableOutboundDetailRepository,
            IRepository<ConsumableInventory> consumableInventoryRepository,
            IRepository<MedicalItem> medicalItemRepository)
        {
            _equipmentFaultRecordRepository = equipmentFaultRecordRepository;
            _medicalEquipmentRepository = medicalEquipmentRepository;
            _consumableInboundRepository = consumableInboundRepository;
            _consumableInboundDetailRepository = consumableInboundDetailRepository;
            _consumableOutboundRepository = consumableOutboundRepository;
            _consumableOutboundDetailRepository = consumableOutboundDetailRepository;
            _consumableInventoryRepository = consumableInventoryRepository;
            _medicalItemRepository = medicalItemRepository;
        }

        /// <summary>
        /// 获取设备故障统计数据
        /// </summary>
        public async Task<EquipmentFaultStatisticsOutput> GetEquipmentFaultStatisticsAsync(EquipmentFaultStatisticsInput input)
        {
            var faultList = await _equipmentFaultRecordRepository.GetAllAsync();
            var equipmentList = await _medicalEquipmentRepository.GetAllAsync();

            // 应用过滤条件
            if (input.StartDate.HasValue)
            {
                faultList = faultList.Where(f => f.FaultOccurTime >= input.StartDate.Value).ToList();
            }
            if (input.EndDate.HasValue)
            {
                faultList = faultList.Where(f => f.FaultOccurTime <= input.EndDate.Value).ToList();
            }
            if (!string.IsNullOrEmpty(input.EquipmentType))
            {
                var equipmentIds = equipmentList.Where(e => e.EquipmentType == input.EquipmentType).Select(e => e.Id).ToList();
                faultList = faultList.Where(f => equipmentIds.Contains(f.EquipmentId)).ToList();
            }
            if (!string.IsNullOrEmpty(input.FaultType))
            {
                faultList = faultList.Where(f => f.FaultType == input.FaultType).ToList();
            }

            var resolvedCount = faultList.Count(f => f.ProcessingStatus == 2);
            var unresolvedCount = faultList.Count - resolvedCount;
            var resolutionRate = faultList.Count > 0 ? Math.Round((decimal)resolvedCount / faultList.Count * 100, 2) : 0;

            // 计算平均解决时间
            var resolvedFaults = faultList.Where(f => f.ProcessingStatus == 2 && f.ProcessingCompleteTime.HasValue);
            var totalResolutionTime = resolvedFaults.Sum(f => (f.ProcessingCompleteTime.Value - f.FaultOccurTime).TotalHours);
            var averageResolutionTime = resolvedFaults.Count() > 0 ? Math.Round((decimal)totalResolutionTime / resolvedFaults.Count(), 2) : 0;

            // 故障类型分布
            var faultTypeDistribution = faultList
                .GroupBy(f => f.FaultType)
                .Select(g => new FaultTypeDistributionDto
                {
                    FaultType = g.Key,
                    Count = g.Count(),
                    Percentage = faultList.Count > 0 ? Math.Round((decimal)g.Count() / faultList.Count * 100, 2) : 0
                })
                .ToList();

            // 设备故障分布
            var equipmentFaultDistribution = faultList
                .GroupBy(f => f.EquipmentId)
                .Select(g => {
                    var equipment = equipmentList.FirstOrDefault(e => e.Id == g.Key);
                    return new EquipmentFaultDistributionDto
                    {
                        EquipmentId = g.Key,
                        EquipmentName = equipment?.EquipmentName ?? "",
                        EquipmentType = equipment?.EquipmentType ?? "",
                        FaultCount = g.Count(),
                        Percentage = faultList.Count > 0 ? Math.Round((decimal)g.Count() / faultList.Count * 100, 2) : 0
                    };
                })
                .OrderByDescending(e => e.FaultCount)
                .Take(10)
                .ToList();

            // 故障趋势
            var faultTrend = faultList
                .GroupBy(f => f.FaultOccurTime.Date)
                .Select(g => {
                    var resolved = g.Count(f => f.ProcessingStatus == 2);
                    return new FaultTrendDto
                    {
                        Date = g.Key,
                        FaultCount = g.Count(),
                        ResolvedCount = resolved
                    };
                })
                .OrderBy(t => t.Date)
                .ToList();

            var output = new EquipmentFaultStatisticsOutput
            {
                TotalFaultCount = faultList.Count,
                ResolvedCount = resolvedCount,
                UnresolvedCount = unresolvedCount,
                ResolutionRate = resolutionRate,
                AverageResolutionTime = averageResolutionTime,
                FaultTypeDistribution = faultTypeDistribution,
                EquipmentFaultDistribution = equipmentFaultDistribution,
                FaultTrend = faultTrend
            };

            return output;
        }

        /// <summary>
        /// 获取耗材进销存统计数据
        /// </summary>
        public async Task<ConsumableInventoryStatisticsOutput> GetConsumableInventoryStatisticsAsync(ConsumableInventoryStatisticsInput input)
        {
            var inboundList = await _consumableInboundRepository.GetAllAsync();
            var inboundDetails = await _consumableInboundDetailRepository.GetAllAsync();
            var outboundList = await _consumableOutboundRepository.GetAllAsync();
            var outboundDetails = await _consumableOutboundDetailRepository.GetAllAsync();
            var inventoryList = await _consumableInventoryRepository.GetAllAsync();
            var medicalItems = await _medicalItemRepository.GetAllAsync();

            // 应用过滤条件
            if (input.StartDate.HasValue)
            {
                inboundList = inboundList.Where(i => i.InboundDate >= input.StartDate.Value).ToList();
                outboundList = outboundList.Where(o => o.OutboundDate >= input.StartDate.Value).ToList();
            }
            if (input.EndDate.HasValue)
            {
                inboundList = inboundList.Where(i => i.InboundDate <= input.EndDate.Value).ToList();
                outboundList = outboundList.Where(o => o.OutboundDate <= input.EndDate.Value).ToList();
            }
            if (!string.IsNullOrEmpty(input.SupplierName))
            {
                inboundList = inboundList.Where(i => i.SupplierName == input.SupplierName).ToList();
            }

            // 过滤入库和出库详情
            var filteredInboundDetails = inboundDetails.Where(d => inboundList.Select(i => i.Id).Contains(d.ConsumableInboundId)).ToList();
            var filteredOutboundDetails = outboundDetails.Where(d => outboundList.Select(o => o.Id).Contains(d.ConsumableOutboundId)).ToList();

            // 应用耗材类型过滤
            if (!string.IsNullOrEmpty(input.ConsumableType))
            {
                var medicalItemIds = medicalItems.Where(m => m.Type == input.ConsumableType).Select(m => m.Id).ToList();
                filteredInboundDetails = filteredInboundDetails.Where(d => medicalItemIds.Contains(d.MedicalItemId)).ToList();
                filteredOutboundDetails = filteredOutboundDetails.Where(d => medicalItemIds.Contains(d.MedicalItemId)).ToList();
                inventoryList = inventoryList.Where(i => medicalItemIds.Contains(i.MedicalItemId)).ToList();
            }

            // 计算总入库量和金额
            var totalInboundQuantity = filteredInboundDetails.Sum(d => d.Quantity);
            var totalInboundAmount = filteredInboundDetails.Sum(d => d.Quantity * d.UnitPrice);

            // 计算总出库量和金额
            var totalOutboundQuantity = filteredOutboundDetails.Sum(d => d.Quantity);
            var totalOutboundAmount = filteredOutboundDetails.Sum(d => d.Quantity * d.UnitPrice);

            // 计算库存总量和金额
            var totalStockQuantity = inventoryList.Sum(i => i.StockQuantity);
            var totalStockAmount = inventoryList.Sum(i => i.StockQuantity * i.UnitPrice);

            // 耗材类型分布
            var consumableTypeDistribution = filteredInboundDetails
                .GroupBy(d => {
                    var medicalItem = medicalItems.FirstOrDefault(m => m.Id == d.MedicalItemId);
                    return medicalItem?.Type ?? "未知";
                })
                .Select(g => {
                    var type = g.Key;
                    var typeMedicalItemIds = medicalItems.Where(m => m.Type == type).Select(m => m.Id).ToList();
                    var typeInbound = filteredInboundDetails.Where(d => typeMedicalItemIds.Contains(d.MedicalItemId)).Sum(d => d.Quantity);
                    var typeOutbound = filteredOutboundDetails.Where(d => typeMedicalItemIds.Contains(d.MedicalItemId)).Sum(d => d.Quantity);
                    var typeStock = inventoryList.Where(i => typeMedicalItemIds.Contains(i.MedicalItemId)).Sum(i => i.StockQuantity);
                    var total = typeInbound + typeOutbound + typeStock;

                    return new ConsumableTypeDistributionDto
                    {
                        ConsumableType = type,
                        InboundQuantity = typeInbound,
                        OutboundQuantity = typeOutbound,
                        StockQuantity = typeStock,
                        Percentage = total > 0 ? Math.Round((decimal)total / (totalInboundQuantity + totalOutboundQuantity + totalStockQuantity) * 100, 2) : 0
                    };
                })
                .ToList();

            // 供应商分布
            var supplierDistribution = inboundList
                .GroupBy(i => i.SupplierName)
                .Select(g => {
                    var supplier = g.Key;
                    var supplierInboundIds = g.Select(i => i.Id).ToList();
                    var supplierInbound = filteredInboundDetails.Where(d => supplierInboundIds.Contains(d.ConsumableInboundId));
                    var supplierQuantity = supplierInbound.Sum(d => d.Quantity);
                    var supplierAmount = supplierInbound.Sum(d => d.Quantity * d.UnitPrice);

                    return new SupplierDistributionDto
                    {
                        SupplierName = supplier,
                        InboundQuantity = supplierQuantity,
                        InboundAmount = supplierAmount,
                        Percentage = totalInboundQuantity > 0 ? Math.Round((decimal)supplierQuantity / totalInboundQuantity * 100, 2) : 0
                    };
                })
                .OrderByDescending(s => s.InboundAmount)
                .Take(10)
                .ToList();

            // 进销存趋势
            var inboundTrend = filteredInboundDetails
                .GroupBy(d => {
                    var inbound = inboundList.FirstOrDefault(i => i.Id == d.ConsumableInboundId);
                    return inbound?.InboundDate.Date ?? DateTime.MinValue;
                })
                .Where(g => g.Key != DateTime.MinValue)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Quantity));

            var outboundTrend = filteredOutboundDetails
                .GroupBy(d => {
                    var outbound = outboundList.FirstOrDefault(o => o.Id == d.ConsumableOutboundId);
                    return outbound?.OutboundDate.Date ?? DateTime.MinValue;
                })
                .Where(g => g.Key != DateTime.MinValue)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Quantity));

            var allDates = inboundTrend.Keys.Union(outboundTrend.Keys).OrderBy(d => d).ToList();

            var inventoryTrend = allDates
                .Select(date => new InventoryTrendDto
                {
                    Date = date,
                    InboundQuantity = inboundTrend.ContainsKey(date) ? inboundTrend[date] : 0,
                    OutboundQuantity = outboundTrend.ContainsKey(date) ? outboundTrend[date] : 0,
                    StockChange = (inboundTrend.ContainsKey(date) ? inboundTrend[date] : 0) - (outboundTrend.ContainsKey(date) ? outboundTrend[date] : 0)
                })
                .ToList();

            var output = new ConsumableInventoryStatisticsOutput
            {
                TotalInboundQuantity = totalInboundQuantity,
                TotalInboundAmount = totalInboundAmount,
                TotalOutboundQuantity = totalOutboundQuantity,
                TotalOutboundAmount = totalOutboundAmount,
                TotalStockQuantity = totalStockQuantity,
                TotalStockAmount = totalStockAmount,
                ConsumableTypeDistribution = consumableTypeDistribution,
                SupplierDistribution = supplierDistribution,
                InventoryTrend = inventoryTrend
            };

            return output;
        }
    }
}
