using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 用户新增/修改输入实体
    /// </summary>
    public class UserNewInput
    {
        public string Id  { get; set; }

        public string UserName { get; set; }

        public string Pwd { get; set; }

        public long PersonId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        public string Note { get; set; }

        public int RoleId { get; set; }

    }

    /// <summary>
    /// 用户修改密码输入实体
    /// </summary>
    public class UserModifyPsInput
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 原始密码
        /// </summary>
        public string OlPwd { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPwd { get; set; }

    }


    /// <summary>
    /// 用户查询输入实体
    /// </summary>
    public class UserSearchInput
    {
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        /// <summary>
        /// 搜索- 可按照员工姓名、账号、机构
        /// </summary>
        public string SearchValue { get; set; }
        /// <summary>
        /// 人员ID
        /// </summary>
        public int? PersonId { get; set; }

        /// <summary>
        /// 是否启用 1 启用 0禁用
        /// </summary>
        public int? IsActive { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class UserInPut
    {

        public string Id  { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string  EmployeeId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string [] RoleId { get; set; }
        /// <summary>
        ///  是否启用 0禁用 1 启用
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
       // public bool IsDelete { get; set; }
    }
    public class UpdateUserInPut
    {

        public string Id  { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string [] RoleId { get; set; }
        /// <summary>
        ///  是否启用 0禁用 1 启用
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
       // public bool IsDelete { get; set; }
    }

    public class UserActiveInPut
    {

        public string [] Id { get; set; }

        /// <summary>
        ///  是否启用 0禁用 1 启用
        /// </summary>
        public bool? IsActive { get; set; }

    }
}
