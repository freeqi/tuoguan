using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class PatientCycleScheduling
    {

        public string Id { get; set; }
        public string CenterId { get; set; }
        public string PatientId { get; set; }
        public string SickbedNo { get; set; }
        public DateTime? Date { get; set; }
        public string Weekday { get; set; }
        public string Shift { get; set; }
        public string DialysisType { get; set; }
        public string Dialyzer { get; set; }
        public string DialysisPerfusion { get; set; }
        public string SchedulingUser { get; set; }
        public string SchedulingBedUser { get; set; }
        public int? IsTemporaryCheckIn { get; set; }
        public int? IsImportInfo { get; set; }
        public decimal? CurrentDryWeight { get; set; }
        public int? CurrentState { get; set; }
        public decimal? CurrentBeforeDialysisWeight { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string EquipmentId { get; set; }
        public string ActualShift { get; set; }
        public string ActualDialysisType { get; set; }
        public string ActualDialyzer { get; set; }
        public string ActualDialysisPerfusion { get; set; }
        public DateTime? SignDate { get; set; }
        public string SignDoctor { get; set; }
        public DateTime? CollectData { get; set; }
        public virtual CurrentDialysisProgram currentDialysisProgram { get; set; }
    }
}
