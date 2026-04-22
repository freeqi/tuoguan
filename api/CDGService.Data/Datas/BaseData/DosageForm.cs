using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 剂型
    /// </summary>
    public class DosageForm : ISoftDelete
    {
        public string Id { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public string ParentId { get; set; }
        public int SortNo { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }
        public int? DataState { get; set; }
        public bool IsDelete { get; set; }
    }
}
