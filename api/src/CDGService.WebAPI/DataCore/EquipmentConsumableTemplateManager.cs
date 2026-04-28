using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class EquipmentConsumableTemplateManager
    {
        private readonly IRepository<EquipmentConsumableTemplate> _equipmentConsumableTemplateRepository;
        private readonly IRepository<EquipmentConsumableTemplateDetail> _equipmentConsumableTemplateDetailRepository;
        private readonly IRepository<ConsumablePurchaseRequest> _consumablePurchaseRequestRepository;
        private readonly IRepository<ConsumablePurchaseRequestDetail> _consumablePurchaseRequestDetailRepository;
        private readonly IRepository<MedicalItemRecord> _medicalItemRecordRepository;
        private readonly IRepository<EquipmentType> _equipmentTypeRepository;
        private readonly IRepository<EquipmentModel> _equipmentModelRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentConsumableTemplateManager(
            IRepository<EquipmentConsumableTemplate> equipmentConsumableTemplateRepository,
            IRepository<EquipmentConsumableTemplateDetail> equipmentConsumableTemplateDetailRepository,
            IRepository<ConsumablePurchaseRequest> consumablePurchaseRequestRepository,
            IRepository<ConsumablePurchaseRequestDetail> consumablePurchaseRequestDetailRepository,
            IRepository<MedicalItemRecord> medicalItemRecordRepository,
            IRepository<EquipmentType> equipmentTypeRepository,
            IRepository<EquipmentModel> equipmentModelRepository,
            IRepository<Tenant> tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _equipmentConsumableTemplateRepository = equipmentConsumableTemplateRepository;
            _equipmentConsumableTemplateDetailRepository = equipmentConsumableTemplateDetailRepository;
            _consumablePurchaseRequestRepository = consumablePurchaseRequestRepository;
            _consumablePurchaseRequestDetailRepository = consumablePurchaseRequestDetailRepository;
            _medicalItemRecordRepository = medicalItemRecordRepository;
            _equipmentTypeRepository = equipmentTypeRepository;
            _equipmentModelRepository = equipmentModelRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取设备耗材模板列表
        /// </summary>
        public async Task<List<EquipmentConsumableTemplateOutput>> GetTemplatesAsync()
        {
            var templates = await _equipmentConsumableTemplateRepository.GetAllAsync();
            var result = new List<EquipmentConsumableTemplateOutput>();

            foreach (var template in templates)
            {
                var details = await _equipmentConsumableTemplateDetailRepository.GetAllAsync(d => d.TemplateId == template.Id);
                var equipmentType = await _equipmentTypeRepository.GetFirstOrDefaultAsync(et => et.Id == template.EquipmentTypeId);
                var equipmentModel = await _equipmentModelRepository.GetFirstOrDefaultAsync(em => em.Id == template.EquipmentModelId);

                var templateOutput = new EquipmentConsumableTemplateOutput
                {
                    Id = template.Id,
                    TemplateName = template.TemplateName,
                    EquipmentTypeId = template.EquipmentTypeId,
                    EquipmentTypeName = equipmentType?.TypeName,
                    EquipmentModelId = template.EquipmentModelId,
                    EquipmentModelName = equipmentModel?.ModelName,
                    TemplateType = template.TemplateType,
                    TemplateTypeText = template.TemplateType == 1 ? "标准模板" : "自定义模板",
                    Status = template.Status,
                    StatusText = template.Status == 1 ? "启用" : "停用",
                    CenterId = template.CenterId,
                    Remark = template.Remark,
                    Founder = template.Founder,
                    FounderDate = template.FounderDate,
                    Modifier = template.Modifier,
                    ModifierDate = template.ModifierDate,
                    Details = new List<EquipmentConsumableTemplateDetailOutput>()
                };

                foreach (var detail in details)
                {
                    var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(mi => mi.Id == detail.MedicalItemId);
                    templateOutput.Details.Add(new EquipmentConsumableTemplateDetailOutput
                    {
                        Id = detail.Id,
                        TemplateId = detail.TemplateId,
                        MedicalItemId = detail.MedicalItemId,
                        ConsumableName = detail.ConsumableName,
                        Specifications = medicalItem?.Specifications,
                        Quantity = detail.Quantity,
                        Unit = detail.Unit,
                        Remark = detail.Remark,
                        Founder = detail.Founder,
                        FounderDate = detail.FounderDate
                    });
                }

                result.Add(templateOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备耗材模板
        /// </summary>
        public async Task<EquipmentConsumableTemplateOutput> GetTemplateByIdAsync(string id)
        {
            var template = await _equipmentConsumableTemplateRepository.GetFirstOrDefaultAsync(t => t.Id == id);
            if (template == null) return null;

            var details = await _equipmentConsumableTemplateDetailRepository.GetAllAsync(d => d.TemplateId == id);
            var equipmentType = await _equipmentTypeRepository.GetFirstOrDefaultAsync(et => et.Id == template.EquipmentTypeId);
            var equipmentModel = await _equipmentModelRepository.GetFirstOrDefaultAsync(em => em.Id == template.EquipmentModelId);

            var result = new EquipmentConsumableTemplateOutput
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                EquipmentTypeId = template.EquipmentTypeId,
                EquipmentTypeName = equipmentType?.TypeName,
                EquipmentModelId = template.EquipmentModelId,
                EquipmentModelName = equipmentModel?.ModelName,
                TemplateType = template.TemplateType,
                TemplateTypeText = template.TemplateType == 1 ? "标准模板" : "自定义模板",
                Status = template.Status,
                StatusText = template.Status == 1 ? "启用" : "停用",
                CenterId = template.CenterId,
                Remark = template.Remark,
                Founder = template.Founder,
                FounderDate = template.FounderDate,
                Modifier = template.Modifier,
                ModifierDate = template.ModifierDate,
                Details = new List<EquipmentConsumableTemplateDetailOutput>()
            };

            foreach (var detail in details)
            {
                var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(mi => mi.Id == detail.MedicalItemId);
                result.Details.Add(new EquipmentConsumableTemplateDetailOutput
                {
                    Id = detail.Id,
                    TemplateId = detail.TemplateId,
                    MedicalItemId = detail.MedicalItemId,
                    ConsumableName = detail.ConsumableName,
                    Specifications = medicalItem?.Specifications,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,
                    Remark = detail.Remark,
                    Founder = detail.Founder,
                    FounderDate = detail.FounderDate
                });
            }

            return result;
        }

        /// <summary>
        /// 添加设备耗材模板
        /// </summary>
        public async Task<bool> AddTemplateAsync(EquipmentConsumableTemplateInput input)
        {
            var template = new EquipmentConsumableTemplate
            {
                Id = Guid.NewGuid().ToString(),
                TemplateName = input.TemplateName,
                EquipmentTypeId = input.EquipmentTypeId,
                EquipmentModelId = input.EquipmentModelId,
                TemplateType = input.TemplateType,
                Status = input.Status,
                CenterId = input.CenterId,
                Remark = input.Remark,
                Founder = input.Founder,
                FounderDate = DateTime.Now,
                Modifier = input.Modifier,
                ModifierDate = DateTime.Now
            };

            _equipmentConsumableTemplateRepository.Insert(template);

            if (input.Details != null && input.Details.Count > 0)
            {
                foreach (var detailInput in input.Details)
                {
                    var detail = new EquipmentConsumableTemplateDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        TemplateId = template.Id,
                        MedicalItemId = detailInput.MedicalItemId,
                        ConsumableName = detailInput.ConsumableName,
                        Quantity = detailInput.Quantity,
                        Unit = detailInput.Unit,
                        Remark = detailInput.Remark,
                        Founder = detailInput.Founder,
                        FounderDate = DateTime.Now
                    };
                    _equipmentConsumableTemplateDetailRepository.Insert(detail);
                }
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备耗材模板
        /// </summary>
        public async Task<bool> UpdateTemplateAsync(EquipmentConsumableTemplateInput input)
        {
            var template = await _equipmentConsumableTemplateRepository.GetFirstOrDefaultAsync(t => t.Id == input.Id);
            if (template == null) return false;

            template.TemplateName = input.TemplateName;
            template.EquipmentTypeId = input.EquipmentTypeId;
            template.EquipmentModelId = input.EquipmentModelId;
            template.TemplateType = input.TemplateType;
            template.Status = input.Status;
            template.CenterId = input.CenterId;
            template.Remark = input.Remark;
            template.Modifier = input.Modifier;
            template.ModifierDate = DateTime.Now;

            _equipmentConsumableTemplateRepository.Update(template);

            // 删除旧的详情
            var oldDetails = await _equipmentConsumableTemplateDetailRepository.GetAllAsync(d => d.TemplateId == input.Id);
            foreach (var oldDetail in oldDetails)
            {
                _equipmentConsumableTemplateDetailRepository.Remove(oldDetail);
            }

            // 添加新的详情
            if (input.Details != null && input.Details.Count > 0)
            {
                foreach (var detailInput in input.Details)
                {
                    var detail = new EquipmentConsumableTemplateDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        TemplateId = template.Id,
                        MedicalItemId = detailInput.MedicalItemId,
                        ConsumableName = detailInput.ConsumableName,
                        Quantity = detailInput.Quantity,
                        Unit = detailInput.Unit,
                        Remark = detailInput.Remark,
                        Founder = detailInput.Founder,
                        FounderDate = DateTime.Now
                    };
                    _equipmentConsumableTemplateDetailRepository.Insert(detail);
                }
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备耗材模板
        /// </summary>
        public async Task<bool> DeleteTemplateAsync(string id)
        {
            var template = await _equipmentConsumableTemplateRepository.GetFirstOrDefaultAsync(t => t.Id == id);
            if (template == null) return false;

            // 删除模板详情
            var details = await _equipmentConsumableTemplateDetailRepository.GetAllAsync(d => d.TemplateId == id);
            foreach (var detail in details)
            {
                _equipmentConsumableTemplateDetailRepository.Remove(detail);
            }

            // 删除模板
            _equipmentConsumableTemplateRepository.Remove(template);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取耗材采购申请列表
        /// </summary>
        public async Task<List<ConsumablePurchaseRequestOutput>> GetPurchaseRequestsAsync()
        {
            var requests = await _consumablePurchaseRequestRepository.GetAllAsync();
            var result = new List<ConsumablePurchaseRequestOutput>();

            foreach (var request in requests)
            {
                var details = await _consumablePurchaseRequestDetailRepository.GetAllAsync(d => d.RequestId == request.Id);
                var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == request.TenantId);

                var requestOutput = new ConsumablePurchaseRequestOutput
                {
                    Id = request.Id,
                    RequestNo = request.RequestNo,
                    TenantId = request.TenantId,
                    TenantName = tenant?.TenantName,
                    RequestDate = request.RequestDate,
                    EstimatedArrivalDate = request.EstimatedArrivalDate,
                    Status = request.Status,
                    StatusText = GetStatusText(request.Status),
                    Remark = request.Remark,
                    Applicant = request.Applicant,
                    Approver = request.Approver,
                    ApprovalDate = request.ApprovalDate,
                    Creator = request.Creator,
                    CreateDate = request.CreateDate,
                    Details = new List<ConsumablePurchaseRequestDetailOutput>()
                };

                foreach (var detail in details)
                {
                    var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(mi => mi.Id == detail.MedicalItemId);
                    requestOutput.Details.Add(new ConsumablePurchaseRequestDetailOutput
                    {
                        Id = detail.Id,
                        RequestId = detail.RequestId,
                        MedicalItemId = detail.MedicalItemId,
                        ConsumableName = medicalItem?.MedicalItemName,
                        Specifications = medicalItem?.Specifications,
                        PurchaseQuantity = detail.PurchaseQuantity,
                        Unit = detail.Unit,
                        EstimatedUnitPrice = detail.EstimatedUnitPrice,
                        EstimatedTotalPrice = detail.EstimatedUnitPrice * detail.PurchaseQuantity,
                        Remark = detail.Remark
                    });
                }

                result.Add(requestOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取耗材采购申请
        /// </summary>
        public async Task<ConsumablePurchaseRequestOutput> GetPurchaseRequestByIdAsync(string id)
        {
            var request = await _consumablePurchaseRequestRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return null;

            var details = await _consumablePurchaseRequestDetailRepository.GetAllAsync(d => d.RequestId == id);
            var tenant = await _tenantRepository.GetFirstOrDefaultAsync(t => t.Id == request.TenantId);

            var result = new ConsumablePurchaseRequestOutput
            {
                Id = request.Id,
                RequestNo = request.RequestNo,
                TenantId = request.TenantId,
                TenantName = tenant?.TenantName,
                RequestDate = request.RequestDate,
                EstimatedArrivalDate = request.EstimatedArrivalDate,
                Status = request.Status,
                StatusText = GetStatusText(request.Status),
                Remark = request.Remark,
                Applicant = request.Applicant,
                Approver = request.Approver,
                ApprovalDate = request.ApprovalDate,
                Creator = request.Creator,
                CreateDate = request.CreateDate,
                Details = new List<ConsumablePurchaseRequestDetailOutput>()
            };

            foreach (var detail in details)
            {
                var medicalItem = await _medicalItemRecordRepository.GetFirstOrDefaultAsync(mi => mi.Id == detail.MedicalItemId);
                result.Details.Add(new ConsumablePurchaseRequestDetailOutput
                {
                    Id = detail.Id,
                    RequestId = detail.RequestId,
                    MedicalItemId = detail.MedicalItemId,
                    ConsumableName = medicalItem?.MedicalItemName,
                    Specifications = medicalItem?.Specifications,
                    PurchaseQuantity = detail.PurchaseQuantity,
                    Unit = detail.Unit,
                    EstimatedUnitPrice = detail.EstimatedUnitPrice,
                    EstimatedTotalPrice = detail.EstimatedUnitPrice * detail.PurchaseQuantity,
                    Remark = detail.Remark
                });
            }

            return result;
        }

        /// <summary>
        /// 添加耗材采购申请
        /// </summary>
        public async Task<bool> AddPurchaseRequestAsync(ConsumablePurchaseRequestInput input)
        {
            var request = new ConsumablePurchaseRequest
            {
                Id = Guid.NewGuid().ToString(),
                RequestNo = input.RequestNo,
                TenantId = input.TenantId,
                RequestDate = input.RequestDate,
                EstimatedArrivalDate = input.EstimatedArrivalDate,
                Status = input.Status,
                Remark = input.Remark,
                Applicant = input.Applicant,
                Approver = input.Approver,
                ApprovalDate = input.ApprovalDate,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            _consumablePurchaseRequestRepository.Insert(request);

            if (input.Details != null && input.Details.Count > 0)
            {
                foreach (var detailInput in input.Details)
                {
                    var detail = new ConsumablePurchaseRequestDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        RequestId = request.Id,
                        MedicalItemId = detailInput.MedicalItemId,
                        PurchaseQuantity = detailInput.PurchaseQuantity,
                        Unit = detailInput.Unit,
                        EstimatedUnitPrice = detailInput.EstimatedUnitPrice,
                        Remark = detailInput.Remark
                    };
                    _consumablePurchaseRequestDetailRepository.Insert(detail);
                }
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新耗材采购申请
        /// </summary>
        public async Task<bool> UpdatePurchaseRequestAsync(ConsumablePurchaseRequestInput input)
        {
            var request = await _consumablePurchaseRequestRepository.GetFirstOrDefaultAsync(r => r.Id == input.Id);
            if (request == null) return false;

            request.RequestNo = input.RequestNo;
            request.TenantId = input.TenantId;
            request.RequestDate = input.RequestDate;
            request.EstimatedArrivalDate = input.EstimatedArrivalDate;
            request.Status = input.Status;
            request.Remark = input.Remark;
            request.Applicant = input.Applicant;
            request.Approver = input.Approver;
            request.ApprovalDate = input.ApprovalDate;

            _consumablePurchaseRequestRepository.Update(request);

            // 删除旧的详情
            var oldDetails = await _consumablePurchaseRequestDetailRepository.GetAllAsync(d => d.RequestId == input.Id);
            foreach (var oldDetail in oldDetails)
            {
                _consumablePurchaseRequestDetailRepository.Remove(oldDetail);
            }

            // 添加新的详情
            if (input.Details != null && input.Details.Count > 0)
            {
                foreach (var detailInput in input.Details)
                {
                    var detail = new ConsumablePurchaseRequestDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        RequestId = request.Id,
                        MedicalItemId = detailInput.MedicalItemId,
                        PurchaseQuantity = detailInput.PurchaseQuantity,
                        Unit = detailInput.Unit,
                        EstimatedUnitPrice = detailInput.EstimatedUnitPrice,
                        Remark = detailInput.Remark
                    };
                    _consumablePurchaseRequestDetailRepository.Insert(detail);
                }
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 审批耗材采购申请
        /// </summary>
        public async Task<bool> ApprovePurchaseRequestAsync(string id, int status, string approver)
        {
            var request = await _consumablePurchaseRequestRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return false;

            request.Status = status;
            request.Approver = approver;
            request.ApprovalDate = DateTime.Now;

            _consumablePurchaseRequestRepository.Update(request);
            return await _unitOfWork.SaveChangesAsync() > 0;
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
                case 3: return "已完成";
                default: return "未知";
            }
        }
    }
}
