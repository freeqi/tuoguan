using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{

    /// <summary>
    /// 设备维修记录
    /// </summary>
    public class MaintenanceRecord
    {
        /// <summary>
        ///  编号      
        /// </summary>
        public string Id { get; set; }                    //      varchar(32) 主键/GUID
        /// <summary>
        /// 设备id    
        /// </summary>
        public string  EquipmentInfoId { get; set; }                 //      varchar(32) 设备Id
        /// <summary>
        ///   维修时间  
        /// </summary>
        public DateTime? MaintenanceDate { get; set; }                 //                        datetime
        /// <summary>
        ///  原因描述  
        /// </summary>
        public string MaintenanceReason { get; set; }                 //                     nvarchar(100)                                                             //
        /// <summary>
        /// 原始照片  
        /// </summary>
        public string OriginalPicture { get; set; }                 //                        varchar(100)    保存图片url
        /// <summary>
        /// 申请人    
        /// </summary>
        public string Applicant { get; set; }                 //                  nvarchar(32)
        /// <summary>
        /// 解决方案  
        /// </summary>
        public string Solution { get; set; }                 //                 nvarchar(100)
        /// <summary>
        /// 维修费用  
        /// </summary>
        public decimal? MaintenanceCost { get; set; }                 //                        decimal (10,4)	
        /// <summary>
        /// 设备状态  
        /// </summary>

        public string EquipmentState { get; set; }                 //            nvarchar(32)    1正常、2.维修、3.报废
        /// <summary>
        ///  维修人    
        /// </summary>
        public string MaintenancePerson { get; set; }                 //                   nvarchar(32)                                                             //
        /// <summary>
        ///备注      
        /// </summary>
        public string Remark { get; set; }                 //                  nvarchar(100)
        /// <summary>
        /// 创建人    
        /// </summary>
        public string Founder { get; set; }                 //           nvarchar(20)
        /// <summary>
        /// 创建时间  
        /// </summary>
        public DateTime? FounderDate { get; set; }                 //                        datetime
        /// <summary>
        /// 修改人    
        /// </summary>
        public string Modifier { get; set; }                 //                   nvarchar(20)
        /// <summary>
        /// 修改时间  
        /// </summary>
        public DateTime? ModifierDate { get; set; }                 //                datetime
        /// <summary>
        /// 数据状态  
        /// </summary>
        public int? DataState { get; set; }                  //                   int	1启用、2停用、3删除
        //public bool IsDelete { get ; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }
}
