using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class VascularMaintainRecord
    {
        public string Id { get; set; }
        public DateTime? RecRegistrationDate { get; set; }
        public string Phenomena { get; set; }
        public string Intervention { get; set; }
        public string DoctorSignature { get; set; }
        public string Remark { get; set; }
        public string RecTypeId { get; set; }
        public string PatientId { get; set; }
        public string IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }

}
