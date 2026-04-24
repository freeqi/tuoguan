﻿﻿using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Utils;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CDGService.Data.Enums;

namespace CDGService.WebAPI.DataCore
{
    public class MaintenanceRecordManger
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly IMapper _mapper;
        public MaintenanceRecordManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IMapper mapper)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _mapper = mapper;
        }
        private IRepository<MaintenanceRecord> MaintenanceRecordStore => _unitOfWork.GetStore<MaintenanceRecord>();

        private IRepository<EquipmentKeepRecord> EquipmentKeepRecordStore => _unitOfWork.GetStore<EquipmentKeepRecord>();

        private IRepository<BiochemicalTest> BiochemicalTestStore => _unitOfWork.GetStore<BiochemicalTest>();
        /// <summary>
        /// 维修
        /// </summary>
        /// <param name="EqId"></param>
        /// <returns></returns>
        public Task<MaintenanceRecordOutPut[]> GetMrByiIdAsync(string EqId)
        {

            return Task.Run(async () =>
            {
                MaintenanceRecordOutPut[] result = null;

                try
                {
                    Expression<Func<MaintenanceRecord, bool>> predicate = t => t.DataState == 1;

                    predicate = predicate.And(t => t.EquipmentInfoId == EqId);
                    var data = await MaintenanceRecordStore.Entities.Where(predicate).ToArrayAsync();
                    result = _mapper.Map<MaintenanceRecordOutPut[]>(data);
                    if (result == null || result.Length <= 0)
                    {
                        // throw new Exception(MessageFormater.PrameterResultsIsNull("查询设备维修记录")); 
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        /// <summary>
        /// 维保 
        /// </summary>
        /// <param name="EqId"></param>
        /// <returns></returns>

        public Task<EquipmentKeepRecordOutPut[]> GetEkrByiIdAsync(string EqId)
        {

            return Task.Run(async () =>
            {
                EquipmentKeepRecordOutPut[] result = null; 
                try
                {
                    Expression<Func<EquipmentKeepRecord, bool>> predicate = t => t.DataState == 1;
                    predicate = predicate.And(t => t.EquipmentInfoId == EqId);
                    var data = await EquipmentKeepRecordStore.Entities.Where(predicate).ToArrayAsync();
                    result = _mapper.Map<EquipmentKeepRecordOutPut[]>(data);
                    if (result == null || result.Length <= 0)
                    {
                        //  throw new Exception(MessageFormater.PrameterResultsIsNull("查询设备维保记录"));
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        /// <summary>
        /// 生化检测
        /// </summary>
        /// <param name="EqId"></param>
        /// <returns></returns>

        public Task<BiochemicalTest[]> GetEbtByiIdAsync(String EqId, string pHEnum, string TestType)
        {

            return Task.Run(async () =>
            {
                BiochemicalTest[] result = null;

                try
                {
                    if (EqId + "" == "0" || EqId + "" == "")
                    {
                        throw new Exception(MessageFormater.PrameterNeedProvider("EqId"));
                    }
                    else
                   {
                        Expression<Func<BiochemicalTest, bool>> predicate = t => t.DataState == 1;

                        predicate = predicate.And(t => t.EquipmentInfoId == EqId && t.TestSpecimens == pHEnum&&t.TestType==TestType);
                        var data = await BiochemicalTestStore.Entities.Where(predicate).ToArrayAsync();
                        result = _mapper.Map<BiochemicalTest[]>(data);
                        if (result == null || result.Length <= 0)
                        {
                            // throw new Exception(MessageFormater.PrameterResultsIsNull("查询设备生化检测记录"));
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }
    }
}
