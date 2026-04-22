using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class HistoryDialysisRecords
    {
        public string Id { get; set; }
        public string CenterId { get; set; }

        [ForeignKey("CenterId")]
        /// <summary>
        /// 透析中心
        /// </summary>
        public virtual CenterDialysis  centerDialysis { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        /// <summary>
        /// 患者
        /// </summary>
        public virtual Patient  patient { get; set; }
        public string PatientCycleSchedulingId { get; set; }

        [ForeignKey("PatientCycleSchedulingId")]
        /// <summary>
        /// 签到表
        /// </summary>
        public virtual PatientCycleScheduling patientCycleScheduling { get; set; }
        public string PreTreatId { get; set; }

        [ForeignKey("PreTreatId")]
        /// <summary>
        /// 上机记录
        /// </summary>
        public virtual PreTreatMessage preTreatMessage { get; set; }

        public string PostPreTreatId { get; set; }

        [ForeignKey("PostPreTreatId")]
        ///// <summary>
        ///// 下机记录
        ///// </summary>
        public virtual PostPreTreatMessage postPreTreatMessage { get; set; }
        public string OutpatientId { get; set; }

        [ForeignKey("OutpatientId")]
        ///// <summary>
        ///// 门诊病历
        ///// </summary>
        public virtual OutpatientMedicalRecord outpatientMedicalRecord { get; set; }
        public string Remark { get; set; }
        public string DoctorSignature { get; set; }
        public int? DialysisNumber { get; set; }
        public string IsFiled { get; set; }
        public DateTime? FiledDate { get; set; }
        public int? IsPrint { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string TxComplication { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public DateTime? CollectData { get; set; }

        /// <summary>
        /// 上机观察记录
        /// </summary>
        public virtual List<ObserveRecord> ObserveRecords { get; set; }
        /// <summary>
        /// 处方单
        /// </summary>
        public virtual List<Prescription> Prescriptions { get; set; }
    }
}
