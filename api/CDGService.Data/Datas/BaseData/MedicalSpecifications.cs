using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas.BaseData
{
    /// <summary>
    /// 档案扩展- 规格
    /// </summary>
    public class MedicalSpecification
    {
        public string Id { get; set; }
        /// <summary>
        /// 档案ID
        /// </summary>
        public string MedicalId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 量
        /// </summary>
        public double SpeNum { get; set; }

        public string SpeUnit { get; set; }
        public string ParentId { get; set; }
        public int SortNum { get; set; }
    }
}
