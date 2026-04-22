using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace CDGService.Data.Datas
{
    public class User : ISoftDelete
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Pwd { get; set; }


        public string Name { get; set; }

        public string  EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        public string Note { get; set; }

        public bool IsDelete { get; set; }
        public virtual List<RoleUser> RoleUsers { get; set; } = new List<RoleUser>();
       // public virtual List<Role> Roles { get; set; } = new List<Role>();
    }
}
