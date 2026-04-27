using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 设备报废
    /// </summary>
    public class EquipmentScrap
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
        /// 报废单号
        /// </summary>
        public string ScrapNo { get; set; }
        
        /// <summary>
        /// 报废原因
        /// </summary>
        public string ScrapReason { get; set; }
        
        /// <summary>
        /// 报废日期
        /// </summary>
        public DateTime ScrapDate { get; set; }
        
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
    /// 设备退租
    /// </summary>
    public class EquipmentReturn
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 租用ID
        /// </summary>
        public string RentalId { get; set; }
        
        /// <summary>
        /// 退租单号
        /// </summary>
        public string ReturnNo { get; set; }
        
        /// <summary>
        /// 退租日期
        /// </summary>
        public DateTime ReturnDate { get; set; }
        
        /// <summary>
        /// 退租原因
        /// </summary>
        public string ReturnReason { get; set; }
        
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
    /// 设备巡检
    /// </summary>
    public class EquipmentInspection
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
        /// 巡检单号
        /// </summary>
        public string InspectionNo { get; set; }
        
        /// <summary>
        /// 巡检日期
        /// </summary>
        public DateTime InspectionDate { get; set; }
        
        /// <summary>
        /// 巡检结果
        /// </summary>
        public string InspectionResult { get; set; }
        
        /// <summary>
        /// 状态 0:待审批 1:已审批 2:已拒绝 3:已完成
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 巡检人
        /// </summary>
        public string Inspector { get; set; }
        
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
}
