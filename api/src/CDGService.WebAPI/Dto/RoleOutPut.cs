using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class RoleOutPut
    {
        public string Id  { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>

        public string RoleName { get; set; }
        /// <summary>
        /// 角色编码
        /// </summary>

        public string RoleCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>

        public int DataState { get; set; }
    }

    //角色权限
    public class RolePermissionsOutPut
    {
        public bool expand { get; set; }
        
        public bool @checked { get; set; } = false;
        public bool IsButton { get; set; } = false;
        public string title { get; set; }
        public string Id { get; set; }

        public List<RolePermissionsOutPut> children { get; set; } = new List<RolePermissionsOutPut>();

        //  public MenuOutPut Menuchilden { get; set; }

        //  public MenuButtonOutPut[] menuButtonOutPuts { get; set; }

    }




}
