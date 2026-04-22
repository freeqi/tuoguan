using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{

    public class CenterUsers
    {
        //   public List<centeruser> centerusers { get; set; } = new List<centeruser>();

    }

    public class CenterUser
    {
        public string centerKey { get; set; }
        public string userName { get; set; }
        public string userPwd { get; set; }
        public string Account { get; set; }
        public string ClientType { get; set; }
        public string Token { get; set; }
    }

}
