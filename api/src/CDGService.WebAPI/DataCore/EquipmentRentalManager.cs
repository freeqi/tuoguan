using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class EquipmentRentalManager
    {
        private readonly IRepository<EquipmentRental> _equipmentRentalRepository;
        private readonly IRepository<EquipmentRentalDetail> _equipmentRentalDetailRepository;
        private readonly IRepository<EquipmentRentalExtension> _equipmentRentalExtensionRepository;
        private readonly IRepository<EquipmentInfo> _equipmentInfoRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentRentalManager(
            IRepository<EquipmentRental> equipmentRentalRepository,
            IRepository<EquipmentRentalDetail> equipmentRentalDetailRepository,
            IRepository<EquipmentRentalExtension> equipmentRentalExtensionRepository,
            IRepository<EquipmentInfo> equipmentInfoRepository,
            IRepository<Tenant> tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _equipmentRentalRepository = equipmentRentalRepository;
            _equipmentRentalDetailRepository = equipmentRentalDetailRepository;
            _equipmentRentalExtensionRepository = equipmentRentalExtensionRepository;
            _equipmentInfoRepository = equipmentInfoRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取设备租用列表
        /// </summary>
        public async Task<List<EquipmentRentalOutput>> GetEquipmentRentalsAsync()
        {
            var rentals = await _equipmentRentalRepository.GetAllAsync();
            var result = new List<EquipmentRentalOutput>();

            foreach (var rental in rentals)
            {
                var details = await _equipmentRentalDetailRepository.GetAllAsync(d => d.RentalId == rental.Id);
                var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == rental.TenantId);

                var rentalOutput = new EquipmentRentalOutput
                {
                    Id = rental.Id,
                    TenantId = rental.TenantId,
                    TenantName = tenant?.TenantName,
                    RentalNo = rental.RentalNo,
                    ContactPerson = rental.ContactPerson,
                    ContactPhone = rental.ContactPhone,
                    DeliveryAddress = rental.DeliveryAddress,
                    RentalDate = rental.RentalDate,
                    EstimatedReturnDate = rental.EstimatedReturnDate,
                    ActualReturnDate = rental.ActualReturnDate,
                    Status = rental.Status,
                    StatusText = GetStatusText(rental.Status),
                    Remark = rental.Remark,
                    Applicant = rental.Applicant,
                    Approver = rental.Approver,
                    ApprovalDate = rental.ApprovalDate,
                    Creator = rental.Creator,
                    CreateDate = rental.CreateDate,
                    Details = new List<EquipmentRentalDetailOutput>()
                };

                foreach (var detail in details)
                {
                    var equipment = await _equipmentInfoRepository.GetFirstOrDefaultAsync(e => e.Id == detail.EquipmentId);
                    rentalOutput.Details.Add(new EquipmentRentalDetailOutput
                    {
                        Id = detail.Id,
                        RentalId = detail.RentalId,
                        EquipmentId = detail.EquipmentId,
                        EquipmentName = equipment?.Name,
                        EquipmentModel = equipment?.Model,
                        Quantity = detail.Quantity,
                        Remark = detail.Remark
                    });
                }

                result.Add(rentalOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备租用
        /// </summary>
        public async Task<EquipmentRentalOutput> GetEquipmentRentalByIdAsync(string id)
        {
            var rental = await _equipmentRentalRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (rental == null) return null;

            var details = await _equipmentRentalDetailRepository.GetAllAsync(d => d.RentalId == id);
            var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == rental.TenantId);

            var result = new EquipmentRentalOutput
            {
                Id = rental.Id,
                TenantId = rental.TenantId,
                TenantName = tenant?.TenantName,
                RentalNo = rental.RentalNo,
                ContactPerson = rental.ContactPerson,
                ContactPhone = rental.ContactPhone,
                DeliveryAddress = rental.DeliveryAddress,
                RentalDate = rental.RentalDate,
                EstimatedReturnDate = rental.EstimatedReturnDate,
                ActualReturnDate = rental.ActualReturnDate,
                Status = rental.Status,
                StatusText = GetStatusText(rental.Status),
                Remark = rental.Remark,
                Applicant = rental.Applicant,
                Approver = rental.Approver,
                ApprovalDate = rental.ApprovalDate,
                Creator = rental.Creator,
                CreateDate = rental.CreateDate,
                Details = new List<EquipmentRentalDetailOutput>()
            };

            foreach (var detail in details)
            {
                var equipment = await _equipmentInfoRepository.GetFirstOrDefaultAsync(e => e.Id == detail.EquipmentId);
                result.Details.Add(new EquipmentRentalDetailOutput
                {
                    Id = detail.Id,
                    RentalId = detail.RentalId,
                    EquipmentId = detail.EquipmentId,
                    EquipmentName = equipment?.Name,
                    EquipmentModel = equipment?.Model,
                    Quantity = detail.Quantity,
                    Remark = detail.Remark
                });
            }

            return result;
        }

        /// <summary>
        /// 添加设备租用
        /// </summary>
        public async Task<bool> AddEquipmentRentalAsync(EquipmentRentalInput input)
        {
            try
            {
                var rental = new EquipmentRental
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = input.TenantId,
                    RentalNo = input.RentalNo,
                    ContactPerson = input.ContactPerson,
                    ContactPhone = input.ContactPhone,
                    DeliveryAddress = input.DeliveryAddress,
                    RentalDate = input.RentalDate,
                    EstimatedReturnDate = input.EstimatedReturnDate,
                    ActualReturnDate = input.ActualReturnDate,
                    Status = input.Status,
                    Remark = input.Remark,
                    Applicant = input.Applicant,
                    Approver = input.Approver,
                    ApprovalDate = input.ApprovalDate,
                    Creator = input.Creator,
                    CreateDate = DateTime.Now
                };

                _equipmentRentalRepository.Insert(rental);

                if (input.Details != null && input.Details.Count > 0)
                {
                    foreach (var detailInput in input.Details)
                    {
                        var detail = new EquipmentRentalDetail
                        {
                            Id = Guid.NewGuid().ToString(),
                            RentalId = rental.Id,
                            EquipmentId = detailInput.EquipmentId,
                            Quantity = detailInput.Quantity,
                            Remark = detailInput.Remark
                        };
                        _equipmentRentalDetailRepository.Insert(detail);
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
        /// 更新设备租用
        /// </summary>
        public async Task<bool> UpdateEquipmentRentalAsync(EquipmentRentalInput input)
        {
            try
            {
                var rental = await _equipmentRentalRepository.GetFirstOrDefaultAsync(r => r.Id == input.Id);
                if (rental == null) return false;

                rental.TenantId = input.TenantId;
                rental.RentalNo = input.RentalNo;
                rental.ContactPerson = input.ContactPerson;
                rental.ContactPhone = input.ContactPhone;
                rental.DeliveryAddress = input.DeliveryAddress;
                rental.RentalDate = input.RentalDate;
                rental.EstimatedReturnDate = input.EstimatedReturnDate;
                rental.ActualReturnDate = input.ActualReturnDate;
                rental.Status = input.Status;
                rental.Remark = input.Remark;
                rental.Applicant = input.Applicant;
                rental.Approver = input.Approver;
                rental.ApprovalDate = input.ApprovalDate;

                _equipmentRentalRepository.Update(rental);

                // 删除旧的详情
                var oldDetails = await _equipmentRentalDetailRepository.GetAllAsync(d => d.RentalId == input.Id);
                foreach (var oldDetail in oldDetails)
                {
                    _equipmentRentalDetailRepository.Remove(oldDetail);
                }

                // 添加新的详情
                if (input.Details != null && input.Details.Count > 0)
                {
                    foreach (var detailInput in input.Details)
                    {
                        var detail = new EquipmentRentalDetail
                        {
                            Id = Guid.NewGuid().ToString(),
                            RentalId = rental.Id,
                            EquipmentId = detailInput.EquipmentId,
                            Quantity = detailInput.Quantity,
                            Remark = detailInput.Remark
                        };
                        _equipmentRentalDetailRepository.Insert(detail);
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
        /// 审批设备租用
        /// </summary>
        public async Task<bool> ApproveEquipmentRentalAsync(string id, int status, string approver)
        {
            var rental = await _equipmentRentalRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (rental == null) return false;

            rental.Status = status;
            rental.Approver = approver;
            rental.ApprovalDate = DateTime.Now;

            _equipmentRentalRepository.Update(rental);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 归还设备
        /// </summary>
        public async Task<bool> ReturnEquipmentAsync(string id, DateTime actualReturnDate)
        {
            var rental = await _equipmentRentalRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (rental == null) return false;

            rental.Status = 3; // 已归还
            rental.ActualReturnDate = actualReturnDate;

            _equipmentRentalRepository.Update(rental);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 添加设备租用延长申请
        /// </summary>
        public async Task<bool> AddEquipmentRentalExtensionAsync(EquipmentRentalExtensionInput input)
        {
            var extension = new EquipmentRentalExtension
            {
                Id = Guid.NewGuid().ToString(),
                RentalId = input.RentalId,
                OriginalReturnDate = input.OriginalReturnDate,
                NewReturnDate = input.NewReturnDate,
                Reason = input.Reason,
                Status = input.Status,
                Applicant = input.Applicant,
                Approver = input.Approver,
                ApprovalDate = input.ApprovalDate,
                CreateDate = DateTime.Now
            };

            _equipmentRentalExtensionRepository.Insert(extension);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 审批设备租用延长申请
        /// </summary>
        public async Task<bool> ApproveEquipmentRentalExtensionAsync(string id, int status, string approver)
        {
            var extension = await _equipmentRentalExtensionRepository.GetFirstOrDefaultAsync(e => e.Id == id);
            if (extension == null) return false;

            try
            {
                extension.Status = status;
                extension.Approver = approver;
                extension.ApprovalDate = DateTime.Now;

                _equipmentRentalExtensionRepository.Update(extension);

                // 如果审批通过，更新租用的预计归还日期
                if (status == 1)
                {
                    var rental = await _equipmentRentalRepository.GetFirstOrDefaultAsync(r => r.Id == extension.RentalId);
                    if (rental != null)
                    {
                        rental.EstimatedReturnDate = extension.NewReturnDate;
                        _equipmentRentalRepository.Update(rental);
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
        /// 获取设备租用延长申请列表
        /// </summary>
        public async Task<List<EquipmentRentalExtensionOutput>> GetEquipmentRentalExtensionsAsync()
        {
            var extensions = await _equipmentRentalExtensionRepository.GetAllAsync();
            var result = new List<EquipmentRentalExtensionOutput>();

            foreach (var extension in extensions)
            {
                var rental = await _equipmentRentalRepository.GetFirstOrDefaultAsync(r => r.Id == extension.RentalId);
                result.Add(new EquipmentRentalExtensionOutput
                {
                    Id = extension.Id,
                    RentalId = extension.RentalId,
                    RentalNo = rental?.RentalNo,
                    OriginalReturnDate = extension.OriginalReturnDate,
                    NewReturnDate = extension.NewReturnDate,
                    Reason = extension.Reason,
                    Status = extension.Status,
                    StatusText = GetStatusText(extension.Status),
                    Applicant = extension.Applicant,
                    Approver = extension.Approver,
                    ApprovalDate = extension.ApprovalDate,
                    CreateDate = extension.CreateDate
                });
            }

            return result;
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
                case 3: return "已归还";
                default: return "未知";
            }
        }
    }
}
