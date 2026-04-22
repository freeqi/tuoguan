using CDGService.Utils;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class BusinessTargetClntroller : Controller
    {

        private readonly Data.DocumentSetting _documentSetting;
        private readonly BusinessTargetStatisticalManager _businessTargetStatisticalManager;
        private readonly CenterDialysisManger _centerDialysisManger;
        public BusinessTargetClntroller(IOptions<Data.DocumentSetting> documentSetting, BusinessTargetStatisticalManager businessTargetStatisticalManager, CenterDialysisManger centerDialysisManger)
        {
            _businessTargetStatisticalManager = businessTargetStatisticalManager;
            _documentSetting = documentSetting.Value;
            _centerDialysisManger = centerDialysisManger;
        }

        //        public Task<BusinessTarget[]> GetBusinessTargets(int MainKey, int IndexKey)
        /// <summary>
        /// 获取统计项
        /// MainKey 1财务管理 2 物资管理 3 能耗管理
        /// IndexKye （1 收支2统计 3 盈利）（1 物资报表 2 物资统计） （1能耗管理）
        /// </summary>
        /// <param name="MainKey">1财务管理 2 物资管理 3 能耗管理</param>
        /// <param name="IndexKey">（1 收支2统计 3 盈利）（1 物资报表 2 物资统计） （1能耗管理）</param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/{MainKey}/{IndexKey}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<Data.BusinessTarget[]>> ObserveIndicatorsAsync([FromRoute] int MainKey, int IndexKey)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetBusinessTargets(MainKey, IndexKey);
                return new ServiceMessage<Data.BusinessTarget[]>(result);
            });
        }


        //  public Task<object> GetSIRatioAsync(IncomeSummaryQueryInPut inPut)
        /// <summary>
        /// 医保报销比率
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/GetSIRatioAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> GetSIRatioAsync([FromBody] IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetNewSIRatioAsync(inPut);
                return new ServiceMessage<object>(result);
            });
        }

        /// <summary>
        /// 物品收入占比
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/GetItemSIRatioAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> GetItemsSIRatioAsync([FromBody] IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetNewItemsSIRatioAsync(inPut);
                return new ServiceMessage<object>(result);
            });
        }

        //  public Task<PatientDialysisOutPut[]> GetPatientDialysisAsync(PatientDialysisInput inPut)
        /// <summary>
        /// 患者透析例次
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/GetPatientDialysis")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PatientDialysisOutPut[]>> GetPatientDialysisAsync([FromBody] PatientDialysisInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetPatientDialysisAsync(inPut);
                return new ServiceMessage<PatientDialysisOutPut[]>(result);
            });
        }
        //  public Task<PatientsReportOutPut[]> GetPatientsReportAsync(PatientsReportQueryInput inPut)
        /// <summary>
        /// 财务统计-患者统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/PatientsReport")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PatientsReportOutPut[]>> GetPatientsReportAsync([FromBody] PatientsReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetPatientsReportAsync(inPut);
                return new ServiceMessage<PatientsReportOutPut[]>(result);
            });
        }
        // public Task<EmployeeReportOutPut[]> GetEmployeesReportAsync(EmployeeReportQueryInput inPut)

        /// <summary>
        /// 财务统计--医护人员统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/Employee")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmployeeReportOutPut[]>> GetEmployeesReportAsync([FromBody] EmployeeReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetEmployeesReportAsync(inPut);
                return new ServiceMessage<EmployeeReportOutPut[]>(result);
            });
        }

        //  public Task<EquipmentReportOutPut[]> GetEquipmentReportAsync(EquipmentReportQueryInput inPut)

        /// <summary>
        /// 财务统计--血透机器数量统计表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/Equipment")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EquipmentReportOutPut[]>> GetEquipmentReportAsync([FromBody] EquipmentReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetEquipmentReportAsync(inPut);
                return new ServiceMessage<EquipmentReportOutPut[]>(result);
            });
        }
        //
        /// <summary>
        /// 财务统计--治疗模式及治疗例次统计表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/CurePatternCheck")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ALLChargeCasesOutPut>> GetCurePatternCheckReportAsync([FromBody] CurePatternQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetCurePatternCheckReportAsync(inPut);
                return new ServiceMessage<ALLChargeCasesOutPut>(result);
            });
        }
        //床位         public Task<BedsUseReportReportOutPut[]> GetBedsUseReportAsync(BedsUseReportQueryInput inPut)
        /// <summary>
        /// 财务统计--床位日均使用次数表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/BedsUseReport")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<BedsUseReportReportOutPut[]>> GetBedsUseReportAsync([FromBody] BedsUseReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetBedsUseReportAsync(inPut);
                return new ServiceMessage<BedsUseReportReportOutPut[]>(result);
            });
        }

        // public Task<PatientSaverageOutPut[]> GetPatientSaverageReportAsync(PatientSaverageQuerInput inPut)

        /// <summary>
        /// 财务统计--患者人均月收费及成本统计表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/PatientSaverage")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PatientSaverageOutPut[]>> GetPatientSaverageReportAsync([FromBody] PatientSaverageQuerInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetPatientSaverageReportAsync(inPut);
                return new ServiceMessage<PatientSaverageOutPut[]>(result);
            });
        }
        /// <summary>
        /// 患者人均每例收费及成本
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/PerCapitaDialysis")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PerCapitaDialysisOutPut[]>> PerCapitaDialysis([FromBody] CurePatternQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.PerCapitaDialysis(inPut);
                return new ServiceMessage<PerCapitaDialysisOutPut[]>(result);
            });
        }

        //    public Task<MedicalRevenueOutPut[]> GetMedicalRevenueOutPutReportAsync(MedicalRevenueQuerInput inPut)

        /// <summary>
        /// 财务统计--医护人员人均月创收表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/MedicalRevenue")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalRevenueOutPut[]>> GetMedicalRevenueOutPutReportAsync([FromBody] MedicalRevenueQuerInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetMedicalRevenueOutPutReportAsync(inPut);
                return new ServiceMessage<MedicalRevenueOutPut[]>(result);
            });
        }

        //   //治疗例次/床位数/30天
        /// <summary>
        /// 单台机器月均收入表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>        
        [HttpPost("BusinessTarget/DialysisMachineRevenue")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DialysisMachineRevenueOutPut[]>> GetDialysisMachineRevenueReportAsync([FromBody] DialysisMachineRevenueInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetDialysisMachineRevenueReportAsync(inPut);
                return new ServiceMessage<DialysisMachineRevenueOutPut[]>(result);
            });
        }

        //   public Task<SaturatedMedicalOutPut[]> GetSaturatedMedicalReportAsync(SaturatedMedicalInput inPut)
        /// <summary>
        /// 医护工作饱和度
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>        
        [HttpPost("BusinessTarget/SaturatedMedical")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SaturatedMedicalOutPut[]>> GetSaturatedMedicalReportAsync([FromBody] SaturatedMedicalInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetSaturatedMedicalReportAsync(inPut);
                return new ServiceMessage<SaturatedMedicalOutPut[]>(result);
            });
        }


        #region  报表

        // public Task<IncomeSummaryOutPut[]> GetBusinessTargetsAsync(IncomeSummaryQueryInPut inPut)
        /// <summary>
        /// 收入总汇-- 安收费项目
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>        
        [HttpPost("BusinessTarget/ALL")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> GetBusinessTargetsAsync([FromBody] IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                if (inPut.Model <= 0)
                    throw new Exception("请提供统计方式，model=1：收费项目；model= 2：结算方式；model=3：回款方式");
                object result = null;
                switch (inPut.Model)
                {
                    case 1:
                        result = await _businessTargetStatisticalManager.GetNewBusinessTargetsAsync(inPut);
                        break;
                    case 2:
                        result = await _businessTargetStatisticalManager.GetNewBusinessPayTypeAsync(inPut);
                        break;
                    default:
                        result = await _businessTargetStatisticalManager.GetNewBusinessBackTypeAsync(inPut);
                        break;
                }

                return new ServiceMessage<object>(result);
            });
        }

        //public Task<object> GetBusinessCostsAsync(IncomeSummaryQueryInPut inPut)

        /// <summary>
        /// 成本总汇-- 按照收费项目
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>        
        [HttpPost("BusinessTarget/CostsAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> GetBusinessCostsAsync([FromBody] CostSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                object result = await _businessTargetStatisticalManager.GetBusinessCostsAsync(inPut);


                return new ServiceMessage<object>(result);
            });
        }
        //日报表    public Task<object> GetDayReportAsync(IncomeSummaryQueryInPut inPut)

        /// <summary>
        /// 日报表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>        
        [HttpPost("BusinessTarget/DayReportAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> GetDayReportAsync([FromBody] IncomeSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {

                object result = await _businessTargetStatisticalManager.GetDayReportAsync(inPut);


                return new ServiceMessage<object>(result);
            });
        }
        //  public Task<RefundOutPut[]> GetRefundAsync(RefundQueryInPut inPut)

        /// <summary>
        /// 退费清单
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>        
        [HttpPost("BusinessTarget/Refund")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<RefundOutPut[]>> GetRefundAsync([FromBody] RefundQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetRefundAsync(inPut);
                return new ServiceMessage<RefundOutPut[]>(result.Result, result.Count);
            });
        }
        //        public Task<RefundDetailOutPut[]> GetRefundDetailAsync(RefundDetailInPut inPut)
        /// <summary>
        /// 退费明细
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/RefundDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<RefundDetailOutPut[]>> GetRefundDetailAsync([FromBody] RefundDetailInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetRefundDetailAsync(inPut);
                return new ServiceMessage<RefundDetailOutPut[]>(result.Result, result.Count);
            });
        }

        // public Task<RefundOutPut[]> GetChargeAsync(RefundQueryInPut inPut)

        /// <summary>
        /// 收费清单
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/Charge")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<RefundOutPut[]>> GetChargeAsync([FromBody] RefundQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetNewChargeAsync(inPut);
                return new ServiceMessage<RefundOutPut[]>(result.Result, result.Count);
            });
        }
        //  public Task<RefundDetailOutPut[]> GetChargeDetailAsync(RefundDetailInPut inPut)
        /// <summary>
        /// 收费明细
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/ChargeDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ChargeDetailOutPut[]>> GetChargeDetailAsync([FromBody] RefundDetailInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetNewChargeDetailAsync(inPut);
                return new ServiceMessage<ChargeDetailOutPut[]>(result.Result, result.Count);
            });
        }
        //导出明细Excel
        /// <summary>
        /// 报表Excel导出
        /// </summary>
        /// <returns></returns>
        [HttpPost("ExportExcel/filedownload")]
        [CheckLogin]

        public virtual FileResult ExportExcelFile([FromBody] RefundDetailInPut InPut)
        {
            try
            {
                var result = _businessTargetStatisticalManager.GetNewChargeDetailAsync(InPut).Result.Result;

                var center = _centerDialysisManger.GetDialysisByiIdAsync(InPut.CenterId.First()).Result.First();
                string fileFullPath = _documentSetting.ExportExcel + "收费明细.xlsx";

                if (result.Length > 0)
                {
                    var temp = result.ToList();
                    temp.Add(new ChargeDetailOutPut()
                    {

                        No = result.Length + 1,
                        PName = "合计",
                        TotalPrice = result.Sum(t => t.TotalPrice)
                    });
                    result = temp.ToArray();
                }

                var excelcolumn = new List<ExcelColumnSetting>();

                excelcolumn.Add(new ExcelColumnSetting("No", "序号") { IsSeqColumn = true });
                //excelcolumn.Add(new ExcelColumnSetting("centerName", "机构"));
                excelcolumn.Add(new ExcelColumnSetting("PName", "姓名"));
                excelcolumn.Add(new ExcelColumnSetting("MZH", "门诊号"));
                excelcolumn.Add(new ExcelColumnSetting("BalanceNo", "结算单号"));
                excelcolumn.Add(new ExcelColumnSetting("OpenDate", "收费时间"));
                excelcolumn.Add(new ExcelColumnSetting("HealthCareType", "参保类别"));
                excelcolumn.Add(new ExcelColumnSetting("PrescriptionNo", "处方号"));
                excelcolumn.Add(new ExcelColumnSetting("SIBalanceSerialNo", "医保结算流水号"));
                excelcolumn.Add(new ExcelColumnSetting("ItemName", "名称"));
                excelcolumn.Add(new ExcelColumnSetting("Specifications", "规格型号"));
                excelcolumn.Add(new ExcelColumnSetting("CompareCode", "医保编码"));
                excelcolumn.Add(new ExcelColumnSetting("CategoryName", "项目类型"));
                excelcolumn.Add(new ExcelColumnSetting("UnitPrice", "单价"));
                excelcolumn.Add(new ExcelColumnSetting("Qty", "数量"));
                excelcolumn.Add(new ExcelColumnSetting("TotalPrice", "金额"));
                excelcolumn.Add(new ExcelColumnSetting("Level", "等级"));
                excelcolumn.Add(new ExcelColumnSetting("payProportion", "支付比例"));
                excelcolumn.Add(new ExcelColumnSetting("ISUpQty", "医保上传数量"));
                excelcolumn.Add(new ExcelColumnSetting("operationMan", "操作员"));
                ExcelHelper.Save<ChargeDetailOutPut>(result, excelcolumn, fileFullPath, "收费明细", center.DialysisName, $"时间：{InPut.BeginTime.ToString("yyyy年MM月dd日 00:00:00")} 至 {InPut.EndTime.ToString("yyyy年MM月dd日 23:59:59")}");



                if (!System.IO.File.Exists(fileFullPath))
                    throw new Exception($"文件{fileFullPath}不存在");
                var fileResult = new PhysicalFileResult(fileFullPath, "application/x-zip-compressed");

                return fileResult;


            }
            catch (Exception exp)
            {

                throw new Exception("下载文件失败，请联系技术人员");
            }

        }







        //GetChargeDetailByIdAsync
        /// <summary>
        /// 根据处方单号收费明细 
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpGet("BusinessTarget/ChargeDetailByNo/{inPut}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ChargeDetailOutPut[]>> GetChargeDetailByIdAsync([FromRoute] string inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetChargeDetailByIdAsync(inPut);
                return new ServiceMessage<ChargeDetailOutPut[]>(result);
            });
        }

        /// <summary>
        /// 收入、成本、毛利变动
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/CostChange")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> GetincomeChangeAsync([FromBody] IncomeChangeInput inPut)
        {
            return Task.Run(async () =>
            {
                object result = null;
                switch (inPut.CostChangeType)
                {
                    case 1:
                        result = await _businessTargetStatisticalManager.GetincomeChangeAsync(inPut);
                        break;

                    case 2:
                        result = await _businessTargetStatisticalManager.GetCostChangeAsync(inPut);
                        break;
                    case 3:
                        result = await _businessTargetStatisticalManager.GetGrossPofitChangeAsync(inPut);
                        break;
                    default:
                        break;
                }

                return new ServiceMessage<object>(result);
            });
        }

        /// <summary>
        /// 医占比
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/MedicalProportion")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> MedicalProportionAsync([FromBody] CostSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                object result = null;

                result = await _businessTargetStatisticalManager.MedicalProportionAsync(inPut);


                return new ServiceMessage<object>(result);
            });
        }

        /// <summary>
        /// 收费例次
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/ChargeCases")]
        //[CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> ChargeCasesAsync([FromBody] CostSummaryQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                object result = null;

                result = await _businessTargetStatisticalManager.ChargeCasesAsync(inPut);
                return new ServiceMessage<object>(result);
            });
        }

        /// <summary>
        /// 营业额及成本
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/IncomeAdnCost")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> GetIncomeAdnCostAsync([FromBody] PatientsReportQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                object result = null;

                result = await _businessTargetStatisticalManager.GetIncomeAdnCostAsync(inPut);
                return new ServiceMessage<object>(result);
            });
        }


        //   public Task<ChargeHifesPayOutPut[]> GetChargeHifesPayAsync(PatientDialysisInput input)

        /// <summary>
        /// 渝快保赔付明细统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/HifesPay")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ChargeHifesPayOutPut[]>> GetChargeHifesPayAsync([FromBody] PatientDialysisInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetChargeHifesPayAsync(inPut);
                return new ServiceMessage<ChargeHifesPayOutPut[]>(result);
            });
        }
        /// <summary>
        /// 医保物资上传数据汇总统计表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/SIItemUploadInfo")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SIItemUploadInfoModel[]>> GetSIItemUploadInfoAsync([FromBody] PatientDialysisInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetSIItemUploadInfoAsync(inPut);
                return new ServiceMessage<SIItemUploadInfoModel[]>(result);
            });
        }
        //public Task<SIItemUploadInfoModel[]> GetSIItemUploadInfoAsync(PatientDialysisInput input)


        //  public Task<FeeDetailsModel[]> GetDealQueryModeDetails(PatientDialysisInput input)

        /// <summary>
        /// 透析模式、例次查询
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("BusinessTarget/SIDealQuery")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<FeeDetailsModel[]>> GetDealQueryModeDetails([FromBody] PatientDialysisInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _businessTargetStatisticalManager.GetDealQueryModeDetails(inPut);
                return new ServiceMessage<FeeDetailsModel[]>(result);
            });
        }

        #endregion
    }
}
