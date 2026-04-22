using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Utils;
using CDGService.WebAPI.Datas;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 结果质控
    /// </summary>
    public class ResultControlManager
    {
        private readonly IGetUserInfo _getUserInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;

        private IRepository<Log> LogStore => _unitOfWork.GetStore<Log>();
        private readonly DictionaryCode _dictionaryCode;
        /// <summary>
        /// 病人在院状态ID
        /// </summary>
        private string HState = "";
        /// <summary>
        /// 结果质控
        /// </summary>
        public ResultControlManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<DictionaryCode> dictionaryCode)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _dictionaryCode = dictionaryCode.Value;
            HState = _dictionaryCode.HStateId;
        }
        private double ReturnTest()
        {
            //double num = 0.80;
            return 0;
            var rd = new Random();
            return rd.Next(45, 100) * 1.00 / 100.00;
        }
        /// <summary>
        /// 血常规检查
        /// </summary>
        private IRepository<RoutineBloodRecord> RoutineBloodRecordStore => _unitOfWork.GetStore<RoutineBloodRecord>();
        private IRepository<LiverFunctionBloodFatBoodSugarRecord> LiverFunctionBloodFatBoodSugarRecordStore => _unitOfWork.GetStore<LiverFunctionBloodFatBoodSugarRecord>();
        private IRepository<DialysisAdequacyRecord> DialysisAdequacyRecordStore => _unitOfWork.GetStore<DialysisAdequacyRecord>();
        private IRepository<CurrentDialysisProgram> CurrentDialysisProgramStore => _unitOfWork.GetStore<CurrentDialysisProgram>();


        private IRepository<SevenElectrolyteTermsRecord> SevenElectrolyteTermsRecordStore => _unitOfWork.GetStore<SevenElectrolyteTermsRecord>();
        private IRepository<PreTreatMessage> PreTreatMessageStore => _unitOfWork.GetStore<PreTreatMessage>();
        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        /// <summary>
        /// 甲状腺
        /// </summary>
        private IRepository<MineralAndBoneMetabolismRecord> MineralAndBoneMetabolismRecordStore => _unitOfWork.GetStore<MineralAndBoneMetabolismRecord>();

        /// <summary>
        /// 透析方案 - 透析期间体重增长控制率
        /// </summary>
        private IRepository<CurrentDialysisProgram> CurrentDialysisProgramsStore => _unitOfWork.GetStore<CurrentDialysisProgram>();


        /*
         ☆高血压质控率	每月   排序后去中间值
         维持性血液透析患者透析前
         血压＜140/90MMHG的60岁以下患者和   
         血压＜150/90MMHG的60岁以上患者占
         同期维持性血液透析患者数量的比率	
         山外山标准 ≥50%
         正常血压 90mmHg<收缩压<140mmHg、60mmHg<舒张压<90mmHg
         */
        //TODO:需要重新计算
        /// <summary>
        /// 高血压质控率 - 图形
        /// </summary>
        public Task<LineOutPut> QualityHypertensionChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                //患者总数
                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                //患者上机记录
                var LCheckData = await PreTreatMessageStore.Entities.Include(t => t.Patient).Where(t => t.LoginTime >= BeginTime && t.LoginTime < BeginTime.AddYears(1)).ToListAsync();//患者上机记录
                LCheckData = LCheckData.Where(t => t.Patient != null).ToList();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {
                            Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                            predicate = predicate.And(t => t.ReceiveDate < i.AddMonths(+2).AddDays(29));

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate < i.AddDays(29));

                            Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                            RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate >= i && t.CheckDate < i.AddMonths(+5).AddDays(29));


                            //检查人数 透析前血压＜140/90MMHG的60岁以下患者
                            var scheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.LoginTime >= i && t.LoginTime < i.AddDays(29) && Convert.ToDouble(t.PreDiastolicPressure) < 90 && Convert.ToDouble(t.PreSystolicPressure) < 140 && DateTime.Now.Year - t.Patient.Birthday?.Year < 60).GroupBy(t => t.PatientId).Count();
                            //血压＜150/90MMHG的60岁以上患者
                            var rcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.LoginTime >= i && t.LoginTime < i.AddDays(29) && Convert.ToDouble(t.PreDiastolicPressure) < 90 && Convert.ToDouble(t.PreSystolicPressure) < 150 && DateTime.Now.Year - t.Patient.Birthday?.Year > 60).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            //double sPercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                            //double rPercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                            var lcheckCount = scheckCount > rcheckCount ? rcheckCount : scheckCount;
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(Math.Round(PercentOfPass + ReturnTest(), 2));

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();
                    serise.name = "质控率";
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == InPut.CenterId && t.ReceiveDate < i.AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate >= i && t.CheckDate < i.AddMonths(+5).AddDays(29));

                        //检查人数
                        //检查人数 透析前血压＜140/90MMHG的60岁以下患者
                        var scheckCount = LCheckData.Where(t => t.CenterId == InPut.CenterId && t.LoginTime >= i && t.LoginTime < i.AddDays(29) && Convert.ToDouble(t.PreDiastolicPressure) < 90 && Convert.ToDouble(t.PreSystolicPressure) < 140 && DateTime.Now.Year - t.Patient.Birthday?.Year < 60).GroupBy(t => t.PatientId).Count();
                        //血压＜150/90MMHG的60岁以上患者
                        var rcheckCount = LCheckData.Where(t => t.CenterId == InPut.CenterId && t.LoginTime >= i && t.LoginTime < i.AddDays(29) && Convert.ToDouble(t.PreDiastolicPressure) < 90 && Convert.ToDouble(t.PreSystolicPressure) < 150 && DateTime.Now.Year - t.Patient.Birthday?.Year > 60).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //double sPercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                        //double rPercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                        var lcheckCount = scheckCount > rcheckCount ? rcheckCount : scheckCount;
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);

                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        //    serise.data.Add(PercentOfPass);
                        serise.data.Add(Math.Round(PercentOfPass + ReturnTest(), 2));
                        j++;

                    }
                    result.serises.Add(serise);
                    result.LegendData.Add("质控率");
                    // 
                }
                return result;
            });
        }

        /// <summary>
        /// 高血压质控率 - 表格
        /// </summary>
        public Task<PageData<QualityHypertensionDataOutPut[]>> QualityHypertensionDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            try
            {

                return Task.Run(async () =>
                {
                    List<QualityHypertensionDataOutPut> result = new List<QualityHypertensionDataOutPut>();
                    bool IsDel = false;
                //   string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                    List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                    List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                    if (InPut == null)
                    {
                        InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                    }
                    InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                    DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                    List<CenterDialysis> centerData = new List<CenterDialysis>();
                    if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                    {
                        centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                    }
                    else
                    {
                        centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                    }

                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                //患者上机记录
                var LCheckData = await PreTreatMessageStore.Entities.Include(t => t.Patient).Where(t => t.LoginTime >= BeginTime && t.LoginTime < BeginTime.AddYears(1) && t.PreDiastolicPressure+""!="").ToListAsync();//患者上机记录

                    LCheckData = LCheckData.Where(t => t.Patient != null).ToList();
                    int j = 1;
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        foreach (var item in centerData)
                        {
                        //Expression<Func<Patient, bool>> predicate = (t => t.CenterDialysisId == item.Id);
                        //predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate < i.AddDays(29));

                        //Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        //RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数 透析前血压＜140/90MMHG的60岁以下患者
                        var scheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.LoginTime >= i && t.LoginTime < i.AddDays(29) && Convert.ToDouble(t.PreDiastolicPressure) < 90 && Convert.ToDouble(t.PreSystolicPressure) < 140 && DateTime.Now.Year - t.Patient.Birthday?.Year < 60).GroupBy(t => t.PatientId).Count();
                        //血压＜150/90MMHG的60岁以上患者
                        var rcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.LoginTime >= i && t.LoginTime < i.AddDays(29) && Convert.ToDouble(t.PreDiastolicPressure) < 90 && Convert.ToDouble(t.PreSystolicPressure) < 150 && DateTime.Now.Year - t.Patient.Birthday?.Year > 60).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double sPercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                            double rPercentOfPass = PatientCount == 0 ? 0 : Math.Round((rcheckCount * 1.00) / (PatientCount * 1.00), 2);


                            result.Add(new QualityHypertensionDataOutPut()
                            {
                                CenterName = item.DialysisName,
                                LcheckCount = scheckCount + "(" + sPercentOfPass * 100 + "%)",
                                RcheckCount = rcheckCount + "(" + rPercentOfPass * 100 + "%)",
                            //   ScheckCount = scheckCount + "(" + sPercentOfPass * 100 + "%)",
                            PanterCount = PatientCount + "人(" + i.Year + "." + i.Month + ")",
                                Title = i.Year + "." + i.Month,
                                Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd")
                            });
                        }
                        j++;

                    }
                    if (InPut.Title != null)

                    {
                        Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                        List<string> tt = new List<string>();
                        foreach (var item in InPut.Title)
                        {
                            if (item.Value)
                                tt.Add(item.Key);
                        }
                        if (tt.Count > 0)
                            result = result.Where(t => tt.Contains(t.Title)).ToList();
                    }
                    int count = result.Count;
                    if (InPut.PageNum > 0 && InPut.PageSize > 0)
                        result = await PaginatedList<QualityHypertensionDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                    return new PageData<QualityHypertensionDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


        }

        /*
         ☆肾性贫血控制率
         血红蛋白≥100G/L的维持性血液透析患者占同期总维持性血液透析患者数量的比率
         ≥90%
         */
        //Hemoglobin
        //TODO:需要重新计算
        /// <summary>
        /// 肾贫血控率 - 图形
        /// </summary>
        public Task<LineOutPut> HemoglobinChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                //患者总数
                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                //患者上机记录
                var CheckData = await RoutineBloodRecordStore.Entities.Include(t => t.Patient).Where(t => t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();//患者上机记录

                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {
                            Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                            predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+2).AddDays(29));

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));

                            Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                            RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                            //检查人数  pa≥50%

                            var lcheckCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29) && t.Hemoglobin >= 100).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(Math.Round(PercentOfPass + ReturnTest(), 2));

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();
                    serise.name = "质控率";
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == InPut.CenterId && t.ReceiveDate.Value < i.AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数

                        var lcheckCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29) && t.Hemoglobin >= 100).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);

                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        //    serise.data.Add(PercentOfPass);
                        serise.data.Add(Math.Round(PercentOfPass + ReturnTest(), 2));
                        j++;

                    }
                    result.serises.Add(serise);
                    result.LegendData.Add("质控率");
                    // 
                }
                return result;
            });
        }

        /// <summary>
        /// 肾贫血质控率 - 表格
        /// </summary>
        public Task<PageData<HemoglobinDataOutPut[]>> HemoglobinDataQueryableAsync(QualityControlQueryInPut InPut)
        {

            return Task.Run(async () =>
            {
                List<HemoglobinDataOutPut> result = new List<HemoglobinDataOutPut>();
                bool IsDel = false;
                //   string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                //患者上机记录
                var LCheckData = await RoutineBloodRecordStore.Entities.Include(t => t.Patient).Where(t => t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();//患者上机记录


                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                {
                    foreach (var item in centerData)
                    {
                        //Expression<Func<Patient, bool>> predicate = (t => t.CenterDialysisId == item.Id);
                        //predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));

                        //Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        //RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数
                        var scheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();

                        var RcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29) && t.Hemoglobin >= 100).GroupBy(t => t.PatientId).Count();

                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double sPercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                        double rPercentOfPass = PatientCount == 0 ? 0 : Math.Round((RcheckCount * 1.00) / (PatientCount * 1.00), 2);

                        result.Add(new HemoglobinDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            CheckCount = scheckCount + "(" + sPercentOfPass * 100 + "%)",
                            RcheckCount = RcheckCount + "(" + rPercentOfPass * 100 + "%)",
                            //   ScheckCount = scheckCount + "(" + sPercentOfPass * 100 + "%)",
                            PanterCount = PatientCount + "人(" + i.Year + "." + i.Month + ")",
                            Title = i.Year + "." + i.Month,
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd")
                        });
                    }
                    j++;

                }
                if (InPut.Title != null)

                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<HemoglobinDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<HemoglobinDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });

        }

        /*
         血钙控制率	 电解质7项
         血钙水平在2.10-2.50MMOL/L的维持性血液透析患者占同期总维持性血液透析患者数量的比率	≥75%         
         */

        /// <summary>
        ///  血钙控制率 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetCaChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                bool IsDel = false;
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                var CheckData = await SevenElectrolyteTermsRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            //达标人数
                            var OkCount = CheckData.Where(t => t.CenterId == item.Id && t.Calcium >= Convert.ToDecimal(2.10) && t.Calcium < Convert.ToDecimal(2.50) && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<MineralAndBoneMetabolismRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));


                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.Calcium >= Convert.ToDecimal(2.10) && t.Calcium < Convert.ToDecimal(2.50) && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass + ReturnTest());
                        serise.name = "合格率";
                        //qualityControlDataOutPuts.Add(new QualityControlDataOutPut()
                        //{
                        //    CenterName = "康美佳",
                        //    checkCount = checkCount,
                        //    PanterCount = PatientCount,
                        //    PercentOfPass = PercentOfPass * 100 + "%",
                        //    Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
                        //});
                        j++;

                    }
                    result.LegendData.Add("合格率");
                    result.serises.Add(serise);
                    // 
                }

                return result;
            });
        }
        /// <summary>
        /// 血钙控制率 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetCaDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                var CheckData = await SevenElectrolyteTermsRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                {
                    foreach (var item in centerData)
                    {


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.CenterId == item.Id && t.Calcium >= Convert.ToDecimal(2.10) && t.Calcium < Convert.ToDecimal(2.50) && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = (i.Year + "." + i.Month);
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = OkCount + "(" + PercentOfPass * 100 + "%)",
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd"),
                            Title = tt,
                        });
                    }
                    j++;
                }
                if (InPut.Title != null)
                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }




        /*
       血磷控制率	电解质7项
       血磷水平在1.13-1.78MMOL/L的维持性血液透析患者占同期总维持性血液透析患者数量的比率
        */
        /// <summary>
        ///  血磷控制率 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetPChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                bool IsDel = false;
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                var CheckData = await SevenElectrolyteTermsRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            //达标人数
                            var OkCount = CheckData.Where(t => t.CenterId == item.Id && t.InorganicPhosphorus >= Convert.ToDecimal(1.13) && t.InorganicPhosphorus < Convert.ToDecimal(1.78) && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<MineralAndBoneMetabolismRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));


                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.InorganicPhosphorus >= Convert.ToDecimal(1.13) && t.InorganicPhosphorus < Convert.ToDecimal(1.78) && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass + ReturnTest());
                        serise.name = "合格率";
                        //qualityControlDataOutPuts.Add(new QualityControlDataOutPut()
                        //{
                        //    CenterName = "康美佳",
                        //    checkCount = checkCount,
                        //    PanterCount = PatientCount,
                        //    PercentOfPass = PercentOfPass * 100 + "%",
                        //    Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
                        //});
                        j++;

                    }
                    result.LegendData.Add("合格率");
                    result.serises.Add(serise);
                    // 
                }

                return result;
            });
        }
        /// <summary>
        /// 血磷控制率 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetPDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                var CheckData = await SevenElectrolyteTermsRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                {
                    foreach (var item in centerData)
                    {


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.CenterId == item.Id && t.InorganicPhosphorus >= Convert.ToDecimal(1.13) && t.InorganicPhosphorus < Convert.ToDecimal(1.78) && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = (i.Year + "." + i.Month);
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = OkCount + "(" + PercentOfPass * 100 + "%)",
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd"),
                            Title = tt,
                        });
                    }
                    j++;
                }
                if (InPut.Title != null)
                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }


        //☆IPTH控制率	IPTH水平在正常值上限2-9倍的维持性血液透析患者占同期总维持性血液透析患者数量的比率	≥50%

        /// <summary>
        /// IPTH水平在正常值上限2-9倍的维持性血液透析患者占同期总维持性血液透析患者数量的比率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetIPTHChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                //   string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                    var CheckData = await MineralAndBoneMetabolismRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {


                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            //达标人数
                            var OkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29) && t.ParathyroidHormone >= 0 && t.ParathyroidHormone < 100).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<MineralAndBoneMetabolismRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数
                        var data = await MineralAndBoneMetabolismRecordStore.Entities.Include(t => t.Patient).Where(RoutineBloodRecordpredicate).ToListAsync();
                        int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = data.Where(t => t.ParathyroidHormone >= 0 && t.ParathyroidHormone < 100).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass + ReturnTest());
                        serise.name = "合格率";
                        //qualityControlDataOutPuts.Add(new QualityControlDataOutPut()
                        //{
                        //    CenterName = "康美佳",
                        //    checkCount = checkCount,
                        //    PanterCount = PatientCount,
                        //    PercentOfPass = PercentOfPass * 100 + "%",
                        //    Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
                        //});
                        j++;

                    }
                    result.LegendData.Add("合格率");
                    result.serises.Add(serise);
                    // 
                }
                return result;
            });

        }
        /// <summary>
        /// IPTH水平在正常值上限2-9倍的维持性血液透析患者占同期总维持性血液透析患者数量的比率-- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetIPTHDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                var CheckData = await MineralAndBoneMetabolismRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                {
                    foreach (var item in centerData)
                    {


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.ParathyroidHormone >= 0 && t.ParathyroidHormone < 100).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = (i.Year + "." + i.Month);
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = OkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = PercentOfPass * 100 + "%",
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd"),
                            Title = tt,
                        });
                    }
                    j++;
                }
                if (InPut.Title != null)
                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }


        /*
         血清白蛋白控制率	
         血清白蛋白＞35G/L的维持性血液透析患者占同期总维持性血液透析患者数量的比率	
         ≥90%
         */
        /// <summary>
        ///  血清白蛋白控制率		 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetSerumChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                bool IsDel = false;
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                    var CheckData = await LiverFunctionBloodFatBoodSugarRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            //达标人数
                            var OkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29) && t.SerumAlbumin > 35).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<MineralAndBoneMetabolismRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数
                        var data = await MineralAndBoneMetabolismRecordStore.Entities.Include(t => t.Patient).Where(RoutineBloodRecordpredicate).ToListAsync();
                        int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = data.Count(t => t.ParathyroidHormone >= 0 && t.ParathyroidHormone < 100);
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass + ReturnTest());
                        serise.name = "合格率";
                        //qualityControlDataOutPuts.Add(new QualityControlDataOutPut()
                        //{
                        //    CenterName = "康美佳",
                        //    checkCount = checkCount,
                        //    PanterCount = PatientCount,
                        //    PercentOfPass = PercentOfPass * 100 + "%",
                        //    Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
                        //});
                        j++;

                    }
                    result.LegendData.Add("合格率");
                    result.serises.Add(serise);
                    // 
                }

                return result;
            });
        }
        /// <summary>
        ///  血清白蛋白控制率		 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetSerumDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                var CheckData = await LiverFunctionBloodFatBoodSugarRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                {
                    foreach (var item in centerData)
                    {


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));
                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.SerumAlbumin > 35 && t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = (i.Year + "." + i.Month);
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = OkCount + "(" + PercentOfPass * 100 + "%)",
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd"),
                            Title = tt,
                        });
                    }
                    j++;
                }
                if (InPut.Title != null)
                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }


        /*
         Kt/V和URR控制率	
         单室KT/V（SPKT/V）大于1.2且URR大于65%的维持性血液透析患者占同期总维持性血液透析患者数量的比率	
         ≥80%
         */
        /// <summary>
        ///  Kt/V和URR控制率	 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetKTVChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                bool IsDel = false;
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                var CheckData = await DialysisAdequacyRecordStore.Entities.ToListAsync();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));

                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.FounderDate >= i && t.FounderDate < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            //达标人数
                            var OkCount = CheckData.Where(t => t.KtV >= Convert.ToDecimal(1.2) && t.URR < 65 && t.CenterId == item.Id && t.FounderDate >= i && t.FounderDate < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<MineralAndBoneMetabolismRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数

                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.FounderDate >= i && t.FounderDate < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.KtV >= Convert.ToDecimal(1.2) && t.URR < 65 && t.CenterId == InPut.CenterId && t.FounderDate >= i && t.FounderDate < i.AddDays(29)).GroupBy(t => t.PatientId).Count();

                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass + ReturnTest());
                        serise.name = "合格率";
                        //qualityControlDataOutPuts.Add(new QualityControlDataOutPut()
                        //{
                        //    CenterName = "康美佳",
                        //    checkCount = checkCount,
                        //    PanterCount = PatientCount,
                        //    PercentOfPass = PercentOfPass * 100 + "%",
                        //    Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
                        //});
                        j++;

                    }
                    result.LegendData.Add("合格率");
                    result.serises.Add(serise);
                    // 
                }

                return result;
            });
        }
        /// <summary>
        ///  Kt/V和URR控制率	 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetKTVDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                var CheckData = await DialysisAdequacyRecordStore.Entities.ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                {
                    foreach (var item in centerData)
                    {


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.FounderDate >= i && t.FounderDate < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Where(t => t.KtV >= Convert.ToDecimal(1.2) && t.URR < 65 && t.CenterId == item.Id && t.FounderDate >= i && t.FounderDate < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = (i.Year + "." + i.Month);
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = OkCount + "(" + PercentOfPass * 100 + "%)",
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd"),
                            Title = tt,
                        });
                    }
                    j++;
                }
                if (InPut.Title != null)
                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }


        /*
         透析期间体重增长控制率	CurrentDialysisProgramsStore
         透析期间体重增长小于5%的维持性血液透析患者占同期总维持性血液透析患者数量的比率 ≥70%
         */
        /// <summary>
        /// 透析期间体重增长控制率 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetWeightGainChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                bool IsDel = false;
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                var CheckData = await CurrentDialysisProgramStore.Entities.ToListAsync();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.Date >= i && t.Date < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                            //达标人数 

                            var OkCount = CheckData.Where(t => t.CenterId == item.Id && t.Date >= i && t.Date < i.AddDays(29) && t.WeightGainRate < 5).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = checkCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (checkCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<MineralAndBoneMetabolismRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.Date >= i && t.Date < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        //达标人数 

                        var OkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.Date >= i && t.Date < i.AddDays(29) && t.WeightGainRate < 5).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double PercentOfPass = checkCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (checkCount * 1.00), 2);
                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass + ReturnTest());
                        serise.name = "合格率";
                        //qualityControlDataOutPuts.Add(new QualityControlDataOutPut()
                        //{
                        //    CenterName = "康美佳",
                        //    checkCount = checkCount,
                        //    PanterCount = PatientCount,
                        //    PercentOfPass = PercentOfPass * 100 + "%",
                        //    Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
                        //});
                        j++;

                    }
                    result.LegendData.Add("合格率");
                    result.serises.Add(serise);
                    // 
                }

                return result;
            });
        }
        /// <summary>
        /// 透析期间体重增长控制率 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetWeightGainDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                var CheckData = await CurrentDialysisProgramStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                {
                    foreach (var item in centerData)
                    {


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                        //检查人数
                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.Date >= i && t.Date < i.AddDays(29)).GroupBy(t => t.PatientId).Count();
                        //达标人数 

                        var OkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.Date >= i && t.Date < i.AddDays(29) && t.WeightGainRate < 5).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = checkCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (checkCount * 1.00), 2);
                        string tt = (i.Year + "." + i.Month);
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = OkCount + "(" + PercentOfPass * 100 + "%)",
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd"),
                            Title = tt,
                        });
                    }
                    j++;
                }
                if (InPut.Title != null)
                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }



        /*
         乙型肝炎和丙型肝炎的发病率	
         维持性血液透析患者中每年新增乙型肝炎和丙型肝炎占同期总维持性血液透析患者数量的比率	
         0
         */

        /// <summary>
        /// 乙型肝炎和丙型肝炎的发病率	图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetHepatitisBChartQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                LineOutPut result = new LineOutPut();
                bool IsDel = false;
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                int j = 1;
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                    var CheckData = await MineralAndBoneMetabolismRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {
                        serise serise = new serise();
                        serise.name = i.Year + "." + i.Month;
                        result.LegendData.Add(i.Year + "." + i.Month);
                        foreach (var item in centerData)
                        {

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                            //检查人数
                            var checkCount = CheckData.Count(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));
                            //达标人数
                            var OkCount = CheckData.Count(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29) && t.ParathyroidHormone >= 0 && t.ParathyroidHormone < 100);
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(1))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<MineralAndBoneMetabolismRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));

                        //检查人数
                        var data = await MineralAndBoneMetabolismRecordStore.Entities.Include(t => t.Patient).Where(RoutineBloodRecordpredicate).ToListAsync();
                        int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = data.Count(t => t.ParathyroidHormone >= 0 && t.ParathyroidHormone < 100);
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(i.Year + "." + i.Month);
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass + ReturnTest());
                        serise.name = "合格率";
                        //qualityControlDataOutPuts.Add(new QualityControlDataOutPut()
                        //{
                        //    CenterName = "康美佳",
                        //    checkCount = checkCount,
                        //    PanterCount = PatientCount,
                        //    PercentOfPass = PercentOfPass * 100 + "%",
                        //    Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
                        //});
                        j++;

                    }
                    result.LegendData.Add("合格率");
                    result.serises.Add(serise);
                    // 
                }

                return result;
            });
        }
        /// <summary>
        /// 乙型肝炎和丙型肝炎的发病率	 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetHepatitisBDataQueryableAsync(QualityControlQueryInPut InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPut() { BeginTime = DateTime.Now, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime.HasValue ? InPut.BeginTime : DateTime.Now;
                DateTime BeginTime = new DateTime(InPut.BeginTime.Value.Year, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                var CheckData = await MineralAndBoneMetabolismRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+1))
                {
                    foreach (var item in centerData)
                    {


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddDays(29));


                        //检查人数
                        var checkCount = CheckData.Count(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddDays(29));
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        //达标人数
                        var OkCount = CheckData.Count(t => t.ParathyroidHormone >= 0 && t.ParathyroidHormone < 100);
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((OkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = (i.Year + "." + i.Month);
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = OkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = PercentOfPass * 100 + "%",
                            Times = i.AddDays(DateTime.DaysInMonth(i.Year, i.Month) - 1).ToString("yyyy-MM-dd"),
                            Title = tt,
                        });
                    }
                    j++;
                }
                if (InPut.Title != null)
                {
                    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();
                    //   keyValuePairs =  InPut.Title;
                    string titleSate = InPut.Title.ToString().Trim();
                    List<string> tt = new List<string>();
                    foreach (var item in InPut.Title)
                    {
                        if (item.Value)
                            tt.Add(item.Key);
                    }
                    if (tt.Count > 0)
                        result = result.Where(t => tt.Contains(t.Title)).ToList();
                }
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }

    }
}
