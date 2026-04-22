﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Helper;
using CDGService.Data.Store;
using CDGService.Utils;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Http;

namespace CDGService.WebAPI.DataCore
{

    /// <summary>
    /// 
    /// </summary>
    public class UserManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetUserInfo _getUserInfo;
        private readonly LogManager _logManager;
        private readonly MenuManager _menuManager;

        public UserManager(IUnitOfWork unitOfWork, IGetUserInfo getUserInfo, LogManager logManager, MenuManager menuManager)
        {
            _unitOfWork = unitOfWork;
            _getUserInfo = getUserInfo;
            _logManager = logManager;
            _menuManager = menuManager;


        }

        private IRepository<User> UserStore => _unitOfWork.GetStore<User>();
        private IRepository<UserToken> UserTokenStore => _unitOfWork.GetStore<UserToken>();

        private IRepository<RoleUser> RoleUserStore => _unitOfWork.GetStore<RoleUser>();
        private IRepository<Role> RoleStore => _unitOfWork.GetStore<Role>();

        private IRepository<RolePermission> RolePermissionStore => _unitOfWork.GetStore<RolePermission>();
        private IRepository<Menu> MenuStore => _unitOfWork.GetStore<Menu>();

        private IRepository<MenuButton> MenuButtonStore => _unitOfWork.GetStore<MenuButton>();

        /// <summary>
        /// 用户登录 
        /// </summary>
        /// <returns></returns> 
        public Task<UserLoginOutPut> UserLoginAsync(string username, string pwd, string Ip)
        {
            return Task.Run(async () =>
            {

                //
                pwd = pwd.Decrypt();
                username = username.Decrypt();
                //  throw new Exception("用户名错误或该员工已删除" );
                UserLoginOutPut outPut = new UserLoginOutPut();
                var user = await UserStore.Entities.Include(t => t.Employee).Where(t => t.UserName == (username) && t.IsDelete == false).FirstOrDefaultAsync();
                if (user == null) throw new Exception("用户名错误或该员工已删除" + username);
                if (user.Employee == null || user.Employee.IsDelete) throw new Exception("用户名错误或该员工已删除" + username);
                if (user.Pwd != pwd.MD5()) throw new Exception("密码错误");
                if (!user.IsActive)
                    throw new Exception("您的账号已经被锁定，无法登录系统，请联系管理员" + username);
                var usertoken = await UserLoginGetTokenAsync(user);
                //登录成功，获取用户角色，权限
                if (username == "admin888")
                {

                    //权限

                    var menuButton = await MenuButtonStore.Entities.Where(t => t.IsDelete == false && t.DataState == 1).ToListAsync();

                    menu menuOutPut = GetUserMenu(menuButton.Select(t => t.MenuId).ToList());

                    List<button> Listbuttons = new List<button>();
                    foreach (var item in menuButton)
                    {
                        Listbuttons.Add(new button() { buttonCode = item.ButtonCode, buttonName = item.ButtonName });
                    }
                    outPut.button = Listbuttons.ToArray();
                    outPut.menu = menuOutPut.children.ToArray();
                    outPut.account = new account() { Id = user.Id, token = usertoken.Token, employeeId = user.EmployeeId + "", userName = user.UserName, EmpName = user.Employee?.Name ?? string.Empty };

                    await _logManager.WriteLoginLogAsync(user.Id, usertoken == null ? "尝试登录系统失败" : "登录系统成功,登录IP:" + Ip);
                    return outPut;
                }

                else
                {


                    var userRole = await RoleUserStore.Entities.Include(t => t.Role).Where(t => t.UserId == user.Id && t.Role.DataState == 1 && t.Role.IsDelete == false).Select(t => t.RoleId).ToListAsync();

                    if (userRole.Count() <= 0)
                    {
                        throw new Exception("您的账号未能获取到可用角色，无法登录系统，请联系管理员" + username);
                    }
                    //权限
                    var userper = await RolePermissionStore.Entities.Where(r => userRole.Contains(r.Roleld)).Select(t => t.MenuButtonCode).ToListAsync();
                    var menuButton = await MenuButtonStore.Entities.Where(t => userper.Distinct().Contains(t.ButtonCode) && t.IsDelete == false).ToListAsync();

                    menu menuOutPut = GetUserMenu(menuButton.Select(t => t.MenuId).ToList());

                    List<button> Listbuttons = new List<button>();
                    foreach (var item in menuButton)
                    {
                        Listbuttons.Add(new button() { buttonCode = item.ButtonCode, buttonName = item.ButtonName });
                    }
                    outPut.button = Listbuttons.ToArray();
                    outPut.menu = menuOutPut.children.ToArray();
                    outPut.account = new account() { Id = user.Id, token = usertoken.Token, employeeId = user.EmployeeId + "", userName = user.UserName, EmpName = user.Employee?.Name ?? string.Empty };

                    await _logManager.WriteLoginLogAsync(user.Id, usertoken == null ? "尝试登录系统失败" : "登录系统成功,登录IP:" + Ip);
                    return outPut;
                }
            });

        }

        //根据按钮获取菜单树

        private menu GetUserMenu(List<string> menuButton)
        {
            try
            {
                var dat = MenuStore.Entities.Where(t => menuButton.Contains(t.Id) && t.IsDelete == false).ToList();
                string[] vs = dat.Select(t => t.ParentMenuCode).Distinct().ToArray();
                //父级
                var mandata = MenuStore.Entities.Where(t => vs.Contains(t.Id) && t.IsDelete == false);
                dat.AddRange(mandata);
                string[] vss = mandata.Select(s => s.ParentMenuCode).Distinct().ToArray();
                var mandatas = MenuStore.Entities.Where(t => vss.Contains(t.Id) && t.IsDelete == false);
                dat.AddRange(mandatas);
                MenuOutPut curItem = new MenuOutPut() { title = "根节点", Id = "0", ParentMenuCode = "0" };


                MenuOutPut[] result = Mapper.Map<MenuOutPut[]>(dat.Distinct());
                menu listmenus = new menu();

                LoopToAppendChildren(result, curItem, listmenus);
                return listmenus;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public void LoopToAppendChildren(MenuOutPut[] catelist, MenuOutPut children, menu menus)
        {
            var subItems = catelist.Where(x => x.ParentMenuCode == children.Id).ToList();
            children.children = new List<MenuOutPut>();
            // subItems.Sort(t=>t.);
            var query = from items in subItems orderby items.MenuSortNo select items;
            // subItems.OrderBy
            children.children.AddRange(query);

            foreach (var item in query)
            {
                menu menu = new menu() { icon = item.IconUrl, hideInMenu = item.HideInMenu, name = item.MenuUrl, meta = new Title() { title = item.title } };
                menus.children.Add(menu);
                LoopToAppendChildren(catelist, item, menu);
            }
        }
        private async Task<UserToken> UserLoginGetTokenAsync(User user)
        {
            var usertoken = new UserToken()
            {
                Id = Guid.NewGuid().tostring32(),
                User = user,
                UserId = user.Id,
                LogingTime = DateTime.Now,
                LastUpdateTime = DateTime.Now,
                Token = Guid.NewGuid().tostring16() + "" + Guid.NewGuid().tostring32(),
            };

            UserTokenStore.Insert(usertoken);

            await _unitOfWork.SaveChangesAsync();
            return usertoken;
        }

        /// <summary>
        /// 清理过期的token
        /// </summary>
        public async Task ClearExpireTokens()
        {
            var tokens = await UserTokenStore.GetAllAsync();

            DateTime dt = DateTime.Now;
            foreach (var ut in tokens)
            {
                if (ut.LogingTime == null || ut.LastUpdateTime == null) break;
                if (dt - ut.LastUpdateTime.Value > TimeSpan.FromDays(1))
                {
                    UserTokenStore.Remove(ut);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        #region 账号相关

        /// <summary>
        /// 判断当前账号是否可用
        /// </summary>
        public Task<bool> IsUserNameValid(UserNewInput input)
        {
            return Task.Run(async () =>
            {
                var user = await UserStore.GetFirstOrDefaultAsync(t => t.UserName == input.UserName && t.IsDelete == false);
                if (user == null)
                    return true;

                if (input.Id + "" != "" && user.Id == input.Id)
                    return true;
                return false;
            });
        }



        /// <summary>
        /// 修改用户密码信息
        /// </summary>
        public Task<bool> ModifyPwdUserAsync(UserModifyPsInput input)
        {
            //判断当前用户名是否可用
            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (input.userName == null || input.userName == "")
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.userName"));
                if (input.OlPwd == null || input.OlPwd == "")
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.OlPwd"));
                if (input.NewPwd == null || input.NewPwd == "")
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.NewPwd"));
                try
                {
                    string userName = input.userName.Decrypt();
                    var user = await UserStore.GetFirstOrDefaultAsync(t => t.UserName == userName);

                    //判断用户原密码是否与数据库一致
                    if (user.Pwd != input.OlPwd.Decrypt().MD5())
                        throw new Exception("请输入正确的旧密码");
                    user.Pwd = input.NewPwd.Decrypt().MD5();
                    UserStore.Update(user);
                    _unitOfWork.SaveChanges();
                    result = true;
                    await _logManager.WritePsModifyLogAsync(user.Id);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;

            });
        }

        /// <summary>
        /// 删除账号-软删除
        /// </summary>
        /// <returns></returns>
        public Task<bool> DeleteUserAsync(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await UserStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;

                    UserStore.Update(data);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }

        /// <summary>
        /// 分配/新增员工账号
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<bool> SetUserAsync(UserInPut input)
        {

            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.Name"));
                if (string.IsNullOrEmpty(input.UserName))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.UserName"));

                if (input.RoleId == null || input.RoleId.Length <= 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.RoleId"));
                var cf = await UserStore.Entities.Include(t => t.Employee).Where(t => t.IsDelete == false && (t.Employee.Id == input.EmployeeId || t.UserName == input.UserName)).ToArrayAsync();
                if (cf != null && cf.Length > 0)
                {
                    throw new Exception("用户名重复或该人员已创建账号");
                }
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    User data = null;


                    if (string.IsNullOrEmpty(input.Pwd))
                        throw new Exception(MessageFormater.PrameterNeedProvider("input.Pwd"));

                    data = Mapper.Map<User>(input);
                    data.Id = Guid.NewGuid().tostring32();
                    data.Pwd = input.Pwd.MD5();
                    data.IsDelete = false;
                    UserStore.Insert(data);

                    _unitOfWork.SaveChanges();
                    RoleUserStore.Remove(t => t.UserId == data.Id);
                    List<RoleUser> ListRoleUser = new List<RoleUser>();
                    foreach (var item in input.RoleId)
                    {
                        ListRoleUser.Add(new RoleUser() { Id = Guid.NewGuid().tostring32(), RoleId = item, UserId = data.Id });

                    }
                    RoleUserStore.Insert(ListRoleUser.ToArray());
                    _unitOfWork.SaveChanges();

                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, "账户信息",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });

        }



        /// <summary>
        /// 修改员工账号
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<bool> UpadateUserAsync(UpdateUserInPut input)
        {

            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (input.Id + "" == "" && input.Id + "" == "0")
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.Id"));
                if (input.RoleId == null || input.RoleId.Length <= 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.RoleId"));

                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    User data = null;

                    data = await UserStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    if (input.Note != "")
                        data.Note = input.Note;

                    //  EntityHelper.CoptyProperty(input, data);

                    if (input.IsActive.HasValue)
                        data.IsActive = input.IsActive.Value;
                    data.IsDelete = false;
                    UserStore.Update(data);


                    _unitOfWork.SaveChanges();
                    RoleUserStore.Remove(t => t.UserId == data.Id);
                    List<RoleUser> ListRoleUser = new List<RoleUser>();
                    foreach (var item in input.RoleId)
                    {
                        ListRoleUser.Add(new RoleUser() { Id = Guid.NewGuid().tostring32(), RoleId = item, UserId = data.Id });

                    }
                    RoleUserStore.Insert(ListRoleUser.ToArray());
                    _unitOfWork.SaveChanges();

                    result = true;
                    await
                        _logManager.WriteLogAsync(LogType.DataUpdate, "账户信息",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });

        }

        public Task<PageData<UserOutput[]>> GetUserListAsync(UserSearchInput input)
        {
            return Task.Run(async () =>
            {
                UserOutput[] result = null;
                bool IsDel = false;
                try
                {
                    Expression<Func<User, bool>> predicate = t => t.IsDelete == IsDel && t.UserName != "admin888";
                    if (!string.IsNullOrEmpty(input.SearchValue))
                        predicate = predicate.And(t => t.UserName.Contains(input.SearchValue) || t.Employee.Name.Contains(input.SearchValue) || t.Employee.CenterDialysis.DialysisName.Contains(input.SearchValue));

                    if (input.IsActive.HasValue)
                        predicate = predicate.And(t => t.IsActive == (input.IsActive.Value == 1 ? true : false));

                    var datas = UserStore.Entities.Include(t => t.Employee).Include(t => t.Employee.SJobTitle).Include(t => t.Employee.Sposition).Include(t => t.Employee.Sdepartment).Include(t => t.Employee.SEducation)
                    .Include(t => t.Employee.CenterDialysis).Where(predicate).OrderByDescending(t => t.IsActive);
                    //.Include(t => t.Sposition).Include(t => t.Sdepartment).Include(t => t.SEducation)
                    int count = datas.Count();

                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        var Dialysis = await PaginatedList<User>.CreateAsync(datas, input.PageNum, input.PageSize);
                        //  result = Mapper.Map<CenterDialysisOutPut[]>(Dialysis);

                        result = Mapper.Map<UserOutput[]>(Dialysis);
                    }
                    else
                    {
                        result = Mapper.Map<UserOutput[]>(datas);
                    }
                    for (int i = 0; i < result.Length; i++)
                    {
                        var data = await RoleUserStore.Entities.Include(t => t.Role).Where(t => t.UserId == result[i].Id).ToArrayAsync();
                        result[i].Role = data.Select(t => t.Role.Id).ToArray();
                        result[i].UserId = datas.Where(t => t.Id == result[i].Id).FirstOrDefault().EmployeeId;
                    }
                    return new PageData<UserOutput[]>(result, count);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }


            });

        }


        //批量启用、禁用
        public Task<bool> EnableUserAsync(UserActiveInPut input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("参数"));
                if (input.Id == null || input.Id.Length < 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.Id"));
                if (!input.IsActive.HasValue)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.IsActive"));
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();


                    var data = await UserStore.Entities.Where(t => input.Id.Contains(t.Id)).ToListAsync();
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));



                    for (int i = 0; i < data.Count(); i++)
                    {
                        data[i].IsActive = input.IsActive.Value;
                    }
                    UserStore.Update(data);


                    _unitOfWork.SaveChanges();

                    result = true;
                    await
                        _logManager.WriteLogAsync(LogType.DataUpdate, "批量更新用户",
                            "");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }

        #endregion

        #region 角色相关
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public Task<Role[]> GetRoleArrayAsync()
        {
            return Task.Run(async () =>
            {
                var roles = await RoleStore.GetAllAsync();
                return roles;
            });
        }

        #endregion

        #region  权限相关
        private List<string> ListButton = new List<string>();
        //设置角色权限 RolePermissionsInPut
        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<bool> SetRolePermissionsAsync(RolePermissionsInPut input)
        {
            return Task.Run(async () =>
            {
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("权限信息"));
                if (input.Roleld + "" == "")
                    throw new Exception(MessageFormater.PrameterNeedProvider("角色"));


                if (input.MenuButtonCode != null && input.MenuButtonCode.Length > 0)
                {
                    RecursiveButton(input.MenuButtonCode);

                }
                List<RolePermission> rolePermissions = new List<RolePermission>();
                RolePermissionStore.Remove(t => t.Roleld == input.Roleld);
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                if (ListButton.Count > 0)
                {
                    foreach (var item in ListButton)
                    {
                        rolePermissions.Add(new RolePermission() { Id = Guid.NewGuid().tostring32(), Roleld = input.Roleld, MenuButtonCode = item, DataState = 1, Founder = userId, FounderDate = DateTime.Now, Modifier = userId, ModifierDate = DateTime.Now });
                    }


                }
                RolePermissionStore.Insert(rolePermissions);

                //if (input == null)
                //    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                //if (input.Roleld <= 0)
                //    throw new Exception(MessageFormater.PrameterNeedProvider("input.Roleld"));
                //var userId = await _getUserInfo.GetCurrentUserIdAsync();
                ////var data = RolePermissionStore.Entities.Where(t=>t.Roleld == input.Roleld)
                //RolePermissionStore.Remove(t => t.Roleld == input.Roleld);
                //List<RolePermission> ListrolePermissions = new List<RolePermission>();
                //foreach (var item in input.MenuButtonCode)
                //{
                //    ListrolePermissions.Add(new RolePermission() { MenuButtonCode = item, DataState = 1, Roleld = input.Roleld, Founder = userId, FounderDate = DateTime.Now, Modifier = userId, ModifierDate = DateTime.Now });

                //}
                //if (ListrolePermissions.Count > 0)
                //{
                //    RolePermissionStore.Insert(ListrolePermissions.ToArray());
                //}
                _unitOfWork.SaveChanges();
                await
                      _logManager.WriteLogAsync(LogType.DataUpdate, "设置角色权限",
                          $"角色编号为{input.Roleld}");
                return true;
            });
        }

        //获取角色权限 RolePermissionsOutPut
        private RolePermissionsOutPut rolePermissionsOutPuts = new RolePermissionsOutPut() { title = "根节点", Id = "0" };
        private List<RolePermission> _rolePermission = new List<RolePermission>();
        public Task<RolePermissionsOutPut[]> GettRolePermissionsAsync(string RoleId)
        {


            return Task.Run(async () =>
            {
                MenuOutPut[] result = null;
                MenuOutPut curItem = new MenuOutPut() { title = "根节点", Id = "0", ParentMenuCode = "0" };
                try
                {
                    var datas = await MenuStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = Mapper.Map<MenuOutPut[]>(datas);
                    _rolePermission = await RolePermissionStore.Entities.Where(t => t.Roleld == RoleId).ToListAsync();
                    LoopToAppendChildren(result, curItem, rolePermissionsOutPuts);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                // return curItem.children.ToArray();
                return rolePermissionsOutPuts.children.ToArray();
            });

            //return new RolePermissionsOutPut[]();


        }

        private void LoopToAppendChildren(MenuOutPut[] catelist, MenuOutPut children, RolePermissionsOutPut rolePermissions)
        {
            var subItems = catelist.Where(x => x.ParentMenuCode == children.Id).ToList();
            children.children = new List<MenuOutPut>();


            // subItems.Sort(t=>t.);
            var query = from items in subItems orderby items.MenuSortNo select items;
            //获取按钮

            children.children.AddRange(query);
            foreach (var item in query)
            {
                var data = _menuManager.GetButtonAsync(new MenuButtonShearch() { MenuId = item.Id });
                RolePermissionsOutPut outPut = new RolePermissionsOutPut() { title = item.title, Id = item.Id + "", IsButton = false };
                if (data != null)
                {
                    foreach (var items in data.Result)
                    {
                        if (_rolePermission.Where(t => t.MenuButtonCode == items.ButtonCode).FirstOrDefault() != null)

                            outPut.children.Add(new RolePermissionsOutPut() { Id = items.ButtonCode, title = items.ButtonName, IsButton = true, @checked = true });
                        else
                            outPut.children.Add(new RolePermissionsOutPut() { Id = items.ButtonCode, title = items.ButtonName, IsButton = true, @checked = false });
                    }

                }
                // item.
                rolePermissions.children.Add(outPut);
                LoopToAppendChildren(catelist, item, outPut);
            }
        }


        private void RecursiveButton(RolePermissionsOutPut[] MenuButtonCode)
        {
            foreach (var item in MenuButtonCode)
            {
                if (item.IsButton)
                {
                    ListButton.Add(item.Id);
                }
                if (item.children.Count > 0)
                {
                    RecursiveButton(item.children.ToArray());
                }
            }

        }
        #endregion
    }
}
