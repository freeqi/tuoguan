using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 设备维保记录
    /// </summary>
   public class EquipmentKeepRecord 
    {

        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }    
        /// <summary>
        /// 设备ID
        /// </summary>
        public string  EquipmentInfoId { get; set; } 
        /// <summary>
        /// 维保时间
        /// </summary>
        public DateTime? MaintenanceDate { get; set; }                                                 
        /// <summary>
        /// 维保内容
        /// </summary>
        public string MaintenanceContent { get; set; }  
        /// <summary>
        /// 维保人
        /// </summary>
        public string MaintenancePerson { get; set; }   
        /// <summary>
        /// 备注
        /// </summary>
        public string  Remark { get; set; }     
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
        /// <summary>
        /// 数据状态
        /// </summary>
        public int? DataState { get; set; }            
       // public bool IsDelete { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
