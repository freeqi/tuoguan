﻿using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CDGService.Data.Enums;
using CDGService.Data;
using Microsoft.Extensions.Options;
using CDGService.Store;
using System.Data.SqlClient;

namespace CDGService.WebAPI.DataCore
{
    public class PatientsManager : XmlSql
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly DictionaryCode _dictionaryCode;





        public PatientsManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<DictionaryCode> dictionaryCode)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _dictionaryCode = dictionaryCode.Value;

        }
        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<SystemDictionary> DictionaryStore => _unitOfWork.GetStore<SystemDictionary>();

        private IRepository<MedicalHistoryFirstPage> MedicalHistoryFirstPagesStore => _unitOfWork.GetStore<MedicalHistoryFirstPage>();
        private IRepository<FirstPageItemCurrentDiagnosis> FirstPageItemCurrentDiagnosissStore => _unitOfWork.GetStore<FirstPageItemCurrentDiagnosis>();
        private IRepository<VascularAccessRecord> VascularAccessRecordsStore => _unitOfWork.GetStore<VascularAccessRecord>();
        private IRepository<InfectiousDiseaseRegister> InfectiousDiseaseRegistersStore => _unitOfWork.GetStore<InfectiousDiseaseRegister>();
        private IRepository<AllergyRegister> AllergyRegistersStore => _unitOfWork.GetStore<AllergyRegister>();
        private IRepository<TumorRegister> TumorRegistersStore => _unitOfWork.GetStore<TumorRegister>();

        private IRepository<FirstOutpatientRecord> FirstOutpatientRecordStore => _unitOfWork.GetStore<FirstOutpatientRecord>();

        private IRepository<OutpatientMedicalRecord> OutpatientMedicalRecordStore => _unitOfWork.GetStore<OutpatientMedicalRecord>();
        private IRepository<DialysisAdequacyRecord> DialysisAdequacyRecordStore => _unitOfWork.GetStore<DialysisAdequacyRecord>();


        private IRepository<LaboratoryItemSetting> LaboratoryItemSettingStore => _unitOfWork.GetStore<LaboratoryItemSetting>();
        private IRepository<LaboratoryItemRecord> LaboratoryItemRecordStore => _unitOfWork.GetStore<LaboratoryItemRecord>();
        private IRepository<LaboratoryCategorySetting> LaboratoryCategorySettingStore => _unitOfWork.GetStore<LaboratoryCategorySetting>();






        #region 患者基本信息

        //获取指定时间段在院患者


        public Task<List<Patient>> GetzyPatientsQueryableAsync(DateTime StatrTime, DateTime EndTime, string CenterId, List<Patient> ALLPatient = null)
        {
            return Task.Run(async () =>
            {
                List<Patient> patients = new List<Patient>();

                if (ALLPatient == null || ALLPatient.Count <= 0)
                    ALLPatient = await PatientStore.Entities.Include(t => t.TransferRecords).Where(t => t.CenterId == CenterId).ToListAsync();
                ALLPatient = ALLPatient.Where(t => t.CenterId == CenterId).ToList();
                //var zy = ALLPatient.Where(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74").ToList();
                foreach (var item in ALLPatient)
                {
                    var TransferRecords = item.TransferRecords.OrderByDescending(t => t.TransferDate);
                    //temp 为0
                    var temp = TransferRecords.Where(t => t.TransferDate > StatrTime && t.TransferDate < EndTime && t.TransferType != "3f4eaafc1d134659bc323bd251041a74").Count();

                    //开始时间之前的最新一条转归应为在院
                    var tempw = TransferRecords.Where(t => t.TransferDate < StatrTime).FirstOrDefault() != null && TransferRecords.Where(t => t.TransferDate < StatrTime).FirstOrDefault().TransferType == "3f4eaafc1d134659bc323bd251041a74" || (TransferRecords.Where(t => t.TransferDate > StatrTime && t.TransferDate < EndTime).LastOrDefault() != null && TransferRecords.Where(t => t.TransferDate > StatrTime && t.TransferDate < EndTime).LastOrDefault().TransferType == "3f4eaafc1d134659bc323bd251041a74");

                    if (item.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && !(temp == 0 && tempw))
                    {

                    }
                    if (temp == 0 && tempw)
                    {
                        patients.Add(item);
                    }
                }
                //var zyP = ALLPatient.Where(t => t.TransferRecords.Where(t => t.TransferDate > StatrTime && t.TransferDate < EndTime && t.TransferType != "3f4eaafc1d134659bc323bd251041a74").Count() <= 0);

                return patients;
            });
        }


        //获取患者列表


        /// <summary>
        /// 获取患者列表, 
        /// </summary>
        public Task<PageData<PatientOutPut[]>> GetPatientsQueryableAsync(PatientQueryInPut input = null)
        {
            return Task.Run(async () =>
            {
                PatientOutPut[] result = null;
                int count = 0;
                Expression<Func<Patient, bool>> predicate = t => t.Id != "";
                if (input.CenterId + "" != "0" && input.CenterId + "" != "")
                    predicate = predicate.And(t => t.CenterId == input.CenterId);
                if (input.Name + "" != "")
                    predicate = predicate.And(t => t.Name.Contains(input.Name) || t.PatientNo.Contains(input.Name));
                if (input.id + "" != "")
                    predicate = predicate.And(t => t.Id == input.id);
                if (input.HospitalStateId + "" != "" && input.HospitalStateId + "" != "0")
                    predicate = predicate.And(t => t.HospitalState == input.HospitalStateId);
                try
                {
                    /*  
                     *  
                     *
                     */
                    var Sum = PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Include(t => t.SHospitalState).Include(t => t.SRHBloodType).Include(t => t.SSIInsuredType).Include(t => t.SABOBloodType).Include(t => t.SEducationBackground).Include(t => t.SMarital).Include(t => t.ChargeDoctorEmployee).Where(predicate).OrderBy(t => t.SHospitalState.ShowSortNo).OrderByDescending(t => t.ReceiveDate);
                    if (input.PageNum > 0 && input.PageSize > 0)
                    { 
                        var persons = await PaginatedList<Patient>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        result = Mapper.Map<PatientOutPut[]>(persons);
                        count = Sum.Count();
                    }
                    else
                    {
                        var datas = await Sum.ToArrayAsync();
                        result = Mapper.Map<PatientOutPut[]>(datas);
                        count = result.Length;

                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return new PageData<PatientOutPut[]>(result, count);
            });
        }


        //统计  职业、性别、年龄、地区、透析中心 医保类型
        //年龄段统计 20下、20-30、30-40、40-50、50-6、60+     
        /// <summary>
        /// 年龄统计 
        /// </summary>
        /// <param name="CenterId"></param>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetPatientByAgeQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {
                //  
                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {
                    Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74";
                    if (CenterId + "" != "0" && CenterId + "" != "")
                    {
                        predicate = predicate.And(t => t.CenterId == CenterId);
                    }
                    var patientsData = await PatientStore.Entities.Where(predicate).ToListAsync();
                    int k = 1;
                    foreach (Ages age in Enum.GetValues(typeof(Ages)))
                    {
                        int AgeNum = (int)age;
                        int count = 0;
                        if (k == 1)
                            count = patientsData.Where(t => t.Birthday.Value.Year >= DateTime.Now.Year - AgeNum).Count();
                        else if (k != 1 && AgeNum != 120)
                            count = patientsData.Where(t => t.Birthday.Value.Year > DateTime.Now.Year - AgeNum && t.Birthday.Value.Year <= DateTime.Now.Year + 10 - AgeNum).Count();
                        if (AgeNum == 120)
                        {
                            count = patientsData.Where(t => t.Birthday.Value.Year <= DateTime.Now.Year - 60).Count();
                        }
                        result.Add(new EmplyeeByStaBisOutPut() { name = age.ToChinese(), value = count });
                        k++;
                    }
                    //2021 01 29 23 20 59 +8 

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });

        }
        /// <summary>
        /// 性别统计
        /// </summary>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetPatientBySexQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.SexTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel && t.Sex == item.Name && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74";
                        if (CenterId + "" != "0" && CenterId + "" != "")
                        {
                            predicate = predicate.And(t => t.CenterId == CenterId);
                        }
                        int count = await PatientStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }

        /// <summary>
        /// 血源性疾病统计
        /// </summary>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetPatientByBloodBorneQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.BloodBorneDiseaseTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel && t.BloodBorneDisease == item.Id && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74";
                        if (CenterId + "" != "0" && CenterId + "" != "")
                        {
                            predicate = predicate.And(t => t.CenterId == CenterId);
                        }
                        int count = await PatientStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }

        //SIInsuredType
        /// <summary>
        /// 医保类型
        /// </summary>
        /// <param name="CenterId"></param>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetPatientBySIInsuredTypeQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.SIInsuredTypeID).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel && t.SIInsuredType == item.Id && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74";
                        if (CenterId + "" != "0" && CenterId + "" != "")
                        {
                            predicate = predicate.And(t => t.CenterId == CenterId);
                        }
                        int count = await PatientStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }




        /// <summary>
        /// 在院状态
        /// </summary>
        /// <param name="CenterId"></param>
        /// <returns></returns>
        public Task<EmplyeeByStaBisOutPut[]> GetPatientByHospitalStateQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {
                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.HospitalStateTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    foreach (var item in JobTitleData)
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel && t.HospitalState == item.Id;
                        if (CenterId + "" != "0" && CenterId + "" != "")
                        {
                            predicate = predicate.And(t => t.CenterId == CenterId);
                        }
                        int count = await PatientStore.Entities.Where(predicate).CountAsync();
                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }



        // 近两年（季度）增长统计
        public Task<LineOutPut> GetPatientByDateQueryableAsync(string CenterId, int Year)
        {
            //1年按月份
            //2年按季度
            //2年以上按年份
            //data['1月','2月','3月',……] data['2017Q1','2017Q1','2017Q1',……]  data['2016年','2017年','2018年',……] 
            return Task.Run(async () =>
            {
                try
                {
                    bool IsDel = false;
                    LineOutPut result = new LineOutPut();

                    List<PatientStatis> ListPatientStatis = new List<PatientStatis>();

                    //&& t.CenterDialysisId == CenterId  && t.ReceiveDate >= DateTime.Now.AddYears(-Year )
                    Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                    if (CenterId + "" != "0" && CenterId + "" != "")
                        predicate = predicate.And(t => t.CenterId == CenterId);
                    var CenterPData = await PatientStore.Entities.Where(predicate).ToListAsync();
                    //0


                    //   DateTime BeginTime = CenterPData.Min(t => t.ReceiveDate.Value);
                    result.LegendData.Add("男性患者");
                    result.LegendData.Add("女性患者");
                    result.LegendData.Add("患者总数");
                    List<string> XA = new List<string>();
                    List<double> manCount = new List<double>();
                    List<double> womanCount = new List<double>();
                    List<double> SumCount = new List<double>();

                    if (Year <= 1)
                    {
                        for (DateTime i = DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1); i < DateTime.Now; i = i.AddMonths(+1))
                        {
                            //&& t.ReceiveDate.Value.Date >= DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1).Date
                            //&& t.ReceiveDate.Value.Date >= DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1).Date
                            int ManCount = CenterPData.Where(t => t.Sex == "男" && t.ReceiveDate?.Date <= i.AddMonths(+1).Date).Count();
                            int WomanCount = CenterPData.Where(t => t.Sex == "女" && t.ReceiveDate?.Date <= i.AddMonths(+1).Date).Count();
                            XA.Add(i.Year + "年" + i.Month + "月");
                            //result.data.Add(i.Month + "月");
                            manCount.Add(ManCount);
                            womanCount.Add(WomanCount);
                            SumCount.Add(ManCount + WomanCount);
                        }
                    }
                    else
                    {
                        int k = 1;
                        for (DateTime i = DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1); i < DateTime.Now; i = i.AddMonths(+3))
                        {
                            //&& t.ReceiveDate.Value.Date >= DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1).Date
                            //&& t.ReceiveDate.Value.Date >= DateTime.Now.AddYears(-Year).AddMonths(-DateTime.Now.Month + 1).Date
                            int ManCount = CenterPData.Where(t => t.Sex == "男" && t.ReceiveDate?.Date < i.AddMonths(+3).Date).Count();
                            int WomanCount = CenterPData.Where(t => t.Sex == "女" && t.ReceiveDate?.Date < i.AddMonths(+3).Date).Count();
                            XA.Add(i.Year + "Q" + k);
                            //result.data.Add(i.Month + "月");
                            manCount.Add(ManCount);
                            womanCount.Add(WomanCount);
                            SumCount.Add(ManCount + WomanCount);
                            k++;
                            k = k == 5 ? 1 : k;
                        }
                    }


                    result.data = XA;
                    result.serises.Add(new serise() { name = "男性患者", data = manCount });
                    result.serises.Add(new serise() { name = "女性患者", data = womanCount });
                    result.serises.Add(new serise() { name = "患者总数", data = SumCount });
                    return result;
                }
                catch (Exception exp)
                {
                    throw new Exception("未获取到数据", exp);
                }

            });
        }
        #endregion

        #region  病历
        /// <summary>
        /// 门诊病历首页
        /// </summary>
        /// <param name="PatientID"></param>
        /// <returns></returns>
        public Task<MedicalRecordsOutPut> GetMedicalRecords(string PatientID)
        {
            return Task.Run(async () =>
            {
                try
                {
                    // bool IsDel = false;
                    MedicalRecordsOutPut result = new MedicalRecordsOutPut();
                    var Patientdata = await PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Include(t => t.SHospitalState).Include(t => t.SRHBloodType).Include(t => t.SSIInsuredType).Include(t => t.SABOBloodType).Include(t => t.SEducationBackground).Include(t => t.SMarital).Where(t => t.Id == PatientID).FirstOrDefaultAsync();
                    var MedicalHistory = await MedicalHistoryFirstPagesStore.Entities.Where(t => t.PatientId == PatientID).FirstOrDefaultAsync();
                    List<FirstPageItemCurrentDiagnosis> firstPageItemCurrentDiagnosis = new List<FirstPageItemCurrentDiagnosis>();
                    if (MedicalHistory != null)
                        //子项
                        firstPageItemCurrentDiagnosis = await FirstPageItemCurrentDiagnosissStore.Entities.Where(t => t.MedicalHistoryFirstPageId == MedicalHistory.Id).ToListAsync();

                    var VascularAccessRecords = await VascularAccessRecordsStore.Entities.Where(t => t.PatientId == PatientID).ToListAsync();
                    var InfectiousDiseaseRegisters = await InfectiousDiseaseRegistersStore.Entities.Where(t => t.PatientId == PatientID).ToListAsync();
                    var AllergyRegisters = await AllergyRegistersStore.Entities.Where(t => t.PatientId == PatientID).ToListAsync();
                    var TumorRegisters = await TumorRegistersStore.Entities.Where(t => t.PatientId == PatientID).ToListAsync();

                    result.medicalHistoryFirst = Mapper.Map<MedicalHistoryFirstOutPut>(MedicalHistory);
                    result.firstPageItemCurrentDiagnosis = Mapper.Map<FirstPageItemCurrentDiagnosisOutPut[]>(firstPageItemCurrentDiagnosis);
                    result.vascularAccessRecord = Mapper.Map<VascularAccessRecordOutPut[]>(VascularAccessRecords);
                    result.infectiousDiseaseRegister = Mapper.Map<InfectiousDiseaseRegisterOutPut[]>(InfectiousDiseaseRegisters);
                    result.allergyRegister = Mapper.Map<AllergyRegisterOutPut[]>(AllergyRegisters);
                    result.tumorRegister = Mapper.Map<TumorRegisterOutPut[]>(TumorRegisters);
                    result.HomePageInfo = Mapper.Map<PatientOutPut>(Patientdata);
                    //result.medicalHistoryFirst = FirstPageItemCurrentDiagnosiss;


                    return result;
                }
                catch (Exception exp)
                {
                    throw new Exception("未获取到数据", exp);
                }

            });
        }

        //专用病历

        /// <summary>
        /// 门诊病历首页
        /// </summary>
        /// <param name="PatientID"></param>
        /// <returns></returns>
        public Task<OutpatientRecordOutPut> GetFirstOutpatientRecord(string PatientID)
        {
            return Task.Run(async () =>
            {
                try
                {
                    OutpatientRecordOutPut outpatientRecordOutPut = new OutpatientRecordOutPut();
                    // bool IsDel = false;
                    FirstOutpatientRecord result = new FirstOutpatientRecord();
                    var Patientdata = await PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Include(t => t.SHospitalState).Include(t => t.SRHBloodType).Include(t => t.SSIInsuredType).Include(t => t.SABOBloodType).Include(t => t.SEducationBackground).Include(t => t.SMarital).Where(t => t.Id == PatientID).FirstOrDefaultAsync();
                    var data = await FirstOutpatientRecordStore.Entities.Where(t => t.PatientId == PatientID).FirstOrDefaultAsync();
                    outpatientRecordOutPut.firstOutpatientRecordOutPut = Mapper.Map<FirstOutpatientRecordOutPut>(data);
                    outpatientRecordOutPut.HomePageInfo = Mapper.Map<PatientOutPut>(Patientdata);

                    return outpatientRecordOutPut;
                }
                catch (Exception exp)
                {
                    throw new Exception("未获取到数据", exp);
                }

            });
        }

        //门诊病例


        public Task<POutpatientMedicalRecordOutPut> GetOutpatientMedicalRecord(string PatientID)
        {
            return Task.Run(async () =>
            {
                try
                {

                    POutpatientMedicalRecordOutPut outpatientRecordOutPut = new POutpatientMedicalRecordOutPut();
                    // bool IsDel = false;
                    FirstOutpatientRecord result = new FirstOutpatientRecord();
                    var Patientdata = await PatientStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Include(t => t.SHospitalState).Include(t => t.SRHBloodType).Include(t => t.SSIInsuredType).Include(t => t.SABOBloodType).Include(t => t.SEducationBackground).Include(t => t.SMarital).Where(t => t.Id == PatientID).FirstOrDefaultAsync();
                    var data = await OutpatientMedicalRecordStore.Entities.Where(t => t.PatientId == PatientID).ToArrayAsync();
                    outpatientRecordOutPut.outpatientMedicalRecord = Mapper.Map<OutpatientMedicalRecordOutPut[]>(data);
                    outpatientRecordOutPut.HomePageInfo = Mapper.Map<PatientOutPut>(Patientdata);
                    data.OrderByDescending(t => t.TreatmentDate).Select(t => t.TreatmentDate.Value).ToList().ForEach(t => outpatientRecordOutPut.DateList.Add(t.ToString("yyyy-MM-dd")));
                    return outpatientRecordOutPut;
                }
                catch (Exception exp)
                {
                    throw new Exception("未获取到数据", exp);
                }

            });
        }


        #endregion

        #region  透析充分性-- 患者检查报告

        public Task<DialysisAdequacyRecord[]> GetDialysisAdequacyRecord(DialysisAdequacyQueryInPut input)
        {


            return Task.Run(async () =>
            {
                DialysisAdequacyRecord[] result = null;
                int count = 0;
                Expression<Func<DialysisAdequacyRecord, bool>> predicate = t => t.Id != ""  && t.DataState == 1;
                if (input.CenterId + "" != "0" && input.CenterId + "" != "")
                    predicate = predicate.And(t => t.CenterId == input.CenterId);
                if (input.DialysisType + "" != "")
                    predicate = predicate.And(t => t.DialysisType.Contains(input.DialysisType));
                if (input.Dialyzer + "" != "")
                    predicate = predicate.And(t => t.Dialyzer.Contains(input.Dialyzer));
                if (input.PatientId + "" != "")
                    predicate = predicate.And(t => t.PatientId == input.PatientId);

                if (input.BeginRecordDate.HasValue && input.EndRecordDate.HasValue)
                    predicate = predicate.And(t => t.RecordDate >= input.BeginRecordDate && t.RecordDate < input.EndRecordDate);
                else
                    predicate = predicate.And(t => t.RecordDate >= DateTime.Now.AddMonths(-4));
                try
                {
                    var Sum = await DialysisAdequacyRecordStore.Entities.Include(t => t.PatientDar).Include(t => t.centerDar).Where(predicate).OrderBy(t => t.PatientId).OrderByDescending(t => t.RecordDate).ToArrayAsync();

                    return Sum;
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });

        }


        /// <summary>
        /// 获取所有检验项目分类配置
        /// </summary>     
        /// <returns></returns>
        public Task<LaboratoryCategorySetting[]> GetAllLaboratoryCategorySetting()

        {
            return Task.Run(async () =>
            {

                var sbs = await LaboratoryCategorySettingStore.Entities.Where(t => t.DataState == 1).OrderBy(t => t.Sort).ToListAsync();
                return sbs.ToArray();

            });

        }



        /// <summary>
        /// 获取所有检验项目分类配置
        /// </summary>     
        /// <returns></returns>
        public Task<LaboratoryItemSetting[]> GetLaboratoryItemSettingByCategoryId(string categoryId)

        {
            return Task.Run(async () =>
            {

                var sbs = await LaboratoryItemSettingStore.Entities.Where(t => t.DataState == 1 && t.CategoryId.Contains(categoryId)).OrderBy(t => t.Sort).ToListAsync();
                return sbs.ToArray();

            });

        }


        //PatientInspQueryInPut

        public Task<List<Dictionary<string, string>>> GetInspResultRecordsByPatientIdAndCheckType(PatientInspQueryInPut inPut)
        {


            return Task.Run(async () =>
               {
                   List<Dictionary<string, string>> returnResult = new List<Dictionary<string, string>>();

                   if (!inPut.StartTime.HasValue)
                       inPut.StartTime = DateTime.Now.AddYears(-2);
                   if (!inPut.EndTime.HasValue)
                       inPut.EndTime = DateTime.Now.AddDays(+1);
                   Expression<Func<LaboratoryItemRecord, bool>> predicate = t => t.itemSetting.CategoryId.Contains(inPut.categoryId) && t.DataState == 1 && t.CheckDate >= inPut.StartTime && t.CheckDate <= inPut.EndTime;
                   if (inPut.CenterId + "" != "0" && inPut.CenterId + "" != "")
                       predicate = predicate.And(t => t.CenterId == inPut.CenterId);
                   if (inPut.patientId + "" != "")
                       predicate = predicate.And(t => t.PatientId == inPut.patientId);

                   var labdata = await LaboratoryItemRecordStore.Entities.Include(t => t.Patient).Where(predicate).OrderByDescending(t => t.CheckDate).ToListAsync();

                   var groupPatientId = labdata.GroupBy(g => g.PatientId).ToList();
                   foreach (var it in groupPatientId)
                   {
                       var newIt = it.ToList();
                       string patientName = newIt[0].Patient.Name;
                       string id = newIt[0].PatientId;

                       var groupReportNo = newIt.GroupBy(g => new { g.ReportNo, g.CheckDate }).ToList();

                       foreach (var item in groupReportNo)
                       {
                           Dictionary<string, string> dic = new Dictionary<string, string>();
                           dic.Add("ReportNo", item.Key.ReportNo);
                           dic.Add("PatientName", patientName);
                           dic.Add("PatientId", id);
                           dic.Add("CheckDate", item.Key.CheckDate != null ? item.Key.CheckDate.Value.ToString("yyyy-MM-dd") : "");
                           var res = newIt.Where(w => w.ReportNo.Equals(item.Key.ReportNo)).ToList();
                           // 

                           #region 移除多余数据
                           for (int i = 0; i < res.Count(); i++)
                           {
                               var aa = res.Where(w => w.InspectionItemId.Equals(res[i].InspectionItemId)).OrderByDescending(o => o.CheckDate).ToList();
                               for (int j = 1; j < aa.Count(); j++)
                               {
                                   res.Remove(aa[j]);

                                   Random rd = new Random();
                                   string newReportNo = $"{item.Key.ReportNo}{rd.Next(1, 9)}";
                                   var newarry = new Dictionary<string, string>();
                                   newarry.Add("ReportNo", newReportNo);
                                   newarry.Add("PatientName", patientName);
                                   newarry.Add("CheckDate", item.Key.CheckDate != null ? item.Key.CheckDate.Value.ToString("yyyy-MM-dd") : "");
                                   newarry.Add(aa[j].InspectionItemId, aa[j].Value);
                                   returnResult.Add(newarry);
                               }
                           }
                           #endregion

                           foreach (var itt in res)
                           {
                               dic.Add(itt.InspectionItemId, itt.Value);
                           }

                           returnResult.Add(dic);
                       }
                   }

                   return returnResult;
               });


        }


        /// <summary>
        /// 查询感控数据项目名称列表
        /// </summary>      
        /// <returns></returns>
        public List<SensingDataProjectName> GetSensingDataGroupByProjectName()
        {
            var list = new List<SensingDataProjectName>();
            var sqlParameterList = new List<SqlParameter>();
            string QuerySQL = $"select ProjectName  from SensingData  where datastate = 1  group by ProjectName ";
            var projectdt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());

            var result = GetTupleByList<SensingDataProjectName>(projectdt);

            if (result.Item1)
                list = result.Item2;
            return list;
        }


        public List<Dictionary<string, string>> GetSensingDataByProjectName(SensingDataQueryInPut inPut)
        {
            var list = new List<Tuple<string, SqlParameter[]>>();
            var addList = new List<Tuple<string, SqlParameter[]>>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql = " select SensingData.*, CenterDialysiss.ShortName  as CenterName  from SensingData left join CenterDialysiss on SensingData.centerid = CenterDialysiss.Id where SensingData.datastate = 1  ";

            if (!inPut.StartTime.HasValue || !inPut.EndTime.HasValue)
            {
                inPut.StartTime = DateTime.Now.AddYears(-2);
                inPut.EndTime = DateTime.Now.AddDays(1);
            }

            sql += " AND (CheckDate>= @StartTime and CheckDate< @EndTime )"; 
            parameters.Add(new SqlParameter("@StartTime", inPut.StartTime));
            parameters.Add(new SqlParameter("@EndTime", inPut.EndTime));

            if (!string.IsNullOrWhiteSpace(inPut.ProjectName))
            {
                sql += $" AND ProjectName=@ProjectName";
                parameters.Add(new SqlParameter("@ProjectName", inPut.ProjectName));
            }
            if (!string.IsNullOrWhiteSpace(inPut.CenterId))
            {
                sql += $" AND CenterId=@CenterId";
                parameters.Add(new SqlParameter("@CenterId", inPut.CenterId));
            }
            // 
            var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, parameters.ToArray());
            var result = GetTupleByList<SensingDataOutPut>(dt);

            List<Dictionary<string, string>> returnResult = new List<Dictionary<string, string>>();

            if (result.Item1)
            {
                var groupPatientId = result.Item2.GroupBy(g => g.Region).ToList(); 
                foreach (var it in groupPatientId)
                {
                    var newIt = it.ToList();
                    var groupReportNo = newIt.GroupBy(g => new { g.ReportNo, g.CheckDate, g.CenterName }).OrderByDescending(o => o.Key.CheckDate).ToList();
                    foreach (var item in groupReportNo)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();

                        dic.Add("CenterName", item.Key.CenterName);
                        dic.Add("Region", it.Key);
                     
                        dic.Add("ReportNo", item.Key.ReportNo);
                        dic.Add("CheckDate", item.Key.CheckDate != null ? item.Key.CheckDate.Value.ToString("yyyy-MM-dd") : "");
                        var res = newIt.Where(w => w.ReportNo.Equals(item.Key.ReportNo)).ToList();

                        #region 移除多余数据
                        for (int i = 0; i < res.Count(); i++)
                        {
                            var aa = res.Where(w => w.ItemDetail.Equals(res[i].ItemDetail)).OrderByDescending(o => o.CheckDate).ToList();
                            for (int j = 1; j < aa.Count(); j++)
                            {
                                res.Remove(aa[j]);

                                Random rd = new Random();
                                string newReportNo = $"{item.Key.ReportNo}{rd.Next(1, 9)}";

                                var newarry = new Dictionary<string, string>();
                                dic.Add("CenterName", item.Key.CenterName);
                                newarry.Add("Region", it.Key);
                                newarry.Add("ReportNo", newReportNo);
                                newarry.Add("CheckDate", item.Key.CheckDate != null ? item.Key.CheckDate.Value.ToString("yyyy-MM-dd") : "");
                                newarry.Add(aa[j].ItemDetail, aa[j].Value);
                                returnResult.Add(newarry); 
                            }
                        }
                        #endregion

                        foreach (var itt in res)
                        {
                            dic.Add(itt.ItemDetail, itt.Value);
                        }

                        returnResult.Add(dic);
                    }
                }
            }

            return returnResult.OrderByDescending(a => a["CheckDate"]).ThenBy(o => o["Region"]).ToList();

        }


        #endregion 
    }
}
