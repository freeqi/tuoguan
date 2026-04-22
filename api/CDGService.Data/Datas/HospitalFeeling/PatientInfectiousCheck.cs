using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 新入患者传染病监测完成情况
    /// </summary>
    public class PatientInfectiousCheck
    {
        public string Id { get; set; }
        public string CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual CenterDialysis CenterDialysis { get; set; }
        public string PatentId { get; set; }

        [ForeignKey("PatentId")]
        public virtual Patient Patient { get; set; }
        /// <summary>
        /// 入院日期
        /// </summary>
        public DateTime GoInTime { get; set; }
        /// <summary>
        /// 内毒素检验结果
        /// </summary>
        public string Endotoxin { get; set; }
        /// <summary>
        /// 检查日期
        /// </summary>
        public DateTime? DisinfectionTime { get; set; }
        /// <summary>
        /// 传染病类型
        /// </summary>
        public string InfectiousType { get; set; }

        [ForeignKey("InfectiousType")]
        public virtual SystemDictionary DicInfectiousType { get; set; }

        /// <summary>
        /// 检查是否完成
        /// </summary>
        public bool IsQualified { get; set; }
        /// <summary>
        /// 治疗编号
        /// </summary>
        public string CureCode { get; set; }
        /// <summary>
        /// 发病时间
        /// </summary>
        public DateTime? MorbidityTime { get; set; }
        public string Remark { get; set; }
        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
    }
}
