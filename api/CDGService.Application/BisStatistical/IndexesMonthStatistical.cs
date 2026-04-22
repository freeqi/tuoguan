using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Helper;
using CDGService.Data.Store;
using CDGService.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Application.BisStatistical
{
    /// <summary>
    /// 系统自动统计月度指标
    /// </summary>
    public class IndexesMonthStatistical
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IGetUserInfo _getUserInfo;


        public IndexesMonthStatistical(IUnitOfWork unitOfWork, IGetUserInfo getUserInfo)
        {
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
        }


        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<MedicalIndexesMonth> MedicalIndexesMonthStore => _unitOfWork.GetStore<MedicalIndexesMonth>();
        private IRepository<HistoryDialysisRecords> HistoryDialysisRecordsStore => _unitOfWork.GetStore<HistoryDialysisRecords>();
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();

        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();

        private IRepository<RoutineBloodRecord> RoutineBloodRecordStore => _unitOfWork.GetStore<RoutineBloodRecord>();
        private IRepository<TransferRecord> TransferRecordStore => _unitOfWork.GetStore<TransferRecord>();

        DateTime dtNow = DateTime.Now;
        DateTime UpdtNow = DateTime.Now.AddMonths(-1);

        public Task<bool> StartObserveAnalyze()
        {
            return Task.Run(async () =>
            {
                await Observe();
                await PatientsObserve();
                await TransferRecordObserve();
                await MonthStatisticalObserve();
                return true;
            });
        }


        /// <summary>
        /// 月度观察指标
        /// </summary>
        /// <returns></returns>
        private Task<bool> Observe()
        {
            /*201904 201804 -- 同比    201904 201905 -- 环比
             指标	上月	本月	同比（+/-）	环比（+/-）	备注
            治疗总人次
            其中：普通透析
            高通血透
            血液透析滤过
            血液灌流
            严重并发症发生例次  
             */
            return Task.Run(async () =>
            {
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(6);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, dtNow.Month, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, dtNow.Month, days, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(UpdtNow.Year, UpdtNow.Month, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(UpdtNow.Year, UpdtNow.Month, UpDays, 23, 59, 59);
                MedicalIndexesMonth medicalIndexesMonth = new MedicalIndexesMonth();
                //本月
                var NowCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).ToListAsync();
                //上月
                var UpCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= UpbeginTime && t.FounderDate < UpendTime && t.DataState == 1).ToListAsync();
                //去年本月
                var UpYearCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= beginTime.AddYears(-1) && t.FounderDate < endTime.AddYears(-1) && t.DataState == 1).ToListAsync();

                int TongBi = 0;
                int HuanBi = 0;
                int CureSum = 0;
                foreach (var item in CenterData)
                {
                    foreach (var type in Typedata)
                    {
                        Expression<Func<HistoryDialysisRecords, bool>> predicate = t => t.DataState == 1 && t.CenterId == item.Id;
                        if (type.EnumValue != 1)//总次数
                        {
                            predicate = predicate.And(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == type.EnumName.Replace('_', '+'));
                        }

                        var Nowhd = NowCureData.Where(predicate.Compile()).Count();
                        var Uphd = UpCureData.Where(predicate.Compile()).Count();
                        var UpYearhd = UpYearCureData.Where(predicate.Compile()).Count();
                        TongBi = Nowhd - UpYearhd;
                        HuanBi = Nowhd - Uphd;

                        var temp = MedicalIndexesMonthStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsMonth)type.EnumValue && t.StatisticalType == MedicalStatisticalType.ObserveIndicators && t.MondthDate.Value.ToString("yyyy-MM") == dtNow.ToString("yyyy-MM")).FirstOrDefault();
                        if (temp == null)
                        {
                            medicalIndexesMonth = new MedicalIndexesMonth()
                            {
                                Id = Guid.NewGuid().tostring32(),
                                CenterId = item.Id,
                                CurrentMonth = Nowhd,
                                DataState = 1,
                                Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                FounderDate = dtNow,
                                Indicators = (MedicalIndicatorsMonth)type.EnumValue,
                                LastMonth = Uphd,
                                MondthDate = dtNow,
                                LastYear = UpYearhd,
                                Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",
                                SameCompared = TongBi,
                                Sequential = HuanBi,
                                StatisticalType = MedicalStatisticalType.ObserveIndicators,
                            };
                            MedicalIndexesMonthStore.Insert(medicalIndexesMonth);
                        }
                        else
                        {

                            //   temp.CenterId = item.Id;
                            temp.CurrentMonth = Nowhd;
                            temp.DataState = 1;
                            temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                            temp.FounderDate = dtNow;
                            //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;
                            temp.LastMonth = Uphd;
                            temp.LastYear = UpYearhd;
                            //  temp.MondthDate = DateTime.Now;
                            temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                            temp.SameCompared = TongBi;
                            temp.Sequential = HuanBi;
                            //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                            MedicalIndexesMonthStore.Update(temp);
                        }
                        _unitOfWork.SaveChanges();
                    }



                }
                // 患者人
                //  await PatientsObserve();


                //环比

                return true;
            });

        }

        /// <summary>
        /// 患者人数
        /// </summary>
        /// <returns></returns>
        private Task<bool> PatientsObserve()
        {
            return Task.Run(async () =>
            {
                /*
             201904 201804 -- 同比    201904 201905 -- 环比
               指标	上月	本月	同比（+/-）	环比（+/-）	备注
                   */
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(4, 6);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, dtNow.Month, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, dtNow.Month, days, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(UpdtNow.Year, UpdtNow.Month, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(UpdtNow.Year, UpdtNow.Month, UpDays, 23, 59, 59);
                MedicalIndexesMonth medicalIndexesMonth = new MedicalIndexesMonth();
                //本月 当前在院人
                var NowP = await PatientStore.Entities.Include(t => t.TransferRecords).Where(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74").ToListAsync();

                //上月  在月底前有转入记录，且在转入时间后无转出记录
                var UpP = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();
                // List<Patient> NowP = new List<Patient>();
                List<Patient> UppCount = new List<Patient>();
                //上年本月
                List<Patient> UpYearCount = new List<Patient>();
                foreach (var item in UpP)
                {
                    //var nowtemptrans = item.TransferRecords.OrderByDescending(t => t.TransferDate).Where(t => t.TransferDate <= endTime).FirstOrDefault();
                    //if (item.ReceiveDate <= endTime && (nowtemptrans == null || (nowtemptrans != null && nowtemptrans.TransferType == "3f4eaafc1d134659bc323bd251041a74")))
                    //    NowP.Add(item);
                    var temptrans = item.TransferRecords.OrderByDescending(t => t.TransferDate).Where(t => t.TransferDate <= UpendTime).FirstOrDefault();

                    var temptrans1 = item.TransferRecords.OrderByDescending(t => t.TransferDate).Where(t => t.TransferDate <= endTime.AddYears(-1)).FirstOrDefault();
                    if (item.ReceiveDate <= UpendTime && (temptrans == null || (temptrans != null && temptrans.TransferType == "3f4eaafc1d134659bc323bd251041a74")))
                        UppCount.Add(item);
                    if (item.ReceiveDate <= endTime.AddYears(-1) && (temptrans1 == null || (temptrans1 != null && temptrans1.TransferType == "3f4eaafc1d134659bc323bd251041a74")))
                        UpYearCount.Add(item);

                }
                int TongBi = 0;
                int HuanBi = 0;
                int CureSum = 0;
                //上年本月
                // var NowYearP = await PatientStore.Entities.Include(t => t.TransferRecords).Where(t => t.ReceiveDate > beginTime.AddYears(-1) && t.ReceiveDate <= endTime.AddYears(-1)).ToListAsync();
                foreach (var item in CenterData)
                {
                    foreach (var type in Typedata)
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.CenterId == item.Id;
                        if (type.EnumValue != 7)//总次数
                        {
                            switch (type.EnumValue)
                            {
                                case 8://职工
                                    predicate = predicate.And(t => t.SIInsuredType == "57");
                                    break;
                                case 9: //居民
                                    predicate = predicate.And(t => t.SIInsuredType == "56");
                                    break;
                                case 10: //其他
                                    predicate = predicate.And(t => t.SIInsuredType != "56" && t.SIInsuredType != "57");
                                    break;

                            }
                        }

                        var Nowhd = NowP.Where(predicate.Compile()).Count();
                        var Uphd = UppCount.Where(predicate.Compile()).Count();
                        var UpYearhd = UpYearCount.Where(predicate.Compile()).Count();
                        TongBi = Nowhd - UpYearhd;
                        HuanBi = Nowhd - Uphd;

                        var temp = MedicalIndexesMonthStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsMonth)type.EnumValue && t.StatisticalType == MedicalStatisticalType.Patients && t.MondthDate.Value.ToString("yyyy-MM") == dtNow.ToString("yyyy-MM")).FirstOrDefault();
                        if (temp == null)
                        {
                            medicalIndexesMonth = new MedicalIndexesMonth()
                            {
                                Id = Guid.NewGuid().tostring32(),
                                CenterId = item.Id,
                                CurrentMonth = Nowhd,
                                DataState = 1,
                                Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                FounderDate = dtNow,
                                Indicators = (MedicalIndicatorsMonth)type.EnumValue,
                                LastMonth = Uphd,
                                MondthDate = dtNow,
                                LastYear = UpYearhd,
                                Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",
                                SameCompared = TongBi,
                                Sequential = HuanBi,
                                StatisticalType = MedicalStatisticalType.Patients,
                            };
                            MedicalIndexesMonthStore.Insert(medicalIndexesMonth);
                        }
                        else
                        {

                            //   temp.CenterId = item.Id;
                            temp.CurrentMonth = Nowhd;
                            temp.DataState = 1;
                            temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                            temp.FounderDate = dtNow;
                            //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;
                            temp.LastMonth = Uphd;
                            temp.LastYear = UpYearhd;
                            //  temp.MondthDate = DateTime.Now;
                            temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                            temp.SameCompared = TongBi;
                            temp.Sequential = HuanBi;
                            //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                            MedicalIndexesMonthStore.Update(temp);
                        }

                        _unitOfWork.SaveChanges();
                    }
                }
                return true;
            });

        }


        /// <summary>
        /// 归转人数
        /// </summary>
        /// <returns></returns>
        private Task<bool> TransferRecordObserve()
        {
            return Task.Run(async () =>
            {
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(6, 10);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, dtNow.Month, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, dtNow.Month, days, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(UpdtNow.Year, UpdtNow.Month, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(UpdtNow.Year, UpdtNow.Month, UpDays, 23, 59, 59);
                MedicalIndexesMonth medicalIndexesMonth = new MedicalIndexesMonth();
                //本月
                var NowTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > beginTime && t.TransferDate <= endTime).ToListAsync();
                var NowNewPa = await PatientStore.Entities.Where(t => t.ReceiveDate > beginTime && t.ReceiveDate <= endTime).ToListAsync();
                //上月
                var UpTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > UpbeginTime && t.TransferDate <= UpendTime).ToListAsync();
                var UpNewPa = await PatientStore.Entities.Where(t => t.ReceiveDate > UpbeginTime && t.ReceiveDate <= UpendTime).ToListAsync();
                //上年本月
                var NowYearTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > beginTime.AddYears(-1) && t.TransferDate <= endTime.AddYears(-1)).ToListAsync();
                var NowYearNewPa = await PatientStore.Entities.Where(t => t.ReceiveDate > beginTime.AddYears(-1) && t.ReceiveDate <= endTime.AddYears(-1)).ToListAsync();
                int TongBi = 0;
                int HuanBi = 0;
                foreach (var item in CenterData)
                {

                    foreach (var type in Typedata)
                    {
                        int NowCount = 0;
                        int UpCount = 0;
                        int UpYearCount = 0;
                        switch (type.EnumValue)
                        {
                            case 11: //新增患者
                                NowCount = NowNewPa.Where(t => t.CenterId == item.Id).Count();
                                UpCount = UpNewPa.Where(t => t.CenterId == item.Id).Count();
                                UpYearCount = NowYearNewPa.Where(t => t.CenterId == item.Id).Count();
                                break;
                            case 12: //死亡
                                NowCount = NowTransData.Where(t => t.CenterId == item.Id && t.TransferType == "feeb7b938b43489a8ff4dedfa0f9dde9").GroupBy(t => t.PatientId).Count();
                                UpCount = UpTransData.Where(t => t.CenterId == item.Id && t.TransferType == "feeb7b938b43489a8ff4dedfa0f9dde9").GroupBy(t => t.PatientId).Count();
                                UpYearCount = NowYearTransData.Where(t => t.CenterId == item.Id && t.TransferType == "feeb7b938b43489a8ff4dedfa0f9dde9").GroupBy(t => t.PatientId).Count();
                                break;

                            case 13:// 转院
                                NowCount = NowTransData.Where(t => t.CenterId == item.Id && t.TransferType == "20ce40fb89344e228199762a279adb63").OrderByDescending(t => t.TransferDate).GroupBy(t => t.PatientId).Count();
                                UpCount = UpTransData.Where(t => t.CenterId == item.Id && t.TransferType == "20ce40fb89344e228199762a279adb63").OrderByDescending(t => t.TransferDate).GroupBy(t => t.PatientId).Count();
                                UpYearCount = NowYearTransData.Where(t => t.CenterId == item.Id && t.TransferType == "20ce40fb89344e228199762a279adb63").OrderByDescending(t => t.TransferDate).GroupBy(t => t.PatientId).Count();
                                break;

                            default: //占时无法得到重病住院、转腹膜透析、肾移植

                                break;
                        }
                        TongBi = NowCount - UpYearCount;
                        HuanBi = NowCount - UpCount;

                        var temp = MedicalIndexesMonthStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsMonth)type.EnumValue && t.StatisticalType == MedicalStatisticalType.Regression && t.MondthDate.Value.ToString("yyyy-MM") == dtNow.ToString("yyyy-MM")).FirstOrDefault();
                        if (temp == null)
                        {
                            medicalIndexesMonth = new MedicalIndexesMonth()
                            {
                                Id = Guid.NewGuid().tostring32(),
                                CenterId = item.Id,
                                CurrentMonth = NowCount,
                                DataState = 1,
                                Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                FounderDate = dtNow,
                                Indicators = (MedicalIndicatorsMonth)type.EnumValue,
                                LastMonth = UpCount,
                                LastYear = UpYearCount,
                                MondthDate = dtNow,
                                Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",
                                SameCompared = TongBi,
                                Sequential = HuanBi,
                                StatisticalType = MedicalStatisticalType.Regression,
                            };
                            MedicalIndexesMonthStore.Insert(medicalIndexesMonth);
                        }
                        else
                        {

                            //   temp.CenterId = item.Id;
                            temp.CurrentMonth = NowCount;
                            temp.DataState = 1;
                            temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                            temp.FounderDate = dtNow;
                            //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;
                            temp.LastMonth = UpCount;
                            temp.LastYear = UpYearCount;
                            //  temp.MondthDate = DateTime.Now;
                            temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                            temp.SameCompared = TongBi;
                            temp.Sequential = HuanBi;
                            //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                            MedicalIndexesMonthStore.Update(temp);
                        }

                        _unitOfWork.SaveChanges();

                    }

                }

                return true;
            });
        }

        /// <summary>
        /// 月度统计指标
        /// </summary>
        /// <returns></returns>
        private Task<bool> MonthStatisticalObserve()
        {
            return Task.Run(async () =>
            {
                /* 
        [ChineseEnum("透析前血压达标率")]
        TXQXYDBL = 17,
        [ChineseEnum("透析后血压达标率")]
        TXHXYDBL = 18,
        [ChineseEnum("血常规采血例次")]
        XCGCXLC = 19,
        [ChineseEnum("血红蛋白（110-120g/L）达标率")]
        XYDB110DBL = 20,
        [ChineseEnum("血红蛋白（100-130g/L）达标率")]
        XHDB100DBL = 21,
                 */
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsMonth>(5, 16);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, dtNow.Month, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, dtNow.Month, days, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(UpdtNow.Year, UpdtNow.Month, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(UpdtNow.Year, UpdtNow.Month, UpDays, 23, 59, 59);
                MedicalIndexesMonth medicalIndexesMonth = new MedicalIndexesMonth();
                //本月
                var NowCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Include(t => t.preTreatMessage).Include(t => t.postPreTreatMessage).Where(t => t.ModifierDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).ToListAsync();

                var NowRoutineBloodDatas = await RoutineBloodRecordStore.Entities.Where(t => t.CheckDate >= beginTime && t.CheckDate < endTime && t.DataState == 1).ToListAsync();

                //上月
                var UpCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.preTreatMessage).Include(t => t.postPreTreatMessage).Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= UpbeginTime && t.FounderDate < UpendTime && t.DataState == 1).ToListAsync();
                var UpRoutineBloodDatas = await RoutineBloodRecordStore.Entities.Where(t => t.CheckDate >= UpbeginTime && t.CheckDate < UpendTime && t.DataState == 1).ToListAsync();
                //去年本月
                var UpYearCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.preTreatMessage).Include(t => t.postPreTreatMessage).Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= beginTime.AddYears(-1) && t.FounderDate < endTime.AddYears(-1) && t.DataState == 1).ToListAsync();
                var UpYearRoutineBloodDatas = await RoutineBloodRecordStore.Entities.Where(t => t.CheckDate >= beginTime.AddYears(-1) && t.CheckDate < endTime.AddYears(-1) && t.DataState == 1).ToListAsync();
                int TongBi = 0;
                int HuanBi = 0;
                try
                {

                    foreach (var item in CenterData)
                    {

                        foreach (var type in Typedata)
                        {
                            int NowCount = 0;
                            int UpCount = 0;
                            int UpYearCount = 0;
                            switch (type.EnumValue)
                            {
                                //90mmHg<收缩压<140mmHg、60mmHg<舒张压<90mmHg
                                case 17: //血压透析前达标

                                    NowCount = NowCureData.Where(t => t.CenterId == item.Id && t.preTreatMessage.PreSystolicPressure + "" != "" && (Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) >= 90 && Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) <= 140)).GroupBy(t => t.PatientId).Count();
                                    UpCount = UpCureData.Where(t => t.CenterId == item.Id && t.preTreatMessage.PreSystolicPressure + "" != "" && (Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) >= 90 && Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) <= 140)).GroupBy(t => t.PatientId).Count();
                                    UpYearCount = UpYearCureData.Where(t => t.CenterId == item.Id && t.preTreatMessage.PreSystolicPressure + "" != "" && (Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) >= 90 && Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) <= 140)).GroupBy(t => t.PatientId).Count();

                                    break;
                                case 18://血压透析后达标
                                    NowCount = NowCureData.Where(t => t.postPreTreatMessage != null && t.postPreTreatMessage.PostSystolicPressure + "" != "" && t.CenterId == item.Id && (Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) >= 90 && Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) <= 140)).GroupBy(t => t.PatientId).Count();
                                    UpCount = UpCureData.Where(t => t.postPreTreatMessage != null && t.postPreTreatMessage.PostSystolicPressure + "" != "" && t.CenterId == item.Id && (Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) >= 90 && Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) <= 140)).GroupBy(t => t.PatientId).Count();
                                    UpYearCount = UpYearCureData.Where(t => t.postPreTreatMessage != null && t.postPreTreatMessage.PostSystolicPressure + "" != "" && t.CenterId == item.Id && (Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) >= 90 && Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) <= 140)).GroupBy(t => t.PatientId).Count();
                                    break;
                                case 19://血常规采血
                                    NowCount = NowRoutineBloodDatas.Where(t => t.CenterId == item.Id).GroupBy(t => t.PatientId).Count();
                                    UpCount = UpRoutineBloodDatas.Where(t => t.CenterId == item.Id).GroupBy(t => t.PatientId).Count();
                                    UpYearCount = UpYearRoutineBloodDatas.Where(t => t.CenterId == item.Id).GroupBy(t => t.PatientId).Count();
                                    break;
                                case 20://血红蛋白110-120
                                    NowCount = NowRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 110 && t.Hematocrit <= 120).GroupBy(t => t.PatientId).Count();
                                    UpCount = UpRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 110 && t.Hematocrit <= 120).GroupBy(t => t.PatientId).Count();
                                    UpYearCount = UpYearRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 110 && t.Hematocrit <= 120).GroupBy(t => t.PatientId).Count();
                                    break;
                                case 21://血红蛋白100-130
                                    NowCount = NowRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 100 && t.Hematocrit <= 130).GroupBy(t => t.PatientId).Count();
                                    UpCount = UpRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 100 && t.Hematocrit <= 130).GroupBy(t => t.PatientId).Count();
                                    UpYearCount = UpYearRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 100 && t.Hematocrit <= 130).GroupBy(t => t.PatientId).Count();
                                    break;
                                default:
                                    break;
                            }
                            TongBi = NowCount - UpYearCount;
                            HuanBi = NowCount - UpCount;

                            var temp = MedicalIndexesMonthStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsMonth)type.EnumValue && t.StatisticalType == MedicalStatisticalType.StatisticalIndicators && t.MondthDate.Value.ToString("yyyy-MM") == dtNow.ToString("yyyy-MM")).FirstOrDefault();
                            if (temp == null)
                            {
                                medicalIndexesMonth = new MedicalIndexesMonth()
                                {
                                    Id = Guid.NewGuid().tostring32(),
                                    CenterId = item.Id,
                                    CurrentMonth = NowCount,
                                    DataState = 1,
                                    Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                    FounderDate = dtNow,
                                    Indicators = (MedicalIndicatorsMonth)type.EnumValue,
                                    LastMonth = UpCount,
                                    LastYear = UpYearCount,
                                    MondthDate = dtNow,
                                    Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",
                                    SameCompared = TongBi,
                                    Sequential = HuanBi,
                                    StatisticalType = MedicalStatisticalType.StatisticalIndicators,
                                };
                                MedicalIndexesMonthStore.Insert(medicalIndexesMonth);
                            }
                            else
                            {

                                //   temp.CenterId = item.Id;
                                temp.CurrentMonth = NowCount;
                                temp.DataState = 1;
                                temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                                temp.FounderDate = dtNow;
                                //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;
                                temp.LastMonth = UpCount;
                                temp.LastYear = UpYearCount;
                                //  temp.MondthDate = DateTime.Now;
                                temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                                temp.SameCompared = TongBi;
                                temp.Sequential = HuanBi;
                                //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                                MedicalIndexesMonthStore.Update(temp);
                            }

                            _unitOfWork.SaveChanges();

                        }
                    }
                }
                catch (Exception exp)
                {
                }
                return true;
            });
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
                        int C = 1;// ReturnTest();
                        int L = 1;// ReturnTest();
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
                        // MedicalIndexesYearStore.Insert(medicalIndexesMonth);
                    }
                }

                _unitOfWork.SaveChanges();
            }




        }


    }


    public class IndexesYearStatistical
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IGetUserInfo _getUserInfo;


        public IndexesYearStatistical(IUnitOfWork unitOfWork, IGetUserInfo getUserInfo)
        {
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
        }


        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<MedicalIndexesYear> MedicalIndexesYearStore => _unitOfWork.GetStore<MedicalIndexesYear>();
        private IRepository<HistoryDialysisRecords> HistoryDialysisRecordsStore => _unitOfWork.GetStore<HistoryDialysisRecords>();
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();

        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();

        private IRepository<RoutineBloodRecord> RoutineBloodRecordStore => _unitOfWork.GetStore<RoutineBloodRecord>();
        private IRepository<TransferRecord> TransferRecordStore => _unitOfWork.GetStore<TransferRecord>();
        //年度观察指标
        private IRepository<InfectiousDiseasesRecord> InfectiousDiseasesRecordsStore => _unitOfWork.GetStore<InfectiousDiseasesRecord>();

        private IRepository<SevenElectrolyteTermsRecord> SevenElectrolyteTermsRecordStore => _unitOfWork.GetStore<SevenElectrolyteTermsRecord>();
        private IRepository<MineralAndBoneMetabolismRecord> MineralAndBoneMetabolismRecordStore => _unitOfWork.GetStore<MineralAndBoneMetabolismRecord>();
        private IRepository<DialysisAdequacyRecord> DialysisAdequacyRecordStore => _unitOfWork.GetStore<DialysisAdequacyRecord>();

        public Task<bool> StartObserveAnalyze()
        {
            return Task.Run(async () =>
            {
                await Observe();
                await InfectionObserve();
                await VascularAccessRecordObserve();
                await YearStatisticalObserve();
                return true;
            });
        }
        DateTime dtNow = DateTime.Now;
        DateTime UpdtNow = DateTime.Now.AddMonths(-1);


        /// <summary>
        /// 观察指标
        /// </summary>
        /// <returns></returns>
        private Task<bool> Observe()
        {
            return Task.Run(async () =>
            {
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(11);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, 1, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, 12, 31, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(dtNow.Year - 1, 1, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(dtNow.Year - 1, 12, 31, 23, 59, 59);
                MedicalIndexesYear medicalIndexesMonth = new MedicalIndexesYear();
                //本年
                var NowCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).ToListAsync();
                var NowTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > beginTime && t.TransferDate <= endTime).ToListAsync();
                //上年
                var UpCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= UpbeginTime && t.FounderDate < UpendTime && t.DataState == 1).ToListAsync();
                var UpTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > UpbeginTime && t.TransferDate <= UpendTime).ToListAsync();

                foreach (var item in CenterData)
                {
                    foreach (var type in Typedata)
                    {
                        int Nowhd = 0;//09.32  18.12
                        int Uphd = 0;
                        switch (type.EnumValue)
                        {
                            case 8://死亡
                                Nowhd = NowTransData.Where(t => t.CenterId == item.Id && t.TransferType == "feeb7b938b43489a8ff4dedfa0f9dde9").Count();
                                Uphd = UpTransData.Where(t => t.CenterId == item.Id && t.TransferType == "feeb7b938b43489a8ff4dedfa0f9dde9").Count();
                                break;
                            default:
                                Expression<Func<HistoryDialysisRecords, bool>> predicate = t => t.DataState == 1 && t.CenterId == item.Id;
                                if (type.EnumValue != 1)//总次数
                                {
                                    predicate = predicate.And(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == type.EnumName.Replace('_', '+'));
                                }
                                Nowhd = NowCureData.Where(predicate.Compile()).Count();
                                Uphd = UpCureData.Where(predicate.Compile()).Count();
                                break;
                        }

                        var temp = MedicalIndexesYearStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsYear)type.EnumValue && t.StatisticalType == MedicalStatisticalYearType.ObserveIndicators && t.FounderDate.Value.ToString("yyyy") == DateTime.Now.ToString("yyyy")).FirstOrDefault(); 
                        if (temp == null)
                        {
                            medicalIndexesMonth = new MedicalIndexesYear()
                            {
                                Id = Guid.NewGuid().tostring32(),
                                CenterId = item.Id,
                                DataState = 1,
                                Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                FounderDate = DateTime.Now,
                                Indicators = (MedicalIndicatorsYear)type.EnumValue,
                                CurrentYear = Nowhd,
                                LastYear = Uphd,
                                Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",

                                StatisticalType = MedicalStatisticalYearType.ObserveIndicators
                            };
                            MedicalIndexesYearStore.Insert(medicalIndexesMonth);
                        }
                        else
                        {

                            //   temp.CenterId = item.Id;

                            temp.DataState = 1;
                            temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                            temp.FounderDate = DateTime.Now;
                            //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;

                            //  temp.MondthDate = DateTime.Now;
                            temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                            temp.LastYear = Uphd;
                            temp.CurrentYear = Nowhd;
                            //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                            MedicalIndexesYearStore.Update(temp);
                        }
                        _unitOfWork.SaveChanges();
                    }



                }
                // 患者人
                //  await PatientsObserve();


                //环比

                return true;
            });

        }

        /// <summary>
        /// 感染检测
        /// </summary>
        /// <returns></returns>
        private Task<bool> InfectionObserve()
        {
            return Task.Run(async () =>
            {
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(3, 11);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, 1, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, 12, 31, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(dtNow.Year - 1, 1, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(dtNow.Year - 1, 12, 31, 23, 59, 59);
                MedicalIndexesYear medicalIndexesMonth = new MedicalIndexesYear();
                //本年
                var NowCureData = await InfectiousDiseasesRecordsStore.Entities.Include(t => t.Patient).Where(t => t.ModifierDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).ToListAsync();

                //上年
                var UpCureData = await InfectiousDiseasesRecordsStore.Entities.Include(t => t.Patient).Where(t => t.ModifierDate >= UpbeginTime && t.FounderDate < UpendTime && t.DataState == 1).ToListAsync();  
                try
                { 
                    foreach (var item in CenterData)
                    {
                        foreach (var type in Typedata)
                        {
                            int Nowhd = 0;
                            int Uphd = 0;
                            switch (type.EnumValue)
                            {

                                case 12://HBsAg转阳  乙肝
                                    Nowhd = NowCureData.Where(t => t.CenterId == item.Id && t.HBsAg != null && (t.HBsAg.Contains("+") || t.HBsAg.Contains("阳")) && t.Patient.BloodBorneDisease != "bb629288d3714a7e964ef895547a70a6").Count();
                                    Uphd = UpCureData.Where(t => t.CenterId == item.Id && t.HBsAg != null && (t.HBsAg.Contains("+") || t.HBsAg.Contains("阳")) && t.Patient.BloodBorneDisease != "bb629288d3714a7e964ef895547a70a6").Count();
                                    break;
                                case 13://HBeAg转阳
                                    Nowhd = NowCureData.Where(t => t.CenterId == item.Id && t.HBeAg != null && (t.HBeAg.Contains("+") || t.HBeAg.Contains("阳")) && t.Patient.BloodBorneDisease != "bb629288d3714a7e964ef895547a70a6").Count();
                                    Uphd = UpCureData.Where(t => t.CenterId == item.Id && t.HBeAg != null && (t.HBeAg.Contains("+") || t.HBeAg.Contains("阳")) && t.Patient.BloodBorneDisease != "bb629288d3714a7e964ef895547a70a6").Count();
                                    break;
                                case 14://HCV抗体转阳 丙肝 360c66d7bdaa4592b84ffa790895de92
                                    Nowhd = NowCureData.Where(t => t.CenterId == item.Id && t.HepatitisCAntibody != null && (t.HepatitisCAntibody.Contains("+") || t.HepatitisCAntibody.Contains("阳")) && t.Patient.BloodBorneDisease != "360c66d7bdaa4592b84ffa790895de92").Count();
                                    Uphd = UpCureData.Where(t => t.CenterId == item.Id && t.HepatitisCAntibody != null && (t.HepatitisCAntibody.Contains("+") || t.HepatitisCAntibody.Contains("阳")) && t.Patient.BloodBorneDisease != "360c66d7bdaa4592b84ffa790895de92").Count();
                                    break;
                                default:
                                    break;
                            }

                            var temp = MedicalIndexesYearStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsYear)type.EnumValue && t.StatisticalType == MedicalStatisticalYearType.Infection && t.FounderDate.Value.ToString("yyyy") == DateTime.Now.ToString("yyyy")).FirstOrDefault();


                            if (temp == null)
                            {
                                medicalIndexesMonth = new MedicalIndexesYear()
                                {
                                    Id = Guid.NewGuid().tostring32(),
                                    CenterId = item.Id,

                                    DataState = 1,
                                    Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                    FounderDate = DateTime.Now,
                                    Indicators = (MedicalIndicatorsYear)type.EnumValue,
                                    CurrentYear = Nowhd,
                                    LastYear = Uphd,
                                    Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",

                                    StatisticalType = MedicalStatisticalYearType.Infection
                                };
                                MedicalIndexesYearStore.Insert(medicalIndexesMonth);
                            }
                            else
                            {
                                //   temp.CenterId = item.Id;
                                temp.DataState = 1;
                                temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                                temp.FounderDate = DateTime.Now;
                                //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;

                                //  temp.MondthDate = DateTime.Now;
                                temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                                temp.LastYear = Uphd;
                                temp.CurrentYear = Nowhd;
                                //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                                MedicalIndexesYearStore.Update(temp);
                            }
                            _unitOfWork.SaveChanges();

                        }
                    }
                }
                catch (Exception exo)
                {

                }


                return true;
            });

        }

        /// <summary>
        /// 血管通路
        /// </summary>
        /// <returns></returns>
        private Task<bool> VascularAccessRecordObserve()
        {
            return Task.Run(async () =>
            {

                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(5, 14);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, 1, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, 12, 31, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(dtNow.Year - 1, 1, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(dtNow.Year - 1, 12, 31, 23, 59, 59);
                MedicalIndexesYear medicalIndexesMonth = new MedicalIndexesYear();
                //本年
                var NowCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Include(t => t.preTreatMessage).Where(t => t.ModifierDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).ToListAsync();
                //  var NowTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > beginTime && t.TransferDate <= endTime).ToListAsync();
                //上年
                var UpCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Include(t => t.preTreatMessage).Where(t => t.ModifierDate >= UpbeginTime && t.FounderDate < UpendTime && t.DataState == 1).ToListAsync();
                //  var UpTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > UpbeginTime && t.TransferDate <= UpendTime).ToListAsync();

                foreach (var item in CenterData)
                {
                    foreach (var type in Typedata)
                    {
                        int Nowhd = 0;
                        int Uphd = 0;
                        switch (type.EnumValue)
                        {
                            case 15://动静脉内瘘
                                Nowhd = NowCureData.Where(t => t.CenterId == item.Id && t.patientCycleScheduling.currentDialysisProgram.BloodAccess.Contains("内瘘")).Count();
                                Uphd = UpCureData.Where(t => t.CenterId == item.Id && t.patientCycleScheduling.currentDialysisProgram.BloodAccess.Contains("内瘘")).Count();
                                break;
                            case 16://NTC
                                Nowhd = NowCureData.Where(t => t.CenterId == item.Id && t.preTreatMessage.PunctureWay == "d3457aadd1e24a2698cd2caf86485c39").Count();
                                Uphd = UpCureData.Where(t => t.CenterId == item.Id && t.preTreatMessage.PunctureWay == "d3457aadd1e24a2698cd2caf86485c39").Count();
                                break;
                            case 17://TCC
                                Nowhd = NowCureData.Where(t => t.CenterId == item.Id && t.preTreatMessage.PunctureWay == "16821ea619624f88a8315cc260af1979").Count();
                                Uphd = UpCureData.Where(t => t.CenterId == item.Id && t.preTreatMessage.PunctureWay == "16821ea619624f88a8315cc260af1979").Count();
                                break;
                            case 18://动静脉直接穿刺
                                break;
                            case 19://人工血管
                                break;
                            default:
                                break;
                        }
                        var temp = MedicalIndexesYearStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsYear)type.EnumValue && t.StatisticalType == MedicalStatisticalYearType.VascularAccess && t.FounderDate.Value.ToString("yyyy") == DateTime.Now.ToString("yyyy")).FirstOrDefault();


                        if (temp == null)
                        {
                            medicalIndexesMonth = new MedicalIndexesYear()
                            {
                                Id = Guid.NewGuid().tostring32(),
                                CenterId = item.Id,

                                DataState = 1,
                                Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                FounderDate = DateTime.Now,
                                Indicators = (MedicalIndicatorsYear)type.EnumValue,
                                CurrentYear = Nowhd,
                                LastYear = Uphd,
                                Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",

                                StatisticalType = MedicalStatisticalYearType.VascularAccess
                            };
                            MedicalIndexesYearStore.Insert(medicalIndexesMonth);
                        }
                        else
                        {

                            //   temp.CenterId = item.Id;

                            temp.DataState = 1;
                            temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                            temp.FounderDate = DateTime.Now;
                            //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;

                            //  temp.MondthDate = DateTime.Now;
                            temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                            temp.LastYear = Uphd;
                            temp.CurrentYear = Nowhd;
                            //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                            MedicalIndexesYearStore.Update(temp);
                        }
                        _unitOfWork.SaveChanges();
                    }
                }

                return true;
            });
        }

        /// <summary>
        /// 年度统计指标
        /// </summary>
        /// <returns></returns>
        private Task<bool> YearStatisticalObserve()
        {

            return Task.Run(async () =>
            {
                var CenterData = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != "").ToListAsync();
                List<EnumberEntity> Typedata = new List<EnumberEntity>();
                Typedata = CDGService.Data.Helper.EnumHelper.EnumToList<MedicalIndicatorsYear>(10, 19);

                int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
                int UpDays = DateTime.DaysInMonth(UpdtNow.Year, UpdtNow.Month);
                DateTime beginTime = new DateTime(dtNow.Year, 1, 1, 0, 0, 0);
                DateTime endTime = new DateTime(dtNow.Year, 12, 31, 23, 59, 59);
                DateTime UpbeginTime = new DateTime(dtNow.Year - 1, 1, 1, 0, 0, 0);
                DateTime UpendTime = new DateTime(dtNow.Year - 1, 12, 31, 23, 59, 59);
                MedicalIndexesYear medicalIndexesMonth = new MedicalIndexesYear();

                //本年 死亡

                var NowTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > beginTime && t.TransferDate <= endTime).ToListAsync();
                //上年

                var UpTransData = await TransferRecordStore.Entities.Where(t => t.TransferDate > UpbeginTime && t.TransferDate <= UpendTime).ToListAsync();


                //本年血压

                var NowCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.preTreatMessage).Include(t => t.postPreTreatMessage).Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).ToListAsync();

                var NowRoutineBloodDatas = await RoutineBloodRecordStore.Entities.Where(t => t.CheckDate >= beginTime && t.CheckDate < endTime && t.DataState == 1).ToListAsync();
                //去年 
                var UpCureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.preTreatMessage).Include(t => t.postPreTreatMessage).Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.ModifierDate >= UpbeginTime && t.FounderDate < UpendTime && t.DataState == 1).ToListAsync();
                var UpRoutineBloodDatas = await RoutineBloodRecordStore.Entities.Where(t => t.CheckDate >= UpbeginTime && t.CheckDate < UpendTime && t.DataState == 1).ToListAsync();

                //电解质（钙 磷） 甲状腺（矿物质） 血透充分性（ktv urr）

                var NowdjzData = await SevenElectrolyteTermsRecordStore.Entities.Where(t => t.CheckDate >= beginTime && t.CheckDate <= endTime).ToListAsync();
                var NowkwzData = await MineralAndBoneMetabolismRecordStore.Entities.Where(t => t.CheckDate >= beginTime && t.CheckDate <= endTime).ToListAsync();
                var NowcfxData = await DialysisAdequacyRecordStore.Entities.Where(t => t.FounderDate >= beginTime && t.FounderDate <= endTime).ToListAsync();

                var UpdjzData = await SevenElectrolyteTermsRecordStore.Entities.Where(t => t.CheckDate >= UpbeginTime && t.CheckDate <= UpendTime).ToListAsync();
                var UpkwzData = await MineralAndBoneMetabolismRecordStore.Entities.Where(t => t.CheckDate >= UpbeginTime && t.CheckDate <= UpendTime).ToListAsync();
                var UpwcfxData = await DialysisAdequacyRecordStore.Entities.Where(t => t.FounderDate >= UpbeginTime && t.FounderDate <= UpendTime).ToListAsync();

                foreach (var item in CenterData)
                {
                    foreach (var type in Typedata)
                    {

                        int Nowhd = 0;
                        int Uphd = 0;
                        switch (type.EnumValue)
                        {
                            case 20://死亡
                                Nowhd = NowTransData.Where(t => t.CenterId == item.Id && t.TransferType == "feeb7b938b43489a8ff4dedfa0f9dde9").Count();
                                Uphd = UpTransData.Where(t => t.CenterId == item.Id && t.TransferType == "feeb7b938b43489a8ff4dedfa0f9dde9").Count();
                                break;

                            case 21: //血压透析前达标

                                Nowhd = NowCureData.Where(t => (t.preTreatMessage != null && t.CenterId == item.Id) && (t.preTreatMessage.PreSystolicPressure + "" != "" && (Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) >= 90 && Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) <= 140))).GroupBy(t => t.PatientId).Count();
                                Uphd = UpCureData.Where(t => (t.preTreatMessage != null && t.CenterId == item.Id) && (t.preTreatMessage.PreSystolicPressure + "" != "" && (Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) >= 90 && Convert.ToInt32(t.preTreatMessage.PreSystolicPressure) <= 140))).GroupBy(t => t.PatientId).Count();

                                break;
                            case 22://血压透析后达标
                                Nowhd = NowCureData.Where(t => (t.postPreTreatMessage != null && t.postPreTreatMessage.PostSystolicPressure + "" != "" && t.CenterId == item.Id) && ((Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) >= 90 && Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) <= 140))).GroupBy(t => t.PatientId).Count();
                                Uphd = UpCureData.Where(t => (t.postPreTreatMessage != null && t.postPreTreatMessage.PostSystolicPressure + "" != "" && t.CenterId == item.Id) && ((Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) >= 90 && Convert.ToInt32(t.postPreTreatMessage.PostSystolicPressure) <= 140))).GroupBy(t => t.PatientId).Count();

                                break;

                            case 26://血红蛋白110-120
                                Nowhd = NowRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 110 && t.Hematocrit <= 120).GroupBy(t => t.PatientId).Count();
                                Uphd = UpRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 110 && t.Hematocrit <= 120).GroupBy(t => t.PatientId).Count();

                                break;
                            case 27://血红蛋白100-130 申请表和使用手册：19 36 38页
                                Nowhd = NowRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 100 && t.Hematocrit <= 130).GroupBy(t => t.PatientId).Count();
                                Uphd = UpRoutineBloodDatas.Where(t => t.CenterId == item.Id && t.Hemoglobin >= 100 && t.Hematocrit <= 130).GroupBy(t => t.PatientId).Count();

                                break;

                            case 23: //钙（2.10-2.50mmol/L）
                                Nowhd = NowdjzData.Where(t => t.CenterId == item.Id && t.Calcium >= Convert.ToDecimal(2.10) && t.Calcium <= Convert.ToDecimal(2.50)).Count();
                                Uphd = UpdjzData.Where(t => t.CenterId == item.Id && t.Calcium >= Convert.ToDecimal(2.10) && t.Calcium <= Convert.ToDecimal(2.50)).Count();
                                break;
                            case 24://磷（1.13-1.78mmol/L）
                                Nowhd = NowdjzData.Where(t => t.CenterId == item.Id && t.InorganicPhosphorus >= Convert.ToDecimal(1.13) && t.InorganicPhosphorus <= Convert.ToDecimal(1.78)).Count();
                                Uphd = UpdjzData.Where(t => t.CenterId == item.Id && t.InorganicPhosphorus >= Convert.ToDecimal(1.13) && t.InorganicPhosphorus <= Convert.ToDecimal(1.78)).Count();

                                break;
                            case 25://PTH（150-300pg/L）

                                Nowhd = NowkwzData.Where(t => t.CenterId == item.Id && t.ParathyroidHormone >= Convert.ToDecimal(150) && t.ParathyroidHormone <= Convert.ToDecimal(300)).Count();
                                Uphd = UpkwzData.Where(t => t.CenterId == item.Id && t.ParathyroidHormone >= Convert.ToDecimal(150) && t.ParathyroidHormone <= Convert.ToDecimal(300)).Count();

                                break;
                            case 28://Kt/V达标率（≥1.2）
                                Nowhd = NowcfxData.Where(t => t.CenterId == item.Id && t.KtV >= Convert.ToDecimal(1.2)).Count();
                                Uphd = UpwcfxData.Where(t => t.CenterId == item.Id && t.KtV >= Convert.ToDecimal(1.2)).Count();
                                break;
                            case 29://URR值达标率（≥60%）
                                Nowhd = NowcfxData.Where(t => t.CenterId == item.Id && t.URR >= Convert.ToDecimal(60)).Count();
                                Uphd = UpwcfxData.Where(t => t.CenterId == item.Id && t.URR >= Convert.ToDecimal(60)).Count();
                                break;
                            default:

                                break;
                        }
                        var temp = MedicalIndexesYearStore.Entities.Where(t => t.CenterId == item.Id && t.Indicators == (MedicalIndicatorsYear)type.EnumValue && t.StatisticalType == MedicalStatisticalYearType.StatisticalIndicators && t.FounderDate.Value.ToString("yyyy") == DateTime.Now.ToString("yyyy")).FirstOrDefault();


                        if (temp == null)
                        {
                            medicalIndexesMonth = new MedicalIndexesYear()
                            {
                                Id = Guid.NewGuid().tostring32(),
                                CenterId = item.Id,

                                DataState = 1,
                                Founder = "a51e842fdb544dfea3c274aee6d5ec0f",
                                FounderDate = DateTime.Now,
                                Indicators = (MedicalIndicatorsYear)type.EnumValue,
                                CurrentYear = Nowhd,
                                LastYear = Uphd,
                                Remarks = $"{DateTime.Now.ToShortDateString()}系统自动更新生成",

                                StatisticalType = MedicalStatisticalYearType.StatisticalIndicators
                            };
                            MedicalIndexesYearStore.Insert(medicalIndexesMonth);
                        }
                        else
                        {

                            //   temp.CenterId = item.Id;

                            temp.DataState = 1;
                            temp.Founder = "a51e842fdb544dfea3c274aee6d5ec0f";
                            temp.FounderDate = DateTime.Now;
                            //   temp.Indicators = (MedicalIndicatorsMonth)type.EnumValue;

                            //  temp.MondthDate = DateTime.Now;
                            temp.Remarks = $"{DateTime.Now}系统自动计算生成";
                            temp.LastYear = Uphd;
                            temp.CurrentYear = Nowhd;
                            //  temp.StatisticalType = MedicalStatisticalType.ObserveIndicators;
                            MedicalIndexesYearStore.Update(temp);
                        }
                        _unitOfWork.SaveChanges();

                    }
                }




                return true;
            });
        }

    }
}
