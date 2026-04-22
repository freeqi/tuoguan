﻿using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using CDGService.Utils;
using CDGService.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CDGService.Data.Enums;
using CDGService.Data;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 医疗质量和安全管理指标统计
    /// </summary>
    public class MedicalIndexesManger
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        public MedicalIndexesManger(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;

        }
        private IRepository<MedicalIndexesMonth> MaintenanceRecordStore => _unitOfWork.GetStore<MedicalIndexesMonth>();
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<MedicalIndexesYear> MedicalIndexesYearStore => _unitOfWork.GetStore<MedicalIndexesYear>();

        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();



        //201904 201903 -- 环比
        //201904 201804 -- 同比

        #region -月度指标
        /*
         1)	月度观察指标（例）
         2)	患者人数（人）
         3)	转归人数（人）
         4)	月度统计指标
         */
        //月度观察指标:(单个透析中心)
        public Task<MedicalIndexesMonthOutPut[]> ObserveIndicatorsAsync(MedicalIndexesMonthQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                MedicalIndexesMonthOutPut[] result = null;
                try
                {
                    int DataSatte = 1;
                    Expression<Func<MedicalIndexesMonth, bool>> predicate = t => t.DataState == DataSatte;
                    predicate = predicate.And(t => t.CenterId == inPut.CenterId);
                    predicate = predicate.And(t => t.StatisticalType == inPut.medicalStatisticalType);
                    predicate = predicate.And(t => t.FounderDate.Value.ToString("yyyy-MM") == inPut.QueryDateTime.ToString("yyyy-MM"));
                    var data = await MaintenanceRecordStore.Entities.Where(predicate).OrderBy(t => t.Indicators).ToArrayAsync();
                    result = Mapper.Map<MedicalIndexesMonthOutPut[]>(data);

                    //死亡名单
                    if (inPut.medicalStatisticalType == MedicalStatisticalType.Regression)
                    {
                        List<Tag> tags = new List<Tag>();
                        tags.Add(new Tag() { Age = 15, Name = "test1", TagTime = "2004-12-01" });
                        tags.Add(new Tag() { Age = 15, Name = "test2", TagTime = "2004-12-01" });
                        tags.Add(new Tag() { Age = 15, Name = "test3", TagTime = "2004-12-01" });
                        tags.Add(new Tag() { Age = 15, Name = "test1", TagTime = "2004-12-01" });
                        tags.Add(new Tag() { Age = 15, Name = "test2", TagTime = "2004-12-01" });
                        tags.Add(new Tag() { Age = 15, Name = "test3", TagTime = "2004-12-01" });
                        var patientData = await PatientStore.Entities.Include(t => t.TransferRecords).Where(t => t.HospitalState == "feeb7b938b43489a8ff4dedfa0f9dde9" && t.CenterId == inPut.CenterId).ToListAsync();
                        foreach (var item in patientData)
                        {
                            var flag = item.TransferRecords.Where(t => t.TransferDate.Value.ToString("yyyy-MM") == inPut.QueryDateTime.ToString("yyyy-MM")).FirstOrDefault();
                            if (flag != null)
                                tags.Add(new Tag() { Id = item.Id, Name = item.Name, Age = flag.TransferDate.Value.Year - item.Birthday.Value.Year, TagTime = flag.TransferDate.Value.ToShortDateString() });
                        }
                        if (result.Count() > 0 && result.Where(t => t.Indicators == MedicalIndicatorsMonth.DeathPatients).Count() > 0)
                            result.Where(t => t.Indicators == MedicalIndicatorsMonth.DeathPatients).FirstOrDefault().tags = tags;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return result;

            });
        }


        //全部透析中心  KeyValue
        /// <summary>
        /// 月度指标
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public List<EnumberEntity> EnumberEntityAsync(MedicalStatisticalType inPut)
        {
            List<EnumberEntity> data = new List<EnumberEntity>();
            // inPut.ToChinese
            switch (inPut)
            {

                case MedicalStatisticalType.ObserveIndicators:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(6);
                    break;
                case MedicalStatisticalType.Patients:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(4, 6);
                    break;
                case MedicalStatisticalType.Regression:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(6, 10);
                    break;
                case MedicalStatisticalType.StatisticalIndicators:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(5, 16);
                    break;
                default:
                    break;
            }
            return data;

        }
        /// <summary>
        /// 月度观察指标:(所有透析中心单指标)
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<AllCenterMedicalIndexesMonthOutPut[]> ObserveIndicatorsAsync(ALLCenterMedicalIndexesMonthQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                AllCenterMedicalIndexesMonthOutPut[] result = null;

                try
                {
                    //透析中心
                    // var CenterData = DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();

                    int DataSatte = 1;
                    Expression<Func<MedicalIndexesMonth, bool>> predicate = t => t.DataState == DataSatte;
                    predicate = predicate.And(t => t.Indicators == inPut.medicalIndicatorsMonth);
                    //   predicate = predicate.And(t => t.StatisticalType == inPut.medicalStatisticalType);
                    predicate = predicate.And(t => t.FounderDate.Value.ToString("yyyy-MM") == inPut.QueryDateTime.ToString("yyyy-MM"));
                    var data = await MaintenanceRecordStore.Entities.Include(t => t.centerDialysis).Where(predicate).OrderBy(t => t.centerDialysis.SortNnm).ToArrayAsync();
                    result = Mapper.Map<AllCenterMedicalIndexesMonthOutPut[]>(data);
                    //死亡名单
                    if (inPut.medicalIndicatorsMonth == MedicalIndicatorsMonth.DeathPatients)
                    {

                        var patientData = await PatientStore.Entities.Include(t => t.TransferRecords).Where(t => t.HospitalState == "feeb7b938b43489a8ff4dedfa0f9dde9").ToListAsync();
                        foreach (var itema in result)
                        {
                            List<Tag> tags = new List<Tag>();
                            foreach (var item in patientData)
                            {
                                var flag = item.TransferRecords.Where(t => t.TransferDate.Value.ToString("yyyy-MM") == inPut.QueryDateTime.ToString("yyyy-MM") && item.CenterId == itema.CenterId).FirstOrDefault();
                                if (flag != null)
                                    tags.Add(new Tag() { Id = item.Id, Name = item.Name, Age = flag.TransferDate.Value.Year - item.Birthday.Value.Year, TagTime = flag.TransferDate.Value.ToShortDateString() });
                            }
                            if (result.Count() > 0 && result.Where(t => t.Indicators == MedicalIndicatorsMonth.DeathPatients).Count() > 0)
                                itema.tags = tags;
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




        #endregion

        #region -年度指标

        /*MedicalIndexesYearStore  MedicalIndexesYearOutPut
            1)	年度观察指标
            2)	感染检测
            3)	血管通路类别
            4)	年度统计指标         
         */

        /// <summary>
        /// 年度指标
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public List<EnumberEntity> EnumberEntityYearAsync(MedicalStatisticalYearType inPut)
        {
            List<EnumberEntity> data = new List<EnumberEntity>();
            // inPut.ToChinese
            switch (inPut)
            {


                case MedicalStatisticalYearType.ObserveIndicators:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(11);
                    break;
                case MedicalStatisticalYearType.Infection:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(3, 11);
                    break;
                case MedicalStatisticalYearType.VascularAccess:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(5, 14);
                    break;
                case MedicalStatisticalYearType.StatisticalIndicators:
                    data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(10, 19);
                    break;
                default:
                    break;
            }
            return data;

        }


        public Task<MedicalIndexesYearOutPut[]> ObserveIndicatorsYearAsync(MedicalIndexesYearQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                MedicalIndexesYearOutPut[] result = null;
                try
                {
                    int DataSatte = 1;
                    Expression<Func<MedicalIndexesYear, bool>> predicate = t => t.DataState == DataSatte;
                    predicate = predicate.And(t => t.CenterId == inPut.CenterId);
                    predicate = predicate.And(t => t.StatisticalType == inPut.medicalStatisticalType);
                    predicate = predicate.And(t => t.FounderDate.Value.ToString("yyyy") == inPut.QueryDateTime.ToString("yyyy"));
                    var data = await MedicalIndexesYearStore.Entities.Where(predicate).OrderBy(t => t.Indicators).ToArrayAsync();
                    //死亡名单
                    if (inPut.medicalStatisticalType == MedicalStatisticalYearType.ObserveIndicators)
                    {
                        List<Tag> tags = new List<Tag>();
                        var patientData = await PatientStore.Entities.Include(t => t.TransferRecords).Where(t => t.HospitalState == "feeb7b938b43489a8ff4dedfa0f9dde9" && t.CenterId == inPut.CenterId).ToListAsync();
                        foreach (var item in patientData)
                        {
                            if (item.ModifierDate.Value.Year == inPut.QueryDateTime.Year)
                                tags.Add(new Tag() { Id = item.Id, Name = item.Name, Age = item.ModifierDate.Value.Year - item.Birthday.Value.Year, TagTime = item.ModifierDate.Value.ToShortDateString() });
                            if (item.TransferRecords != null && item.TransferRecords.Count > 0)
                            {
                                var flag = item.TransferRecords.Where(t => t.TransferDate != null && t.TransferDate.Value.ToString("yyyy") == inPut.QueryDateTime.ToString("yyyy")).FirstOrDefault();
                                if (flag != null)
                                    tags.Add(new Tag() { Id = item.Id, Name = item.Name, Age = flag.TransferDate.Value.Year - item.Birthday.Value.Year, TagTime = flag.TransferDate.Value.ToShortDateString() });
                            }
                        }
                        result = Mapper.Map<MedicalIndexesYearOutPut[]>(data);
                        if (result.Count() > 0 && result.Where(t => t.Indicators == MedicalIndicatorsYear.SWZLS).Count() > 0)
                            result.Where(t => t.Indicators == MedicalIndicatorsYear.SWZLS).FirstOrDefault().tags = tags;
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
        /// 年度观察指标:(所有透析中心单指标) 
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<AllCenterMedicalIndexesYearOutPut[]> ObserveIndicatorsYearAsync(ALLCenterMedicalIndexesYearQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                AllCenterMedicalIndexesYearOutPut[] result = null;

                try
                {
                    //透析中心 
                    // var CenterData = DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();

                    int DataSatte = 1;
                    Expression<Func<MedicalIndexesYear, bool>> predicate = t => t.DataState == DataSatte;
                    predicate = predicate.And(t => t.Indicators == inPut.medicalIndicatorsYear);
                    //   predicate = predicate.And(t => t.StatisticalType == inPut.medicalStatisticalType);
                    predicate = predicate.And(t => t.FounderDate.Value.ToString("yyyy") == inPut.QueryDateTime.ToString("yyyy"));
                    var data = await MedicalIndexesYearStore.Entities.Include(t => t.centerDialysis).Where(predicate).OrderBy(t => t.centerDialysis.SortNnm).ToArrayAsync();
                    result = Mapper.Map<AllCenterMedicalIndexesYearOutPut[]>(data);
                    //死亡名单
                    if (inPut.medicalIndicatorsYear == MedicalIndicatorsYear.SWZLS)
                    {
                        var patientData = await PatientStore.Entities.Include(t => t.TransferRecords).Where(t => t.HospitalState == "feeb7b938b43489a8ff4dedfa0f9dde9").ToListAsync();
                        foreach (var itema in result)
                        {
                            List<Tag> tags = new List<Tag>();
                            foreach (var item in patientData)
                            {
                                var flag = item.TransferRecords.Where(t => t.TransferDate.Value.ToString("yyyy") == inPut.QueryDateTime.ToString("yyyy") && item.CenterId == itema.CenterId).FirstOrDefault();
                                if (flag != null)
                                    tags.Add(new Tag() { Id = item.Id, Name = item.Name, Age = flag.TransferDate.Value.Year - item.Birthday.Value.Year, TagTime = flag.TransferDate.Value.ToShortDateString() });
                            }
                            if (result.Count() > 0 && result.Where(t => t.Indicators == MedicalIndicatorsYear.SWZLS).Count() > 0)
                                itema.tags = tags;
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

        #endregion
        private int ReturnTest()
        {
            //double num = 0.80;

            var rd = new Random();
            return rd.Next(45, 120);
        }
        //插入测试数据
        public void AddData()
        {

            var CenterData = DialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
            List<EnumberEntity> data = new List<EnumberEntity>();
            foreach (var Center in CenterData.Result)
            {
                foreach (MedicalStatisticalYearType item in Enum.GetValues(typeof(MedicalStatisticalYearType)))
                {
                    switch (item)
                    {

                        case MedicalStatisticalYearType.ObserveIndicators:
                            data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(11);

                            break;
                        case MedicalStatisticalYearType.Infection:
                            data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(3, 11);
                            break;
                        case MedicalStatisticalYearType.VascularAccess:
                            data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(5, 14);
                            break;
                        case MedicalStatisticalYearType.StatisticalIndicators:
                            data = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(10, 19);
                            break;
                        default:
                            break;
                    }
                    foreach (var items in data)
                    {
                        int C = ReturnTest();
                        int L = ReturnTest();
                        MedicalIndexesYear medicalIndexesMonth = new MedicalIndexesYear()
                        {
                            Id = Guid.NewGuid().tostring32(),
                            CurrentYear = C,
                            DataState = 1,
                            Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                            FounderDate = DateTime.Now.AddYears(-1),
                            Indicators = (MedicalIndicatorsYear)items.EnumValue,
                            LastYear = L,
                            Remarks = "2019-04添加测试数据",
                            StatisticalType = item,
                            CenterId = Center.Id,
                        };
                        MedicalIndexesYearStore.Insert(medicalIndexesMonth);
                    }
                }

                _unitOfWork.SaveChanges();
            }




        }
    }
}
