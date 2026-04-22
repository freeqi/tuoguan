using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using CDGService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CDGService.WebAPI.Datas;
using CDGService.Data;
using Microsoft.Extensions.Options;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 过程质控
    /// </summary>
    public class QualityControlManager
    {
        private readonly IGetUserInfo _getUserInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly PatientsManager _PatientsManager;

        private IRepository<Log> LogStore => _unitOfWork.GetStore<Log>();
        private readonly DictionaryCode _dictionaryCode;




        /// <summary>
        /// 病人在院状态ID
        /// </summary>
        private string HState = "";
        /// <summary>
        /// 质控管理
        /// </summary>
        public QualityControlManager(IUnitOfWork unitOfWork, LogManager logManager, PatientsManager patientsManager, IGetUserInfo getUserInfo, IOptions<DictionaryCode> dictionaryCode)
        {
            _PatientsManager = patientsManager;
            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _dictionaryCode = dictionaryCode.Value;
            HState = _dictionaryCode.HStateId;
        }

        /// <summary>
        /// 血常规检查
        /// </summary>
        private IRepository<RoutineBloodRecord> RoutineBloodRecordStore => _unitOfWork.GetStore<RoutineBloodRecord>();

        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        /// <summary>
        /// 电解质
        /// </summary>
        private IRepository<SevenElectrolyteTermsRecord> SevenElectrolyteTermsRecordStore => _unitOfWork.GetStore<SevenElectrolyteTermsRecord>();
        /// <summary>
        /// 肾功能
        /// </summary>
        private IRepository<RenalFunctionRecord> RenalFunctionRecordStore => _unitOfWork.GetStore<RenalFunctionRecord>();
        /// <summary>
        /// 肝功、血脂
        /// </summary>
        private IRepository<LiverFunctionBloodFatBoodSugarRecord> LiverFunctionBloodFatBoodSugarRecordStore => _unitOfWork.GetStore<LiverFunctionBloodFatBoodSugarRecord>();
        /// <summary>
        /// 甲状腺
        /// </summary>
        private IRepository<MineralAndBoneMetabolismRecord> MineralAndBoneMetabolismRecordStore => _unitOfWork.GetStore<MineralAndBoneMetabolismRecord>();

        /// <summary>
        /// 尿素
        /// </summary>
        private IRepository<StageSummary> StageSummaryStore => _unitOfWork.GetStore<StageSummary>();
        /// <summary>
        /// C蛋白
        /// </summary>
        private IRepository<InspectionOtherRecord> InspectionOtherRecordStore => _unitOfWork.GetStore<InspectionOtherRecord>();
        private IRepository<IronParametersRecord> IronParametersRecordStore => _unitOfWork.GetStore<IronParametersRecord>();



        private IRepository<LaboratoryItemSetting> LaboratoryItemSettingStore => _unitOfWork.GetStore<LaboratoryItemSetting>();
        private IRepository<LaboratoryItemRecord> LaboratoryItemRecordStore => _unitOfWork.GetStore<LaboratoryItemRecord>();
        private IRepository<LaboratoryCategorySetting> LaboratoryCategorySettingStore => _unitOfWork.GetStore<LaboratoryCategorySetting>();

        private IRepository<DialysisAdequacyRecord> DialysisAdequacyRecordStore => _unitOfWork.GetStore<DialysisAdequacyRecord>();








        private double ReturnTest()
        {
            //double num = 0.80;
            return 0;
            //var rd = new Random();
            //return rd.Next(45, 100) * 1.00 / 100.00;
        }





        //血常规 3月- 95% QualityControlQueryInPut  QualityControlOutPut
        /// <summary>
        /// 血常规-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetQualityControlChartQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;

                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();
                var CheckData = await LaboratoryItemRecordStore.Entities.Include(t => t.itemSetting).Where(t => t.itemSetting.CategoryId.Contains("def9da45uAh0e3874150802960551937") && t.DataState == 1 && t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).OrderBy(t => t.SortNnm).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+3))
                    {
                        DateTime EndTime = i.AddMonths(+3).AddDays(-1);
                        serise serise = new serise();
                        serise.name = "第" + j + "季度";
                        result.LegendData.Add("第" + j + "季度");
                        foreach (var item in centerData)
                        {
                            Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                            predicate = predicate.And(t => t.ReceiveDate < i.AddMonths(+2).AddDays(29));

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                            int PatientCount = cenp.Count();



                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass);

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();


                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+3))
                    {
                        DateTime EndTime = i.AddMonths(+3).AddDays(-1);
                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, InPut.CenterId, panterData);
                        int PatientCount = cenp.Count();

                        Expression<Func<LaboratoryItemRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId && t.itemSetting.CategoryId == "def9da45uAh0e3874150802960551937");
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate >= i && t.CheckDate < i.AddMonths(+2).AddDays(29));

                        //检查人数
                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add("第" + j + "季度");
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        serise.data.Add(PercentOfPass);
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
        /// 血常规-- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetQualityControlDataQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).OrderBy(t => t.SortNnm).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();
                var CheckData = await LaboratoryItemRecordStore.Entities.Include(t => t.itemSetting).Where(t => t.itemSetting.CategoryId.Contains("def9da45uAh0e3874150802960551937") && t.DataState == 1 && t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();
                int j = 1;

                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+3))
                {
                    DateTime EndTime = i.AddMonths(+3).AddDays(-1);
                    foreach (var item in centerData)
                    {
                        //Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                        //predicate = predicate.And(t => t.ReceiveDate.Value < EndTime);

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数 (当前范围内最后一条)

                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                        int PatientCount = cenp.Count;
                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);

                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.ShortName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(第" + j + "季度)",
                            PercentOfPass = PercentOfPass * 100 + "%",
                            Times = EndTime.ToString("yyyy-MM-dd"),
                            Title = "第" + j + "季度",

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
                    result = await PaginatedList<QualityControlDataOutPut>.CreateAsync(result, InPut.PageNum, InPut.PageSize);
                return new PageData<QualityControlDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }

        /*
         MHD患者的血液生化定时检验完成率 
         电解质≥80%；肾功≥50%；肝功、血脂≥30%
         */
        //BloodBiochemical
        /// <summary>
        /// MHD患者的血液生化定时检验完成率  - 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetBloodBiochemicalChartQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                // string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;
                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();

                /*
                 
def9da41kS5HI3874150768152023040  电解质七项 
def9da42J8YVF3874501291715727361  肝功、血脂、血糖
def9da4Dx13PO3874150558730424320  肾功能
                 */

                var CheckData = await LaboratoryItemRecordStore.Entities.Include(t => t.itemSetting).Where(t => (t.itemSetting.CategoryId.Contains("def9da41kS5HI3874150768152023040") || t.itemSetting.CategoryId.Contains("def9da42J8YVF3874501291715727361") || t.itemSetting.CategoryId.Contains("def9da4Dx13PO3874150558730424320")) && t.DataState == 1 && t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();

                var SCheckData = CheckData.Where(t => t.itemSetting.CategoryId.Contains("def9da41kS5HI3874150768152023040")).ToList();//电解质
                var RCheckData = CheckData.Where(t => t.itemSetting.CategoryId.Contains("def9da4Dx13PO3874150558730424320")).ToList();//肾功
                var LCheckData = CheckData.Where(t => t.itemSetting.CategoryId.Contains("def9da42J8YVF3874501291715727361")).ToList();//肝功、血脂、血糖
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+3))
                    {
                        DateTime EndTime = i.AddMonths(+3).AddDays(-1);
                        serise serise = new serise();
                        serise.name = "第" + j + "季度";
                        result.LegendData.Add("第" + j + "季度");
                        foreach (var item in centerData)
                        {

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                            int PatientCount = cenp.Count;

                        
                            //检查人数  电解质≥80%；肾功≥50%；肝功、血脂≥30%
                            var scheckCount = SCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                            var rcheckCount = RCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                            var lcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                            PercentOfPass = (PatientCount == 0 ? 0 : Math.Round((rcheckCount * 1.00) / (PatientCount * 1.00), 2)) < PercentOfPass ? (PatientCount == 0 ? 0 : Math.Round((rcheckCount * 1.00) / (PatientCount * 1.00), 2)) : PercentOfPass;
                            PercentOfPass = (PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2)) < PercentOfPass ? (PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2)) : PercentOfPass;
                            serise.data.Add(PercentOfPass);

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();
                    serise.name = "合格率";
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+3))
                    {
                        DateTime EndTime = i.AddMonths(+3).AddDays(-1);


                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, InPut.CenterId, panterData);
                        int PatientCount = cenp.Count; 
                        //检查人数
                        var scheckCount = SCheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        var rcheckCount = RCheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        var lcheckCount = LCheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                        PercentOfPass  = (PatientCount == 0 ? 0 : Math.Round((rcheckCount * 1.00) / (PatientCount * 1.00), 2))< PercentOfPass? (PatientCount == 0 ? 0 : Math.Round((rcheckCount * 1.00) / (PatientCount * 1.00), 2)): PercentOfPass;
                        PercentOfPass = (PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2))< PercentOfPass? (PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2)): PercentOfPass;

                        result.data.Add("第" + j + "季度");
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        //    serise.data.Add(PercentOfPass);
                        serise.data.Add(PercentOfPass);
                        j++;

                    }
                    result.serises.Add(serise);
                    result.LegendData.Add("合格率");
                    // 
                }
                return result;
            });

        }

        /// <summary>
        /// MHD患者的血液生化定时检验完成率  -- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<BloodBiochemicalDataOutPut[]>> GetBloodBiochemicalDataQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                List<BloodBiochemicalDataOutPut> result = new List<BloodBiochemicalDataOutPut>();
                bool IsDel = false;
                //   string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }

                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();

                /*
                 
def9da41kS5HI3874150768152023040  电解质七项 
def9da42J8YVF3874501291715727361  肝功、血脂、血糖
def9da4Dx13PO3874150558730424320  肾功能
                 */
            
                var CheckData = await LaboratoryItemRecordStore.Entities.Include(t=>t.itemSetting).Where(t => (t.itemSetting.CategoryId.Contains("def9da41kS5HI3874150768152023040") || t.itemSetting.CategoryId.Contains("def9da42J8YVF3874501291715727361") || t.itemSetting.CategoryId.Contains("def9da4Dx13PO3874150558730424320")) && t.DataState == 1 && t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();
             
                var SCheckData = CheckData.Where(t=>t.itemSetting.CategoryId.Contains("def9da41kS5HI3874150768152023040")).ToList();//电解质
                var RCheckData = CheckData.Where(t => t.itemSetting.CategoryId.Contains("def9da4Dx13PO3874150558730424320")).ToList();//肾功
                var LCheckData = CheckData.Where(t => t.itemSetting.CategoryId.Contains("def9da42J8YVF3874501291715727361")).ToList();//肝功、血脂、血糖

                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+3))
                {
                    DateTime EndTime = i.AddMonths(+3).AddDays(-1);
                    foreach (var item in centerData)
                    {
                     

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                        int PatientCount = cenp.Count;
 
                        //检查人数
                        var scheckCount = SCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        var lcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        var rcheckCount = RCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double sPercentOfPass = PatientCount == 0 ? 0 : Math.Round((scheckCount * 1.00) / (PatientCount * 1.00), 2);
                        double lPercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);
                        double rPercentOfPass = PatientCount == 0 ? 0 : Math.Round((rcheckCount * 1.00) / (PatientCount * 1.00), 2);

                        result.Add(new BloodBiochemicalDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            LcheckCount = lcheckCount + "(" + lPercentOfPass * 100 + "%)",
                            RcheckCount = rcheckCount + "(" + rPercentOfPass * 100 + "%)",
                            ScheckCount = scheckCount + "(" + sPercentOfPass * 100 + "%)",
                            PanterCount = PatientCount + "人(第" + j + "季度)",
                            Title = "第" + j + "季度",
                            Times = i.AddMonths(+2).AddDays(29).ToString("yyyy-MM-dd")
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
                    result = await PaginatedList<BloodBiochemicalDataOutPut>.CreateAsync(result.OrderBy(t => t.CenterName).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<BloodBiochemicalDataOutPut[]>(result.ToArray(), count); //result.ToArray();// qualityControlOutPut;
            });


        }



        //MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成率	6月	-	≥20%顽固贫血者≥80%


        /// <summary>
        /// MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetFerritinChartQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                    var CheckData = await IronParametersRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        serise serise = new serise();
                        serise.name = j == 1 ? "上半年" : "下半年";
                        result.LegendData.Add(j == 1 ? "上半年" : "下半年");
                        foreach (var item in centerData)
                        {
                            Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                            predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                            Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                            RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<IronParametersRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                        //检查人数
                        var data = await IronParametersRecordStore.Entities.Include(t => t.Patient).Where(RoutineBloodRecordpredicate).ToListAsync();
                        int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(j == 1 ? "上半年" : "下半年");
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
        /// MHD患者的血清铁蛋白、转铁蛋白饱和度定时检验完成率-- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetFerritinDataQueryableAsync(QualityControlQueryInPuts InPut)
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
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
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
                var CheckData = await IronParametersRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                {
                    foreach (var item in centerData)
                    {
                        Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = j == 1 ? "上半年" : "下半年";
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = PercentOfPass * 100 + "%",
                            Times = i.AddMonths(+5).AddDays(29).ToString("yyyy-MM-dd"),
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




        //MHD患者HBV标志物和HCV标志物定时检验完成率	6月	100%	100%





        //MHD患者的全段甲状旁腺素（IPTH）定时检验完成率	6月	-	≥80%

        /// <summary>
        /// 全段甲状旁腺素（IPTH）定时检验完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetIPTHChartQueryableAsync(QualityControlQueryInPuts InPut)
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
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;

                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();
                var CheckData = await LaboratoryItemRecordStore.Entities.Include(t => t.itemSetting).Where(t => t.InspectionItemId == "def9da4Nl0Ca93875608020801163265" && t.DataState == 1 && t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
              
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        DateTime EndTime = i.AddMonths(+6).AddDays(-1);
                        serise serise = new serise();
                        serise.name = j == 1 ? "上半年" : "下半年";
                        result.LegendData.Add(j == 1 ? "上半年" : "下半年");
                        foreach (var item in centerData)
                        {
                            var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                            int PatientCount = cenp.Count;

                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id  && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        DateTime EndTime = i.AddMonths(+6).AddDays(-1);
                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, InPut.CenterId, panterData);
                        int PatientCount = cenp.Count;

                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId  && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(j == 1 ? "上半年" : "下半年");
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
        /// 全段甲状旁腺素（IPTH）定时检验完成率-- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetIPTHDataQueryableAsync(QualityControlQueryInPuts InPut)
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
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }


                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();
                var CheckData = await LaboratoryItemRecordStore.Entities.Include(t => t.itemSetting).Where(t => t.InspectionItemId =="def9da4Nl0Ca93875608020801163265" && t.DataState == 1 && t.CheckDate >= BeginTime && t.CheckDate < BeginTime.AddYears(1)).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                {
                    DateTime EndTime = i.AddMonths(+6).AddDays(-1);
                    foreach (var item in centerData)
                    {
                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                        int PatientCount = cenp.Count;

                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = j == 1 ? "上半年" : "下半年";
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = PercentOfPass * 100 + "%",
                            Times = i.AddMonths(+5).AddDays(29).ToString("yyyy-MM-dd"),
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


        //MHD患者的尿素清除指数（KT/V）、尿素下降率（URR）记录完成率	6月	-	≥80%

        /// <summary>
        /// 尿素清除指数（KT/V）、尿素下降率（URR）记录完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetURRChartQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;
                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();
                var CheckData = await DialysisAdequacyRecordStore.Entities.Where(t => t.DataState == 1 && t.RecordDate >= BeginTime && t.RecordDate < BeginTime.AddYears(1)).ToListAsync();

                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();                  
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        DateTime EndTime = i.AddMonths(+6).AddDays(-1);
                        
                        serise serise = new serise();
                        serise.name = j == 1 ? "上半年" : "下半年";
                        result.LegendData.Add(j == 1 ? "上半年" : "下半年");
                        foreach (var item in centerData)
                        {
                            var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                            int PatientCount = cenp.Count;
                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.RecordDate.Value >= i && t.RecordDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();
                    
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        DateTime EndTime = i.AddMonths(+6).AddDays(-1);
                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, InPut.CenterId, panterData);
                        int PatientCount = cenp.Count;
                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == InPut.CenterId && t.RecordDate.Value >= i && t.RecordDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(j == 1 ? "上半年" : "下半年");
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
        /// 尿素清除指数（KT/V）、尿素下降率（URR）记录完成率 -- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetURRDataQueryableAsync(QualityControlQueryInPuts InPut)
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
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                List<CenterDialysis> centerData = new List<CenterDialysis>();
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();

                }
                else
                {
                    centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel && t.Id == InPut.CenterId).ToListAsync();
                }
                // await DialysisAdequacyRecordStore.Entities.Include(t => t.PatientDar).Include(t => t.centerDar).Where(predicate).OrderBy(t => t.PatientId).OrderByDescending(t => t.RecordDate).ToArrayAsync();
                var panterData = await PatientStore.Entities.Include(t => t.TransferRecords).ToListAsync();
                var CheckData = await DialysisAdequacyRecordStore.Entities.Where(t =>  t.DataState == 1 && t.RecordDate >= BeginTime && t.RecordDate < BeginTime.AddYears(1)).ToListAsync();


                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                {
                    DateTime EndTime = i.AddMonths(+6).AddDays(-1);
                    foreach (var item in centerData)
                    {
                        Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        var cenp = await _PatientsManager.GetzyPatientsQueryableAsync(i, EndTime, item.Id, panterData);
                        int PatientCount = cenp.Count;
                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.RecordDate.Value >= i && t.RecordDate.Value < EndTime).GroupBy(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = j == 1 ? "上半年" : "下半年";
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = PercentOfPass * 100 + "%",
                            Times = i.AddMonths(+5).AddDays(29).ToString("yyyy-MM-dd"),
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

        //MHD患者的B2微球蛋白定时检验完成率	6月	-	50%  （暂不做要求）

        /// <summary>
        /// MHD患者的B2微球蛋白定时检验完成率 - 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetB2ChartQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                // string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;
                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                var LCheckData = await RenalFunctionRecordStore.Entities.Include(t => t.Patient).ToListAsync();//PA
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        serise serise = new serise();
                        serise.name = j == 1 ? "上半年" : "下半年";
                        result.LegendData.Add(j == 1 ? "上半年" : "下半年");
                        foreach (var item in centerData)
                        {
                            Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                            predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+2).AddDays(29));

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                            Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                            RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                            //检查人数  pa≥50%

                            var lcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(Math.Round(PercentOfPass / 3, 2) + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();
                    serise.name = "合格率";
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+2).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == InPut.CenterId && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                        //检查人数

                        var lcheckCount = LCheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);

                        result.data.Add(j == 1 ? "上半年" : "下半年");
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        //    serise.data.Add(PercentOfPass);
                        serise.data.Add(Math.Round(PercentOfPass / 3, 2) + ReturnTest());
                        j++;

                    }
                    result.serises.Add(serise);
                    result.LegendData.Add("合格率");
                    // 
                }
                return result;
            });

        }

        /// <summary>
        /// MHD患者的B2微球蛋白定时检验完成率  -- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetB2DataQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                // string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
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

                var LCheckData = await RenalFunctionRecordStore.Entities.Include(t => t.Patient).ToListAsync();//PA

                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                {
                    foreach (var item in centerData)
                    {
                        Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                        //检查人数

                        var lcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();

                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double lPercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = j == 1 ? "上半年" : "下半年";

                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = lcheckCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = lPercentOfPass * 100 + "%",
                            Times = i.AddMonths(+5).AddDays(29).ToString("yyyy-MM-dd"),
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




        //MHD患者的血清前白蛋白定时检验完成率	6月	-	50%   （暂不做要求）

        /// <summary>
        /// MHD患者血清前白蛋白定时检验完成率 - 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetPAChartQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                // string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;
                var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();

                var LCheckData = await LiverFunctionBloodFatBoodSugarRecordStore.Entities.Include(t => t.Patient).ToListAsync();//PA
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        serise serise = new serise();
                        serise.name = j == 1 ? "上半年" : "下半年";
                        result.LegendData.Add(j == 1 ? "上半年" : "下半年");
                        foreach (var item in centerData)
                        {
                            Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                            predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+2).AddDays(29));

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                            Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                            RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                            //检查人数  pa≥50%

                            var lcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(Math.Round(PercentOfPass / 3, 2) + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();
                    serise.name = "合格率";
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+2).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74

                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == InPut.CenterId && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                        //检查人数

                        var lcheckCount = LCheckData.Where(t => t.CenterId == InPut.CenterId && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);

                        result.data.Add(j == 1 ? "上半年" : "下半年");
                        //qualityControlChartOutPuts.Add(new QualityControlChartOutPut()
                        //{
                        //    name = "第" + j + "季度",
                        //    value = PercentOfPass,
                        //});

                        //    serise.data.Add(PercentOfPass);
                        serise.data.Add(Math.Round(PercentOfPass / 3, 2) + ReturnTest());
                        j++;

                    }
                    result.serises.Add(serise);
                    result.LegendData.Add("合格率");
                    // 
                }
                return result;
            });

        }

        /// <summary>
        /// MHD患者的血清前白蛋白定时检验完成率  -- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetPADataQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //   string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
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

                var LCheckData = await LiverFunctionBloodFatBoodSugarRecordStore.Entities.Include(t => t.Patient).ToListAsync();//PA

                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                {
                    foreach (var item in centerData)
                    {
                        Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                        //检查人数

                        var lcheckCount = LCheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();

                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();

                        double lPercentOfPass = PatientCount == 0 ? 0 : Math.Round((lcheckCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = j == 1 ? "上半年" : "下半年";

                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = lcheckCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = lPercentOfPass * 100 + "%",
                            Times = i.AddMonths(+5).AddDays(29).ToString("yyyy-MM-dd"),
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


        //MHD患者的C反映蛋白（CRP）定时检验完成率	6月		50% （暂不做要求）


        /// <summary>
        /// MHD患者的C反映蛋白（CRP）定时检验完成率-- 图形
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<LineOutPut> GetCRPChartQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                // string HState = "3f4eaafc1d134659bc323bd251041a74";
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();
                LineOutPut result = new LineOutPut();
                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
                int j = 1;
                if (InPut.CenterId + "" == "" || InPut.CenterId + "" == "0")
                {
                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    result.data = centerData.Select(t => t.DialysisName).ToList();
                    var panterData = await PatientStore.Entities.Where(t => t.IsDelete == IsDel && t.HospitalState == HState).ToListAsync();
                    var CheckData = await InspectionOtherRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {
                        serise serise = new serise();
                        serise.name = j == 1 ? "上半年" : "下半年";
                        result.LegendData.Add(j == 1 ? "上半年" : "下半年");
                        foreach (var item in centerData)
                        {
                            Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                            predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                            //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                            //患者总人数
                            int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                            Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                            RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                            //检查人数
                            var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29)).GroupBy(t => t.PatientId).Count();
                            // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                            double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                            serise.data.Add(PercentOfPass + ReturnTest());

                        }
                        result.serises.Add(serise);
                        j++;
                    }

                }
                else
                {
                    serise serise = new serise();

                    for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                    {

                        Expression<Func<Patient, bool>> predicate = t => t.IsDelete == IsDel;
                        predicate = predicate.And(t => t.HospitalState == HState);

                        predicate = predicate.And(t => t.CenterId == InPut.CenterId);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+5).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = PatientStore.Entities.Count(predicate);

                        Expression<Func<InspectionOtherRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+5).AddDays(29));

                        //检查人数
                        var data = await InspectionOtherRecordStore.Entities.Include(t => t.Patient).Where(RoutineBloodRecordpredicate).ToListAsync();
                        int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        result.data.Add(j == 1 ? "上半年" : "下半年");
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
        /// MHD患者的C反映蛋白（CRP）定时检验完成率-- 表格
        /// </summary>
        /// <param name="InPut"></param>
        /// <returns></returns>
        public Task<PageData<QualityControlDataOutPut[]>> GetCRPDataQueryableAsync(QualityControlQueryInPuts InPut)
        {
            return Task.Run(async () =>
            {
                List<QualityControlDataOutPut> result = new List<QualityControlDataOutPut>();
                bool IsDel = false;
                //  string HState = "3f4eaafc1d134659bc323bd251041a74";
                //TODO:  病人在院状态 字典ID  
                QualityControlOutPut qualityControlOutPut = new QualityControlOutPut();
                List<QualityControlChartOutPut> qualityControlChartOutPuts = new List<QualityControlChartOutPut>();
                List<QualityControlDataOutPut> qualityControlDataOutPuts = new List<QualityControlDataOutPut>();

                if (InPut == null)
                {
                    InPut = new QualityControlQueryInPuts() { BeginTime = DateTime.Now.Year, CenterId = "" };
                }
                InPut.BeginTime = InPut.BeginTime == 0 ? DateTime.Now.Year : InPut.BeginTime;
                DateTime BeginTime = new DateTime(InPut.BeginTime, 1, 1);
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
                var CheckData = await InspectionOtherRecordStore.Entities.Include(t => t.Patient).ToListAsync();
                int j = 1;
                for (DateTime i = BeginTime.Date.AddMonths(-BeginTime.Month + 1).AddDays(-BeginTime.Day + 1); i < BeginTime.Date.AddMonths(-BeginTime.Month + 12).AddDays(-BeginTime.Day + 31); i = i.AddMonths(+6))
                {
                    foreach (var item in centerData)
                    {
                        Expression<Func<Patient, bool>> predicate = (t => t.CenterId == item.Id);
                        predicate = predicate.And(t => t.ReceiveDate.Value < i.AddMonths(+2).AddDays(29));

                        //患者在院状态（转入/在院）：3f4eaafc1d134659bc323bd251041a74
                        //患者总人数
                        int PatientCount = panterData.Count(t => t.CenterId == item.Id && t.ReceiveDate.Value < i.AddMonths(+2).AddDays(29));

                        Expression<Func<RoutineBloodRecord, bool>> RoutineBloodRecordpredicate = (t => t.CenterId == InPut.CenterId);
                        RoutineBloodRecordpredicate = RoutineBloodRecordpredicate.And(t => t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+2).AddDays(29));

                        //检查人数
                        var checkCount = CheckData.Where(t => t.CenterId == item.Id && t.CheckDate.Value >= i && t.CheckDate.Value < i.AddMonths(+2).AddDays(29)).GroupBy(t => t.PatientId).Count();
                        // int checkCount = data.CompareDistinct(t => t.PatientId).Count();
                        double PercentOfPass = PatientCount == 0 ? 0 : Math.Round((checkCount * 1.00) / (PatientCount * 1.00), 2);
                        string tt = j == 1 ? "上半年" : "下半年";
                        result.Add(new QualityControlDataOutPut()
                        {
                            CenterName = item.DialysisName,
                            checkCount = checkCount,
                            PanterCount = PatientCount + "人(" + tt + ")",
                            PercentOfPass = PercentOfPass * 100 + "%",
                            Times = i.AddMonths(+5).AddDays(29).ToString("yyyy-MM-dd"),
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
