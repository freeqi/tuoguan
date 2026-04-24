﻿﻿using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Utils;
using CDGService.Store;
using CDGService.Data.Helper;

namespace CDGService.WebAPI.DataCore
{
    public class HospitalFeelingManager : XmlSql
    {
        private readonly Data.DocumentSetting _documentSetting;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly string _ClassName;
        private readonly IMapper _mapper;
        public HospitalFeelingManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<Data.DocumentSetting> documentSetting, IMapper mapper)
        {
            _ClassName = GetType().Name;//当前类名称
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _documentSetting = documentSetting.Value;
            _mapper = mapper;
        }
        private IRepository<OutpatientDetailsLog> OutpatientDetailsLogStore => _unitOfWork.GetStore<OutpatientDetailsLog>();
        private IRepository<OutpatientLog> OutpatientLogStore => _unitOfWork.GetStore<OutpatientLog>();
        private IRepository<DisinfectionRoom> DisinfectionRoomStore => _unitOfWork.GetStore<DisinfectionRoom>();
        private IRepository<InspectionWaterPollution> InspectionWaterPollutionStore => _unitOfWork.GetStore<InspectionWaterPollution>();
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        //public virtual List<MedicalDrugExtension> MedicalDrugExtensions { get; set; } = new List<MedicalDrugExtension>();

        private IRepository<PatientInfectiousCheck> PatientInfectiousCheckStore => _unitOfWork.GetStore<PatientInfectiousCheck>();
        private IRepository<InfectiousDiseaseRegister> InfectiousDiseaseRegisterStore => _unitOfWork.GetStore<InfectiousDiseaseRegister>();

        private IRepository<InfectiousDiseasesRecord> InfectiousDiseasesRecordStore => _unitOfWork.GetStore<InfectiousDiseasesRecord>();
        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<BiochemicalTest> BiochemicalTestStore => _unitOfWork.GetStore<BiochemicalTest>();
        #region 门诊日志

        /// <summary>
        /// 门诊日志列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<OutpatientDetailsLogOutPut[]>> GetOutpatientDetailsLogQueryableAsync(OutpatientDetailsLogQueryInPut input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                List<OutpatientDetailsLogOutPut> result = new
                 List<OutpatientDetailsLogOutPut>();
                int count = 0;

                try
                {
                    if (input == null || input.PageNum <= 0 || input.PageSize <= 0)
                    {
                        var datas = await OutpatientDetailsLogStore.Entities.Include(t => t.Patient).ToArrayAsync();
                        result = _mapper.Map<OutpatientDetailsLogOutPut[]>(datas).ToList();
                        count = result.Count();
                    }
                    else
                    {
                        Expression<Func<OutpatientDetailsLog, bool>> predicate = t => t.DataState == 1;
                        if (input.CenterId + "" != "" && input.CenterId + "" != "0")
                            predicate = predicate.And(t => t.CenterId == input.CenterId);
                        if (input.BeginTime + "" != "")
                        {
                            DateTime btime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, input.BeginTime.Value.Day, 0, 0, 0);
                            DateTime etime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, input.BeginTime.Value.Day, 23, 59, 59);

                            predicate = predicate.And(t => btime <= t.DiagnosisDate && etime >= t.DiagnosisDate);
                        }
                        if (input.PageNum > 0 && input.PageSize > 0)
                        {

                            var Sum = await OutpatientDetailsLogStore.Entities.Include(t => t.Patient).Where(predicate).ToArrayAsync();

                            if (Sum != null && Sum.Length > 0)
                            {
                                var items = Sum.Skip((input.PageNum - 1) * input.PageSize).Take(input.PageSize).ToList();
                                //var Dialysis = await PaginatedList<OutpatientDetailsLog>.CreateAsync(Sum.ListOutpatientDetailsLog.AsQueryable(), input.PageNum, input.PageSize);
                                // result = _mapper.Map<CenterDialysisOutPut[]>(Dialysis);
                                count = Sum.Length;

                                result = _mapper.Map<OutpatientDetailsLogOutPut[]>(items).ToList();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<OutpatientDetailsLogOutPut[]>(result.ToArray(), count);
            });
        }

        #endregion

        #region 院感控制
        /// <summary>
        /// 消毒
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<DisinfectionRoomOutPut[]>> GetDisinfectionRoomQueryableAsync(DisinfectionRoomQueryInPut input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                DisinfectionRoomOutPut[] result = null;

                int count = 0;

                try
                {
                    if (input == null || input.PageNum <= 0 || input.PageSize <= 0)
                    {
                        var datas = await DisinfectionRoomStore.Entities.Include(t => t.CenterDialysis).Include(t => t.Partition).OrderByDescending(t => t.DisinfectionTime).ToArrayAsync();
                        result = _mapper.Map<DisinfectionRoomOutPut[]>(datas);
                        count = result.Count();
                    }
                    else
                    {
                        Expression<Func<DisinfectionRoom, bool>> predicate = t => t.DataState == 1;
                        if (input.CenterId + "" != "" && input.CenterId + "" != "0")
                            predicate = predicate.And(t => t.CenterId == input.CenterId);
                        if (input.IsQualified.HasValue && input.IsQualified != 2)
                        {
                            bool falg = input.IsQualified == 1 ? true : false;
                            predicate = predicate.And(t => t.IsQualified == falg);
                        }

                        if (input.BeginTime + "" != "")
                            predicate = predicate.And(t => t.DisinfectionTime.Value >= input.BeginTime.Value);
                        if (input.EndTime + "" != "")
                            predicate = predicate.And(t => t.DisinfectionTime.Value <= input.EndTime.Value);
                        if (input.PageNum > 0 && input.PageSize > 0)
                        {
                            var Sum = DisinfectionRoomStore.Entities.Include(t => t.CenterDialysis).Include(t => t.Partition).Where(predicate).OrderByDescending(t => t.DisinfectionTime);
                            if (Sum != null)
                            {
                                //   var items = Sum.ListOutpatientDetailsLog.Skip((input.PageNum - 1) * input.PageSize).Take(input.PageSize).ToList();
                                var Dialysis = await PaginatedList<DisinfectionRoom>.CreateAsync(Sum, input.PageNum, input.PageSize);
                                result = _mapper.Map<DisinfectionRoomOutPut[]>(Dialysis);
                                count = Sum.Count();


                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<DisinfectionRoomOutPut[]>(result, count);
            });
        }

        //InspectionWaterPollution


        /// <summary>
        /// 生物水检测
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<InspectionWaterPollutionOutPut[]>> GetInspectionWaterPollutionQueryableAsync(InspectionWaterPollutionQueryInPut input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                InspectionWaterPollutionOutPut[] result = null;

                int count = 0;

                try
                {
                    if (input == null || input.PageNum <= 0 || input.PageSize <= 0)
                    {
                        var datas = await InspectionWaterPollutionStore.Entities.Include(t => t.CenterDialysis).ToArrayAsync();
                        result = _mapper.Map<InspectionWaterPollutionOutPut[]>(datas);
                        count = result.Count();
                    }
                    else
                    {
                        Expression<Func<InspectionWaterPollution, bool>> predicate = t => t.DataState == 1;
                        if (input.CenterId + "" != "" && input.CenterId + "" != "0")
                            predicate = predicate.And(t => t.CenterId == input.CenterId);
                        if (input.IsQualified.HasValue && input.IsQualified != 2)
                        {
                            bool falg = input.IsQualified == 1 ? true : false;
                            predicate = predicate.And(t => t.IsQualified == falg);
                        }
                        if (input.BeginTime + "" != "")
                            predicate = predicate.And(t => t.DisinfectionTime.Value >= input.BeginTime.Value);
                        if (input.EndTime + "" != "")
                            predicate = predicate.And(t => t.DisinfectionTime.Value <= input.EndTime.Value);
                        if (input.PageNum > 0 && input.PageSize > 0)
                        {
                            var Sum = InspectionWaterPollutionStore.Entities.Include(t => t.CenterDialysis).Where(predicate);
                            if (Sum != null)
                            {
                                
                                //   var items = Sum.ListOutpatientDetailsLog.Skip((input.PageNum - 1) * input.PageSize).Take(input.PageSize).ToList();
                                var Dialysis = await PaginatedList<InspectionWaterPollution>.CreateAsync(Sum, input.PageNum, input.PageSize);
                                result = _mapper.Map<InspectionWaterPollutionOutPut[]>(Dialysis);
                                count = Sum.Count();


                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<InspectionWaterPollutionOutPut[]>(result, count);
            });
        }

        /// <summary>
        /// 新入患者传染病发病率统计 列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<PatientInfectiousCheckOutPut[]>> GetPatientInfectiousCheckQueryableAsync(PatientInfectiousCheckQueryInPut input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                List<PatientInfectiousCheckOutPut> result = new List<PatientInfectiousCheckOutPut>();
                List<Patient> PatientData = new List<Patient>();
                int count = 0;

                try
                {
                    if (input == null || input.PageNum <= 0 || input.PageSize <= 0)
                        throw new Exception("请提供有效参数");
                    DateTime beginTime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, 1, 0, 0, 0);
                    DateTime EndTime = new DateTime(input.EndTime.Value.Year, input.EndTime.Value.Month, 28, 0, 0, 0);
                    //该时段入院的在院患者
                    if (input.CenterId + "" == "" || input.CenterId + "" == "0")
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Where(t => t.IsDelete == false && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    else
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Where(t => t.IsDelete == false && input.CenterId == t.CenterId && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    List<string> paIdList = PatientData.Select(t => t.Id).ToList();
                    if (paIdList != null && paIdList.Count > 0)
                    {
                        List<InfectiousDiseaseRegister> InfectiousData = new List<InfectiousDiseaseRegister>();
                        string HIV = "";
                        //病史
                        switch (input.IsQualified)
                        {
                            case 2:
                                HIV = "乙";
                                InfectiousData = await InfectiousDiseaseRegisterStore.Entities.Where(t => t.DataState == 1 && t.Description.Contains(HIV) && !t.Description.Contains("无") && !t.Description.Contains("正常") && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();
                                break;
                            case 3:
                                HIV = "丙";
                                InfectiousData = await InfectiousDiseaseRegisterStore.Entities.Where(t => t.DataState == 1 && t.Description.Contains(HIV) && !t.Description.Contains("无") && !t.Description.Contains("正常") && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();
                                break;
                            default:
                                InfectiousData = await InfectiousDiseaseRegisterStore.Entities.Where(t => t.DataState == 1 && !t.Description.Contains("无") && !t.Description.Contains("正常") && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();
                                break;
                        }
                        foreach (var item in InfectiousData)
                        {
                            var patient = PatientData.Where(t => t.Id == item.PatientId).First();
                            PatientInfectiousCheckOutPut put = new PatientInfectiousCheckOutPut()
                            {
                                CenterId = patient.CenterId,
                                InfectiousType = item.Description,
                                CenterName = patient.Dialysis.ShortName,
                                CureCode = patient.PatientNo,
                                DisinfectionTime = item.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                MorbidityTime = item.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                PatentName = patient.Name,
                                PatentSex = patient.Sex,
                                InfectiousTypeName = item.Description,
                            };
                            result.Add(put);
                        }
                    }

                    //else {
                    //}
                    //Expression<Func<PatientInfectiousCheck, bool>> predicate = t => t.DataState == 1;
                    //if (input.CenterId + "" != "" && input.CenterId + "" != "0")
                    //    predicate = predicate.And(t => t.CenterId == input.CenterId);
                    //if (input.IsQualified.HasValue && input.IsQualified != 2)
                    //{
                    //    bool falg = input.IsQualified == 1 ? true : false;
                    //    predicate = predicate.And(t => t.IsQualified == falg);
                    //}
                    //if (input.BeginTime + "" != "")
                    //    predicate = predicate.And(t => t.DisinfectionTime.Value >= input.BeginTime.Value);
                    //if (input.EndTime + "" != "")
                    //    predicate = predicate.And(t => t.DisinfectionTime.Value <= input.EndTime.Value);
                    //if (input.PageNum > 0 && input.PageSize > 0)
                    //{
                    //    var Sum = PatientInfectiousCheckStore.Entities.Include(t => t.CenterDialysis).Include(t => t.Patient).Include(t => t.DicInfectiousType).Where(predicate);
                    //    if (Sum != null)
                    //    {

                    //        //   var items = Sum.ListOutpatientDetailsLog.Skip((input.PageNum - 1) * input.PageSize).Take(input.PageSize).ToList();
                    //        var Dialysis = await PaginatedList<PatientInfectiousCheck>.CreateAsync(Sum, input.PageNum, input.PageSize);
                    //        result = _mapper.Map<PatientInfectiousCheckOutPut[]>(Dialysis);
                    //        count = Sum.Count();
                    //    }
                    //}

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<PatientInfectiousCheckOutPut[]>(result.ToArray(), count);
            });
        }
        /// <summary>
        ///新入患者体检情况（发病率）统计 图形
        /// </summary>
        /// <returns></returns>
        public Task<LineOutPut> GetPatientInfectiousChecksStatisticsByCenterAsync(PatientInfectiousCheckQueryInPut input)
        {

            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<PatientInfectiousCheck> list = new List<PatientInfectiousCheck>();
                bool IsDel = false;
                List<PatientInfectiousCheckOutPut> result1 = new List<PatientInfectiousCheckOutPut>();
                List<Patient> PatientData = new List<Patient>();
                List<CenterDialysis> centerDialyses = new List<CenterDialysis>();

                try
                {
                    if (input == null)
                        throw new Exception("请提供有效参数");
                    DateTime beginTime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, 1, 0, 0, 0);
                    DateTime EndTime = new DateTime(input.EndTime.Value.Year, input.EndTime.Value.Month, 28, 0, 0, 0);
                    //该时段入院的在院患者
                    if (input.CenterId + "" == "" || input.CenterId + "" == "0")
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Where(t => t.IsDelete == false && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    else
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Where(t => t.IsDelete == false && input.CenterId == t.CenterId && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    List<string> paIdList = PatientData.Select(t => t.Id).ToList();
                    if (paIdList != null && paIdList.Count > 0)
                    {
                        List<InfectiousDiseaseRegister> InfectiousData = new List<InfectiousDiseaseRegister>();
                        string HIV = "";
                        //病史
                        switch (input.IsQualified)
                        {
                            case 2:
                                HIV = "乙";
                                InfectiousData = await InfectiousDiseaseRegisterStore.Entities.Where(t => t.DataState == 1 && t.Description.Contains(HIV) && !t.Description.Contains("无") && !t.Description.Contains("正常") && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();
                                break;
                            case 3:
                                HIV = "丙";
                                InfectiousData = await InfectiousDiseaseRegisterStore.Entities.Where(t => t.DataState == 1 && t.Description.Contains(HIV) && !t.Description.Contains("无") && !t.Description.Contains("正常") && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();
                                break;
                            default:
                                InfectiousData = await InfectiousDiseaseRegisterStore.Entities.Where(t => t.DataState == 1 && !t.Description.Contains("无") && !t.Description.Contains("正常") && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();
                                break;
                        }
                        foreach (var item in InfectiousData)
                        {
                            var patient = PatientData.Where(t => t.Id == item.PatientId).First();
                            PatientInfectiousCheckOutPut put = new PatientInfectiousCheckOutPut()
                            {
                                CenterId = patient.CenterId,
                                InfectiousType = item.Description,
                                CenterName = patient.Dialysis.ShortName,
                                CureCode = patient.PatientNo,
                                DisinfectionTime = item.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                MorbidityTime = item.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                PatentName = patient.Name,
                                PatentSex = patient.Sex,
                                InfectiousTypeName = item.Description,
                            };
                            result1.Add(put);
                        }
                    }
                    if (input.CenterId != "" && input.CenterId != "0")
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.Id == input.CenterId).ToListAsync();
                    else
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToListAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "发病次数", "新入患者人数" };
                    result.data = centerDialyses.Select(t => t.DialysisName).ToList();
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "发病次数" : "新入患者人数";
                        foreach (var item in centerDialyses)
                        {
                            int count = 0;
                            if (state)
                                count = result1.Where(t => t.CenterId == item.Id).Count();
                            else
                                count = PatientData.Where(t => t.CenterId == item.Id).Count();
                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }


        ///新入患者传染病检测完成率

        public Task<PageData<PatientInfectiousCheckOutPut[]>> GetInfectiousDiseasesRecordStoreCheckQueryableAsync(PatientInfectiousCheckQueryInPut input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                List<PatientInfectiousCheckOutPut> result = new List<PatientInfectiousCheckOutPut>();
                List<Patient> PatientData = new List<Patient>();
                int count = 0;

                try
                {
                    if (input == null || input.PageNum <= 0 || input.PageSize <= 0)
                        throw new Exception("请提供有效参数");
                    DateTime beginTime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, 1, 0, 0, 0);
                    DateTime EndTime = new DateTime(input.EndTime.Value.Year, input.EndTime.Value.Month, 28, 0, 0, 0);
                    //该时段入院的在院患者
                    if (input.CenterId + "" == "" || input.CenterId + "" == "0")
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Where(t => t.IsDelete == false && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    else
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Where(t => t.IsDelete == false && input.CenterId == t.CenterId && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    List<string> paIdList = PatientData.Select(t => t.Id).ToList();
                    if (paIdList != null && paIdList.Count > 0)
                    {
                        List<InfectiousDiseasesRecord> InfectiousData = new List<InfectiousDiseasesRecord>();

                        //病史

                        InfectiousData = await InfectiousDiseasesRecordStore.Entities.Where(t => t.DataState == 1 && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();

                        foreach (var item in PatientData)
                        {
                            var patient = InfectiousData.Where(t => t.PatientId == item.Id).FirstOrDefault();
                            PatientInfectiousCheckOutPut put = new PatientInfectiousCheckOutPut()
                            {
                                CenterId = item.CenterId,
                                InfectiousType = item.SBloodBorneDisease.Name,
                                CenterName = item.Dialysis.ShortName,
                                CureCode = item.PatientNo,
                                DisinfectionTime = patient != null ? patient.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                                MorbidityTime = item.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                PatentName = item.Name,
                                PatentSex = item.Sex,
                                InfectiousTypeName = item.SBloodBorneDisease.Name,
                                IsQualified = patient == null ? "未完成" : "完成",

                            };
                            result.Add(put);
                        }
                    }

                    //else {
                    //}
                    //Expression<Func<PatientInfectiousCheck, bool>> predicate = t => t.DataState == 1;
                    //if (input.CenterId + "" != "" && input.CenterId + "" != "0")
                    //    predicate = predicate.And(t => t.CenterId == input.CenterId);
                    //if (input.IsQualified.HasValue && input.IsQualified != 2)
                    //{
                    //    bool falg = input.IsQualified == 1 ? true : false;
                    //    predicate = predicate.And(t => t.IsQualified == falg);
                    //}
                    //if (input.BeginTime + "" != "")
                    //    predicate = predicate.And(t => t.DisinfectionTime.Value >= input.BeginTime.Value);
                    //if (input.EndTime + "" != "")
                    //    predicate = predicate.And(t => t.DisinfectionTime.Value <= input.EndTime.Value);
                    //if (input.PageNum > 0 && input.PageSize > 0)
                    //{
                    //    var Sum = PatientInfectiousCheckStore.Entities.Include(t => t.CenterDialysis).Include(t => t.Patient).Include(t => t.DicInfectiousType).Where(predicate);
                    //    if (Sum != null)
                    //    {

                    //        //   var items = Sum.ListOutpatientDetailsLog.Skip((input.PageNum - 1) * input.PageSize).Take(input.PageSize).ToList();
                    //        var Dialysis = await PaginatedList<PatientInfectiousCheck>.CreateAsync(Sum, input.PageNum, input.PageSize);
                    //        result = _mapper.Map<PatientInfectiousCheckOutPut[]>(Dialysis);
                    //        count = Sum.Count();
                    //    }
                    //}

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<PatientInfectiousCheckOutPut[]>(result.ToArray(), count);
            });
        }


        /// <summary>
        ///新入患者体检情（检查完成率）统计 图形
        /// </summary>
        /// <returns></returns>
        public Task<LineOutPut> GetInfectiousDiseasesRecordChartCheckQueryableAsync(PatientInfectiousCheckQueryInPut input)
        {

            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<PatientInfectiousCheck> list = new List<PatientInfectiousCheck>();
                bool IsDel = false;
                List<PatientInfectiousCheckOutPut> result1 = new List<PatientInfectiousCheckOutPut>();
                List<Patient> PatientData = new List<Patient>();
                List<CenterDialysis> centerDialyses = new List<CenterDialysis>();
                List<InfectiousDiseasesRecord> InfectiousData = new List<InfectiousDiseasesRecord>();
                try
                {
                    if (input == null)
                        throw new Exception("请提供有效参数");
                    DateTime beginTime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, 1, 0, 0, 0);
                    DateTime EndTime = new DateTime(input.EndTime.Value.Year, input.EndTime.Value.Month, 28, 0, 0, 0);
                    //该时段入院的在院患者
                    if (input.CenterId + "" == "" || input.CenterId + "" == "0")
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Where(t => t.IsDelete == false && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    else
                        PatientData = await PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Where(t => t.IsDelete == false && input.CenterId == t.CenterId && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate >= beginTime && t.ReceiveDate < EndTime).ToListAsync();
                    List<string> paIdList = PatientData.Select(t => t.Id).ToList();
                    if (paIdList != null && paIdList.Count > 0)
                    {


                        //病史

                        InfectiousData = await InfectiousDiseasesRecordStore.Entities.Where(t => t.DataState == 1 && t.FounderDate >= beginTime && t.FounderDate < EndTime.AddMonths(1) && paIdList.Contains(t.PatientId)).ToListAsync();

                        //foreach (var item in PatientData)
                        //{
                        //    var patient = InfectiousData.Where(t => t.PatientId == item.Id).FirstOrDefault();
                        //    PatientInfectiousCheckOutPut put = new PatientInfectiousCheckOutPut()
                        //    {
                        //        CenterId = item.CenterDialysisId,
                        //        InfectiousType = item.SBloodBorneDisease.Name,
                        //        CenterName = item.Dialysis.ShortName,
                        //        CureCode = item.PatientNo,
                        //        DisinfectionTime = patient != null ? patient.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        //        MorbidityTime = item.FounderDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        //        PatentName = item.Name,
                        //        PatentSex = item.Sex,
                        //        InfectiousTypeName = item.SBloodBorneDisease.Name,
                        //        IsQualified = patient == null ? "未完成" : "完成",

                        //    };
                        //    result1.Add(put);
                        //}
                    }
                    if (input.CenterId != "" && input.CenterId != "0")
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.Id == input.CenterId).ToListAsync();
                    else
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToListAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "新入患者", "检测人数" };
                    result.data = centerDialyses.Select(t => t.ShortName).ToList();
                    bool flag = false;
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "新入患者" : "检测人数";
                        foreach (var item in centerDialyses)
                        {
                            //if (!flag)
                            //    result.data.Add(i.Year + "年" + i.Month + "月");

                            int count = 0;
                            if (state)
                                count = PatientData.Where(t => t.CenterId == item.Id).Count();
                            else
                                count = InfectiousData.Where(t => t.CenterId == item.Id).Count();

                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);


                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        /*
         每月处理水菌落数≤100CFU/ML;每3月检查内毒素≤0.25EU/ML
         */
        public Task<LineOutPut> GetInspectionWaterPollutionsStatisticsAsync(PatientInfectiousCheckQueryInPut input = null)

        {
            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<BiochemicalTest> list = new List<BiochemicalTest>();
                bool IsDel = false;
                List<CenterDialysis> centerDialyses = new List<CenterDialysis>();

                try
                {
                    if (input == null)
                        throw new Exception("请提供有效参数");
                    DateTime beginTime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, 1, 0, 0, 0);
                    DateTime EndTime = new DateTime(input.EndTime.Value.Year, input.EndTime.Value.Month, 28, 0, 0, 0);

                    var BioData = await BiochemicalTestStore.Entities.Where(t => t.DataState == 1 && t.TestType == input.BioType).ToListAsync();
                    if (input.CenterId != "" && input.CenterId != "0")
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.Id == input.CenterId).ToListAsync();
                    else
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToListAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "检验次数", "合格次数" };
                    result.data = centerDialyses.Select(t => t.ShortName).ToList();
                    bool flag = false;
                    double? Value = 0;
                    switch (input.BioType)
                    {
                        case "细菌培养":

                            Value = 100;
                            break;
                        case "内毒素":
                            Value = 0.25;
                            break;

                        default:
                            break;
                    }
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "检验次数" : "合格次数";
                        foreach (var item in centerDialyses)
                        {
                            //if (!flag)
                            //    result.data.Add(i.Year + "年" + i.Month + "月");

                            int count = 0;
                            if (state)
                                count = BioData.Where(t => t.CenterId == item.Id).Count();
                            else
                                count = BioData.Where(t => t.CenterId == item.Id && t.Values <= Value).Count();

                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);


                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });

        }

        public Task<PageData<InspectionWaterPollutionOutPut[]>> GetInspectionWaterPollutionQueryableAsync(PatientInfectiousCheckQueryInPut input = null)

        {
            return Task.Run(async () =>
            {
                List<InspectionWaterPollutionOutPut> result = new List<InspectionWaterPollutionOutPut>();
                List<BiochemicalTest> list = new List<BiochemicalTest>();
                bool IsDel = false;
                List<CenterDialysis> centerDialyses = new List<CenterDialysis>();
                int count = 1;

                try
                {
                    if (input == null)
                        throw new Exception("请提供有效参数");
                    double? Value = 0;
                    switch (input.BioType)
                    {
                        case "细菌培养":

                            Value = 100;
                            break;
                        case "内毒素":
                            Value = 0.25;
                            break;
                        default:
                            break;
                    }
                    DateTime beginTime = new DateTime(input.BeginTime.Value.Year, input.BeginTime.Value.Month, 1, 0, 0, 0);
                    DateTime EndTime = new DateTime(input.EndTime.Value.Year, input.EndTime.Value.Month, 28, 0, 0, 0);

                    var BioData = await BiochemicalTestStore.Entities.Include(t=>t.SystemDictionary).Include(t=>t.CenterDialysis).Where(t => t.DataState == 1 && t.TestType == input.BioType).ToListAsync();
                    if (input.CenterId != "" && input.CenterId != "0")
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.Id == input.CenterId).ToListAsync();
                    else
                        centerDialyses = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToListAsync();

                    foreach (var item in BioData)
                    {
                        result.Add(new InspectionWaterPollutionOutPut()
                        {

                            CenterName = item.CenterDialysis.ShortName,
                            DisinfectionTime = item.TestDate.Value.ToString("yyyy-MMidd HH:mm:ss"),
                            WaterColonyCount = item.Values + "",
                            Specimen = item.SystemDictionary.Name,
                            Endotoxin = item.Values + "",
                            IsQualified = item.Values > Value ? "不合格" : "合格", 
                             
                        });
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<InspectionWaterPollutionOutPut[]>(result.ToArray(), count);
            });

        }
        #region 统计

        //治疗室消毒合格率统计
        /// <summary>
        /// 按月份统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetDisinfectionRoomStatisticsAsync(DisinfectionRoomStatisticsQueryInput input = null)
        {

            return Task.Run(async () =>
            {
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();
                List<DisinfectionRoom> list = new List<DisinfectionRoom>();

                try
                {
                    if (input == null)
                    {
                        list = await DisinfectionRoomStore.Entities.Include(t => t.CenterDialysis).ToListAsync();
                    }
                    else
                    {
                        Expression<Func<DisinfectionRoom, bool>> predicate = t => t.DataState == 1;
                        if (input.CenterId + "" != "" && input.CenterId + "" != "0")
                            predicate = predicate.And(t => t.CenterId == input.CenterId);

                        if (input.BeginTime + "" != "")
                            predicate = predicate.And(t => t.DisinfectionTime.Value >= input.BeginTime.Value);
                        if (input.EndTime + "" != "")
                            predicate = predicate.And(t => t.DisinfectionTime.Value <= input.EndTime.Value);
                        list = await DisinfectionRoomStore.Entities.Include(t => t.CenterDialysis).Where(predicate).ToListAsync();
                    }

                    //if (list.Count <= 0)
                    //    return result.ToArray();
                    int Year = 1;
                    for (DateTime i = DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1); i < DateTime.Now; i = i.AddMonths(+1))
                    {
                        var MonthData = list.Where(t => t.DisinfectionTime.Value.Year == i.Year && t.DisinfectionTime.Value.Month == i.Month);
                        var OKData = MonthData.Where(t => t.IsQualified == true);
                        EmplyeeByStaBisOutPut output = new EmplyeeByStaBisOutPut();
                        output.name = i.Year + "年" + i.Month + "月";
                        output.value = MonthData.Count() == 0 ? 0.00 : ValueHelper.ToRound(OKData.Count() * 1.00 / MonthData.Count() * 1.00).Value;
                        result.Add(output);
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result.ToArray();
            });
        }


        //按透析中心统计

        /// <summary>
        ///治疗室消毒合格率统计- 按月份统计
        /// </summary>
        /// <returns></returns>
        public Task<LineOutPut> GetDisinfectionRoomStatisticsByCenterAsync(DateTime dateTime)
        {

            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<DisinfectionRoom> list = new List<DisinfectionRoom>();

                try
                {


                    Expression<Func<DisinfectionRoom, bool>> predicate = t => t.DataState == 1;

                    predicate = predicate.And(t => t.DisinfectionTime.Value.Year == dateTime.Year && t.DisinfectionTime.Value.Month == dateTime.Month);

                    list = await DisinfectionRoomStore.Entities.Include(t => t.CenterDialysis).Where(predicate).ToListAsync();


                    //if (list.Count <= 0)
                    //    return result;
                    var center = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "合格", "不合格" };
                    result.data = center.Select(t => t.DialysisName).ToList();
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "合格" : "不合格";
                        foreach (var item in center)
                        {
                            int count = list.Where(t => t.CenterId == item.Id && t.IsQualified == state).Count();
                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);
                    }
                    //int Year = 1;
                    //for (DateTime i = DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1); i < DateTime.Now; i = i.AddMonths(+1))
                    //{
                    //    var MonthData = list.Where(t => t.DisinfectionTime.Value.Year == i.Year && t.DisinfectionTime.Value.Month == i.Month);
                    //    var OKData = MonthData.Where(t => t.IsQualified == true);
                    //    EmplyeeByStaBisOutPut output = new EmplyeeByStaBisOutPut();
                    //    output.name = i.Year + "年" + i.Month + "月";
                    //    output.value = MonthData.Count() == 0 ? 0.00 : ValueHelper.ToRound(OKData.Count() * 1.00 / MonthData.Count() * 1.00).Value;
                    //    result.Add(output);
                    //}

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }



        /// <summary>
        /// 患者检测完成率
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetPatientInfectiousChecksOKStatisticsByMonthAsync(DisinfectionRoomStatisticsQueryInput input = null)
        {

            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<PatientInfectiousCheck> list = new List<PatientInfectiousCheck>();

                try
                {
                    Expression<Func<PatientInfectiousCheck, bool>> predicate = t => t.DataState == 1;
                    if (input != null && input.CenterId + "" != "0" && input.CenterId + "" != "")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);

                    list = await PatientInfectiousCheckStore.Entities.Include(t => t.CenterDialysis).Where(predicate).ToListAsync();


                    //if (list.Count <= 0)
                    //    return result;
                    var center = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "新入患者", "检测人数" };
                    //  result.data = center.Select(t => t.DialysisName).ToList();
                    bool flag = false;
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "新入患者" : "检测人数";
                        for (DateTime i = DateTime.Now.AddMonths(-6); i < DateTime.Now; i = i.AddMonths(+1))
                        {
                            if (!flag)
                                result.data.Add(i.Year + "年" + i.Month + "月");

                            int count = 0;
                            if (state)
                                count = list.Where(t => t.GoInTime.Year == i.Year && t.GoInTime.Month == i.Month).Count();
                            else
                                count = list.Where(t => t.GoInTime.Year == i.Year && t.GoInTime.Month == i.Month && t.DisinfectionTime.HasValue).Count();
                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);
                        flag = true;
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
        /// 患者检测完成率
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetPatientInfectiousChecksOkStatisticsByCenterAsync(DateTime dateTime)
        {

            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<PatientInfectiousCheck> list = new List<PatientInfectiousCheck>();

                try
                {
                    Expression<Func<PatientInfectiousCheck, bool>> predicate = t => t.DataState == 1;
                    predicate = predicate.And(t => t.GoInTime.Year == dateTime.Year && t.GoInTime.Month == dateTime.Month);

                    list = await PatientInfectiousCheckStore.Entities.Include(t => t.CenterDialysis).Where(predicate).ToListAsync();


                    //if (list.Count <= 0)
                    //    return result;
                    var center = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "新入人数", "检测人数" };
                    result.data = center.Select(t => t.DialysisName).ToList();
                    bool flag = false;
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "新入人数" : "检测人数";
                        foreach (var item in center)
                        {
                            //if (!flag)
                            //    result.data.Add(i.Year + "年" + i.Month + "月");

                            int count = 0;
                            if (state)
                                count = list.Where(t => t.CenterId == item.Id).Count();
                            else
                                count = list.Where(t => t.CenterId == item.Id && t.DisinfectionTime.HasValue).Count();
                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);
                        flag = true;
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }


        //标本菌落数 内毒素 合格统计

        public Task<LineOutPut> GetInspectionWaterPollutionsStatisticsByMonthAsync(DisinfectionRoomStatisticsQueryInput input = null)
        {

            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<InspectionWaterPollution> list = new List<InspectionWaterPollution>();

                try
                {
                    Expression<Func<InspectionWaterPollution, bool>> predicate = t => t.DataState == 1;
                    if (input != null && input.CenterId + "" != "" && input.CenterId + "" != "0")
                        predicate = predicate.And(t => t.CenterId == input.CenterId);

                    list = await InspectionWaterPollutionStore.Entities.Include(t => t.CenterDialysis).Where(predicate).ToListAsync();


                    //if (list.Count <= 0)
                    //    return result;
                    var center = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "检验次数", "合格次数" };
                    // result.data = center.Select(t => t.DialysisName).ToList();
                    bool flag = false;
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "检验次数" : "合格次数";
                        for (DateTime i = DateTime.Now.AddMonths(-6); i < DateTime.Now; i = i.AddMonths(+1))
                        {
                            if (!flag)
                                result.data.Add(i.Year + "年" + i.Month + "月");

                            int count = 0;
                            if (state)
                                count = list.Where(t => t.DisinfectionTime.Value.Year == i.Year && t.DisinfectionTime.Value.Month == i.Month).Count();
                            else
                                count = list.Where(t => t.DisinfectionTime.Value.Year == i.Year && t.DisinfectionTime.Value.Month == i.Month && t.IsQualified == true).Count();
                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);
                        flag = true;
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        public Task<LineOutPut> GetInspectionWaterPollutionsStatisticsByMonthAsync(DateTime dateTime)
        {

            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                List<InspectionWaterPollution> list = new List<InspectionWaterPollution>();

                try
                {
                    Expression<Func<InspectionWaterPollution, bool>> predicate = t => t.DataState == 1;

                    predicate = predicate.And(t => t.DisinfectionTime.Value.Year == dateTime.Year && t.DisinfectionTime.Value.Month == dateTime.Month);

                    list = await InspectionWaterPollutionStore.Entities.Include(t => t.CenterDialysis).Where(predicate).ToListAsync();


                    //if (list.Count <= 0)
                    //    return result;
                    var center = await DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    bool[] states = new bool[2] { true, false };
                    result.LegendData = new List<string>() { "检验次数", "合格次数" };
                    result.data = center.Select(t => t.DialysisName).ToList();
                    bool flag = false;
                    foreach (var state in states)
                    {
                        serise serise = new serise();
                        serise.name = state ? "检验次数" : "合格次数";
                        foreach (var data in center)
                        {
                            int count = 0;
                            if (state)
                                count = list.Where(t => t.CenterId == data.Id).Count();
                            else
                                count = list.Where(t => t.CenterId == data.Id && t.IsQualified == true).Count();
                            serise.data.Add(count);

                        }
                        result.serises.Add(serise);
                        flag = true;
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        #endregion

        #endregion
    }
}
