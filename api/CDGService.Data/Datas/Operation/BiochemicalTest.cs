using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class BiochemicalTest 
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
        public string  TestType { get; set; }
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
        public string  IsFiled { get; set; }
        /// <summary>
        /// 归档日期
        /// </summary>
        public DateTime? FiledDate { get; set; }
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
        [ForeignKey("CenterId")]
        public virtual CenterDialysis CenterDialysis { get; set; }
        public DateTime? CollectData { get; set; }
        public string TestSpecimens { get; set; }
        [ForeignKey("TestSpecimens")]
        public virtual SystemDictionary SystemDictionary { get; set; }
        public double? Values { get; set; }
    }
}
