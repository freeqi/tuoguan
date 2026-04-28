using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 设备租用
    /// </summary>
    public class EquipmentRental
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// 租用单号
        /// </summary>
        public string RentalNo { get; set; }
        
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; }
        
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; set; }
        
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DeliveryAddress { get; set; }
        
        /// <summary>
        /// 租用日期
        /// </summary>
        public DateTime RentalDate { get; set; }
        
        /// <summary>
        /// 预计归还日期
        /// </summary>
        public DateTime EstimatedReturnDate { get; set; }
        
        /// <summary>
        /// 实际归还日期
        /// </summary>
        public DateTime? ActualReturnDate { get; set; }
        
        /// <summary>
        /// 状态 0:待审批 1:已审批 2:已拒绝 3:已归还
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
    /// 设备租用详情
    /// </summary>
    public class EquipmentRentalDetail
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
        /// 设备ID
        /// </summary>
        public string EquipmentId { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    
    /// <summary>
    /// 设备租用延长
    /// </summary>
    public class EquipmentRentalExtension
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
        /// 原归还日期
        /// </summary>
        public DateTime OriginalReturnDate { get; set; }
        
        /// <summary>
        /// 新归还日期
        /// </summary>
        public DateTime NewReturnDate { get; set; }
        
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
