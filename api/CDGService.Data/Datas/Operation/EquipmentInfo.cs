using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class EquipmentInfo : ISoftDelete
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }                          //       varchar(32)
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }        //  archar(32) 存数据字典Id值
        [ForeignKey(nameof(Name))]
        public virtual SystemDictionary SName { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquipType { get; set; }        //     varchar(32) 存数据字典ID值
        [ForeignKey(nameof(EquipType))]
        public virtual SystemDictionary SEquipType { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }        //  varchar(32)

        public string CenterId { get; set; }

        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis Dialysis { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string SerialNumber { get; set; }        //设备编码        varchar(32)
        /// <summary>
        /// 分区
        /// </summary>
        public string TreatmentRegion { get; set; }        //治疗分区       varchar(32) 存数据字典ID值

        /// <summary>
        /// 分区
        /// </summary>
        [ForeignKey(nameof(TreatmentRegion))]
        public virtual SystemDictionary DicTreatmentRegion { get; set; }        //治疗分区       varchar(32) 存数据字典ID值
        public string EquipmentState { get; set; }
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

        [ForeignKey(nameof(BloodBorneDisease))]
        public virtual SystemDictionary SBloodBorneDisease { get; set; }

        /// <summary>
        /// 治疗模式
        /// </summary>
        public string TreatmentModels { get; set; }      //治疗模式                      varchar(500)    存数据字典ID值，可以选多个治疗模式（数据格式：xxx|xxx|xxx）
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
        public DateTime? PurchaseDate { get; set; }      //购买日期               datetime
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
        public Decimal? PurchaseMoney { get; set; }      //采购价                           money
        /// <summary>
        /// 生产商
        /// </summary>
        public string Producer { get; set; }   //生产商                                 varchar(32) 存数据库字典ID值

        /// <summary>
        /// 生产商电话
        /// </summary>
        public string ProducerTelphone { get; set; }    //生产商电话                          varchar(50)
        /// <summary>
        /// 生产日期     
        /// </summary>
        public DateTime? ProduceDate { get; set; }    //生产日期                            datetime
        /// <summary>
        /// 保质期
        /// </summary>
        public DateTime? MaintenanceDate { get; set; } //质保期                       datetime
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }    //数据状态                        tinyint	1启用/2停用/3删除
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; } //创建人                        nvarchar(32)
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }    //创建时间                       datetime
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; } //修改人                   nvarchar(32)
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }       //修改时间 datetime
        public bool IsDelete { get; set; }
    }


    public class EquipmentType
    {
        public string Id { get; set; }
        public string TypeName { get; set; }
        public int BedSign { get; set; }
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }
        public int IsDefault { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }


    public class EquipmentSupplier
    {

        public string Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone { get; set; }
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }

    public class EquipmentModel
    {
        public string Id { get; set; }
        public string ManufacturersId { get; set; }
        public string TypeId { get; set; }
        public string ModelName { get; set; }
        public string Number { get; set; }
        public string TreatmentType { get; set; }
        public string DefaultName { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public int? IsDefault { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }

    public class EquipmentManufacturers
    {
        public string Id { get; set; }
        public string ManufacturersName { get; set; }
        public string ManufacturersPhone { get; set; }
        public string Remarks { get; set; }
        public int? IsSupplier { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }


    }


    public class EquipmentBedConfigur
    {
        public string Id { get; set; }
        public string BedNumber { get; set; }
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }


    }

}
