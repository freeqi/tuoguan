using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class CurePattern
    {
        public string CenterId { get; set; }
        public string ShortName { get; set; }
        public string DialysisType { get; set; }
        public string Dialyzer { get; set; }
        public int sumNum { get; set; }
         
    }


}
