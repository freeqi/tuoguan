using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Data.Datas; 

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 用户输出实体
    /// </summary>
    public class UserOutput
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        public string UserId { get; set; }

        public string Name { get; set; }

        

        public string [] Role { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string department { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsActive { get; set; }

        public string Note { get; set; }
        public string CenterDialysisName { get; set; }
        /// <summary>
        /// 是否检测权限
        /// </summary>
        public int IsCheckRights { get; set; } = 1;
	}

    ///// <summary>
    ///// 角色输出实体
    ///// </summary>
    //public class RoleOutput
    //{
    //    public string Id { get; set; }

    //    public string RoleName { get; set; }

    //    public string RoleAliasName { get; set; }

    //    public string Note { get; set; }
    //}
}
