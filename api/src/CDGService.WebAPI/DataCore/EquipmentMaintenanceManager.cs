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
        private readonly IRepository<EquipmentInfo> _equipmentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentMaintenanceManager(
            IRepository<EquipmentMaintenance> equipmentMaintenanceRepository,
            IRepository<EquipmentMaintenanceDetail> equipmentMaintenanceDetailRepository,
            IRepository<EquipmentMaintenanceExtension> equipmentMaintenanceExtensionRepository,
            IRepository<EquipmentInfo> equipmentRepository,
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
                var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == maintenance.EquipmentId);

                var maintenanceOutput = new EquipmentMaintenanceOutput
                {
                    Id = maintenance.Id,
                    MaintenanceNo = maintenance.MaintenanceNo,
                    EquipmentId = maintenance.EquipmentId,
                    EquipmentName = equipment?.Name,
                    MaintenanceDate = maintenance.MaintenanceDate,
                    MaintenanceType = Convert.ToInt32(maintenance.MaintenanceType),
                    MaintenanceTypeText = GetMaintenanceTypeText(Convert.ToInt32(maintenance.MaintenanceType)),
                    MaintenanceContent = maintenance.MaintenanceContent,
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
                        PartName = detail.MaintenanceItem,
                        PartModel = detail.MaintenanceResult,
                        Quantity = 1,
                        UnitPrice = 0,
                        TotalPrice = 0,
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
            var maintenance = await _equipmentMaintenanceRepository.GetFirstOrDefaultAsync(m => m.Id == id);
            if (maintenance == null) return null;

            var details = await _equipmentMaintenanceDetailRepository.GetAllAsync(d => d.MaintenanceId == id);
            var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == maintenance.EquipmentId);

            var result = new EquipmentMaintenanceOutput
            {
                Id = maintenance.Id,
                MaintenanceNo = maintenance.MaintenanceNo,
                EquipmentId = maintenance.EquipmentId,
                EquipmentName = equipment?.Name,
                MaintenanceDate = maintenance.MaintenanceDate,
                MaintenanceType = Convert.ToInt32(maintenance.MaintenanceType),
                MaintenanceTypeText = GetMaintenanceTypeText(Convert.ToInt32(maintenance.MaintenanceType)),
                MaintenanceContent = maintenance.MaintenanceContent,
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
                    PartName = detail.MaintenanceItem,
                    PartModel = detail.MaintenanceResult,
                    Quantity = 1,
                    UnitPrice = 0,
                    TotalPrice = 0,
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
            var maintenance = new EquipmentMaintenance
            {
                Id = Guid.NewGuid().ToString(),
                MaintenanceNo = input.MaintenanceNo,
                EquipmentId = input.EquipmentId,
                MaintenanceDate = input.MaintenanceDate,
                MaintenanceType = input.MaintenanceType.ToString(),
                MaintenanceContent = input.MaintenanceContent,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            _equipmentMaintenanceRepository.Insert(maintenance);

            if (input.Details != null && input.Details.Count > 0)
            {
                foreach (var detailInput in input.Details)
                {
                    var detail = new EquipmentMaintenanceDetail
                        {
                            Id = Guid.NewGuid().ToString(),
                            MaintenanceId = maintenance.Id,
                            MaintenanceItem = detailInput.PartName,
                            MaintenanceResult = detailInput.PartModel,
                            Remark = detailInput.Remark
                        };
                    _equipmentMaintenanceDetailRepository.Insert(detail);
                }
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备维保记录
        /// </summary>
        public async Task<bool> UpdateMaintenanceAsync(EquipmentMaintenanceInput input)
        {
            var maintenance = await _equipmentMaintenanceRepository.GetFirstOrDefaultAsync(m => m.Id == input.Id);
            if (maintenance == null) return false;

            maintenance.MaintenanceNo = input.MaintenanceNo;
            maintenance.EquipmentId = input.EquipmentId;
            maintenance.MaintenanceDate = input.MaintenanceDate;
            maintenance.MaintenanceType = input.MaintenanceType.ToString();
            maintenance.MaintenanceContent = input.MaintenanceContent;
            maintenance.Remark = input.Remark;

            _equipmentMaintenanceRepository.Update(maintenance);

            // 删除旧的详情
            var oldDetails = await _equipmentMaintenanceDetailRepository.GetAllAsync(d => d.MaintenanceId == input.Id);
            foreach (var oldDetail in oldDetails)
            {
                _equipmentMaintenanceDetailRepository.Remove(oldDetail);
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
                            MaintenanceItem = detailInput.PartName,
                            MaintenanceResult = detailInput.PartModel,
                            Remark = detailInput.Remark
                        };
                    _equipmentMaintenanceDetailRepository.Insert(detail);
                }
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备维保记录
        /// </summary>
        public async Task<bool> DeleteMaintenanceAsync(string id)
        {
            var maintenance = await _equipmentMaintenanceRepository.GetFirstOrDefaultAsync(m => m.Id == id);
            if (maintenance == null) return false;

            // 删除维保延长申请
            var extensions = await _equipmentMaintenanceExtensionRepository.GetAllAsync(e => e.MaintenanceId == id);
            foreach (var extension in extensions)
            {
                _equipmentMaintenanceExtensionRepository.Remove(extension);
            }

            // 删除维保详情
            var details = await _equipmentMaintenanceDetailRepository.GetAllAsync(d => d.MaintenanceId == id);
            foreach (var detail in details)
            {
                _equipmentMaintenanceDetailRepository.Remove(detail);
            }

            // 删除维保记录
            _equipmentMaintenanceRepository.Remove(maintenance);

            return await _unitOfWork.SaveChangesAsync() > 0;
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
                var maintenance = await _equipmentMaintenanceRepository.GetFirstOrDefaultAsync(m => m.Id == extension.MaintenanceId);
                var equipment = maintenance != null ? await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == maintenance.EquipmentId) : null;

                var extensionOutput = new EquipmentMaintenanceExtensionOutput
                {
                    Id = extension.Id,
                    MaintenanceId = extension.MaintenanceId,
                    MaintenanceNo = maintenance?.MaintenanceNo,
                    EquipmentId = maintenance?.EquipmentId,
                    EquipmentName = equipment?.Name,
                    OriginalEndDate = extension.OriginalCompletionDate,
                    NewEndDate = extension.NewCompletionDate,
                    ExtensionReason = extension.Reason,
                    Status = extension.Status,
                    StatusText = GetStatusText(extension.Status),
                    Applicant = extension.Applicant,
                    Approver = extension.Approver,
                    ApprovalDate = extension.ApprovalDate,
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
            var extension = await _equipmentMaintenanceExtensionRepository.GetFirstOrDefaultAsync(e => e.Id == id);
            if (extension == null) return null;

            var maintenance = await _equipmentMaintenanceRepository.GetFirstOrDefaultAsync(m => m.Id == extension.MaintenanceId);
            var equipment = maintenance != null ? await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == maintenance.EquipmentId) : null;

            var result = new EquipmentMaintenanceExtensionOutput
            {
                Id = extension.Id,
                MaintenanceId = extension.MaintenanceId,
                MaintenanceNo = maintenance?.MaintenanceNo,
                EquipmentId = maintenance?.EquipmentId,
                EquipmentName = equipment?.Name,
                OriginalEndDate = extension.OriginalCompletionDate,
                NewEndDate = extension.NewCompletionDate,
                ExtensionReason = extension.Reason,
                Status = extension.Status,
                StatusText = GetStatusText(extension.Status),
                Applicant = extension.Applicant,
                Approver = extension.Approver,
                ApprovalDate = extension.ApprovalDate,
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
                MaintenanceId = input.MaintenanceId,
                OriginalCompletionDate = input.OriginalEndDate ?? DateTime.Now,
                NewCompletionDate = input.NewEndDate,
                Reason = input.ExtensionReason,
                Status = input.Status,
                Applicant = input.Applicant,
                Approver = input.Approver,
                ApprovalDate = input.ApprovalDate,
                CreateDate = DateTime.Now
            };

            _equipmentMaintenanceExtensionRepository.Insert(extension);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备维保延长申请
        /// </summary>
        public async Task<bool> UpdateMaintenanceExtensionAsync(EquipmentMaintenanceExtensionInput input)
        {
            var extension = await _equipmentMaintenanceExtensionRepository.GetFirstOrDefaultAsync(e => e.Id == input.Id);
            if (extension == null) return false;

            extension.MaintenanceId = input.MaintenanceId;
            extension.OriginalCompletionDate = input.OriginalEndDate ?? DateTime.Now;
            extension.NewCompletionDate = input.NewEndDate;
            extension.Reason = input.ExtensionReason;
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
            var extension = await _equipmentMaintenanceExtensionRepository.GetFirstOrDefaultAsync(e => e.Id == id);
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
            var extension = await _equipmentMaintenanceExtensionRepository.GetFirstOrDefaultAsync(e => e.Id == id);
            if (extension == null) return false;

            _equipmentMaintenanceExtensionRepository.Remove(extension);
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
