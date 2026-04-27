using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class EquipmentMonitoringManager
    {
        private readonly IRepository<EquipmentMonitoring> _equipmentMonitoringRepository;
        private readonly IRepository<EquipmentFaultWarning> _equipmentFaultWarningRepository;
        private readonly IRepository<EquipmentFaultRecord> _equipmentFaultRecordRepository;
        private readonly IRepository<Equipment> _equipmentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentMonitoringManager(
            IRepository<EquipmentMonitoring> equipmentMonitoringRepository,
            IRepository<EquipmentFaultWarning> equipmentFaultWarningRepository,
            IRepository<EquipmentFaultRecord> equipmentFaultRecordRepository,
            IRepository<Equipment> equipmentRepository,
            IRepository<Tenant> tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _equipmentMonitoringRepository = equipmentMonitoringRepository;
            _equipmentFaultWarningRepository = equipmentFaultWarningRepository;
            _equipmentFaultRecordRepository = equipmentFaultRecordRepository;
            _equipmentRepository = equipmentRepository;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取设备监控记录列表
        /// </summary>
        public async Task<List<EquipmentMonitoringOutput>> GetMonitoringsAsync()
        {
            var monitorings = await _equipmentMonitoringRepository.GetAllAsync();
            var result = new List<EquipmentMonitoringOutput>();

            foreach (var monitoring in monitorings)
            {
                var equipment = await _equipmentRepository.GetByIdAsync(monitoring.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(monitoring.TenantId);

                var monitoringOutput = new EquipmentMonitoringOutput
                {
                    Id = monitoring.Id,
                    EquipmentId = monitoring.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = monitoring.TenantId,
                    TenantName = tenant?.TenantName,
                    MonitoringTime = monitoring.MonitoringTime,
                    Status = monitoring.Status,
                    StatusText = GetStatusText(monitoring.Status),
                    Temperature = monitoring.Temperature,
                    Humidity = monitoring.Humidity,
                    Pressure = monitoring.Pressure,
                    Voltage = monitoring.Voltage,
                    Current = monitoring.Current,
                    Remark = monitoring.Remark,
                    Creator = monitoring.Creator,
                    CreateDate = monitoring.CreateDate
                };

                result.Add(monitoringOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备监控记录
        /// </summary>
        public async Task<EquipmentMonitoringOutput> GetMonitoringByIdAsync(string id)
        {
            var monitoring = await _equipmentMonitoringRepository.GetByIdAsync(id);
            if (monitoring == null) return null;

            var equipment = await _equipmentRepository.GetByIdAsync(monitoring.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(monitoring.TenantId);

            var result = new EquipmentMonitoringOutput
            {
                Id = monitoring.Id,
                EquipmentId = monitoring.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = monitoring.TenantId,
                TenantName = tenant?.TenantName,
                MonitoringTime = monitoring.MonitoringTime,
                Status = monitoring.Status,
                StatusText = GetStatusText(monitoring.Status),
                Temperature = monitoring.Temperature,
                Humidity = monitoring.Humidity,
                Pressure = monitoring.Pressure,
                Voltage = monitoring.Voltage,
                Current = monitoring.Current,
                Remark = monitoring.Remark,
                Creator = monitoring.Creator,
                CreateDate = monitoring.CreateDate
            };

            return result;
        }

        /// <summary>
        /// 添加设备监控记录
        /// </summary>
        public async Task<bool> AddMonitoringAsync(EquipmentMonitoringInput input)
        {
            var monitoring = new EquipmentMonitoring
            {
                Id = Guid.NewGuid().ToString(),
                EquipmentId = input.EquipmentId,
                TenantId = input.TenantId,
                MonitoringTime = input.MonitoringTime,
                Status = input.Status,
                Temperature = input.Temperature,
                Humidity = input.Humidity,
                Pressure = input.Pressure,
                Voltage = input.Voltage,
                Current = input.Current,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            await _equipmentMonitoringRepository.AddAsync(monitoring);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备监控记录
        /// </summary>
        public async Task<bool> UpdateMonitoringAsync(EquipmentMonitoringInput input)
        {
            var monitoring = await _equipmentMonitoringRepository.GetByIdAsync(input.Id);
            if (monitoring == null) return false;

            monitoring.EquipmentId = input.EquipmentId;
            monitoring.TenantId = input.TenantId;
            monitoring.MonitoringTime = input.MonitoringTime;
            monitoring.Status = input.Status;
            monitoring.Temperature = input.Temperature;
            monitoring.Humidity = input.Humidity;
            monitoring.Pressure = input.Pressure;
            monitoring.Voltage = input.Voltage;
            monitoring.Current = input.Current;
            monitoring.Remark = input.Remark;

            _equipmentMonitoringRepository.Update(monitoring);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备监控记录
        /// </summary>
        public async Task<bool> DeleteMonitoringAsync(string id)
        {
            var monitoring = await _equipmentMonitoringRepository.GetByIdAsync(id);
            if (monitoring == null) return false;

            _equipmentMonitoringRepository.Delete(monitoring);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取设备故障预警列表
        /// </summary>
        public async Task<List<EquipmentFaultWarningOutput>> GetFaultWarningsAsync()
        {
            var warnings = await _equipmentFaultWarningRepository.GetAllAsync();
            var result = new List<EquipmentFaultWarningOutput>();

            foreach (var warning in warnings)
            {
                var equipment = await _equipmentRepository.GetByIdAsync(warning.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(warning.TenantId);

                var warningOutput = new EquipmentFaultWarningOutput
                {
                    Id = warning.Id,
                    EquipmentId = warning.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = warning.TenantId,
                    TenantName = tenant?.TenantName,
                    WarningTime = warning.WarningTime,
                    WarningLevel = warning.WarningLevel,
                    WarningLevelText = GetWarningLevelText(warning.WarningLevel),
                    WarningType = warning.WarningType,
                    WarningContent = warning.WarningContent,
                    ProcessingStatus = warning.ProcessingStatus,
                    ProcessingStatusText = GetProcessingStatusText(warning.ProcessingStatus),
                    Handler = warning.Handler,
                    ProcessingTime = warning.ProcessingTime,
                    ProcessingResult = warning.ProcessingResult,
                    Remark = warning.Remark,
                    Creator = warning.Creator,
                    CreateDate = warning.CreateDate
                };

                result.Add(warningOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备故障预警
        /// </summary>
        public async Task<EquipmentFaultWarningOutput> GetFaultWarningByIdAsync(string id)
        {
            var warning = await _equipmentFaultWarningRepository.GetByIdAsync(id);
            if (warning == null) return null;

            var equipment = await _equipmentRepository.GetByIdAsync(warning.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(warning.TenantId);

            var result = new EquipmentFaultWarningOutput
            {
                Id = warning.Id,
                EquipmentId = warning.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = warning.TenantId,
                TenantName = tenant?.TenantName,
                WarningTime = warning.WarningTime,
                WarningLevel = warning.WarningLevel,
                WarningLevelText = GetWarningLevelText(warning.WarningLevel),
                WarningType = warning.WarningType,
                WarningContent = warning.WarningContent,
                ProcessingStatus = warning.ProcessingStatus,
                ProcessingStatusText = GetProcessingStatusText(warning.ProcessingStatus),
                Handler = warning.Handler,
                ProcessingTime = warning.ProcessingTime,
                ProcessingResult = warning.ProcessingResult,
                Remark = warning.Remark,
                Creator = warning.Creator,
                CreateDate = warning.CreateDate
            };

            return result;
        }

        /// <summary>
        /// 添加设备故障预警
        /// </summary>
        public async Task<bool> AddFaultWarningAsync(EquipmentFaultWarningInput input)
        {
            var warning = new EquipmentFaultWarning
            {
                Id = Guid.NewGuid().ToString(),
                EquipmentId = input.EquipmentId,
                TenantId = input.TenantId,
                WarningTime = input.WarningTime,
                WarningLevel = input.WarningLevel,
                WarningType = input.WarningType,
                WarningContent = input.WarningContent,
                ProcessingStatus = input.ProcessingStatus,
                Handler = input.Handler,
                ProcessingTime = input.ProcessingTime,
                ProcessingResult = input.ProcessingResult,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            await _equipmentFaultWarningRepository.AddAsync(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备故障预警
        /// </summary>
        public async Task<bool> UpdateFaultWarningAsync(EquipmentFaultWarningInput input)
        {
            var warning = await _equipmentFaultWarningRepository.GetByIdAsync(input.Id);
            if (warning == null) return false;

            warning.EquipmentId = input.EquipmentId;
            warning.TenantId = input.TenantId;
            warning.WarningTime = input.WarningTime;
            warning.WarningLevel = input.WarningLevel;
            warning.WarningType = input.WarningType;
            warning.WarningContent = input.WarningContent;
            warning.ProcessingStatus = input.ProcessingStatus;
            warning.Handler = input.Handler;
            warning.ProcessingTime = input.ProcessingTime;
            warning.ProcessingResult = input.ProcessingResult;
            warning.Remark = input.Remark;

            _equipmentFaultWarningRepository.Update(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 处理设备故障预警
        /// </summary>
        public async Task<bool> HandleFaultWarningAsync(string id, int processingStatus, string handler, string processingResult)
        {
            var warning = await _equipmentFaultWarningRepository.GetByIdAsync(id);
            if (warning == null) return false;

            warning.ProcessingStatus = processingStatus;
            warning.Handler = handler;
            warning.ProcessingTime = DateTime.Now;
            warning.ProcessingResult = processingResult;

            _equipmentFaultWarningRepository.Update(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备故障预警
        /// </summary>
        public async Task<bool> DeleteFaultWarningAsync(string id)
        {
            var warning = await _equipmentFaultWarningRepository.GetByIdAsync(id);
            if (warning == null) return false;

            _equipmentFaultWarningRepository.Delete(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取设备故障记录列表
        /// </summary>
        public async Task<List<EquipmentFaultRecordOutput>> GetFaultRecordsAsync()
        {
            var records = await _equipmentFaultRecordRepository.GetAllAsync();
            var result = new List<EquipmentFaultRecordOutput>();

            foreach (var record in records)
            {
                var equipment = await _equipmentRepository.GetByIdAsync(record.EquipmentId);
                var tenant = await _tenantRepository.GetByIdAsync(record.TenantId);

                var recordOutput = new EquipmentFaultRecordOutput
                {
                    Id = record.Id,
                    FaultNo = record.FaultNo,
                    EquipmentId = record.EquipmentId,
                    EquipmentName = equipment?.EquipmentName,
                    TenantId = record.TenantId,
                    TenantName = tenant?.TenantName,
                    FaultOccurTime = record.FaultOccurTime,
                    FaultResolveTime = record.FaultResolveTime,
                    FaultType = record.FaultType,
                    FaultDescription = record.FaultDescription,
                    FaultReason = record.FaultReason,
                    HandlingMethod = record.HandlingMethod,
                    Handler = record.Handler,
                    FaultStatus = record.FaultStatus,
                    FaultStatusText = GetFaultStatusText(record.FaultStatus),
                    Remark = record.Remark,
                    Creator = record.Creator,
                    CreateDate = record.CreateDate
                };

                result.Add(recordOutput);
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取设备故障记录
        /// </summary>
        public async Task<EquipmentFaultRecordOutput> GetFaultRecordByIdAsync(string id)
        {
            var record = await _equipmentFaultRecordRepository.GetByIdAsync(id);
            if (record == null) return null;

            var equipment = await _equipmentRepository.GetByIdAsync(record.EquipmentId);
            var tenant = await _tenantRepository.GetByIdAsync(record.TenantId);

            var result = new EquipmentFaultRecordOutput
            {
                Id = record.Id,
                FaultNo = record.FaultNo,
                EquipmentId = record.EquipmentId,
                EquipmentName = equipment?.EquipmentName,
                TenantId = record.TenantId,
                TenantName = tenant?.TenantName,
                FaultOccurTime = record.FaultOccurTime,
                FaultResolveTime = record.FaultResolveTime,
                FaultType = record.FaultType,
                FaultDescription = record.FaultDescription,
                FaultReason = record.FaultReason,
                HandlingMethod = record.HandlingMethod,
                Handler = record.Handler,
                FaultStatus = record.FaultStatus,
                FaultStatusText = GetFaultStatusText(record.FaultStatus),
                Remark = record.Remark,
                Creator = record.Creator,
                CreateDate = record.CreateDate
            };

            return result;
        }

        /// <summary>
        /// 添加设备故障记录
        /// </summary>
        public async Task<bool> AddFaultRecordAsync(EquipmentFaultRecordInput input)
        {
            var record = new EquipmentFaultRecord
            {
                Id = Guid.NewGuid().ToString(),
                FaultNo = input.FaultNo,
                EquipmentId = input.EquipmentId,
                TenantId = input.TenantId,
                FaultOccurTime = input.FaultOccurTime,
                FaultResolveTime = input.FaultResolveTime,
                FaultType = input.FaultType,
                FaultDescription = input.FaultDescription,
                FaultReason = input.FaultReason,
                HandlingMethod = input.HandlingMethod,
                Handler = input.Handler,
                FaultStatus = input.FaultStatus,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            await _equipmentFaultRecordRepository.AddAsync(record);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备故障记录
        /// </summary>
        public async Task<bool> UpdateFaultRecordAsync(EquipmentFaultRecordInput input)
        {
            var record = await _equipmentFaultRecordRepository.GetByIdAsync(input.Id);
            if (record == null) return false;

            record.FaultNo = input.FaultNo;
            record.EquipmentId = input.EquipmentId;
            record.TenantId = input.TenantId;
            record.FaultOccurTime = input.FaultOccurTime;
            record.FaultResolveTime = input.FaultResolveTime;
            record.FaultType = input.FaultType;
            record.FaultDescription = input.FaultDescription;
            record.FaultReason = input.FaultReason;
            record.HandlingMethod = input.HandlingMethod;
            record.Handler = input.Handler;
            record.FaultStatus = input.FaultStatus;
            record.Remark = input.Remark;

            _equipmentFaultRecordRepository.Update(record);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 解决设备故障
        /// </summary>
        public async Task<bool> ResolveFaultAsync(string id, DateTime faultResolveTime, string handler, string handlingMethod)
        {
            var record = await _equipmentFaultRecordRepository.GetByIdAsync(id);
            if (record == null) return false;

            record.FaultResolveTime = faultResolveTime;
            record.Handler = handler;
            record.HandlingMethod = handlingMethod;
            record.FaultStatus = 2; // 已解决

            _equipmentFaultRecordRepository.Update(record);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备故障记录
        /// </summary>
        public async Task<bool> DeleteFaultRecordAsync(string id)
        {
            var record = await _equipmentFaultRecordRepository.GetByIdAsync(id);
            if (record == null) return false;

            _equipmentFaultRecordRepository.Delete(record);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取状态文本
        /// </summary>
        private string GetStatusText(string status)
        {
            switch (status)
            {
                case "running": return "运行中";
                case "idle": return "待机";
                case "fault": return "故障";
                case "maintenance": return "维护中";
                default: return "未知";
            }
        }

        /// <summary>
        /// 获取预警级别文本
        /// </summary>
        private string GetWarningLevelText(int warningLevel)
        {
            switch (warningLevel)
            {
                case 1: return "低";
                case 2: return "中";
                case 3: return "高";
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

        /// <summary>
        /// 获取故障状态文本
        /// </summary>
        private string GetFaultStatusText(int faultStatus)
        {
            switch (faultStatus)
            {
                case 0: return "待处理";
                case 1: return "处理中";
                case 2: return "已解决";
                case 3: return "无法解决";
                default: return "未知";
            }
        }
    }
}
