using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 设备维保
    /// </summary>
    public class EquipmentMaintenance
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
        /// 维保单号
        /// </summary>
        public string MaintenanceNo { get; set; }
        
        /// <summary>
        /// 维保类型
        /// </summary>
        public string MaintenanceType { get; set; }
        
        /// <summary>
        /// 维保日期
        /// </summary>
        public DateTime MaintenanceDate { get; set; }
        
        /// <summary>
        /// 预计完成日期
        /// </summary>
        public DateTime EstimatedCompletionDate { get; set; }
        
        /// <summary>
        /// 实际完成日期
        /// </summary>
        public DateTime? ActualCompletionDate { get; set; }
        
        /// <summary>
        /// 维保内容
        /// </summary>
        public string MaintenanceContent { get; set; }
        
        /// <summary>
        /// 维保结果
        /// </summary>
        public string MaintenanceResult { get; set; }
        
        /// <summary>
        /// 状态 0:待审批 1:已审批 2:已拒绝 3:已完成
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }
        
        /// <summary>
        /// 审批人
        /// </summary>
        public string Approver { get; set; }
        
        /// <summary>
        /// 审批日期
        /// </summary>
        public DateTime? ApprovalDate { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
    
    /// <summary>
    /// 设备维保详情
    /// </summary>
    public class EquipmentMaintenanceDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 维保ID
        /// </summary>
        public string MaintenanceId { get; set; }
        
        /// <summary>
        /// 维保项目
        /// </summary>
        public string MaintenanceItem { get; set; }
        
        /// <summary>
        /// 维保结果
        /// </summary>
        public string MaintenanceResult { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    
    /// <summary>
    /// 设备维保延长
    /// </summary>
    public class EquipmentMaintenanceExtension
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 维保ID
        /// </summary>
        public string MaintenanceId { get; set; }
        
        /// <summary>
        /// 原完成日期
        /// </summary>
        public DateTime OriginalCompletionDate { get; set; }
        
        /// <summary>
        /// 新完成日期
        /// </summary>
        public DateTime NewCompletionDate { get; set; }
        
        /// <summary>
        /// 延长原因
        /// </summary>
        public string Reason { get; set; }
        
        /// <summary>
        /// 状态 0:待审批 1:已审批 2:已拒绝
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }
        
        /// <summary>
        /// 审批人
        /// </summary>
        public string Approver { get; set; }
        
        /// <summary>
        /// 审批日期
        /// </summary>
        public DateTime? ApprovalDate { get; set; }
        
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
