using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class DashboardManager
    {
        private readonly IRepository<EquipmentInfo> _equipmentInfoRepository;
        private readonly IRepository<MedicalItemRecord> _medicalItemRecordRepository;
        private readonly IRepository<ConsumableInventory> _consumableInventoryRepository;
        private readonly IRepository<ConsumableInventoryWarning> _consumableInventoryWarningRepository;
        private readonly IRepository<EquipmentFaultRecord> _equipmentFaultRecordRepository;
        private readonly IRepository<ConsumableInbound> _consumableInboundRepository;
        private readonly IRepository<ConsumableInboundDetail> _consumableInboundDetailRepository;
        private readonly IRepository<ConsumableOutbound> _consumableOutboundRepository;
        private readonly IRepository<ConsumableOutboundDetail> _consumableOutboundDetailRepository;

        public DashboardManager(
            IRepository<EquipmentInfo> equipmentInfoRepository,
            IRepository<MedicalItemRecord> medicalItemRecordRepository,
            IRepository<ConsumableInventory> consumableInventoryRepository,
            IRepository<ConsumableInventoryWarning> consumableInventoryWarningRepository,
            IRepository<EquipmentFaultRecord> equipmentFaultRecordRepository,
            IRepository<ConsumableInbound> consumableInboundRepository,
            IRepository<ConsumableInboundDetail> consumableInboundDetailRepository,
            IRepository<ConsumableOutbound> consumableOutboundRepository,
            IRepository<ConsumableOutboundDetail> consumableOutboundDetailRepository)
        {
            _equipmentInfoRepository = equipmentInfoRepository;
            _medicalItemRecordRepository = medicalItemRecordRepository;
            _consumableInventoryRepository = consumableInventoryRepository;
            _consumableInventoryWarningRepository = consumableInventoryWarningRepository;
            _equipmentFaultRecordRepository = equipmentFaultRecordRepository;
            _consumableInboundRepository = consumableInboundRepository;
            _consumableInboundDetailRepository = consumableInboundDetailRepository;
            _consumableOutboundRepository = consumableOutboundRepository;
            _consumableOutboundDetailRepository = consumableOutboundDetailRepository;
        }

        /// <summary>
        /// 获取仪表盘统计数据
        /// </summary>
        public async Task<DashboardStatisticsOutput> GetDashboardStatisticsAsync()
        {
            var output = new DashboardStatisticsOutput();

            // 获取设备统计
            output.EquipmentStatistics = await GetEquipmentStatisticsAsync();

            // 获取耗材统计
            output.ConsumableStatistics = await GetConsumableStatisticsAsync();

            // 获取故障统计
            output.FaultStatistics = await GetFaultStatisticsAsync();

            // 获取库存预警
            output.InventoryWarnings = await GetInventoryWarningsAsync();

            // 获取最近设备故障
            output.RecentFaults = await GetRecentFaultsAsync();

            // 获取最近耗材入库
            output.RecentInbounds = await GetRecentInboundsAsync();

            // 获取最近耗材出库
            output.RecentOutbounds = await GetRecentOutboundsAsync();

            return output;
        }

        /// <summary>
        /// 获取设备统计数据
        /// </summary>
        private async Task<EquipmentStatisticsDto> GetEquipmentStatisticsAsync()
        {
            var equipmentList = await _equipmentInfoRepository.GetAllAsync();
            var equipmentListCount = equipmentList.Count();

            var statistics = new EquipmentStatisticsDto
            {
                TotalCount = equipmentListCount,
                RunningCount = equipmentList.Count(e => e.EquipmentState == "1"), // 运行中
                IdleCount = equipmentList.Count(e => e.EquipmentState == "2"), // 待机
                FaultCount = equipmentList.Count(e => e.EquipmentState == "3"), // 故障
                MaintenanceCount = equipmentList.Count(e => e.EquipmentState == "4") // 维护中
            };

            return statistics;
        }

        /// <summary>
        /// 获取耗材统计数据
        /// </summary>
        private async Task<ConsumableStatisticsDto> GetConsumableStatisticsAsync()
        {
            var medicalItems = await _medicalItemRecordRepository.GetAllAsync();
            var inventoryList = await _consumableInventoryRepository.GetAllAsync();
            var warningList = await _consumableInventoryWarningRepository.GetAllAsync();
            var inboundList = await _consumableInboundRepository.GetAllAsync(i => i.InboundDate >= DateTime.Now.AddMonths(-1));
            var outboundList = await _consumableOutboundRepository.GetAllAsync(o => o.OutboundDate >= DateTime.Now.AddMonths(-1));

            var inboundDetails = await _consumableInboundDetailRepository.GetAllAsync(d => inboundList.Select(i => i.Id).Contains(d.InboundId));
            var outboundDetails = await _consumableOutboundDetailRepository.GetAllAsync(d => outboundList.Select(o => o.Id).Contains(d.OutboundId));

            var statistics = new ConsumableStatisticsDto
            {
                TotalTypes = medicalItems.Count(),
                TotalStock = inventoryList.Sum(i => i.StockQuantity),
                WarningCount = warningList.Count(w => w.ProcessingStatus == 0),
                MonthInbound = inboundDetails.Sum(d => d.Quantity),
                MonthOutbound = outboundDetails.Sum(d => d.Quantity)
            };

            return statistics;
        }

        /// <summary>
        /// 获取故障统计数据
        /// </summary>
        private async Task<FaultStatisticsDto> GetFaultStatisticsAsync()
        {
            var faultList = await _equipmentFaultRecordRepository.GetAllAsync(f => f.FaultTime >= DateTime.Now.AddMonths(-1));
            var faultListCount = faultList.Count();
            var resolvedCount = faultList.Count(f => f.Status == 2); // 已解决

            var faultTypeDistribution = faultList
                .GroupBy(f => f.FaultType)
                .Select(g => new FaultTypeDistributionDto
                {
                    FaultType = g.Key,
                    Count = g.Count(),
                    Percentage = faultListCount > 0 ? Math.Round((decimal)g.Count() / faultListCount * 100, 2) : 0
                })
                .ToList();

            var statistics = new FaultStatisticsDto
            {
                MonthFaultCount = faultListCount,
                MonthResolvedCount = resolvedCount,
                ResolutionRate = faultListCount > 0 ? Math.Round((decimal)resolvedCount / faultListCount * 100, 2) : 0,
                FaultTypeDistribution = faultTypeDistribution
            };

            return statistics;
        }

        /// <summary>
        /// 获取库存预警
        /// </summary>
        private async Task<List<InventoryWarningDto>> GetInventoryWarningsAsync()
        {
            var warningList = await _consumableInventoryWarningRepository.GetAllAsync(w => w.ProcessingStatus == 0);
            var inventoryList = await _consumableInventoryRepository.GetAllAsync();
            var medicalItems = await _medicalItemRecordRepository.GetAllAsync();

            var warnings = warningList
                .Select(w => {
                    var inventory = inventoryList.FirstOrDefault(i => i.MedicalItemId == w.MedicalItemId);
                    var medicalItem = medicalItems.FirstOrDefault(m => m.Id == w.MedicalItemId);

                    return new InventoryWarningDto
                    {
                        Id = w.Id,
                        ConsumableName = medicalItem?.MedicalItemName ?? "",
                        Specifications = medicalItem?.Specifications ?? "",
                        WarningType = w.WarningType.ToString(),
                        CurrentStock = inventory?.StockQuantity ?? 0,
                        WarningThreshold = 10, // 示例预警阈值
                        CreateDate = w.CreateDate
                    };
                })
                .Take(5)
                .ToList();

            return warnings;
        }

        /// <summary>
        /// 获取最近设备故障
        /// </summary>
        private async Task<List<RecentFaultDto>> GetRecentFaultsAsync()
        {
            var faultList = await _equipmentFaultRecordRepository.GetAllAsync();
            var equipmentList = await _equipmentInfoRepository.GetAllAsync();

            var recentFaults = faultList
                .OrderByDescending(f => f.FaultTime)
                .Select(f => {
                    var equipment = equipmentList.FirstOrDefault(e => e.Id == f.EquipmentId);

                    return new RecentFaultDto
                    {
                        Id = f.Id,
                        EquipmentName = equipment?.Name ?? "",
                        FaultType = f.FaultType,
                        FaultDescription = f.FaultDescription,
                        FaultOccurTime = f.FaultTime,
                        Status = f.Status == 0 ? "待处理" : f.Status == 1 ? "处理中" : "已解决"
                    };
                })
                .Take(5)
                .ToList();

            return recentFaults;
        }

        /// <summary>
        /// 获取最近耗材入库
        /// </summary>
        private async Task<List<RecentInboundDto>> GetRecentInboundsAsync()
        {
            var inboundList = await _consumableInboundRepository.GetAllAsync();
            var inboundDetails = await _consumableInboundDetailRepository.GetAllAsync();

            var recentInbounds = inboundList
                .OrderByDescending(i => i.InboundDate)
                .Select(i => {
                    var details = inboundDetails.Where(d => d.InboundId == i.Id);

                    return new RecentInboundDto
                    {
                        Id = i.Id,
                        InboundNo = i.InboundNo,
                        SupplierName = i.SupplierName,
                        InboundDate = i.InboundDate,
                        TotalQuantity = details.Sum(d => d.Quantity)
                    };
                })
                .Take(5)
                .ToList();

            return recentInbounds;
        }

        /// <summary>
        /// 获取最近耗材出库
        /// </summary>
        private async Task<List<RecentOutboundDto>> GetRecentOutboundsAsync()
        {
            var outboundList = await _consumableOutboundRepository.GetAllAsync();
            var outboundDetails = await _consumableOutboundDetailRepository.GetAllAsync();

            var recentOutbounds = outboundList
                .OrderByDescending(o => o.OutboundDate)
                .Select(o => {
                    var details = outboundDetails.Where(d => d.OutboundId == o.Id);

                    return new RecentOutboundDto
                    {
                        Id = o.Id,
                        OutboundNo = o.OutboundNo,
                        DepartmentName = o.DepartmentName,
                        OutboundDate = o.OutboundDate,
                        TotalQuantity = details.Sum(d => d.Quantity)
                    };
                })
                .Take(5)
                .ToList();

            return recentOutbounds;
        }
    }
}
