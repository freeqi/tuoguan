using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class EquipmentMaintenanceManager
    {
        private readonly IRepository<EquipmentMaintenance> _equipmentMaintenanceRepository;
        private readonly IRepository<EquipmentMaintenanceDetail> _equipmentMaintenanceDetailRepository;
        private readonly IRepository<EquipmentMaintenanceExtension> _equipmentMaintenanceExtensionRepository;
        private readonly IRepository<Equipment> _equipmentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentMaintenanceManager(
            IRepository<EquipmentMaintenance> equipmentMaintenanceRepository,
            IRepository<EquipmentMaintenanceDetail> equipmentMaintenanceDetailRepository,
            IRepository<EquipmentMaintenanceExtension> equipmentMaintenanceExtensionRepository,
            IRepository<Equipment> equipmentRepository,
            IRepository<Tenant> tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _equipmentMaintenanceRepository = equipmentMaintenanceRepository;
            _equipmentMaintenanceDetailRepository = equipmentMaintenanceDetailRepository;
            _equipmentMaintenanceExtensionRepository = equipmentMaintenanceExtensionRepository;
            _equipmentRepository = equipmentRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取设备维保记录列表
        /// </summary>
        public async Task<List<EquipmentMaintenanceOutput>> GetMaintenancesAsync()
        {
            var maintenances = await _equipmentMaintenanceRepository.GetAllAsync();
            var result = new List<EquipmentMaintenanceOutput>();

            foreach (var maintenance in maintenances)
            {
                var details = await _equipmentMaintenanceDetailRepository.GetAllAsync(d => d.MaintenanceId == maintenance.Id);
                var equipment = await _equipmentRepository.GetByIdAsync(maintenance.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(maintenance.TenantId);

                var maintenanceOutput = new EquipmentMaintenanceOutput
                {
                    Id = maintenance.Id,
                    MaintenanceNo = maintenance.MaintenanceNo,
                    EquipmentId = maintenance.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = maintenance.TenantId,
                    TenantName = tenant?.TenantName,
                    MaintenanceDate = maintenance.MaintenanceDate,
                    MaintenanceType = maintenance.MaintenanceType,
                    MaintenanceTypeText = GetMaintenanceTypeText(maintenance.MaintenanceType),
                    MaintenanceContent = maintenance.MaintenanceContent,
                    ServiceProvider = maintenance.ServiceProvider,
                    ContactPerson = maintenance.ContactPerson,
                    ContactPhone = maintenance.ContactPhone,
                    MaintenanceCost = maintenance.MaintenanceCost,
                    Remark = maintenance.Remark,
                    Creator = maintenance.Creator,
                    CreateDate = maintenance.CreateDate,
                    Details = new List<EquipmentMaintenanceDetailOutput>()
                };

                foreach (var detail in details)
                {
                    maintenanceOutput.Details.Add(new EquipmentMaintenanceDetailOutput
                    {
                        Id = detail.Id,
                        MaintenanceId = detail.MaintenanceId,
                        PartName = detail.PartName,
                        PartModel = detail.PartModel,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        TotalPrice = detail.Quantity * detail.UnitPrice,
                        Remark = detail.Remark
                    });
                }

                result.Add(maintenanceOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备维保记录
        /// </summary>
        public async Task<EquipmentMaintenanceOutput> GetMaintenanceByIdAsync(string id)
        {
            var maintenance = await _equipmentMaintenanceRepository.GetByIdAsync(id);
            if (maintenance == null) return null;

            var details = await _equipmentMaintenanceDetailRepository.GetAllAsync(d => d.MaintenanceId == id);
            var equipment = await _equipmentRepository.GetByIdAsync(maintenance.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(maintenance.TenantId);

            var result = new EquipmentMaintenanceOutput
            {
                Id = maintenance.Id,
                MaintenanceNo = maintenance.MaintenanceNo,
                EquipmentId = maintenance.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = maintenance.TenantId,
                TenantName = tenant?.TenantName,
                MaintenanceDate = maintenance.MaintenanceDate,
                MaintenanceType = maintenance.MaintenanceType,
                MaintenanceTypeText = GetMaintenanceTypeText(maintenance.MaintenanceType),
                MaintenanceContent = maintenance.MaintenanceContent,
                ServiceProvider = maintenance.ServiceProvider,
                ContactPerson = maintenance.ContactPerson,
                ContactPhone = maintenance.ContactPhone,
                MaintenanceCost = maintenance.MaintenanceCost,
                Remark = maintenance.Remark,
                Creator = maintenance.Creator,
                CreateDate = maintenance.CreateDate,
                Details = new List<EquipmentMaintenanceDetailOutput>()
            };

            foreach (var detail in details)
            {
                result.Details.Add(new EquipmentMaintenanceDetailOutput
                {
                    Id = detail.Id,
                    MaintenanceId = detail.MaintenanceId,
                    PartName = detail.PartName,
                    PartModel = detail.PartModel,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.Quantity * detail.UnitPrice,
                    Remark = detail.Remark
                });
            }

            return result;
        }

        /// <summary>
        /// 添加设备维保记录
        /// </summary>
        public async Task<bool> AddMaintenanceAsync(EquipmentMaintenanceInput input)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var maintenance = new EquipmentMaintenance
                    {
                        Id = Guid.NewGuid().ToString(),
                        MaintenanceNo = input.MaintenanceNo,
                        EquipmentId = input.EquipmentId,
                        TenantId = input.TenantId,
                        MaintenanceDate = input.MaintenanceDate,
                        MaintenanceType = input.MaintenanceType,
                        MaintenanceContent = input.MaintenanceContent,
                        ServiceProvider = input.ServiceProvider,
                        ContactPerson = input.ContactPerson,
                        ContactPhone = input.ContactPhone,
                        MaintenanceCost = input.MaintenanceCost,
                        Remark = input.Remark,
                        Creator = input.Creator,
                        CreateDate = DateTime.Now
                    };

                    await _equipmentMaintenanceRepository.AddAsync(maintenance);

                    if (input.Details != null && input.Details.Count > 0)
                    {
                        foreach (var detailInput in input.Details)
                        {
                            var detail = new EquipmentMaintenanceDetail
                            {
                                Id = Guid.NewGuid().ToString(),
                                MaintenanceId = maintenance.Id,
                                PartName = detailInput.PartName,
                                PartModel = detailInput.PartModel,
                                Quantity = detailInput.Quantity,
                                UnitPrice = detailInput.UnitPrice,
                                Remark = detailInput.Remark
                            };
                            await _equipmentMaintenanceDetailRepository.AddAsync(detail);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// 更新设备维保记录
        /// </summary>
        public async Task<bool> UpdateMaintenanceAsync(EquipmentMaintenanceInput input)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var maintenance = await _equipmentMaintenanceRepository.GetByIdAsync(input.Id);
                    if (maintenance == null) return false;

                    maintenance.MaintenanceNo = input.MaintenanceNo;
                    maintenance.EquipmentId = input.EquipmentId;
                    maintenance.TenantId = input.TenantId;
                    maintenance.MaintenanceDate = input.MaintenanceDate;
                    maintenance.MaintenanceType = input.MaintenanceType;
                    maintenance.MaintenanceContent = input.MaintenanceContent;
                    maintenance.ServiceProvider = input.ServiceProvider;
                    maintenance.ContactPerson = input.ContactPerson;
                    maintenance.ContactPhone = input.ContactPhone;
                    maintenance.MaintenanceCost = input.MaintenanceCost;
                    maintenance.Remark = input.Remark;

                    _equipmentMaintenanceRepository.Update(maintenance);

                    // 删除旧的详情
                    var oldDetails = await _equipmentMaintenanceDetailRepository.GetAllAsync(d => d.MaintenanceId == input.Id);
                    foreach (var oldDetail in oldDetails)
                    {
                        _equipmentMaintenanceDetailRepository.Delete(oldDetail);
                    }

                    // 添加新的详情
                    if (input.Details != null && input.Details.Count > 0)
                    {
                        foreach (var detailInput in input.Details)
                        {
                            var detail = new EquipmentMaintenanceDetail
                            {
                                Id = Guid.NewGuid().ToString(),
                                MaintenanceId = maintenance.Id,
                                PartName = detailInput.PartName,
                                PartModel = detailInput.PartModel,
                                Quantity = detailInput.Quantity,
                                UnitPrice = detailInput.UnitPrice,
                                Remark = detailInput.Remark
                            };
                            await _equipmentMaintenanceDetailRepository.AddAsync(detail);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// 删除设备维保记录
        /// </summary>
        public async Task<bool> DeleteMaintenanceAsync(string id)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var maintenance = await _equipmentMaintenanceRepository.GetByIdAsync(id);
                    if (maintenance == null) return false;

                    // 删除维保延长申请
                    var extensions = await _equipmentMaintenanceExtensionRepository.GetAllAsync(e => e.MaintenanceId == id);
                    foreach (var extension in extensions)
                    {
                        _equipmentMaintenanceExtensionRepository.Delete(extension);
                    }

                    // 删除维保详情
                    var details = await _equipmentMaintenanceDetailRepository.GetAllAsync(d => d.MaintenanceId == id);
                    foreach (var detail in details)
                    {
                        _equipmentMaintenanceDetailRepository.Delete(detail);
                    }

                    // 删除维保记录
                    _equipmentMaintenanceRepository.Delete(maintenance);

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// 获取设备维保延长申请列表
        /// </summary>
        public async Task<List<EquipmentMaintenanceExtensionOutput>> GetMaintenanceExtensionsAsync()
        {
            var extensions = await _equipmentMaintenanceExtensionRepository.GetAllAsync();
            var result = new List<EquipmentMaintenanceExtensionOutput>();

            foreach (var extension in extensions)
            {
                var maintenance = await _equipmentMaintenanceRepository.GetByIdAsync(extension.MaintenanceId);
                var equipment = await _equipmentRepository.GetByIdAsync(extension.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(extension.TenantId);

                var extensionOutput = new EquipmentMaintenanceExtensionOutput
                {
                    Id = extension.Id,
                    ExtensionNo = extension.ExtensionNo,
                    MaintenanceId = extension.MaintenanceId,
                    MaintenanceNo = maintenance?.MaintenanceNo,
                    EquipmentId = extension.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = extension.TenantId,
                    TenantName = tenant?.TenantName,
                    ApplyDate = extension.ApplyDate,
                    OriginalEndDate = extension.OriginalEndDate,
                    NewEndDate = extension.NewEndDate,
                    ExtensionReason = extension.ExtensionReason,
                    Status = extension.Status,
                    StatusText = GetStatusText(extension.Status),
                    Applicant = extension.Applicant,
                    Approver = extension.Approver,
                    ApprovalDate = extension.ApprovalDate,
                    Creator = extension.Creator,
                    CreateDate = extension.CreateDate
                };

                result.Add(extensionOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备维保延长申请
        /// </summary>
        public async Task<EquipmentMaintenanceExtensionOutput> GetMaintenanceExtensionByIdAsync(string id)
        {
            var extension = await _equipmentMaintenanceExtensionRepository.GetByIdAsync(id);
            if (extension == null) return null;

            var maintenance = await _equipmentMaintenanceRepository.GetByIdAsync(extension.MaintenanceId);
            var equipment = await _equipmentRepository.GetByIdAsync(extension.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(extension.TenantId);

            var result = new EquipmentMaintenanceExtensionOutput
            {
                Id = extension.Id,
                ExtensionNo = extension.ExtensionNo,
                MaintenanceId = extension.MaintenanceId,
                MaintenanceNo = maintenance?.MaintenanceNo,
                EquipmentId = extension.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = extension.TenantId,
                TenantName = tenant?.TenantName,
                ApplyDate = extension.ApplyDate,
                OriginalEndDate = extension.OriginalEndDate,
                NewEndDate = extension.NewEndDate,
                ExtensionReason = extension.ExtensionReason,
                Status = extension.Status,
                StatusText = GetStatusText(extension.Status),
                Applicant = extension.Applicant,
                Approver = extension.Approver,
                ApprovalDate = extension.ApprovalDate,
                Creator = extension.Creator,
                CreateDate = extension.CreateDate
            };

            return result;
        }

        /// <summary>
        /// 添加设备维保延长申请
        /// </summary>
        public async Task<bool> AddMaintenanceExtensionAsync(EquipmentMaintenanceExtensionInput input)
        {
            var extension = new EquipmentMaintenanceExtension
            {
                Id = Guid.NewGuid().ToString(),
                ExtensionNo = input.ExtensionNo,
                MaintenanceId = input.MaintenanceId,
                EquipmentId = input.EquipmentId,
                TenantId = input.TenantId,
                ApplyDate = input.ApplyDate,
                OriginalEndDate = input.OriginalEndDate,
                NewEndDate = input.NewEndDate,
                ExtensionReason = input.ExtensionReason,
                Status = input.Status,
                Applicant = input.Applicant,
                Approver = input.Approver,
                ApprovalDate = input.ApprovalDate,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            await _equipmentMaintenanceExtensionRepository.AddAsync(extension);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备维保延长申请
        /// </summary>
        public async Task<bool> UpdateMaintenanceExtensionAsync(EquipmentMaintenanceExtensionInput input)
        {
            var extension = await _equipmentMaintenanceExtensionRepository.GetByIdAsync(input.Id);
            if (extension == null) return false;

            extension.ExtensionNo = input.ExtensionNo;
            extension.MaintenanceId = input.MaintenanceId;
            extension.EquipmentId = input.EquipmentId;
            extension.TenantId = input.TenantId;
            extension.ApplyDate = input.ApplyDate;
            extension.OriginalEndDate = input.OriginalEndDate;
            extension.NewEndDate = input.NewEndDate;
            extension.ExtensionReason = input.ExtensionReason;
            extension.Status = input.Status;
            extension.Applicant = input.Applicant;
            extension.Approver = input.Approver;
            extension.ApprovalDate = input.ApprovalDate;

            _equipmentMaintenanceExtensionRepository.Update(extension);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 审批设备维保延长申请
        /// </summary>
        public async Task<bool> ApproveMaintenanceExtensionAsync(string id, int status, string approver)
        {
            var extension = await _equipmentMaintenanceExtensionRepository.GetByIdAsync(id);
            if (extension == null) return false;

            extension.Status = status;
            extension.Approver = approver;
            extension.ApprovalDate = DateTime.Now;

            _equipmentMaintenanceExtensionRepository.Update(extension);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备维保延长申请
        /// </summary>
        public async Task<bool> DeleteMaintenanceExtensionAsync(string id)
        {
            var extension = await _equipmentMaintenanceExtensionRepository.GetByIdAsync(id);
            if (extension == null) return false;

            _equipmentMaintenanceExtensionRepository.Delete(extension);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取维保类型文本
        /// </summary>
        private string GetMaintenanceTypeText(int maintenanceType)
        {
            switch (maintenanceType)
            {
                case 1: return "日常维护";
                case 2: return "定期保养";
                case 3: return "故障维修";
                case 4: return "预防性维护";
                default: return "其他";
            }
        }

        /// <summary>
        /// 获取状态文本
        /// </summary>
        private string GetStatusText(int status)
        {
            switch (status)
            {
                case 0: return "待审批";
                case 1: return "已审批";
                case 2: return "已拒绝";
                default: return "未知";
            }
        }
    }
}
