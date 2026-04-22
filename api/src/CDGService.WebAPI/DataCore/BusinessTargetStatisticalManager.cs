using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using CDGService.Data.Helper;
using CDGService.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Data.Enums;
using CDGService.WebAPI.Datas;
using CDGService.Store;
using System.Data.SqlClient;
using System.Data;
using static CDGService.WebAPI.DataCore.DataManager;

namespace CDGService.WebAPI.DataCore
{
    /// <summary>
    /// 财务报表统计
    /// </summary>
    public class BusinessTargetStatisticalManager : XmlSql
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly List<BusinessTarget> businessTargets;
        private readonly string _ClassName;
        public BusinessTargetStatisticalManager(IUnitOfWork unitOfWork, LogManager logManager, Microsoft.Extensions.Options.IOptions<List<BusinessTarget>> _BusinessTarget)
        {
            _ClassName = GetType().Name;//当前类名称
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            businessTargets = _BusinessTarget.Value;
        }
        /// <summary>
        /// 收入汇总表（按收费项目）
        /// </summary>
        private IRepository<DayItemIncome> DayItemIncomeStore => _unitOfWork.GetStore<DayItemIncome>();

        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<Employee> EmployeeStore => _unitOfWork.GetStore<Employee>();
        /// <summary>
        /// 透析例次
        /// </summary>
        private IRepository<CurrentDialysisProgram> CurrentDialysisProgramStore => _unitOfWork.GetStore<CurrentDialysisProgram>();
        private IRepository<HistoryDialysisRecords> HistoryDialysisRecordsStore => _unitOfWork.GetStore<HistoryDialysisRecords>();
        private IRepository<BatchNoSecondaryStoreroomDetail> BatchNoSecondaryStoreroomDetailStore => _unitOfWork.GetStore<BatchNoSecondaryStoreroomDetail>();
        private IRepository<CurePatternCheck> CurePatternCheckStore => _unitOfWork.GetStore<CurePatternCheck>();
        private IRepository<EquipmentInfo> EquipmentInfoStore => _unitOfWork.GetStore<EquipmentInfo>();

        private IRepository<SystemDictionary> SystemDictionaryStore => _unitOfWork.GetStore<SystemDictionary>();

        private IRepository<BalanceMain> BalanceMainStore => _unitOfWork.GetStore<BalanceMain>();
        private IRepository<BalanceDetails> BalanceDetailsStore => _unitOfWork.GetStore<BalanceDetails>();
        private IRepository<Prescription> PrescriptionStore => _unitOfWork.GetStore<Prescription>();
        private IRepository<PrescriptionDetail> PrescriptionDetailStore => _unitOfWork.GetStore<PrescriptionDetail>();
        private IRepository<SI_CFMX> SI_CFMXStore => _unitOfWork.GetStore<SI_CFMX>();


        private IRepository<SI_JSMX> SI_JSMXStore => _unitOfWork.GetStore<SI_JSMX>();
        private IRepository<WaterFuel> WaterFuelsStore => _unitOfWork.GetStore<WaterFuel>();

        private IRepository<BalancePayType> BalancePayTypeStore => _unitOfWork.GetStore<BalancePayType>();
        private IRepository<MedicalItemRecord> MedicalItemRecordStore => _unitOfWork.GetStore<MedicalItemRecord>();
        //  private IRepository<BatchNoSecondaryStoreroomDetail> BatchNoSecondaryStoreroomDetailStore => _unitOfWork.GetStore<BatchNoSecondaryStoreroomDetail>();
        public Task<BusinessTarget[]> GetBusinessTargets(int MainKey, int IndexKey)
        {

            return Task.Run(() =>
            {
                if (MainKey <= 0 || IndexKey <= 0)
                    throw new Exception("请提供有效的KEY");

                return businessTargets.Where(T => T.IndexKey == IndexKey && T.MainKey == MainKey).ToArray();
            });

        }

        /// <summary>
        /// 返回机构简称 如康美 弹子石
        /// </summary>
        /// <param name="centerName"></param>
        /// <returns></returns>
        private string ReturnCenttSortName(string centerName)
        {
            string name = centerName.Replace("透析中心", "");
            name = name.Replace("门诊部", "");
            return name;
        }


        #region 统计分析

        //医保报销利率表

        public Task<object> GetSIRatioAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                List<SIRatioOutPut> ListSIRatioOutPut = new List<SIRatioOutPut>();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                ALLSIRatioOutPut aLLDayIncomeOutPut = new ALLSIRatioOutPut();
                try
                {
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId == null || (inPut.CenterId != null && inPut.CenterId.Contains("0")))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    #region 表头
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Children = new List<TableHeader>() {
                                 new TableHeader(){    Key = PropertiesName, title= "金额"},
                                 new TableHeader(){    Key = PropertiesName+"ratios", title= "比率"},
                             }
                        });
                    }
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Children = new List<TableHeader>() {
                                 new TableHeader(){    Key = "total", title= "金额"},
                                 new TableHeader(){    Key = "totalratios", title= "比率"},
                             }
                    });
                    #endregion
                    #region 数据
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("1", "职工医保");
                    keyValuePairs.Add("4", "职工自费");
                    keyValuePairs.Add("2", "居民医保");
                    keyValuePairs.Add("5", "居民自费");
                    keyValuePairs.Add("3", "现金支付");
                    inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                    inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                    Expression<Func<BalanceMain, bool>> predicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 1);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    //结算总表
                    var BalanceData = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();
                    // 医保结算

                    Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();
                    foreach (var item in keyValuePairs)
                    {
                        SIRatioOutPut dayIncomeOutPut = new SIRatioOutPut();
                        dayIncomeOutPut.itemName = item.Value;
                        decimal? total = 0;
                        decimal? YBPrice = 0;
                        foreach (var items in CenterDialysiss)
                        {
                            /*
                             * TCZF = TCZF - MZJZ - YBZLZFS; 
                             总金额（ZJE） = 统筹支付（TCZF）+公务员补助（GWYBZ）+大额理赔（DELP） + 单病种定点医疗机构垫资（DBZDDYLJGDZ）+账户支付（ZHZF）+现金支付（XJZF）
                             */
                            //总额 = 
                            total += BalanceData.Where(t => t.CenterId == items.Id).Sum(t => t.SumPrice);
                            //自费
                            decimal? SelfPaySum = BalanceData.Where(t => t.BalanceType == 2 && t.CenterId == items.Id).Sum(t => t.SumPrice);
                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? Prices = 0;
                            List<string> SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Select(t => t.SIBalanceSerialNo).ToList();
                            /*
                   dayReportOutPut.mzbz = SiData.Where(t => t.CBLB == "1").Sum(t => t.MZJZ);//民政救助
                                                                                         //即实际的统筹支付应该是 TCZF - MZJZ - YBZLZFS (职工参保，统筹支付 =统筹支付，如果是居民参保，就需要减去民政救助)
                dayReportOutPut.detc = SiData.Where(t => t.CBLB == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));// 大额统筹 + SiData.Where(t => t.CBLB == "2").Sum(t =>( t.TCZF-t.MZJZ-t.YBZLZFS));
                dayReportOutPut.ybjj = SiData.Where(t => t.CBLB == "1").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                                                                                                     //居民
                             */
                            switch (item.Key)
                            {

                                case "1"://职工医保
                                    Prices = SiData.Where(t => t.CBLB == item.Key && t.CenterId == items.Id).Sum(t => (t.TCZF + t.GWYBZ + t.DBZDDYLJGDZ + t.DELP - t.YBZLZFS));
                                    break;
                                case "2"://居民医保
                                    Prices = SiData.Where(t => t.CBLB == item.Key && t.CenterId == items.Id).Sum(t => (t.TCZF + t.GWYBZ + t.DBZDDYLJGDZ + t.DELP - t.YBZLZFS));
                                    break;
                                case "4"://职工自费
                                    Prices = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Sum(t => t.XJZF + t.ZHZF);
                                    break;
                                case "5"://居民自费
                                    Prices = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Sum(t => t.XJZF + t.ZHZF);
                                    break;

                                case "3"://自费    SelfPaySum += SiData.Sum(t => t.XJZF);//现金支付 
                                    Prices = SelfPaySum + (BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE));
                                    break;
                                default:
                                    break;
                            }
                            YBPrice += Prices;
                            //  decimal? numbers = DetailsDatas.Where(t => t.MedicalItemRecord.FeeType.Name.Contains(item.Value) && t.CenterId == items.Id).Sum(t => t.TotalPrice);
                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, Prices);
                            decimal bili = BalanceData.Where(t => t.CenterId == items.Id).Sum(t => t.SumPrice) == 0 ? 0 : Math.Round((Prices / BalanceData.Where(t => t.CenterId == items.Id).Sum(t => t.SumPrice)).Value, 4) * 100;
                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName + "ratios").First().SetValue(dayIncomeOutPut, Math.Round(bili, 2) + "%");
                            //total += Convert.ToDecimal(dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().GetValue(dayIncomeOutPut));
                        }
                        dayIncomeOutPut.total = YBPrice;
                        dayIncomeOutPut.totalratios = Math.Round(Math.Round((YBPrice / total).Value, 4) * 100, 2) + "%";
                        ListSIRatioOutPut.Add(dayIncomeOutPut);
                    }
                    SIRatioOutPut sumdayIncomeOutPut = new SIRatioOutPut();
                    sumdayIncomeOutPut.itemName = "合计";
                    sumdayIncomeOutPut.kmtxzx = ListSIRatioOutPut.Sum(t => t.kmtxzx);

                    sumdayIncomeOutPut.xstxzx = ListSIRatioOutPut.Sum(t => t.xstxzx);
                    sumdayIncomeOutPut.dzsmzb = ListSIRatioOutPut.Sum(t => t.dzsmzb);
                    sumdayIncomeOutPut.jlptxzx = ListSIRatioOutPut.Sum(t => t.jlptxzx);
                    sumdayIncomeOutPut.tltxzx = ListSIRatioOutPut.Sum(t => t.tltxzx);
                    sumdayIncomeOutPut.spbtxzx = ListSIRatioOutPut.Sum(t => t.spbtxzx);
                    sumdayIncomeOutPut.fltxzx = ListSIRatioOutPut.Sum(t => t.fltxzx);

                    sumdayIncomeOutPut.zxtxzx = ListSIRatioOutPut.Sum(t => t.zxtxzx);

                    sumdayIncomeOutPut.tjmzb = ListSIRatioOutPut.Sum(t => t.tjmzb);
                    sumdayIncomeOutPut.total = sumdayIncomeOutPut.kmtxzx + sumdayIncomeOutPut.xstxzx + sumdayIncomeOutPut.dzsmzb + sumdayIncomeOutPut.jlptxzx + sumdayIncomeOutPut.tltxzx + sumdayIncomeOutPut.spbtxzx + sumdayIncomeOutPut.fltxzx + sumdayIncomeOutPut.zxtxzx + sumdayIncomeOutPut.tjmzb;
                    ListSIRatioOutPut.Add(sumdayIncomeOutPut);
                    #endregion
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.SIRatioOutPuts = ListSIRatioOutPut;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return (object)aLLDayIncomeOutPut;
            });

        }


        //患者人数统计表
        /// <summary>
        /// 患者人数统计表	分职工、居民并含增减情况 
        /// 按透析中心分月统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<PatientsReportOutPut[]> GetPatientsReportAsync(PatientsReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                List<PatientsReportOutPut> result = new List<PatientsReportOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                var Patients = await PatientStore.Entities.Where(t => t.IsDelete == false).AsNoTracking().ToListAsync();
                var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                int no = 1;
                foreach (var item in CenterDialysiss)
                {
                    PatientsReportOutPut put = new PatientsReportOutPut();
                    put.No = no;
                    put.CenterName = ReturnCenttSortName(item.ShortName);
                    put.Total = Patients.Count(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.CenterId == item.Id);
                    put.WorkerHealthCount = Patients.Count(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.SIInsuredType == "310" && t.CenterId == item.Id);
                    put.ResidentsHealthCount = Patients.Count(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.SIInsuredType == "390" && t.CenterId == item.Id);
                    put.OtherCount = Patients.Count(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.SIInsuredType != "310" && t.SIInsuredType != "390" && t.CenterId == item.Id);
                    put.NewCount = Patients.Count(t => t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.CenterId == item.Id && t.FounderDate.Value >= beginTime && t.FounderDate.Value < endTime);
                    put.LossCount = Patients.Count(t => (t.LeaveDate.HasValue) && (t.HospitalState != "3f4eaafc1d134659bc323bd251041a74" && t.CenterId == item.Id && t.LeaveDate.Value >= beginTime && t.LeaveDate.Value < endTime));
                    put.TurnoverRate = put.Total != 0 ? Math.Round(((put.LossCount * 1.000) / (put.Total * 1.000)), 3) * 100 + "%" : "0%";
                    result.Add(put);
                    no++;
                }
                // result = result.OrderByDescending(t => t.Total).ToList();
                result.Add(new PatientsReportOutPut()
                {
                    No = no,
                    CenterName = "合计",
                    LossCount = result.Sum(t => t.LossCount),
                    NewCount = result.Sum(t => t.NewCount),
                    OtherCount = result.Sum(t => t.OtherCount),
                    ResidentsHealthCount = result.Sum(t => t.ResidentsHealthCount),
                    Total = result.Sum(t => t.Total),
                    TurnoverRate = result.Sum(t => t.Total) != 0 ? Math.Round(((result.Sum(t => t.LossCount) * 1.000) / (result.Sum(t => t.Total) * 1.000)), 3) * 100 + "%" : "0%",
                    WorkerHealthCount = result.Sum(t => t.WorkerHealthCount),

                });

                return result.ToArray();
            });
        }


        //医护人员统计
        public Task<EmployeeReportOutPut[]> GetEmployeesReportAsync(EmployeeReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                List<EmployeeReportOutPut> result = new List<EmployeeReportOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                var Employees = await EmployeeStore.Entities.Where(t => t.IsDelete == false && t.WorkingState != "e81a52d7e88e49a28dd536548ca8fe8a").AsNoTracking().ToListAsync();
                var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                int no = 1;
                foreach (var item in CenterDialysiss)
                {
                    EmployeeReportOutPut put = new EmployeeReportOutPut();
                    put.No = no;
                    put.CenterName = ReturnCenttSortName(item.ShortName);
                    put.Total = Employees.Count(t => t.CenterDialysisId == item.Id);
                    put.DoctorCount = Employees.Count(t => t.CenterDialysisId == item.Id && (t.positionId == "21677ce204f8m6c7ad0d8c3a96c3b48e" || t.positionId == "d8c2c0dfa0e4458e9cedff7f0b08ed22"));
                    put.RnurseCount = Employees.Count(t => (t.positionId == "21677ce204f847c7ad228c3a96c3b48e" || t.positionId == "d7ecafb7409b4104855ab9fb0f262bc8") && t.CenterDialysisId == item.Id);
                    put.OtherCount = Employees.Count(t => t.CenterDialysisId == item.Id) - (put.DoctorCount + put.RnurseCount);
                    put.MedicalProportion = put.RnurseCount != 0 ? Math.Round(((put.DoctorCount * 1.000) / (put.RnurseCount * 1.000)), 3) * 100 + "%" : "0%";
                    put.RnurseProportion = Math.Round(((put.RnurseCount * 1.000) / (20.00)), 3) * 100 + "%";

                    result.Add(put);
                    no++;
                }
                result.Add(new EmployeeReportOutPut()
                {
                    CenterName = "合计",
                    No = no,
                    DoctorCount = result.Sum(t => t.DoctorCount),
                    RnurseCount = result.Sum(t => t.RnurseCount),
                    OtherCount = result.Sum(t => t.OtherCount),
                    MedicalProportion = Math.Round(((result.Sum(t => t.DoctorCount) * 1.000) / (result.Sum(t => t.RnurseCount) * 1.000)), 3) * 100 + "%",
                    Total = result.Sum(t => t.Total),
                    RnurseProportion = Math.Round(((result.Sum(t => t.RnurseCount) * 1.000) / (CenterDialysiss.Count * 20 * 1.000)), 3) * 100 + "%",


                });

                return result.ToArray();
            });
        }

        //设备 EquipmentInfoStore

        public Task<EquipmentReportOutPut[]> GetEquipmentReportAsync(EquipmentReportQueryInput inPut)
        {
            // GetCurePatternCheckReportAsync(null);
            return Task.Run(async () =>
            {
                List<EquipmentReportOutPut> result = new List<EquipmentReportOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                var Equipments = await EquipmentInfoStore.Entities.Where(t => t.IsDelete == false && t.EquipmentState != "1281344f7cae41e9867cbd4cfbabab18" && t.EquipType == "6f65d7538e0844c2bb9f8c92e650b493").AsNoTracking().ToListAsync(); //剔除已报废的。
                var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();

                //

                //var query = from l in CenterDialysiss
                //            group l by new { l.Id } into g  
                //            select new
                //            {
                //                Id = g.Key.Id, 
                //                Count = g.Count(),
                //            };
                int no = 1;

                foreach (var item in CenterDialysiss)
                {
                    EquipmentReportOutPut put = new EquipmentReportOutPut();
                    put.No = no;
                    put.CenterName = ReturnCenttSortName(item.ShortName);
                    put.Total = Equipments.Count(t => t.CenterId == item.Id); //
                    put.SWS4000Count = Equipments.Count(t => t.CenterId == item.Id && String.Equals(t.Model.Trim(), "SWS-4000", StringComparison.CurrentCultureIgnoreCase));
                    put.SWS4000ACount = Equipments.Count(t => t.CenterId == item.Id && String.Equals(t.Model.Trim(), "SWS-4000A", StringComparison.CurrentCultureIgnoreCase));
                    put.SWS6000Count = Equipments.Count(t => t.CenterId == item.Id && String.Equals(t.Model.Trim(), "SWS-6000", StringComparison.CurrentCultureIgnoreCase));
                    put.SWS6000ACount = Equipments.Count(t => t.CenterId == item.Id && String.Equals(t.Model.Trim(), "SWS-6000A", StringComparison.CurrentCultureIgnoreCase));
                    result.Add(put);
                    no++;
                }
                // result = result.OrderByDescending(t => t.Total).ToList();

                result.Add(new EquipmentReportOutPut()
                {
                    No = no,
                    CenterName = "合计",
                    Total = result.Sum(t => t.Total),
                    SWS6000Count = result.Sum(t => t.SWS6000Count),
                    SWS4000ACount = result.Sum(t => t.SWS4000ACount),
                    SWS4000Count = result.Sum(t => t.SWS4000Count),
                    SWS6000ACount = result.Sum(t => t.SWS6000ACount),

                });

                return result.ToArray();
            });
        }


        //治疗模式及治疗例次统计表（按月）		
        //CurePatternQueryInput CurePatternOutPut  CurePatternCheck
        public Task<ALLChargeCasesOutPut> GetCurePatternCheckReportAsync(CurePatternQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                ALLChargeCasesOutPut curePatternOutPuts = new ALLChargeCasesOutPut();
                try
                {

                    DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                    DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                    var CureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.FounderDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).AsNoTracking().ToListAsync();
                    var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    var SystemDictionarys = await SystemDictionaryStore.Entities.Where(t => t.TypeId == "e2d0fa22cc114841b86d7040ba144829" && t.IsDelete == false).AsNoTracking().ToListAsync();
                    //e2d0fa22cc114841b86d7040ba144829
                    List<CurePatternData> ListCurePatternData = new List<CurePatternData>();

                    //表头
                    List<TableHeader> tableHeaders = new List<TableHeader>();
                    tableHeaders.Add(new TableHeader() { title = "序号", Key = "no" });
                    tableHeaders.Add(new TableHeader() { title = "机构名称", Key = "centerName" });

                    //透析模式
                    var TempCureData = CureData.GroupBy(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType);
                    DataTable dt = new DataTable();
                    dt.Columns.Add("no", typeof(int));
                    dt.Columns.Add("centerName", typeof(string));

                    int i = 1;
                    int j = 1;

                    bool isadd = true;
                    foreach (var cen in CenterDialysiss)
                    {
                        DataRow dr = dt.NewRow();
                        dr["no"] = i;
                        dr["centerName"] = cen.ShortName;
                        foreach (var item in TempCureData)
                        {
                            //var isContainSex = dataDt.Columns.Contains("Sex");

                            TableHeader header = null;
                            if (tableHeaders.Where(t => t.title == item.Key).Count() <= 0)
                            {
                                isadd = true;
                                header = new TableHeader() { title = item.Key, Children = new List<TableHeader>() };
                            }
                            else
                            {
                                isadd = false;
                                header = tableHeaders.Where(t => t.title == item.Key).First();
                            }


                            //透析器型号
                            var diaData = item.ToList().GroupBy(t => t.patientCycleScheduling.currentDialysisProgram.Dialyzer);
                            foreach (var items in diaData)
                            {//dictionary.Name.Replace("+", "")
                                string dtcom = $"{item.Key.Replace("+", "_")}{items.Key.Replace("+", "_")}";
                                var isContain = dt.Columns.Contains(dtcom.ToLower());
                                if (!isContain)
                                {
                                    dt.Columns.Add(dtcom.ToLower(), typeof(int));
                                    header.Children.Add(new TableHeader() { title = items.Key, Key = dtcom.ToLower() });
                                }

                                int number = items.Where(t => t.CenterId == cen.Id).Count();
                                dr[dtcom] = number;

                                j++;
                            }

                            if (header != null && isadd)
                                tableHeaders.Add(header);
                        }
                        if (i == 1)
                        {
                            tableHeaders.Add(new TableHeader() { title = "合计", Key = "total" });
                            dt.Columns.Add("total", typeof(int));
                        }
                        dr["total"] = CureData.Where(t => t.CenterId == cen.Id).Count();
                        dt.Rows.Add(dr);
                        i++;

                    }

                    //合计
                    DataRow hjdr = dt.NewRow();
                    foreach (DataColumn item in dt.Columns)
                    {
                        if (item.ColumnName == "no")
                            hjdr["no"] = dt.Rows.Count + 1;
                        else if (item.ColumnName == "centerName")
                            hjdr["centerName"] = "合计";
                        else
                            hjdr[item.ColumnName] = dt.AsEnumerable().Sum(t => t.Field<int>(item.ColumnName));

                    }

                    dt.Rows.Add(hjdr);

                    curePatternOutPuts.tableHeaders = tableHeaders;
                    curePatternOutPuts.SiRatioOutPuts = dt;
                    return curePatternOutPuts;





                    //DateTime time = DateTime.Now.AddDays(-45);
                    //int no = 1;

                    //foreach (var item in CenterDialysiss)
                    //{
                    //    // CurePatternOutPut outPut = new CurePatternOutPut(); 
                    //    CurePatternData output = new CurePatternData();
                    //    output.no = no;
                    //    foreach (var dictionary in SystemDictionarys)
                    //    {
                    //        TableHeader header = new TableHeader() { title = dictionary.Name };
                    //        header.Children = new List<TableHeader>();
                    //        foreach (CureModelEnum e in Enum.GetValues(typeof(CureModelEnum)))
                    //        {
                    //            string PropertiesName = (dictionary.Name.Replace("+", "") + e.ToString().Substring(1)).ToLower();
                    //            output.CenterName = ReturnCenttSortName(item.ShortName);
                    //            output.Total = CureData.Where(t => t.CenterId == item.Id).Count();
                    //            if (output.GetType().GetProperties().Where(t => t.Name == PropertiesName).FirstOrDefault() != null)
                    //            {

                    //                if (i == 0)
                    //                {// 
                    //                    header.Children.Add(new TableHeader() { title = e.ToChinese(), Key = PropertiesName });
                    //                }
                    //                // Console.WriteLine(PropertiesName.ToLower());
                    //                //FileHelper.WriteLog("", PropertiesName.ToLower());
                    //                int number = CureData.Where(t => t.CenterId == item.Id && t.patientCycleScheduling.currentDialysisProgram.DialysisType == dictionary.Name && t.patientCycleScheduling.currentDialysisProgram.Dialyzer == e.ToChinese()).Count();
                    //                output.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(output, number);
                    //            }
                    //        }
                    //        if (i == 0)
                    //        {
                    //            tableHeaders.Add(header);
                    //        }

                    //    }
                    //    i = 1;

                    //    ListCurePatternData.Add(output);
                    //    no++;
                    //}
                    //// ListCurePatternData = ListCurePatternData.OrderByDescending(t => t.Total).ToList();
                    //ListCurePatternData.Add(new CurePatternData()
                    //{
                    //    no = no,
                    //    CenterName = "合计",
                    //    #region
                    //    hd150g = ListCurePatternData.Sum(t => t.hd150g),
                    //    hd15_lc = ListCurePatternData.Sum(t => t.hd15_lc),
                    //    hd15_uc = ListCurePatternData.Sum(t => t.hd15_uc),
                    //    hdfx8 = ListCurePatternData.Sum(t => t.hdfx8),
                    //    hdf15_uc = ListCurePatternData.Sum(t => t.hdf15_uc),
                    //    hdffx60 = ListCurePatternData.Sum(t => t.hdffx60),
                    //    hdffx80 = ListCurePatternData.Sum(t => t.hdffx80),
                    //    hdfx10 = ListCurePatternData.Sum(t => t.hdfx10),
                    //    hdfx80 = ListCurePatternData.Sum(t => t.hdfx80),
                    //    hdhp150g = ListCurePatternData.Sum(t => t.hdhp150g),
                    //    hdhp15_lc = ListCurePatternData.Sum(t => t.hdhp15_lc),
                    //    hdhpfx10 = ListCurePatternData.Sum(t => t.hdhpfx10),
                    //    hdhpfx80 = ListCurePatternData.Sum(t => t.hdhpfx80),
                    //    hdhpfx8 = ListCurePatternData.Sum(t => t.hdhpfx8),
                    //    hdhpps160 = ListCurePatternData.Sum(t => t.hdhpps160),
                    //    hdps160 = ListCurePatternData.Sum(t => t.hdps160),
                    //    hfhd15_uc = ListCurePatternData.Sum(t => t.hfhd15_uc),
                    //    hfhdfx60 = ListCurePatternData.Sum(t => t.hfhdfx60),
                    //    hfhdfx80 = ListCurePatternData.Sum(t => t.hfhdfx80),
                    //    scuf150g = ListCurePatternData.Sum(t => t.scuf150g),
                    //    hdffx600 = ListCurePatternData.Sum(t => t.hdffx600),
                    //    hdffx800 = ListCurePatternData.Sum(t => t.hdffx800),
                    //    hdfx60 = ListCurePatternData.Sum(t => t.hdfx60),
                    //    hdhp15_uc = ListCurePatternData.Sum(t => t.hdhp15_uc),
                    //    hdhpfx60 = ListCurePatternData.Sum(t => t.hdhpfx60),
                    //    hdhpha130 = ListCurePatternData.Sum(t => t.hdhpha130),
                    //    hdhpnv15t = ListCurePatternData.Sum(t => t.hdhpnv15t),
                    //    hdhpnv18t = ListCurePatternData.Sum(t => t.hdhpnv18t),
                    //    hdnv15t = ListCurePatternData.Sum(t => t.hdnv15t),
                    //    hdnv18t = ListCurePatternData.Sum(t => t.hdnv18t),
                    //    hfhd150g = ListCurePatternData.Sum(t => t.hfhd150g),
                    //    hfhd15_lc = ListCurePatternData.Sum(t => t.hfhd15_lc),
                    //    hfhdfx10 = ListCurePatternData.Sum(t => t.hfhdfx10),
                    //    hfhdnv15t = ListCurePatternData.Sum(t => t.hfhdnv15t),
                    //    hfhdnv18t = ListCurePatternData.Sum(t => t.hfhdnv18t),
                    //    hfhdps160 = ListCurePatternData.Sum(t => t.hfhdps160),
                    //    scuf15_lc = ListCurePatternData.Sum(t => t.scuf15_lc),
                    //    #endregion
                    //    Total = ListCurePatternData.Sum(t => t.Total)

                    //});
                    //tableHeaders.Add(new TableHeader() { title = "合计", Key = "total" });
                    //tableHeaders.RemoveAll(t => (t.Children != null && t.Children.Count == 0) && (t.title != "合计" || t.title != "机构名称"));
                    //curePatternOutPuts.tableHeaders = tableHeaders;
                    //curePatternOutPuts.curePatternDatas = ListCurePatternData;
                }
                catch (Exception exp)
                {

                    throw;
                }


                return curePatternOutPuts;

            });
        }

        //床位日均使用次数表（按月） BedsUseReportQueryInput BedsUseReportReportOutPut

        public Task<BedsUseReportReportOutPut[]> GetBedsUseReportAsync(BedsUseReportQueryInput inPut)
        {
            return Task.Run(async () =>
             {
                 List<BedsUseReportReportOutPut> reportOutPuts = new List<BedsUseReportReportOutPut>();
                 DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                 DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                 var Equipments = await EquipmentInfoStore.Entities.Where(t => t.IsDelete == false && t.EquipmentState == "eaf8cb315c9f4b409390e6a55bf57814" && t.EquipType == "285b4938d09d425d9bf2f5b0c6c657c1").AsNoTracking().ToListAsync(); //剔除已报废的。
                 var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                 var CureData = await HistoryDialysisRecordsStore.Entities.Where(t => t.FounderDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).AsNoTracking().ToListAsync();
                 int day = (endTime - beginTime).Days == 0 ? 1 : (endTime - beginTime).Days;
                 int no = 1;
                 foreach (var item in CenterDialysiss)
                 {
                     BedsUseReportReportOutPut bedsUse = new BedsUseReportReportOutPut();
                     bedsUse.BedsCount = Equipments.Where(t => t.CenterId == item.Id).Count();
                     bedsUse.CenterName = ReturnCenttSortName(item.ShortName);
                     bedsUse.no = no;
                     bedsUse.CurePattern = CureData.Where(t => t.CenterId == item.Id).Count();
                     bedsUse.DayCout = bedsUse.BedsCount == 0 ? 0 : Math.Round(
                     (bedsUse.CurePattern * 1.00) / (bedsUse.BedsCount * day), 2);
                     bedsUse.UsageRate = bedsUse.BedsCount == 0 ? "0%" : $"{Math.Round((bedsUse.DayCout * 1.00) / 2, 3) * 100}%";
                     reportOutPuts.Add(bedsUse);
                     no++;
                 }
                 //reportOutPuts.OrderByDescending(t => t.DayCout);
                 reportOutPuts.Add(new BedsUseReportReportOutPut
                 {
                     no = no,
                     BedsCount = Equipments.Count(),
                     CurePattern = CureData.Count(),
                     CenterName = "合计",
                     DayCout = Equipments.Count() == 0 ? 0 : Math.Round(
                     (CureData.Count() * 1.00) / (Equipments.Count() * day), 2),
                     UsageRate = $"{Math.Round(((CureData.Count * 1.00) / (Equipments.Count() * CenterDialysiss.Count() * day * 2)), 3) * 100}%"
                 });
                 return reportOutPuts.ToArray();
             });
        }

        //患者人均月收费及成本统计表 PatientSaverage PatientSaverageOutPut  PatientSaverageQuerInput
        /// <summary>
        /// 患者人均月收费及成本统计表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<PatientSaverageOutPut[]> GetPatientSaverageReportAsync(PatientSaverageQuerInput inPut)
        {
            return Task.Run(async () =>
            {
                // AddData();
                List<PatientSaverageOutPut> DataPuts = new List<PatientSaverageOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                var Patients = await PatientStore.Entities.Where(t => t.IsDelete == false && t.HospitalState == "3f4eaafc1d134659bc323bd251041a74" && t.ReceiveDate < inPut.EndReportDate).AsNoTracking().ToListAsync();
                //收入
                // var DayItemIncomeDatas = await DayItemIncomeStore.Entities.Where(t => t.SettlementDate >= beginTime && t.SettlementDate < endTime).AsNoTracking().ToListAsync();
                //成本 



                var CBData = await BatchNoSecondaryStoreroomDetailStore.Entities.Include(t => t.batchNoSecondaryStoreroom).ThenInclude(t => t.MedicalItemRecord).ThenInclude(t => t.FeeType).Where(t => t.OutInBoundType == "a7d83b6d81104475b7ede8ea467bab1f" && t.FounderDate >= beginTime && t.FounderDate < endTime && t.CenterId != "d0701c7d2cb242028445fca1034fb4ec").AsNoTracking().ToListAsync();
                Expression<Func<BalanceDetails, bool>> predicates = (t => t.CreateDate >= beginTime && t.CreateDate < endTime && t.CenterId != "d0701c7d2cb242028445fca1034fb4ec");

                var DetailsDatas = await BalanceDetailsStore.Entities.Include(t => t.MedicalItemRecord).ThenInclude(t => t.FeeType).Where(predicates).AsNoTracking().ToListAsync();


                int no = 1;
                foreach (var item in CenterDialysiss)
                {
                    PatientSaverageOutPut put = new PatientSaverageOutPut();
                    put.CenterName = ReturnCenttSortName(item.ShortName);
                    put.no = no;
                    //药品（中药、西药、药品8c8d1b15f8e8423aa5ba5550d83039b0 a4c3c759c20f4fa7b8c6b662b42808db cba9cbccc0764f27b4835a16433b1ae9）
                    List<string> DrugFlag = new List<string>()
                    { "8c8d1b15f8e8423aa5ba5550d83039b0",
                      "a4c3c759c20f4fa7b8c6b662b42808db",
                      "cba9cbccc0764f27b4835a16433b1ae9" };

                    int PatientsCount = Patients.Where(t => t.CenterId == item.Id).Count();
                    if (PatientsCount > 0)
                    {//.FeeType.Name.Contains("药") FeeType.Name.Contains("材料费")
                        decimal? Ynumber = DetailsDatas.Where(t => t.MedicalItemRecord.MedicalItemType == 1 && t.CenterId == item.Id).Sum(t => t.TotalPrice);
                        decimal? Cnumber = DetailsDatas.Where(t => t.MedicalItemRecord.MedicalItemType == 2 && t.CenterId == item.Id).Sum(t => t.TotalPrice);
                        decimal? Znumber = DetailsDatas.Where(t => t.CenterId == item.Id).Sum(t => t.TotalPrice) - (Ynumber + Cnumber);

                        //成本
                        decimal? CYnumber = CBData.Where(t => t.batchNoSecondaryStoreroom.MedicalItemRecord.MedicalItemType == 1 && t.CenterId == item.Id).Sum(t => t.batchNoSecondaryStoreroom.InPrice * t.OutInBoundQty);
                        decimal? CCnumber = CBData.Where(t => t.batchNoSecondaryStoreroom.MedicalItemRecord.MedicalItemType == 2 && t.CenterId == item.Id).Sum(t => t.batchNoSecondaryStoreroom.InPrice * t.OutInBoundQty);
                        // decimal? CZnumber = CBData.Where(t => t.CenterId == item.Id).Sum(t => t.TotalPrice) - (Ynumber+Cnumber);

                        put.DrugCharge = (Ynumber.Value / PatientsCount).ToRounds(4);//收益
                        put.MaterialCharge = (Cnumber.Value / PatientsCount).ToRounds(4);//收益
                        put.TreatmentCharge = (Znumber.Value / PatientsCount).ToRounds(4);
                        put.DrugEarnings = (CYnumber.Value / PatientsCount).ToRounds(4); //成本
                        put.MaterialEarnings = (CCnumber.Value / PatientsCount).ToRounds(4);
                        put.TreatmentEarnings = 0;// (DayItemIncomeDatas.Where(t => t.CenterId == item.Id).Sum(t => t.CostMoney) - (put.DrugEarnings + put.MaterialEarnings) / PatientsCount).ToRounds(4);
                        put.ChargeTotal = (DetailsDatas.Where(t => t.CenterId == item.Id).Sum(t => t.TotalPrice).Value / PatientsCount).ToRounds(4);
                        put.EarningsTotal = (CBData.Where(t => t.CenterId == item.Id).Sum(t => t.batchNoSecondaryStoreroom.InPrice * t.OutInBoundQty).Value / PatientsCount).ToRounds(4);
                    }
                    DataPuts.Add(put);
                    no++;
                }
                //  DataPuts = DataPuts.OrderByDescending(t => t.ChargeTotal).ToList();
                DataPuts.Add(new PatientSaverageOutPut()
                {
                    no = no,
                    CenterName = "合计",
                    ChargeTotal = DataPuts.Sum(t => t.ChargeTotal).ToRounds(4),
                    DrugCharge = DataPuts.Sum(t => t.DrugCharge).ToRounds(4),
                    DrugEarnings = DataPuts.Sum(t => t.DrugEarnings).ToRounds(4),
                    EarningsTotal = DataPuts.Sum(t => t.EarningsTotal).ToRounds(4),
                    MaterialCharge = DataPuts.Sum(t => t.MaterialCharge).ToRounds(4),
                    MaterialEarnings = DataPuts.Sum(t => t.MaterialEarnings).ToRounds(4),
                    TreatmentCharge = DataPuts.Sum(t => t.TreatmentCharge).ToRounds(4),
                    TreatmentEarnings = DataPuts.Sum(t => t.TreatmentEarnings).ToRounds(4),
                });

                return DataPuts.ToArray();
            });
        }

        //患者人均每例收费及成本(按透析、耗材、药品同时分HD、HDF、HD+HP分别计算每例收入成本)
        //Prescription 处方单（DialysisId） historyDialysisRecords 透析记录单

        public Task<PerCapitaDialysisOutPut[]> PerCapitaDialysis(CurePatternQueryInput inPut)
        {
            return Task.Run(async () =>
             {
                 List<PerCapitaDialysisOutPut> perCapitaDialysisOutPuts = new List<PerCapitaDialysisOutPut>();


                 DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                 DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                 try
                 {
                     var CenterData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.DataState == 1).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();

                     //透析模块
                     var CureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.FounderDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).AsNoTracking().ToListAsync();
                     //药品、耗材收费及成本
                     var ChargeData = await BatchNoSecondaryStoreroomDetailStore.Entities.Include(t => t.batchNoSecondaryStoreroom).ThenInclude(t => t.MedicalItemRecord).Include(T => T.materialOutboundDetail).ThenInclude(t => t.PrescriptionDetail).ThenInclude(t => t.prescriptions).Where(t => t.materialOutboundDetail.PrescriptionDetail.PrescriptionDate >= beginTime && t.materialOutboundDetail.PrescriptionDetail.PrescriptionDate < endTime && t.materialOutboundDetail.PrescriptionDetail.DataState == 1 && t.materialOutboundDetail.PrescriptionDetail.ChargeStatus == "2" && t.materialOutboundDetail.PrescriptionDetail.OutboundStatus == 2).AsNoTracking().ToListAsync();
                     // var ChargeData = await PrescriptionDetailStore.Entities.Include(t => t.materialOutboundDetail).ThenInclude(t => t.batchNoSecondaryStoreroomDetails).Where(t => t.PrescriptionDate >= beginTime && t.PrescriptionDate < endTime && t.DataState == 1 && t.ChargeStatus == "2" && t.OutboundStatus == 2).AsNoTracking().ToListAsync();

                     //诊疗项目 收入
                     var ZChargeData = await PrescriptionDetailStore.Entities.Include(t => t.prescriptions).Where(t => t.PrescriptionDate >= beginTime && t.PrescriptionDate < endTime && t.DataState == 1 && t.ChargeStatus == "2" && t.medicalItemRecord.MedicalItemType == 5).AsNoTracking().ToListAsync();
                     foreach (var item in CenterData)
                     {
                         PerCapitaDialysisOutPut put = new PerCapitaDialysisOutPut();

                         put.CenterName = ReturnCenttSortName(item.ShortName);
                         PerCapitaDialysisDetail Youtput = new PerCapitaDialysisDetail() { ItemName = "药品" };
                         PerCapitaDialysisDetail Houtput = new PerCapitaDialysisDetail() { ItemName = "耗材" };
                         PerCapitaDialysisDetail Zoutput = new PerCapitaDialysisDetail() { ItemName = "诊疗" };

                         PerCapitaDialysisDetail Soutput = new PerCapitaDialysisDetail() { ItemName = "合计" };//var hd = CureData.Where(t => t.CenterId == item.Id && t.patientCycleScheduling.currentDialysisProgram.DialysisType == "HD").ToList();
                         foreach (TreatmentModelEnum e in Enum.GetValues(typeof(TreatmentModelEnum)))
                         {

                             string PropertiesName = e.ToChinese().Replace("+", "").ToLower();

                             if (Youtput.GetType().GetProperties().Where(t => t.Name == (PropertiesName + "Cost")).FirstOrDefault() != null)
                             {
                                 decimal? YCost = 0;
                                 decimal? YIncome = 0;
                                 decimal? HCost = 0;
                                 decimal? HIncome = 0;
                                 decimal? ZCost = 0;
                                 decimal? ZIncome = 0;
                                 int count = 1;

                                 var hd = CureData.Where(t => t.CenterId == item.Id && t.patientCycleScheduling.currentDialysisProgram.DialysisType == e.ToChinese()).ToList();
                                 List<string> hdid = hd.Select(t => t.Id).ToList();
                                 if (hdid.Count > 0)
                                 {
                                     YCost = ChargeData.Where(t => t.CenterId == item.Id && t.batchNoSecondaryStoreroom.MedicalItemRecord.MedicalItemType == 1 && hdid.Contains(t.materialOutboundDetail.PrescriptionDetail.prescriptions.DialysisId)).Sum(t => t.OutInBoundQty * t.batchNoSecondaryStoreroom.InPrice);
                                     YIncome = ChargeData.Where(t => t.CenterId == item.Id && t.batchNoSecondaryStoreroom.MedicalItemRecord.MedicalItemType == 1 && hdid.Contains(t.materialOutboundDetail.PrescriptionDetail.prescriptions.DialysisId)).Sum(t => t.OutInBoundQty * t.batchNoSecondaryStoreroom.SalePrice);
                                     HCost = ChargeData.Where(t => t.CenterId == item.Id && t.batchNoSecondaryStoreroom.MedicalItemRecord.MedicalItemType == 2 && hdid.Contains(t.materialOutboundDetail.PrescriptionDetail.prescriptions.DialysisId)).Sum(t => t.OutInBoundQty * t.batchNoSecondaryStoreroom.InPrice);
                                     HIncome = ChargeData.Where(t => t.CenterId == item.Id && t.batchNoSecondaryStoreroom.MedicalItemRecord.MedicalItemType == 2 && hdid.Contains(t.materialOutboundDetail.PrescriptionDetail.prescriptions.DialysisId)).Sum(t => t.OutInBoundQty * t.batchNoSecondaryStoreroom.SalePrice);
                                     // ZCost = ChargeData.Where(t => t.CenterId == item.Id && t.batchNoSecondaryStoreroom.MedicalItemRecord.MedicalItemType == 1 && hdid.Contains(t.materialOutboundDetail.PrescriptionDetail.prescriptions.DialysisId)).Sum(t => t.OutInBoundQty * t.batchNoSecondaryStoreroom.InPrice);
                                     ZIncome = ZChargeData.Where(t => hdid.Contains(t.prescriptions.DialysisId)).Sum(t => t.ReceivablePrice);
                                 }
                                 if (hd.Count != 0) count = hd.Count;
                                 Youtput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Cost").First().SetValue(Youtput, (YCost / count).Value.ToRounds());
                                 Youtput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Income").First().SetValue(Youtput, (YIncome / count).Value.ToRounds());
                                 Houtput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Cost").First().SetValue(Houtput, (HCost / count).Value.ToRounds());
                                 Houtput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Income").First().SetValue(Houtput, (HIncome / count).Value.ToRounds());
                                 Zoutput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Cost").First().SetValue(Zoutput, (ZCost / count).Value.ToRounds());
                                 Zoutput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Income").First().SetValue(Zoutput, (ZIncome / count).Value.ToRounds());

                                 Soutput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Cost").First().SetValue(Soutput, (ZCost / count).Value.ToRounds() + (HCost / count).Value.ToRounds() + (YCost / count).Value.ToRounds());
                                 Soutput.GetType().GetProperties().Where(t => t.Name == PropertiesName + "Income").First().SetValue(Soutput, (ZIncome / count).Value.ToRounds() + (HIncome / count).Value.ToRounds() + (YIncome / count).Value.ToRounds());

                             }
                         }
                         put.perCapitaDialysisDetail.Add(Youtput);
                         put.perCapitaDialysisDetail.Add(Houtput);
                         put.perCapitaDialysisDetail.Add(Zoutput);
                         put.perCapitaDialysisDetail.Add(Soutput);

                         //put.perCapitaDialysisDetail.Add(new PerCapitaDialysisDetail()
                         //{
                         //    ItemName = "药品",
                         //    hdCost = ChargeData.Where(t => t.materialOutboundDetail.PrescriptionDetail.prescriptions),
                         //    hdIncome = 0,

                         //    hdfCost = 0,
                         //    hdfIncome = 0,

                         //    hdhpCost = 0,
                         //    hdhpIncome = 0,

                         //    hfhdCost = 0,
                         //    hfhdIncome = 0,

                         //    sucfCost = 0,
                         //    sucfIncome = 0,


                         //});
                         //put.perCapitaDialysisDetail.Add(new PerCapitaDialysisDetail()
                         //{
                         //    ItemName = "耗材",
                         //    hdCost = 0,
                         //    hdIncome = 0,
                         //    hdfCost = 0,
                         //    hdfIncome = 0,
                         //    hdhpCost = 0,
                         //    hdhpIncome = 0,
                         //    hfhdCost = 0,
                         //    hfhdIncome = 0,
                         //    sucfCost = 0,
                         //    sucfIncome = 0,


                         //});
                         //put.perCapitaDialysisDetail.Add(new PerCapitaDialysisDetail()
                         //{
                         //    ItemName = "诊疗",
                         //    hdCost = 0,
                         //    hdIncome = 0,
                         //    hdfCost = 0,
                         //    hdfIncome = 0,
                         //    hdhpCost = 0,
                         //    hdhpIncome = 0,
                         //    hfhdCost = 0,
                         //    hfhdIncome = 0,
                         //    sucfCost = 0,
                         //    sucfIncome = 0,


                         //});
                         perCapitaDialysisOutPuts.Add(put);
                     }
                 }
                 catch (Exception exp)
                 {
                     throw new Exception(exp.Message, exp);
                 }


                 return perCapitaDialysisOutPuts.ToArray();
             });
        }


        //医护人员人均月创收表		MedicalRevenueOutPut	 MedicalRevenueQuerInput


        public Task<MedicalRevenueOutPut[]> GetMedicalRevenueOutPutReportAsync(MedicalRevenueQuerInput inPut)
        {
            return Task.Run(async () =>
            {
                // AddData();
                List<MedicalRevenueOutPut> DataPuts = new List<MedicalRevenueOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                var Employees = await EmployeeStore.Entities.Where(t => t.IsDelete == false && t.WorkingState != "e81a52d7e88e49a28dd536548ca8fe8a").AsNoTracking().ToListAsync();
                Expression<Func<BalancePayType, bool>> predicates = (t => t.CreateDate >= beginTime && t.CreateDate < endTime && t.DataState == 1);
                //if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                //    predicates = predicates.And(t => inPut.CenterId.Contains(t.CenterId));
                //if (BalanceMainIds != null && BalanceMainIds.Count > 0)
                //    predicates = predicates.And(t => !BalanceMainIds.Contains(t.BalanceNo));
                var DayItemIncomeDatas = await BalancePayTypeStore.Entities.Where(predicates).AsNoTracking().ToListAsync();
                var CBData = await BatchNoSecondaryStoreroomDetailStore.Entities.Include(t => t.batchNoSecondaryStoreroom).ThenInclude(t => t.MedicalItemRecord).ThenInclude(t => t.FeeType).Where(t => t.OutInBoundType == "a7d83b6d81104475b7ede8ea467bab1f" && t.FounderDate >= beginTime && t.FounderDate < endTime).AsNoTracking().ToListAsync();
                //    var DayItemIncomeDatas = await DayItemIncomeStore.Entities.Where(t => t.SettlementDate >= beginTime && t.SettlementDate < endTime).AsNoTracking().ToListAsync();

                Expression<Func<BalanceMain, bool>> predicate = (t => t.BalanceDate >= beginTime && t.BalanceDate < endTime && t.BalanceState == 1);

                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();

                // 医保结算
                Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= beginTime && t.JSRQ < endTime);

                var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();


                foreach (var item in CenterDialysiss)
                {

                    List<string> SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id).Select(t => t.SIBalanceSerialNo).ToList();
                    var Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);
                    MedicalRevenueOutPut put = new MedicalRevenueOutPut();
                    put.CenterName = ReturnCenttSortName(item.ShortName);
                    //医护 （医生+护士+主任+护士长）
                    List<string> EmpFlag = new List<string>()
                    { "21677ce204f8m6c7ad0d8c3a96c3b48e",
                      "d8c2c0dfa0e4458e9cedff7f0b08ed22",
                      "21677ce204f847c7ad228c3a96c3b48e",
                      "d7ecafb7409b4104855ab9fb0f262bc8"};

                    int EmployeesCount = Employees.Where(t => t.CenterDialysisId == item.Id && EmpFlag.Contains(t.positionId)).Count();
                    if (EmployeesCount > 0)
                    {
                        decimal? CYnumber = CBData.Where(t => t.CenterId == item.Id).Sum(t => t.batchNoSecondaryStoreroom.InPrice * t.OutInBoundQty);
                        put.EarningsTotal = CYnumber.Value.ToRounds(4);
                        put.MedicalCount = EmployeesCount;
                        put.RevenueTotal = (DayItemIncomeDatas.Where(t => t.CenterId == item.Id).Sum(t => t.CostMoney.Value)).ToRounds(4) + (Prices.HasValue ? Prices.Value : 0);
                        put.PerCapita = (put.RevenueTotal / EmployeesCount).ToRounds(4);
                        put.ProfitTotal = put.RevenueTotal - put.EarningsTotal;
                        put.PerProfit = (put.ProfitTotal / put.MedicalCount).ToRounds(4).ToRounds(4);
                    }
                    DataPuts.Add(put);
                }
                DataPuts = DataPuts.OrderByDescending(t => t.RevenueTotal).ToList();
                DataPuts.Add(new MedicalRevenueOutPut()
                {
                    CenterName = "合计",

                    EarningsTotal = DataPuts.Sum(t => t.EarningsTotal).ToRounds(4),
                    MedicalCount = DataPuts.Sum(t => t.MedicalCount).ToRounds(4),
                    RevenueTotal = DataPuts.Sum(t => t.RevenueTotal).ToRounds(4),
                    PerCapita = (DataPuts.Sum(t => t.RevenueTotal).ToRounds(4) / DataPuts.Sum(t => t.MedicalCount).ToRounds(4)).ToRounds(4),
                    ProfitTotal = DataPuts.Sum(t => t.ProfitTotal).ToRounds(4),
                    PerProfit = (DataPuts.Sum(t => t.ProfitTotal).ToRounds(4) / DataPuts.Sum(t => t.MedicalCount).ToRounds(4)).ToRounds(4),

                });

                return DataPuts.ToArray();
            });
        }

        //单台机器月均收入表 DialysisMachineRevenueOutPut DialysisMachineRevenueInput        
        /// <summary>
        /// 单台机器月均收入表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<DialysisMachineRevenueOutPut[]> GetDialysisMachineRevenueReportAsync(DialysisMachineRevenueInput inPut)
        {
            return Task.Run(async () =>
            {
                // AddData();
                List<DialysisMachineRevenueOutPut> DataPuts = new List<DialysisMachineRevenueOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                //机构
                var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                //机器
                var Equipments = await EquipmentInfoStore.Entities.Where(t => t.IsDelete == false && t.EquipmentState != "1281344f7cae41e9867cbd4cfbabab18" && t.EquipType == "6f65d7538e0844c2bb9f8c92e650b493").AsNoTracking().ToListAsync(); //剔除已报废的。
                Expression<Func<BalancePayType, bool>> predicates = (t => t.CreateDate >= beginTime && t.CreateDate < endTime && t.DataState == 1);
                //if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                //    predicates = predicates.And(t => inPut.CenterId.Contains(t.CenterId));
                //if (BalanceMainIds != null && BalanceMainIds.Count > 0)
                //    predicates = predicates.And(t => !BalanceMainIds.Contains(t.BalanceNo));
                var DayItemIncomeDatas = await BalancePayTypeStore.Entities.Where(predicates).AsNoTracking().ToListAsync();
                //透析次数
                var CureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(t => t.FounderDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).AsNoTracking().ToListAsync();

                Expression<Func<BalanceMain, bool>> predicate = (t => t.BalanceDate >= beginTime && t.BalanceDate < endTime && t.BalanceState == 1);

                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();

                // 医保结算
                Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= beginTime && t.JSRQ < endTime);

                var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();

                foreach (var item in CenterDialysiss)
                {
                    List<string> SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id).Select(t => t.SIBalanceSerialNo).ToList();
                    var Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);

                    DialysisMachineRevenueOutPut put = new DialysisMachineRevenueOutPut();
                    put.CenterName = ReturnCenttSortName(item.ShortName);

                    int EquipmentsCount = Equipments.Where(t => t.CenterId == item.Id).Count();
                    if (EquipmentsCount > 0)
                    {
                        put.RevenueTotal = (DayItemIncomeDatas.Where(t => t.CenterId == item.Id).Sum(t => t.CostMoney).Value).ToRounds(4) + (Prices.HasValue ? Prices.Value : 0);
                        put.MachineCount = EquipmentsCount;
                        put.CurePattern = CureData.Where(t => t.CenterId == item.Id).Count();
                        put.MachineCapita = (put.RevenueTotal / put.MachineCount).ToRounds(4);

                    }
                    DataPuts.Add(put);
                }
                //  DataPuts = DataPuts.OrderByDescending(t => t.RevenueTotal).ToList();
                DataPuts.Add(new DialysisMachineRevenueOutPut()
                {
                    CenterName = "合计",
                    RevenueTotal = DataPuts.Sum(t => t.RevenueTotal).ToRounds(4),
                    CurePattern = DataPuts.Sum(t => t.CurePattern),
                    MachineCount = DataPuts.Sum(t => t.MachineCount),
                    MachineCapita = (DataPuts.Sum(t => t.MachineCapita) / DataPuts.Count).ToRounds(4),


                });

                return DataPuts.ToArray();
            });
        }

        //医护人员利用率  实际工作时间（治疗例次*4小时/例)/应工作时间（医护人数*21.75天*8小时)
        //SaturatedMedicalOutPut SaturatedMedicalInput

        /// <summary>
        /// 医护人员工作饱和率
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<SaturatedMedicalOutPut[]> GetSaturatedMedicalReportAsync(SaturatedMedicalInput inPut)
        {
            return Task.Run(async () =>
            {
                // AddData();
                List<SaturatedMedicalOutPut> DataPuts = new List<SaturatedMedicalOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                //机构
                var CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                //医护
                var Employees = await EmployeeStore.Entities.Where(t => t.IsDelete == false && t.WorkingState != "e81a52d7e88e49a28dd536548ca8fe8a").AsNoTracking().ToListAsync();
                //收入
                // var DayItemIncomeDatas = await DayItemIncomeStore.Entities.Where(t => t.SettlementDate >= beginTime && t.SettlementDate < endTime).AsNoTracking().ToListAsync();
                //透析次数
                var CureData = await HistoryDialysisRecordsStore.Entities.Where(t => t.FounderDate >= beginTime && t.FounderDate < endTime && t.DataState == 1).AsNoTracking().ToListAsync();
                int no = 1;
                foreach (var item in CenterDialysiss)
                {

                    SaturatedMedicalOutPut put = new SaturatedMedicalOutPut();
                    put.CenterName = ReturnCenttSortName(item.ShortName);
                    put.no = no;
                    //医护 （医生+护士+主任+护士长）
                    List<string> EmpFlag = new List<string>()
                    { "21677ce204f8m6c7ad0d8c3a96c3b48e",
                      "d8c2c0dfa0e4458e9cedff7f0b08ed22",
                      "21677ce204f847c7ad228c3a96c3b48e",
                      "d7ecafb7409b4104855ab9fb0f262bc8"
                    };

                    int EmployeesCount = Employees.Where(t => t.CenterDialysisId == item.Id && EmpFlag.Contains(t.positionId)).Count();
                    if (EmployeesCount > 0)
                    {
                        put.RevenueTotal = CureData.Where(t => t.CenterId == item.Id).Count() * 4;
                        put.MachineCount = EmployeesCount;
                        put.CurePattern = CureData.Where(t => t.CenterId == item.Id).Count();
                        put.MachineCapita = Convert.ToDecimal((EmployeesCount * 21.75 * 8)).ToRounds();
                        put.Saturation = ((put.RevenueTotal / put.MachineCapita).ToRounds(4)) * 100 + "%";
                    }
                    DataPuts.Add(put);
                    no++;
                }
                DataPuts = DataPuts.OrderByDescending(t => t.RevenueTotal).ToList();
                DataPuts.Add(new SaturatedMedicalOutPut()
                {
                    no = no,
                    CenterName = "合计",
                    RevenueTotal = DataPuts.Sum(t => t.RevenueTotal).ToRounds(4),
                    CurePattern = DataPuts.Sum(t => t.CurePattern),
                    MachineCount = DataPuts.Sum(t => t.MachineCount),
                    MachineCapita = DataPuts.Sum(t => t.MachineCapita),
                    Saturation = ((DataPuts.Sum(t => t.RevenueTotal).ToRounds(4) / DataPuts.Sum(t => t.MachineCapita)).ToRounds(4)) * 100 + "%",
                });

                return DataPuts.ToArray();
            });
        }

        //药占比
        //总收入、 药品收入（职工） 药占比（职工）    药品收入（居民） 药占比（居民）    
        /// <summary>
        /// 药占比
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetItemsSIRatioAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                string ItemType = "药品";
                switch (inPut.Model)
                {
                    case 1:

                        ItemType = "药品";
                        break;
                    case 2:
                        ItemType = "卫生耗材";
                        break;

                    case 3:
                        ItemType = "诊疗项目";
                        break;

                    default:
                        break;
                }


                List<SIRatioOutPut> ListSIRatioOutPut = new List<SIRatioOutPut>();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                ALLSIRatioOutPut aLLDayIncomeOutPut = new ALLSIRatioOutPut();
                try
                {
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId == null || (inPut.CenterId != null && inPut.CenterId.Contains("0")))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    #region 表头
                    tableHeaders.Add(new TableHeader()
                    {
                        title = inPut.EndTime.Month + "月",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });
                    }
                    //tableHeaders.Add(new TableHeader()
                    //{
                    //    title = "合计",
                    //    Children = new List<TableHeader>() {
                    //             new TableHeader(){    Key = "total", title= "金额"},
                    //             new TableHeader(){    Key = "totalratios", title= "比率"},
                    //         }
                    //});
                    #endregion
                    #region 数据
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("1", "职工总收入（元）");
                    keyValuePairs.Add("2", ItemType + "收入（职工）");
                    keyValuePairs.Add("3", ItemType + "占比（职工）");
                    keyValuePairs.Add("4", "居民总收入（元）");
                    keyValuePairs.Add("5", ItemType + "收入（居民）");
                    keyValuePairs.Add("6", ItemType + "占比（居民）");
                    inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                    inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                    Expression<Func<BalanceMain, bool>> predicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 1);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    //结算总表
                    var BalanceData = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();
                    // 医保结算

                    Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime && t.TFBZ == "1");
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();
                    decimal? total = 0;
                    foreach (var item in keyValuePairs)
                    {
                        SIRatioOutPut dayIncomeOutPut = new SIRatioOutPut();
                        dayIncomeOutPut.itemName = item.Value;

                        decimal? YBPrice = 0;
                        decimal? bili = 0;
                        foreach (var items in CenterDialysiss)
                        {
                            /*
                             * TCZF = TCZF - MZJZ - YBZLZFS;
                             总金额（ZJE） = 统筹支付（TCZF）+公务员补助（GWYBZ）+大额理赔（DELP） + 单病种定点医疗机构垫资（DBZDDYLJGDZ）+账户支付（ZHZF）+现金支付（XJZF）
                             */
                            //总额 = 
                            //   total = BalanceData.Where(t => t.CenterId == items.Id).Sum(t => t.SumPrice);
                            //自费
                            decimal? SelfPaySum = BalanceData.Where(t => t.BalanceType == 2 && t.CenterId == items.Id).Sum(t => t.SumPrice);
                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? Prices = 0;
                            List<string> SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Select(t => t.SIBalanceSerialNo).ToList();

                            switch (item.Key)
                            {
                                //总收入 药品收入（职工） 占比  药品收入（居民） 占比
                                case "1":
                                    var zlist = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var zBlist = BalanceData.Where(t => zlist.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    Prices = BalanceDetailsStore.Entities.Where(t => zBlist.Contains(t.BalanceNo) && t.CenterId == items.Id).Sum(t => t.TotalPrice);
                                    total = Prices;
                                    break;
                                case "2"://药品收入（职工） 
                                    var list = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var Blist = BalanceData.Where(t => list.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    Prices = BalanceDetailsStore.Entities.Where(t => Blist.Contains(t.BalanceNo) && t.CategoryName == ItemType && t.CenterId == items.Id).Sum(t => t.TotalPrice);

                                    break;
                                case "3"://职工占比

                                    var zlist1 = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var zBlist1 = BalanceData.Where(t => zlist1.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    total = BalanceDetailsStore.Entities.Where(t => zBlist1.Contains(t.BalanceNo) && t.CenterId == items.Id).Sum(t => t.TotalPrice);

                                    var list2 = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var Blist2 = BalanceData.Where(t => list2.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    var aPrices = BalanceDetailsStore.Entities.Where(t => Blist2.Contains(t.BalanceNo) && t.CategoryName == ItemType && t.CenterId == items.Id).Sum(t => t.TotalPrice);


                                    Prices = Math.Round((aPrices.Value / (total > 0 ? total.Value : 1)), 4);
                                    break;
                                case "4"://居民
                                    var jlist = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var jBlist = BalanceData.Where(t => jlist.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    Prices = BalanceDetailsStore.Entities.Where(t => jBlist.Contains(t.BalanceNo) && t.CenterId == items.Id).Sum(t => t.TotalPrice);

                                    break;
                                case "5"://居民
                                    var list1 = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var Blist1 = BalanceData.Where(t => list1.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    Prices = BalanceDetailsStore.Entities.Where(t => Blist1.Contains(t.BalanceNo) && t.CategoryName == ItemType && t.CenterId == items.Id).Sum(t => t.TotalPrice);

                                    bili = Math.Round((Prices.Value / (total > 0 ? total.Value : 1)), 4);
                                    break;
                                case "6"://占比
                                    var jlist1 = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var jBlist1 = BalanceData.Where(t => jlist1.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    total = BalanceDetailsStore.Entities.Where(t => jBlist1.Contains(t.BalanceNo) && t.CenterId == items.Id).Sum(t => t.TotalPrice);

                                    var list21 = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Select(t => t.JSJYLSH);
                                    var Blist21 = BalanceData.Where(t => list21.Contains(t.SIBalanceSerialNo) && t.CenterId == items.Id).Select(t => t.BalanceNo);
                                    var aPrices1 = BalanceDetailsStore.Entities.Where(t => Blist21.Contains(t.BalanceNo) && t.CategoryName == ItemType && t.CenterId == items.Id).Sum(t => t.TotalPrice);


                                    Prices = Math.Round((aPrices1.Value / (total > 0 ? total.Value : 1)), 4);
                                    break;

                                default:
                                    break;
                            }

                            //  decimal? numbers = DetailsDatas.Where(t => t.MedicalItemRecord.FeeType.Name.Contains(item.Value) && t.CenterId == items.Id).Sum(t => t.TotalPrice);
                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, Prices);

                            //decimal bili = BalanceData.Where(t => t.CenterId == items.Id).Sum(t => t.SumPrice) == 0 ? 0 : Math.Round((Prices / BalanceData.Where(t => t.CenterId == items.Id).Sum(t => t.SumPrice)).Value, 4) * 100;
                            //dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName + "ratios").First().SetValue(dayIncomeOutPut, Math.Round(bili, 2) + "%");
                            //total += Convert.ToDecimal(dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().GetValue(dayIncomeOutPut));
                        }

                        ListSIRatioOutPut.Add(dayIncomeOutPut);
                    }

                    #endregion
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.SIRatioOutPuts = ListSIRatioOutPut;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return (object)aLLDayIncomeOutPut;
            });

        }


        /// <summary>
        /// 患者透析列表
        /// </summary>
        /// <returns></returns>
        public Task<PatientDialysisOutPut[]> GetPatientDialysisAsync(PatientDialysisInput inPut)
        {
            return Task.Run(async () =>
            {

                List<PatientDialysisOutPut> outPuts = new List<PatientDialysisOutPut>();
                DateTime beginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                DateTime endTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                Expression<Func<HistoryDialysisRecords, bool>> predicate = (t => t.FounderDate >= beginTime && t.FounderDate < endTime && t.DataState == 1);
                if (inPut.CenterId != null && inPut.CenterId != "0")
                    predicate = predicate.And(t => inPut.CenterId == (t.CenterId));

                var CureData = await HistoryDialysisRecordsStore.Entities.Include(t => t.patient).Include(t => t.centerDialysis).Include(t => t.patientCycleScheduling).ThenInclude(t => t.currentDialysisProgram).Where(predicate).AsNoTracking().ToListAsync();
                int No = 1;
                var temp = CureData.GroupBy(t => t.PatientId);

                foreach (var item in temp)
                {

                    outPuts.Add(new PatientDialysisOutPut()
                    {
                        No = No,
                        PrantName = item.First().patient.Name,
                        hd = item.Where(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == "HD").Count(),
                        hdf = item.Where(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == "HDF").Count(),
                        hd_hp = item.Where(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == "HD+HP").Count(),
                        hfhd = item.Where(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == "HFHD").Count(),
                        hf = item.Where(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == "HF").Count(),
                        scuf = item.Where(t => t.patientCycleScheduling.currentDialysisProgram.DialysisType == "SCUF").Count(),
                        count = item.Count(),
                    });
                    No++;
                }
                if (outPuts.Count > 0)
                    outPuts.Add(new PatientDialysisOutPut()
                    {
                        No = No,
                        PrantName = "合计",
                        hd = outPuts.Sum(t => t.hd),
                        hdf = outPuts.Sum(t => t.hdf),
                        hd_hp = outPuts.Sum(t => t.hd_hp),
                        hfhd = outPuts.Sum(t => t.hfhd),
                        hf = outPuts.Sum(t => t.hf),
                        scuf = outPuts.Sum(t => t.scuf),
                        count = outPuts.Sum(t => t.count),
                    });
                return outPuts.ToArray();
            });
        }


        //2021年1月12日11:11:00 新增医占比表
        //分职工、居民统计

        /// <summary>
        /// 门诊医占比
        /// </summary>
        /// <param name="inPut"></param>
        public Task<object> MedicalProportionAsync(CostSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                inPut.BeginTime = new DateTime(inPut.BeginTime.Value.Year, inPut.BeginTime.Value.Month, inPut.BeginTime.Value.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Value.Year, inPut.EndTime.Value.Month, inPut.EndTime.Value.Day, 23, 59, 59);

                ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                int NoIndex = 1;

                //表头
                #region  表头
                List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                if (inPut.CenterId.Contains("0"))
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                else
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                tableHeaders.Add(new TableHeader()
                {
                    title = "序号",
                    Key = "no",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "医保类别",
                    Key = "itemMainName",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "项目",
                    Key = "itemName",
                });
                foreach (var item in CenterDialysiss)
                {
                    string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = ReturnCenttSortName(item.ShortName),
                        Key = PropertiesName,
                    });
                }
                aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                #endregion

                List<string> ItemGroup = new List<string>() { "总医疗费", "医保可报销的总医疗费", "药品费", "药占比（占总医疗费用）", "治疗费、材料费", "治疗材料比（占总医疗费用）", "患者自费额", "患者自费额占比（占总医疗费用）", "月就诊人数", "当月新增患者数", "当月人均费用" };
                //SI_CFMX 处方明细 BalanceMain 结算主表  SI_JSMX 医保结算主表
                //患者人
                var Patients = await PatientStore.Entities.Where(t => t.IsDelete == false).AsNoTracking().ToListAsync();


                Expression<Func<BalanceMain, bool>> manpredicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    manpredicate = manpredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                manpredicate = manpredicate.And(t => t.CancelDate == null || t.CancelDate > inPut.EndTime);
                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(manpredicate).AsNoTracking().ToListAsync();



                //跨月退费数据
                var strMainId = BalanceData.Where(t => t.BalanceState == 0).Select(t => t.BalanceNo);
                //跨月退费数据
                var CancelData = BalanceData.Where(t => t.CancelFlag == 1).Select(t => t.SISourceBalanceNo).ToList();

                Expression<Func<SI_JSMX, bool>> Sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime && t.TFBZ == "1");
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    Sipredicate = Sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                Sipredicate = Sipredicate.Or(t => CancelData.Contains(t.JSJYLSH));
                /*
                       var querb = from q in Maiindata
                                join ac in SI_YPMLSStore.Entities on q.Medical.HiCenterCode equals ac.YPLSH into ud
                                from ud1 in ud.DefaultIfEmpty()
                                select new { q, ud1 };
                 
                 */
                //医保结算总表
                var SIData = await SI_JSMXStore.Entities.Where(Sipredicate).AsNoTracking().ToListAsync();
                // 
                var SILSH = SIData.Select(T => T.JSJYLSH);
                //医保处方明细
                var SICF = await SI_CFMXStore.Entities.Where(T => SILSH.Contains(T.JSJYLSH)).Join(MedicalItemRecordStore.Entities, a => a.YYNM, b => b.MedicalItemCode, (a, b) => new { CFMX = a, MedItem = b }).ToArrayAsync();

                var CFData = SICF.Select(a => { var data = a.CFMX; data.MedicalItem = a.MedItem; return data; }).ToList();
                List<CenterDayIncomeOutPut> centerDayIncomeOutPuts = new List<CenterDayIncomeOutPut>();
                int j = 1;
                for (int i = 1; i < 3; i++)
                {
                    string cblb = i == 1 ? "310" : "390";
                    var silsh = SIData.Where(t => t.CBLB == cblb).Select(T => T.JSJYLSH);
                    var mxData = CFData.Where(t => silsh.Contains(t.JSJYLSH)).ToList();

                    foreach (var item in ItemGroup)
                    {
                        CenterDayIncomeOutPut put = new CenterDayIncomeOutPut();
                        put.itemName = item;

                        foreach (var items in CenterDialysiss)
                        {
                            decimal? PaySum = SIData.Where(t => t.CenterId == items.Id && t.CBLB == cblb).Sum(t => t.ZJE);
                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? Prices = 0;
                            switch (item)
                            {
                                case "总医疗费":
                                    put.itemMainName = cblb == "310" ? "职工医保" : "居民医保";
                                    Prices = PaySum;
                                    break;

                                case "医保可报销的总医疗费":
                                    Prices = SIData.Where(t => t.CenterId == items.Id && t.CBLB == cblb).Sum(t => t.BCFHYBFWFY);
                                    break;
                                case "药品费":
                                    Prices = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType == 1).Sum(t => (t.JE - t.ZFJE));
                                    break;
                                case "药占比（占总医疗费用）":
                                    var Prices1 = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType == 1).Sum(t => (t.JE - t.ZFJE));
                                    Prices = (Prices1 / PaySum).Value.ToRounds(4);
                                    break;
                                case "治疗费、材料费":
                                    Prices = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType != 1).Sum(t => (t.JE - t.ZFJE));
                                    break;
                                case "治疗材料比（占总医疗费用）":
                                    var Prices2 = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType != 1).Sum(t => (t.JE - t.ZFJE));
                                    Prices = (Prices2 / PaySum).Value.ToRounds(4);
                                    break;

                                case "患者自费额":
                                    Prices = mxData.Where(t => t.CenterId == items.Id).Sum(t => t.ZFJE);
                                    break;
                                case "患者自费额占比（占总医疗费用）":
                                    var Prices3 = mxData.Where(t => t.CenterId == items.Id).Sum(t => t.ZFJE);
                                    Prices = (Prices3 / PaySum).Value.ToRounds(4);
                                    break;
                                case "月就诊人数":
                                    Prices = SIData.Where(t => t.CBLB == cblb && t.CenterId == items.Id).GroupBy(t => t.SHBZH).Count();

                                    break;
                                case "当月新增患者数":

                                    Prices = Patients.Where(t => t.CenterId == items.Id && t.ReceiveDate < inPut.EndTime && t.ReceiveDate >= inPut.BeginTime && t.SIInsuredType == cblb).Count();
                                    break;
                                //( BalanceData.Where(t => t.SISourceBalanceNo != null).Sum(t => t.SumPrice)) -( SiData.Where(t=>t.TFBZ =="1").Sum(t => t.ZJE)+ SiData.Where(t => CancelData.Contains(t.JSJYLSH)).Sum(t => t.ZJE));
                                case "当月人均费用":

                                    var Prices4 = SIData.Where(t => t.CBLB == cblb && t.CenterId == items.Id).GroupBy(t => t.SHBZH).Count();

                                    Prices = (PaySum / Prices4).Value.ToRounds(4);
                                    break;
                                default:
                                    break;
                            }
                            put.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(put, Prices);
                        }

                        put.no = j;
                        centerDayIncomeOutPuts.Add(put);
                        j++;
                    }
                }

                //总费用
                //  var TotolPrice = BalanceData.Sum(t => t.SumPrice);
                //医保可报销总费用 （）

                //职工
                //居民

                //患者人数
                //当月新增患者
                aLLDayIncomeOutPut.CenterDayIncomeOutPuts = centerDayIncomeOutPuts;
                return (object)aLLDayIncomeOutPut;

            });
        }

        #endregion

        #region 收支报表

        /// <summary>
        /// 收入汇总表（按收费项目统计）
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetBusinessTargetsAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                //  List<IncomeSummaryOutPut> result = new List<IncomeSummaryOutPut>();
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                int NoIndex = 1;

                //Expression<Func<BalanceMain, bool>> predicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 0);
                //if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                //    predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));

                //结算主表 筛选以退费项目  2019年6月11日14:33:28 确认退费的不剔除
                //  List<string> BalanceMainIds = await BalanceMainStore.Entities.Where(predicate).Select(t => t.BalanceNo).AsNoTracking().ToListAsync();

                Expression<Func<BalanceMain, bool>> manpredicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    manpredicate = manpredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                manpredicate = manpredicate.And(t => t.CancelDate == null || t.CancelDate > inPut.EndTime);
                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(manpredicate).AsNoTracking().ToListAsync();

                //跨月退费数据
                var strMainId = BalanceData.Where(t => t.BalanceState == 0).Select(t => t.BalanceNo);

                //明细
                Expression<Func<BalanceDetails, bool>> predicates = (t => t.OpenDateTime >= inPut.BeginTime && t.OpenDateTime < inPut.EndTime && t.DataState == 1);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    predicates = predicates.And(t => inPut.CenterId.Contains(t.CenterId));
                if (strMainId != null && strMainId.Count() > 0)
                    predicates = predicates.Or(t => strMainId.Contains(t.BalanceNo));
                var DetailsDatas = await BalanceDetailsStore.Entities.Include(t => t.MedicalItemRecord).ThenInclude(t => t.FeeType).Where(predicates).AsNoTracking().ToListAsync();

                //var desum = DetailsDatas.Where(t=>t.CenterId == "abc3d60b474c4fef946a66887348c41a").Sum(t => t.TotalPrice);
                //var dsum = BalanceData.Where(t => t.CenterId == "abc3d60b474c4fef946a66887348c41a").Sum(t => t.SumPrice);
                // 医保结算
                //跨月退费数据
                var CancelData = BalanceData.Where(t => t.CancelFlag == 1).Select(t => t.SISourceBalanceNo).ToList();
                Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime);
                sipredicate = sipredicate.Or(t => CancelData.Contains(t.JSJYLSH));
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));


                var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();




                //var data = DetailsDatas.Where(t => t.MedicalItemRecord == null || t.MedicalItemRecord.FeeType == null);
                //Expression<Func<DayItemIncome, bool>> predicate = t => t.SettlementDate >= inPut.BeginTime && t.SettlementDate <= inPut.EndTime;
                //if (inPut.CenterId + "" != "" && inPut.CenterId + "" != "0")
                //    predicate = predicate.And(t => t.CenterId == inPut.CenterId);

                //var DayMoney = await DayItemIncomeStore.Entities.Include(t => t.centerDialysis).Include(t => t.SysDicCost).Where(predicate).AsNoTracking().ToListAsync();
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("药品费", "药");
                keyValuePairs.Add("材料费", "材料费");
                keyValuePairs.Add("检查费", "检");
                keyValuePairs.Add("治疗费", "治疗费");
                keyValuePairs.Add("透析费", "透析治疗费");
                keyValuePairs.Add("护理费", "护理费");
                keyValuePairs.Add("氧气费", "氧气费");

                if (inPut.CenterId != null && (inPut.CenterId.Count > 1 || inPut.CenterId.Contains("0")))
                {
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId.Contains("0"))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    List<TableHeader> tableHeaders = new List<TableHeader>();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "序号",
                        Key = "no",
                    });
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });
                    }

                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Key = "total",
                    });


                    // var CostData = DayMoney.GroupBy(t => t.Cost);
                    List<CenterDayIncomeOutPut> dayIncomes = new List<CenterDayIncomeOutPut>();

                    foreach (var item in keyValuePairs)
                    {
                        CenterDayIncomeOutPut dayIncomeOutPut = new CenterDayIncomeOutPut();
                        dayIncomeOutPut.itemName = item.Key;
                        decimal total = 0;
                        foreach (var items in CenterDialysiss)
                        {
                            /* 
                               string PropertiesName = (dictionary.Name.Replace("+", "") + e.ToString().Substring(1)).ToLower();
                                output.CenterName = item.ShortName;
                                output.Total = CureData.Where(t => t.CenterId == item.Id).Sum(t => t.CureNumber);
                                if (output.GetType().GetProperties().Where(t => t.Name == PropertiesName).FirstOrDefault() != null)
                                 output.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(output, number);
                             */


                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            if (item.Value.Contains("费") && dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).FirstOrDefault() != null)
                            {
                                decimal? number = DetailsDatas.Where(t => t.MedicalItemRecord.FeeType.Name == (item.Value) && t.CenterId == items.Id).Sum(t => t.TotalPrice);
                                dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, number);
                            }
                            else if (dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).FirstOrDefault() != null)
                            {
                                decimal? numbers = DetailsDatas.Where(t => t.MedicalItemRecord.FeeType.Name.Contains(item.Value) && t.CenterId == items.Id).Sum(t => t.TotalPrice);
                                dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, numbers);
                            }
                            total += Convert.ToDecimal(dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().GetValue(dayIncomeOutPut));
                        }
                        dayIncomeOutPut.total = total;
                        dayIncomes.Add(dayIncomeOutPut);
                    }
                    CenterDayIncomeOutPut dayIncomeOutPuts = new CenterDayIncomeOutPut();
                    dayIncomeOutPuts.itemName = "其他";
                    dayIncomeOutPuts.total = 0;
                    foreach (var items in CenterDialysiss)
                    {
                        decimal? number = 0;
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                        switch (PropertiesName)
                        {
                            case "kmtxzx":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.kmtxzx);
                                break;
                            case "xstxzx":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.xstxzx);
                                break;
                            case "dzsmzb":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.dzsmzb);
                                break;
                            case "jlptxzx":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.jlptxzx);
                                break;
                            case "tltxzx":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.tltxzx);
                                break;
                            case "spbtxzx":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.spbtxzx);
                                break;
                            case "fltxzx":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.fltxzx);
                                break;
                            case "zxtxzx":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.zxtxzx);
                                break;
                            case "tjmzb":
                                number = DetailsDatas.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.tjmzb);
                                break;
                            default:
                                break;
                        }
                        dayIncomeOutPuts.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPuts, number);
                        dayIncomeOutPuts.total += number;
                    }

                    dayIncomes.Add(dayIncomeOutPuts);

                    //差额
                    Expression<Func<BalancePayType, bool>> predicate = t => t.CreateDate >= inPut.BeginTime && t.CreateDate <= inPut.EndTime && t.DataState == 1;
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    predicate = predicate.Or(t => strMainId.Contains(t.BalanceNo));
                    var DayMoney = await BalancePayTypeStore.Entities.Include(t => t.centerDialysis).Where(predicate).AsNoTracking().ToListAsync();
                    CenterDayIncomeOutPut CEdayIncomeOutPuts = new CenterDayIncomeOutPut();
                    CEdayIncomeOutPuts.itemName = "差额";
                    CEdayIncomeOutPuts.total = 0;
                    /*
                         var DayMoney = await BalancePayTypeStore.Entities.Include(t => t.centerDialysis).Where(predicate).AsNoTracking().ToListAsync();

                    var SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null).Select(t => t.SIBalanceSerialNo).ToList();
                    decimal? Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);
                     */
                    foreach (var items in CenterDialysiss)
                    {
                        // var SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Select(t => t.SIBalanceSerialNo).ToList();
                        decimal? Prices = 0;//BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH) && t.CenterId == items.Id).Sum(t => t.ZJE);
                        decimal? number = 0;
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                        switch (PropertiesName)
                        {
                            case "kmtxzx":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.kmtxzx) + Prices;
                                CEdayIncomeOutPuts.kmtxzx = number;
                                break;
                            case "xstxzx":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.xstxzx) + Prices;
                                CEdayIncomeOutPuts.xstxzx = number;
                                break;
                            case "dzsmzb":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.dzsmzb) + Prices;
                                CEdayIncomeOutPuts.dzsmzb = number;
                                break;
                            case "jlptxzx":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.jlptxzx) + Prices;
                                CEdayIncomeOutPuts.jlptxzx = number;
                                break;
                            case "tltxzx":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.tltxzx) + Prices;
                                CEdayIncomeOutPuts.tltxzx = number;
                                break;

                            case "spbtxzx":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.spbtxzx) + Prices;
                                CEdayIncomeOutPuts.spbtxzx = number;
                                break;
                            case "fltxzx":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.fltxzx) + Prices;
                                CEdayIncomeOutPuts.fltxzx = number;
                                break;

                            case "zxtxzx":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.zxtxzx) + Prices;
                                CEdayIncomeOutPuts.zxtxzx = number;
                                break;
                            case "tjmzb":
                                number = DayMoney.Where(t => t.CenterId == items.Id).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.tjmzb) + Prices;
                                CEdayIncomeOutPuts.tjmzb = number;
                                break;
                            default:
                                break;
                        }
                        // TotaldayIncomeOutPuts.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPuts, number);
                        CEdayIncomeOutPuts.total += number;
                    }

                    dayIncomes.Add(CEdayIncomeOutPuts);

                    //合计
                    CenterDayIncomeOutPut TotaldayIncomeOutPuts = new CenterDayIncomeOutPut();
                    TotaldayIncomeOutPuts.itemName = "合计";
                    TotaldayIncomeOutPuts.total = 0;

                    foreach (var items in CenterDialysiss)
                    {
                        decimal? number = 0;
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                        switch (PropertiesName)
                        {
                            case "kmtxzx":
                                number = dayIncomes.Sum(t => t.kmtxzx);
                                TotaldayIncomeOutPuts.kmtxzx = number;
                                break;
                            case "xstxzx":
                                number = dayIncomes.Sum(t => t.xstxzx);
                                TotaldayIncomeOutPuts.xstxzx = number;
                                break;
                            case "dzsmzb":
                                number = dayIncomes.Sum(t => t.dzsmzb);
                                TotaldayIncomeOutPuts.dzsmzb = number;
                                break;
                            case "jlptxzx":
                                number = dayIncomes.Sum(t => t.jlptxzx);
                                TotaldayIncomeOutPuts.jlptxzx = number;
                                break;
                            case "tltxzx":
                                number = dayIncomes.Sum(t => t.tltxzx);
                                TotaldayIncomeOutPuts.tltxzx = number;
                                break;
                            case "spbtxzx":
                                number = dayIncomes.Sum(t => t.spbtxzx);
                                TotaldayIncomeOutPuts.spbtxzx = number;
                                break;
                            case "fltxzx":
                                number = dayIncomes.Sum(t => t.fltxzx);
                                TotaldayIncomeOutPuts.fltxzx = number;
                                break;
                            case "zxtxzx":
                                number = dayIncomes.Sum(t => t.zxtxzx);
                                TotaldayIncomeOutPuts.zxtxzx = number;
                                break;
                            case "tjmzb":
                                number = dayIncomes.Sum(t => t.tjmzb);
                                TotaldayIncomeOutPuts.tjmzb = number;
                                break;

                            default:
                                break;
                        }
                        // TotaldayIncomeOutPuts.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPuts, number);
                        TotaldayIncomeOutPuts.total += number;
                    }

                    dayIncomes.Add(TotaldayIncomeOutPuts);
                    //dayIncomes.Add(new DayIncomeOutPut()
                    //{
                    //    ItemName = "合计",
                    //    TotalMoney = dayIncomes.Sum(t => t.TotalMoney)
                    //});
                    dayIncomes.ForEach(t => { t.no = NoIndex; t.kmtxzx = t.kmtxzx.Value.ToRounds(); t.xstxzx = t.xstxzx.Value.ToRounds(); t.dzsmzb = t.dzsmzb.Value.ToRounds(); t.jlptxzx = t.jlptxzx.Value.ToRounds(); t.tltxzx = t.tltxzx.Value.ToRounds(); t.spbtxzx = t.spbtxzx.Value.ToRounds(); t.fltxzx = t.fltxzx.Value.ToRounds(); t.total = t.total.Value.ToRounds(); t.zxtxzx = t.zxtxzx.Value.ToRounds(); t.tjmzb = t.tjmzb.Value.ToRounds(); NoIndex++; });
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.CenterDayIncomeOutPuts = dayIncomes;
                    return (object)aLLDayIncomeOutPut;
                }
                else
                {
                    List<DayIncomeOutPut> dayIncomes = new List<DayIncomeOutPut>();
                    //keyValuePairs.Add("其他费", "药");
                    foreach (var item in keyValuePairs)
                    {
                        DayIncomeOutPut dayIncomeOutPut = new DayIncomeOutPut();
                        dayIncomeOutPut.ItemName = item.Key;
                        if (item.Value.Contains("费"))
                            dayIncomeOutPut.TotalMoney = DetailsDatas.Where(t => t.MedicalItemRecord.FeeType.Name == (item.Value) && t.CenterId == inPut.CenterId[0]).Sum(t => t.TotalPrice);
                        else
                            dayIncomeOutPut.TotalMoney = DetailsDatas.Where(t => t.MedicalItemRecord.FeeType.Name.Contains(item.Value) && t.CenterId == inPut.CenterId[0]).Sum(t => t.TotalPrice);


                        dayIncomes.Add(dayIncomeOutPut);
                    }
                    dayIncomes.Add(new DayIncomeOutPut()
                    {

                        ItemName = "其他",
                        TotalMoney = DetailsDatas.Where(t => t.CenterId == inPut.CenterId[0]).Sum(t => t.TotalPrice) - dayIncomes.Sum(t => t.TotalMoney)
                    });
                    //差额
                    Expression<Func<BalancePayType, bool>> predicate = t => t.CreateDate >= inPut.BeginTime && t.CreateDate <= inPut.EndTime && t.DataState == 1;

                    predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    if (strMainId != null && strMainId.Count() > 0)
                        predicate = predicate.Or(t => strMainId.Contains(t.BalanceNo));

                    var DayMoney = await BalancePayTypeStore.Entities.Include(t => t.centerDialysis).Where(predicate).AsNoTracking().ToListAsync();

                    //var SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null).Select(t => t.SIBalanceSerialNo).ToList();
                    //decimal? Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);
                    dayIncomes.Add(new DayIncomeOutPut()
                    {

                        ItemName = "差额",
                        TotalMoney = DayMoney.Where(t => t.CenterId == inPut.CenterId[0]).Sum(t => t.CostMoney) - dayIncomes.Sum(t => t.TotalMoney),
                    });


                    dayIncomes.Add(new DayIncomeOutPut()
                    {

                        ItemName = "合计",
                        TotalMoney = dayIncomes.Sum(t => t.TotalMoney)
                    });
                    dayIncomes.ForEach(t => { t.no = NoIndex; t.TotalMoney = t.TotalMoney.Value.ToRounds(); NoIndex++; });
                    return (object)dayIncomes;

                }
            });
        }
        /// <summary>
        /// 收入汇总表（按结算方式）
        /// </summary>
        /// <returns></returns>
        public Task<object> GetBusinessPayTypeAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                List<DayIncomeOutPut> dayIncomes = new List<DayIncomeOutPut>();
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                Expression<Func<BalanceMain, bool>> predicate = (t => t.Id != "");
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    predicate = (t => (t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 1 && inPut.CenterId.Contains(t.CenterId)) || (t.CancelFlag == 1 && t.CancelDate > inPut.EndTime && inPut.CenterId.Contains(t.CenterId) && t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime));
                else
                    predicate = (t => (t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 1) || (t.CancelFlag == 1 && t.CancelDate > inPut.EndTime && t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime));
                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();
                //跨月退费数据
                var CancelData = BalanceData.Where(t => t.CancelFlag == 1).Select(t => t.SISourceBalanceNo).ToList();
                //结算明细 
                Expression<Func<BalanceDetails, bool>> depredicate = (t => t.OpenDateTime >= inPut.BeginTime && t.OpenDateTime < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    depredicate = depredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                //  var BalanceBalanceDetailsData = await BalanceDetailsStore.Entities.Where(depredicate).AsNoTracking().ToListAsync();
                // 医保结算
                Expression<Func<SI_JSMX, bool>> sipredicate = (t => (t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime) || CancelData.Contains(t.JSJYLSH));
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();

                Expression<Func<BalancePayType, bool>> typepredicate = t => t.CreateDate >= inPut.BeginTime && t.CreateDate <= inPut.EndTime && t.DataState == 1;
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    typepredicate = typepredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                var DayMoney = await BalancePayTypeStore.Entities.Include(t => t.centerDialysis).Where(typepredicate).AsNoTracking().ToListAsync();
                var typeData = DayMoney.GroupBy(t => t.PayType);
                int NoIndex = 1;
                if (inPut.CenterId != null && (inPut.CenterId.Count > 1 || inPut.CenterId.Contains("0")))
                {
                    ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId.Contains("0"))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    List<TableHeader> tableHeaders = new List<TableHeader>();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "序号",
                        Key = "no",
                    });
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });


                    }
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Key = "total",
                    });
                    List<CenterDayIncomeOutPut> centerDayIncomeOutPuts = new List<CenterDayIncomeOutPut>();
                    foreach (var items in typeData)
                    {
                        CenterDayIncomeOutPut centerdayIncomeOutPuts = new CenterDayIncomeOutPut();
                        centerdayIncomeOutPuts.itemName = ((PayTypeEnum)items.Key).ToChinese();
                        foreach (var item in CenterDialysiss)
                        {
                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                            decimal? num = items.Where(t => t.CenterId == item.Id).Sum(t => t.ReceivableMoney);
                            if (items.Key == 1)
                            {
                                //   //  Prices = SiData.Where(t => t.CenterId == items.Id && t.TFBZ == "1").Sum(t => t.XJZF) + BalanceData.Where(t => t.CenterId == items.Id && t.BalanceState == 1 && t.BalanceType == 2).Sum(t => t.SumPrice)+SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == items.Id).Sum(t => t.XJZF); 
                                num = SiData.Where(t => t.CenterId == item.Id && t.TFBZ == "1").Sum(t => t.XJZF) + BalanceData.Where(t => t.CenterId == item.Id && t.BalanceState == 1 && t.BalanceType == 2).Sum(t => t.SumPrice) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == item.Id).Sum(t => t.XJZF);
                            }
                            centerdayIncomeOutPuts.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(centerdayIncomeOutPuts, num);
                        }
                        centerdayIncomeOutPuts.total = centerdayIncomeOutPuts.kmtxzx + centerdayIncomeOutPuts.xstxzx + centerdayIncomeOutPuts.dzsmzb + centerdayIncomeOutPuts.jlptxzx + centerdayIncomeOutPuts.tltxzx + centerdayIncomeOutPuts.spbtxzx + centerdayIncomeOutPuts.fltxzx + centerdayIncomeOutPuts.zxtxzx + centerdayIncomeOutPuts.tjmzb;
                        centerDayIncomeOutPuts.Add(centerdayIncomeOutPuts);
                    }

                    CenterDayIncomeOutPut TotaldayIncomeOutPuts = new CenterDayIncomeOutPut();
                    TotaldayIncomeOutPuts.itemName = "合计";
                    TotaldayIncomeOutPuts.total = 0;
                    centerDayIncomeOutPuts.Add(new CenterDayIncomeOutPut()
                    {
                        itemName = "现金差额",
                        kmtxzx = -DayMoney.Where(t => t.CenterId == "abc3d60b474c4fef946a66887348c41a").Sum(t => t.DifferenceMoney),
                        dzsmzb = -DayMoney.Where(t => t.CenterId == "b379bd0591584feba7e0809739784a47").Sum(t => t.DifferenceMoney),
                        xstxzx = -DayMoney.Where(t => t.CenterId == "c07406f87d0946c99a031be5034382ec").Sum(t => t.DifferenceMoney),
                        jlptxzx = -DayMoney.Where(t => t.CenterId == "5b79a3d1843b4162b7cbd7f88a5bd4fd").Sum(t => t.DifferenceMoney),
                        tltxzx = -DayMoney.Where(t => t.CenterId == "f17a9c72fa8e47b78988757c6913b22b").Sum(t => t.DifferenceMoney),
                        spbtxzx = -DayMoney.Where(t => t.CenterId == "c7c97515763a42c1a914d1bd7e8a5474").Sum(t => t.DifferenceMoney),
                        fltxzx = -DayMoney.Where(t => t.CenterId == "57071610d4f04e43a84d3de143ffc730").Sum(t => t.DifferenceMoney),
                        zxtxzx = -DayMoney.Where(t => t.CenterId == "d0701c7d2cb242028445fca1034fb4ec").Sum(t => t.DifferenceMoney),
                        tjmzb = -DayMoney.Where(t => t.CenterId == "bb223c13dac248cebcb05152ed7b789d").Sum(t => t.DifferenceMoney),
                        total = -DayMoney.Where(t => CenterDialysiss.Select(k => k.Id).Contains(t.CenterId)).Sum(t => t.DifferenceMoney),
                    });
                    centerDayIncomeOutPuts.Add(new CenterDayIncomeOutPut()
                    {// Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == items.Id).Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == items.Id).Sum(t => t.ZJE));
                        itemName = "医保实结差额",
                        kmtxzx = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "abc3d60b474c4fef946a66887348c41a").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "abc3d60b474c4fef946a66887348c41a").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "abc3d60b474c4fef946a66887348c41a").Sum(t => t.ZJE)),
                        //b379bd0591584feba7e0809739784a47
                        dzsmzb = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "b379bd0591584feba7e0809739784a47").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "b379bd0591584feba7e0809739784a47").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "b379bd0591584feba7e0809739784a47").Sum(t => t.ZJE)),
                        //c07406f87d0946c99a031be5034382ec
                        xstxzx = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "c07406f87d0946c99a031be5034382ec").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "c07406f87d0946c99a031be5034382ec").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "c07406f87d0946c99a031be5034382ec").Sum(t => t.ZJE)),
                        //5b79a3d1843b4162b7cbd7f88a5bd4fd
                        jlptxzx = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "5b79a3d1843b4162b7cbd7f88a5bd4fd").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "5b79a3d1843b4162b7cbd7f88a5bd4fd").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "5b79a3d1843b4162b7cbd7f88a5bd4fd").Sum(t => t.ZJE)),
                        //f17a9c72fa8e47b78988757c6913b22b
                        tltxzx = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "f17a9c72fa8e47b78988757c6913b22b").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "f17a9c72fa8e47b78988757c6913b22b").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "f17a9c72fa8e47b78988757c6913b22b").Sum(t => t.ZJE)),
                        //c7c97515763a42c1a914d1bd7e8a5474
                        spbtxzx = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "c7c97515763a42c1a914d1bd7e8a5474").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "c7c97515763a42c1a914d1bd7e8a5474").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "c7c97515763a42c1a914d1bd7e8a5474").Sum(t => t.ZJE)),
                        //57071610d4f04e43a84d3de143ffc730
                        fltxzx = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "57071610d4f04e43a84d3de143ffc730").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "57071610d4f04e43a84d3de143ffc730").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "57071610d4f04e43a84d3de143ffc730").Sum(t => t.ZJE)),
                        //d0701c7d2cb242028445fca1034fb4ec
                        zxtxzx = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "d0701c7d2cb242028445fca1034fb4ec").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "d0701c7d2cb242028445fca1034fb4ec").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "d0701c7d2cb242028445fca1034fb4ec").Sum(t => t.ZJE)),
                        //bb223c13dac248cebcb05152ed7b789d
                        tjmzb = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == "bb223c13dac248cebcb05152ed7b789d").Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == "bb223c13dac248cebcb05152ed7b789d").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == "bb223c13dac248cebcb05152ed7b789d").Sum(t => t.ZJE)),

                        total = BalanceData.Where(t => t.SIBalanceSerialNo != null).Sum(t => t.SumPrice) - SiData.Where(t => BalanceData.Where(k => k.SIBalanceSerialNo != null).Select(k => k.SIBalanceSerialNo).Contains(t.JSJYLSH)).Sum(t => t.ZJE),
                    });
                    foreach (var items in CenterDialysiss)
                    {
                        decimal? number = 0;
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                        switch (PropertiesName)
                        {
                            case "kmtxzx":
                                number = centerDayIncomeOutPuts.Sum(t => t.kmtxzx);
                                break;
                            case "xstxzx":
                                number = centerDayIncomeOutPuts.Sum(t => t.xstxzx);
                                break;
                            case "dzsmzb":
                                number = centerDayIncomeOutPuts.Sum(t => t.dzsmzb);
                                break;

                            case "jlptxzx":
                                number = centerDayIncomeOutPuts.Sum(t => t.jlptxzx);
                                break;
                            case "tltxzx":
                                number = centerDayIncomeOutPuts.Sum(t => t.tltxzx);
                                break;
                            case "spbtxzx":
                                number = centerDayIncomeOutPuts.Sum(t => t.spbtxzx);
                                break;
                            case "fltxzx":
                                number = centerDayIncomeOutPuts.Sum(t => t.fltxzx);
                                break;
                            case "zxtxzx":
                                number = centerDayIncomeOutPuts.Sum(t => t.zxtxzx);
                                break;
                            case "tjmzb":
                                number = centerDayIncomeOutPuts.Sum(t => t.tjmzb);
                                break;
                            default:
                                break;
                        }
                        TotaldayIncomeOutPuts.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(TotaldayIncomeOutPuts, number);
                        TotaldayIncomeOutPuts.total += number;
                    }

                    centerDayIncomeOutPuts.Add(TotaldayIncomeOutPuts);

                    centerDayIncomeOutPuts.ForEach(t =>
                    {

                        t.no = NoIndex; t.kmtxzx = t.kmtxzx.Value.ToRounds(); t.xstxzx = t.xstxzx.Value.ToRounds(); t.dzsmzb = t.dzsmzb.Value.ToRounds(); t.jlptxzx = t.jlptxzx.Value.ToRounds(); t.total = t.total.Value.ToRounds(); NoIndex++; t.tltxzx = t.tltxzx.Value.ToRounds(); t.fltxzx = t.fltxzx.Value.ToRounds(); t.spbtxzx = t.spbtxzx.Value.ToRounds(); t.zxtxzx = t.zxtxzx.Value.ToRounds(); t.tjmzb = t.tjmzb.Value.ToRounds();


                    });
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.CenterDayIncomeOutPuts = centerDayIncomeOutPuts;
                    return (object)aLLDayIncomeOutPut;
                }
                else
                {
                    List<string> SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null).Select(t => t.SIBalanceSerialNo).ToList();
                    foreach (var item in typeData)
                    {
                        decimal? TotalMoney = item.Sum(t => t.ReceivableMoney);

                        if (item.Key == 1)
                        {
                            TotalMoney = SiData.Where(t => t.TFBZ == "1").Sum(t => t.XJZF) + BalanceData.Where(t => t.BalanceState == 1 && t.BalanceType == 2).Sum(t => t.SumPrice) + SiData.Where(t => CancelData.Contains(t.JSJYLSH)).Sum(t => t.XJZF); ;
                        }
                        dayIncomes.Add(new DayIncomeOutPut()
                        {
                            ItemName = ((PayTypeEnum)item.Key).ToChinese(),

                            TotalMoney = TotalMoney
                        });
                    }
                    dayIncomes.Add(new DayIncomeOutPut()
                    {
                        ItemName = "现金差额",
                        TotalMoney = DayMoney.Sum(t => t.DifferenceMoney)

                    });
                    // Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == items.Id).Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == items.Id).Sum(t => t.ZJE));
                    dayIncomes.Add(new DayIncomeOutPut()
                    {
                        ItemName = "医保实结差额",

                        TotalMoney = BalanceData.Where(t => t.SIBalanceSerialNo != null).Sum(t => t.SumPrice) - (SiData.Where(t => CancelData.Contains(t.JSJYLSH)).Sum(t => t.ZJE) + SiData.Where(t => t.TFBZ == "1").Sum(t => t.ZJE))
                    });
                    dayIncomes.Add(new DayIncomeOutPut()
                    {
                        ItemName = "合计",
                        TotalMoney = dayIncomes.Sum(t => t.TotalMoney)
                    });

                    dayIncomes.ForEach(t => { t.no = NoIndex; t.TotalMoney = t.TotalMoney.Value.ToRounds(); NoIndex++; });
                }
                return (object)dayIncomes.ToArray();
            });
        }

        /// <summary>
        /// 收入汇总表（按回款方式）
        /// </summary>
        /// <returns></returns>
        public Task<object> GetBusinessBackTypeAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                List<string> ItemGroup = new List<string>() { "现金", "现金差额", "个人账户(职工)", "民政救助(职工)", "大额统筹(职工)", "医保基金(职工)", "个人账户(居民)", "民政救助(居民)", "大额统筹(居民)", "医保基金(居民)", "医保实结差额", "开业优惠" };
                int NoIndex = 1;
                List<DayIncomeOutPut> dayIncomes = new List<DayIncomeOutPut>();
                DayReportOutPut dayReportOutPut = new DayReportOutPut();

                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                Expression<Func<BalanceMain, bool>> manpredicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    manpredicate = manpredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                manpredicate = manpredicate.And(t => t.CancelDate == null || t.CancelDate > inPut.EndTime);
                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(manpredicate).AsNoTracking().ToListAsync();

                //跨月退费数据 
                var strMainId = BalanceData.Where(t => t.BalanceState == 0).Select(t => t.BalanceNo);
                //跨月退费数据 
                var CancelData = BalanceData.Where(t => t.CancelFlag == 1).Select(t => t.SISourceBalanceNo).ToList();
                //结算明细 
                Expression<Func<BalanceDetails, bool>> depredicate = (t => t.OpenDateTime >= inPut.BeginTime && t.OpenDateTime < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    depredicate = depredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                //  var BalanceBalanceDetailsData = await BalanceDetailsStore.Entities.Where(depredicate).AsNoTracking().ToListAsync();
                // 医保结算  
                Expression<Func<SI_JSMX, bool>> sipredicate = (t => (t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime) || CancelData.Contains(t.JSJYLSH));
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();

                //   var kk = SiData.Sum(z => z.ZJE); 
                Expression<Func<BalancePayType, bool>> paysipredicate = (t => t.DataState == 1 && t.CreateDate >= inPut.BeginTime && t.CreateDate < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    paysipredicate = paysipredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                if (strMainId != null && strMainId.Count() > 0)
                    paysipredicate = paysipredicate.Or(t => strMainId.Contains(t.BalanceNo));
                //结算方式表中获取差额
                var DifferenceMoneyData = await BalancePayTypeStore.Entities.Where(paysipredicate).AsNoTracking().ToListAsync();
                if (inPut.CenterId != null)// && (inPut.CenterId.Count > 1 || inPut.CenterId.Contains("0"))  
                {
                    ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId.Contains("0"))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    List<TableHeader> tableHeaders = new List<TableHeader>();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "序号",
                        Key = "no",
                    });
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });
                    }
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Key = "total",
                    });
                    List<CenterDayIncomeOutPut> centerDayIncomeOutPuts = new List<CenterDayIncomeOutPut>();
                    foreach (var item in ItemGroup)
                    {
                        CenterDayIncomeOutPut put = new CenterDayIncomeOutPut();
                        put.itemName = item;
                        foreach (var items in CenterDialysiss)
                        {
                            decimal? PaySum = BalanceData.Where(t => t.CenterId == items.Id).Sum(t => t.SumPrice);
                            List<string> ListNo = BalanceData.Where(t => t.CenterId == items.Id && t.BalanceType == 2).Select(t => t.BalanceNo).ToList();
                            // decimal? SelfPaySum = BalanceBalanceDetailsData.Where(t => ListNo.Contains(t.BalanceNo) && t.DataState == 1).Sum(t => t.TotalPrice);
                            List<string> SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Select(t => t.SIBalanceSerialNo).ToList();
                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? Prices = 0;
                            switch (item)
                            {
                                case "现金":
                                    //+ (SiData.Where(t => t.CenterId == items.Id && t.TFBZ=="1").Sum(t => t.XJZF))
                                    /*   Prices = SiData.Where(t => t.CenterId == items.Id && t.TFBZ == "1").Sum(t => t.XJZF) + DifferenceMoneyData.Where(t => t.CenterId == items.Id && t.PayType==1).Sum(t => t.ReceivableMoney) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == items.Id).Sum(t => t.XJZF);*/ //现金支付//现金支付 
                                    Prices = DifferenceMoneyData.Where(t => t.CenterId == items.Id && t.PayType == 1).Sum(t => t.ReceivableMoney);
                                    break;

                                case "现金差额":
                                    Prices = 0 - DifferenceMoneyData.Where(t => t.CenterId == items.Id).Sum(t => t.DifferenceMoney);
                                    break;
                                case "个人账户(职工)":
                                    Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "1" && t.TFBZ == "1").Sum(t => t.ZHZF);//个人账户 ;
                                    break;
                                case "民政救助(职工)":
                                    Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "1" && t.TFBZ == "1").Sum(t => t.MZJZ);//民政救助
                                    break;
                                case "大额统筹(职工)":
                                    if (items.Id == "f17a9c72fa8e47b78988757c6913b22b")//铜梁
                                        Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "1" && t.TFBZ == "1").Sum(t => (t.TCZF));
                                    else
                                        Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "1" && t.TFBZ == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                                    break;
                                case "医保基金(职工)":

                                    Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "1" && t.TFBZ == "1").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                    break;

                                case "个人账户(居民)":
                                    Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "2" && t.TFBZ == "1").Sum(t => t.ZHZF);//个人账户;
                                    break;
                                case "民政救助(居民)":
                                    Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "2" && t.TFBZ == "1").Sum(t => t.MZJZ);//民政救助  
                                    break;
                                case "大额统筹(居民)":
                                    if (items.Id == "f17a9c72fa8e47b78988757c6913b22b")//铜梁
                                        Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "2" && t.TFBZ == "1").Sum(t => (t.TCZF));
                                    else
                                        Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "2" && t.TFBZ == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                                    break;
                                case "医保基金(居民)":
                                    Prices = SiData.Where(t => t.CenterId == items.Id && t.CBLB == "2" && t.TFBZ == "1").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                    break;
                                //( BalanceData.Where(t => t.SISourceBalanceNo != null).Sum(t => t.SumPrice)) -( SiData.Where(t=>t.TFBZ =="1").Sum(t => t.ZJE)+ SiData.Where(t => CancelData.Contains(t.JSJYLSH)).Sum(t => t.ZJE));   
                                case "医保实结差额":
                                    Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Sum(t => t.SumPrice) - (SiData.Where(t => t.TFBZ == "1" && t.CenterId == items.Id).Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH) && t.CenterId == items.Id).Sum(t => t.ZJE));
                                    break;

                                case "开业优惠":
                                    Prices = DifferenceMoneyData.Where(t => t.CenterId == items.Id && t.PayType == 8).Sum(t => t.CostMoney);//开业优惠
                                    break;
                                //case "合计":
                                //    Prices = PaySum;
                                //    break;

                                default:
                                    break;
                            }
                            put.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(put, Prices);
                        }
                        put.total = put.kmtxzx + put.dzsmzb + put.xstxzx + put.jlptxzx + put.tltxzx + put.spbtxzx + put.fltxzx + put.zxtxzx + put.tjmzb;

                        centerDayIncomeOutPuts.Add(put);
                    }



                    centerDayIncomeOutPuts.Add(new CenterDayIncomeOutPut()
                    {
                        itemName = "合计",
                        xstxzx = centerDayIncomeOutPuts.Sum(t => t.xstxzx),
                        kmtxzx = centerDayIncomeOutPuts.Sum(t => t.kmtxzx),
                        dzsmzb = centerDayIncomeOutPuts.Sum(t => t.dzsmzb),
                        jlptxzx = centerDayIncomeOutPuts.Sum(t => t.jlptxzx),
                        tltxzx = centerDayIncomeOutPuts.Sum(t => t.tltxzx),
                        spbtxzx = centerDayIncomeOutPuts.Sum(t => t.spbtxzx),
                        fltxzx = centerDayIncomeOutPuts.Sum(t => t.fltxzx),
                        zxtxzx = centerDayIncomeOutPuts.Sum(t => t.zxtxzx),
                        tjmzb = centerDayIncomeOutPuts.Sum(t => t.tjmzb),
                        total = centerDayIncomeOutPuts.Sum(t => t.total),

                    });
                    centerDayIncomeOutPuts.ForEach(t =>
                    {
                        t.no = NoIndex; t.kmtxzx = t.kmtxzx.Value.ToRounds(); t.xstxzx = t.xstxzx.Value.ToRounds(); t.dzsmzb = t.dzsmzb.Value.ToRounds(); t.jlptxzx = t.jlptxzx.Value.ToRounds(); t.total = t.total.Value.ToRounds(); NoIndex++; t.tltxzx = t.tltxzx.Value.ToRounds(); t.spbtxzx = t.spbtxzx.Value.ToRounds(); t.zxtxzx = t.zxtxzx.Value.ToRounds(); t.tjmzb = t.tjmzb.Value.ToRounds();
                        t.fltxzx = t.fltxzx.Value.ToRounds();
                    });
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.CenterDayIncomeOutPuts = centerDayIncomeOutPuts;
                    return (object)aLLDayIncomeOutPut;
                }
                else
                {
                    decimal? PaySum = BalanceData.Sum(t => t.SumPrice);

                    List<string> ListNo = BalanceData.Where(t => t.BalanceType == 2).Select(t => t.BalanceNo).ToList();
                    // decimal? SelfPaySum = BalanceBalanceDetailsData.Where(t => ListNo.Contains(t.BalanceNo) && t.DataState == 1).Sum(t => t.TotalPrice);
                    List<string> SiBsNo = BalanceData.Where(t => t.SISourceBalanceNo != null).Select(t => t.SIBalanceSerialNo).ToList();
                    foreach (var item in ItemGroup)
                    {
                        DayIncomeOutPut put = new DayIncomeOutPut();
                        put.ItemName = item;
                        switch (item)
                        {
                            case "现金":
                                //+ (SiData.Where(t=>t.TFBZ  == "1").Sum(t => t.XJZF))
                                put.TotalMoney = (SiData.Where(t => t.TFBZ == "1").Sum(t => t.XJZF)) + BalanceData.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice) + SiData.Where(t => CancelData.Contains(t.JSJYLSH)).Sum(t => t.XJZF);// DifferenceMoneyData.Where(t => t.PayType == 1).Sum(t => t.ReceivableMoney) ;//现金支付
                                break;
                            case "现金差额":
                                put.TotalMoney = DifferenceMoneyData.Where(t => t.CenterId == inPut.CenterId.First() && t.PayType == 1).Sum(t => t.DifferenceMoney);

                                break;
                            case "个人账户(职工)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "1").Sum(t => t.ZHZF);//个人账户 ;
                                break;
                            case "民政救助(职工)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "1").Sum(t => t.MZJZ);//民政救助
                                break;
                            case "大额统筹(职工)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                                break;
                            case "医保基金(职工)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "1").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                break;

                            case "个人账户(居民)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "2").Sum(t => t.ZHZF);//个人账户 ;
                                break;
                            case "民政救助(居民)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "2").Sum(t => t.MZJZ);//民政救助
                                break;
                            case "大额统筹(居民)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "2").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                                break;
                            case "医保基金(居民)":
                                put.TotalMoney = SiData.Where(t => t.CBLB == "2").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                break;

                            case "医保实结差额":
                                put.TotalMoney = (BalanceData.Where(t => t.SISourceBalanceNo != null).Sum(t => t.SumPrice)) - (SiData.Where(t => t.TFBZ == "1").Sum(t => t.ZJE) + SiData.Where(t => CancelData.Contains(t.JSJYLSH)).Sum(t => t.ZJE));
                                break;
                            case "开业优惠":
                                put.TotalMoney = DifferenceMoneyData.Where(t => t.PayType == 8).Sum(t => t.CostMoney);//开业优惠
                                break;
                            case "合计":
                                put.TotalMoney = PaySum;
                                break;

                            default:
                                break;
                        }

                        dayIncomes.Add(put);

                    }
                    dayIncomes.Add(new DayIncomeOutPut()
                    {
                        ItemName = "合计",
                        TotalMoney = dayIncomes.Sum(t => t.TotalMoney),
                    });

                    ////总额
                    //dayReportOutPut.total = PaySum;
                    ////现金(自费)
                    //dayReportOutPut.Individual = SiData.Sum(t => t.ZHZF);//个人账户 
                    //SelfPaySum += SiData.Sum(t => t.XJZF);//现金支付
                    //dayReportOutPut.xj = SelfPaySum;
                    ////医保
                    //dayReportOutPut.mzbz = SiData.Sum(t => t.MZJZ);//民政救助
                    ////即实际的统筹支付应该是 TCZF - MZJZ - YBZLZFS (职工参保，统筹支付 =统筹支付，如果是居民参保，就需要减去民政救助)
                    //dayReportOutPut.detc = SiData.Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));// + SiData.Where(t => t.CBLB == "2").Sum(t =>( t.TCZF-t.MZJZ-t.YBZLZFS));
                    //dayReportOutPut.ybjj = SiData.Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                    //dayReportOutPut.yycb = SiData.Sum(t => t.CBKK);//医院超标 
                    dayIncomes.ForEach(t =>
            {
                t.no = NoIndex; t.TotalMoney = t.TotalMoney.Value.ToRounds(); NoIndex++;
            });
                    return (object)dayIncomes.ToArray();
                }
            });
        }


        /// <summary>
        /// 成本总汇表（按收费项目）销售 ，退货退费，领用，报废
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetBusinessCostsAsync(CostSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var list = new List<CostModel>();

                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetSumCost", "成本总汇");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = string.Format(sql, inPut.BeginTime.Value.Date, inPut.EndTime);
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                var tuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(dt);

                var data = await WaterFuelsStore.Entities.Where(t => t.IsDelete == false && t.Month >= inPut.BeginTime.Value.Date && t.Month <= inPut.EndTime.Value.Date).AsNoTracking().ToListAsync();

                var CenterDialysiss = new List<CenterDialysis>();
                if (inPut.CenterId != null)
                {
                    //查询全部 
                    if (inPut.CenterId.Count == 1 && inPut.CenterId[0] == "0")
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    }

                    var model = new CostModel();
                    string title = "序号,类别,";
                    string value = string.Empty;

                    var totalModel = new CostModel();
                    totalModel.title = "6,合计,";
                    foreach (var item in CenterDialysiss)
                    {
                        value += ReturnCenttSortName(item.ShortName) + ",";
                        var OperatingCost = data.FindAll(a => a.CenterId == item.Id).Sum(a => a.AmountPriec);
                        var SumCost = tuple.Item1 == false ? 0 : tuple.Item2.FindAll(a => a.CenterId == item.Id && (a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" || a.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || a.ItemType == "01f769807f9a4736be41db800606447e")).Sum(a => (a.InPrice * a.ItemQty)) - tuple.Item2.FindAll(a => a.CenterId == item.Id && a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => (a.InPrice * a.ItemQty));
                        totalModel.title += (SumCost ?? 0).ToRounds(4) + (OperatingCost ?? 0).ToRounds(4) + ",";
                    }
                    model.title = title + value + "合计";
                    list.Add(model);
                    //类别  
                    var typeDic = new Dictionary<int, string>();
                    typeDic.Add(1, "药品");
                    typeDic.Add(2, "卫生耗材");
                    typeDic.Add(3, "固定资产");
                    typeDic.Add(4, "低值易耗");
                    typeDic.Add(5, "经营成本");
                    //合计成本
                    var costDic = new Dictionary<string, decimal>();
                    decimal? totalCost = 0;
                    int i = 0;
                    foreach (var key in typeDic.Keys)
                    {
                        i++;
                        var model1 = new CostModel();
                        string title1 = i + "," + typeDic[key] + ",";
                        string value1 = string.Empty;
                        decimal? totalTypeCost = 0;

                        foreach (var item in CenterDialysiss)
                        {
                            if (key == 5)
                            {
                                var sumCost = data.FindAll(a => a.CenterId == item.Id).Sum(a => a.AmountPriec);
                                value1 += (sumCost ?? 0).ToRounds(4) + ",";
                                totalTypeCost += sumCost;
                                totalCost += sumCost;
                            }
                            else
                            {
                                var sumCost = tuple.Item2.FindAll(a => a.MedicalItemType == key && a.CenterId == item.Id && (a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" || a.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || a.ItemType == "01f769807f9a4736be41db800606447e")).Sum(a => (a.InPrice * a.ItemQty)) - tuple.Item2.FindAll(a => a.MedicalItemType == key && a.CenterId == item.Id && a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => (a.InPrice * a.ItemQty));
                                value1 += tuple.Item1 == false ? "0," : (sumCost ?? 0).ToRounds(4) + ",";
                                totalTypeCost += sumCost;
                                totalCost += sumCost;
                            }
                        }
                        model1.title = title1 + value1 + (totalTypeCost ?? 0).ToRounds(4);
                        list.Add(model1);
                    }
                    totalModel.title += (totalCost ?? 0).ToRounds(4);
                    list.Add(totalModel);
                }
                return (object)list.ToArray();
            });
        }

        //日报表 DayReportOutPut 
        public Task<object> GetDayReportAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                DayReportOutPut dayReportOutPut = new DayReportOutPut();

                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                Expression<Func<BalanceMain, bool>> predicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 1);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                //结算总表 
                var BalanceData = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();
                // 医保结算  

                Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();

                decimal? PaySum = BalanceData.Sum(t => t.SumPrice);

                //   decimal? SelfPaySum = BalanceData.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice);

                Expression<Func<BalancePayType, bool>> paysipredicate = (t => t.DataState == 1 && t.CreateDate >= inPut.BeginTime && t.CreateDate < inPut.EndTime && t.PayType == 1);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    paysipredicate = paysipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                //结算方式表中获取差额
                var DifferenceMoneyData = await BalancePayTypeStore.Entities.Where(paysipredicate).AsNoTracking().ToListAsync();
                //结算主表 筛选以退费项目  
                //  List<string> BalanceMainIds = await BalanceMainStore.Entities.Where(predicate).Select(t => t.BalanceNo).AsNoTracking().ToListAsync();
                //总额    
                // dayReportOutPut.total = PaySum; 
                //现金(自费)
                //差额
                var SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null).Select(t => t.SIBalanceSerialNo).ToList();
                var Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);
                // SelfPaySum += SiData.Sum(t => t.XJZF);//现金支付
                dayReportOutPut.xj = DifferenceMoneyData.Sum(t => t.ReceivableMoney) - DifferenceMoneyData.Sum(t => t.DifferenceMoney) + Prices;//现金支付;
                                                                                                                                                //医保 cblb 为 1的 是职工参保  2是居民参保
                                                                                                                                                //职工
                dayReportOutPut.Individual = SiData.Where(t => t.CBLB == "1").Sum(t => t.ZHZF);//个人账户 
                dayReportOutPut.mzbz = SiData.Where(t => t.CBLB == "1").Sum(t => t.MZJZ);//民政救助
                                                                                         //即实际的统筹支付应该是 TCZF - MZJZ - YBZLZFS (职工参保，统筹支付 =统筹支付，如果是居民参保，就需要减去民政救助)
                dayReportOutPut.detc = SiData.Where(t => t.CBLB == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));// 大额统筹 + SiData.Where(t => t.CBLB == "2").Sum(t =>( t.TCZF-t.MZJZ-t.YBZLZFS));
                dayReportOutPut.ybjj = SiData.Where(t => t.CBLB == "1").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                                                                                                     //居民


                dayReportOutPut.jmindividual = SiData.Where(t => t.CBLB == "2").Sum(t => t.ZHZF);//个人账户 
                dayReportOutPut.jmmzbz = SiData.Where(t => t.CBLB == "2").Sum(t => t.MZJZ);//民政救助
                                                                                           //即实际的统筹支付应该是 TCZF - MZJZ - YBZLZFS (职工参保，统筹支付 =统筹支付，如果是居民参保，就需要减去民政救助)
                dayReportOutPut.jmdetc = SiData.Where(t => t.CBLB == "2").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));// 大额统筹 + SiData.Where(t => t.CBLB == "2").Sum(t =>( t.TCZF-t.MZJZ-t.YBZLZFS));
                dayReportOutPut.jmybjj = SiData.Where(t => t.CBLB == "2").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金

                dayReportOutPut.yycb = SiData.Sum(t => t.CBKK);//医院超标
                dayReportOutPut.total = dayReportOutPut.Individual + dayReportOutPut.xj + dayReportOutPut.mzbz + dayReportOutPut.detc + dayReportOutPut.ybjj + dayReportOutPut.yycb - dayReportOutPut.yycb + dayReportOutPut.jmdetc + dayReportOutPut.jmindividual + dayReportOutPut.jmmzbz + dayReportOutPut.jmybjj;
                return (object)dayReportOutPut;
            });

        }


        //日报表 DayReportOutPut
        public Task<object> GetDayReportToCenterAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                DayReportOutPut dayReportOutPut = new DayReportOutPut();

                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                Expression<Func<BalanceMain, bool>> predicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 1);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();
                // 医保结算

                Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();

                decimal? PaySum = BalanceData.Sum(t => t.SumPrice);

                //   decimal? SelfPaySum = BalanceData.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice);

                Expression<Func<BalancePayType, bool>> paysipredicate = (t => t.DataState == 1 && t.CreateDate >= inPut.BeginTime && t.CreateDate < inPut.EndTime && t.PayType == 1);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    paysipredicate = paysipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                //结算方式表中获取差额
                var DifferenceMoneyData = await BalancePayTypeStore.Entities.Where(paysipredicate).AsNoTracking().ToListAsync();
                //结算主表 筛选以退费项目
                //  List<string> BalanceMainIds = await BalanceMainStore.Entities.Where(predicate).Select(t => t.BalanceNo).AsNoTracking().ToListAsync();
                //总额
                // dayReportOutPut.total = PaySum;
                //现金(自费)
                //差额


                #region -----------------
                ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                if (inPut.CenterId.Contains("0"))
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                else
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                tableHeaders.Add(new TableHeader()
                {
                    title = "序号",
                    Key = "no",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "项目",
                    Key = "itemName",
                });
                foreach (var item in CenterDialysiss)
                {
                    string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = ReturnCenttSortName(item.ShortName),
                        Key = PropertiesName,
                    });
                }
                tableHeaders.Add(new TableHeader()
                {
                    title = "合计",
                    Key = "total",
                });
                List<CenterDayIncomeOutPut> centerDayIncomeOutPuts = new List<CenterDayIncomeOutPut>();

                List<string> ItemGroup = new List<string>() { "合计", "现金", "个人账户(职工)", "民政救助(职工)", "大额(职工)", "统筹(职工)", "个人账户(居民)", "民政救助(居民)", "大额(居民)", "统筹(居民)", "医院超标" };
                int NoIndex = 1;
                foreach (var item in ItemGroup)
                {
                    CenterDayIncomeOutPut put = new CenterDayIncomeOutPut();
                    put.itemName = item;
                    foreach (var items in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                        decimal? TempPrices = 0;
                        var SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Select(t => t.SIBalanceSerialNo).ToList();
                        var Prices = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == items.Id).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH) && t.CenterId == items.Id).Sum(t => t.ZJE);
                        #region   case
                        switch (item)
                        {
                            case "合计":
                                break;
                            case "现金":
                                TempPrices = DifferenceMoneyData.Where(t => t.CenterId == items.Id).Sum(t => t.ReceivableMoney) - DifferenceMoneyData.Where(t => t.CenterId == items.Id).Sum(t => t.DifferenceMoney) + Prices;//现金支付;
                                break;
                            case "个人账户(职工)":
                                TempPrices = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Sum(t => t.ZHZF);
                                break;
                            case "民政救助(职工)":
                                TempPrices = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Sum(t => t.MZJZ);//民政救助  
                                break;
                            case "大额(职工)":
                                TempPrices = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                                break;
                            case "统筹(职工)":
                                TempPrices = SiData.Where(t => t.CBLB == "1" && t.CenterId == items.Id).Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                break;
                            case "个人账户(居民)":
                                TempPrices = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Sum(t => t.ZHZF);//个人账户 
                                break;
                            case "民政救助(居民)":
                                TempPrices = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Sum(t => t.MZJZ);
                                break;
                            case "大额(居民)":
                                TempPrices = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                                break;
                            case "统筹(居民)":
                                TempPrices = SiData.Where(t => t.CBLB == "2" && t.CenterId == items.Id).Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                break;
                            case "医院超标":
                                TempPrices = SiData.Where(t => t.CenterId == items.Id).Sum(t => t.CBKK);//医院超标 
                                break;


                            default:
                                break;
                        }
                        put.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(put, TempPrices);
                        #endregion
                    }

                }
                #endregion


                // SelfPaySum += SiData.Sum(t => t.XJZF);//现金支付
                // dayReportOutPut.xj = DifferenceMoneyData.Sum(t => t.ReceivableMoney) - DifferenceMoneyData.Sum(t => t.DifferenceMoney) + Prices;//现金支付;
                //医保 cblb 为 1的 是职工参保  2是居民参保
                //职工
                dayReportOutPut.Individual = SiData.Where(t => t.CBLB == "1").Sum(t => t.ZHZF);//个人账户 
                dayReportOutPut.mzbz = SiData.Where(t => t.CBLB == "1").Sum(t => t.MZJZ);//民政救助
                                                                                         //即实际的统筹支付应该是 TCZF - MZJZ - YBZLZFS (职工参保，统筹支付 =统筹支付，如果是居民参保，就需要减去民政救助)
                dayReportOutPut.detc = SiData.Where(t => t.CBLB == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));// 大额统筹 + SiData.Where(t => t.CBLB == "2").Sum(t =>( t.TCZF-t.MZJZ-t.YBZLZFS));
                dayReportOutPut.ybjj = SiData.Where(t => t.CBLB == "1").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金
                                                                                                                     //居民


                dayReportOutPut.jmindividual = SiData.Where(t => t.CBLB == "2").Sum(t => t.ZHZF);//个人账户  
                dayReportOutPut.jmmzbz = SiData.Where(t => t.CBLB == "2").Sum(t => t.MZJZ);//民政救助  
                                                                                           //即实际的统筹支付应该是 TCZF - MZJZ - YBZLZFS (职工参保，统筹支付 =统筹支付，如果是居民参保，就需要减去民政救助)
                dayReportOutPut.jmdetc = SiData.Where(t => t.CBLB == "2").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));// 大额统筹 + SiData.Where(t => t.CBLB == "2").Sum(t =>( t.TCZF-t.MZJZ-t.YBZLZFS));
                dayReportOutPut.jmybjj = SiData.Where(t => t.CBLB == "2").Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金

                dayReportOutPut.yycb = SiData.Sum(t => t.CBKK);//医院超标
                dayReportOutPut.total = dayReportOutPut.Individual + dayReportOutPut.xj + dayReportOutPut.mzbz + dayReportOutPut.detc + dayReportOutPut.ybjj + dayReportOutPut.yycb - dayReportOutPut.yycb + dayReportOutPut.jmdetc + dayReportOutPut.jmindividual + dayReportOutPut.jmmzbz + dayReportOutPut.jmybjj;
                return (object)dayReportOutPut;
            });

        }



        //收费清单 charge
        public Task<PageData<RefundOutPut[]>> GetChargeAsync(RefundQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                List<RefundOutPut> refundOutPuts = new List<RefundOutPut>();
                int count = 0;
                try
                {
                    var Parents = await PatientStore.Entities.AsNoTracking().ToListAsync();//在院患者
                    Expression<Func<BalanceMain, bool>> predicate = (t => inPut.BeginTime < t.BalanceDate && inPut.EndTime > t.BalanceDate);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    predicate = predicate.And(t => t.CancelDate == null || t.CancelDate > inPut.EndTime);
                    if (inPut.keyword + "" != "")
                    {
                        List<string> patientNos = Parents.Where(t => t.Name.Contains(inPut.keyword)).Select(t => t.PatientNo).ToList();
                        predicate = predicate.And(t => patientNos.Contains(t.PatientNo) || t.BalanceNo.Contains(inPut.keyword));
                    }

                    var data = await BalanceMainStore.Entities.Include(t => t.center).Where(predicate).AsNoTracking().ToListAsync();
                    var parNos = data.Select(t => t.PatientNo).ToList();
                    var RecipelNos = data.Select(t => t.BalanceNo).ToList();


                    //var BalanceDetailss = await BalanceDetailsStore.Entities.Where(t => RecipelNos.Contains(t.BalanceNo)).OrderByDescending(t => t.CreateDate).AsNoTracking().ToListAsync();
                    //var GroupBalanceDetailss = BalanceDetailss.GroupBy(t => t.RecipelNo);
                    count = data.Count() + 1;
                    decimal? sum = data.Sum(t => t.SumPrice);
                    // var BalanceDetailssPage = await PaginatedList<BalanceDetails>.CreateAsync(BalanceDetailss, inPut.PageNum, inPut.PageSize);
                    //var group = BalanceDetailssPage.GroupBy(m => new { m.RecipelNo });
                    //Num 页面 size 条数 1  40  var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
                    //foreach (var item in BalanceDetailss)
                    //{

                    //}

                    int PageCount = (((inPut.PageNum - 1) * inPut.PageSize) + inPut.PageSize - 1);
                    int PageIndex = ((inPut.PageNum - 1) * inPut.PageSize);

                    bool LastSize = (count / inPut.PageSize) + ((count % inPut.PageSize) > 0 ? 1 : 0) == inPut.PageNum ? true : false;
                    var SizeData = data.Skip((inPut.PageNum - 1) * inPut.PageSize).Take(inPut.PageSize);
                    var TempBaNo = SizeData.Select(t => t.RecipelNos).ToList();
                    List<string> ReNo = new List<string>();
                    foreach (var item in TempBaNo)
                    {
                        var itemTemp = item.Split(',').ToList();
                        foreach (var items in itemTemp)
                        {
                            ;
                            ReNo.Add(items.Replace("’", ""));
                        }

                    }
                    ReNo = ReNo.CompareDistinct(t => t).ToList();
                    //获取上传医保信息 并获取医保结算信息
                    // var JsLsList = await SI_CFMXStore.Entities.Where(t => ReNo.Contains(t.CFH)).AsNoTracking().ToListAsync();
                    var jsIdList = data.Where(t => t.SIBalanceSerialNo != null).Select(t => t.SIBalanceSerialNo); // JsLsList.Select(k => k.JSJYLSH).ToList().CompareDistinct(t => t);

                    Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime && jsIdList.Contains(t.JSJYLSH));
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    var JsData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();
                    //  var JsData = await SI_JSMXStore.Entities.Where(t => jsIdList.Contains(t.JSJYLSH)).AsNoTracking().ToListAsync();

                    foreach (var item in SizeData)
                    {
                        var patient = Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId);
                        if (patient == null) continue;
                        //  string ParentNo = data.Where(t => t.BalanceNo == item.FirstOrDefault().BalanceNo).FirstOrDefault().PatientNo;
                        RefundOutPut put = new RefundOutPut();
                        SI_JSMX sI_JSMX = new SI_JSMX();
                        sI_JSMX = item.SIBalanceSerialNo + "" != "" ?
                       JsData.Where(t => t.JSJYLSH == item.SIBalanceSerialNo).FirstOrDefault() : new SI_JSMX();
                        //new SI_JSMX();
                        ////对接医保信息
                        //string CFJYLSH = "";
                        //string firstNo = item.RecipelNos.Split(',').First().Replace("’", "");
                        //if ((JsLsList.Where(t => t.CFH == firstNo).FirstOrDefault() != null))
                        //{
                        //    CFJYLSH = (JsLsList.Where(t => t.CFH == firstNo).First().JSJYLSH);
                        //    sI_JSMX = JsData.Where(t => t.JSJYLSH == CFJYLSH).FirstOrDefault();
                        //}
                        put.SumPrice = item.SumPrice;// item.Sum(c => c.TotalPrice).Value;
                        put.CancelDate = item.BalanceDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        put.Age = DateTime.Now.Year - Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).Birthday.Value.Year;
                        if (sI_JSMX.CBLB != "")
                            put.HealthCareType = sI_JSMX.CBLB == "1" ? "职工参保" : "居民参保";
                        else
                            put.HealthCareType = Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).SIInsuredType == "390" ? "居民医保" : Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).SIInsuredType == "310" ? "职工医保" : "其他";


                        put.MZType = ((ALLCategories)Convert.ToInt32(sI_JSMX.YLLB)).ToChinese();




                        put.MZJZ = sI_JSMX.MZJZ.HasValue ? sI_JSMX.MZJZ : 0;
                        put.PName = Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).Name;
                        put.PrescriptionNo = item.BalanceNo;
                        put.Sex = Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).Sex;
                        put.WX = sI_JSMX.ZJE.HasValue ? item.SumPrice.Value - sI_JSMX.ZJE : 0;
                        put.DETC = (sI_JSMX.TCZF.HasValue && sI_JSMX.MZJZ.HasValue && sI_JSMX.YBZLZFS.HasValue) ? sI_JSMX.TCZF - sI_JSMX.MZJZ - sI_JSMX.YBZLZFS : 0;

                        put.MZJZ = sI_JSMX.MZJZ.HasValue ? sI_JSMX.MZJZ : 0;
                        put.XJ = sI_JSMX.XJZF.HasValue ? sI_JSMX.XJZF : item.SumPrice.Value;
                        put.YBJJ = sI_JSMX.GWYBZ + sI_JSMX.DBZDDYLJGDZ + sI_JSMX.DELP;
                        put.YYCB = sI_JSMX.CBKK.HasValue ? sI_JSMX.CBKK : 0;
                        put.ZHZF = sI_JSMX.ZHZF.HasValue ? sI_JSMX.ZHZF : 0;
                        put.No = PageIndex + 1;
                        put.Id = item.Id;
                        put.centerName = ReturnCenttSortName(item.center.ShortName);
                        refundOutPuts.Add(put);
                        PageIndex++;
                    }

                    if (LastSize)
                    {
                        RefundOutPut refundOutPut = new RefundOutPut();
                        refundOutPut.No = count;
                        refundOutPut.PName = "合计";
                        refundOutPut.MZJZ = JsData.Sum(t => t.MZJZ);
                        refundOutPut.DETC = JsData.Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                        refundOutPut.SumPrice = sum.Value;//refundOutPuts.Sum(t => t.SumPrice),
                        refundOutPut.WX = data.Where(t => t.BalanceType == 1).Sum(t => t.SumPrice) - JsData.Sum(t => t.ZJE);

                        refundOutPut.YBJJ = JsData.Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金,
                        refundOutPut.YYCB = JsData.Sum(t => t.CBKK);
                        refundOutPut.ZHZF = JsData.Sum(t => t.ZHZF);
                        refundOutPut.XJ = data.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice) + JsData.Sum(t => t.XJZF);

                        refundOutPuts.Add(refundOutPut);
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

                return new PageData<RefundOutPut[]>(refundOutPuts.ToArray(), count);
            });
        }

        //收费明细
        public Task<PageData<ChargeDetailOutPut[]>> GetChargeDetailAsync(RefundDetailInPut inPut)
        {
            return Task.Run(async () =>
            {
                int count = 1;
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                List<ChargeDetailOutPut> refundOutPuts = new List<ChargeDetailOutPut>();
                try
                {
                    var Parents = await PatientStore.Entities.AsNoTracking().ToListAsync();//患者
                    Expression<Func<BalanceMain, bool>> predicate = (t => inPut.BeginTime <= t.BalanceDate && inPut.EndTime > t.BalanceDate);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    predicate = predicate.And(t => t.CancelDate == null || t.CancelDate > inPut.EndTime);
                    if (inPut.keyword + "" != "")
                    {
                        List<string> patientNos = Parents.Where(t => t.Name.Contains(inPut.keyword)).Select(t => t.PatientNo).ToList();
                        predicate = predicate.And(t => patientNos.Contains(t.PatientNo) || t.RecipelNos.Contains(inPut.keyword));
                    }
                    var data = await BalanceMainStore.Entities.Include(t => t.center).Where(predicate).AsNoTracking().ToListAsync();
                    var parNos = data.Select(t => t.PatientNo).ToList();
                    var RecipelNos = data.Select(t => t.BalanceNo).ToList();

                    Expression<Func<BalanceDetails, bool>> depredicate = (t => RecipelNos.Contains(t.BalanceNo));
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        depredicate = depredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    var BalanceDetailss = await BalanceDetailsStore.Entities.Include(t => t.center).Where(depredicate).AsNoTracking().ToListAsync();

                    var BalanceDetailssPage = await PaginatedList<BalanceDetails>.CreateAsync(BalanceDetailss, inPut.PageNum, inPut.PageSize);
                    //   var group = BalanceDetailss.GroupBy(m => new { m.RecipelNo });
                    decimal? sum = data.Sum(t => t.SumPrice);
                    count = BalanceDetailssPage.Count + 1; //BalanceDetailss.Count() + 1;
                    bool LastSize = (count / inPut.PageSize) + ((count % inPut.PageSize) > 0 ? 1 : 0) == inPut.PageNum ? true : false;
                    //对接医保上传信息
                    List<string> SI_RecipelNos = BalanceDetailss.Select(t => t.RecipelNo).ToList();


                    Expression<Func<SI_CFMX, bool>> sipredicate = (t => SI_RecipelNos.Contains(t.CFH) && inPut.BeginTime <= t.JSRQ && inPut.EndTime > t.JSRQ);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    var SIUpdateData = await SI_CFMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();

                    int No = ((inPut.PageNum - 1) * inPut.PageSize) + 1;
                    foreach (var item in BalanceDetailssPage)
                    {
                        var tempData = SIUpdateData.Where(t => t.CFH == item.RecipelNo && t.YYNM == item.ItemCode && t.CenterId == item.CenterId).FirstOrDefault();
                        string ParentNo = data.Where(t => t.BalanceNo == item.BalanceNo && t.CenterId == item.CenterId).FirstOrDefault().PatientNo;
                        ChargeDetailOutPut put = new ChargeDetailOutPut();
                        put.No = No;
                        put.Qty = item.Qty;
                        put.OpenDate = data.Where(t => t.BalanceNo == item.BalanceNo).FirstOrDefault().BalanceDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        put.MZH = ParentNo;  //TODO:医保那几个表的说明添加后处理
                        put.PName = Parents.FirstOrDefault(t => t.PatientNo == ParentNo && t.CenterId == item.CenterId).Name;
                        put.PrescriptionNo = item.RecipelNo;
                        put.ItemName = item.ItemName;
                        put.CategoryName = item.CategoryName;
                        put.TotalPrice = item.TotalPrice;
                        put.UnitPrice = item.UnitPrice;
                        put.operationMan = item.OpenPersonName;
                        put.Specifications = item.Specifications;
                        put.Level = tempData == null ? "自费" : tempData.FYDJ == "1" ? "甲类" : tempData.FYDJ == "2" ? "乙类" : tempData.FYDJ == "3" ? "自费" : "混合";
                        put.PayAmount = tempData == null ? item.TotalPrice : tempData.ZFJE;
                        put.payProportion = tempData == null ? 100 : tempData.ZFBL;//100.0;
                        put.ISPayAmount = item.TotalPrice - put.PayAmount;
                        put.ISUpQty = tempData == null ? 0 : tempData.SL;//100.0;
                        put.centerName = ReturnCenttSortName(item.center.ShortName);
                        put.SIBalanceSerialNo = tempData == null ? "" : tempData.JSJYLSH;
                        refundOutPuts.Add(put);
                        No++;
                    }
                    //var sumq = SIUpdateData.Sum(t => (t.JE));

                    //var sum2 = data.Where(t=>t.BalanceType==2).Sum(t => t.SumPrice);
                    if (LastSize)
                    {
                        refundOutPuts.Add(new ChargeDetailOutPut()
                        {
                            No = count,
                            PName = "合计",
                            TotalPrice = sum,
                            ISUpQty = SIUpdateData.Sum(t => t.SL),
                            Qty = BalanceDetailss.Sum(t => t.Qty),
                            ISPayAmount = SIUpdateData.Sum(t => (t.JE - t.ZFJE)),
                            PayAmount = data.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice) + SIUpdateData.Sum(t => (t.ZFJE)),
                        });
                    }
                }
                catch (Exception exp)
                {

                }
                return new PageData<ChargeDetailOutPut[]>(refundOutPuts.ToArray(), count);
                //  return refundOutPuts.ToArray();
            });
        }

        /// <summary>
        /// 收费明细、退费明细
        /// </summary>
        /// <param name="RecipelNo"></param>
        /// <returns></returns>
        public Task<ChargeDetailOutPut[]> GetChargeDetailByIdAsync(string RecipelNo)
        {
            return Task.Run(async () =>
            {
                int count = 1;

                List<ChargeDetailOutPut> refundOutPuts = new List<ChargeDetailOutPut>();
                try
                {

                    var data = await BalanceMainStore.Entities.Where(t => t.Id == RecipelNo).FirstOrDefaultAsync();

                    var BalanceDetailss = await BalanceDetailsStore.Entities.Include(t => t.center).Where(t => t.BalanceNo == data.BalanceNo && t.CenterId == data.CenterId).AsNoTracking().ToListAsync();


                    var Parents = await PatientStore.Entities.Where(t => data.PatientNo == (t.PatientNo)).AsNoTracking().ToListAsync();//患者 
                                                                                                                                       //对接医保上传信息
                    List<string> SI_RecipelNos = BalanceDetailss.Select(t => t.RecipelNo).ToList();
                    var SIUpdateData = await SI_CFMXStore.Entities.Where(t => SI_RecipelNos.Contains(t.CFH)).AsNoTracking().ToListAsync();
                    decimal? sum = BalanceDetailss.Sum(t => t.TotalPrice);
                    count = BalanceDetailss.Count() + 1;
                    int no = 1;
                    foreach (var item in BalanceDetailss)
                    {
                        var tempData = SIUpdateData.Where(t => t.CFH == item.RecipelNo && t.YYNM == item.ItemCode).FirstOrDefault();
                        string ParentNo = data.PatientNo;
                        ChargeDetailOutPut put = new ChargeDetailOutPut();
                        put.No = no;
                        put.Qty = item.Qty;
                        put.OpenDate = data.BalanceDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        put.CancelDate = data.CancelDate.HasValue ? data.CancelDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                        put.MZH = ParentNo;  //TODO:医保那几个表的说明添加后处理
                        put.PName = Parents.FirstOrDefault(t => t.PatientNo == ParentNo && t.CenterId == item.CenterId).Name;
                        put.PrescriptionNo = item.RecipelNo;
                        put.ItemName = item.ItemName;
                        put.TotalPrice = item.TotalPrice;
                        put.UnitPrice = item.UnitPrice;

                        put.Specifications = item.Specifications;
                        put.operationMan = item.OpenPersonName;
                        put.Level = tempData == null ? "自费" : tempData.FYDJ == "1" ? "甲类" : tempData.FYDJ == "2" ? "乙类" : tempData.FYDJ == "3" ? "自费" : "混合";
                        put.PayAmount = tempData == null ? item.TotalPrice : tempData.ZFJE;
                        put.payProportion = tempData == null ? 100 : tempData.ZFBL;//100.0;
                        put.ISPayAmount = item.TotalPrice - put.PayAmount;
                        put.ISUpQty = tempData == null ? 0 : tempData.SL;//100.0;
                        put.centerName = ReturnCenttSortName(item.center.ShortName);
                        refundOutPuts.Add(put);
                        no++;
                    }

                    refundOutPuts.Add(new ChargeDetailOutPut()
                    {
                        No = count,
                        PName = "合计",
                        TotalPrice = sum
                    });
                }
                catch (Exception exp)
                {

                }
                return refundOutPuts.ToArray();
                //  return refundOutPuts.ToArray();
            });
        }

        //退费清单 RefundQueryInPut RefundOutPut
        public Task<PageData<RefundOutPut[]>> GetRefundAsync(RefundQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                List<RefundOutPut> refundOutPuts = new List<RefundOutPut>();
                int count = 0;
                try
                {
                    var Parents = await PatientStore.Entities.AsNoTracking().ToListAsync();//在院患者
                    Expression<Func<BalanceMain, bool>> predicate = (t => inPut.BeginTime < t.CancelDate && inPut.EndTime > t.CancelDate && t.CancelFlag == 1);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));

                    if (inPut.keyword + "" != "")
                    {
                        List<string> patientNos = Parents.Where(t => t.Name.Contains(inPut.keyword)).Select(t => t.PatientNo).ToList();
                        predicate = predicate.And(t => patientNos.Contains(t.PatientNo) || t.BalanceNo.Contains(inPut.keyword));
                    }

                    var data = await BalanceMainStore.Entities.Include(t => t.center).Where(predicate).AsNoTracking().ToListAsync();
                    var parNos = data.Select(t => t.PatientNo).ToList();
                    var RecipelNos = data.Select(t => t.BalanceNo).ToList();


                    //var BalanceDetailss = await BalanceDetailsStore.Entities.Where(t => RecipelNos.Contains(t.BalanceNo)).OrderByDescending(t => t.CreateDate).AsNoTracking().ToListAsync();
                    //var GroupBalanceDetailss = BalanceDetailss.GroupBy(t => t.RecipelNo);
                    count = data.Count() + 1;
                    decimal? sum = data.Sum(t => t.SumPrice);
                    // var BalanceDetailssPage = await PaginatedList<BalanceDetails>.CreateAsync(BalanceDetailss, inPut.PageNum, inPut.PageSize);
                    //var group = BalanceDetailssPage.GroupBy(m => new { m.RecipelNo });
                    //Num 页面 size 条数 1  40  var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
                    //foreach (var item in BalanceDetailss)
                    //{

                    //}

                    int PageCount = (((inPut.PageNum - 1) * inPut.PageSize) + inPut.PageSize - 1);
                    int PageIndex = ((inPut.PageNum - 1) * inPut.PageSize);

                    bool LastSize = PageCount >= count ? true : false;
                    var SizeData = data.Skip((inPut.PageNum - 1) * inPut.PageSize).Take(inPut.PageSize);
                    var TempBaNo = SizeData.Select(t => t.RecipelNos).ToList();
                    List<string> ReNo = new List<string>();
                    foreach (var item in TempBaNo)
                    {
                        var itemTemp = item.Split(',').ToList();
                        foreach (var items in itemTemp)
                        {
                            ;
                            ReNo.Add(items.Replace("'", ""));
                        }

                    }
                    ReNo = ReNo.CompareDistinct(t => t).ToList();
                    //获取上传医保信息 并获取医保结算信息
                    //var JsLsList = await SI_CFMXStore.Entities.Where(t => ReNo.Contains(t.CFH)).AsNoTracking().ToListAsync();
                    //var jsIdList = JsLsList.Select(k => k.JSJYLSH).ToList().CompareDistinct(t => t);

                    var ListJslsh = data.Where(t => t.SISourceBalanceNo + "" != "").Select(t => t.SISourceBalanceNo).ToList();

                    var JsData = await SI_JSMXStore.Entities.Where(t => ListJslsh.Contains(t.JSJYLSH)).AsNoTracking().ToListAsync();

                    foreach (var item in SizeData)
                    {
                        //  string ParentNo = data.Where(t => t.BalanceNo == item.FirstOrDefault().BalanceNo).FirstOrDefault().PatientNo;
                        RefundOutPut put = new RefundOutPut();
                        SI_JSMX sI_JSMX = JsData.Where(t => t.JSJYLSH == item.SISourceBalanceNo).FirstOrDefault();// new SI_JSMX();
                                                                                                                  //对接医保信息
                        string CFJYLSH = "";
                        //string firstNo = item.RecipelNos.Split(',').First().Replace("'", "");
                        //if ((JsLsList.Where(t => t.CFH == firstNo).FirstOrDefault() != null))
                        //{
                        //    CFJYLSH = (JsLsList.Where(t => t.CFH == firstNo).First().JSJYLSH);
                        //    sI_JSMX = JsData.Where(t => t.JSJYLSH == CFJYLSH).FirstOrDefault();
                        //}
                        sI_JSMX = sI_JSMX == null ? new SI_JSMX() : sI_JSMX;
                        put.SumPrice = item.SumPrice;// item.Sum(c => c.TotalPrice).Value;
                        put.CancelDate = item.CancelDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        if (Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId) != null && Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).Birthday.HasValue)
                            put.Age = DateTime.Now.Year - Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).Birthday.Value.Year;
                        else
                            put.Age = 0;
                        put.HealthCareType = Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).SIInsuredType == "390" ? "居民医保" : Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).SIInsuredType == "310" ? "职工医保" : "其他";


                        put.MZType = ((ALLCategories)Convert.ToInt32(sI_JSMX.YLLB)).ToChinese();



                        put.MZJZ = sI_JSMX.MZJZ.HasValue ? sI_JSMX.MZJZ : 0;
                        put.PName = Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).Name;
                        put.PrescriptionNo = item.BalanceNo;
                        put.Sex = Parents.FirstOrDefault(t => t.PatientNo == item.PatientNo && t.CenterId == item.CenterId).Sex;
                        put.WX = sI_JSMX.ZJE.HasValue ? item.SumPrice.Value - sI_JSMX.ZJE : 0;
                        put.DETC = (sI_JSMX.TCZF.HasValue && sI_JSMX.MZJZ.HasValue && sI_JSMX.YBZLZFS.HasValue) ? sI_JSMX.TCZF - sI_JSMX.MZJZ - sI_JSMX.YBZLZFS : 0;
                        put.MZJZ = sI_JSMX.MZJZ.HasValue ? sI_JSMX.MZJZ : 0;
                        put.XJ = sI_JSMX.XJZF.HasValue ? sI_JSMX.XJZF : item.SumPrice.Value;
                        put.YBJJ = (sI_JSMX.GWYBZ + sI_JSMX.DBZDDYLJGDZ + sI_JSMX.DELP).HasValue ? sI_JSMX.GWYBZ + sI_JSMX.DBZDDYLJGDZ + sI_JSMX.DELP : 0;
                        put.YYCB = sI_JSMX.CBKK.HasValue ? sI_JSMX.CBKK : 0;
                        put.ZHZF = sI_JSMX.ZHZF.HasValue ? sI_JSMX.ZHZF : 0;
                        put.oth_pay = sI_JSMX.oth_pay.HasValue ? sI_JSMX.oth_pay : 0;
                        put.hifes_pay = sI_JSMX.hifes_pay.HasValue ? sI_JSMX.hifes_pay : 0;
                        put.GWYBZ = sI_JSMX.GWYBZ.HasValue ? sI_JSMX.GWYBZ : 0;
                        put.No = PageIndex + 1;
                        put.Id = item.Id;
                        put.BalanceDate = item.BalanceDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        put.centerName = ReturnCenttSortName(item.center.ShortName);
                        refundOutPuts.Add(put);
                        PageIndex++;
                    }

                    if (LastSize)
                        refundOutPuts.Add(new RefundOutPut()
                        {
                            No = count,
                            PName = "合计",
                            MZJZ = JsData.Sum(t => t.MZJZ),
                            DETC = JsData.Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS)),
                            SumPrice = sum.Value,//refundOutPuts.Sum(t => t.SumPrice),
                            WX = data.Where(t => t.BalanceType == 1).Sum(t => t.SumPrice) - JsData.Sum(t => t.ZJE),
                            XJ = data.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice) + JsData.Sum(t => t.XJZF),
                            YBJJ = JsData.Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP)),
                            YYCB = JsData.Sum(t => t.CBKK),
                            ZHZF = JsData.Sum(t => t.ZHZF),
                        });
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

                return new PageData<RefundOutPut[]>(refundOutPuts.ToArray(), count);
            });
        }


        //退费明细 RefundDetailOutPut RefundDetailInPut

        public Task<PageData<RefundDetailOutPut[]>> GetRefundDetailAsync(RefundDetailInPut inPut)
        {
            return Task.Run(async () =>
          {

              int count = 1;
              inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
              inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
              List<RefundDetailOutPut> refundOutPuts = new List<RefundDetailOutPut>();
              try
              {
                  var Parents = await PatientStore.Entities.AsNoTracking().ToListAsync();//患者
                  Expression<Func<BalanceMain, bool>> predicate = (t => inPut.BeginTime < t.CancelDate && inPut.EndTime > t.CancelDate && t.CancelFlag == 1);
                  if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                      predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                  if (inPut.keyword + "" != "")
                  {
                      List<string> patientNos = Parents.Where(t => t.Name.Contains(inPut.keyword)).Select(t => t.PatientNo).ToList();
                      predicate = predicate.And(t => patientNos.Contains(t.PatientNo) || t.RecipelNos.Contains(inPut.keyword));
                  }
                  var data = await BalanceMainStore.Entities.Where(predicate).AsNoTracking().ToListAsync();

                  var parNos = data.Select(t => t.PatientNo).ToList();
                  var RecipelNos = data.Select(t => t.BalanceNo).ToList();

                  Expression<Func<BalanceDetails, bool>> depredicate = (t => RecipelNos.Contains(t.BalanceNo));
                  if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                      depredicate = depredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                  var BalanceDetailss = await BalanceDetailsStore.Entities.Include(t => t.center).Where(depredicate).OrderByDescending(t => t.CreateDate).AsNoTracking().ToListAsync();

                  var BalanceDetailssPage = await PaginatedList<BalanceDetails>.CreateAsync(BalanceDetailss, inPut.PageNum, inPut.PageSize);
                  //   var group = BalanceDetailss.GroupBy(m => new { m.RecipelNo });
                  int No = ((inPut.PageNum - 1) * inPut.PageSize) + 1;
                  decimal? sum = BalanceDetailss.Sum(t => t.TotalPrice);
                  count = BalanceDetailss.Count() + 1;
                  bool LastSize = (count / inPut.PageSize) + ((count % inPut.PageSize) > 0 ? 1 : 0) == inPut.PageNum ? true : false;
                  foreach (var item in BalanceDetailssPage)
                  {
                      string ParentNo = data.Where(t => t.BalanceNo == item.BalanceNo && t.CenterId == item.CenterId).FirstOrDefault().PatientNo;
                      RefundDetailOutPut put = new RefundDetailOutPut();
                      put.No = No;
                      put.Qty = item.Qty;
                      put.CancelDate = data.Where(t => t.BalanceNo == item.BalanceNo).FirstOrDefault().CancelDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                      put.MZH = ParentNo;  //TODO:医保那几个表的说明添加后处理
                      put.PName = Parents.FirstOrDefault(t => t.PatientNo == ParentNo && t.CenterId == item.CenterId).Name;
                      put.PrescriptionNo = item.RecipelNo;
                      put.ItemName = item.ItemName;
                      put.TotalPrice = item.TotalPrice;
                      put.UnitPrice = item.UnitPrice;
                      put.operationMan = item.OpenPersonName;
                      put.centerName = ReturnCenttSortName(item.center.ShortName);
                      refundOutPuts.Add(put);
                      No++;
                  }

                  if (LastSize)
                      refundOutPuts.Add(new RefundDetailOutPut()
                      {
                          No = count,
                          PName = "合计",
                          TotalPrice = sum
                      });
              }
              catch (Exception exp)
              {

              }
              return new PageData<RefundDetailOutPut[]>(refundOutPuts.ToArray(), count);
              //  return refundOutPuts.ToArray();
          });
        }

        /* 
        季度 /年度盈利增长	收入增减变动表	按降序排列各透析中心
                        成本增减变动表	按降序排列各透析中心
                        毛利增减变动表	按降序排列各透析中心
        */

        // 季度 /年度盈利增长 收入增减变动表	按降序排列各透析中心 CostChangeOutPut CostChangeInput
        /// <summary>
        /// 收入增减变动表
        /// </summary>
        /// <param name="inPut">入参</param>
        /// <returns></returns>
        public Task<object> GetincomeChangeAsync(IncomeChangeInput inPut)
        {

            return Task.Run(async () =>
            {
                List<IncomeChangeData> outPuts = new List<IncomeChangeData>();
                List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                try
                {

                    //本期时间 
                    inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                    inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                    //上期时间
                    int Month = (inPut.EndTime.Year - inPut.BeginTime.Year) * 12 + (inPut.EndTime.Month - inPut.BeginTime.Month);
                    if (Month == 0) Month = 1;
                    DateTime UpBeginTime = inPut.BeginTime.AddMonths(-Month);
                    Expression<Func<BalancePayType, bool>> predicate = t => t.CreateDate >= UpBeginTime && t.CreateDate <= inPut.EndTime && t.DataState == 1;
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicate = predicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    //上期-本期数据
                    var DayMoney = await BalancePayTypeStore.Entities.Include(t => t.centerDialysis).Where(predicate).AsNoTracking().ToListAsync();

                    Expression<Func<BalanceMain, bool>> mainpredicate = (t => t.BalanceDate >= UpBeginTime && t.BalanceDate < inPut.EndTime && t.BalanceState == 1);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        mainpredicate = mainpredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    //结算总表
                    var BalanceData = await BalanceMainStore.Entities.Where(mainpredicate).AsNoTracking().ToListAsync();

                    // 医保结算  
                    Expression<Func<SI_JSMX, bool>> sipredicate = (t => t.JSRQ >= UpBeginTime && t.JSRQ < inPut.EndTime);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        sipredicate = sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    var SiData = await SI_JSMXStore.Entities.Where(sipredicate).AsNoTracking().ToListAsync();




                    //上年  
                    Expression<Func<BalancePayType, bool>> predicates = t => t.CreateDate >= inPut.BeginTime.AddYears(-1) && t.CreateDate <= inPut.EndTime.AddYears(-1);
                    if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                        predicates = predicates.And(t => inPut.CenterId.Contains(t.CenterId));
                    var UpDayMoney = await BalancePayTypeStore.Entities.Include(t => t.centerDialysis).Where(predicates).AsNoTracking().ToListAsync();


                    //Expression<Func<BalanceMain, bool>> upmainpredicate = (t => t.BalanceDate >= inPut.BeginTime.AddYears(-1) && t.BalanceDate < inPut.EndTime.AddYears(-1) && t.BalanceState == 1);
                    //if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    //    upmainpredicate = upmainpredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    ////结算总表
                    //var upBalanceData = await BalanceMainStore.Entities.Where(upmainpredicate).AsNoTracking().ToListAsync();

                    //// 医保结算
                    //Expression<Func<SI_JSMX, bool>> upsipredicate = (t => t.JSRQ >= inPut.BeginTime.AddYears(-1) && t.JSRQ < inPut.EndTime.AddYears(-1));
                    //if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    //    upsipredicate = upsipredicate.And(t => inPut.CenterId.Contains(t.CenterId));
                    //var upSiData = await SI_JSMXStore.Entities.Where(upsipredicate).AsNoTracking().ToListAsync();


                    if (inPut.CenterId.Contains("0"))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    for (int i = 0; i < CenterDialysiss.Count; i++)
                    {
                        var item = CenterDialysiss[i];
                        //   put.TotalMoney = BalanceData.Where(t => t.SIBalanceSerialNo != null).Sum(t => t.SumPrice) - SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);

                        List<string> SiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id && t.BalanceDate >= inPut.BeginTime).Select(t => t.SIBalanceSerialNo).ToList();
                        List<string> upSiBsNo = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id && t.BalanceDate < inPut.BeginTime).Select(t => t.SIBalanceSerialNo).ToList();
                        IncomeChangeData put = new IncomeChangeData();
                        put.no = i + 1;
                        put.CenterName = ReturnCenttSortName(item.ShortName);

                        var de = SiData.Where(t => SiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);
                        var dee = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id && t.BalanceDate >= inPut.BeginTime).Sum(t => t.SumPrice);

                        var upde = SiData.Where(t => upSiBsNo.Contains(t.JSJYLSH)).Sum(t => t.ZJE);
                        var updee = BalanceData.Where(t => t.SIBalanceSerialNo != null && t.CenterId == item.Id && t.BalanceDate < inPut.BeginTime).Sum(t => t.SumPrice);

                        put.NowIncome = DayMoney.Where(t => t.CenterId == item.Id && t.CreateDate >= inPut.BeginTime).Sum(t => t.CostMoney) + (dee - de);



                        put.UpIncome = DayMoney.Where(t => t.CenterId == item.Id && t.CreateDate < inPut.BeginTime).Sum(t => t.CostMoney) + (updee - upde);


                        put.DifferIncome = put.NowIncome - put.UpIncome;
                        put.DifferRate = $"{((put.DifferIncome / (put.UpIncome == 0 ? 1 : put.UpIncome)) * 100).Value.ToRounds()} % ";

                        put.UpYearNowIncome = UpDayMoney.Where(t => t.CenterId == item.Id).Sum(t => t.CostMoney);
                        put.UpYearDifferIncome = put.NowIncome - put.UpYearNowIncome;
                        put.UpYearDifferRate = $"{((put.UpYearDifferIncome / (put.UpYearNowIncome == 0 ? 1 : put.UpYearNowIncome)) * 100).Value.ToRounds()} % ";
                        outPuts.Add(put);
                    }


                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return (object)outPuts.ToArray();

            });

        }


        // 成本增减变动表	按降序排列各透析中心
        /// <summary>
        /// 成本增减变动表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<object> GetCostChangeAsync(IncomeChangeInput inPut)
        {

            return Task.Run(async () =>
            {
                List<IncomeChangeData> outPuts = new List<IncomeChangeData>();
                List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();

                //本期时间
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                //上期时间
                int Month = (inPut.EndTime.Year - inPut.BeginTime.Year) * 12 + (inPut.EndTime.Month - inPut.BeginTime.Month);
                if (Month == 0) Month = 1;
                DateTime UpBeginTime = inPut.BeginTime.AddMonths(-Month);

                var list = new List<CostModel>();

                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetSumCost", "成本总汇");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = string.Format(sql, UpBeginTime, inPut.EndTime);//上期与本期
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                var tuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(dt);

                //经营成本 本期+上期
                var data = await WaterFuelsStore.Entities.Where(t => t.IsDelete == false && t.Month >= UpBeginTime && t.Month <= inPut.EndTime).AsNoTracking().ToListAsync();

                //上年
                var upxmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetSumCost", "成本总汇");
                string upsql = GetQuerySql(xmlSqlParameter);
                upsql = string.Format(upsql, inPut.BeginTime.AddYears(-1), inPut.EndTime.AddYears(-1));//上期与本期
                var updt = MsSqlHelper.GetSingleObj().GetDataTable(upsql);
                var uptuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(updt);

                //经营成本 本期+上期
                var updata = await WaterFuelsStore.Entities.Where(t => t.IsDelete == false && t.Month >= inPut.BeginTime.AddYears(-1) && t.Month <= inPut.EndTime.AddYears(-1)).AsNoTracking().ToListAsync();
                if (inPut.CenterId.Contains("0"))
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                else
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                for (int i = 0; i < CenterDialysiss.Count; i++)
                {
                    var item = CenterDialysiss[i];
                    var OperatingCost = data.Where(a => a.CenterId == item.Id && a.Month >= inPut.BeginTime).Sum(a => a.AmountPriec);
                    var SumCost = tuple.Item1 == false ? 0 : tuple.Item2.FindAll(a => a.CenterId == item.Id && a.putInstorageDate >= inPut.BeginTime && (a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" || a.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || a.ItemType == "01f769807f9a4736be41db800606447e")).Sum(a => (a.InPrice * a.ItemQty)) - tuple.Item2.FindAll(a => a.CenterId == item.Id && a.putInstorageDate >= inPut.BeginTime && a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => (a.InPrice * a.ItemQty));

                    var upOperatingCost = data.Where(a => a.CenterId == item.Id && a.Month < inPut.BeginTime).Sum(a => a.AmountPriec);
                    var upSumCost = tuple.Item1 == false ? 0 : tuple.Item2.FindAll(a => a.CenterId == item.Id && a.putInstorageDate < inPut.BeginTime && (a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" || a.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || a.ItemType == "01f769807f9a4736be41db800606447e")).Sum(a => (a.InPrice * a.ItemQty)) - tuple.Item2.FindAll(a => a.CenterId == item.Id && a.putInstorageDate < inPut.BeginTime && a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => (a.InPrice * a.ItemQty));

                    var upyearOperatingCost = updata.Where(a => a.CenterId == item.Id).Sum(a => a.AmountPriec);
                    var upyearSumCost = uptuple.Item1 == false ? 0 : uptuple.Item2.FindAll(a => a.CenterId == item.Id && (a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" || a.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || a.ItemType == "01f769807f9a4736be41db800606447e")).Sum(a => (a.InPrice * a.ItemQty)) - uptuple.Item2.FindAll(a => a.CenterId == item.Id && a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => (a.InPrice * a.ItemQty));


                    IncomeChangeData put = new IncomeChangeData();
                    put.CenterName = ReturnCenttSortName(item.ShortName);

                    put.no = i + 1;
                    put.CenterName = ReturnCenttSortName(item.ShortName);
                    put.NowIncome = OperatingCost + SumCost;
                    put.UpIncome = upOperatingCost + upSumCost;
                    put.DifferIncome = put.NowIncome - put.UpIncome;
                    put.DifferRate = $"{((put.DifferIncome / (put.UpIncome == 0 ? 1 : put.UpIncome)) * 100).Value.ToRounds()} % ";

                    put.UpYearNowIncome = upyearOperatingCost + upyearSumCost;
                    put.UpYearDifferIncome = put.NowIncome - put.UpYearNowIncome;
                    put.UpYearDifferRate = $"{((put.UpYearDifferIncome / (put.UpYearNowIncome == 0 ? 1 : put.UpYearNowIncome)) * 100).Value.ToRounds()} % ";
                    outPuts.Add(put);

                }



                return (object)outPuts.ToArray();
            });

        }


        //毛利增减变动表	按降序排列各透析中心Gross profit
        /// <summary>
        /// 毛利增减变动表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetGrossPofitChangeAsync(IncomeChangeInput inPut)
        {
            return Task.Run(async () =>
            {
                List<ProfitChangeData> outPuts = new List<ProfitChangeData>();
                List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                var CostData = await GetCostChangeAsync(inPut) as IncomeChangeData[];
                var Income = await GetincomeChangeAsync(inPut) as IncomeChangeData[];
                if (inPut.CenterId.Contains("0"))
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                else
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                for (int i = 0; i < CenterDialysiss.Count; i++)
                {

                    var item = CenterDialysiss[i];
                    var centerName = ReturnCenttSortName(item.ShortName);
                    var CenIncome = Income.Where(t => t.CenterName == centerName).FirstOrDefault();
                    var CenCost = CostData.Where(t => t.CenterName == centerName).FirstOrDefault();
                    ProfitChangeData data = new ProfitChangeData();
                    data.no = i + 1;
                    data.CenterName = centerName;
                    data.NowIncome = CenIncome.NowIncome;
                    //data.NowIncome = CenIncome.NowIncome;
                    data.NowCost = CenCost.NowIncome;
                    data.NowProfit = CenIncome.NowIncome - CenCost.NowIncome;
                    data.UpIncome = CenIncome.UpIncome;
                    data.UpCost = CenCost.UpIncome;
                    data.UpYearNowIncome = CenIncome.UpYearNowIncome;
                    data.UpYearNowCost = CenCost.UpYearNowIncome;
                    data.NowProfit = data.NowIncome - data.NowCost;
                    data.ProfitRate = (data.NowProfit / (data.NowIncome > 0 ? data.NowIncome : 1)).Value.ToRounds();
                    data.upProfit = data.UpIncome - data.UpCost;
                    data.upProfitRate = (data.upProfit / (data.UpIncome > 0 ? data.UpIncome : 1)).Value.ToRounds();
                    data.DifferIncome = CenIncome.DifferIncome;
                    data.DifferCost = CenCost.DifferIncome;
                    data.DifferProfit = data.NowProfit - data.upProfit;
                    data.ProfitRateRate = data.ProfitRate - data.upProfitRate;
                    data.UpYearNowProfit = data.UpYearNowIncome - data.UpYearNowCost;
                    data.UpYearProfitRate = (data.UpYearNowProfit / (data.UpYearNowIncome > 0 ? data.UpYearNowIncome : 1).Value.ToRounds());
                    data.UpYearDifferIncome = CenIncome.UpYearDifferIncome;
                    data.UpYearDifferCost = CenCost.UpYearDifferIncome;
                    data.UpYearDifferProfit = data.NowProfit - data.UpYearNowProfit;
                    data.UpYearDifferProfitRate = data.ProfitRate - data.UpYearProfitRate;
                    outPuts.Add(data);
                }
                return (object)outPuts.ToArray();

            });

        }

        //


        #endregion

        //插入测试数据

        private int ReturnTest()
        {
            //double num = 0.80;

            var rd = new Random();
            return rd.Next(2000, 5000);
        }
        private int ReturnTests()
        {
            //double num = 0.80;

            var rd = new Random();
            return rd.Next(50, 200);
        }

        public void AddData()
        {

            var CenterData = CenterDialysisStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
            List<EnumberEntity> data = new List<EnumberEntity>();
            var ttt = SystemDictionaryStore.Entities.Where(t => t.IsDelete == false && t.TypeId == "31166b331f7b2596974b6977b9fd5ac2").AsNoTracking().ToListAsync().Result;
            var hdf = SystemDictionaryStore.Entities.Where(t => t.IsDelete == false && t.TypeId == "e2d0fa22cc114841b86d7040ba144829").AsNoTracking().ToListAsync().Result;
            foreach (var Center in hdf)
            {

                foreach (var items in ttt)
                {
                    int C = ReturnTest();
                    int L = ReturnTest();
                    int Y = ReturnTests();
                    DayItemIncome medicalIndexesMonth = new DayItemIncome()
                    {
                        Id = Guid.NewGuid().tostring32(),
                        Cost = items.Id,
                        CostMoney = C,
                        CurePattern = Center.Id,
                        DiscountsMoney = Y,
                        IncomeMoney = L,
                        SettlementDate = DateTime.Now,
                        Source = "测试",
                        CenterId = "abc3d60b474c4fef946a66887348c41a",// Center.Id,
                    };
                    DayItemIncomeStore.Insert(medicalIndexesMonth);
                }
            }

            _unitOfWork.SaveChanges();
        }




        #region --------2022收支报表新接口-------------
        /// <summary>
        /// 收费项目
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetNewBusinessTargetsAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {


                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                int NoIndex = 1;
                string sqlWhere = "";
                if (inPut.CenterId.Count == 1 && !inPut.CenterId.Contains("0"))
                {
                    sqlWhere = $" and  bm.CenterId = '{inPut.CenterId[0]}' and bd.CenterId = '{inPut.CenterId[0]}' ";
                }
                string QuerySql = $@"
--按收费项目
--本月正常收费数据
select SUM(d.TotalPrice) as TotalPrice ,d.FreeTypeName,d.CenterId from (
select  b.TotalPrice ,b.FreeTypeName ,b.CenterId  from (
select bd.TotalPrice,bd.CenterId,s.Name as FreeTypeName from  BalanceMain  as bm 
left join BalanceDetails as bd  on bm.BalanceNo = bd.BalanceNo and  bm.CenterId =bd.CenterId
left join MedicalItemRecords m on m.Id = bd.ItemId
left join SystemDictionarys s on s.Id = m.FeeTypeId
where     bm.BalanceState =1 and  bm.BalanceDate >'{inPut.BeginTime}'and  bm.BalanceDate <'{inPut.EndTime}' {sqlWhere}
) b    
  UNION ALL -- 跨月退费数据
select  0-c.TotalPrice as TotalPrice,c.FreeTypeName, c.CenterId from (
select  bd.TotalPrice,bd.CenterId,s.Name as FreeTypeName from BalanceMain  as bm
left join  BalanceDetails as bd  on bm.BalanceNo = bd.BalanceNo and bm.CenterId =bd.CenterId
left join MedicalItemRecords m on m.Id = bd.ItemId
left join SystemDictionarys s on s.Id = m.FeeTypeId
where      bm.BalanceState =0 and  bm.BalanceDate <'{inPut.BeginTime}'and  bm.CancelDate >'{inPut.BeginTime}' and  bm.CancelDate <'{inPut.EndTime}' {sqlWhere}
) c 
UNION ALL -- 次月退费数据

select e.TotalPrice as TotalPrice,e.FreeTypeName, e.CenterId from (
select  bd.TotalPrice,bd.CenterId,s.Name as FreeTypeName from    BalanceMain  as bm
left join BalanceDetails as bd on bm.BalanceNo = bd.BalanceNo and bm.CenterId =bd.CenterId
left join MedicalItemRecords m on m.Id = bd.ItemId
left join SystemDictionarys s on s.Id = m.FeeTypeId
where      bm.BalanceState =0 and  bm.BalanceDate >'{inPut.BeginTime}'and  bm.BalanceDate <'{inPut.EndTime}' and  bm.CancelDate >'{inPut.EndTime}' {sqlWhere}
) e
 ) d group by d.FreeTypeName ,d.CenterId  ";
                if (inPut.CenterId != null && (inPut.CenterId.Count > 1 || inPut.CenterId.Contains("0")))
                {
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId.Contains("0"))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    #region  ----------表头----------
                    List<TableHeader> tableHeaders = new List<TableHeader>();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "序号",
                        Key = "no",
                    });
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        width = 125,
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });
                    }
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Key = "total",
                    });
                    #endregion


                    List<CenterDayIncomeOutPut> dayIncomes = new List<CenterDayIncomeOutPut>();
                    var sqlParameterList = new List<SqlParameter>();


                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySql, sqlParameterList.ToArray());
                    var tuple = GetTupleByList<ChargeTable>(dt);
                    var ChargeType = tuple.Item2.Select(t => t.FreeTypeName).ToList().Distinct().ToList();
                    foreach (var item in ChargeType)
                    {
                        CenterDayIncomeOutPut dayIncomeOutPut = new CenterDayIncomeOutPut();
                        dayIncomeOutPut.itemName = item;

                        decimal total = 0;
                        foreach (var items in CenterDialysiss)
                        {

                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? number = tuple.Item2.Where(t => t.CenterId == items.Id && t.FreeTypeName == item).Sum(t => t.TotalPrice);
                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, number);
                            number = number.HasValue ? number : 0;
                            total += Convert.ToDecimal(dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().GetValue(dayIncomeOutPut));
                        }
                        dayIncomeOutPut.total = total;
                        dayIncomeOutPut.no = NoIndex++;
                        dayIncomes.Add(dayIncomeOutPut);
                    }

                    dayIncomes.Add(new CenterDayIncomeOutPut() { no = NoIndex++, itemName = "合计", dzsmzb = dayIncomes.Sum(t => t.dzsmzb), jlptxzx = dayIncomes.Sum(t => t.jlptxzx), kmtxzx = dayIncomes.Sum(t => t.kmtxzx), spbtxzx = dayIncomes.Sum(t => t.spbtxzx), tjmzb = dayIncomes.Sum(t => t.tjmzb), tltxzx = dayIncomes.Sum(t => t.tltxzx), total = dayIncomes.Sum(t => t.total), xstxzx = dayIncomes.Sum(t => t.xstxzx), zxtxzx = dayIncomes.Sum(t => t.zxtxzx) });
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.CenterDayIncomeOutPuts = dayIncomes;
                    return (object)aLLDayIncomeOutPut;
                }
                else
                {
                    var sqlParameterList = new List<SqlParameter>();
                    List<DayIncomeOutPut> dayIncomes = new List<DayIncomeOutPut>();

                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySql, sqlParameterList.ToArray());
                    var tuple = GetTupleByList<ChargeTable>(dt);
                    foreach (var item in tuple.Item2)
                    {
                        dayIncomes.Add(new DayIncomeOutPut() { TotalMoney = item.TotalPrice.Value.ToRounds(), ItemName = item.FreeTypeName, no = NoIndex++ });
                    }

                    dayIncomes.Add(new DayIncomeOutPut()
                    {

                        ItemName = "合计",
                        TotalMoney = dayIncomes.Sum(t => t.TotalMoney),
                        no = NoIndex++
                    });
                    //
                    //
                    // dayIncomes.ForEach(t => { t.no = NoIndex; t.TotalMoney = t.TotalMoney.Value.ToRounds(); NoIndex++; });
                    return (object)dayIncomes;

                }
            });
        }
        /// <summary>
        /// 结算方式
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetNewBusinessPayTypeAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                //  List<DayIncomeOutPut> dayIncomes = new List<DayIncomeOutPut>();
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                int NoIndex = 1;
                // 
                if (inPut.CenterId != null && (inPut.CenterId.Count > 1 || inPut.CenterId.Contains("0")))
                {
                    List<CenterDayIncomeOutPut> dayIncomes = new List<CenterDayIncomeOutPut>();
                    ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId.Contains("0"))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();

                    #region
                    List<TableHeader> tableHeaders = new List<TableHeader>();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "序号",
                        Key = "no",
                    });
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });


                    }
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Key = "total",
                    });
                    #endregion
                    List<CenterDayIncomeOutPut> centerDayIncomeOutPuts = new List<CenterDayIncomeOutPut>();
                    List<ChargeTable> chargeTables = new List<ChargeTable>();
                    foreach (var items in CenterDialysiss)
                    {
                        string SqlQuery = $@"select SUM( TotalPrice ) as TotalPrice ,CenterId ,FreeTypeName as FreeTypeName  from (
select CostMoney  as  TotalPrice,CenterId,PayTypeName as FreeTypeName  from BalancePayType where CenterId = '{items.Id}' and CreateDate >'{inPut.BeginTime}'  and  CreateDate<'{inPut.EndTime}' and DataState =1  

  UNION ALL -- 跨月退费数据
  select( 0- CostMoney )as  TotalPrice,CenterId,PayTypeName as FreeTypeName  from BalancePayType where CenterId = '{items.Id}' and 
  BalanceNo   in (select bm.BalanceNo from BalanceMain as bm where bm.CenterId=  '{items.Id}' and bm.BalanceState=0 and bm.BalanceDate <'{inPut.BeginTime}'and  bm.CancelDate >'{inPut.BeginTime}' and  bm.CancelDate <'{inPut.EndTime}')
  UNION ALL -- 次月退费数据
  select  CostMoney as  TotalPrice,CenterId,PayTypeName as FreeTypeName  from BalancePayType where CenterId = '{items.Id}' and 
  BalanceNo   in (select bm.BalanceNo from BalanceMain as bm where bm.CenterId=  '{items.Id}' and bm.BalanceState=0 and bm.BalanceDate >'{inPut.BeginTime}'and  bm.BalanceDate <'{inPut.EndTime}' and  bm.CancelDate >'{inPut.EndTime}')

  ) a group by CenterId,FreeTypeName";
                        var sqlParameterList = new List<SqlParameter>();

                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(SqlQuery, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<ChargeTable>(dt);
                        chargeTables.AddRange(tuple.Item2);
                    }

                    var typeData = chargeTables.Select(t => t.FreeTypeName).ToList().Distinct().ToList();
                    foreach (var item in typeData)
                    {
                        CenterDayIncomeOutPut dayIncomeOutPut = new CenterDayIncomeOutPut();
                        dayIncomeOutPut.itemName = item;

                        decimal total = 0;
                        foreach (var items in CenterDialysiss)
                        {

                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? number = chargeTables.Where(t => t.CenterId == items.Id && t.FreeTypeName == item).Sum(t => t.TotalPrice);
                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, number);
                            number = number.HasValue ? number : 0;
                            total += Convert.ToDecimal(dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().GetValue(dayIncomeOutPut));
                        }
                        dayIncomeOutPut.total = total;
                        dayIncomeOutPut.no = NoIndex++;
                        dayIncomes.Add(dayIncomeOutPut);
                    }

                    dayIncomes.Add(new CenterDayIncomeOutPut() { no = NoIndex++, itemName = "合计", dzsmzb = dayIncomes.Sum(t => t.dzsmzb), jlptxzx = dayIncomes.Sum(t => t.jlptxzx), kmtxzx = dayIncomes.Sum(t => t.kmtxzx), spbtxzx = dayIncomes.Sum(t => t.spbtxzx), tjmzb = dayIncomes.Sum(t => t.tjmzb), tltxzx = dayIncomes.Sum(t => t.tltxzx), total = dayIncomes.Sum(t => t.total), xstxzx = dayIncomes.Sum(t => t.xstxzx), zxtxzx = dayIncomes.Sum(t => t.zxtxzx) });
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.CenterDayIncomeOutPuts = dayIncomes;
                    return (object)aLLDayIncomeOutPut;
                }
                else
                {
                    //
                    var sqlParameterList = new List<SqlParameter>();
                    List<DayIncomeOutPut> dayIncomes = new List<DayIncomeOutPut>();
                    string SqlQuery = $@"select SUM( TotalPrice ) as TotalPrice ,CenterId ,FreeTypeName as FreeTypeName  from (
select CostMoney  as  TotalPrice,CenterId,PayTypeName as FreeTypeName  from BalancePayType where CenterId = '{inPut.CenterId[0]}' and CreateDate >'{inPut.BeginTime}'  and  CreateDate<'{inPut.EndTime}' and DataState =1  

  UNION ALL -- 跨月退费数据
  select( 0- CostMoney )as  TotalPrice,CenterId,PayTypeName as FreeTypeName  from BalancePayType where CenterId = '{inPut.CenterId[0]}' and 
  BalanceNo   in (select bm.BalanceNo from BalanceMain as bm where bm.CenterId=  '{inPut.CenterId[0]}' and bm.BalanceState=0 and bm.BalanceDate <'{inPut.BeginTime}'and  bm.CancelDate >'{inPut.BeginTime}' and  bm.CancelDate <'{inPut.EndTime}')
  UNION ALL -- 次月退费数据
  select  CostMoney as  TotalPrice,CenterId,PayTypeName as FreeTypeName  from BalancePayType where CenterId = '{inPut.CenterId[0]}' and 
  BalanceNo   in (select bm.BalanceNo from BalanceMain as bm where bm.CenterId=  '{inPut.CenterId[0]}' and bm.BalanceState=0 and bm.BalanceDate >'{inPut.BeginTime}'and  bm.BalanceDate <'{inPut.EndTime}' and  bm.CancelDate >'{inPut.EndTime}')

  ) a group by CenterId,FreeTypeName";
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(SqlQuery, sqlParameterList.ToArray());
                    var tuple = GetTupleByList<ChargeTable>(dt);
                    foreach (var item in tuple.Item2)
                    {
                        dayIncomes.Add(new DayIncomeOutPut() { TotalMoney = item.TotalPrice.Value.ToRounds(), ItemName = item.FreeTypeName, no = NoIndex++ });
                    }

                    dayIncomes.Add(new DayIncomeOutPut()
                    {

                        ItemName = "合计",
                        TotalMoney = dayIncomes.Sum(t => t.TotalMoney),
                        no = NoIndex++
                    });
                    // dayIncomes.ForEach(t => { t.no = NoIndex; t.TotalMoney = t.TotalMoney.Value.ToRounds(); NoIndex++; });
                    return (object)dayIncomes;
                }

            });
        }
        /// <summary>
        /// 回款方式
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetNewBusinessBackTypeAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                List<string> ItemGroup = new List<string>() { "现金", "公务员补助", "个人账户(职工)", "民政救助(职工)", "统筹基金(职工)", "大额医疗(职工)", "企业补充医疗保险基金(职工)", "其他支出(职工)", "个人账户(居民)", "民政救助(居民)", "统筹基金(居民)", "大病保险(居民)", "企业补充医疗保险基金(居民)", "其他支出(居民)", "超标扣款", "开业优惠" };
                int NoIndex = 1;
                List<DayIncomeOutPut> dayIncomes = new List<DayIncomeOutPut>();
                DayReportOutPut dayReportOutPut = new DayReportOutPut();

                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                string sqlWhere = "";
                if (inPut.CenterId.Count == 1 && !inPut.CenterId.Contains("0"))
                {
                    sqlWhere = $"  and   bm.CenterId = '{inPut.CenterId[0]}'  ";
                }

                string YBSQLQuery = $@"select CenterId,CBLB,sum(XJZF)as XJZF,SUM(TCZF) as TCZF ,SUM(YBZLZFS) as  YBZLZFS,SUM(MZJZ) AS MZJZ,SUM(GWYBZ) AS GWYBZ,SUM(DELP) AS DELP,Sum(CBKK)  as CBKK ,
   SUM(ZJE)ZJE , SUM(oth_pay) AS oth_pay,SUM(hifes_pay) AS hifes_pay,SUM( ZHZF) ZHZF ,   medins_setl_id from (
   select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,1 as medins_setl_id from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate <'{inPut.EndTime}' and bm.BalanceState =1 and bm.BalanceType=1   {sqlWhere} and mx.medins_setl_id is null  group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id
     UNION ALL
	  select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,0 as medins_setl_id from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate <'{inPut.EndTime}' and bm.BalanceState =1 and bm.BalanceType=1 {sqlWhere}  and mx.medins_setl_id is not null  group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id

 UNION ALL
    select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,0 as medins_setl_id from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate <'{inPut.EndTime}' and bm.BalanceState =0 and bm.BalanceType=1 and    CancelDate  >'{inPut.EndTime}'  and mx.medins_setl_id is not null {sqlWhere} group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id

     UNION ALL
   
	 select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,1 as medins_setl_id from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate <'{inPut.EndTime}' and bm.BalanceState =0 and bm.BalanceType=1 and   CancelDate  >'{inPut.EndTime}' and mx.medins_setl_id is   null  {sqlWhere}  group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id


     UNION ALL
   
	 select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,1 as medins_setl_id from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SIBalanceSerialNo  and mx.CenterId = bm.CenterId
   where    bm. BalanceDate <'{inPut.BeginTime}'  and bm.BalanceState =0 and bm.BalanceType=1 and   CancelDate  >'{inPut.BeginTime}' and  CancelDate  <'{inPut.EndTime}' and mx.medins_setl_id is   null   {sqlWhere}    group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id


    UNION ALL
   
	 select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,0 as medins_setl_id from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SIBalanceSerialNo  and mx.CenterId = bm.CenterId
   where    bm. BalanceDate <'{inPut.BeginTime}'  and bm.BalanceState =0 and bm.BalanceType=1 and   CancelDate  >'{inPut.BeginTime}' and  CancelDate  <'{inPut.EndTime}' and mx.medins_setl_id is  not   null   {sqlWhere}   group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id

   ) b  group by CBLB,CenterId,medins_setl_id";

                string XJSQLQuery = $@" select SUM(bm.SumPrice) as XJZF , bm.CenterId from BalanceMain as bm where  (  bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate <'{inPut.EndTime}' and bm.BalanceState =1 and bm.BalanceType=2 {sqlWhere})   or    (bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate <'{inPut.EndTime}' and bm.BalanceState=0 and bm.BalanceType=2  and CancelDate  >'{inPut.EndTime}'     {sqlWhere} )  group by CenterId";

                string KYTFSQLQuery = $@"  select( 0-  sum( SumPrice) )as  XJZF ,CenterId  from BalanceMain as bm where    bm.BalanceState=0 and bm.BalanceDate <'{inPut.BeginTime}'and  bm.CancelDate >'{inPut.BeginTime}' and  bm.CancelDate <'{inPut.EndTime}' and bm.BalanceType=2  {sqlWhere}   group by CenterId";


                var sqlParameterList = new List<SqlParameter>();
                //医保结算 
                var ybdt = MsSqlHelper.GetSingleObj().GetDataTable(YBSQLQuery, sqlParameterList.ToArray());
                var ybData = GetTupleByList<SI_JSMX>(ybdt).Item2;
                //现金结算
                var xjdt = MsSqlHelper.GetSingleObj().GetDataTable(XJSQLQuery, sqlParameterList.ToArray());
                var xjData = GetTupleByList<SI_JSMX>(xjdt).Item2;
                //跨月退费 
                var tfdt = MsSqlHelper.GetSingleObj().GetDataTable(KYTFSQLQuery, sqlParameterList.ToArray());
                var tfData = GetTupleByList<SI_JSMX>(tfdt).Item2;


                if (inPut.CenterId != null && (inPut.CenterId.Count > 1 || inPut.CenterId.Contains("0")))
                {
                    ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId.Contains("0"))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    #region Table(表头)
                    List<TableHeader> tableHeaders = new List<TableHeader>();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "序号",
                        Key = "no",
                    });
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        width = 135,
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });
                    }
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Key = "total",
                    });
                    #endregion


                    List<CenterDayIncomeOutPut> centerDayIncomeOutPuts = new List<CenterDayIncomeOutPut>();
                    foreach (var item in ItemGroup)
                    {
                        CenterDayIncomeOutPut put = new CenterDayIncomeOutPut();
                        put.itemName = item;
                        foreach (var items in CenterDialysiss)
                        {

                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? Prices = 0;
                            switch (item)
                            {
                                case "现金":

                                    Prices = ybData.Where(t => t.CenterId == items.Id).Sum(t => t.XJZF) + xjData.Where(t => t.CenterId == items.Id).Sum(t => t.XJZF) + tfData.Where(t => t.CenterId == items.Id).Sum(t => t.XJZF);
                                    break;

                                case "公务员补助":
                                    Prices = ybData.Where(t => t.CenterId == items.Id).Sum(t => t.GWYBZ);
                                    break;
                                case "个人账户(职工)":
                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => t.ZHZF);//个人账户 ;
                                    break;
                                case "民政救助(职工)":
                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => t.MZJZ);//民政救助
                                    break;
                                case "统筹基金(职工)":
                                    if (items.Id == "f17a9c72fa8e47b78988757c6913b22b")//铜梁
                                        Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => (t.TCZF));
                                    else
                                        Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310" && t.medins_setl_id == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS)) + ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310" && t.medins_setl_id == "0").Sum(t => (t.TCZF));
                                    break;
                                case "大额医疗(职工)":

                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => (t.DELP));//医保基金
                                    break;
                                case "企业补充医疗保险基金(职工)":

                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => (t.hifes_pay));//医保基金
                                    break;
                                case "其他支出(职工)":

                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => (t.oth_pay));//医保基金
                                    break;

                                case "个人账户(居民)":
                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => t.ZHZF);//个人账户;
                                    break;
                                case "民政救助(居民)":
                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => t.MZJZ);//民政救助  
                                    break;
                                case "统筹基金(居民)":
                                    if (items.Id == "f17a9c72fa8e47b78988757c6913b22b")//铜梁
                                        Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => (t.TCZF));
                                    else
                                        Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390" && t.medins_setl_id == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS)) + ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390" && t.medins_setl_id == "0").Sum(t => (t.TCZF));
                                    break;
                                case "大病保险(居民)":
                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => (t.DELP));//医保基金
                                    break;
                                case "企业补充医疗保险基金(居民)":

                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => (t.hifes_pay));//医保基金
                                    break;
                                case "其他支出(居民)":

                                    Prices = ybData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => (t.oth_pay));//医保基金
                                    break;
                                case "超标扣款":

                                    Prices = ybData.Where(t => t.CenterId == items.Id).Sum(t => t.CBKK);
                                    break;

                                case "开业优惠":
                                    Prices = 0; //DifferenceMoneyData.Where(t => t.CenterId == items.Id && t.PayType == 8).Sum(t => t.CostMoney);//开业优惠
                                    break;
                                //case "合计":
                                //    Prices = PaySum; 
                                //    break; // 

                                default:
                                    break;
                            }
                            put.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(put, Prices);
                        }
                        put.total = put.kmtxzx + put.dzsmzb + put.xstxzx + put.jlptxzx + put.tltxzx + put.spbtxzx + put.fltxzx + put.zxtxzx + put.tjmzb;

                        centerDayIncomeOutPuts.Add(put);
                    }


                    //


                    centerDayIncomeOutPuts.Add(new CenterDayIncomeOutPut()
                    {
                        itemName = "合计",
                        xstxzx = centerDayIncomeOutPuts.Sum(t => t.xstxzx),
                        kmtxzx = centerDayIncomeOutPuts.Sum(t => t.kmtxzx),
                        dzsmzb = centerDayIncomeOutPuts.Sum(t => t.dzsmzb),
                        jlptxzx = centerDayIncomeOutPuts.Sum(t => t.jlptxzx),
                        tltxzx = centerDayIncomeOutPuts.Sum(t => t.tltxzx),
                        spbtxzx = centerDayIncomeOutPuts.Sum(t => t.spbtxzx),
                        fltxzx = centerDayIncomeOutPuts.Sum(t => t.fltxzx),
                        zxtxzx = centerDayIncomeOutPuts.Sum(t => t.zxtxzx),
                        tjmzb = centerDayIncomeOutPuts.Sum(t => t.tjmzb),
                        total = centerDayIncomeOutPuts.Sum(t => t.total),

                    });
                    centerDayIncomeOutPuts.ForEach(t =>
                    {
                        t.no = NoIndex; t.kmtxzx = t.kmtxzx.Value.ToRounds(); t.xstxzx = t.xstxzx.Value.ToRounds(); t.dzsmzb = t.dzsmzb.Value.ToRounds(); t.jlptxzx = t.jlptxzx.Value.ToRounds(); t.total = t.total.Value.ToRounds(); NoIndex++; t.tltxzx = t.tltxzx.Value.ToRounds(); t.spbtxzx = t.spbtxzx.Value.ToRounds(); t.zxtxzx = t.zxtxzx.Value.ToRounds(); t.tjmzb = t.tjmzb.Value.ToRounds();
                        t.fltxzx = t.fltxzx.Value.ToRounds();
                    });
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.CenterDayIncomeOutPuts = centerDayIncomeOutPuts;
                    return (object)aLLDayIncomeOutPut;
                }
                else
                {

                    foreach (var item in ItemGroup)
                    {
                        DayIncomeOutPut put = new DayIncomeOutPut();
                        put.ItemName = item;
                        switch (item)
                        {
                            case "现金":
                                //+ (SiData.Where(t=>t.TFBZ  == "1").Sum(t => t.XJZF))
                                put.TotalMoney = ybData.Sum(t => t.XJZF) + xjData.Sum(t => t.XJZF) + tfData.Sum(t => t.XJZF);
                                break;
                            case "公务员补助":
                                put.TotalMoney = ybData.Sum(t => t.GWYBZ);
                                break;
                            case "个人账户(职工)":
                                put.TotalMoney = ybData.Where(t => t.CBLB == "310").Sum(t => t.ZHZF);//个人账户 ;
                                break;
                            case "民政救助(职工)":
                                put.TotalMoney = ybData.Where(t => t.CBLB == "310").Sum(t => t.MZJZ);//民政救助
                                break;
                            case "统筹基金(职工)":
                                if (inPut.CenterId[0] == "f17a9c72fa8e47b78988757c6913b22b")//铜梁
                                    put.TotalMoney = ybData.Where(t => t.CBLB == "310").Sum(t => (t.TCZF));
                                else
                                    put.TotalMoney = ybData.Where(t => t.CBLB == "310" && t.medins_setl_id == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS)) + ybData.Where(t => t.CBLB == "310" && t.medins_setl_id == "0").Sum(t => (t.TCZF));
                                break;
                            case "大额医疗(职工)":

                                put.TotalMoney = ybData.Where(t => t.CBLB == "310").Sum(t => (t.DELP));//医保基金
                                break;
                            case "企业补充医疗保险基金(职工)":

                                put.TotalMoney = ybData.Where(t => t.CBLB == "310").Sum(t => (t.hifes_pay));//医保基金
                                break;
                            case "其他支出(职工)":

                                put.TotalMoney = ybData.Where(t => t.CBLB == "310").Sum(t => (t.oth_pay));//医保基金
                                break;

                            case "个人账户(居民)":
                                put.TotalMoney = ybData.Where(t => t.CBLB == "390").Sum(t => t.ZHZF);//个人账户;
                                break;
                            case "民政救助(居民)":
                                put.TotalMoney = ybData.Where(t => t.CBLB == "390").Sum(t => t.MZJZ);//民政救助  
                                break;
                            case "统筹基金(居民)":
                                if (inPut.CenterId[0] == "f17a9c72fa8e47b78988757c6913b22b")//铜梁
                                    put.TotalMoney = ybData.Where(t => t.CBLB == "390").Sum(t => (t.TCZF));
                                else
                                    put.TotalMoney = ybData.Where(t => t.CBLB == "390" && t.medins_setl_id == "1").Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS)) + ybData.Where(t => t.CBLB == "390" && t.medins_setl_id == "0").Sum(t => (t.TCZF));
                                break;
                            case "大病保险(居民)":
                                put.TotalMoney = ybData.Where(t => t.CBLB == "390").Sum(t => (t.DELP));//医保基金
                                break;
                            case "企业补充医疗保险基金(居民)":

                                put.TotalMoney = ybData.Where(t => t.CBLB == "390").Sum(t => (t.hifes_pay));//医保基金
                                break;
                            case "其他支出(居民)":

                                put.TotalMoney = ybData.Where(t => t.CBLB == "390").Sum(t => (t.oth_pay));//医保基金
                                break;
                            case "超标扣款":

                                put.TotalMoney = ybData.Sum(t => t.CBKK);
                                break;

                            case "开业优惠":
                                put.TotalMoney = 0; //DifferenceMoneyData.Where(t => t.CenterId == items.Id && t.PayType == 8).Sum(t => t.CostMoney);//开业优惠
                                break;
                            case "合计":
                                put.TotalMoney = 0;// PaySum;
                                break;

                            default:
                                break;
                        }

                        dayIncomes.Add(put);

                    }
                    dayIncomes.Add(new DayIncomeOutPut()
                    {
                        ItemName = "合计",
                        TotalMoney = dayIncomes.Sum(t => t.TotalMoney),
                    });


                    dayIncomes.ForEach(t =>
                    {
                        t.no = NoIndex; t.TotalMoney = t.TotalMoney.Value.ToRounds(); NoIndex++;
                    });
                    return (object)dayIncomes.ToArray();
                }
            });
        }


        /// <summary>
        /// 收费清单 charge
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<PageData<RefundOutPut[]>> GetNewChargeAsync(RefundQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                List<RefundOutPut> refundOutPuts = new List<RefundOutPut>();
                int count = 0;
                try
                {
                    var center = CenterDialysisStore.Entities.Where(t => t.Id == inPut.CenterId[0]).FirstOrDefault();

                    if (center == null)
                    {
                        throw new Exception("请选择指定的透析中心");
                    }


                    string QuerySQL = $@"   select bm.Id,    p.Name as pName,p.Sex, (datepart(YYYY,bm.BalanceDate)-datepart(YYYY,p.Birthday)) as Age,bm.BalanceDate as BalanceDate,bm.CancelDate,
   (CASE mx.CBLB when '310' then '职工' when '390' then '居民' else '自费' end) as HealthCareType ,mx.medins_setl_id,
   (CASE mx.YLLB when '11' then '普通门诊' when '12' then '门诊挂号' when '14' then '门诊慢特病' when '9901' then '重大疾病门诊' else '普通门诊' end) as MZType  ,bm.BalanceNo as PrescriptionNo,  CASE  
   
	    WHEN ((bm.BalanceState  !=1 and bm.CancelDate <'{inPut.EndTime}') ) THEN
  0- bm.SumPrice
  else 
   bm.SumPrice
   end as SumPrice,mx.TCZF as DETC,mx.XJZF as XJ,mx.GWYBZ,mx.DELP as YBJJ,mx.ZHZF as ZHZF,mx.hifes_pay,mx.oth_pay, mx.DBZDDYLJGDZ as YYCB,mx.MZJZ as MZJZ,bm.BalanceType
    from BalanceMain as bm
   left join Patient p on bm.PatientNo =p.PatientNo  
   left join  SI_JSMX mx on mx.JSJYLSH = bm.SISourceBalanceNo  
   where (  bm.BalanceDate>'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'  and  bm.BalanceState =1 ) or (    bm.BalanceDate<'{inPut.BeginTime}' and bm.CancelDate>'{inPut.BeginTime}' and   bm.CancelDate< '{inPut.EndTime}') or (      bm.BalanceDate>'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'  and bm.CancelDate>'{inPut.EndTime}' and  bm.BalanceState !=1  )

   order by bm.BalanceDate desc offset {(inPut.PageNum - 1) * inPut.PageSize} rows  fetch next {inPut.PageSize} rows only;";

                    string QueryCountSQL = $@"select count(*) as Count from  BalanceMain as bm where (    bm.BalanceDate>'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}' and  bm.BalanceState =1) or (     bm.BalanceDate<'{inPut.BeginTime}' and bm.CancelDate>'{inPut.BeginTime}'  and   bm.CancelDate< '{inPut.EndTime}')or (      bm.BalanceDate>'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'  and bm.CancelDate>'{inPut.EndTime}' and  bm.BalanceState !=1  ) ";

                    var sqlParameterList = new List<SqlParameter>();
                    //医保结算
                    var ybdt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, center.CenterConn, sqlParameterList.ToArray());
                    refundOutPuts = GetTupleByList<RefundOutPut>(ybdt).Item2;


                    //医保结算
                    var dtcount = MsSqlHelper.GetSingleObj().GetDataTable(QueryCountSQL, center.CenterConn, sqlParameterList.ToArray());
                    count = GetTupleByList<PageCount>(dtcount).Item2.First().count;

                    int No = (inPut.PageNum - 1) * inPut.PageSize + 1;
                    refundOutPuts.ForEach(t =>
                    {
                        t.Sex = t.Sex == "732cd8f9d16343b4940808c3a6f0ee2f" ? "男" : "女";
                        t.XJ = t.BalanceType != 1 ? t.XJ = t.SumPrice : t.XJ;
                        t.YBJJ = (string.IsNullOrWhiteSpace(t.medins_setl_id) && inPut.CenterId[0] != "f17a9c72fa8e47b78988757c6913b22b") ? t.YBJJ = t.YBJJ - t.MZJZ : t.YBJJ;
                        t.CancelDate = Convert.ToDateTime(t.CancelDate) > DateTime.Now.AddYears(-20) ? Convert.ToDateTime(t.CancelDate).ToString("yyyy-MM-dd") : "";
                        t.No = No++;
                        t.WX = t.SumPrice - (t.XJ + t.DETC + t.YBJJ + t.GWYBZ + t.MZJZ + t.oth_pay + t.hifes_pay + t.ZHZF);
                        t.BalanceDate = Convert.ToDateTime(t.BalanceDate).ToString("yyyy-MM-dd");
                    });

                    //if (LastSize)
                    //{
                    //    RefundOutPut refundOutPut = new RefundOutPut();
                    //    refundOutPut.No = count;
                    //    refundOutPut.PName = "合计";
                    //    refundOutPut.MZJZ = JsData.Sum(t => t.MZJZ);
                    //    refundOutPut.DETC = JsData.Sum(t => (t.TCZF - t.MZJZ - t.YBZLZFS));
                    //    refundOutPut.SumPrice = sum.Value;//refundOutPuts.Sum(t => t.SumPrice),
                    //    refundOutPut.WX = data.Where(t => t.BalanceType == 1).Sum(t => t.SumPrice) - JsData.Sum(t => t.ZJE);

                    //    refundOutPut.YBJJ = JsData.Sum(t => (t.GWYBZ + t.DBZDDYLJGDZ + t.DELP));//医保基金,
                    //    refundOutPut.YYCB = JsData.Sum(t => t.CBKK);
                    //    refundOutPut.ZHZF = JsData.Sum(t => t.ZHZF);
                    //    refundOutPut.XJ = data.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice) + JsData.Sum(t => t.XJZF);

                    //    refundOutPuts.Add(refundOutPut);
                    //}
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }

                return new PageData<RefundOutPut[]>(refundOutPuts.ToArray(), count);
            });
        }
        /// <summary>
        /// 收费明细
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<PageData<ChargeDetailOutPut[]>> GetNewChargeDetailAsync(RefundDetailInPut inPut)
        {
            return Task.Run(async () =>
            {
                int count = 1;
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                if (inPut.PageSize == 99999) inPut.PageSize = 999999;
                List<ChargeDetailOutPut> refundOutPuts = new List<ChargeDetailOutPut>();
                try
                {

                    var center = CenterDialysisStore.Entities.Where(t => t.Id == inPut.CenterId[0]).FirstOrDefault();

                    if (center == null)
                    {
                        throw new Exception("请选择指定的透析中心");
                    }

                    string QuerySQL = $@"select ROW_NUMBER() OVER (ORDER BY bm.BalanceDate )as Number, bd.Id,   p.Name as PName,p.PatientNo as MZH, RecipelNo as PrescriptionNo,bd.BalanceNo,bm.BalanceType,bm.BalanceDate as OpenDate,bm.SISourceBalanceNo as SIBalanceSerialNo,bd.ItemName,bd.Specifications,bd.CategoryName,bd.UnitPrice,bd.Qty,bd.TotalPrice ,bd.CompareCode,
   mr.HiLevel as Level, 
(CASE mx.CBLB when '310' then '职工' when '390' then '居民' else '自费' end) as HealthCareType,
    (select top 1   selfpay_prop from SI_OwnExpenseInfo where hilist_code = mr.NationItemCode and enddate is null) as payProportion,bm.CreatePerson as operationMan,
	(CASE bm.BalanceType when 1 then bd.Qty  else 0 end)  as ISUpQty,bd.OpenDateTime
   from BalanceDetails as bd
   left join BalanceMain as  bm on  bm.BalanceNo = bd.BalanceNo 
   left join  MedicalItemRecords as  mr on mr.Id =  bd.ItemId   
   left join Patient as p on p.PatientNo = bm.PatientNo 
left join  SI_JSMX mx on mx.JSJYLSH = bm.SISourceBalanceNo  
    where (bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'   and bm.BalanceState =1)  or (   bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}' and bm.CancelDate>'{inPut.EndTime}'  ) order by bm.BalanceDate desc offset {(inPut.PageNum - 1) * inPut.PageSize} rows  fetch next {inPut.PageSize} rows only;";



                    string QueryCountSQL = $@"select COUNT(*) as Count from BalanceDetails as bd
   left join BalanceMain as  bm on  bm.BalanceNo = bd.BalanceNo  
    where (bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'  and bm.BalanceState =1) or (   bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}' and bm.CancelDate>'{inPut.EndTime}'  ) ";

                    var sqlParameterList = new List<SqlParameter>();
                    //医保结算
                    var ybdt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, center.CenterConn, sqlParameterList.ToArray());
                    refundOutPuts = GetTupleByList<ChargeDetailOutPut>(ybdt).Item2;


                    //医保结算
                    var dtcount = MsSqlHelper.GetSingleObj().GetDataTable(QueryCountSQL, center.CenterConn, sqlParameterList.ToArray());
                    count = GetTupleByList<PageCount>(dtcount).Item2.First().count;

                    int No = (inPut.PageNum - 1) * inPut.PageSize + 1;
                    refundOutPuts.ForEach(t =>
                    {
                        t.TotalPrice = Convert.ToDateTime(t.OpenDate) < inPut.BeginTime ? 0 - t.TotalPrice : t.TotalPrice;
                        t.Qty = Convert.ToDateTime(t.OpenDate) < inPut.BeginTime ? 0 - t.Qty : t.Qty;
                        t.ISUpQty = Convert.ToDateTime(t.OpenDate) < inPut.BeginTime ? 0 - t.ISUpQty : t.ISUpQty;
                        t.OpenDate = Convert.ToDateTime(t.OpenDate).ToString("yyyy-MM-dd HH:mm:ss");
                        t.OpenDateTime = Convert.ToDateTime(t.OpenDateTime).ToString("yyyy-MM-dd HH:mm:ss");
                        t.Level = t.Level == "1" ? "甲" : t.Level == "2" ? "乙" : "丙";
                        t.No = No++;

                    });

                    //if (LastSize)
                    //{
                    //    refundOutPuts.Add(new ChargeDetailOutPut()
                    //    {
                    //        No = count,
                    //        PName = "合计",
                    //        TotalPrice = sum,
                    //        ISUpQty = SIUpdateData.Sum(t => t.SL),
                    //        Qty = BalanceDetailss.Sum(t => t.Qty),
                    //        ISPayAmount = SIUpdateData.Sum(t => (t.JE - t.ZFJE)),
                    //        PayAmount = data.Where(t => t.BalanceType == 2).Sum(t => t.SumPrice) + SIUpdateData.Sum(t => (t.ZFJE)),
                    //    });
                    //}
                }
                catch (Exception exp)
                {

                }
                return new PageData<ChargeDetailOutPut[]>(refundOutPuts.ToArray(), count);

            });
        }
        /// <summary>
        /// 物品收入占比
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetNewItemsSIRatioAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                string ItemType = "药品";
                switch (inPut.Model)
                {
                    case 1:

                        ItemType = "药品";
                        break;
                    case 2:
                        ItemType = "卫生耗材";
                        break;

                    case 3:
                        ItemType = "诊疗项目";
                        break;

                    default:
                        break;
                }


                List<SIRatioOutPut> ListSIRatioOutPut = new List<SIRatioOutPut>();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                ALLSIRatioOutPut aLLDayIncomeOutPut = new ALLSIRatioOutPut();
                try
                {
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId == null || (inPut.CenterId != null && inPut.CenterId.Contains("0")))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    #region 表头
                    tableHeaders.Add(new TableHeader()
                    {
                        title = inPut.EndTime.Month + "月",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Key = PropertiesName,
                        });
                    }

                    #endregion
                    #region 数据
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("1", "职工总收入（元）");
                    keyValuePairs.Add("2", ItemType + "收入（职工）");
                    keyValuePairs.Add("3", ItemType + "占比（职工）");
                    keyValuePairs.Add("4", "居民总收入（元）");
                    keyValuePairs.Add("5", ItemType + "收入（居民）");
                    keyValuePairs.Add("6", ItemType + "占比（居民）");
                    inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                    inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                    List<ALLSIRatioData> listData = new List<ALLSIRatioData>();
                    string sqlWhere = "";
                    string CategoryName = "";
                    switch (inPut.Model)
                    {
                        case 1:
                            CategoryName = "药品";
                            break;
                        case 2:
                            CategoryName = "卫生耗材";
                            break;
                        case 3:
                            CategoryName = "诊疗项目";
                            break;
                        default:
                            break;
                    }
                    if (inPut.CenterId.Count == 1 && !inPut.CenterId.Contains("0"))
                        sqlWhere += $@" and bd.CenterId = '{inPut.CenterId[0]}'";
                    string SqlQuery = $@" select    bd.CategoryName,sum(bd.TotalPrice ) as TotalPrice, sxm.CBLB,bd.CenterId
   from BalanceDetails as bd
   left join BalanceMain as  bm on  bm.BalanceNo = bd.BalanceNo and bd.CenterId = bm.CenterId
   left join SI_JSMX as sxm on sxm.JSJYLSH = bm.SISourceBalanceNo and sxm.CenterId = bd.CenterId
    where (bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'   and bm.BalanceState =1 {sqlWhere})  or(bm.BalanceState =0 and bm.BalanceDate>'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'and bm.CancelDate>'{inPut.EndTime}' {sqlWhere}  )  group by CBLB,bd.CenterId,CategoryName order by bd.CenterId";

                    var sqlParameterList = new List<SqlParameter>();
                    //医保结算
                    var ybdt = MsSqlHelper.GetSingleObj().GetDataTable(SqlQuery, sqlParameterList.ToArray());
                    listData = GetTupleByList<ALLSIRatioData>(ybdt).Item2;

                    decimal Prices = 0;

                    decimal total = 0;
                    string bili = "";

                    foreach (var item in keyValuePairs)
                    {
                        SIRatioOutPut dayIncomeOutPut = new SIRatioOutPut();
                        dayIncomeOutPut.itemName = item.Value;


                        foreach (var items in CenterDialysiss)
                        {

                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();



                            switch (item.Key)
                            {
                                //总收入 药品收入（职工） 占比  药品收入（居民） 占比
                                case "1":

                                    Prices = listData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => t.TotalPrice);
                                    break;
                                case "2"://药品收入（职工） 

                                    Prices = listData.Where(t => t.CenterId == items.Id && t.CBLB == "310" && t.CategoryName == CategoryName).Sum(t => t.TotalPrice);

                                    break;
                                case "3"://职工占比

                                    total = listData.Where(t => t.CenterId == items.Id && t.CBLB == "310").Sum(t => t.TotalPrice);



                                    Prices = Math.Round((listData.Where(t => t.CenterId == items.Id && t.CBLB == "310" && t.CategoryName == CategoryName).Sum(t => t.TotalPrice) / (total > 0 ? total : 1)), 2);
                                    break;
                                case "4"://居民
                                    Prices = listData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => t.TotalPrice);
                                    break;
                                case "5"://居民
                                    Prices = listData.Where(t => t.CenterId == items.Id && t.CBLB == "390" && t.CategoryName == CategoryName).Sum(t => t.TotalPrice);
                                    break;
                                case "6"://占比
                                    total = listData.Where(t => t.CenterId == items.Id && t.CBLB == "390").Sum(t => t.TotalPrice);


                                    Prices = Math.Round((listData.Where(t => t.CenterId == items.Id && t.CBLB == "390" && t.CategoryName == CategoryName).Sum(t => t.TotalPrice) / (total > 0 ? total : 1)), 2);
                                    break;

                                default:
                                    break;
                            }


                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, Prices);


                        }

                        ListSIRatioOutPut.Add(dayIncomeOutPut);
                    }

                    #endregion
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.SIRatioOutPuts = ListSIRatioOutPut;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return (object)aLLDayIncomeOutPut;
            });

        }


        /// <summary>
        /// 医保报销利率表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetNewSIRatioAsync(IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                List<SIRatioOutPut> ListSIRatioOutPut = new List<SIRatioOutPut>();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                ALLSIRatioOutPut aLLDayIncomeOutPut = new ALLSIRatioOutPut();
                inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);

                try
                {
                    List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                    if (inPut.CenterId == null || (inPut.CenterId != null && inPut.CenterId.Contains("0")))
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    else
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                    #region 表头
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "项目",
                        Key = "itemName",
                    });
                    foreach (var item in CenterDialysiss)
                    {
                        string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                        tableHeaders.Add(new TableHeader()
                        {
                            title = ReturnCenttSortName(item.ShortName),
                            Children = new List<TableHeader>() {
                                 new TableHeader(){    Key = PropertiesName, title= "金额"},
                                 new TableHeader(){    Key = PropertiesName+"ratios", title= "比率"},
                             }
                        });
                    }
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "合计",
                        Children = new List<TableHeader>() {
                                 new TableHeader(){    Key = "total", title= "金额"},
                                 new TableHeader(){    Key = "totalratios", title= "比率"},
                             }
                    });
                    #endregion
                    #region 数据
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs.Add("1", "职工医保");
                    keyValuePairs.Add("4", "职工自费");
                    keyValuePairs.Add("2", "居民医保");
                    keyValuePairs.Add("5", "居民自费");
                    keyValuePairs.Add("3", "现金支付");
                    inPut.BeginTime = new DateTime(inPut.BeginTime.Year, inPut.BeginTime.Month, inPut.BeginTime.Day, 0, 0, 0);
                    inPut.EndTime = new DateTime(inPut.EndTime.Year, inPut.EndTime.Month, inPut.EndTime.Day, 23, 59, 59);
                    string sqlWhere = "";
                    if (inPut.CenterId.Count == 1 && !inPut.CenterId.Contains("0"))
                        sqlWhere += $@" and bm.CenterId = '{inPut.CenterId[0]}'";

                    string QuerySQL = $@"select   SUM(SumPrice) as TotalPrice,SUM(XJZF+ZHZF) as XJZF, MX.CBLB ,bm.CenterId from BalanceMain as bm
	left join  SI_JSMX  as mx  on bm.SISourceBalanceNo = mx.JSJYLSH and mx.CenterId = bm.CenterId
	where (bm.BalanceDate >'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'   and bm.BalanceState =1  {sqlWhere} )  or (bm.BalanceState =0 and bm.BalanceDate>'{inPut.BeginTime}' and bm.BalanceDate<'{inPut.EndTime}'and bm.CancelDate>'{inPut.EndTime}'  {sqlWhere} )
	 group by bm.CenterId,MX.CBLB 
  UNION ALL
  select   SUM(-SumPrice) as TotalPrice,SUM(XJZF+ZHZF) as XJZF, MX.CBLB ,bm.CenterId from BalanceMain as bm
	left join  SI_JSMX  as mx  on bm.SISourceBalanceNo = mx.JSJYLSH and mx.CenterId = bm.CenterId
	where bm.BalanceDate <'{inPut.BeginTime}' and  bm.CancelDate >'{inPut.BeginTime}' and bm.CancelDate <'{inPut.EndTime}' and bm.BalanceState = 0  {sqlWhere}
	 group by bm.CenterId,MX.CBLB 
";

                    var sqlParameterList = new List<SqlParameter>();
                    //医保结算
                    var ybdt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());
                    var listData = GetTupleByList<ALLSIRatioData>(ybdt).Item2;

                    decimal? Prices = 0;
                    decimal? total = listData.Sum(t => t.TotalPrice);
                    foreach (var item in keyValuePairs)
                    {
                        SIRatioOutPut dayIncomeOutPut = new SIRatioOutPut();
                        dayIncomeOutPut.itemName = item.Value;

                        decimal? YBPrice = 0;
                        foreach (var items in CenterDialysiss)
                        {
                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            switch (item.Key)
                            {

                                case "1"://职工医保
                                    Prices = listData.Where(t => t.CBLB == "310" && t.CenterId == items.Id).Sum(t => t.TotalPrice - t.XJZF);
                                    break;
                                case "2"://居民医保
                                    Prices = listData.Where(t => t.CBLB == "390" && t.CenterId == items.Id).Sum(t => t.TotalPrice - t.XJZF);
                                    break;
                                case "4"://职工自费
                                    Prices = listData.Where(t => t.CBLB == "310" && t.CenterId == items.Id).Sum(t => t.XJZF);
                                    break;
                                case "5"://居民自费
                                    Prices = listData.Where(t => t.CBLB == "390" && t.CenterId == items.Id).Sum(t => t.XJZF);
                                    break;

                                case "3"://自费    SelfPaySum += SiData.Sum(t => t.XJZF);//现金支付 
                                    Prices = listData.Where(t => t.CenterId == items.Id && string.IsNullOrWhiteSpace(t.CBLB)).Sum(t => t.TotalPrice);
                                    break;
                                default:
                                    break;
                            }
                            YBPrice += Prices;
                            //  decimal? numbers = DetailsDatas.Where(t => t.MedicalItemRecord.FeeType.Name.Contains(item.Value) && t.CenterId == items.Id).Sum(t => t.TotalPrice);
                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(dayIncomeOutPut, Prices);
                            decimal bili = listData.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice) == 0 ? 0 : Math.Round((Prices / listData.Where(t => t.CenterId == items.Id).Sum(t => t.TotalPrice)).Value, 4) * 100;
                            dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName + "ratios").First().SetValue(dayIncomeOutPut, Math.Round(bili, 2) + "%");
                            //total += Convert.ToDecimal(dayIncomeOutPut.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().GetValue(dayIncomeOutPut));
                        }
                        dayIncomeOutPut.total = YBPrice;
                        dayIncomeOutPut.totalratios = Math.Round(Math.Round((YBPrice / total).Value, 4) * 100, 2) + "%";
                        ListSIRatioOutPut.Add(dayIncomeOutPut);
                    }
                    SIRatioOutPut sumdayIncomeOutPut = new SIRatioOutPut();
                    sumdayIncomeOutPut.itemName = "合计";
                    sumdayIncomeOutPut.kmtxzx = ListSIRatioOutPut.Sum(t => t.kmtxzx);

                    sumdayIncomeOutPut.xstxzx = ListSIRatioOutPut.Sum(t => t.xstxzx);
                    sumdayIncomeOutPut.dzsmzb = ListSIRatioOutPut.Sum(t => t.dzsmzb);
                    sumdayIncomeOutPut.jlptxzx = ListSIRatioOutPut.Sum(t => t.jlptxzx);
                    sumdayIncomeOutPut.tltxzx = ListSIRatioOutPut.Sum(t => t.tltxzx);
                    sumdayIncomeOutPut.spbtxzx = ListSIRatioOutPut.Sum(t => t.spbtxzx);
                    sumdayIncomeOutPut.fltxzx = ListSIRatioOutPut.Sum(t => t.fltxzx);

                    sumdayIncomeOutPut.zxtxzx = ListSIRatioOutPut.Sum(t => t.zxtxzx);

                    sumdayIncomeOutPut.tjmzb = ListSIRatioOutPut.Sum(t => t.tjmzb);
                    sumdayIncomeOutPut.total = sumdayIncomeOutPut.kmtxzx + sumdayIncomeOutPut.xstxzx + sumdayIncomeOutPut.dzsmzb + sumdayIncomeOutPut.jlptxzx + sumdayIncomeOutPut.tltxzx + sumdayIncomeOutPut.spbtxzx + sumdayIncomeOutPut.fltxzx + sumdayIncomeOutPut.zxtxzx + sumdayIncomeOutPut.tjmzb;
                    ListSIRatioOutPut.Add(sumdayIncomeOutPut);
                    #endregion
                    aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                    aLLDayIncomeOutPut.SIRatioOutPuts = ListSIRatioOutPut;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
                return (object)aLLDayIncomeOutPut;
            });

        }


        /// <summary>
        /// 医占比
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>

        public Task<object> NewMedicalProportionAsync(CostSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                inPut.BeginTime = new DateTime(inPut.BeginTime.Value.Year, inPut.BeginTime.Value.Month, inPut.BeginTime.Value.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Value.Year, inPut.EndTime.Value.Month, inPut.EndTime.Value.Day, 23, 59, 59);

                ALLDayIncomeOutPut aLLDayIncomeOutPut = new ALLDayIncomeOutPut();
                int NoIndex = 1;

                //表头
                #region  表头
                List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                if (inPut.CenterId.Contains("0"))
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                else
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                tableHeaders.Add(new TableHeader()
                {
                    title = "序号",
                    Key = "no",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "医保类别",
                    Key = "itemMainName",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "项目",
                    Key = "itemName",
                });
                foreach (var item in CenterDialysiss)
                {
                    string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(item.ShortName).ToLower();
                    tableHeaders.Add(new TableHeader()
                    {
                        title = ReturnCenttSortName(item.ShortName),
                        Key = PropertiesName,
                    });
                }
                aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                #endregion

                List<string> ItemGroup = new List<string>() { "总医疗费", "医保可报销的总医疗费", "药品费", "药占比（占总医疗费用）", "治疗费、材料费", "治疗材料比（占总医疗费用）", "患者自费额", "患者自费额占比（占总医疗费用）", "月就诊人数", "当月新增患者数", "当月人均费用" };
                //SI_CFMX 处方明细 BalanceMain 结算主表  SI_JSMX 医保结算主表
                //患者人
                var Patients = await PatientStore.Entities.Where(t => t.IsDelete == false).AsNoTracking().ToListAsync();


                Expression<Func<BalanceMain, bool>> manpredicate = (t => t.BalanceDate >= inPut.BeginTime && t.BalanceDate < inPut.EndTime);
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    manpredicate = manpredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                manpredicate = manpredicate.And(t => t.CancelDate == null || t.CancelDate > inPut.EndTime);
                //结算总表
                var BalanceData = await BalanceMainStore.Entities.Where(manpredicate).AsNoTracking().ToListAsync();



                //跨月退费数据
                var strMainId = BalanceData.Where(t => t.BalanceState == 0).Select(t => t.BalanceNo);
                //跨月退费数据
                var CancelData = BalanceData.Where(t => t.CancelFlag == 1).Select(t => t.SISourceBalanceNo).ToList();

                Expression<Func<SI_JSMX, bool>> Sipredicate = (t => t.JSRQ >= inPut.BeginTime && t.JSRQ < inPut.EndTime && t.TFBZ == "1");
                if (inPut.CenterId != null && inPut.CenterId.Count > 0 && !inPut.CenterId.Contains("0"))
                    Sipredicate = Sipredicate.And(t => inPut.CenterId.Contains(t.CenterId));

                Sipredicate = Sipredicate.Or(t => CancelData.Contains(t.JSJYLSH));
                /*
                       var querb = from q in Maiindata
                                join ac in SI_YPMLSStore.Entities on q.Medical.HiCenterCode equals ac.YPLSH into ud
                                from ud1 in ud.DefaultIfEmpty()
                                select new { q, ud1 };
                 
                 */
                //医保结算总表
                var SIData = await SI_JSMXStore.Entities.Where(Sipredicate).AsNoTracking().ToListAsync();

                var SILSH = SIData.Select(T => T.JSJYLSH);
                //医保处方明细
                var SICF = await SI_CFMXStore.Entities.Where(T => SILSH.Contains(T.JSJYLSH)).Join(MedicalItemRecordStore.Entities, a => a.YYNM, b => b.MedicalItemCode, (a, b) => new { CFMX = a, MedItem = b }).ToArrayAsync();

                var CFData = SICF.Select(a => { var data = a.CFMX; data.MedicalItem = a.MedItem; return data; }).ToList();
                List<CenterDayIncomeOutPut> centerDayIncomeOutPuts = new List<CenterDayIncomeOutPut>();
                int j = 1;
                for (int i = 1; i < 3; i++)
                {
                    string cblb = i == 1 ? "310" : "390";
                    var silsh = SIData.Where(t => t.CBLB == cblb).Select(T => T.JSJYLSH);
                    var mxData = CFData.Where(t => silsh.Contains(t.JSJYLSH)).ToList();

                    foreach (var item in ItemGroup)
                    {
                        CenterDayIncomeOutPut put = new CenterDayIncomeOutPut();
                        put.itemName = item;

                        foreach (var items in CenterDialysiss)
                        {
                            decimal? PaySum = SIData.Where(t => t.CenterId == items.Id && t.CBLB == cblb).Sum(t => t.ZJE);
                            string PropertiesName = CDGService.Data.Helper.Pinyin.GetInitials(items.ShortName).ToLower();
                            decimal? Prices = 0;
                            switch (item)
                            {
                                case "总医疗费":
                                    put.itemMainName = cblb == "310" ? "职工医保" : "居民医保";
                                    Prices = PaySum;
                                    break;

                                case "医保可报销的总医疗费":
                                    Prices = SIData.Where(t => t.CenterId == items.Id && t.CBLB == cblb).Sum(t => t.BCFHYBFWFY);
                                    break;
                                case "药品费":
                                    Prices = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType == 1).Sum(t => (t.JE - t.ZFJE));
                                    break;
                                case "药占比（占总医疗费用）":
                                    var Prices1 = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType == 1).Sum(t => (t.JE - t.ZFJE));
                                    Prices = (Prices1 / PaySum).Value.ToRounds(4);
                                    break;
                                case "治疗费、材料费":
                                    Prices = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType != 1).Sum(t => (t.JE - t.ZFJE));
                                    break;
                                case "治疗材料比（占总医疗费用）":
                                    var Prices2 = mxData.Where(t => t.CenterId == items.Id && t.MedicalItem.MedicalItemType != 1).Sum(t => (t.JE - t.ZFJE));
                                    Prices = (Prices2 / PaySum).Value.ToRounds(4);
                                    break;

                                case "患者自费额":
                                    Prices = mxData.Where(t => t.CenterId == items.Id).Sum(t => t.ZFJE);
                                    break;
                                case "患者自费额占比（占总医疗费用）":
                                    var Prices3 = mxData.Where(t => t.CenterId == items.Id).Sum(t => t.ZFJE);
                                    Prices = (Prices3 / PaySum).Value.ToRounds(4);
                                    break;
                                case "月就诊人数":
                                    Prices = SIData.Where(t => t.CBLB == cblb && t.CenterId == items.Id).GroupBy(t => t.SHBZH).Count();

                                    break;
                                case "当月新增患者数":
                                    string stype = cblb;
                                    Prices = Patients.Where(t => t.CenterId == items.Id && t.ReceiveDate < inPut.EndTime && t.ReceiveDate >= inPut.BeginTime && t.SIInsuredType == stype).Count();
                                    break;
                                //( BalanceData.Where(t => t.SISourceBalanceNo != null).Sum(t => t.SumPrice)) -( SiData.Where(t=>t.TFBZ =="1").Sum(t => t.ZJE)+ SiData.Where(t => CancelData.Contains(t.JSJYLSH)).Sum(t => t.ZJE));  king8888
                                case "当月人均费用":

                                    var Prices4 = SIData.Where(t => t.CBLB == cblb && t.CenterId == items.Id).GroupBy(t => t.SHBZH).Count();

                                    Prices = (PaySum / Prices4).Value.ToRounds(4);
                                    break;
                                default:
                                    break;
                            }
                            put.GetType().GetProperties().Where(t => t.Name == PropertiesName).First().SetValue(put, Prices);
                        }

                        put.no = j;
                        centerDayIncomeOutPuts.Add(put);
                        j++;
                    }
                }

                //总费用
                //  var TotolPrice = BalanceData.Sum(t => t.SumPrice);
                //医保可报销总费用 （）

                //职工
                //居民

                //患者人数
                //当月新增患者
                aLLDayIncomeOutPut.CenterDayIncomeOutPuts = centerDayIncomeOutPuts;
                return (object)aLLDayIncomeOutPut;

            });
        }

        /// <summary>
        /// 透析例次表 
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> ChargeCasesAsync(CostSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                inPut.BeginTime = new DateTime(inPut.BeginTime.Value.Year, inPut.BeginTime.Value.Month, inPut.BeginTime.Value.Day, 0, 0, 0);
                inPut.EndTime = new DateTime(inPut.EndTime.Value.Year, inPut.EndTime.Value.Month, inPut.EndTime.Value.Day, 23, 59, 59);

                ALLChargeCasesOutPut aLLDayIncomeOutPut = new ALLChargeCasesOutPut();
                int NoIndex = 1;

                //表头
                #region  表头
                List<CenterDialysis> CenterDialysiss = new List<CenterDialysis>();
                if (inPut.CenterId.Contains("0"))
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                else
                    CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && inPut.CenterId.Contains(t.Id)).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                List<TableHeader> tableHeaders = new List<TableHeader>();
                DataTable dt = new DataTable();
                dt.Columns.Add("CenterName", typeof(string));
                dt.Columns.Add("month", typeof(string));
                dt.Columns.Add("PatientName", typeof(string));
                dt.Columns.Add("PatientNo", typeof(string));
                dt.Columns.Add("PatientAge", typeof(string));
                dt.Columns.Add("SIInsuredType", typeof(string));
                dt.Columns.Add("SumPrice", typeof(decimal));
                tableHeaders.Add(new TableHeader()
                {
                    title = "机构",
                    Key = "centerName",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "月份",
                    Key = "month",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "患者姓名",
                    Key = "patientName",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "患者编号",
                    Key = "patientNo",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "年龄",
                    Key = "patientAge",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "参保类别",
                    Key = "siInsuredType",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "透析费用",
                    Key = "sumPrice",
                });


                #endregion

                #region  SQL
                string sqlWhere = "";
                if (inPut.CenterId.Count == 1 && !inPut.CenterId.Contains("0"))
                {
                    sqlWhere = $"  where   z.CenterId = '{inPut.CenterId[0]}' ";
                }
                #region   sql
                string QuerySQL = $@"
select z.患者姓名 as PatientName ,z.患者编号 as PatientNo ,z.月份 as DeteMonth,sum(z.费用) as SumPrice,z.透析次数 as HdCount,z.CenterId,cd.ShortName,(datepart(YYYY,'{inPut.EndTime}')-datepart(YYYY,pt.Birthday)) as  PatientAge,  z.透析模式 as ActualDialysisType ,  pt.SIInsuredType
 from (select aa.PatientName as 患者姓名,aa.PatientNo as 患者编号 ,0 as 透析次数,aa.Month as 月份,sum(aa.Price) as 费用  ,aa.CenterId, '' as  透析模式
 from (
SELECT  e.Name as PatientName,e.PatientNo, e.Birthday,CONVERT(varchar(7),b.BalanceDate,120) as Month,sum(b.SumPrice) as Price,'无退费收费' as ChargeState ,b.CenterId
  FROM dbo.BalanceMain b
  left join dbo.Patient e on e.PatientNo = b.PatientNo and e.CenterId = b.CenterId
  where b.BalanceState = 1 and BalanceDate > '{inPut.BeginTime}' and BalanceDate< '{inPut.EndTime}'
  group by e.Name,e.PatientNo,e.Birthday,CONVERT(varchar(7), b.BalanceDate, 120),b.CenterId
 
  union all
 
  SELECT e.Name as PatientName,e.PatientNo, e.Birthday,CONVERT(varchar(7), b.BalanceDate, 120) as Month,sum(b.SumPrice) as Price,'退费收费' as ChargeState,b.CenterId
  FROM dbo.BalanceMain b
  left join dbo.Patient e on e.PatientNo = b.PatientNo and e.CenterId = b.CenterId
  where b.BalanceState = 0 and BalanceDate > '{inPut.BeginTime}' and BalanceDate< '{inPut.EndTime}'
  group by e.Name,e.PatientNo,e.Birthday,CONVERT(varchar(7), b.BalanceDate, 120),b.CenterId

  union all

  SELECT e.Name as PatientName,e.PatientNo, e.Birthday,CONVERT(varchar(7), b.CancelDate, 120) as Month,sum(-b.SumPrice) as Price,'退费' as ChargeState,b.CenterId
  FROM dbo.BalanceMain b
  left join dbo.Patient e on e.PatientNo = b.PatientNo and e.CenterId = b.CenterId
  where b.BalanceState = 0 and CancelDate > '{inPut.BeginTime}' and CancelDate< '{inPut.EndTime}'
  group by  b.CenterId, e.Name,e.PatientNo,e.Birthday,CONVERT(varchar(7), b.CancelDate, 120)
  
  ) aa
    group by aa.PatientName,aa.PatientNo,aa.Month,aa.CenterId
    union all
    --1、先统计次数
   select aa.患者姓名,aa.患者编号,count(CONVERT(varchar(7), aa.日期, 120)) as 透析次数,CONVERT(varchar(7), aa.日期, 120) as 月份,0 as 费用 ,aa.CenterId ,aa.透析模式 from(
      SELECT a.Name as 患者姓名, a.PatientNo as 患者编号,
            case when b.Name = '转入' then '在院' else b.Name end as 在院状态,
            '透析次数' as 类型,
            d.Date as 日期,a.CenterId ,d.ActualDialysisType as 透析模式
       FROM dbo.Patient a
       left join dbo.SystemDictionarys b on a.HospitalState = b.Id  
       left join dbo.HistoryDialysisRecords c on c.PatientId = a.Id and c.CenterId = a.CenterId
       left join dbo.PatientCycleScheduling d on c.PatientCycleSchedulingId = d.Id and d.CenterId = c.CenterId
       where c.PreTreatId is not null  and d.SignDate > '{inPut.BeginTime}' and d.SignDate < '{inPut.EndTime}') aa
       group by aa.患者姓名,aa.患者编号,Convert(varchar(7), aa.日期, 120),aa.CenterId, aa.透析模式   
	   
	   ) z
  left join CenterDialysiss as cd on cd.Id = z.CenterId
  left join Patient as pt on pt.PatientNo = z.患者编号 and pt.CenterId = z.CenterId {sqlWhere}
         group by z.患者姓名,z.患者编号,z.月份,z.CenterId,cd.ShortName,pt.Birthday ,z.透析次数,z.透析模式 ,  pt.SIInsuredType order by z.CenterId";

                #endregion



                #endregion






                var sqlParameterList = new List<SqlParameter>();
                //医保结算   
                var Casesdt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());
                var CasesData = GetTupleByList<ChargeCasesModel>(Casesdt).Item2;
                var groupByPatientList = CasesData.GroupBy(w => new { w.CenterId, w.PatientName, w.PatientNo }).ToList();
                //ReturnCenttSortName(item.ShortName)

                var typeHd = CasesData.GroupBy(t => t.ActualDialysisType);
                List<TableHeader> ChildrentableHeaders = new List<TableHeader>();
                foreach (var item in typeHd)
                {

                    if (!string.IsNullOrWhiteSpace(item.Key))
                    {
                        ChildrentableHeaders.Add(new TableHeader()
                        {
                            Key = item.Key.Replace('+', '_').ToLower(),
                            title = item.Key,
                        });
                        dt.Columns.Add(item.Key.Replace('+', '_').ToLower(), typeof(string));
                    }
                }

                tableHeaders.Add(new TableHeader()
                {
                    title = "透析方式",

                    Children = ChildrentableHeaders

                });


                tableHeaders.Add(new TableHeader()
                {
                    title = "合计次数",
                    Key = "hdCount",
                });
                dt.Columns.Add("HdCount", typeof(string));
                aLLDayIncomeOutPut.tableHeaders = tableHeaders;

                if (inPut.MonthModel == 1)
                {
                    for (DateTime i = inPut.BeginTime.Value; i < inPut.EndTime;)
                    {
                        string deMonth = i.ToString("yyyy-MM");
                        foreach (var item in groupByPatientList)
                        {
                            DataRow dr = dt.NewRow();
                            dr["CenterName"] = ReturnCenttSortName(item.First().ShortName);

                            dr["month"] = deMonth;
                            dr["PatientName"] = item.First().PatientName;
                            dr["PatientNo"] = item.First().PatientNo;
                            dr["PatientAge"] = item.First().PatientAge;
                            dr["SIInsuredType"] = item.First().SIInsuredType == "310" ? "职工医保" : item.First().SIInsuredType == "390" ? "居民医保" : "自费";

                            var sprice = item.Where(t => t.DeteMonth == deMonth).Sum(t => t.SumPrice);
                            dr["SumPrice"] = sprice;
                            foreach (var itemty in typeHd)
                            {
                                if (!string.IsNullOrWhiteSpace(itemty.Key))
                                {
                                    var temp = item.Where(t => t.ActualDialysisType == itemty.Key && t.DeteMonth == deMonth).FirstOrDefault();
                                    dr[itemty.Key.Replace('+', '_').ToLower()] = temp == null ? 0 : temp.HdCount;
                                }

                            }
                            var hd = item.Where(t => t.DeteMonth == deMonth).Sum(t => t.HdCount);
                            dr["HdCount"] = hd;
                            dt.Rows.Add(dr);
                        }

                        i = i.AddMonths(1);
                    }
                }
                else if (inPut.MonthModel == 2)
                {
                    string deMonth = inPut.BeginTime.Value.ToString("yyyy-MM") + "-" + inPut.EndTime.Value.ToString("yyyy-MM");
                    foreach (var item in groupByPatientList)
                    {
                        DataRow dr = dt.NewRow();
                        dr["CenterName"] = ReturnCenttSortName(item.First().ShortName);
                        dr["month"] = deMonth;
                        dr["PatientName"] = item.First().PatientName;
                        dr["PatientNo"] = item.First().PatientNo;
                        dr["PatientAge"] = item.First().PatientAge;
                        dr["SIInsuredType"] = item.First().SIInsuredType == "310" ? "职工医保" : item.First().SIInsuredType == "390" ? "居民医保" : "自费";

                        var sprice = item.Sum(t => t.SumPrice);
                        dr["SumPrice"] = sprice;
                        foreach (var itemty in typeHd)
                        {
                            if (!string.IsNullOrWhiteSpace(itemty.Key))
                            {

                                var temp = item.Where(t => t.ActualDialysisType == itemty.Key).Sum(t => t.HdCount);
                                dr[itemty.Key.Replace('+', '_').ToLower()] = temp == null ? 0 : temp;
                            }

                        }
                        var hd = item.Sum(t => t.HdCount);

                        dr["HdCount"] = hd;
                        dt.Rows.Add(dr);
                    }

                }
                aLLDayIncomeOutPut.SiRatioOutPuts = dt;
                return (object)aLLDayIncomeOutPut;

            });
        }

        //
        /// <summary>
        /// 营业额及成本统计表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        public Task<object> GetIncomeAdnCostAsync(PatientsReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                inPut.BeginReportDate = new DateTime(inPut.BeginReportDate.Year, inPut.BeginReportDate.Month, inPut.BeginReportDate.Day, 0, 0, 0);
                inPut.EndReportDate = new DateTime(inPut.EndReportDate.Year, inPut.EndReportDate.Month, inPut.EndReportDate.Day, 23, 59, 59);
                string sqlWhere = "";

                var CenterList = await CenterDialysisStore.Entities.Where(t => t.DataState == 1 && t.IsDelete == false).OrderBy(t => t.SortNnm).AsNoTracking().ToListAsync();
                //表头
                #region   表头
                List<TableHeader> tableHeaders = new List<TableHeader>();
                if (inPut.staType == 1)
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "日期",
                        Fixed = "left",


                        Key = "balanceDate",
                    });
                else
                    tableHeaders.Add(new TableHeader()
                    {
                        title = "日期",
                        Fixed = "left",
                        width = 180,

                        Key = "balanceDate",
                    });
                tableHeaders.Add(new TableHeader()
                {
                    title = "公司",
                    Fixed = "left",
                    width = 125,
                    Key = "centerName",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "应存现金",
                    Key = "xj",
                });
                List<TableHeader> yyetableHeaders = new List<TableHeader>();

                yyetableHeaders.Add(new TableHeader()
                {
                    title = "透析费",
                    Key = "txf",
                });

                yyetableHeaders.Add(new TableHeader()
                {
                    title = "药费",
                    Key = "yf",
                });
                yyetableHeaders.Add(new TableHeader()
                {
                    title = "材料费",
                    Key = "clf",
                });
                yyetableHeaders.Add(new TableHeader()
                {
                    title = "其他",
                    Key = "yyeqt",
                });
                yyetableHeaders.Add(new TableHeader()
                {
                    title = "小计",
                    Key = "yyexj",
                });
                tableHeaders.Add(new TableHeader()
                {
                    title = "营业额",

                    Children = yyetableHeaders
                });
                List<TableHeader> yftableHeaders = new List<TableHeader>();
                yftableHeaders.Add(new TableHeader()
                {
                    title = "处方出库",
                    Key = "cfck",
                });
                yftableHeaders.Add(new TableHeader()
                {
                    title = "其他出库",
                    Key = "ypqtck",
                });
                List<TableHeader> wctableHeaders = new List<TableHeader>();
                wctableHeaders.Add(new TableHeader()
                {
                    title = "处方出库",
                    Key = "wcck",
                });
                wctableHeaders.Add(new TableHeader()
                {
                    title = "其他出库",
                    Key = "wcqtck",
                });

                List<TableHeader> cbtableHeaders = new List<TableHeader>();

                cbtableHeaders.Add(new TableHeader()
                {
                    title = "西药房",
                    Children = yftableHeaders
                });

                cbtableHeaders.Add(new TableHeader()
                {
                    title = "卫材库",
                    Children = wctableHeaders

                });


                tableHeaders.Add(new TableHeader()
                {
                    title = "当日出库成本金额",
                    Children = cbtableHeaders
                });

                List<TableHeader> mlltableHeaders = new List<TableHeader>();

                mlltableHeaders.Add(new TableHeader()
                {
                    title = "透析",
                    Key = "tx",
                });

                mlltableHeaders.Add(new TableHeader()
                {
                    title = "药品",
                    Key = "yp",
                });
                mlltableHeaders.Add(new TableHeader()
                {
                    title = "卫材",
                    Key = "cl",
                });

                tableHeaders.Add(new TableHeader()
                {
                    title = "当日成本控制情况（毛利率）",
                    Children = mlltableHeaders,
                });
                #endregion

                DataTable dt = new DataTable();

                dt.Columns.Add("balanceDate", typeof(string));
                dt.Columns.Add("centerName", typeof(string));
                dt.Columns.Add("xj", typeof(string));
                dt.Columns.Add("txf", typeof(string));
                dt.Columns.Add("yf", typeof(string));
                dt.Columns.Add("clf", typeof(string));
                dt.Columns.Add("yyeqt", typeof(string));
                dt.Columns.Add("yyexj", typeof(string));
                dt.Columns.Add("cfck", typeof(string));
                dt.Columns.Add("ypqtck", typeof(string));
                dt.Columns.Add("wcck", typeof(string));
                dt.Columns.Add("wcqtck", typeof(string));
                dt.Columns.Add("tx", typeof(string));
                dt.Columns.Add("yp", typeof(string));
                dt.Columns.Add("cl", typeof(string));

                #region  基础数据获取
                //收入


                string QuerySql = $@"
--按收费项目
--本月正常收费数据
select SUM(d.TotalPrice) as TotalPrice ,d.FreeTypeName,d.CenterId,DATEADD(dd, DATEDIFF(dd, 0, BalanceDate),0) as  BalanceDate from (
select  b.TotalPrice ,b.FreeTypeName ,b.CenterId, BalanceDate   from (
select bd.*,s.Name as FreeTypeName ,bm.BalanceDate from BalanceDetails as bd 
left join BalanceMain  as bm on bm.BalanceNo = bd.BalanceNo and  bm.CenterId =bd.CenterId
left join MedicalItemRecords m on m.Id = bd.ItemId
left join SystemDictionarys s on s.Id = m.FeeTypeId
where     bm.BalanceState =1 and  bm.BalanceDate >'{inPut.BeginReportDate}'and  bm.BalanceDate <'{inPut.EndReportDate}' {sqlWhere}
) b    
  UNION ALL -- 跨月退费数据
select  0-c.TotalPrice as TotalPrice,c.FreeTypeName, c.CenterId ,CancelDate as BalanceDate from (
select bd.*,s.Name as FreeTypeName,bm.CancelDate from BalanceDetails as bd 
left join BalanceMain  as bm on bm.BalanceNo = bd.BalanceNo and bm.CenterId =bd.CenterId
left join MedicalItemRecords m on m.Id = bd.ItemId
left join SystemDictionarys s on s.Id = m.FeeTypeId
where      bm.BalanceState =0 and  bm.BalanceDate <'{inPut.BeginReportDate}'and  bm.CancelDate >'{inPut.BeginReportDate}' and  bm.CancelDate <'{inPut.EndReportDate}' {sqlWhere}
) c 
UNION ALL -- 次月退费数据
select e.TotalPrice as TotalPrice,e.FreeTypeName, e.CenterId ,BalanceDate from (
select bd.*,s.Name as FreeTypeName ,bm.BalanceDate  from BalanceDetails as bd 
left join BalanceMain  as bm on bm.BalanceNo = bd.BalanceNo and bm.CenterId =bd.CenterId
left join MedicalItemRecords m on m.Id = bd.ItemId
left join SystemDictionarys s on s.Id = m.FeeTypeId
where      bm.BalanceState =0 and  bm.BalanceDate >'{inPut.BeginReportDate}'and  bm.BalanceDate <'{inPut.EndReportDate}' and  bm.CancelDate >'{inPut.EndReportDate}' {sqlWhere}
) as e) d group by d.FreeTypeName ,d.CenterId ,DATEADD(dd, DATEDIFF(dd, 0, BalanceDate),0) ";

                string YBSQLQuery = $@"select CenterId,CBLB,sum(XJZF)as XJZF,SUM(TCZF) as TCZF ,SUM(YBZLZFS) as  YBZLZFS,SUM(MZJZ) AS MZJZ,SUM(GWYBZ) AS GWYBZ,SUM(DELP) AS DELP,Sum(CBKK)  as CBKK ,
   SUM(ZJE)ZJE , SUM(oth_pay) AS oth_pay,SUM(hifes_pay) AS hifes_pay,SUM( ZHZF) ZHZF ,   medins_setl_id  , DATEADD(dd, DATEDIFF(dd, 0, BalanceDate),0) as  BalanceDate  from (
   select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,1 as medins_setl_id ,bm.BalanceDate  from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginReportDate}' and bm.BalanceDate <'{inPut.EndReportDate}' and bm.BalanceState =1 and bm.BalanceType=1   {sqlWhere} and mx.medins_setl_id is null  group by bm.BalanceDate, mx.CenterId, mx.CBLB,  mx.medins_setl_id
     UNION ALL
	  select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,0 as medins_setl_id ,bm.BalanceDate from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginReportDate}' and bm.BalanceDate <'{inPut.EndReportDate}' and bm.BalanceState =1 and bm.BalanceType=1 {sqlWhere}  and mx.medins_setl_id is not null  group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id ,bm.BalanceDate

 UNION ALL
    select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,0 as medins_setl_id , bm.BalanceDate from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginReportDate}' and bm.BalanceDate <'{inPut.EndReportDate}' and bm.BalanceState =0 and bm.BalanceType=1 and    CancelDate  >'{inPut.EndReportDate}'  and mx.medins_setl_id is not null {sqlWhere} group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id , bm.BalanceDate

     UNION ALL
   
	 select mx.CenterId,mx.CBLB,sum(mx.XJZF)as XJZF,SUM(mx.TCZF) as TCZF ,SUM(mx.YBZLZFS) as  YBZLZFS,SUM(mx.MZJZ) AS MZJZ,SUM(mx.GWYBZ) AS GWYBZ,SUM(mx.DELP) AS DELP,
   SUM(mx.DBZDDYLJGDZ)
    as CBKK ,
   SUM(mx.ZJE)ZJE , SUM(mx.oth_pay) AS oth_pay,SUM(mx.hifes_pay) AS hifes_pay,SUM( mx.ZHZF) ZHZF,1 as medins_setl_id ,bm.BalanceDate from SI_JSMX  as mx
   left join BalanceMain as bm on mx.JSJYLSH = bm.SISourceBalanceNo  and mx.CenterId = bm.CenterId
   where bm. BalanceDate >'{inPut.BeginReportDate}' and bm.BalanceDate <'{inPut.EndReportDate}' and bm.BalanceState =0 and bm.BalanceType=1 and   CancelDate  >'{inPut.EndReportDate}' and mx.medins_setl_id is   null  {sqlWhere}  group by  mx.CenterId, mx.CBLB,  mx.medins_setl_id,bm.BalanceDate



   ) b  group by CBLB,CenterId,medins_setl_id,DATEADD(dd, DATEDIFF(dd, 0, BalanceDate),0)";

                string XJSQLQuery = $@" select SUM(bm.SumPrice) as XJZF , bm.CenterId , DATEADD(dd, DATEDIFF(dd, 0, bm.BalanceDate),0) as BalanceDate from BalanceMain as bm where  (  bm.BalanceDate >'{inPut.BeginReportDate}' and bm.BalanceDate <'{inPut.EndReportDate}' and bm.BalanceState =1 and bm.BalanceType=2 {sqlWhere})   or    (bm.BalanceDate >'{inPut.BeginReportDate}' and bm.BalanceDate <'{inPut.EndReportDate}' and bm.BalanceState=0 and bm.BalanceType=2  and CancelDate  >'{inPut.EndReportDate}'     {sqlWhere} )  group by    CenterId,DATEADD(dd, DATEDIFF(dd, 0, BalanceDate),0)";

                string KYTFSQLQuery = $@"  select( 0-  sum( SumPrice) )as  XJZF ,bm.CenterId ,DATEADD(dd, DATEDIFF(dd, 0, bm.CancelDate),0) as BalanceDate  from BalanceMain as bm where    bm.BalanceState=0 and bm.BalanceDate <'{inPut.BeginReportDate}'and  bm.CancelDate >'{inPut.BeginReportDate}' and  bm.CancelDate <'{inPut.EndReportDate}' and bm.BalanceType =2  {sqlWhere}   group by bm.CenterId,DATEADD(dd, DATEDIFF(dd, 0, bm.CancelDate),0)     UNION ALL

		   select(  sum( mx.XJZF) )as  XJZF ,bm.CenterId  , DATEADD(dd, DATEDIFF(dd, 0, bm.CancelDate),0) as BalanceDate  from BalanceMain as bm  
		   left join SI_JSMX as mx on mx.JSJYLSH = bm.SIBalanceSerialNo  where    bm.BalanceState=0 and bm.BalanceDate <'{inPut.BeginReportDate}'and  bm.CancelDate >'{inPut.BeginReportDate}' and  bm.CancelDate <'{inPut.EndReportDate}' and bm.BalanceType =1  {sqlWhere}   group by bm.CenterId,DATEADD(dd, DATEDIFF(dd, 0, bm.CancelDate),0)";

                var sqlParameterList = new List<SqlParameter>();
                //医保结算 
                var ybdt = MsSqlHelper.GetSingleObj().GetDataTable(YBSQLQuery, sqlParameterList.ToArray());
                var ybData = GetTupleByList<SI_JSMX>(ybdt).Item2;
                //现金结算
                var xjdt = MsSqlHelper.GetSingleObj().GetDataTable(XJSQLQuery, sqlParameterList.ToArray());
                var xjData = GetTupleByList<SI_JSMX>(xjdt).Item2;
                //跨月退费 
                var tfdt = MsSqlHelper.GetSingleObj().GetDataTable(KYTFSQLQuery, sqlParameterList.ToArray());
                var tfData = GetTupleByList<SI_JSMX>(tfdt).Item2;

                var lbdt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<ChargeTable>(lbdt);
                var ChargeTableData = tuple.Item2;
                //成本
                var xmlSqlParameter1 = GetXmlSqlParameter("MaterialsStatisticaManager", "SqlQueryArticleDistributionByDate", "时间段查询物品进销存数据");
                string sql = GetQuerySql(xmlSqlParameter1);
                var Putindt = MsSqlHelper.GetSingleObj().GetDataTable(sql + $"and    putInstorageDate >'{inPut.BeginReportDate}' and  putInstorageDate <'{inPut.EndReportDate}' and MedicalItemType in (1,2)", sqlParameterList.ToArray());
                var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(Putindt);

                var data = putinTuple.Item2;
                #endregion

                //毛利率
                if (inPut.staType == 1)
                    for (DateTime i = inPut.BeginReportDate; i <= inPut.EndReportDate;)
                    {
                        foreach (var item in CenterList)
                        {
                            DataRow dr = dt.NewRow();

                            dr["balanceDate"] = i.ToString("yyyy-MM-dd");
                            dr["centerName"] = item.ShortName;
                            dr["xj"] = (ybData.Where(t => t.CenterId == item.Id && t.BalanceDate.Value == i).Sum(t => t.XJZF) + xjData.Where(t => t.CenterId == item.Id && t.BalanceDate.Value == i).Sum(t => t.XJZF) + tfData.Where(t => t.CenterId == item.Id && t.BalanceDate.Value == i).Sum(t => t.XJZF)).Value.ToFillRounds();
                            dr["txf"] = ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName == "透析治疗费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice).Value.ToFillRounds();
                            dr["yf"] = ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName == "药品费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice).Value.ToFillRounds();
                            dr["clf"] = ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName == "材料费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice).Value.ToFillRounds();
                            dr["yyeqt"] = (ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName != "药品费" && t.FreeTypeName != "材料费" && t.FreeTypeName != "透析治疗费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice)).Value.ToFillRounds();
                            dr["yyexj"] = (ChargeTableData.Where(t => t.CenterId == item.Id && t.BalanceDate.Value == i).Sum(t => t.TotalPrice)).Value.ToFillRounds();
                            dr["cfck"] = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" && t.MedicalItemType == 1 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1" && t.MedicalItemType == 1 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds();
                            dr["ypqtck"] = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" && t.MedicalItemType == 1 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" && t.MedicalItemType == 1 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() + (data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" && t.MedicalItemType == 1 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0))).ToFillRounds();
                            dr["wcck"] = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" && t.MedicalItemType == 2 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1" && t.MedicalItemType == 2 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds();
                            dr["wcqtck"] = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" && t.MedicalItemType == 2 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" && t.MedicalItemType == 2 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() + (data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" && t.MedicalItemType == 2 && t.CenterId == item.Id && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0))).ToFillRounds();
                            dr["tx"] = dr["txf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(dr["txf"]) - Convert.ToDecimal(dr["ypqtck"]) - Convert.ToDecimal(dr["wcqtck"])) / Convert.ToDecimal(dr["txf"])) * 100).ToFillRounds() + "%";
                            dr["yp"] = dr["yf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(dr["yf"]) - Convert.ToDecimal(dr["cfck"])) / Convert.ToDecimal(dr["yf"])) * 100).ToFillRounds() + "%";
                            dr["cl"] = dr["clf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(dr["clf"]) - Convert.ToDecimal(dr["wcck"])) / Convert.ToDecimal(dr["clf"])) * 100).ToFillRounds() + "%";
                            dt.Rows.Add(dr);


                        }

                        DataRow drs = dt.NewRow();
                        drs["balanceDate"] = "";
                        drs["centerName"] = "合计";
                        drs["xj"] = (ybData.Where(t => t.BalanceDate.Value == i).Sum(t => t.XJZF) + xjData.Where(t => t.BalanceDate.Value == i).Sum(t => t.XJZF) + tfData.Where(t => t.BalanceDate.Value == i).Sum(t => t.XJZF)).Value.ToFillRounds();
                        drs["txf"] = ChargeTableData.Where(t => t.FreeTypeName == "透析治疗费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice).Value.ToFillRounds();
                        drs["yf"] = ChargeTableData.Where(t => t.FreeTypeName == "药品费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice).Value.ToFillRounds();
                        drs["clf"] = ChargeTableData.Where(t => t.FreeTypeName == "材料费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice).Value.ToFillRounds();
                        drs["yyeqt"] = (ChargeTableData.Where(t => t.FreeTypeName != "药品费" && t.FreeTypeName != "材料费" && t.FreeTypeName != "透析治疗费" && t.BalanceDate.Value == i).Sum(t => t.TotalPrice)).Value.ToFillRounds();
                        drs["yyexj"] = (ChargeTableData.Where(t => t.BalanceDate.Value == i).Sum(t => t.TotalPrice)).Value.ToFillRounds();
                        drs["cfck"] = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" && t.MedicalItemType == 1 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1" && t.MedicalItemType == 1 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds();
                        drs["ypqtck"] = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" && t.MedicalItemType == 1 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" && t.MedicalItemType == 1 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() + (data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" && t.MedicalItemType == 1 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0))).ToFillRounds();
                        drs["wcck"] = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" && t.MedicalItemType == 2 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1" && t.MedicalItemType == 2 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds();
                        drs["wcqtck"] = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" && t.MedicalItemType == 2 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" && t.MedicalItemType == 2 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() + (data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" && t.MedicalItemType == 2 && t.putInstorageDate.Value.ToShortDateString() == i.ToShortDateString()).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0))).ToFillRounds();
                        drs["tx"] = drs["txf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(drs["txf"]) - Convert.ToDecimal(drs["ypqtck"]) - Convert.ToDecimal(drs["wcqtck"])) / Convert.ToDecimal(drs["txf"])) * 100).ToFillRounds() + "%";
                        drs["yp"] = drs["yf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(drs["yf"]) - Convert.ToDecimal(drs["cfck"])) / Convert.ToDecimal(drs["yf"])) * 100).ToFillRounds() + "%";
                        drs["cl"] = drs["clf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(drs["clf"]) - Convert.ToDecimal(drs["wcck"])) / Convert.ToDecimal(drs["clf"])) * 100).ToFillRounds() + "%";
                        dt.Rows.Add(drs);

                        i = i.AddDays(1);
                    }
                else
                {
                    foreach (var item in CenterList)
                    {

                        DataRow dr = dt.NewRow();
                        dr["balanceDate"] = inPut.BeginReportDate.ToString("yyy-MM-dd") + "至" + inPut.EndReportDate.ToString("yyy-MM-dd");

                        dr["centerName"] = item.ShortName;
                        dr["xj"] = (ybData.Where(t => t.CenterId == item.Id).Sum(t => t.XJZF) + xjData.Where(t => t.CenterId == item.Id).Sum(t => t.XJZF) + tfData.Where(t => t.CenterId == item.Id).Sum(t => t.XJZF)).Value.ToFillRounds();
                        dr["txf"] = ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName == "透析治疗费").Sum(t => t.TotalPrice).Value.ToFillRounds();
                        dr["yf"] = ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName == "药品费").Sum(t => t.TotalPrice).Value.ToFillRounds();
                        dr["clf"] = ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName == "材料费").Sum(t => t.TotalPrice).Value.ToFillRounds();
                        dr["yyeqt"] = (ChargeTableData.Where(t => t.CenterId == item.Id && t.FreeTypeName != "药品费" && t.FreeTypeName != "材料费" && t.FreeTypeName != "透析治疗费").Sum(t => t.TotalPrice)).Value.ToFillRounds();
                        dr["yyexj"] = (ChargeTableData.Where(t => t.CenterId == item.Id).Sum(t => t.TotalPrice)).Value.ToFillRounds();
                        dr["cfck"] = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" && t.MedicalItemType == 1 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1" && t.MedicalItemType == 1 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds();
                        dr["ypqtck"] = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" && t.MedicalItemType == 1 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" && t.MedicalItemType == 1 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() + (data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" && t.MedicalItemType == 1 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0))).ToFillRounds();
                        dr["wcck"] = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" && t.MedicalItemType == 2 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1" && t.MedicalItemType == 2 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds();
                        dr["wcqtck"] = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" && t.MedicalItemType == 2 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" && t.MedicalItemType == 2 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)).ToFillRounds() + (data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" && t.MedicalItemType == 2 && t.CenterId == item.Id).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0))).ToFillRounds();
                        dr["tx"] = dr["txf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(dr["txf"]) - Convert.ToDecimal(dr["ypqtck"]) - Convert.ToDecimal(dr["wcqtck"])) / Convert.ToDecimal(dr["txf"])) * 100).ToFillRounds() + "%";
                        dr["yp"] = dr["yf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(dr["yf"]) - Convert.ToDecimal(dr["cfck"])) / Convert.ToDecimal(dr["yf"])) * 100).ToFillRounds() + "%";
                        dr["cl"] = dr["clf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(dr["clf"]) - Convert.ToDecimal(dr["wcck"])) / Convert.ToDecimal(dr["clf"])) * 100).ToFillRounds() + "%";
                        dt.Rows.Add(dr);
                    }
                }
                DataRow drtj = dt.NewRow();
                drtj["centerName"] = "合计";
                if (inPut.staType == 1)
                {
                    drtj["xj"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("xj"))).Sum() / 2;
                    drtj["txf"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("txf"))).Sum() / 2;
                    drtj["yf"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("yf"))).Sum() / 2;
                    drtj["clf"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("clf"))).Sum() / 2;
                    drtj["yyeqt"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("yyeqt"))).Sum() / 2;
                    drtj["yyexj"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("yyexj"))).Sum() / 2;
                    drtj["cfck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("cfck"))).Sum() / 2;
                    drtj["ypqtck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("ypqtck"))).Sum() / 2;
                    drtj["wcck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("wcck"))).Sum() / 2;
                    drtj["wcqtck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("wcqtck"))).Sum() / 2;
                }
                else
                {
                    drtj["xj"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("xj"))).Sum();
                    drtj["txf"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("txf"))).Sum();
                    drtj["yf"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("yf"))).Sum();
                    drtj["clf"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("clf"))).Sum();
                    drtj["yyeqt"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("yyeqt"))).Sum();
                    drtj["yyexj"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("yyexj"))).Sum();
                    drtj["cfck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("cfck"))).Sum();
                    drtj["ypqtck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("ypqtck"))).Sum();
                    drtj["wcck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("wcck"))).Sum();
                    drtj["wcqtck"] = dt.AsEnumerable().Select(d => Convert.ToDecimal(d.Field<string>("wcqtck"))).Sum();
                }
                drtj["tx"] = drtj["txf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(drtj["txf"]) - Convert.ToDecimal(drtj["ypqtck"]) - Convert.ToDecimal(drtj["wcqtck"])) / Convert.ToDecimal(drtj["txf"])) * 100).ToFillRounds() + "%";
                drtj["yp"] = drtj["yf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(drtj["yf"]) - Convert.ToDecimal(drtj["cfck"])) / Convert.ToDecimal(drtj["yf"])) * 100).ToFillRounds() + "%";
                drtj["cl"] = drtj["clf"].ToString() == "0" ? "-" : (((Convert.ToDecimal(drtj["clf"]) - Convert.ToDecimal(drtj["wcck"])) / Convert.ToDecimal(drtj["clf"])) * 100).ToFillRounds() + "%";
                dt.Rows.Add(drtj);


                ALLChargeCasesOutPut aLLDayIncomeOutPut = new ALLChargeCasesOutPut();
                aLLDayIncomeOutPut.tableHeaders = tableHeaders;
                aLLDayIncomeOutPut.SiRatioOutPuts = dt;
                return (object)aLLDayIncomeOutPut;
            });

        }

        /// <summary>
        /// 渝快保赔付统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public Task<ChargeHifesPayOutPut[]> GetChargeHifesPayAsync(PatientDialysisInput input)
        {
            return Task.Run(() =>
           {
               if (input == null)
                   throw new Exception("请输入有效的查询参数");
               if (string.IsNullOrWhiteSpace(input.CenterId))
                   throw new Exception("请选择有效的透析中心");

               input.BeginTime = new DateTime(input.BeginTime.Year, input.BeginTime.Month, input.BeginTime.Day, 0, 0, 0);
               input.EndTime = new DateTime(input.EndTime.Year, input.EndTime.Month, input.EndTime.Day, 23, 59, 59);
               string QuerySQL = $@" SELECT ROW_NUMBER()  OVER (ORDER BY a.JSRQ ASC) AS SerialNo,cd.ShortName,mdtrt_id ,JSJYLSH ,XM  ,c.CardNum ,CONVERT(varchar(100),a.JSRQ,120) AS JSRQ,'' AS RYZDMC,a.ZJE  , hifes_pay FROM dbo.SI_JSMX  AS a
LEFT JOIN  dbo.BalanceMain AS  b  ON a.JSJYLSH= b.SISourceBalanceNo
LEFT JOIN  dbo.Patient AS  c  ON b.PatientNo=c.PatientNo and b.CenterId = c.CenterId
LEFT JOIN dbo.SI_JZDJMX AS d  ON a.ZYMZH=d.ZYMZH
left join dbo.CenterDialysiss as cd on cd.Id = a.CenterId
 WHERE hifes_pay>0  AND b.BalanceState =1 AND a.JSRQ>='{input.BeginTime}' and a.JSRQ<='{input.EndTime}'    and a.CenterId ='{input.CenterId}'   ORDER BY a.JSRQ ASC 
";
               var sqlParameterList = new List<SqlParameter>();
               //医保结算 
               var HifesPaydt = MsSqlHelper.GetSingleObj().GetDataTable(QuerySQL, sqlParameterList.ToArray());

               var HifesPayData = GetTupleByList<ChargeHifesPayOutPut>(HifesPaydt).Item2;
               HifesPayData.ForEach(t => { t.StrJSRQ = t.JSRQ.ToString("yyyy-MM-dd HH:mm:ss"); t.StrCYRQ = t.JSRQ.ToString("yyyy-MM-dd"); });
               return HifesPayData.ToArray();
           });
        }

        public Task<SIItemUploadInfoModel[]> GetSIItemUploadInfoAsync(PatientDialysisInput input)
        {
            return Task.Run(() =>
            {
                if (input == null)
                    throw new Exception("请输入有效的查询参数");
                if (string.IsNullOrWhiteSpace(input.CenterId))
                    throw new Exception("请选择有效的透析中心");

                input.BeginTime = new DateTime(input.BeginTime.Year, input.BeginTime.Month, input.BeginTime.Day, 0, 0, 0);
                input.EndTime = new DateTime(input.EndTime.Year, input.EndTime.Month, input.EndTime.Day, 23, 59, 59);
                string sql = $@"  SELECT aaa.ItemType,aaa.XMMC,aaa.Specifications,aaa.YBLSH,aaa.YYNM,aaa.Manufacturer,aaa.strHiLevel,SUM(aaa.SL) AS TotalCount FROM (            
--查询正常收费
SELECT CASE WHEN c.MedicalItemType=2 THEN '卫生耗材' WHEN c.MedicalItemType=5 THEN '诊疗项目' WHEN c.MedicalItemType=1 THEN  '药品' ELSE '其他类别'END AS ItemType, 
CASE WHEN c.HiLevel=1 THEN '甲类' WHEN c.HiLevel=2 THEN '乙类'  ELSE '丙类'END AS strHiLevel, 
bd.ItemName  as XMMC, c.Specifications, bd.NationItemCode as  YBLSH,bd.ItemCode as  YYNM,bd.Qty as SL,bm.BalanceDate  as JSRQ,c.Manufacturer,bd. OpenDateTime  as  KFRQ
from  BalanceMain bm  
left join  BalanceDetails bd  on bm.BalanceNo =bd.BalanceNo  and bm.CenterId = bd.CenterId
LEFT JOIN dbo.MedicalItemRecords c ON c.Id  = bd.ItemId 
where bm.BalanceDate >='{input.BeginTime}' and bm.BalanceDate <'{input.EndTime}'  and bm.CenterId='{input.CenterId}' 
 and  BalanceType=1   and BalanceState=1  
   UNION ALL    
   --查询正常收费
SELECT   CASE WHEN c.MedicalItemType=2 THEN '卫生耗材' WHEN c.MedicalItemType=5 THEN '诊疗项目' WHEN c.MedicalItemType=1 THEN  '药品' ELSE '其他类别'END AS ItemType,
CASE WHEN c.HiLevel=1 THEN '甲类' WHEN c.HiLevel=2 THEN '乙类'  ELSE '丙类'END AS strHiLevel, 
bd.ItemName  as XMMC, c.Specifications, bd.NationItemCode as  YBLSH,bd.ItemCode as  YYNM,bd.Qty as SL,bm.BalanceDate  as JSRQ,c.Manufacturer,bd. OpenDateTime  as  KFRQ
from  BalanceMain bm  
left join  BalanceDetails bd  on bm.BalanceNo =bd.BalanceNo  and bm.CenterId = bd.CenterId
LEFT JOIN dbo.MedicalItemRecords c ON c.Id  = bd.ItemId 
where   bm.BalanceDate >='{input.BeginTime}' and bm.BalanceDate < '{input.EndTime}' and  BalanceType=1  and BalanceState=0    and    bm.CenterId='{input.CenterId}' 
UNION ALL     --查询在查询月份退费                
SELECT   CASE WHEN c.MedicalItemType=2 THEN '卫生耗材' WHEN c.MedicalItemType=5 THEN '诊疗项目' WHEN c.MedicalItemType=1 THEN  '药品' ELSE '其他类别'END AS ItemType,
CASE WHEN c.HiLevel=1 THEN '甲类' WHEN c.HiLevel=2 THEN '乙类'  ELSE '丙类'END AS strHiLevel, 
bd.ItemName  as XMMC, c.Specifications, bd.NationItemCode as  YBLSH,bd.ItemCode as  YYNM,-bd.Qty as SL,bm.BalanceDate  as JSRQ,c.Manufacturer,bd. OpenDateTime  as  KFRQ
from  BalanceMain bm  
left join  BalanceDetails bd  on bm.BalanceNo =bd.BalanceNo  and bm.CenterId = bd.CenterId
LEFT JOIN dbo.MedicalItemRecords c ON c.Id  = bd.ItemId 
where    BalanceType=1  and BalanceState=0   and CancelDate >='{input.BeginTime}' and CancelDate < '{input.EndTime}' and  bm.CenterId='{input.CenterId}'

 ) aaa
WHERE aaa.ItemType IN ('药品','卫生耗材') 
GROUP BY aaa.ItemType,aaa.XMMC,aaa.Specifications,aaa.YBLSH,aaa.YYNM,aaa.Manufacturer,aaa.strHiLevel
ORDER BY aaa.ItemType desc;";

                #region
                //                string zymzhSql = $@"SELECT aa.ZYMZH,aa.TFBZ,COUNT(aa.ZYMZH) AS Count FROM(SELECT DISTINCT a.ZYMZH, a.TFBZ, a.JSJYLSH FROM dbo.SI_JSMX a
                //                         WHERE (a.JSRQ > '{input.BeginTime}' AND a.JSRQ < '{input.EndTime}')  and a.CenterId='{input.CenterId}' ) aa
                //                         GROUP BY aa.ZYMZH,aa.TFBZ";

                //                List<SIItemUploadInfoModel> sIItems = new List<SIItemUploadInfoModel>();


                var sqlParameterList = new List<SqlParameter>();
                //                //医保结算 
                //                var zymzhInfoDt = MsSqlHelper.GetSingleObj().GetDataTable(zymzhSql, sqlParameterList.ToArray());
                //                var zymzhInfo = GetTupleByList<TempZYMZHModel>(zymzhInfoDt).Item2;

                //                var zymzhList = zymzhInfo.ToList();
                //                if (zymzhList != null && zymzhList.Count > 0)
                //                {
                //                    //分成三类
                //                    var tfbzCount2List = zymzhList.Where(w => w.TFBZ == "-1" && w.Count == 2).Select(w => w.ZYMZH).ToList();
                //                    var tfbzCount1List = zymzhList.Where(w => w.TFBZ == "-1" && w.Count == 1).Select(w => w.ZYMZH).ToList();
                //                    var count1List = zymzhList.Where(w => w.TFBZ == "1" && w.Count == 1).Select(w => w.ZYMZH).ToList();

                //                    var count1ListStr = StringHelper.ListToStrsString(count1List, false);
                //                    var tfbzCount1ListStr = StringHelper.ListToStrsString(tfbzCount1List, false);

                //                    //查询所有的收费记录
                //                    string sql = $@"

                //SELECT aaa.ItemType,aaa.XMMC,aaa.Specifications,aaa.YBLSH,aaa.YYNM,aaa.Manufacturer,SUM(aaa.SL) AS TotalCount FROM (SELECT CASE
                //           WHEN LEFT(b.YYNM, 1) = 'U' THEN
                //               '卫生耗材'
                //           WHEN LEFT(b.YYNM, 1) = 'T' THEN
                //               '诊疗项目'
                //           WHEN LEFT(b.YYNM, 1) = 'D' THEN
                //               '药品'
                //           ELSE
                //               '其他类别'
                //       END AS ItemType,
                //	   b.XMMC,
                //	   c.Specifications,
                //	   b.YBLSH,
                //	   b.YYNM,
                //	   b.SL,
                //	   a.JSRQ,
                //	   c.Manufacturer,
                //       b.KFRQ,
                //  a.CenterId
                //FROM dbo.SI_JSMX a
                //    LEFT JOIN dbo.SI_CFMX b
                //        ON a.JSJYLSH = b.JSJYLSH and a.CenterId = b.CenterId
                //LEFT JOIN dbo.MedicalItemRecords c ON c.MedicalItemCode = b.YYNM
                //WHERE     a.CenterId='{input.CenterId}' and  a.JSJYLSH IS NOT NULL 
                //AND a.ZYMZH IN ({count1ListStr}) 
                //UNION ALL
                //SELECT CASE
                //           WHEN LEFT(b.YYNM, 1) = 'U' THEN
                //               '卫生耗材'
                //           WHEN LEFT(b.YYNM, 1) = 'T' THEN
                //               '诊疗项目'
                //           WHEN LEFT(b.YYNM, 1) = 'D' THEN
                //               '药品'
                //           ELSE
                //               '其他类别'
                //       END AS ItemType,
                //	   b.XMMC,
                //	   c.Specifications,
                //	   b.YBLSH,
                //	   b.YYNM,
                //	   b.SL,
                //	   a.JSRQ,
                //	   c.Manufacturer,
                //       b.KFRQ,
                //  a.CenterId
                //FROM dbo.SI_JSMX a
                //    LEFT JOIN dbo.SI_CFMX b
                //        ON a.ZYMZH = b.ZYMZH  and a.CenterId = b.CenterId
                //LEFT JOIN dbo.MedicalItemRecords c ON c.MedicalItemCode = b.YYNM
                //WHERE   a.CenterId='{input.CenterId}' and   a.JSJYLSH IS NOT NULL AND a.ZYMZH IN ({tfbzCount1ListStr}) and b.SL < 0 AND a.JSRQ > '{input.BeginTime}' and a.JSRQ < '{input.EndTime}'
                //) aaa
                //WHERE aaa.ItemType IN ('药品','卫生耗材')    and aaa.CenterId='{input.CenterId}' and aaa.JSRQ > '{input.BeginTime}' and aaa.JSRQ < '{input.EndTime}'
                //GROUP BY aaa.ItemType,aaa.XMMC,aaa.Specifications,aaa.YBLSH,aaa.YYNM,aaa.Manufacturer
                //ORDER BY aaa.ItemType desc

                //";
                #endregion
                //                    //医保结算 

                var HifesPaydt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
               var      sIItems = GetTupleByList<SIItemUploadInfoModel>(HifesPaydt).Item2;
                
                return sIItems.ToArray();


            });
        }


        public Task<FeeDetailsModel[]> GetDealQueryModeDetails(PatientDialysisInput input)
        {

            return Task.Run(() =>
            {
                if (input == null)
                    throw new Exception("请输入有效的查询参数");
                if (string.IsNullOrWhiteSpace(input.CenterId))
                    throw new Exception("请选择有效的透析中心");

                input.BeginTime = new DateTime(input.BeginTime.Year, input.BeginTime.Month, input.BeginTime.Day, 0, 0, 0);
                input.EndTime = new DateTime(input.EndTime.Year, input.EndTime.Month, input.EndTime.Day, 23, 59, 59);
                string CenterIdSql = "";
                if (input.CenterId != "0")
                {
                    CenterIdSql = $" AND a.CenterId='{input.CenterId}'";
                }
                else
                {
                    throw new Exception("请勿查询全部透析中心");
                }
                string zymzhSql = $@"
SELECT CASE when b.BalanceType = 1 then '医保结算' when b.BalanceType = 2 then '自费结算' else '其他类别结算' end as BalanceType,a.RecipelNo,a.BalanceNo,
a.ItemName,a.ItemCode,a.Qty,a.UnitPrice,a.TotalPrice,b.BalanceDate,b.CancelDate,b.BalanceState,a.DetailsId,b.PatientNo,a.CategoryName,a.Specifications,
p.Name   AS PatientName,
cc.FounderDate AS PrescriptionDetailFounderDate, 
case
	
	when
	pre.OldPrescriptionId Is NULL
	then
	(SELECT  cur.DialysisType FROM  CurrentDialysisProgram AS cur WHERE  cur.PatientId=pre.PatientId AND CONVERT(varchar(10),cur.Date ,120)= CONVERT(varchar(10),cc.FounderDate ,120) )
	else
	(SELECT  cur.DialysisType FROM  CurrentDialysisProgram AS cur WHERE  cur.PatientId=pre.PatientId AND CONVERT(varchar(10),cur.Date ,120)= CONVERT(varchar(10), ( select FounderDate from Prescription where id =pre.OldPrescriptionId  ) ,120) )
	
end AS DialysisType,'1' AS 'DialysisQty'
  FROM dbo.BalanceDetails a
  LEFT JOIN dbo.BalanceMain b   ON a.BalanceNo = b.BalanceNo and a.CenterId = b.CenterId

  LEFT JOIN dbo.PrescriptionDetail cc  ON cc.Id = a.DetailsId 
  LEFT JOIN dbo.Prescription AS  pre  ON pre.Id=cc.PrescriptionId 
LEFT JOIN dbo.Patient p   ON p.id=pre.PatientId
 
  WHERE b.SourceBalanceNo IS NULL AND  a.ItemName in ('血液灌流' ,'血液滤过','血液透析','血液透析滤过','血液透析滤过费','血液透析费','血液透析灌流费','血液滤过费','血液灌流费')
  AND cc.DataState = 1 AND b.BalanceState=1 AND (b.BalanceDate >=  '{input.BeginTime}'  AND b.BalanceDate <=  '{input.EndTime}' )
  {CenterIdSql}  order by b.BalanceDate

";


                var sqlParameterList = new List<SqlParameter>();
                //医保结算 
                var zymzhInfoDt = MsSqlHelper.GetSingleObj().GetDataTable(zymzhSql, sqlParameterList.ToArray());


                var zymzhInfo = GetTupleByList<FeeDetailsModel>(zymzhInfoDt).Item2;
                //zymzhInfo.Add(new FeeDetailsModel()
                //{
                //    Qty = zymzhInfo.Sum(t => t.Qty),
                //    TotalPrice = zymzhInfo.Sum(t => t.TotalPrice)
                //});
                return zymzhInfo.ToArray();


            });
        }



        #endregion









    }

}

