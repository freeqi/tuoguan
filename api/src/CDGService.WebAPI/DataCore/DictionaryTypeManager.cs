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

    /// <summary>
    /// 数据字典类型
    /// </summary>
    public class DictionaryTypeManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly IMapper _mapper;
        public DictionaryTypeManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IMapper mapper)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _mapper = mapper;
        }
        private IRepository<DictionaryType> DictionaryTypeStore => _unitOfWork.GetStore<DictionaryType>();

        /// <summary>
        ///  新增/修改字典类型
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateDictionaryTypeAsync(DictionaryTypeInput input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                string id = input.Id + "";
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("字典类型信息"));
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception(MessageFormater.PrameterNeedProvider("字典类型名称"));
                bool isRe = await GetNameRepeat(input.Name, id);
                if (isRe)
                    throw new Exception(MessageFormater.PrameterValueIsUsed("字典类型名称", input.Name));
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    DictionaryType data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await DictionaryTypeStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        // data.TypeCode = ValueHelper.ToASCII(input.Name);
                        DictionaryTypeStore.Update(data);
                    }
                    else
                    {

                        data = _mapper.Map<DictionaryType>(input);
                        data.Id =Guid.NewGuid().tostring32();
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.TypeCode =Guid.NewGuid().tostring32();
                        data.DataState = 1;
                        DictionaryTypeStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" && input.Id + "" != "0" ? LogType.DataUpdate : LogType.DataAdded, "数据字典类型",
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
        /// 删除字典类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> DelDictionaryTypeAsync(string  id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await DictionaryTypeStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;
                    data.DataState = 3;
                    DictionaryTypeStore.Update(data);
                    _unitOfWork.SaveChanges();
                    await
                       _logManager.WriteLogAsync(LogType.DataDelete, "数据字典类型",
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
        /// 获取字典类型列表
        /// </summary>
        public Task<DictionaryTypeOutPut[]> GetDictionaryTypesQueryableAsync(DictionaryTypeSearchInput input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                DictionaryTypeOutPut[] result = null;
                try
                {
                    Expression<Func<DictionaryType, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrEmpty(input.Name))
                        predicate = predicate.And(t => t.Name.Contains(input.Name));
                    if (input.Id + "" != "")
                        predicate = predicate.And(t => t.Id == input.Id);
                    var datas = await DictionaryTypeStore.Entities.Where(predicate).ToArrayAsync();
                    result = _mapper.Map<DictionaryTypeOutPut[]>(datas);

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        /// <summary>
        /// 重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        private Task<bool> GetNameRepeat(string name, string  id = "0")
        {
            return Task.Run(async () =>
            {
                bool IsRepeat = false;
                try
                {
                    var data = await DictionaryTypeStore.Entities.Where(t => t.Name == name).ToArrayAsync();
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
