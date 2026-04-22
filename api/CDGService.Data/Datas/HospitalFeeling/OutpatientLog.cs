using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 门诊日志
    /// </summary>
    public class OutpatientLog
    {

        public string Id { get; set; }
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }
        /// <summary>
        /// 系统编码O开头
        /// </summary>f
        public string OutpatientCode { get; set; }
        /// <summary>
        /// 日志标头
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 日志日期
        /// </summary>
        public DateTime OutpatientDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }

        public virtual List<OutpatientDetailsLog> ListOutpatientDetailsLog { get; set; } = new List<OutpatientDetailsLog>();

    }
    public class OutpatientLogModel
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime? MorbidityDate { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Age { get; set; }
        public string Professional { get; set; }
        public string NowAddress { get; set; }
        public string MainSymptomsAndSigns { get; set; }
        public string Diagnose { get; set; }
        public string IsDiagnoseAgain { get; set; }
        public DateTime? ReportDate { get; set; }
        public string ReportPerson { get; set; }
        public string PreSystolicPressure { get; set; }
        public string PreDiastolicPressure { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterDialysisId { get; set; }
    }
}
