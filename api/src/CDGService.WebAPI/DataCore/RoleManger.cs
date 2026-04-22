﻿using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Helper;
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

namespace CDGService.WebAPI.DataCore
{
    public class RoleManger

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
       
        public RoleManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
     

        }
        private IRepository<Role> RoleStore => _unitOfWork.GetStore<Role>();

        /// <summary>
        ///  新增/修改角色
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateRoleTypeAsync(RoleInPut input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                string  id = input.Id + "" != "" ? input.Id : "0";
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.RoleName))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.Name"));
                bool isRe = await GetNameRepeat(input.RoleName, id);
                if (isRe)
                    throw new Exception(MessageFormater.PrameterValueIsUsed("input.Name", input.RoleName));
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    Role data = null;
                    if (input.Id + "" != "")
                    {
                        data = await RoleStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.RoleCode=data.RoleCode+""==""? IdHelper.tostring16(Guid.NewGuid()): data.RoleCode;
                        RoleStore.Update(data);
                    }
                    else
                    {

                        data = Mapper.Map<Role>(input);
                        data.Id =Guid.NewGuid().tostring32(); 
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.RoleCode = Guid.NewGuid().tostring16();
                        RoleStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, "角色",
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
        /// 获取角色列表
        /// </summary>
        public Task<RoleOutPut[]> GetRoleQueryableAsync(RoleSearchInput input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                RoleOutPut[] result = null;
                try
                {
                    Expression<Func<Role, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrEmpty(input.RoleName))
                        predicate = predicate.And(t => t.RoleName.Contains(input.RoleName));
                    if (!string.IsNullOrEmpty(input.RoleCode))
                        predicate = predicate.And(t => t.RoleCode.Contains(input.RoleCode));
                    if (input.RoleId + "" != "")
                        predicate = predicate.And(t => t.Id == input.RoleId); 
                    var datas = await RoleStore.Entities.Where(predicate).ToArrayAsync();
                    result = Mapper.Map<RoleOutPut[]>(datas);

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }


        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> DelRoleAsync(string  id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await RoleStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;
                    data.DataState = 3;
                    RoleStore.Update(data);
                    _unitOfWork.SaveChanges();
                    await
                       _logManager.WriteLogAsync(LogType.DataDelete, "角色",
                           $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
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

        private Task<bool> GetNameRepeat(string name, string  id = "0")
        {
            return Task.Run(async () =>
            {
                bool IsRepeat = false;
                try
                {
                    var data = await RoleStore.Entities.Where(t => t.RoleName == name && t.IsDelete ==false).ToArrayAsync();
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

    }
}
