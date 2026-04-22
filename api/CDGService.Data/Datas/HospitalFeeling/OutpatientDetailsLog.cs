using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 门诊日志详情
    /// </summary>
    public class OutpatientDetailsLog
    {

        public string Id { get; set; }
        ///// <summary>
        ///// 日志表ID
        ///// </summary>
        //public string OutpatientId { get; set; }
        //[ForeignKey("OutpatientId")]
        //public OutpatientLog outpatientLog { get; set; }
        public string CenterId { get; set; }
        /// <summary>
        /// 就诊日期
        /// </summary>
        public DateTime? DiagnosisDate { get; set; }
        /// <summary>
        ///  发病时间
        /// </summary>
        public DateTime? MorbidityDate { get; set; }
        /// <summary>
        /// 患者ID
        /// </summary>
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        /// <summary>
        /// 主要症状
        /// </summary>
        public string Symptoms { get; set; }
        /// <summary>
        /// 诊断
        /// </summary>
        public string Diagnosis { get; set; }
        /// <summary>
        /// 复诊
        /// </summary>
        public string SubsequentVisit { get; set; }
        /// <summary>
        /// 报告日
        /// </summary>
        public DateTime? ReportDate { get; set; }
        /// <summary>
        /// 报告人
        /// </summary>
        public string ReportUser { get; set; }
        /// <summary>
        /// 血压
        /// </summary>
        public string BloodPressure { get; set; }
        public string Remark { get; set; }
        public int? DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public DateTime? CollectData { get; set; }

    }
}
