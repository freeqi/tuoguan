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
using Microsoft.Extensions.Options;
using CDGService.Data;

namespace CDGService.WebAPI.DataCore
{
    public class SystemDictionaryManger
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly DictionaryCode _dictionaryCode;
        private readonly IMapper _mapper;
        public SystemDictionaryManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<DictionaryCode> dictionaryCode, IMapper mapper)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _dictionaryCode = dictionaryCode.Value;
            _mapper = mapper;
        }
        private IRepository<SystemDictionary> SystemDictionaryStore => _unitOfWork.GetStore<SystemDictionary>();
        private IRepository<DictionaryType> DictionaryStore => _unitOfWork.GetStore<DictionaryType>();

        /// <summary>
        ///  新增/修改字典 
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateDictionaryAsync(SystemDictionaryInput input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                bool isRe = false;
                string id = "";
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.Name"));
                if (input.TypeId + "" == "")
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.TypeId"));
                if (input.Id + "" != "")
                    id = input.Id;
                isRe = await GetNameRepeat(input.TypeId, input.Name, id);

                if (isRe)
                    throw new Exception(MessageFormater.PrameterValueIsUsed("input.Name", input.Name));
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    SystemDictionary data = null;
                    var typeData = await DictionaryStore.GetFirstOrDefaultAsync(t => t.Id == input.TypeId);
                    if (typeData == null)
                    {
                        throw new Exception(MessageFormater.PrameterNeedProvider("input.TypeId"));
                    }
                    if (input.Id + "" != "")
                    {
                        data = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyPropertys(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.TypeCode = typeData.TypeCode;                        
                        SystemDictionaryStore.Update(data); 
                    }
                    else
                    {

                        data = _mapper.Map<SystemDictionary>(input);
                        data.Id = Guid.NewGuid().tostring32();
                        data.TypeCode = typeData.TypeCode;
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.DCode = Guid.NewGuid().tostring32();
                        data.DataState = 1;
                        SystemDictionaryStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, "数据字典类型",
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
        /// 获取字典类型列表
        /// </summary>
        public Task<SystemDictionaryOutPut[]> GetDictionaryQueryableAsync(DictionarySearchInput input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                SystemDictionaryOutPut[] result = null;
                try
                {
                    Expression<Func<SystemDictionary, bool>> predicate = t => t.IsDelete == IsDel && t.IsCentDic == IsDel;
                    if (!string.IsNullOrEmpty(input.Name))
                        predicate = predicate.And(t => t.Name.Contains(input.Name));
                    if (input.Id + "" != "")
                        predicate = predicate.And(t => t.Id == input.Id);
                    if (input.TypeId + "" != "")
                        predicate = predicate.And(t => t.TypeId == input.TypeId);
                    //
                    if (!string.IsNullOrEmpty(input.typeCode))
                        predicate = predicate.And(t => t.TypeCode == (input.typeCode));
                    var datas = await SystemDictionaryStore.Entities.Include(t => t.DictionaryType).Where(predicate).OrderBy(t => t.ShowSortNo).ToListAsync();
                    if (input.TypeId == _dictionaryCode.HospitalStateTypeId)//患者在院状态+全部
                    {
                        datas.Insert(0, new SystemDictionary() { TypeId = _dictionaryCode.HospitalStateTypeId,   Name = "全部", Id = "0" });
                    }
                    result = _mapper.Map<SystemDictionaryOutPut[]>(datas);

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        /// <summary>
        /// 删除字典 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> DelDictionaryAsync(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await SystemDictionaryStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;
                    data.DataState = 3;
                    SystemDictionaryStore.Update(data);
                    _unitOfWork.SaveChanges();
                    await
                       _logManager.WriteLogAsync(LogType.DataDelete, "数据字典",
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
        /// <param name="TypeId"></param>
        /// <param name="name"></param>
        /// <returns></returns>

        private Task<bool> GetNameRepeat(string TypeId, string name, string Id = "0")
        {
            return Task.Run(async () =>
            {
                bool IsRepeat = false;
                try
                {
                    var data = await SystemDictionaryStore.Entities.Where(t => t.Name == name && t.TypeId == TypeId).ToArrayAsync();
                    if (data == null)
                        IsRepeat = false;
                    else
                    {
                        if (data.Where(t => t.Id != Id).Count() > 0)
                            IsRepeat = true;
                    }

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
