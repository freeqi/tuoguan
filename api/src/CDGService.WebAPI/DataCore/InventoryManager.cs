using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class InventoryManager
    {
        private readonly IRepository<ConsumableInbound> _consumableInboundRepository;
        private readonly IRepository<ConsumableInboundDetail> _consumableInboundDetailRepository;
        private readonly IRepository<ConsumableOutbound> _consumableOutboundRepository;
        private readonly IRepository<ConsumableOutboundDetail> _consumableOutboundDetailRepository;
        private readonly IRepository<ConsumableInventory> _consumableInventoryRepository;
        private readonly IRepository<ConsumableInventoryWarning> _consumableInventoryWarningRepository;
        private readonly IRepository<MedicalItemRecord> _medicalItemRecordRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InventoryManager(
            IRepository<ConsumableInbound> consumableInboundRepository,
            IRepository<ConsumableInboundDetail> consumableInboundDetailRepository,
            IRepository<ConsumableOutbound> consumableOutboundRepository,
            IRepository<ConsumableOutboundDetail> consumableOutboundDetailRepository,
            IRepository<ConsumableInventory> consumableInventoryRepository,
            IRepository<ConsumableInventoryWarning> consumableInventoryWarningRepository,
            IRepository<MedicalItemRecord> medicalItemRecordRepository,
            IRepository<Tenant> tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _consumableInboundRepository = consumableInboundRepository;
            _consumableInboundDetailRepository = consumableInboundDetailRepository;
            _consumableOutboundRepository = consumableOutboundRepository;
            _consumableOutboundDetailRepository = consumableOutboundDetailRepository;
            _consumableInventoryRepository = consumableInventoryRepository;
            _consumableInventoryWarningRepository = consumableInventoryWarningRepository;
            _medicalItemRecordRepository = medicalItemRecordRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取耗材入库记录列表
        /// </summary>
        public async Task<List<ConsumableInboundOutput>> GetInboundsAsync()
        {
            var inbounds = await _consumableInboundRepository.GetAllAsync();
            var result = new List<ConsumableInboundOutput>();

            foreach (var inbound in inbounds)
            {
                var details = await _consumableInboundDetailRepository.GetAllAsync(d => d.InboundId == inbound.Id);
                var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == inbound.TenantId);

                var inboundOutput = new ConsumableInboundOutput
                {
                    Id = inbound.Id,
                    InboundNo = inbound.InboundNo,
                    TenantId = inbound.TenantId,
                    TenantName = tenant?.TenantName,
                    SupplierId = inbound.SupplierId,
                    SupplierName = inbound.SupplierName,
                    InboundDate = inbound.InboundDate,
                    Operator = inbound.Operator,
                    Remark = inbound.Remark,
                    Creator = inbound.Creator,
                    CreateDate = inbound.CreateDate,
                    Details = new List<ConsumableInboundDetailOutput>()
                };

                foreach (var detail in details)
                {
                    var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(m => m.Id == detail.MedicalItemId);
                    inboundOutput.Details.Add(new ConsumableInboundDetailOutput
                    {
                        Id = detail.Id,
                        InboundId = detail.InboundId,
                        MedicalItemId = detail.MedicalItemId,
                        ConsumableName = medicalItem?.MedicalItemName,
                        Specifications = medicalItem?.Specifications,
                        BatchNo = detail.BatchNo,
                        ExpiryDate = detail.ExpiryDate,
                        Quantity = detail.Quantity,
                        Unit = medicalItem?.PackageUnit,
                        UnitPrice = detail.UnitPrice,
                        TotalPrice = detail.Quantity * detail.UnitPrice,
                        Remark = detail.Remark
                    });
                }

                result.Add(inboundOutput);
            }

            return result;
        }

        /// <summary>
        /// 添加耗材入库记录
        /// </summary>
        public async Task<bool> AddInboundAsync(ConsumableInboundInput input)
        {
            try
            {
                var inbound = new ConsumableInbound
                {
                    Id = Guid.NewGuid().ToString(),
                    InboundNo = input.InboundNo,
                    TenantId = input.TenantId,
                    SupplierId = input.SupplierId,
                    InboundDate = input.InboundDate,
                    Operator = input.Operator,
                    Remark = input.Remark,
                    Creator = input.Creator,
                    CreateDate = DateTime.Now
                };

                _consumableInboundRepository.Insert(inbound);

                if (input.Details != null && input.Details.Count > 0)
                {
                    foreach (var detailInput in input.Details)
                    {
                        var detail = new ConsumableInboundDetail
                        {
                            Id = Guid.NewGuid().ToString(),
                            InboundId = inbound.Id,
                            MedicalItemId = detailInput.MedicalItemId,
                            BatchNo = detailInput.BatchNo,
                            ExpiryDate = detailInput.ExpiryDate,
                            Quantity = detailInput.Quantity,
                            UnitPrice = detailInput.UnitPrice,
                            Remark = detailInput.Remark
                        };
                        _consumableInboundDetailRepository.Insert(detail);

                        // 更新库存
                        await UpdateInventoryAsync(detailInput.MedicalItemId, input.TenantId, detailInput.BatchNo, detailInput.ExpiryDate, detailInput.Quantity, detailInput.UnitPrice);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _unitOfWork.DisChanges();
                throw;
            }
        }

        /// <summary>
        /// 更新库存
        /// </summary>
        private async Task UpdateInventoryAsync(string medicalItemId, string tenantId, string batchNo, DateTime? expiryDate, int quantity, decimal unitPrice)
        {
            var inventories = await _consumableInventoryRepository.GetAllAsync(i => i.MedicalItemId == medicalItemId && i.TenantId == tenantId && i.BatchNo == batchNo);
            var existingInventory = inventories.FirstOrDefault();

            if (existingInventory != null)
            {
                existingInventory.StockQuantity += quantity;
                existingInventory.UnitPrice = unitPrice;
                existingInventory.LastInboundDate = DateTime.Now;
                _consumableInventoryRepository.Update(existingInventory);
            }
            else
            {
                var inventory = new ConsumableInventory
                {
                    Id = Guid.NewGuid().ToString(),
                    MedicalItemId = medicalItemId,
                    TenantId = tenantId,
                    BatchNo = batchNo,
                    ExpiryDate = expiryDate,
                    StockQuantity = quantity,
                    UnitPrice = unitPrice,
                    LastInboundDate = DateTime.Now,
                    CreateDate = DateTime.Now
                };
                _consumableInventoryRepository.Insert(inventory);
            }

            // 检查库存预警
            await CheckInventoryWarningAsync(medicalItemId, tenantId);
        }

        /// <summary>
        /// 检查库存预警
        /// </summary>
        private async Task CheckInventoryWarningAsync(string medicalItemId, string tenantId)
        {
            var inventories = await _consumableInventoryRepository.GetAllAsync(i => i.MedicalItemId == medicalItemId && i.TenantId == tenantId);
            var inventory = inventories.FirstOrDefault();
            if (inventory == null) return;

            // 这里可以根据实际业务逻辑设置预警阈值
            int warningThreshold = 10; // 示例预警阈值

            if (inventory.StockQuantity <= warningThreshold)
            {
                var warnings = await _consumableInventoryWarningRepository.GetAllAsync(w => w.MedicalItemId == medicalItemId && w.TenantId == tenantId && w.ProcessingStatus == 0);
                var existingWarning = warnings.FirstOrDefault();
                if (existingWarning == null)
                {
                    var warning = new ConsumableInventoryWarning
                {
                    Id = Guid.NewGuid().ToString(),
                    MedicalItemId = medicalItemId,
                    TenantId = tenantId,
                    WarningType = 1, // 库存不足
                    WarningThreshold = warningThreshold,
                    CurrentStock = inventory.StockQuantity,
                    WarningMessage = $"耗材 {medicalItemId} 库存不足，当前库存 {inventory.StockQuantity}，预警阈值 {warningThreshold}",
                    ProcessingStatus = 0, // 未处理
                    CreateDate = DateTime.Now
                };
                _consumableInventoryWarningRepository.Insert(warning);
                }
            }
        }

        /// <summary>
        /// 获取耗材出库记录列表
        /// </summary>
        public async Task<List<ConsumableOutboundOutput>> GetOutboundsAsync()
        {
            var outbounds = await _consumableOutboundRepository.GetAllAsync();
            var result = new List<ConsumableOutboundOutput>();

            foreach (var outbound in outbounds)
            {
                var details = await _consumableOutboundDetailRepository.GetAllAsync(d => d.OutboundId == outbound.Id);
                var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == outbound.TenantId);

                var outboundOutput = new ConsumableOutboundOutput
                {
                    Id = outbound.Id,
                    OutboundNo = outbound.OutboundNo,
                    TenantId = outbound.TenantId,
                    TenantName = tenant?.TenantName,
                    DepartmentId = outbound.DepartmentId,
                    DepartmentName = outbound.DepartmentName,
                    OutboundDate = outbound.OutboundDate,
                    Operator = outbound.Operator,
                    Remark = outbound.Remark,
                    Creator = outbound.Creator,
                    CreateDate = outbound.CreateDate,
                    Details = new List<ConsumableOutboundDetailOutput>()
                };

                foreach (var detail in details)
                {
                    var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(m => m.Id == detail.MedicalItemId);
                    outboundOutput.Details.Add(new ConsumableOutboundDetailOutput
                    {
                        Id = detail.Id,
                        OutboundId = detail.OutboundId,
                        MedicalItemId = detail.MedicalItemId,
                        ConsumableName = medicalItem?.MedicalItemName,
                        Specifications = medicalItem?.Specifications,
                        BatchNo = detail.BatchNo,
                        Quantity = detail.Quantity,
                        Unit = medicalItem?.PackageUnit,
                        Remark = detail.Remark
                    });
                }

                result.Add(outboundOutput);
            }

            return result;
        }

        /// <summary>
        /// 添加耗材出库记录
        /// </summary>
        public async Task<bool> AddOutboundAsync(ConsumableOutboundInput input)
        {
            try
            {
                var outbound = new ConsumableOutbound
                {
                    Id = Guid.NewGuid().ToString(),
                    OutboundNo = input.OutboundNo,
                    TenantId = input.TenantId,
                    DepartmentId = input.DepartmentId,
                    OutboundDate = input.OutboundDate,
                    Operator = input.Operator,
                    Remark = input.Remark,
                    Creator = input.Creator,
                    CreateDate = DateTime.Now
                };

                _consumableOutboundRepository.Insert(outbound);

                if (input.Details != null && input.Details.Count > 0)
                {
                    foreach (var detailInput in input.Details)
                    {
                        var detail = new ConsumableOutboundDetail
                        {
                            Id = Guid.NewGuid().ToString(),
                            OutboundId = outbound.Id,
                            MedicalItemId = detailInput.MedicalItemId,
                            BatchNo = detailInput.BatchNo,
                            Quantity = detailInput.Quantity,
                            Remark = detailInput.Remark
                        };
                        _consumableOutboundDetailRepository.Insert(detail);

                        // 扣减库存
                        await DeductInventoryAsync(detailInput.MedicalItemId, input.TenantId, detailInput.BatchNo, detailInput.Quantity);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _unitOfWork.DisChanges();
                throw;
            }
        }

        /// <summary>
        /// 扣减库存
        /// </summary>
        private async Task DeductInventoryAsync(string medicalItemId, string tenantId, string batchNo, int quantity)
        {
            var inventories = await _consumableInventoryRepository.GetAllAsync(i => i.MedicalItemId == medicalItemId && i.TenantId == tenantId && i.BatchNo == batchNo);
            var inventory = inventories.FirstOrDefault();
            if (inventory == null || inventory.StockQuantity < quantity)
            {
                throw new Exception("库存不足");
            }

            inventory.StockQuantity -= quantity;
            inventory.LastOutboundDate = DateTime.Now;
            _consumableInventoryRepository.Update(inventory);

            // 检查库存预警
            await CheckInventoryWarningAsync(medicalItemId, tenantId);
        }

        /// <summary>
        /// 获取耗材库存预警列表
        /// </summary>
        public async Task<List<ConsumableInventoryWarningOutput>> GetInventoryWarningsAsync()
        {
            var warnings = await _consumableInventoryWarningRepository.GetAllAsync();
            var result = new List<ConsumableInventoryWarningOutput>();

            foreach (var warning in warnings)
            {
                var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(m => m.Id == warning.MedicalItemId);
                var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == warning.TenantId);

                var warningOutput = new ConsumableInventoryWarningOutput
                {
                    Id = warning.Id,
                    MedicalItemId = warning.MedicalItemId,
                    ConsumableName = medicalItem?.MedicalItemName,
                    Specifications = medicalItem?.Specifications,
                    TenantId = warning.TenantId,
                    TenantName = tenant?.TenantName,
                    WarningType = warning.WarningType,
                    WarningTypeText = GetWarningTypeText(warning.WarningType),
                    WarningThreshold = warning.WarningThreshold,
                    CurrentStock = warning.CurrentStock,
                    WarningMessage = warning.WarningMessage,
                    ProcessingStatus = warning.ProcessingStatus,
                    ProcessingStatusText = GetProcessingStatusText(warning.ProcessingStatus),
                    Handler = warning.Handler,
                    ProcessingTime = warning.ProcessingTime,
                    ProcessingResult = warning.ProcessingResult,
                    Creator = warning.Creator,
                    CreateDate = warning.CreateDate
                };

                result.Add(warningOutput);
            }

            return result;
        }

        /// <summary>
        /// 处理库存预警
        /// </summary>
        public async Task<bool> HandleInventoryWarningAsync(string id, int processingStatus, string handler, string processingResult)
        {
            var warning = await _consumableInventoryWarningRepository.GetFirstOrDefaultAsync(w => w.Id == id);
            if (warning == null) return false;

            warning.ProcessingStatus = processingStatus;
            warning.Handler = handler;
            warning.ProcessingTime = DateTime.Now;
            warning.ProcessingResult = processingResult;

            _consumableInventoryWarningRepository.Update(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 查询耗材库存
        /// </summary>
        public async Task<List<ConsumableInventoryOutput>> QueryInventoryAsync(ConsumableInventoryQueryInput input)
        {
            var inventories = await _consumableInventoryRepository.GetAllAsync(i => 
                (string.IsNullOrEmpty(input.TenantId) || i.TenantId == input.TenantId) &&
                (string.IsNullOrEmpty(input.MedicalItemId) || i.MedicalItemId == input.MedicalItemId)
            );

            var result = new List<ConsumableInventoryOutput>();
            foreach (var inventory in inventories)
            {
                var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(m => m.Id == inventory.MedicalItemId);
                var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == inventory.TenantId);

                if (!string.IsNullOrEmpty(input.ConsumableName) && (medicalItem == null || !medicalItem.MedicalItemName.Contains(input.ConsumableName)))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(input.BatchNo) && !inventory.BatchNo.Contains(input.BatchNo))
                {
                    continue;
                }

                var inventoryOutput = new ConsumableInventoryOutput
                {
                    Id = inventory.Id,
                    MedicalItemId = inventory.MedicalItemId,
                    ConsumableName = medicalItem?.MedicalItemName,
                    Specifications = medicalItem?.Specifications,
                    BatchNo = inventory.BatchNo,
                    ExpiryDate = inventory.ExpiryDate,
                    StockQuantity = inventory.StockQuantity,
                    Unit = medicalItem?.PackageUnit,
                    UnitPrice = inventory.UnitPrice,
                    TenantId = inventory.TenantId,
                    TenantName = tenant?.TenantName,
                    CreateDate = inventory.CreateDate,
                    LastInboundDate = inventory.LastInboundDate,
                    LastOutboundDate = inventory.LastOutboundDate
                };

                result.Add(inventoryOutput);
            }

            // 分页处理
            if (input.PageSize > 0)
            {
                result = result.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToList();
            }

            return result;
        }

        /// <summary>
        /// 获取预警类型文本
        /// </summary>
        private string GetWarningTypeText(int warningType)
        {
            switch (warningType)
            {
                case 1: return "库存不足";
                case 2: return "即将过期";
                case 3: return "库存过多";
                default: return "未知";
            }
        }

        /// <summary>
        /// 获取处理状态文本
        /// </summary>
        private string GetProcessingStatusText(int processingStatus)
        {
            switch (processingStatus)
            {
                case 0: return "未处理";
                case 1: return "处理中";
                case 2: return "已处理";
                default: return "未知";
            }
        }
    }
}
