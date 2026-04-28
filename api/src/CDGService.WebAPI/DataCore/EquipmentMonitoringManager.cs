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
        private readonly IRepository<EquipmentInfo> _equipmentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentMonitoringManager(
            IRepository<EquipmentMonitoring> equipmentMonitoringRepository,
            IRepository<EquipmentFaultWarning> equipmentFaultWarningRepository,
            IRepository<EquipmentFaultRecord> equipmentFaultRecordRepository,
            IRepository<EquipmentInfo> equipmentRepository,
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
                var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == monitoring.EquipmentId);

                var monitoringOutput = new EquipmentMonitoringOutput
                {
                    Id = monitoring.Id,
                    EquipmentId = monitoring.EquipmentId,
                    EquipmentName = equipment?.Name,
                    MonitoringTime = monitoring.MonitoringTime,
                    Status = monitoring.Status.ToString(),
                    StatusText = GetStatusText(monitoring.Status.ToString()),
                    Remark = monitoring.Remark
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
            var monitoring = await _equipmentMonitoringRepository.GetFirstOrDefaultAsync(m => m.Id == id);
            if (monitoring == null) return null;

            var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == monitoring.EquipmentId);

            var result = new EquipmentMonitoringOutput
            {
                Id = monitoring.Id,
                EquipmentId = monitoring.EquipmentId,
                EquipmentName = equipment?.Name,
                MonitoringTime = monitoring.MonitoringTime,
                Status = monitoring.Status.ToString(),
                StatusText = GetStatusText(monitoring.Status.ToString()),
                Remark = monitoring.Remark
            };

            return result;
        }

        /// <summary>
        /// 添加设备监控记录
        /// </summary>
        public async Task<bool> AddMonitoringAsync(EquipmentMonitoringInput input)
        {
            var parameters = new { input.Temperature, input.Humidity, input.Pressure, input.Voltage, input.Current };
            var operatingParameters = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
            
            var monitoring = new EquipmentMonitoring
            {
                Id = Guid.NewGuid().ToString(),
                EquipmentId = input.EquipmentId,
                MonitoringTime = input.MonitoringTime,
                Status = int.Parse(input.Status),
                OperatingParameters = operatingParameters,
                Remark = input.Remark
            };

            _equipmentMonitoringRepository.Insert(monitoring);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备监控记录
        /// </summary>
        public async Task<bool> UpdateMonitoringAsync(EquipmentMonitoringInput input)
        {
            var monitoring = await _equipmentMonitoringRepository.GetFirstOrDefaultAsync(m => m.Id == input.Id);
            if (monitoring == null) return false;

            var parameters = new { input.Temperature, input.Humidity, input.Pressure, input.Voltage, input.Current };
            var operatingParameters = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);

            monitoring.EquipmentId = input.EquipmentId;
            monitoring.MonitoringTime = input.MonitoringTime;
            monitoring.Status = int.Parse(input.Status);
            monitoring.OperatingParameters = operatingParameters;
            monitoring.Remark = input.Remark;

            _equipmentMonitoringRepository.Update(monitoring);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备监控记录
        /// </summary>
        public async Task<bool> DeleteMonitoringAsync(string id)
        {
            var monitoring = await _equipmentMonitoringRepository.GetFirstOrDefaultAsync(m => m.Id == id);
            if (monitoring == null) return false;

            _equipmentMonitoringRepository.Remove(monitoring);
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
                var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == warning.EquipmentId);

                var warningOutput = new EquipmentFaultWarningOutput
                {
                    Id = warning.Id,
                    EquipmentId = warning.EquipmentId,
                    EquipmentName = equipment?.Name,
                    WarningTime = warning.WarningTime,
                    WarningLevel = warning.WarningLevel,
                    WarningLevelText = GetWarningLevelText(warning.WarningLevel),
                    WarningType = warning.WarningType,
                    WarningContent = warning.WarningContent,
                    ProcessingStatus = warning.ProcessStatus,
                    ProcessingStatusText = GetProcessingStatusText(warning.ProcessStatus),
                    Handler = warning.Processor,
                    ProcessingTime = warning.ProcessTime,
                    ProcessingResult = warning.ProcessResult,
                    Remark = warning.Remark
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
            var warning = await _equipmentFaultWarningRepository.GetFirstOrDefaultAsync(w => w.Id == id);
            if (warning == null) return null;

            var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == warning.EquipmentId);

            var result = new EquipmentFaultWarningOutput
            {
                Id = warning.Id,
                EquipmentId = warning.EquipmentId,
                EquipmentName = equipment?.Name,
                WarningTime = warning.WarningTime,
                WarningLevel = warning.WarningLevel,
                WarningLevelText = GetWarningLevelText(warning.WarningLevel),
                WarningType = warning.WarningType,
                WarningContent = warning.WarningContent,
                ProcessingStatus = warning.ProcessStatus,
                ProcessingStatusText = GetProcessingStatusText(warning.ProcessStatus),
                Handler = warning.Processor,
                ProcessingTime = warning.ProcessTime,
                ProcessingResult = warning.ProcessResult,
                Remark = warning.Remark
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
                WarningTime = input.WarningTime,
                WarningLevel = input.WarningLevel,
                WarningType = input.WarningType,
                WarningContent = input.WarningContent,
                ProcessStatus = input.ProcessingStatus,
                Processor = input.Handler,
                ProcessTime = input.ProcessingTime,
                ProcessResult = input.ProcessingResult,
                Remark = input.Remark
            };

            _equipmentFaultWarningRepository.Insert(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备故障预警
        /// </summary>
        public async Task<bool> UpdateFaultWarningAsync(EquipmentFaultWarningInput input)
        {
            var warning = await _equipmentFaultWarningRepository.GetFirstOrDefaultAsync(w => w.Id == input.Id);
            if (warning == null) return false;

            warning.EquipmentId = input.EquipmentId;
            warning.WarningTime = input.WarningTime;
            warning.WarningLevel = input.WarningLevel;
            warning.WarningType = input.WarningType;
            warning.WarningContent = input.WarningContent;
            warning.ProcessStatus = input.ProcessingStatus;
            warning.Processor = input.Handler;
            warning.ProcessTime = input.ProcessingTime;
            warning.ProcessResult = input.ProcessingResult;
            warning.Remark = input.Remark;

            _equipmentFaultWarningRepository.Update(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 处理设备故障预警
        /// </summary>
        public async Task<bool> HandleFaultWarningAsync(string id, int processingStatus, string handler, string processingResult)
        {
            var warning = await _equipmentFaultWarningRepository.GetFirstOrDefaultAsync(w => w.Id == id);
            if (warning == null) return false;

            warning.ProcessStatus = processingStatus;
            warning.Processor = handler;
            warning.ProcessTime = DateTime.Now;
            warning.ProcessResult = processingResult;

            _equipmentFaultWarningRepository.Update(warning);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备故障预警
        /// </summary>
        public async Task<bool> DeleteFaultWarningAsync(string id)
        {
            var warning = await _equipmentFaultWarningRepository.GetFirstOrDefaultAsync(w => w.Id == id);
            if (warning == null) return false;

            _equipmentFaultWarningRepository.Remove(warning);
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
                var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == record.EquipmentId);

                var recordOutput = new EquipmentFaultRecordOutput
                {
                    Id = record.Id,
                    FaultNo = record.FaultNo,
                    EquipmentId = record.EquipmentId,
                    EquipmentName = equipment?.Name,
                    FaultOccurTime = record.FaultTime,
                    FaultResolveTime = record.ProcessTime,
                    FaultType = record.FaultType,
                    FaultDescription = record.FaultDescription,
                    FaultReason = record.ProcessResult,
                    HandlingMethod = record.ProcessResult,
                    Handler = record.Processor,
                    FaultStatus = record.Status,
                    FaultStatusText = GetFaultStatusText(record.Status),
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
            var record = await _equipmentFaultRecordRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (record == null) return null;

            var equipment = await _equipmentRepository.GetFirstOrDefaultAsync(e => e.Id == record.EquipmentId);

            var result = new EquipmentFaultRecordOutput
            {
                Id = record.Id,
                FaultNo = record.FaultNo,
                EquipmentId = record.EquipmentId,
                EquipmentName = equipment?.Name,
                FaultOccurTime = record.FaultTime,
                FaultResolveTime = record.ProcessTime,
                FaultType = record.FaultType,
                FaultDescription = record.FaultDescription,
                FaultReason = record.ProcessResult,
                HandlingMethod = record.ProcessResult,
                Handler = record.Processor,
                FaultStatus = record.Status,
                FaultStatusText = GetFaultStatusText(record.Status),
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
                FaultTime = input.FaultOccurTime,
                FaultType = input.FaultType,
                FaultDescription = input.FaultDescription,
                Status = input.FaultStatus,
                Processor = input.Handler,
                ProcessTime = input.FaultResolveTime,
                ProcessResult = input.HandlingMethod,
                Remark = input.Remark,
                Creator = input.Creator,
                CreateDate = DateTime.Now
            };

            _equipmentFaultRecordRepository.Insert(record);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备故障记录
        /// </summary>
        public async Task<bool> UpdateFaultRecordAsync(EquipmentFaultRecordInput input)
        {
            var record = await _equipmentFaultRecordRepository.GetFirstOrDefaultAsync(r => r.Id == input.Id);
            if (record == null) return false;

            record.FaultNo = input.FaultNo;
            record.EquipmentId = input.EquipmentId;
            record.FaultTime = input.FaultOccurTime;
            record.FaultType = input.FaultType;
            record.FaultDescription = input.FaultDescription;
            record.Status = input.FaultStatus;
            record.Processor = input.Handler;
            record.ProcessTime = input.FaultResolveTime;
            record.ProcessResult = input.HandlingMethod;
            record.Remark = input.Remark;

            _equipmentFaultRecordRepository.Update(record);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 解决设备故障
        /// </summary>
        public async Task<bool> ResolveFaultAsync(string id, DateTime processTime, string processor, string processResult)
        {
            var record = await _equipmentFaultRecordRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (record == null) return false;

            record.ProcessTime = processTime;
            record.Processor = processor;
            record.ProcessResult = processResult;
            record.Status = 3; // 已解决

            _equipmentFaultRecordRepository.Update(record);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备故障记录
        /// </summary>
        public async Task<bool> DeleteFaultRecordAsync(string id)
        {
            var record = await _equipmentFaultRecordRepository.GetFirstOrDefaultAsync(r => r.Id == id);
            if (record == null) return false;

            _equipmentFaultRecordRepository.Remove(record);
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
