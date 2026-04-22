using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using Microsoft.EntityFrameworkCore;

namespace CDGService.WebAPI.DataCore
{
    public class GetUserInfo : IGetUserInfo
    {

        private readonly IUnitOfWork _unitOfWork;

        public GetUserInfo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private IRepository<User> UserStore => _unitOfWork.GetStore<User>();
        private IRepository<RoleUser> RoleUserStore => _unitOfWork.GetStore<RoleUser>();
        private IRepository<RolePermission> RolePermissionStore => _unitOfWork.GetStore<RolePermission>();
        private IRepository<Role> RoleStore => _unitOfWork.GetStore<Role>();

        private IRepository<UserToken> UserTokenStore => _unitOfWork.GetStore<UserToken>();


        public string Token { get; private set; }

        private string username;
        private string userid;

        #region 接口实现


        /// <summary>
        /// 获取当前登录用户的名称
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCurrentUserNameAsync()
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                await GetUserInfofromDb();
            }

            return username;
        }



        /// <summary>
        /// 获取当前登录用户的Id
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCurrentUserIdAsync()
        {
            if (userid + "" == "")
            {
                await GetUserInfofromDb();
            }
            return userid ?? "";

        }

        private string[] permissions;

        /// <summary>
        /// 获取当前登录用户的权限
        /// </summary>
        public async Task<string[]> GetCurrentPermissionNamesAsync()
        {
            if (permissions == null)
            {
                await GetPermissionfromdb();
            }
            return permissions;
        }

        private async Task GetUserInfofromDb()
        {
            var userToken = await UserTokenStore.GetFirstOrDefaultAsync(t => t.Token == Token, o => o.User);
            if (userToken == null) throw new Exception("用户未登录授权");

            this.userid = userToken.User.Id;
            this.username = userToken.User.Name;

        }

        private async Task GetPermissionfromdb()
        {
            var uid = (await GetCurrentUserIdAsync());
            var roleUser = RoleUserStore.GetFirstOrDefault(t => t.UserId == uid);
            if (roleUser == null)
            {
                throw new Exception("未能获取到该账户角色");
            }
            // Expression<Func<RolePermission, bool>> prevalid = (t) => t.IsGranted && t.UserId == uid;

            //if (roleUser != null)
            Expression<Func<RolePermission, bool>> prevalid = (t) => t.Roleld == roleUser.RoleId;

            this.permissions = RolePermissionStore.GetQueryable(prevalid).Select(t => t.MenuButtonCode).ToArray();
        }


        /// <summary>
        /// 检查当前登录用户权限
        /// </summary>
        /// <param name="permission">权限名称</param>
        /// <returns></returns>
        public Task<bool> CheckPermisssion(string permission)
        {
            if (string.IsNullOrEmpty(permission))
                return Task.FromResult(true);

            if (this.permissions.Contains(permission))

                return Task.FromResult(true);

            return Task.FromResult(false);
        }
        /// <summary>
        /// 检查当前登录用户权限
        /// </summary>
        /// <param name="permission">权限名称数组</param>
        /// <param name="isall">是否检查所有权限</param>
        /// <returns></returns>
        public Task<bool> CheckPermissions(string[] permission, bool isall)
        {
            if (permission == null || permission.Length == 0)
                return Task.FromResult(true);

            if (isall)
                return Task.FromResult(this.permissions.All(permission.Contains));
            return Task.FromResult(this.permissions.Any(permission.Contains));
        }

        /// <summary>
        /// 设置当前登录用户信息
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        async Task IGetUserInfo.SetUserInfo(string token)
        {
            this.Token = token;
        }

        public async Task<string> GetCurrentUserEmpNameAsync()
        {
            // throw new NotImplementedException();
            string Name = "";
            if (userid + "" != "")
            {
                var userToken = await UserTokenStore.Entities.Include(t => t.User).ThenInclude(t => t.Employee).Where(t => t.Token == Token).FirstOrDefaultAsync();
                if (userToken != null && userToken.User != null && userToken.User.Employee != null)
                {
                    Name = userToken.User.Employee.Name;

                }
            }
            return Name;
        }

        public async Task<User> GetUserAsync()
        {
            //throw new NotImplementedException();

            var userToken = await UserTokenStore.GetFirstOrDefaultAsync(t => t.Token == Token, o => o.User);
            if (userToken == null) throw new Exception("用户未登录授权");

            this.userid = userToken.User.Id;
            var user = await UserStore.Entities.Include(t => t.Employee).ThenInclude(t => t.CenterDialysis).Include(t => t.RoleUsers).Where(t => t.Id == userid).FirstOrDefaultAsync();
            //  List<string> RoleId = user.RoleUsers.Select(t => t.RoleId).ToList();
            //  user.Roles= await RoleStore.Entities.Where(t => RoleId.Contains(t.Id)).ToListAsync();


            return user;
            //this.username = userToken.User.Name;
        }

        public async Task<UserToken> GetUserToken()
        {
            var userToken = await UserTokenStore.GetFirstOrDefaultAsync(t => t.Token == Token);
            if (userToken == null) throw new Exception("用户未登录授权");
            return userToken;

        }


        public async Task<string> GetCurrentUserEmpNameAsync(string userid)
        {
            // throw new NotImplementedException();
            string Name = "";
            if (userid + "" != "")
            {
                var userToken = await UserStore.Entities.Include(t => t.Employee).Where(t => t.Id == userid || t.EmployeeId == userid).FirstOrDefaultAsync();
                if (userToken != null && userToken.Employee != null)
                {
                    Name = userToken.Employee.Name;

                }
            }
            return Name;
        }

        #endregion
    }


}
