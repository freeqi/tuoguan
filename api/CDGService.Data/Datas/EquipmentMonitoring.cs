using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 设备运行监控
    /// </summary>
    public class EquipmentMonitoring
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// 监控时间
        /// </summary>
        public DateTime MonitoringTime { get; set; }
        
        /// <summary>
        /// 设备状态 1:正常 2:警告 3:故障
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 运行参数
        /// </summary>
        public string OperatingParameters { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    
    /// <summary>
    /// 设备故障预警
    /// </summary>
    public class EquipmentFaultWarning
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// 预警时间
        /// </summary>
        public DateTime WarningTime { get; set; }
        
        /// <summary>
        /// 预警类型
        /// </summary>
        public string WarningType { get; set; }
        
        /// <summary>
        /// 预警级别 1:低 2:中 3:高
        /// </summary>
        public int WarningLevel { get; set; }
        
        /// <summary>
        /// 预警内容
        /// </summary>
        public string WarningContent { get; set; }
        
        /// <summary>
        /// 处理状态 0:未处理 1:已处理
        /// </summary>
        public int ProcessStatus { get; set; }
        
        /// <summary>
        /// 处理人
        /// </summary>
        public string Processor { get; set; }
        
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ProcessTime { get; set; }
        
        /// <summary>
        /// 处理结果
        /// </summary>
        public string ProcessResult { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    
    /// <summary>
    /// 设备故障记录
    /// </summary>
    public class EquipmentFaultRecord
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// 故障单号
        /// </summary>
        public string FaultNo { get; set; }
        
        /// <summary>
        /// 故障发生时间
        /// </summary>
        public DateTime FaultTime { get; set; }
        
        /// <summary>
        /// 故障类型
        /// </summary>
        public string FaultType { get; set; }
        
        /// <summary>
        /// 故障描述
        /// </summary>
        public string FaultDescription { get; set; }
        
        /// <summary>
        /// 故障状态 0:待处理 1:处理中 2:已处理 3:已解决
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 处理人
        /// </summary>
        public string Processor { get; set; }
        
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ProcessTime { get; set; }
        
        /// <summary>
        /// 处理结果
        /// </summary>
        public string ProcessResult { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
