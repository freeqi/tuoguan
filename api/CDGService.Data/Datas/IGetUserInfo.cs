using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGetUserInfo
    {
        string Token { get; }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        Task<string> GetCurrentUserNameAsync();


        Task<string> GetCurrentUserIdAsync();
        /// <summary>
        /// 获取当前用户实体
        /// </summary>
        /// <returns></returns>
        Task<User> GetUserAsync();

        Task<string[]> GetCurrentPermissionNamesAsync();

        /// <summary>
        /// 获取当前用户登录的人员名称（张三）
        /// </summary>
        /// <returns></returns>
        Task<string> GetCurrentUserEmpNameAsync();

        Task<string> GetCurrentUserEmpNameAsync(string userid);

        /// <summary>
        /// 检查当前登录用户是否具有权限
        /// </summary>
        /// <param name="permission">权限名称</param>
        /// <returns></returns>
        Task<bool> CheckPermisssion(string permission);

        /// <summary>
        /// 检查当前登录用户是否具有权限
        /// </summary>
        /// <param name="permission">权限名称</param>
        /// <param name="isall">是否检查所有权限</param>
        /// <returns></returns>
        Task<bool> CheckPermissions(string[] permission, bool isall);

        /// <summary>
        /// 设置当前登录用户
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetUserInfo(string token);

        Task<UserToken> GetUserToken();
    }


}
