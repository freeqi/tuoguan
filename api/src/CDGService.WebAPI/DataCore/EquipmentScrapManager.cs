using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class EquipmentScrapManager
    {
        private readonly IRepository<EquipmentScrap> _equipmentScrapRepository;
        private readonly IRepository<EquipmentReturn> _equipmentReturnRepository;
        private readonly IRepository<EquipmentInspection> _equipmentInspectionRepository;
        private readonly IRepository<Equipment> _equipmentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<EquipmentRental> _equipmentRentalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentScrapManager(
            IRepository<EquipmentScrap> equipmentScrapRepository,
            IRepository<EquipmentReturn> equipmentReturnRepository,
            IRepository<EquipmentInspection> equipmentInspectionRepository,
            IRepository<Equipment> equipmentRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<EquipmentRental> equipmentRentalRepository,
            IUnitOfWork unitOfWork)
        {
            _equipmentScrapRepository = equipmentScrapRepository;
            _equipmentReturnRepository = equipmentReturnRepository;
            _equipmentInspectionRepository = equipmentInspectionRepository;
            _equipmentRepository = equipmentRepository;
            _tenantRepository = tenantRepository;
            _equipmentRentalRepository = equipmentRentalRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取设备报废记录列表
        /// </summary>
        public async Task<List<EquipmentScrapOutput>> GetScrapsAsync()
        {
            var scraps = await _equipmentScrapRepository.GetAllAsync();
            var result = new List<EquipmentScrapOutput>();

            foreach (var scrap in scraps)
            {
                var equipment = await _equipmentRepository.GetByIdAsync(scrap.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(scrap.TenantId);

                var scrapOutput = new EquipmentScrapOutput
                {
                    Id = scrap.Id,
                    ScrapNo = scrap.ScrapNo,
                    EquipmentId = scrap.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = scrap.TenantId,
                    TenantName = tenant?.TenantName,
                    ScrapDate = scrap.ScrapDate,
                    ScrapReasonType = scrap.ScrapReasonType,
                    ScrapReasonTypeText = GetScrapReasonTypeText(scrap.ScrapReasonType),
                    ScrapReason = scrap.ScrapReason,
                    ScrapHandler = scrap.ScrapHandler,
                    Remark = scrap.Remark,
                    Creator = scrap.Creator,
                    CreateDate = scrap.CreateDate
                };

                result.Add(scrapOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备报废记录
        /// </summary>
        public async Task<EquipmentScrapOutput> GetScrapByIdAsync(string id)
        {
            var scrap = await _equipmentScrapRepository.GetByIdAsync(id);
            if (scrap == null) return null;

            var equipment = await _equipmentRepository.GetByIdAsync(scrap.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(scrap.TenantId);

            var result = new EquipmentScrapOutput
            {
                Id = scrap.Id,
                ScrapNo = scrap.ScrapNo,
                EquipmentId = scrap.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = scrap.TenantId,
                TenantName = tenant?.TenantName,
                ScrapDate = scrap.ScrapDate,
                ScrapReasonType = scrap.ScrapReasonType,
                ScrapReasonTypeText = GetScrapReasonTypeText(scrap.ScrapReasonType),
                ScrapReason = scrap.ScrapReason,
                ScrapHandler = scrap.ScrapHandler,
                Remark = scrap.Remark,
                Creator = scrap.Creator,
                CreateDate = scrap.CreateDate
            };

            return result;
        }

        /// <summary>
        /// 添加设备报废记录
        /// </summary>
        public async Task<bool> AddScrapAsync(EquipmentScrapInput input)
        {
            var scrap = new EquipmentScrap
            {
                Id = Guid.NewGuid().ToString(),
                ScrapNo = input.ScrapNo,
                EquipmentId = input.EquipmentId,
                TenantId = input.TenantId,
                ScrapDate = input.ScrapDate,
                ScrapReasonType = input.ScrapReasonType,
                ScrapReason = input.ScrapReason,
                ScrapHandler = input.ScrapHandler,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            await _equipmentScrapRepository.AddAsync(scrap);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备报废记录
        /// </summary>
        public async Task<bool> UpdateScrapAsync(EquipmentScrapInput input)
        {
            var scrap = await _equipmentScrapRepository.GetByIdAsync(input.Id);
            if (scrap == null) return false;

            scrap.ScrapNo = input.ScrapNo;
            scrap.EquipmentId = input.EquipmentId;
            scrap.TenantId = input.TenantId;
            scrap.ScrapDate = input.ScrapDate;
            scrap.ScrapReasonType = input.ScrapReasonType;
            scrap.ScrapReason = input.ScrapReason;
            scrap.ScrapHandler = input.ScrapHandler;
            scrap.Remark = input.Remark;

            _equipmentScrapRepository.Update(scrap);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备报废记录
        /// </summary>
        public async Task<bool> DeleteScrapAsync(string id)
        {
            var scrap = await _equipmentScrapRepository.GetByIdAsync(id);
            if (scrap == null) return false;

            _equipmentScrapRepository.Delete(scrap);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取设备退租记录列表
        /// </summary>
        public async Task<List<EquipmentReturnOutput>> GetReturnsAsync()
        {
            var returns = await _equipmentReturnRepository.GetAllAsync();
            var result = new List<EquipmentReturnOutput>();

            foreach (var returnRecord in returns)
            {
                var equipment = await _equipmentRepository.GetByIdAsync(returnRecord.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(returnRecord.TenantId);
                var rental = await _equipmentRentalRepository.GetByIdAsync(returnRecord.RentalId);

                var returnOutput = new EquipmentReturnOutput
                {
                    Id = returnRecord.Id,
                    ReturnNo = returnRecord.ReturnNo,
                    EquipmentId = returnRecord.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = returnRecord.TenantId,
                    TenantName = tenant?.TenantName,
                    RentalId = returnRecord.RentalId,
                    RentalNo = rental?.RentalNo,
                    ReturnDate = returnRecord.ReturnDate,
                    ReturnStatus = returnRecord.ReturnStatus,
                    ReturnStatusText = GetReturnStatusText(returnRecord.ReturnStatus),
                    ReturnReason = returnRecord.ReturnReason,
                    ReturnHandler = returnRecord.ReturnHandler,
                    Remark = returnRecord.Remark,
                    Creator = returnRecord.Creator,
                    CreateDate = returnRecord.CreateDate
                };

                result.Add(returnOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备退租记录
        /// </summary>
        public async Task<EquipmentReturnOutput> GetReturnByIdAsync(string id)
        {
            var returnRecord = await _equipmentReturnRepository.GetByIdAsync(id);
            if (returnRecord == null) return null;

            var equipment = await _equipmentRepository.GetByIdAsync(returnRecord.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(returnRecord.TenantId);
            var rental = await _equipmentRentalRepository.GetByIdAsync(returnRecord.RentalId);

            var result = new EquipmentReturnOutput
            {
                Id = returnRecord.Id,
                ReturnNo = returnRecord.ReturnNo,
                EquipmentId = returnRecord.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = returnRecord.TenantId,
                TenantName = tenant?.TenantName,
                RentalId = returnRecord.RentalId,
                RentalNo = rental?.RentalNo,
                ReturnDate = returnRecord.ReturnDate,
                ReturnStatus = returnRecord.ReturnStatus,
                ReturnStatusText = GetReturnStatusText(returnRecord.ReturnStatus),
                ReturnReason = returnRecord.ReturnReason,
                ReturnHandler = returnRecord.ReturnHandler,
                Remark = returnRecord.Remark,
                Creator = returnRecord.Creator,
                CreateDate = returnRecord.CreateDate
            };

            return result;
        }

        /// <summary>
        /// 添加设备退租记录
        /// </summary>
        public async Task<bool> AddReturnAsync(EquipmentReturnInput input)
        {
            var returnRecord = new EquipmentReturn
            {
                Id = Guid.NewGuid().ToString(),
                ReturnNo = input.ReturnNo,
                EquipmentId = input.EquipmentId,
                TenantId = input.TenantId,
                RentalId = input.RentalId,
                ReturnDate = input.ReturnDate,
                ReturnStatus = input.ReturnStatus,
                ReturnReason = input.ReturnReason,
                ReturnHandler = input.ReturnHandler,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            await _equipmentReturnRepository.AddAsync(returnRecord);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备退租记录
        /// </summary>
        public async Task<bool> UpdateReturnAsync(EquipmentReturnInput input)
        {
            var returnRecord = await _equipmentReturnRepository.GetByIdAsync(input.Id);
            if (returnRecord == null) return false;

            returnRecord.ReturnNo = input.ReturnNo;
            returnRecord.EquipmentId = input.EquipmentId;
            returnRecord.TenantId = input.TenantId;
            returnRecord.RentalId = input.RentalId;
            returnRecord.ReturnDate = input.ReturnDate;
            returnRecord.ReturnStatus = input.ReturnStatus;
            returnRecord.ReturnReason = input.ReturnReason;
            returnRecord.ReturnHandler = input.ReturnHandler;
            returnRecord.Remark = input.Remark;

            _equipmentReturnRepository.Update(returnRecord);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备退租记录
        /// </summary>
        public async Task<bool> DeleteReturnAsync(string id)
        {
            var returnRecord = await _equipmentReturnRepository.GetByIdAsync(id);
            if (returnRecord == null) return false;

            _equipmentReturnRepository.Delete(returnRecord);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取设备巡检记录列表
        /// </summary>
        public async Task<List<EquipmentInspectionOutput>> GetInspectionsAsync()
        {
            var inspections = await _equipmentInspectionRepository.GetAllAsync();
            var result = new List<EquipmentInspectionOutput>();

            foreach (var inspection in inspections)
            {
                var equipment = await _equipmentRepository.GetByIdAsync(inspection.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(inspection.TenantId);

                var inspectionOutput = new EquipmentInspectionOutput
                {
                    Id = inspection.Id,
                    InspectionNo = inspection.InspectionNo,
                    EquipmentId = inspection.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = inspection.TenantId,
                    TenantName = tenant?.TenantName,
                    InspectionDate = inspection.InspectionDate,
                    InspectionType = inspection.InspectionType,
                    InspectionTypeText = GetInspectionTypeText(inspection.InspectionType),
                    Inspector = inspection.Inspector,
                    InspectionResult = inspection.InspectionResult,
                    InspectionResultText = GetInspectionResultText(inspection.InspectionResult),
                    ProblemDescription = inspection.ProblemDescription,
                    HandlingSuggestion = inspection.HandlingSuggestion,
                    Remark = inspection.Remark,
                    Creator = inspection.Creator,
                    CreateDate = inspection.CreateDate
                };

                result.Add(inspectionOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备巡检记录
        /// </summary>
        public async Task<EquipmentInspectionOutput> GetInspectionByIdAsync(string id)
        {
            var inspection = await _equipmentInspectionRepository.GetByIdAsync(id);
            if (inspection == null) return null;

            var equipment = await _equipmentRepository.GetByIdAsync(inspection.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(inspection.TenantId);

            var result = new EquipmentInspectionOutput
            {
                Id = inspection.Id,
                InspectionNo = inspection.InspectionNo,
                EquipmentId = inspection.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = inspection.TenantId,
                TenantName = tenant?.TenantName,
                InspectionDate = inspection.InspectionDate,
                InspectionType = inspection.InspectionType,
                InspectionTypeText = GetInspectionTypeText(inspection.InspectionType),
                Inspector = inspection.Inspector,
                InspectionResult = inspection.InspectionResult,
                InspectionResultText = GetInspectionResultText(inspection.InspectionResult),
                ProblemDescription = inspection.ProblemDescription,
                HandlingSuggestion = inspection.HandlingSuggestion,
                Remark = inspection.Remark,
                Creator = inspection.Creator,
                CreateDate = inspection.CreateDate
            };

            return result;
        }

        /// <summary>
        /// 添加设备巡检记录
        /// </summary>
        public async Task<bool> AddInspectionAsync(EquipmentInspectionInput input)
        {
            var inspection = new EquipmentInspection
            {
                Id = Guid.NewGuid().ToString(),
                InspectionNo = input.InspectionNo,
                EquipmentId = input.EquipmentId,
                TenantId = input.TenantId,
                InspectionDate = input.InspectionDate,
                InspectionType = input.InspectionType,
                Inspector = input.Inspector,
                InspectionResult = input.InspectionResult,
                ProblemDescription = input.ProblemDescription,
                HandlingSuggestion = input.HandlingSuggestion,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            await _equipmentInspectionRepository.AddAsync(inspection);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备巡检记录
        /// </summary>
        public async Task<bool> UpdateInspectionAsync(EquipmentInspectionInput input)
        {
            var inspection = await _equipmentInspectionRepository.GetByIdAsync(input.Id);
            if (inspection == null) return false;

            inspection.InspectionNo = input.InspectionNo;
            inspection.EquipmentId = input.EquipmentId;
            inspection.TenantId = input.TenantId;
            inspection.InspectionDate = input.InspectionDate;
            inspection.InspectionType = input.InspectionType;
            inspection.Inspector = input.Inspector;
            inspection.InspectionResult = input.InspectionResult;
            inspection.ProblemDescription = input.ProblemDescription;
            inspection.HandlingSuggestion = input.HandlingSuggestion;
            inspection.Remark = input.Remark;

            _equipmentInspectionRepository.Update(inspection);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备巡检记录
        /// </summary>
        public async Task<bool> DeleteInspectionAsync(string id)
        {
            var inspection = await _equipmentInspectionRepository.GetByIdAsync(id);
            if (inspection == null) return false;

            _equipmentInspectionRepository.Delete(inspection);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取报废原因类型文本
        /// </summary>
        private string GetScrapReasonTypeText(int scrapReasonType)
        {
            switch (scrapReasonType)
            {
                case 1: return "设备老化";
                case 2: return "故障无法修复";
                case 3: return "技术淘汰";
                case 4: return "意外损坏";
                case 5: return "其他原因";
                default: return "未知";
            }
        }

        /// <summary>
        /// 获取退租状态文本
        /// </summary>
        private string GetReturnStatusText(int returnStatus)
        {
            switch (returnStatus)
            {
                case 1: return "正常退租";
                case 2: return "提前退租";
                case 3: return "违约退租";
                default: return "未知";
            }
        }

        /// <summary>
        /// 获取巡检类型文本
        /// </summary>
        private string GetInspectionTypeText(int inspectionType)
        {
            switch (inspectionType)
            {
                case 1: return "日常巡检";
                case 2: return "定期巡检";
                case 3: return "专项巡检";
                case 4: return "故障巡检";
                default: return "其他";
            }
        }

        /// <summary>
        /// 获取巡检结果文本
        /// </summary>
        private string GetInspectionResultText(int inspectionResult)
        {
            switch (inspectionResult)
            {
                case 1: return "正常";
                case 2: return "轻微问题";
                case 3: return "严重问题";
                case 4: return "故障";
                default: return "未知";
            }
        }
    }
}
