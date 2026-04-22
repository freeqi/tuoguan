using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class EquipmentOutPut
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }                          //       varchar(32)
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }        //  archar(32) 存数据字典Id值
        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipType { get; set; }        //     varchar(32) 存数据字典ID值
        public string Center { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }        //  varchar(32)

        /// <summary>
        /// 数据有效性 0 正常数据 1编码有异常 2 型号有异常
        /// </summary>
        public int validity { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string SerialNumber { get; set; }        //设备编码        varchar(32)
        /// <summary>
        /// 分区
        /// </summary>
        public string TreatmentRegion { get; set; }        //治疗分区       varchar(32) 存数据字典ID值
        /// <summary>
        /// IP
        /// </summary>
        public string IPAddress { get; set; }        //IP地址               varchar(20)
        /// <summary>
        /// 床位号
        /// </summary>
        public string BedNo { get; set; }        //床位号                 nvarchar(10)
        /// <summary>
        /// 血源性疾病
        /// </summary>
        public string BloodBorneDisease { get; set; }      //血源性疾病              varchar(32) 存数据字典ID值
        /// <summary>
        /// 治疗模式
        /// </summary>
        public string[] TreatmentModelsK { get; set; }      //治疗模式                      varchar(500)    存数据字典ID值，可以选多个治疗模式（数据格式：xxx|xxx|xxx）
        /// <summary>
        /// 工程师
        /// </summary>
        public string EngineerName { get; set; }        //设备工程师            nvarchar(32)    存工程师名字或ID
        /// <summary>
        /// 工程师电话
        /// </summary>
        public string EngineerPhone { get; set; }      //工程师电话                    varchar(50)
        /// <summary>
        /// 购买日期
        /// </summary>
        public string PurchaseDate { get; set; }      //购买日期               datetime
        /// <summary>
        /// 供货商
        /// </summary>
        public string Supplier { get; set; }     //供货商                                varchar(32) 存数据库字典ID值
        /// <summary>
        /// 供货商电话
        /// </summary>
        public string SupplierTelphone { get; set; }    //供货商电话                   varchar(50)
        /// <summary>
        /// 采购价
        /// </summary>
        public decimal? PurchaseMoney { get; set; }      //采购价                           money
        /// <summary>
        /// 生产商
        /// </summary>
        public string Producer { get; set; }   //生产商                                 varchar(32) 存数据库字典ID值
        /// <summary>
        /// 生产商电话
        /// </summary>
        public string ProducerTelphone { get; set; }    //生产商电话                          varchar(50)
        public string ProduceDate { get; set; }    //生产日期                            datetime
        /// <summary>
        /// 保质期
        /// </summary>
        public string MaintenanceDate { get; set; } //质保期                       datetime
        /// <summary>
        /// 数据状态
        /// </summary>
        public EquiState DataState { get; set; }    //数据状态                        tinyint	1启用/2停用/3删除
        public string SDataState { get; set; }
    }

    public class MaintenanceRecordOutPut
    {

        /// <summary>
        ///  编号      
        /// </summary>
        public string Id { get; set; }                    //      varchar(32) 主键/GUID
        /// <summary>
        /// 设备id    
        /// </summary>
        public string EquipmentInfoId { get; set; }                 //      varchar(32) 设备Id
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentInfoName { get; set; }
        /// <summary>
        ///   维修时间  
        /// </summary>
        public string MaintenanceDate { get; set; }                 //                        datetime
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
        /// 数据状态  
        /// </summary>
        public int? DataState { get; set; }                  //                   int	1启用、2停用、3删除

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
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }

    public class EquipmentKeepRecordOutPut
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentInfoId { get; set; }
        /// <summary>
        /// 维保时间
        /// </summary>
        public string MaintenanceDate { get; set; }
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
        public string Remark { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int? DataState { get; set; }

    }

    /// <summary>
    /// 生化检测-PH
    /// </summary>
    public class BiochemicalTestOutPut
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentInfoId { get; set; }
        /// <summary>
        /// 检测人
        /// </summary>
        public string TestPersonnel { get; set; }
        /// <summary>
        /// 检测日期
        /// </summary>
        public string TestDate { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string TestType { get; set; }
        /// <summary>
        /// PH
        /// </summary>
        public double? PH { get; set; }
        /// <summary>
        /// 电导
        /// </summary>
        public double? Conductivity { get; set; }
        /// <summary>
        /// 渗透压
        /// </summary>
        public double? OsmoticPressure { get; set; }
        /// <summary>
        /// 钠离子
        /// </summary>
        public double? IonNa { get; set; }
        /// <summary>
        /// 钾离子
        /// </summary>
        public double? IonK { get; set; }
        /// <summary>
        /// 钙离子
        /// </summary>
        public double? IonGa { get; set; }
        /// <summary>
        /// 镁离子
        /// </summary>
        public double? IonMg { get; set; }
        /// <summary>
        /// 氯离子
        /// </summary>
        public double? IonCl { get; set; }
        /// <summary>
        /// 醋酸根
        /// </summary>
        public double? IonAcetate { get; set; }
        /// <summary>
        /// 葡萄糖
        /// </summary>
        public double? Glucose { get; set; }
        /// <summary>
        /// 碳酸氢根
        /// </summary>
        public double? IonHCO3 { get; set; }
        /// <summary>
        /// 二氧化碳
        /// </summary>
        public double? IonPCO2 { get; set; }
        /// <summary>
        /// PH值图片
        /// </summary>
        public string PHImage { get; set; }
        /// <summary>
        /// 细菌培养
        /// </summary>
        public string BacterialCulture { get; set; }
        /// <summary>
        /// 内毒素
        /// </summary>
        public string Endotoxin { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public string IsFiled { get; set; }
        /// <summary>
        /// 归档日期
        /// </summary>
        public string FiledDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        //  public bool IsDelete { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
        public string TestSpecimens { get; set; }
        public double? Values { get; set; }

    }

    public class BacterialCultureOutPut
    { /// <summary>
      /// 编号
      /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentInfoId { get; set; }
        /// <summary>
        /// 检测人
        /// </summary>
        public string TestPersonnel { get; set; }
        /// <summary>
        /// 检测日期
        /// </summary>
        public DateTime? TestDate { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string TestType { get; set; }
        /// <summary>
        /// 细菌培养
        /// </summary>
        public string BacterialCulture { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public string IsFiled { get; set; }
        /// <summary>
        /// 归档日期
        /// </summary>
        public DateTime? FiledDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int DataState { get; set; }

    }

    public class EndotoxinOutPut
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EquipmentInfoId { get; set; }
        /// <summary>
        /// 检测人
        /// </summary>
        public string TestPersonnel { get; set; }
        /// <summary>
        /// 检测日期
        /// </summary>
        public DateTime? TestDate { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string TestType { get; set; }

        /// <summary>
        /// 内毒素
        /// </summary>
        public string Endotoxin { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public string IsFiled { get; set; }
        /// <summary>
        /// 归档日期
        /// </summary>
        public DateTime? FiledDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int? DataState { get; set; }

    }
}
