using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class MenuController : Controller
    {

        private readonly MenuManager _menuManager;
        public MenuController(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }
        #region  菜单
        /// <summary>
        ///新增/修改菜单
        /// Id为0或null.视为新增
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">字典类型信息参数</param>
        /// <returns></returns>
        [HttpPost("CreateUpdateMenu")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateMenuAsync([FromBody]MenuInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.CreateUpdateMenuAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 菜单树
        /// </summary>
        /// <returns></returns>
        [HttpPost("tree")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MenuOutPut[]>> GetMenuTreeQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.GetMenuTreeQueryableAsync();
                return new ServiceMessage<MenuOutPut[]>(result);
            });
        }
        /// <summary>
        /// 菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("arry")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MenuOutPut[]>> GetMenuArryQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.GetMenuQueryableAsync();
                return new ServiceMessage<MenuOutPut[]>(result);
            });
        }

        //

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("del/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DelMenuAsync([FromRoute]string  id)
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.DelMenuAsync(id);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 获取父级菜单
        /// </summary>
        /// <param name="ParentMenuCode">父级ID</param>
        /// <returns></returns>
        [HttpPost("ParentMenu/{ParentMenuCode}")]
        [CheckLogin]
        [ServiceMessageTryCatch]

        public virtual Task<ServiceMessage<MenuOutPut>> GetUpMenuAsync([FromRoute]string  ParentMenuCode)
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.GetUpMenu(ParentMenuCode);
                return new ServiceMessage<MenuOutPut>(result);
            });
        }
        #endregion


        #region 按钮
        //
        /// <summary>
        ///新增/修改按钮
        /// Id为0或null.视为新增
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">字典类型信息参数</param>
        /// <returns></returns>
        [HttpPost("CreateUpdateMenuButton")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateMenuButtonAsync([FromBody]MenuButtonInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.CreateUpdateMenuButtonAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("ButtonArry")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MenuButtonOutPut[]>> GetButtonAsync([FromBody]MenuButtonShearch input)
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.GetButtonAsync(input);
                return new ServiceMessage<MenuButtonOutPut[]>(result);
            });
        }


        //DelMenuButtonAsync

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost("delbutton/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DelMenuButtonAsync([FromRoute]string  id)
        {
            return Task.Run(async () =>
            {
                var result = await _menuManager.DelMenuButtonAsync(id);
                return new ServiceMessage<bool>(result);
            });
        }
        #endregion
    }
}
