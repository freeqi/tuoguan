using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CDGService.Data.Datas;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Extenstions;
using CDGService.WebAPI.Dto;
using Microsoft.AspNetCore.Cors;

namespace CDGService.WebAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class PermissionController : Controller
    {
        private readonly IGetUserInfo _userinfo;
        private readonly UserManager _userManager;

        /// <summary>
        /// 权限相关操作
        /// </summary>
        /// <param name="userinfo"></param>
        public PermissionController(IGetUserInfo userinfo, UserManager userManager)
        {
            _userinfo = userinfo;
            _userManager = userManager;
        }


        /// <summary>
        /// 获取登录用户权限。把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("right")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<string[]>> Right()
        {
            return Task.Run(async () =>
            {
                var permissions = await _userinfo.GetCurrentPermissionNamesAsync();
                return new ServiceMessage<string[]>(permissions);
            });
        }


        /// <summary>
        /// 检查登录用户权限。把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("checkPermission")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CheckPermission([FromBody]string permission)
        {
            return Task.Run(async () =>
            {
                var isPassed = await _userinfo.CheckPermisssion(permission);
                return new ServiceMessage<bool>(isPassed);
            });
        }

        /// <summary>
        /// 检查登录用户权限。把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("checkPermissions")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CheckPermissions([FromBody]PermissionsInput permissionInput)
        {
            return Task.Run(async () =>
            {
                var isPassed = await _userinfo.CheckPermissions(permissionInput.Permissions, permissionInput.Isall);
                return new ServiceMessage<bool>(isPassed);
            });
        }


        /// <summary>
        /// 获取角色列表。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("role/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<Role[]>> GetRoleArrayAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _userManager.GetRoleArrayAsync();
                return new ServiceMessage<Role[]>(result);
            });
        }


        //权限相关 SetRolePermissions
        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="inPut">权限参数</param>
        /// <returns></returns>
        [HttpPost("set/RolePermissions")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> SetRolePermissionsAsync([FromBody]RolePermissionsInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _userManager.SetRolePermissionsAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        //获取角色权限getRolePermissionsAsyn
        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="inPut">权限参数</param>
        /// <returns></returns>
        [HttpPost("get/RolePermissions/{RoleId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<RolePermissionsOutPut[]>> getRolePermissionsAsyn([FromRoute]string  RoleId)
        {
            return Task.Run(async () =>
            {
               var result = await _userManager.GettRolePermissionsAsync(RoleId);
                return new ServiceMessage<RolePermissionsOutPut[]>(result);

            });
        }
        /// <summary>
        /// 权限参数
        /// </summary>
        public class PermissionsInput
        {
            /// <summary>
            /// 
            /// </summary>
            public string[] Permissions { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool Isall { get; set; }
        }
    }
}
