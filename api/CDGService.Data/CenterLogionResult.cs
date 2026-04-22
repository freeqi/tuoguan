using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{
    public class CenterLogionResult
    {
        public string Token { get; set; }
        public JSON JSON { get; set; }
    }

    public class JSON {
        public account account { get; set; }
       

    }

    public class account
    {
        public string employeeId { get; set; }
    }

}
