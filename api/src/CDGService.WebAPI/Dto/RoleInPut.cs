using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    #region 角色
    public class RoleInPut
    {
        public string Id  { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>

        public string RoleName { get; set; }
        /// <summary>
        /// 角色编码
        /// </summary>

        //  public string RoleCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>

        public int DataState { get; set; }
    }

    public class RoleSearchInput
    {
        public string RoleName { get; set; }
        public  string RoleId { get; set; }
        public string RoleCode { get; set; }
    }
    #endregion

    #region  角色权限

    //角色权限
    public class RolePermissionsInPut
    {
        /// <summary>
        /// 
        /// </summary>
        //public string Id { get; set; }
        /// <summary>
        /// 按钮code
        /// </summary>
        public RolePermissionsOutPut[] MenuButtonCode { get; set; }
        
        /// <summary>
        /// 角色ID
        /// </summary>
        public string  Roleld { get; set; }
        ///// <summary>
        ///// 数据状态
        ///// </summary>
        //public int DataState { get; set; }
    }

    #endregion
}
