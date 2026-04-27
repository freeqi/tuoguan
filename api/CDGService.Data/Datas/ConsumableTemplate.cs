using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 设备耗材模板
    /// </summary>
    public class EquipmentConsumableTemplate
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }
        
        /// <summary>
        /// 设备类型ID
        /// </summary>
        public string EquipmentTypeId { get; set; }
        
        /// <summary>
        /// 设备型号ID
        /// </summary>
        public string EquipmentModelId { get; set; }
        
        /// <summary>
        /// 模板类型 1:标准模板 2:自定义模板
        /// </summary>
        public int TemplateType { get; set; }
        
        /// <summary>
        /// 状态 1:启用 0:停用
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
    }
    
    /// <summary>
    /// 设备耗材模板详情
    /// </summary>
    public class EquipmentConsumableTemplateDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 模板ID
        /// </summary>
        public string TemplateId { get; set; }
        
        /// <summary>
        /// 医疗物品ID
        /// </summary>
        public string MedicalItemId { get; set; }
        
        /// <summary>
        /// 耗材名称
        /// </summary>
        public string ConsumableName { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
    }
    
    /// <summary>
    /// 耗材采购申请
    /// </summary>
    public class ConsumablePurchaseRequest
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 申请单号
        /// </summary>
        public string RequestNo { get; set; }
        
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime RequestDate { get; set; }
        
        /// <summary>
        /// 预计到货日期
        /// </summary>
        public DateTime? EstimatedArrivalDate { get; set; }
        
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
    /// 耗材采购申请详情
    /// </summary>
    public class ConsumablePurchaseRequestDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 申请ID
        /// </summary>
        public string RequestId { get; set; }
        
        /// <summary>
        /// 医疗物品ID
        /// </summary>
        public string MedicalItemId { get; set; }
        
        /// <summary>
        /// 采购数量
        /// </summary>
        public int PurchaseQuantity { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// 预计单价
        /// </summary>
        public decimal? EstimatedUnitPrice { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
