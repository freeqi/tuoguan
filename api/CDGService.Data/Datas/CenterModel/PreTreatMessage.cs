using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 上机登录信息 -- ☆高血压质控率
    /// </summary>
    public class PreTreatMessage
    {
        public string Id { get; set; }
        public string PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public string PreSystolicPressure { get; set; }
        public string PreDiastolicPressure { get; set; }
        public string PreTemperature { get; set; }
        public string PrePulse { get; set; }
        public string PreHeartRate { get; set; }
        public string Conscious { get; set; }
        public string PunctureWay { get; set; }
        public string VascularAccess { get; set; }
        public string Infection { get; set; }
        public string Appetite { get; set; }
        public string SleepCondition { get; set; }
        public string BleedingCondition { get; set; }
        public string BleedingPart { get; set; }
        public string IsDropsy { get; set; }
        public string IsAnhelation { get; set; }
        public string Discomfort { get; set; }
        public string EntranceWay { get; set; }

        [ForeignKey("EntranceWay")]
        public virtual SystemDictionary DictionaryEntr { get; set; }
        public string Others { get; set; }
        public string LoginNurseId { get; set; }
        public string PunctureNurseId { get; set; }
        public string LoginCondition { get; set; }
        public DateTime? LoginTime { get; set; }
        public string IsFiled { get; set; }
        public DateTime? FiledDate { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CheckNurseId { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CheckState { get; set; }
        public string ProcessSteps { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }


    }
}
