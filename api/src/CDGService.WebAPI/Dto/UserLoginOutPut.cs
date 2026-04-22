using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class UserLoginOutPut
    {
        public account account { get; set; }
        public button[] button { get; set; }
        public menu[] menu { get; set; }

    }
    public class account
    {
        public string employeeId { get; set; }
        public string userName { get; set; }
        public string token { get; set; }
        public string Id { get; set; }

        public string EmpName { get; set; }
    }

    public class button
    {
        public string buttonCode { get; set; }
        public string buttonName { get; set; }
    }
    public class menu
    {
        public string icon { get; set; }
        public string name { get; set; }
        public Title meta { get; set; }
        public int hideInMenu { get; set; }
      
        public List<menu> children { get; set; } = new List<menu>();

    }
    public class Title
    {
        public string title { get; set; }
      //  public List<string> Roles { get; set; } = new List<string>();
    }

}
