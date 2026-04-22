using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Helper;
using CDGService.Data.Store;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CDGService.WebAPI.Controllers
{

    /// <summary>
    /// 用户相关操作
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager _usermanager;
        private readonly IGetUserInfo _userinfo;
        

        /// <summary>
        /// 用户相关操作
        /// </summary>
        /// <param name="usermanager"></param>
        /// <param name="userinfo"></param>
        /// <param name="logManager"></param>
        public UserController(UserManager usermanager, IGetUserInfo userinfo)
        {
            _usermanager = usermanager;
            _userinfo = userinfo;
            
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="uinput"></param>
        /// <returns></returns>
        [HttpPost("login")]
        //[CheckLogin]
        [ServiceMessageTryCatch]
        public virtual async Task<ServiceMessage<UserLoginOutPut>> Login([FromBody]UserInput uinput)
        {
           
          
            var result = await _usermanager.UserLoginAsync(uinput?.UserName, uinput?.Pwd, HttpContext.GetUserIp());
            return new ServiceMessage<UserLoginOutPut>(result);
        }

        /// <summary>
        /// 测试用户是否登录成功。把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("Test")]       
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<string>> Test()
        {
            return Task.FromResult(new ServiceMessage<string>("OK"));
        }



        /// <summary>
        /// 登录的参数
        /// </summary>
        public class UserInput
        {
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string Pwd { get; set; }
        }

        /// <summary>
        /// 修改用户手机userId，chanelId
        /// </summary>
        public class UserPhoneInput
        {
            /// <summary>
            /// 用户id
            /// </summary>
            public long PersonId { get; set; }
            /// <summary>
            /// 手机id
            /// </summary>
            public string UserId { get; set; }
            /// <summary>
            /// 手机ChannelId
            /// </summary>
            public string ChannelId { get; set; }
        }
        #region 人员相关接口 

        #endregion

        #region 账号相关接口


        /// <summary>
        /// 修改账号密码。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("user/mpwd")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ModifyPwdUserAsync([FromBody]UserModifyPsInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _usermanager.ModifyPwdUserAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }


        /// <summary>
        /// 删除账号。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/del/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        [LogRecord("账号", LogType.DataDelete)]
        public virtual Task<ServiceMessage<bool>> DeleteUserAsync([FromRoute] string  id)
        {
            return Task.Run(async () =>
            {
                var result = await _usermanager.DeleteUserAsync(id);
                return new ServiceMessage<bool>(result);
            });
        }

        //分配账号 SetUserAsync
        /// <summary>
        /// 分配新增员工账号
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Setuser")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> SetUserAsync([FromBody] UserInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _usermanager.SetUserAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        //


        /// <summary>
        /// 修改员工账号
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Updateuser")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpadateUserAsync([FromBody] UpdateUserInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _usermanager.UpadateUserAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }
        //

        /// <summary>
        /// 批量启用、禁用账号
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("userActive")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> EnableUserAsync([FromBody] UserActiveInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _usermanager.EnableUserAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 获取员工账号列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("userlist")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        [LogRecord("账号", LogType.DataAccess)]
        public virtual Task<ServiceMessage<UserOutput[]>> GetUserListAsync([FromBody] UserSearchInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _usermanager.GetUserListAsync(input);
                return new ServiceMessage<UserOutput[]>(result.Result, result.Count);
            });
        }
        #endregion

    }
}