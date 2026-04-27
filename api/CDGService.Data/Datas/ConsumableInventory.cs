using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 耗材入库
    /// </summary>
    public class ConsumableInbound
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 入库单号
        /// </summary>
        public string InboundNo { get; set; }
        
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierId { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }
        
        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime InboundDate { get; set; }
        
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
        
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
    
    /// <summary>
    /// 耗材入库详情
    /// </summary>
    public class ConsumableInboundDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 入库ID
        /// </summary>
        public string InboundId { get; set; }
        
        /// <summary>
        /// 医疗物品ID
        /// </summary>
        public string MedicalItemId { get; set; }
        
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
    
    /// <summary>
    /// 耗材出库
    /// </summary>
    public class ConsumableOutbound
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundNo { get; set; }
        
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartmentId { get; set; }
        
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        
        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime OutboundDate { get; set; }
        
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
        
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
    
    /// <summary>
    /// 耗材出库详情
    /// </summary>
    public class ConsumableOutboundDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 出库ID
        /// </summary>
        public string OutboundId { get; set; }
        
        /// <summary>
        /// 医疗物品ID
        /// </summary>
        public string MedicalItemId { get; set; }
        
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        
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
    /// 耗材库存
    /// </summary>
    public class ConsumableInventory
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 医疗物品ID
        /// </summary>
        public string MedicalItemId { get; set; }
        
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
        
        /// <summary>
        /// 库存数量
        /// </summary>
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// 上次入库日期
        /// </summary>
        public DateTime? LastInboundDate { get; set; }
        
        /// <summary>
        /// 上次出库日期
        /// </summary>
        public DateTime? LastOutboundDate { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
    
    /// <summary>
    /// 耗材库存预警
    /// </summary>
    public class ConsumableInventoryWarning
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 医疗物品ID
        /// </summary>
        public string MedicalItemId { get; set; }
        
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// 预警类型 1:库存不足 2:即将过期 3:库存过多
        /// </summary>
        public int WarningType { get; set; }
        
        /// <summary>
        /// 预警阈值
        /// </summary>
        public int WarningThreshold { get; set; }
        
        /// <summary>
        /// 当前库存
        /// </summary>
        public int CurrentStock { get; set; }
        
        /// <summary>
        /// 预警信息
        /// </summary>
        public string WarningMessage { get; set; }
        
        /// <summary>
        /// 处理状态 0:未处理 1:处理中 2:已处理
        /// </summary>
        public int ProcessingStatus { get; set; }
        
        /// <summary>
        /// 处理人
        /// </summary>
        public string Handler { get; set; }
        
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ProcessingTime { get; set; }
        
        /// <summary>
        /// 处理结果
        /// </summary>
        public string ProcessingResult { get; set; }
        
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