using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{
    public class BusinessTarget
    {
         
        public int MainKey { get; set; }
        public string MainValue { get; set; }
        public int  IndexKey { get; set; }
        public string  IndexValue { get; set; }
        public int ChildKey { get; set; }
        public string  ChildValue { get; set; }
    }
}
