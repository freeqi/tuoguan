﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Store;
using CDGService.Utils;
using CDGService.Data.Helper;
using CDGService.WebAPI.Dto.AutoCenterPut;
using AutoMapper;
using CDGService.WebAPI.Extenstions;
using CDGService.WebAPI.Datas;
using System.Linq.Expressions;
using CDGService.Application.DataCollect;
using Microsoft.EntityFrameworkCore;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public class LogManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetUserInfo _getUserInfo;
        private readonly CollentStartup _purchasManagerService;
        private readonly IMapper _mapper;
        private readonly IDictionary<LogType, string> _logDscritpionDict = new Dictionary<LogType, string>()
            {
                {LogType.Exception,"异常" },
                {LogType.DataAccess,"数据访问" },
                {LogType.DataUpdate,"数据更新" },
                {LogType.DataAdded, "数据添加" },
                {LogType.DataDelete,"数据删除" },
                {LogType.UserLogin,"用户登录" },
            {LogType.PassWordModify, "密码修改" }
            };
        public LogManager(IUnitOfWork unitOfWork, IGetUserInfo getUserInfo, CollentStartup purchasManagerService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _getUserInfo = getUserInfo;
            _purchasManagerService = purchasManagerService;
            _mapper = mapper;
        }

        private IRepository<Log> LogStore => _unitOfWork.GetStore<Log>();
        private IRepository<SysPublishInfo> SysPublishInfoStore => _unitOfWork.GetStore<SysPublishInfo>();
        private IRepository<CenterPublishLog> CenterPublishLogStore => _unitOfWork.GetStore<CenterPublishLog>();
        private IRepository<AppPublishInfo> AppPublishInfoStore => _unitOfWork.GetStore<AppPublishInfo>();

        /// <summary>
        /// 写入日志，当前访问用户从系统中获取
        /// 删除日志
        /// </summary>
        public Task WriteDeleteLogAsync(string dataName, string id)
        {
            return WriteLogAsync(LogType.DataDelete, dataName, $"编号为{id}");
        }

        /// <summary>
        /// 写入日志，当前访问用户从系统中获取
        /// 支持更新、删除、异常三种日志
        /// </summary>
        public Task WriteLogAsync(LogType logType, string dataName, string logContent)
        {
            return Task.Run(async () =>
            {
                var logProfile = $"{dataName}{_logDscritpionDict[logType]}";
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                var log = new Log()
                {
                    Id = Guid.NewGuid().tostring32(),

                    CreateTime = DateTime.Now,
                    LogCode = logType,
                    LogContent = $"{logProfile}:{logContent}",
                    LogProfile = $"{logProfile}日志",
                    OperatorId = userId
                };
                try
                {
                    LogStore.Insert(log);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"写入日志异常：{ex.Message}");
                }


            });
        }

        /// <summary>
        /// 写入数据访问日志
        /// </summary>
        public Task WriteDataAccessLogAsync(string dataName)
        {
            return Task.Run(async () =>
            {
                var logProfile = $"{dataName}{_logDscritpionDict[LogType.DataAccess]}";
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                var log = new Log()
                {
                    Id = Guid.NewGuid().tostring32(),
                    CreateTime = DateTime.Now,
                    LogCode = LogType.DataAccess,
                    LogContent = logProfile,
                    LogProfile = $"{logProfile}日志",
                    OperatorId = userId
                };
                try
                {
                    LogStore.Insert(log);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"写入日志异常：{ex.Message}");
                }

            });
        }

        /// <summary>
        /// 写入登录日志，需要提供访问用户编号
        /// </summary>
        public Task WriteLoginLogAsync(string userId, string logContent)
        {

            return Task.Run(async () =>
            {
                var logProfile = $"{_logDscritpionDict[LogType.UserLogin]}";
                var log = new Log()
                {
                    Id = Guid.NewGuid().tostring32(),
                    CreateTime = DateTime.Now,
                    LogCode = LogType.UserLogin,
                    LogContent = $"{logProfile}:{logContent}",
                    LogProfile = $"{logProfile}日志",
                    OperatorId = userId

                };
                try
                {
                    LogStore.Insert(log);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"写入日志异常：{ex.Message}");
                }


            });
        }

        /// <summary>
        /// 写入密码修改日志，需要提供访问用户编号
        /// </summary>
        public Task WritePsModifyLogAsync(string userId)
        {
            return WriteLoginPsModifyLogAsync(LogType.PassWordModify, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private Task WriteLoginPsModifyLogAsync(LogType logType, string userId, string logContent)
        {

            return Task.Run(async () =>
            {
                var logProfile = $"{_logDscritpionDict[LogType.PassWordModify]}";
                var log = new Log()
                {
                    Id = Guid.NewGuid().tostring32(),
                    CreateTime = DateTime.Now,
                    LogCode = LogType.PassWordModify,
                    LogContent = $"{logProfile}:{logContent}",
                    LogProfile = $"{logProfile}日志",
                    OperatorId = userId

                };
                try
                {
                    LogStore.Insert(log);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"写入日志异常：{ex.Message}");
                }
            });
        }


        #region 中心端发布记录

        /// <summary>
        /// 新增修改发布记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> AddUpdatePublishAsync(SysPublishInfoInput Input)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                SysPublishInfo data = null;
                if (string.IsNullOrEmpty(Input.Id))
                {
                    data = _mapper.Map<SysPublishInfo>(Input);
                    data.Id = Guid.NewGuid().tostring32();
                    data.Founder = userId;
                    data.FounderDate = DateTime.Now;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.DataState = 1;
                    data.isPush = false;
                    SysPublishInfoStore.Insert(data);
                    foreach (var item in data.UpdateCenter.Split(','))
                    {
                        CenterPublishLogStore.Insert(new CenterPublishLog()
                        {
                            Id = Guid.NewGuid().tostring32(),
                            CenterId = item,
                            CenterPublishId = data.Id,
                            isPush = false
                        });
                    }
                    _unitOfWork.SaveChanges();
                }
                else
                {
                    data = await SysPublishInfoStore.Entities.Include(t => t.CenterPublishLogs).FirstOrDefaultAsync(t => t.Id == Input.Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    EntityHelper.CoptyProperty(Input, data);
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.DataState = 1;
                    SysPublishInfoStore.Update(data);
                    foreach (var item in data.UpdateCenter.Split(','))
                    {
                        if (data.CenterPublishLogs.Where(t => t.CenterId == item).Count() <= 0)
                            CenterPublishLogStore.Insert(new CenterPublishLog()
                            {
                                Id = Guid.NewGuid().tostring32(),
                                CenterId = item,
                                CenterPublishId = data.Id,
                                isPush = false
                            });
                    }
                    _unitOfWork.SaveChanges();
                    var result = await _purchasManagerService.SyncSendSysPublishInfo(data);
                }

                Flag = true;

                return Flag;

            });
        }
        /// <summary>
        /// 删除发布记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> DelPublishAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                SysPublishInfo data = null;

                data = await SysPublishInfoStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                if (data == null)
                    throw new Exception(MessageFormater.PrameterNeedExist("id"));
                if (data.isPush)
                    throw new Exception("已推送的记录不可删除");
                data.DataState = 2;
                data.ModifierDate = DateTime.Now;
                data.Modifier = userId;

                SysPublishInfoStore.Update(data);

                _unitOfWork.SaveChanges();
                Flag = true;
                return Flag;

            });
        }



        /// <summary>
        /// 推送发布记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SendPublishAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                SysPublishInfo data = null;

                data = await SysPublishInfoStore.Entities.Include(t => t.CenterPublishLogs).FirstOrDefaultAsync(t => t.Id == Id);
                if (data == null)
                    throw new Exception(MessageFormater.PrameterNeedExist("id"));
                if (data.isPush)
                    throw new Exception("请勿重复推送");
                var result = await _purchasManagerService.SyncSendSysPublishInfo(data);
                if (result)
                {
                    data.isPush = true;
                    SysPublishInfoStore.Update(data);
                    _unitOfWork.SaveChanges();
                    return Flag;
                }
                else
                    return false;



            });
        }
        /// <summary>
        /// 中心端发布记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<SysPublishInfoOutPut[]>> GetPublisQueryableAsync(SysPublishInfoQuery input = null)
        {

            return Task.Run(async () =>
            {
                SysPublishInfoOutPut[] result = null;
                try
                {

                    if (input == null)
                    {
                        input = new SysPublishInfoQuery();

                    }

                    if (input.PageNum <= 0 || input.PageSize <= 0)
                    {
                        input.PageSize = 10; input.PageNum = 1;
                    }

                    Expression<Func<SysPublishInfo, bool>> predicate = t => t.DataState == 1;
                    if (input.BeginRunDate.HasValue)
                        predicate = predicate.And(t => t.PublishDate >= input.BeginRunDate.Value.AddHours(-input.BeginRunDate.Value.Hour));
                    if (input.EndRunDate.HasValue)
                        predicate = predicate.And(t => t.PublishDate <= input.EndRunDate.Value.AddHours((+24 - input.EndRunDate.Value.Hour)));

                    var Sum = SysPublishInfoStore.Entities.Where(predicate).OrderByDescending(t => t.PublishDate);
                    var Dialysis = await PaginatedList<SysPublishInfo>.CreateAsync(Sum, input.PageNum, input.PageSize);

                    result = _mapper.Map<SysPublishInfoOutPut[]>(Dialysis);
                    return new PageData<SysPublishInfoOutPut[]>(result, Dialysis.Count);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

            });
        }


        /// <summary>
        /// 新增修改app发布记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> AddUpdateAPPPublishAsync(AppPublishInfoInput Input)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                AppPublishInfo data = null;
                if (string.IsNullOrEmpty(Input.Id))
                {
                    data = _mapper.Map<AppPublishInfo>(Input);
                    data.Id = Guid.NewGuid().tostring32();
                    data.Founder = userId;
                    data.FounderDate = DateTime.Now;
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.DataState = 1;
                    data.isPush = false;
                    AppPublishInfoStore.Insert(data);
                }
                else
                {
                    data = await AppPublishInfoStore.GetFirstOrDefaultAsync(t => t.Id == Input.Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    EntityHelper.CoptyProperty(Input, data);
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.DataState = 1;
                    AppPublishInfoStore.Update(data);

                }
                _unitOfWork.SaveChanges();
                Flag = true;

                return Flag;

            });
        }
        /// <summary>
        /// 删除app发布记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> DelAPPPublishAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool Flag = false;
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                AppPublishInfo data = null;

                data = await AppPublishInfoStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                if (data == null)
                    throw new Exception(MessageFormater.PrameterNeedExist("id"));

                data.DataState = 2;
                data.ModifierDate = DateTime.Now;
                data.Modifier = userId;

                AppPublishInfoStore.Update(data);

                _unitOfWork.SaveChanges();
                Flag = true;
                return Flag;

            });
        }



        /// <summary>
        /// 推送app发布记录
        /// </summary>
        /// <returns></returns>
        public Task<bool> SendAPPPublishAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool Flag = true;
                SysPublishInfo data = null;

                data = await SysPublishInfoStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                if (data == null)
                    throw new Exception(MessageFormater.PrameterNeedExist("id"));
                if (data.isPush)
                    throw new Exception("请勿重复推送");
                await _purchasManagerService.SyncSendSysPublishInfo(data);
                data.isPush = true;
                SysPublishInfoStore.Update(data);
                _unitOfWork.SaveChanges();
                return Flag;

            });
        }
        /// <summary>
        /// 中心端app发布记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<AppPublishInfoOutPut[]>> GetAPPPublisQueryableAsync(SysPublishInfoQuery input = null)
        {

            return Task.Run(async () =>
            {
                AppPublishInfoOutPut[] result = null;
                try
                {

                    if (input == null)
                    {
                        input = new SysPublishInfoQuery();

                    }

                    if (input.PageNum <= 0 || input.PageSize <= 0)
                    {
                        input.PageSize = 10; input.PageNum = 1;
                    }

                    Expression<Func<AppPublishInfo, bool>> predicate = t => t.DataState == 1;
                    if (input.BeginRunDate.HasValue)
                        predicate = predicate.And(t => t.FounderDate >= input.BeginRunDate.Value.AddHours(-input.BeginRunDate.Value.Hour));
                    if (input.EndRunDate.HasValue)
                        predicate = predicate.And(t => t.FounderDate <= input.EndRunDate.Value.AddHours((+24 - input.EndRunDate.Value.Hour)));

                    var Sum = AppPublishInfoStore.Entities.Where(predicate).OrderByDescending(t => t.FounderDate);
                    var Dialysis = await PaginatedList<AppPublishInfo>.CreateAsync(Sum, input.PageNum, input.PageSize);

                    result = _mapper.Map<AppPublishInfoOutPut[]>(Dialysis);
                    return new PageData<AppPublishInfoOutPut[]>(result, Dialysis.Count);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

            });
        }

        /// <summary>
        /// 获取最新
        /// </summary>
        /// <returns></returns>
        public Task<AppPublishInfoOutPut> GetFirstAPPPublisQueryableAsync()
        {

            return Task.Run(async () =>
            {
                AppPublishInfoOutPut outPut = null;
                var data = await AppPublishInfoStore.GetFirstOrDefaultAsync(t => t.FounderDate == AppPublishInfoStore.Entities.Where(r => r.DataState == 1).Max(r => r.FounderDate));
                if (data != null)
                    outPut = _mapper.Map<AppPublishInfoOutPut>(data);

                return outPut;

            });
        }


        #endregion

    }
}
