﻿﻿using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Store;
using CDGService.Utils;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Data.Helper;
namespace CDGService.WebAPI.DataCore
{
    public class MenuManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly IMapper _mapper;
        public MenuManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IMapper mapper)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _mapper = mapper;
        }
        private IRepository<Menu> MenuStore => _unitOfWork.GetStore<Menu>();

        private IRepository<MenuButton> MenuButtonStore => _unitOfWork.GetStore<MenuButton>();


        #region  菜单

        /// <summary>
        ///  新增/修改菜单
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateMenuAsync(MenuInput input)
        {
            return Task.Run(async () =>
            {
                bool result = false;

                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.MenuName))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.MenuName"));
                string id = input.Id + "";
                bool isRe = await GetNameRepeat(input.MenuName, id);
                if (isRe)
                    throw new Exception(MessageFormater.PrameterValueIsUsed("input.MenuName", input.MenuName));
                try
                {
                    MenuOutPut ParentMenu = null;
                    if (input.ParentMenuCode + "" != "" && input.ParentMenuCode + "" != "0")
                    {
                        ParentMenu = await GetUpMenu(input.ParentMenuCode);
                    }
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    Menu data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await MenuStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyPropertys(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;

                        if (ParentMenu == null)
                        {
                            data.MenuType = 1;
                            data.ParentMenuCode = "0";
                        }
                        else
                        {
                            data.ParentMenuCode = ParentMenu.Id;
                            data.MenuType = ParentMenu.MenuType + 1;
                        }
                        MenuStore.Update(data);
                    }
                    else
                    {
                        data = _mapper.Map<Menu>(input);
                        data.Id = Guid.NewGuid().tostring32();
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        if (ParentMenu == null)
                        {
                            data.MenuType = 1;
                        }
                        else
                            data.MenuType = ParentMenu.MenuType + 1;
                        MenuStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" && input.Id + "" != "0" ? LogType.DataUpdate : LogType.DataAdded, "菜单",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }


        /// <summary>
        /// 获取菜单列表(树)
        /// </summary>
        public Task<MenuOutPut[]> GetMenuTreeQueryableAsync()
        {
            return Task.Run(async () =>
            {
                MenuOutPut[] result = null;
                MenuOutPut curItem = new MenuOutPut() { title = "根节点", Id = "0", ParentMenuCode = "0" };
                try
                {
                    var datas = await MenuStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = _mapper.Map<MenuOutPut[]>(datas);

                    LoopToAppendChildren(result, curItem);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return curItem.children.ToArray();
            });
        }

        /// <summary>
        /// 获取菜单列表[]
        /// </summary>
        public Task<MenuOutPut[]> GetMenuQueryableAsync()
        {
            return Task.Run(async () =>
            {
                MenuOutPut[] result = null; 
                try
                {
                    var datas = await MenuStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = _mapper.Map<MenuOutPut[]>(datas); 
                    
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                } 
                return result;
            });
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<bool> DelMenuAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    Menu data = null;

                    data = await MenuStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    if (await GetcMenus(data.Id))
                        throw new Exception("该菜单存在子菜单，不可删除！");
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.IsDelete = true;
                    data.DataState = 3;
                    MenuStore.Update(data);

                    await
                        _logManager.WriteLogAsync(LogType.DataDelete, "菜单",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }



        public void LoopToAppendChildren(MenuOutPut[] catelist, MenuOutPut children)
        {
            var subItems = catelist.Where(x => x.ParentMenuCode == children.Id).ToList();
            children.children = new List<MenuOutPut>();
            // subItems.Sort(t=>t.);
            var query = from items in subItems orderby items.MenuSortNo select items;
            children.children.AddRange(query);
            foreach (var item in subItems)
            {
                LoopToAppendChildren(catelist, item);
            }
        }
        /// <summary>
        /// 获取父级菜单
        /// </summary>
        /// <param name="ParentMenuCode"></param>           
        public Task<MenuOutPut> GetUpMenu(string ParentMenuCode)
        {
            return Task.Run(async () =>
            {
                var data = await MenuStore.GetFirstOrDefaultAsync(t => t.Id == ParentMenuCode && t.IsDelete == false);
                return _mapper.Map<MenuOutPut>(data);
            });

        }

        /// <summary>
        /// 判断是否有子菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Task<bool> GetcMenus(string id)
        {
            return Task.Run(async () =>
            {
                var data = await MenuStore.GetFirstOrDefaultAsync(t => t.ParentMenuCode == id && t.IsDelete == false);
                if (data == null)
                {
                    return false;
                }
                return true;
            });
        }
        /// <summary>
        /// 重复
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>

        private Task<bool> GetNameRepeat(string name, string id)
        {
            return Task.Run(async () =>
            {
                bool IsRepeat = false;
                try
                {
                    var data = await MenuStore.Entities.Where(t => t.MenuName == name && t.IsDelete == false).ToArrayAsync();
                    if (data == null)
                        IsRepeat = false;
                    if (data != null && data.Where(t => t.Id != id).Count() > 0)
                        IsRepeat = true;

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return IsRepeat;
            });

        }
        #endregion

        #region 按钮
        //

        /// <summary>
        ///  新增/修改按钮
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateMenuButtonAsync(MenuButtonInput input)
        {
            return Task.Run(async () =>
            {
                bool result = false;

                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.ButtonName))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.ButtonName"));
                if (string.IsNullOrEmpty(input.ButtonCode))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.ButtonCode"));
                var code = MenuButtonStore.GetFirstOrDefaultAsync(t => t.ButtonCode == input.ButtonCode && t.IsDelete == false);

                if (code.Result != null && input.Id + "" == "")
                    throw new Exception("按钮Code重复");

                var MENUcode = MenuStore.GetFirstOrDefaultAsync(t => t.Id == input.MenuId && t.IsDelete == false);
                if (MENUcode.Result == null)
                    throw new Exception("未能获取到按钮所属菜单ID");
                //int id = input.Id.HasValue ? input.Id.Value : 0;
                //bool isRe = await GetNameRepeat(input.MenuName, id);
                //if (isRe)
                //    throw new Exception(MessageFormater.PrameterValueIsUsed("input.MenuName", input.MenuName));
                try
                {
                    //MenuOutPut ParentMenu = null;
                    //if (input.ParentMenuCode.HasValue) 
                    //{
                    //    ParentMenu = await GetUpMenu(input.ParentMenuCode.Value);
                    //}
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    MenuButton data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await MenuButtonStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;

                        MenuButtonStore.Update(data);
                    }
                    else
                    {

                        data = _mapper.Map<MenuButton>(input);
                        data.Id = Guid.NewGuid().tostring32();
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;

                        MenuButtonStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" && input.Id + "" != "0" ? LogType.DataUpdate : LogType.DataAdded, "按钮",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }

        /// <summary>
        /// 获取按钮列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<MenuButtonOutPut[]> GetButtonAsync(MenuButtonShearch input)
        {

            return Task.Run(async () =>
            {
                MenuButtonOutPut[] result = null;

                try
                {
                    Expression<Func<MenuButton, bool>> predicate = t => t.IsDelete == false;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                        predicate = predicate.And(t => t.Id == input.Id);
                    if (input.MenuId + "" != "")
                        predicate = predicate.And(t => t.MenuId == input.MenuId);
                    var datas = await MenuButtonStore.Entities.Where(predicate).ToArrayAsync();
                    var query = from items in datas orderby items.SortNo select items;
                    result = _mapper.Map<MenuButtonOutPut[]>(query);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return result;
            });
        }


        /// <summary>
        ///  删除按钮   
        /// </summary>
        public Task<bool> DelMenuButtonAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    MenuButton data = null;

                    data = await MenuButtonStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));

                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.IsDelete = true;
                    data.DataState = 3;
                    MenuButtonStore.Update(data);

                    await
                        _logManager.WriteLogAsync(LogType.DataDelete, "按钮",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }
        #endregion

    }
}
