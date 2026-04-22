using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /*-----------------------------
     *-----------------------------
     * 财务报表相关InPu OutPut
     *-----------------------------
     *-----------------------------
     */
    #region  收支报表
    /// <summary>
    /// 收入日报表
    /// </summary>
    public class DayItemIncomeQuerInPut
    {
        public string CenterId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }


    }

    public class DayItemIncomeOutPut
    {

        public string Id { get; set; }
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }

        public string CenterName { get; set; }
        /// <summary>
        /// 收费项目
        /// </summary>
        public string SysDicCost { get; set; }
        /// <summary>
        /// 来源 （药品-药房 材料 - 库房  透析费-肾内科）
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string Money { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        public string SettlementDate { get; set; }

    }



    /// <summary>
    /// 收入总汇表
    /// </summary>
    public class IncomeSummaryQueryInPut
    {
        public List<string> CenterId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 结算方式 1 收费项目 2 结算方式 3 回款方式
        /// 收入类型：1 药品 2 耗材 3、诊疗项目
        /// </summary>
        public int Model { get; set; }
    }


    public class PatientDialysisInput
    {
        public string CenterId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }


    }

    /// <summary>
    /// 成本总汇表
    /// </summary>
    public class CostSummaryQueryInPut
    {
        public List<string> CenterId { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 1 按月统计 2 合并统计
        /// </summary>
        public int MonthModel { get; set; } =1;

    }
    public class IncomeSummaryOutPut
    {

        // public string Id { get; set; }
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }

        public string ItemName { get; set; }
        /// <summary>
        /// 药品费
        /// </summary>
        public decimal YP { get; set; }
        /// <summary>
        /// 检查
        /// </summary>
        public decimal JC { get; set; }
        /// <summary>
        /// 治疗
        /// </summary>
        public decimal ZL { get; set; }
        /// <summary>
        /// 透析
        /// </summary>
        public decimal TX { get; set; }
        /// <summary>
        /// 护理
        /// </summary>
        public decimal HL { get; set; }
        /// <summary>
        /// 氧气
        /// </summary>
        public decimal YQ { get; set; }
        /// <summary>
        /// 材料
        /// </summary>
        public decimal CL { get; set; }
        /// <summary>
        /// 其他
        /// </summary>
        public decimal QT { get; set; }
        /// <summary>1
        /// 合计金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
       // public string SettlementDate { get; set; }
    }

    /// <summary>
    /// 日报表
    /// </summary>
    public class DayReportOutPut
    {
        /// <summary>
        /// 收费情况
        /// </summary>        

        public decimal? total { get; set; }
        /// <summary>
        /// 现金
        /// </summary>
        public decimal? xj { get; set; }
        /// <summary>
        /// 个人账户
        /// </summary>
        public decimal? Individual { get; set; }
        /// <summary>
        /// 民政补助
        /// </summary>
        public decimal? mzbz { get; set; }
        /// <summary>
        /// 大额统筹
        /// </summary>
        public decimal? detc { get; set; }
        /// <summary>
        /// 医保基金
        /// </summary>
        public decimal? ybjj { get; set; }
        /*居民*/

        /// <summary>
        /// 个人账户
        /// </summary>
        public decimal? jmindividual { get; set; }
        /// <summary>
        /// 民政补助
        /// </summary>
        public decimal? jmmzbz { get; set; }
        /// <summary>
        /// 大额统筹
        /// </summary>
        public decimal? jmdetc { get; set; }
        /// <summary>
        /// 医保基金
        /// </summary>
        public decimal? jmybjj { get; set; }



        /// <summary>
        /// 医院超标
        /// </summary>
        public decimal? yycb { get; set; }

        /// <summary>
        /// 票据收退
        /// </summary>
        public string pjst { get; set; }
        /// <summary>
        /// 号码范围
        /// </summary>
        public string hmfw { get; set; }
        /// <summary>
        /// 实际票号
        /// </summary>
        public string sjph { get; set; }
    }






    public class DayIncomeOutPut
    {
        public int no { get; set; }
        public string ItemName { get; set; }
        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal? TotalMoney { get; set; }
    }

    public class CenterDayIncomeOutPut
    {
        public int no { get; set; }
        public string itemMainName { get; set; }
        public string itemName { get; set; }

        public decimal? kmtxzx { get; set; } = 0;
        public decimal? xstxzx { get; set; } = 0;
        public decimal? dzsmzb { get; set; } = 0;
        public decimal? jlptxzx { get; set; } = 0;
        public decimal? tltxzx { get; set; } = 0;
        public decimal? fltxzx { get; set; } = 0;
        public decimal? spbtxzx { get; set; } = 0;
        public decimal? zxtxzx { get; set; } = 0;
        public decimal? tjmzb { get; set; } = 0;

        public decimal? total { get; set; } = 0;

    }

    public class ChargeTable
    {
        public string CenterId { get; set; }
        public decimal? TotalPrice { get; set; }
        public string FreeTypeName { get; set; }

        public DateTime? BalanceDate { get; set; }
    }


    public class ALLDayIncomeOutPut
    {
        public List<TableHeader> tableHeaders { get; set; }
        public List<CenterDayIncomeOutPut> CenterDayIncomeOutPuts { get; set; }
    }

    /// <summary>
    /// 退费清单
    /// </summary>
    public class RefundQueryInPut
    {
        public int PageNum { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string keyword { get; set; }
        public List<string> CenterId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }
    }


    public class RefundOutPut
    {
        public string Id { get; set; }
        public string centerName { get; set; }
        public int No { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PName { get; set; }

        public int BalanceType { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }
        /// <summary>
        /// 医保类型
        /// </summary>
        public string HealthCareType { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string MZType { get; set; } = "";
        /// <summary>
        /// 处方号
        /// </summary>
        public string PrescriptionNo { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public decimal? SumPrice { get; set; }
        /// <summary>
        /// 医保基金
        /// </summary>
        public decimal? YBJJ { get; set; }
        /// <summary>
        /// 大额
        /// </summary>
        public decimal? DETC { get; set; }
        /// <summary>
        /// 统筹
        /// </summary>
      //  public decimal? tc { get; set; }
        /// <summary>
        /// 民政救助
        /// </summary>
        public decimal? MZJZ { get; set; }
        /// <summary>
        /// 账户支付
        /// </summary>
        public decimal? ZHZF { get; set; }
        /// <summary>
        /// 现金
        /// </summary>
        public decimal? XJ { get; set; }
        /// <summary>
        /// 医院超标
        /// </summary>
        public decimal? YYCB { get; set; }
        /// <summary>
        /// 误差
        /// </summary>
        public decimal? WX { get; set; }
        /// <summary>
        /// 退费时间
        /// </summary>
        public string CancelDate { get; set; }
        public string BalanceDate { get; set; }

        public string medins_setl_id { get; set; }
        /// <summary>
        /// 公务员补助
        /// </summary>
        public decimal? GWYBZ { get; set; }
        /// <summary>
        /// 其他支付
        /// </summary>
        public decimal? oth_pay { get; set; }
        /// <summary>
        /// 企业补充医疗保险基金
        /// </summary>
        public decimal? hifes_pay { get; set; }
    }

    public class RefundDetailInPut
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; } = 1;
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; } = 25;
        /// <summary>
        /// 处方号/患者姓名
        /// </summary>
        public string keyword { get; set; }
        public List<string> CenterId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }
    }
    public class RefundDetailOutPut
    {
        public int No { get; set; }
        public string centerName { get; set; }
        //姓名	门诊号	退费日期	处方号	名称	 单价 	数量	 金额 	操作员
        public string PName { get; set; }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string MZH { get; set; }
        /// <summary>
        /// 退费时间
        /// </summary>
        public string CancelDate { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string PrescriptionNo { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Qty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal? TotalPrice { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string operationMan { get; set; }
    }

    public class ChargeDetailOutPut
    {
        public int No { get; set; }
        public string Id { get; set; }
        public string centerName { get; set; }
        public string PName { get; set; }
        public string BalanceNo { get; set; }
        /// <summary>
        /// 医保类型
        /// </summary>
        public string HealthCareType { get; set; }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string MZH { get; set; }
        /// <summary>
        /// 收费时间
        /// </summary>
        public string OpenDate { get; set; }
        public string OpenDateTime { get; set; }
        /// <summary>
        /// 退费时间
        /// </summary>
        public string CancelDate { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string PrescriptionNo { get; set; }
        //医保结算流水号
        public string SIBalanceSerialNo { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Qty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// 支付比例
        /// </summary>
        public decimal? payProportion { get; set; }
        /// <summary>
        /// 自费支付金额
        /// </summary>
        public decimal? PayAmount { get; set; }
        /// <summary>
        /// 医保支付金额
        /// </summary>
        public decimal? ISPayAmount { get; set; }
        /// <summary>
        /// 医保上传数量
        /// </summary>
        public decimal? ISUpQty { get; set; }
        /// <summary>
        ///等级
        /// </summary>
        public string Level { get; set; }

        public string CompareCode { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string operationMan { get; set; }
    }

    #endregion

    #region 财务统计分析

    //医保报销比率
    public class SIRatioOutPut
    {
        public string itemName { get; set; }
        public decimal? kmtxzx { get; set; } = 0;
        public string kmtxzxratios { get; set; }
        public decimal? xstxzx { get; set; } = 0;
        public string xstxzxratios { get; set; }
        public decimal? dzsmzb { get; set; } = 0;
        public decimal? jlptxzx { get; set; } = 0;
        public string jlptxzxratios { get; set; }
        public string dzsmzbratios { get; set; }

        public decimal? tltxzx { get; set; } = 0;
        public string tltxzxratios { get; set; }

        public decimal? spbtxzx { get; set; } = 0;
        public string spbtxzxratios { get; set; }

        public decimal? fltxzx { get; set; } = 0;
        public string fltxzxratios { get; set; }

        public decimal? zxtxzx { get; set; } = 0;
        public decimal? tjmzb { get; set; } = 0;
        public string zxtxzxratios { get; set; }
        public string tjmzbratios { get; set; }
        public decimal? jlkdtxzx { get; set; } = 0;
        public string jlkdbratios { get; set; }
        public decimal? total { get; set; } = 0;
        public string totalratios { get; set; }
    }
    public class ALLSIRatioOutPut
    {
        public List<TableHeader> tableHeaders { get; set; }
        public List<SIRatioOutPut> SIRatioOutPuts { get; set; }
    }

    public class ALLSIRatioData
    {
        public string CategoryName { get; set; }

        public decimal TotalPrice { get; set; }
        public string CBLB { get; set; }
        public string CenterId { get; set; }
        public decimal? XJZF { get; set; }

    }


    //患者
    public class PatientsReportQueryInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }
        /// <summary>
        /// 营业额统计处的统计方式  1每日统计 2综合统计
        /// </summary>
        public int staType { get; set; }

    }
    public class PatientsReportOutPut
    {
        public int No { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string CenterName { get; set; }
        /// <summary>
        /// 职工医保人数
        /// </summary>
        public int WorkerHealthCount { get; set; }
        /// <summary>
        /// 居民医保人数
        /// </summary>
        public int ResidentsHealthCount { get; set; }
        /// <summary>
        /// 其他或无医保人数
        /// </summary>
        public int OtherCount { get; set; }
        /// <summary>
        /// 新增患者人数
        /// </summary>
        public int NewCount { get; set; }
        /// <summary>
        /// 离院人数
        /// </summary>
        public int LossCount { get; set; }
        /// <summary>
        /// 离院率
        /// </summary>
        public string TurnoverRate { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public int Total { get; set; }
    }
    //员工 Employee

    //医护
    public class EmployeeReportQueryInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }
    public class EmployeeReportOutPut
    {
        public int No { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string CenterName { get; set; }
        /// <summary>
        /// 医生人数
        /// </summary>
        public int DoctorCount { get; set; }
        /// <summary>
        /// 护士人数
        /// </summary>
        public int RnurseCount { get; set; }
        /// <summary>
        /// 其他人数
        /// </summary>
        public int OtherCount { get; set; }
        ///// <summary>
        ///// 新增患者人数
        ///// </summary>
        //public int NewCount { get; set; }
        ///// <summary>
        ///// 离院人数
        ///// </summary>
        //public int LossCount { get; set; }
        /// <summary>
        /// 医护比例
        /// </summary>
        public string MedicalProportion { get; set; }
        /// <summary>
        /// 护士与床位比
        /// </summary>
        public string RnurseProportion { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public int Total { get; set; }
    }


    //设备

    public class EquipmentReportQueryInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }
    public class EquipmentReportOutPut
    {
        public int No { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string CenterName { get; set; }
        /// <summary>
        /// SWS-6000
        /// </summary>
        public int SWS6000Count { get; set; }
        /// <summary>
        /// SWS-6000A
        /// </summary>
        public int SWS6000ACount { get; set; }
        /// <summary>
        /// SWS-4000
        /// </summary>
        public int SWS4000Count { get; set; }

        /// <summary>
        /// SWS-4000A
        /// </summary>
        public int SWS4000ACount { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public int Total { get; set; }
    }


    //治疗模式例次  CurePatternQueryInput CurePatternOutPut
    public class CurePatternQueryInput
    {
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }

    public class TableHeader
    {
        public string title { get; set; }
        public string Key { get; set; }
        public string align { get { return "center"; } set { } }
        public int minWidth { get; set; } = 90;
        public int width { get; set; } = 90;
        public string Fixed { get; set; } = "";
        public List<TableHeader> Children { get; set; }
    }

    public class CurePatternData
    {
        public int no { get; set; }
        public string CenterName { get; set; }
        public int Total { get; set; }

        public int hd15_lc { get; set; }
        public int hdps160 { get; set; }
        public int hd150g { get; set; }
        public int hdfx10 { get; set; }
        public int hdfx8 { get; set; }
        public int hdnv15t { get; set; }
        public int hdnv18t { get; set; }
        public int hd15_uc { get; set; }
        public int hdfx60 { get; set; }
        public int hdfx80 { get; set; }
        public int hdhpfx10 { get; set; }
        public int hdhp150g { get; set; }
        public int hdhpfx80 { get; set; }
        public int hdhpfx8 { get; set; }
        public int hdhpfx60 { get; set; }
        public int hdhpps160 { get; set; }
        public int hdhp15_uc { get; set; }
        public int hdhp15_lc { get; set; }
        public int hdhpnv18t { get; set; }
        public int hdhpnv15t { get; set; }
        public int hdhpha130 { get; set; }
        public int hdffx80 { get; set; }
        public int hdffx60 { get; set; }
        public int hdf15_uc { get; set; }
        public int hdffx600 { get; set; }
        public int hdffx800 { get; set; }
        public int hfhdnv15t { get; set; }
        public int hfhdnv18t { get; set; }
        public int hfhd15_uc { get; set; }
        public int hfhdfx60 { get; set; }
        public int hfhdfx80 { get; set; }
        public int hfhd15_lc { get; set; }
        public int hfhdps160 { get; set; }
        public int hfhd150g { get; set; }
        public int hfhdfx10 { get; set; }
        public int scuf150g { get; set; }
        public int scuf15_lc { get; set; }


        /*
               * 

      15lc
      ps160
      150g
      fx10
      nv15t
      nv18t
      15_uc
      fx60
      fx80
      不使用
      fx10
      150g
      fx80
      fx60
      pc160
      15_uc
      15_lc
      nv18t
      nv15t
      ha130
      fx80
      fx60
      15_uc
      fx600
      fx800
      不使用
      nv15t
      nv18t
      15_uc
      fx60
      fx80
      15_lc
      ps160
      150g
      fx10
      不使用
      150g
      15_lc
      不使用


                   */
        /*
   public int hdf15_uc { get; set; }
   public int hdffx80 { get; set; }
   public int hdhp150g { get; set; }
   public int hdhpfx80 { get; set; }
   public int hdhpps160 { get; set; }
   public int hdhp15_lc { get; set; }
   public int hdhpfx10 { get; set; }
   public int hfhdfx60 { get; set; }
   public int hfhd15_uc { get; set; }
   public int hfhdfx80 { get; set; }
   public int scuf150g { get; set; }
   public int hd150g { get; set; }
   public int hd15_uc { get; set; }
   public int hdfx80 { get; set; }
   public int hdps160 { get; set; }
   public int hd15_lc { get; set; }
   public int hdfx10 { get; set; }
   public int hdffx60 { get; set; }
   */
    }
    public class PatientDialysisOutPut
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PrantName
        {
            get; set;
        }
        public int hd { get; set; }
        /// <summary>
        /// 透析时间
        /// </summary>
        public int hfhd { get; set; }
        /// <summary>
        /// 透析模式
        /// </summary>
        public int hdf { get; set; }

        public int hd_hp { get; set; }

        public int scuf { get; set; }

        public int hf { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public int count { get; set; }
    }



    public class CurePatternOutPut
    {
        public List<TableHeader> tableHeaders { get; set; }
        public List<CurePatternData> curePatternDatas { get; set; }
    }


    public class PerCapitaDialysisOutPut
    {
        public string CenterName { get; set; }
        public List<PerCapitaDialysisDetail> perCapitaDialysisDetail { get; set; } = new List<PerCapitaDialysisDetail>();

    }


    public class PerCapitaDialysisDetail
    {
        public string ItemName { get; set; }
        /// <summary>
        /// HD 成本
        /// </summary>
        public decimal? hdCost { get; set; }
        /// <summary>
        /// HD 收入
        /// </summary>
        public decimal? hdIncome { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? hdfCost { get; set; }
        /// <summary>
        ///  收入
        /// </summary>
        public decimal? hdfIncome { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? hfhdCost { get; set; }
        /// <summary>
        /// 收入
        /// </summary>
        public decimal? hfhdIncome { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? hdhpCost { get; set; }
        /// <summary>
        /// 收入
        /// </summary>
        public decimal? hdhpIncome { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public decimal? scufCost { get; set; }
        /// <summary>
        ///  收入
        /// </summary>
        public decimal? scufIncome { get; set; }
    }



    //床位日均使用次数表 
    public class BedsUseReportQueryInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }
    public class BedsUseReportReportOutPut
    {
        public int no { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 床位数
        /// </summary>
        public int BedsCount { get; set; }
        /// <summary>
        /// 透析次数
        /// </summary>
        public int CurePattern { get; set; }
        /// <summary>
        /// 日均使用次数
        /// </summary>
        public double DayCout { get; set; }
        /// <summary>
        /// 床位使用率
        /// </summary>
        public string UsageRate { get; set; }
    }

    /// <summary>
    /// 患者人均月收费及成本
    /// </summary>
    public class PatientSaverageQuerInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }
    /// <summary>
    /// 患者人均月收费及成本
    /// </summary>
    public class PatientSaverageOutPut
    {
        public int no { get; set; }
        //耗材收入(元)	药品收入	诊疗收入	耗材成本	药品成本	诊疗成本	合计	
        //收入(万元)  成本(万元)

        public string CenterName { get; set; }
        /// <summary>
        /// 耗材成本
        /// </summary>
        public decimal MaterialEarnings { get; set; }
        /// <summary>
        /// 药品成本
        /// </summary>
        public decimal DrugEarnings { get; set; }
        /// <summary>
        /// 透析费成本
        /// </summary>
        public decimal TreatmentEarnings { get; set; }
        /// <summary>
        /// 耗材收益
        /// </summary>
        public decimal MaterialCharge { get; set; }
        /// <summary>
        /// 药品收益
        /// </summary>
        public decimal DrugCharge { get; set; }
        /// <summary>
        /// 透析费收益
        /// </summary>
        public decimal TreatmentCharge { get; set; }
        /// <summary>
        /// 成本合计
        /// </summary>
        public decimal EarningsTotal { get; set; }
        /// <summary>
        /// 收益合计
        /// </summary>
        public decimal ChargeTotal { get; set; }
    }

    //医护人员人均月创收表

    /// <summary>
    ///医护人员人均月创收
    /// </summary>
    public class MedicalRevenueQuerInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }
    /// <summary>
    /// 医护人员人均月创收表
    /// </summary>
    public class MedicalRevenueOutPut
    {
        //机构名称（降序排列）	总创收（万元）	医护人员数量	人均创收(万元)

        //收入(万元)  成本(万元)

        public string CenterName { get; set; }

        /// <summary>
        /// 医护人数
        /// </summary>
        public decimal MedicalCount { get; set; }

        /// <summary>
        /// 总创收
        /// </summary>
        public decimal RevenueTotal { get; set; }
        /// <summary>
        /// 成本合计
        /// </summary>
        public decimal EarningsTotal { get; set; }
        /// <summary>
        /// 总创利
        /// </summary>
        public decimal ProfitTotal { get; set; }
        /// <summary>
        /// 人均创收
        /// </summary>
        public decimal PerCapita { get; set; }
        /// <summary>
        /// 人均创利
        /// </summary>
        public decimal PerProfit { get; set; }



    }


    // 单台机器月均收入 DialysisMachineRevenueOutPut DialysisMachineRevenueInput 

    /// <summary>
    ///单台机器月均收入
    /// </summary>
    public class DialysisMachineRevenueInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }
    /// <summary>
    /// 单台机器月均收入
    /// </summary>
    public class DialysisMachineRevenueOutPut
    {

        public string CenterName { get; set; }

        /// <summary>
        /// 机器数量
        /// </summary>
        public decimal MachineCount { get; set; }
        /// <summary>
        /// 透析次数
        /// </summary>
        public int CurePattern { get; set; }
        /// <summary>
        /// 机器均创收
        /// </summary>
        public decimal MachineCapita { get; set; }
        /// <summary>
        /// 总创收
        /// </summary>
        public decimal RevenueTotal { get; set; }
    }

    // 医护人员利用率 SaturatedMedical

    /// <summary>
    /// 医护人员工作饱和度
    /// </summary>
    public class SaturatedMedicalInput
    {
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime BeginReportDate { get; set; }
        public DateTime EndReportDate { get; set; }

    }
    /// <summary>
    /// 医护人员工作饱和度
    /// </summary>
    public class SaturatedMedicalOutPut
    {
        public int no { get; set; }
        public string CenterName { get; set; }

        /// <summary>
        /// 医护数量
        /// </summary>
        public decimal MachineCount { get; set; }
        /// <summary>
        /// 透析次数
        /// </summary>
        public int CurePattern { get; set; }
        /// <summary>
        /// 理论工作时间
        /// </summary>
        public decimal MachineCapita { get; set; }
        /// <summary>
        /// 透析工作时间
        /// </summary>
        public decimal RevenueTotal { get; set; }
        /// <summary>
        /// 饱和度
        /// </summary>
        public string Saturation { get; set; }
    }

    public class IncomeChangeInput
    {
        public List<string> CenterId { get; set; } = new List<string>();

        /// <summary>
        /// 1收入2成本3毛利
        /// </summary>
        public int CostChangeType { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    //成本变动  收入变动 毛利变动

    public class CostChangeInput
    {
        public string CenterId { get; set; }
        /// <summary>
        /// 1季度、2年度、3月度
        /// </summary>
        public int monthOrYear { get; set; }
        /// <summary>
        /// 1收入2成本3毛利
        /// </summary>
        public int CostChangeType { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    public class CostChangeOutPut
    {
        public List<TableHeader> tableHeaders { get; set; } = new List<TableHeader>();
        public List<CostChangeData> costChangeData { get; set; }
    }
    public class CostChangeData
    {
        public string itemName { get; set; }
        /// <summary>
        /// 1月/季度
        /// </summary>
        public decimal first { get; set; }
        /// <summary>
        /// 2月/季度
        /// </summary>
        public decimal second { get; set; }
        /// <summary>
        /// 3月/季度
        /// </summary>
        public decimal three { get; set; }
        /// <summary>
        ///4月/季度
        /// </summary>
        public decimal? four { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public decimal? total { get; set; }

    }

    public class IncomeChangeData
    {

        public int no { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 本期收入
        /// </summary>
        public decimal? NowIncome { get; set; }
        /// <summary>
        /// 上期收入
        /// </summary>
        public decimal? UpIncome { get; set; }
        /// <summary>
        /// 变动额
        /// </summary>
        public decimal? DifferIncome { get; set; }
        /// <summary>
        /// 变动率
        /// </summary>
        public string DifferRate { get; set; }
        /// <summary>
        /// 上年同期
        /// </summary>
        public decimal? UpYearNowIncome { get; set; }
        /// <summary>
        /// 上年同期变动额
        /// </summary>
        public decimal? UpYearDifferIncome { get; set; }
        /// <summary>
        /// 上年同期变动率
        /// </summary>
        public string UpYearDifferRate { get; set; }



    }
    /// <summary>
    /// 毛利增减变动表
    /// </summary>

    public class ProfitChangeData
    {
        public int no { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 本期收入
        /// </summary>
        public decimal? NowIncome { get; set; }
        /// <summary>
        /// 本期成本
        /// </summary>
        public decimal? NowCost { get; set; }
        /// <summary>
        /// 本期毛利
        /// </summary>
        public decimal? NowProfit { get; set; }
        /// <summary>
        /// 本期毛利率
        /// </summary>
        public decimal? ProfitRate { get; set; }

        /// <summary>
        /// 上期收入
        /// </summary>
        public decimal? UpIncome { get; set; }
        /// <summary>
        /// 上期成本
        /// </summary>
        public decimal? UpCost { get; set; }
        /// <summary>
        /// 上期毛利
        /// </summary>
        public decimal? upProfit { get; set; }
        /// <summary>
        /// 上期毛利率
        /// </summary>
        public decimal? upProfitRate { get; set; }

        /// <summary>
        /// 收入变动
        /// </summary>
        public decimal? DifferIncome { get; set; }

        /// <summary>
        /// 成本变动
        /// </summary>
        public decimal? DifferCost { get; set; }


        /// <summary>
        /// 毛利变动
        /// </summary>
        public decimal? DifferProfit { get; set; }
        /// <summary>
        /// 毛利率变动
        /// </summary>
        public decimal? ProfitRateRate { get; set; }


        /// <summary>
        /// 上年同期收入
        /// </summary>
        public decimal? UpYearNowIncome { get; set; }

        /// <summary>
        /// 上年同期成本
        /// </summary>
        public decimal? UpYearNowCost { get; set; }
        /// <summary>
        /// 上年同期毛利
        /// </summary>
        public decimal? UpYearNowProfit { get; set; }

        /// <summary>
        /// 上年同期毛利率
        /// </summary>
        public decimal? UpYearProfitRate { get; set; }


        /// <summary>
        /// 上年同期收入变动额
        /// </summary>
        public decimal? UpYearDifferIncome { get; set; }
        /// <summary>
        /// 上年同期成本变动额
        /// </summary>
        public decimal? UpYearDifferCost { get; set; }
        /// <summary>
        /// 上年同期毛利变动额
        /// </summary>
        public decimal? UpYearDifferProfit { get; set; }
        /// <summary>
        /// 上年同期毛利率变动
        /// </summary>
        public decimal? UpYearDifferProfitRate { get; set; }

    }

    #endregion


    #region 物资报表
    //请求
    public class MaterialsConfluenceInPut
    {
        /// <summary>
        /// 1药品 2 耗材  3 固定资产  4低值易耗
        /// </summary>
        public string MedicalItemType { get; set; }
        public string CenterId { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string SupplierId { get; set; }
        public string MedicalItemName { get; set; }
        public string OrderNo { get; set; }
        public string Id { get; set; }
        public string SupplierName { get; set; }

        public int? GroupType { get; set; }
    }

    public class MaterialsConfluenceInPuts
    {
        /// <summary>
        /// 1药品 2 耗材  3 固定资产  4低值易耗
        /// </summary>
        public string MedicalItemType { get; set; }
        public List<string> CenterId { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string SupplierId { get; set; }
        public string MedicalItemName { get; set; }
        public string OrderNo { get; set; }
        public string Id { get; set; }
        public string SupplierName { get; set; }

        public int? IsSettlement { get; set; }
    }

    public class SettlemenInPuts
    {
        /// <summary>
        /// 入库单ID
        /// </summary>
        public List<string> Id { get; set; }
        /// <summary>
        /// 结算/勾稽日期
        /// </summary>
        public DateTime? SettlementDate { get; set; }
    }

    //请求
    public class MaterialsInPut
    {
        /// <summary>
        /// 1药品 2 耗材  3 固定资产  4低值易耗
        /// </summary>
        public string MedicalItemType { get; set; }
        /// <summary>
        /// 透析中心ID
        /// </summary>
        public string CenterId { get; set; }
        /// <summary>
        /// 供货商
        /// </summary>
        public string SupplierId { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ValidData { get; set; }
    }
    public class MaterialsRequst
    {
        public string MedicalItemType { get; set; }
        public List<string> CenterId { get; set; }
        public int? Month { get; set; }
    }
    public class PrescriptionDetailModel
    {


        public string MaterialId { get; set; }
        public string OutboundId { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? SalePrice { get; set; }
        public string CenterId { get; set; }
        public string ReturnWarehouseId { get; set; }

        public int MedicalItemType { get; set; }
        public int? MayCharges { get; set; }
        public string WareHouseParentIds { get; set; }
    }
    /// <summary>
    /// 物品出入库汇总
    /// </summary>
    public class MaterialsConfluenceOutPut
    {
        public int SerialNumber { get; set; }
        public string DialysisName { get; set; }

        public string MedicalItemTypeName { get; set; }
        /// <summary>
        /// 业务大类
        /// </summary>
        public string ParentBusinessType { get; set; }
        /// <summary>
        /// 业务分类
        /// </summary>
        public string ChildBusinessType { get; set; }
        /// <summary>
        /// 成本金额
        /// </summary>
        public decimal? InPriceSum { get; set; }
        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal? SalePriceSum { get; set; }
        /// <summary>
        /// 差价
        /// </summary>
        public decimal? DifferencePrice { get; set; }

    }
    /// <summary>
    /// 进销存统计
    /// </summary>
    public class ItemsTalesAndInventorySummarySqlModel
    {
        public string NationItemCode { get; set; }
        public string ApprovalNum { get; set; }
        public string CatalogueName { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeName { get; set; }
        public string MedicalItemId { get; set; }
        public string MedicalItemName { get; set; }
        public string MedicalItemCode { get; set; }
        public bool? IsSplited { get; set; }
        public string Packaging { get; set; }
        public string GoodsName { get; set; }
        public string Specifications { get; set; }
        public string SpecificationsUnitName { get; set; }
        public string PackageUnitName { get; set; }
        public string Manufacturer { get; set; }

        public int? MedicalItemType { get; set; }
        /// <summary>
        /// 出入库数量
        /// </summary>
        public decimal? ItemQty { get; set; }

        public DateTime? putInstorageDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        /// <summary>
        /// 剩余库存
        /// </summary>
        public decimal? InQty { get; set; }

        //  public decimal? ReceivablePrice { get; set; }
        public int? SerialNumber { get; set; }


        public string OutInBoundDetailId { get; set; }
        public string OutboundNo { get; set; }

        public string UnitName { get; set; }
        public decimal? BeginCount { get; set; }
        public decimal? BeginCostPrice { get; set; }
        public decimal? BeginSalePrice { get; set; }
        public decimal? BeginDifference { get; set; }

        public decimal? PutInStorageCount { get; set; }
        public decimal? PutInStorageCostPrice { get; set; }
        public decimal? PutInStorageSalePrice { get; set; }
        public decimal? PutInStorageDifference { get; set; }

        public decimal? OutboundCount { get; set; }
        public decimal? OutboundCostPrice { get; set; }
        public decimal? OutboundSalePrice { get; set; }
        public decimal? OutboundDifference { get; set; }

        public decimal? SalesCount { get; set; }
        public decimal? SalesCostPrice { get; set; }
        public decimal? SalesSalePrice { get; set; }
        public decimal? SalesDifference { get; set; }

        public decimal? EndCount { get; set; }
        public decimal? EndCostPrice { get; set; }
        public decimal? EndSalePrice { get; set; }
        public decimal? EndDifference { get; set; }

        public string CenterId { get; set; }
        public string DialysisName { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

        public int? MayCharges { get; set; }

        public string WareHouseParentIds { get; set; }
    }

    /// <summary>
    /// 进销存统计
    /// </summary>
    public class ItemsTalesAndInventorySummaryModel
    {
        public string NationItemCode { get; set; }
        public string ApprovalNum { get; set; }
        public string SerialNumber { get; set; }
        public string MedicalItemName { get; set; }
        public string MedicalItemCode { get; set; }
        public string Specifications { get; set; }
        public string Manufacturer { get; set; }

        public string UnitName { get; set; }

        public decimal? InPrice { get; set; }


        public string ItemTypeName { get; set; }
        public decimal? BeginCount { get; set; }
        public decimal? BeginCostPrice { get; set; }
        public decimal? BeginSalePrice { get; set; }
        public decimal? BeginDifference { get; set; }

        public decimal? PutInStorageCount { get; set; }
        public decimal? PutInStorageCostPrice { get; set; }
        public decimal? PutInStorageSalePrice { get; set; }
        public decimal? PutInStorageDifference { get; set; }

        public decimal? CXPutInStorageCount { get; set; }
        public decimal? CXPutInStorageCostPrice { get; set; }
        public decimal? CXPutInStorageSalePrice { get; set; }
        public decimal? CXPutInStorageDifference { get; set; }

        public decimal? OutboundCount { get; set; }
        public decimal? OutboundCostPrice { get; set; }
        public decimal? OutboundSalePrice { get; set; }
        public decimal? OutboundDifference { get; set; }

        public decimal? ScrapCount { get; set; }
        public decimal? ScrapCostPrice { get; set; }//报废出库总额
        public decimal? ScrapSalePrice { get; set; }
        public decimal? ScrapDifference { get; set; }

        public decimal? ReceiveCount { get; set; }
        public decimal? ReceiveCostPrice { get; set; }//领用出库总额
        public decimal? ReceiveSalePrice { get; set; }
        public decimal? ReceiveDifference { get; set; }

        public decimal? CXReceiveCount { get; set; }
        public decimal? CXReceiveCostPrice { get; set; }//冲销领用出库总额
        public decimal? CXReceiveSalePrice { get; set; }
        public decimal? CXReceiveDifference { get; set; }

        public decimal? ReturnCount { get; set; }
        public decimal? ReturnCostPrice { get; set; }//退货出库总额
        public decimal? ReturnSalePrice { get; set; }
        public decimal? ReturnDifference { get; set; }

        public decimal? SalesCount { get; set; }
        public decimal? SalesCostPrice { get; set; }
        public decimal? SalesSalePrice { get; set; }
        public decimal? SalesDifference { get; set; }

        public decimal? RefundSalesCount { get; set; }
        public decimal? RefundSalesCostPrice { get; set; }
        public decimal? RefundSalesSalePrice { get; set; }
        public decimal? RefundSalesDifference { get; set; }

        public decimal? EndCount { get; set; }
        public decimal? EndCostPrice { get; set; }
        public decimal? EndSalePrice { get; set; }

        public decimal? EndDifference { get; set; }

        public string DialysisName { get; set; }
        public string SupplierName { get; set; }
    }


    /// <summary>
    /// 实际出入库模型
    /// </summary>
    public class ItemFortheModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 物品
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string ItemTypeName { get; set; }
        /// <summary>
        /// 期初
        /// </summary>
        public decimal? BeginCount { get; set; }
        /// <summary>
        /// 销售(实际出库)
        /// </summary>
        public decimal? SalesCount { get; set; }
        /// <summary>
        /// 期末
        /// </summary>
        public decimal? EndCount { get; set; }
        /// <summary>
        /// 实际入库
        /// </summary>
        public decimal? putInStorageCount { get; set; }
    }



    /// <summary>
    ///外购入库开单信息报表统计
    /// </summary>
    public class PutInStorageBillTjPageModel
    {
        public string Id { get; set; }
        public string InTypeId { get; set; }
        public string RowNumber { get; set; }
        public string PurchaseNo { get; set; }
        public string InStorageNo { get; set; }
        public string InTypeName { get; set; }
        public string GoodsNo { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? InPriceSum { get; set; }
        public decimal? SalePriceSum { get; set; }
        public decimal? Difference { get; set; }
        public string AuditPerson { get; set; }
        public string AuditConditionName { get; set; }
        public string AuditDate { get; set; }
        public string Advice { get; set; }
        public string Remark { get; set; }
        public string FounderName { get; set; }
        public string FounderDate { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }
        public int? IsSettlement { get; set; }
        public DateTime? SettlementDate { get; set; }

        public int? IsChecked { get; set; }
        public DateTime? CheckedDate { get; set; }
        public string SupplierId { get; set; }

        public string SupplierName { get; set; }
    }

    /// <summary>
    /// 外购入库明细统计
    /// </summary>
    public class PutInStorageBillDetailsTjTmpModel
    {
        public string CatalogueName { get; set; }
        public string InTypeId { get; set; }
        public string InTypeName { get; set; }
        public string RowNumber { get; set; }
        public string InStorageNo { get; set; }
        public string MedicalItemType { get; set; }
        public int? Type { get; set; }
        public string MedicalItemCode { get; set; }
        public string MedicalItemName { get; set; }
        public string BatchNo { get; set; }
        public string SupplierName { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        // public decimal? ActualQty { get; set; }
        public decimal? InQty { get; set; }
        public string Specifications { get; set; }
        public string ProcurementPackage { get; set; }
        public string UnitName { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string DialysisName { get; set; }
        public decimal? CostAmount { get; set; }
        public decimal? SalesAmount { get; set; }
        public string AuditPerson { get; set; }
        public string AuditDate { get; set; }
        public string Brand { get; set; }
        public int? IsSettlement { get; set; }

        public string ApprovalNum { get; set; }

        public string Manufacturer { get; set; }

    }
    /// <summary>
    ///  其它出库统计、退货、报废
    /// </summary>
    public class OtherOutboundModel
    {
        public string Id { get; set; }
        public string SerialNumber { get; set; }
        public string OutboundNo { get; set; }
        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }

        public decimal? DifferenceAmount { get; set; }

        public string Name { get; set; }
        public string FounderDate { get; set; }
        public string AuditorName { get; set; }
        public string AuditDate { get; set; }
        public string Remark { get; set; }
        public decimal? InPrice { get; set; }

        public decimal? SalePrice { get; set; }
        public string OutboundType { get; set; }
        public decimal? ItemQty { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }
        public string WareHouseName { get; set; }

        public string SupplierName { get; set; }
        public int? IsSettlement { get; set; }
        public int? IsChecked { get; set; }
        public DateTime? CheckedDate { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string ReturnAuditor { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    /// <summary>
    ///  其它出库明细统计、退货、报废
    /// </summary>
    public class OtherOutboundDetailModel
    {
        public string SerialNumber { get; set; }
        public string OutboundNo { get; set; }
        public string MedicalItemName { get; set; }
        public string Specifications { get; set; }
        public string Manufacturer { get; set; }
        public string BatchNo { get; set; }
        public string QualityDate { get; set; }
        public decimal? ItemQty { get; set; }

        public string UnitName { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }
        public decimal? DifferenceAmount { get; set; }

        public string MedicalItemCode { get; set; }
        public string MedicalItemWorkCode { get; set; }
        public string Brand { get; set; }
        public bool IsSplited { get; set; }
        public string Packaging { get; set; }

        public string SpecificationsUnitName { get; set; }
        public string PackageUnitName { get; set; }
        public int? MedicalItemType { get; set; }
        public string DialysisName { get; set; }
        public string ItemTypeName { get; set; }
        public string OutboundType { get; set; }
        public string GoodsName { get; set; }
        public string SupplierName { get; set; }
        public string WareHouseName { get; set; }
    }

    /// <summary>
    /// 领用出库统计
    /// </summary>
    public class AccuratelyRecipientsModel
    {
        public string Id { get; set; }
        public string OutboundNo { get; set; }
        public DateTime? OutboundDate { get; set; }
        public string Name { get; set; }
        public decimal? OutInBoundQty { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? InPrice { get; set; }
        public DateTime? ChargeDate { get; set; }
        public int? MedicalItemType { get; set; }

        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }

        public string OutboundType { get; set; }

        public DateTime? FounderDate { get; set; }
        public string AuditorName { get; set; }

        public DateTime? AuditDate { get; set; }

        public string Remark { get; set; }
        public string SerialNumber { get; set; }


        public decimal? DifferenceAmount { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }


    }


    /// <summary>
    /// 领用出库明细统计
    /// </summary>
    public class AccuratelyRecipientsDetailModel
    {
        public string Name { get; set; }
        public string OutboundNo { get; set; }
        public string ApprovalNum { get; set; }
        public string CatalogueName { get; set; }

        public string MedicalItemCode { get; set; }
        public string MedicalItemName { get; set; }
        public string GoodsName { get; set; }

        public string Specifications { get; set; }

        public bool IsSplited { get; set; }
        public string Packaging { get; set; }

        public string SpecificationsUnitName { get; set; }
        public string PackageUnitName { get; set; }
        public string Manufacturer { get; set; }
        public string SupplierName { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ProductionDate { get; set; }

        public DateTime? QualityDate { get; set; }

        public decimal? SalePrice { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? OutInBoundQty { get; set; }

        public DateTime? OutboundDate { get; set; }

        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }

        public string OutboundType { get; set; }
        public int? MedicalItemType { get; set; }

        public string MedicalItemWorkCode { get; set; }

        public decimal? MaterialQuantity { get; set; }
        public DateTime? FounderDate { get; set; }
        public string AuditorName { get; set; }
        public DateTime? AuditDate { get; set; }
        public string SerialNumber { get; set; }
        public string UnitName { get; set; }
        public decimal? DifferenceAmount { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }

    }

    /// <summary>
    /// 销售出库统计
    /// </summary>
    public class AccuratelyOutboundSqlModel
    {
        public string Id { get; set; }
        public string OutboundNo { get; set; }
        public DateTime? OutboundDate { get; set; }
        public string PatientName { get; set; }
        public decimal? OutInBoundQty { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? InPrice { get; set; }
        public string ChargeDate { get; set; }
        public int? MedicalItemType { get; set; }

        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }

        public string OutboundType { get; set; }
        public decimal? ReturnWarehouseQty { get; set; }

        public decimal? ReceivablePrice { get; set; }

        public string OutInBoundDetailId { get; set; }
        public string OutInBoundType { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }
    }
    /// <summary>
    ///  销售出库统计
    /// </summary>
    public class AccuratelyOutboundModel
    {
        public string Id { get; set; }
        public string SerialNumber { get; set; }
        // public string PatientName { get; set; }
        public string OutboundNo { get; set; }
        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }

        public string OutboundType { get; set; }
        //  public string ChargeDate { get; set; }
        public string OutboundDate { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }
    }

    /// <summary>
    /// 销售出库明细统计
    /// </summary>
    public class AccuratelyOutboundDetailModel
    {
        public string MaterialId { get; set; }
        public string CatalogueName { get; set; }
        public string ApprovalNum { get; set; }
        public decimal? Ybscl { get; set; }
        public string PrescriptionNo { get; set; }
        public string PatientName { get; set; }
        public string OutboundNo { get; set; }

        public string MedicalItemCode { get; set; }
        public string MedicalItemName { get; set; }
        public string GoodsName { get; set; }

        public string Specifications { get; set; }

        public bool IsSplited { get; set; }
        public string Packaging { get; set; }

        public string SpecificationsUnitName { get; set; }
        public string PackageUnitName { get; set; }
        public string Manufacturer { get; set; }
        public string SupplierName { get; set; }
        public string BatchNo { get; set; }
        public string ProductionDate { get; set; }

        public string QualityDate { get; set; }

        public decimal? UnitPrice { get; set; }
        public decimal? OutInBoundQty { get; set; }

        public DateTime? OutboundDate { get; set; }

        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }

        public string OutboundType { get; set; }
        public int? MedicalItemType { get; set; }
        public decimal? ReturnWarehouseQty { get; set; }

        public string Id { get; set; }
        public decimal? ReceivablePrice { get; set; }

        public decimal? MaterialTotal { get; set; }

        public string SerialNumber { get; set; }

        public string UnitName { get; set; }

        public decimal? SalePrice { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }
        public decimal? SalePrices { get; set; }
    }

    public class PriceModel
    {
        public string Id { get; set; }
        public string WarehouseId { get; set; }
        public string BatchNo { get; set; }
        public string MedicalItemId { get; set; }
        public string SupplierId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? InQty { get; set; }
        public decimal? TakeInventory { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public string FounderDate { get; set; }
        public string Modifier { get; set; }
        public string ModifierDate { get; set; }
        public int? DataState { get; set; }

        public string OutInBoundDetailId { get; set; }

        public string SupplierName { get; set; }
        public string CenterId { get; set; }
        public string DialysisName { get; set; }

        public decimal? OutInBoundQty { get; set; }
        public string QualityDateStr { get; set; }
        public string ProductionDateStr { get; set; }
    }

    /// <summary>
    /// 近效期物品
    /// </summary>
    public class RecentValiditymModel
    {

        public string SerialNumber { get; set; }
        public string MedicalItemName { get; set; }
        public string GoodsName { get; set; }

        public string Specifications { get; set; }

        public bool IsSplited { get; set; }
        public string Packaging { get; set; }

        public string SpecificationsUnitName { get; set; }
        public string PackageUnitName { get; set; }
        public string Manufacturer { get; set; }

        public int? MedicalItemType { get; set; }

        public string Id { get; set; }

        public string ItemTypeName { get; set; }
        /// <summary>
        /// 警戒库存
        /// </summary>
        public decimal? MinInventory { get; set; }
        public string DialysisName { get; set; }

        public string Brand { get; set; }
        public string BatchNo { get; set; }

        public DateTime? QualityDate { get; set; }


        public decimal? InQty { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }

        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public string UnitName { get; set; }
        public string CenterId { get; set; }

        public decimal? CostAmount { get; set; }

        public decimal? SalesAmount { get; set; }


    }
    #endregion
    #region  物资统计
    /// <summary>
    /// 物品警戒库存
    /// </summary>
    public class LowInventoryWarningModel
    {

        public string MedicalItemName { get; set; }
        public string GoodsName { get; set; }

        public string Specifications { get; set; }

        public bool IsSplited { get; set; }
        public string Packaging { get; set; }

        public string SpecificationsUnitName { get; set; }
        public string PackageUnitName { get; set; }
        public string Manufacturer { get; set; }

        public int? MedicalItemType { get; set; }

        public string Id { get; set; }

        public string ItemTypeName { get; set; }
        /// <summary>
        /// 警戒库存
        /// </summary>
        public decimal? MinInventory { get; set; }
        /// <summary>
        /// 剩余库存
        /// </summary>
        public decimal? Inventory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? InQty { get; set; }
        public string DialysisName { get; set; }
        public string UnitName { get; set; }
        public string CenterId { get; set; }
        public string MedicalItemId { get; set; }
    }

    /// <summary>
    /// 供应商入库统计
    /// </summary>
    public class SupplierPutInStorageModel
    {

        public string MedicalItemName { get; set; }
        public string GoodsName { get; set; }

        public string UnitName { get; set; }

        public string ProcurementPackage { get; set; }
        public string Manufacturer { get; set; }

        public int? MedicalItemType { get; set; }

        public string Id { get; set; }

        public string ItemTypeName { get; set; }
        public string DialysisName { get; set; }

        public string Brand { get; set; }
        //public string BatchNo { get; set; }

        public decimal? InQty { get; set; }
        //public decimal? InPrice { get; set; }
        //public decimal? SalePrice { get; set; }

        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public string CenterId { get; set; }

    }
    #endregion

    /// <summary>
    /// 物品库存信息
    /// </summary>
    public class GoodsInStockdModel
    {
        public string Id { get; set; }
        public string MedicalItemId { get; set; }
        public string MedicalItemName { get; set; }
        public string HiCenterCode { get; set; }
        public string MedicalItemCode { get; set; }
        public int? MedicalItemType { get; set; }
        public string Packaging { get; set; }
        public string PackageUnit { get; set; }
        public string PackageUnitName { get; set; }
        public string Specifications { get; set; }
        public string SpecificationsUnit { get; set; }
        public string SpecificationsUnitName { get; set; }
        public string BatchNo { get; set; }
        public decimal? TakeInventory { get; set; }
        public decimal? InQty { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string YPGG { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? QualityDate { get; set; }
        public string Manufacturer { get; set; }
        public string SupplierName { get; set; }
        /// 根据是否可拆分展示包装单位或规格单位
        /// </summary>
        public string MedicalItemSpecifications { get; set; }
        public string MedicalItemUnit { get; set; }
        public string MedicalItemUnitName { get; set; }
        /// <summary>
        /// 是否警示
        /// </summary>
        public bool? IsWarning { get; set; }
        public int? MinInventory { get; set; }
        public bool? IsSplited { get; set; }
        public string SupplierId { get; set; }
        public string UnitName { get; set; }
        public string GoodsName { get; set; }
        public string Brand { get; set; }
        public string ItemTypeName { get; set; }

        public decimal? TotalInPrice { get; set; }
        public decimal? TotalSalePrice { get; set; }
        public decimal? TotalInQty { get; set; }

        public decimal? MonthInQty { get; set; }
        public string CenterId { get; set; }
        public string DialysisName { get; set; }

        /// <summary>
        /// 社保指导价
        /// </summary>
        public decimal? SocialSecurityPrice { get; set; }
        /// <summary>
        /// 采购金额
        /// </summary>
        public decimal? PurchaseAmount { get; set; }
        public string ItemType { get; set; }
        public decimal? ItemQty { get; set; }
        public string OutboundType { get; set; }

        public decimal? DualPrice { get; set; }


        public decimal? MonthAverage { get; set; }
        public DateTime outDate { get; set; }
  public string outDateMonty { get; set; }
        public string outDateValue { get; set; }
    }

    /// <summary>
    /// 月均用量
    /// </summary>
    public class MonthGoodsInStockdModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 规格单位
        /// </summary>
        public string SpecificationsUnitName { get; set; }
        /// <summary>
        /// 月均成本总额
        /// </summary>
        public decimal? MonthInMoney { get; set; }

        public decimal? InPrice { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string ItemTypeName { get; set; }

        /// <summary>
        /// 总量
        /// </summary>
        public decimal? TotalInQty { get; set; }
        /// <summary>
        /// 月均用量
        /// </summary>
        public int? MonthInQty { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CenterId { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string DialysisName { get; set; }
        /// <summary>
        /// 类型ID
        /// </summary>
        public string ItemType { get; set; }
    }

    /// <summary>
    /// 成本总汇
    /// </summary>
    public class CostModel
    {
        public string title { get; set; }
    }
    /// <summary>
    /// 台账
    /// </summary>
    public class ItemStandingBookModel
    {
        public string Id { get; set; }
        public string SerialNumber { get; set; }
        public string MedicalItemName { get; set; }
        public string Specifications { get; set; }
        public string Packaging { get; set; }
        public string Manufacturer { get; set; }

        public string UnitName { get; set; }

        public string ItemTypeName { get; set; }
        public string RuChuKuNo { get; set; }
        public DateTime? putInstorageDate { get; set; }
        public string BatchNo { get; set; }
        public string Remark { get; set; }
        public decimal? InQty { get; set; }
        public decimal? InPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string OutInBoundDetailId { get; set; }

        public string MedicalItemId { get; set; }
        public string ItemType { get; set; }
        public decimal? ItemQty { get; set; }

        public decimal? PutInStorageCount { get; set; }
        public decimal? PutInStorageCost { get; set; }
        public decimal? PutInStorageSale { get; set; }
        public decimal? PutInStorageCostPrice { get; set; }
        public decimal? PutInStorageSalePrice { get; set; }
        public decimal? PutInStorageDifference { get; set; }


        public decimal? OutboundCount { get; set; }
        public decimal? OutboundCost { get; set; }
        public decimal? OutboundSale { get; set; }
        public decimal? OutboundCostPrice { get; set; }
        public decimal? OutboundSalePrice { get; set; }
        public decimal? OutboundDifference { get; set; }

        public decimal? EndCount { get; set; }
        public decimal? EndCostPrice { get; set; }
        public decimal? EndSalePrice { get; set; }

        public decimal? EndDifference { get; set; }

        public bool IsSplited { get; set; }
        public string SpecificationsUnitName { get; set; }
        public string PackageUnitName { get; set; }
        public string DialysisName { get; set; }
        public string CenterId { get; set; }
    }


    public class SI_CFMXOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 医保类型
        /// </summary>
        public string HealthCareType { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? ZJE { get; set; }
        /// <summary>
        /// 统筹支付
        /// </summary>
        public decimal? TCZF { get; set; }
        /// <summary>
        /// 民政救助
        /// </summary>
        public decimal? MZJZ { get; set; }
        /// <summary>
        /// 医保账户支付
        /// </summary>
        public decimal? ZHZF { get; set; }
        /// <summary>
        /// 应收现金
        /// </summary>
        public decimal? YSXJ { get; set; }
        /// <summary>
        /// 实收现金
        /// </summary>
        public decimal? SSXJ { get; set; }
        /// <summary>
        /// 大额理赔
        /// </summary>
        public decimal? DELP { get; set; }
        /// <summary>
        /// 公务员补助
        /// </summary>
        public decimal? GWYBZ { get; set; }
        /// <summary>
        /// 医疗机构垫支
        /// </summary>
        public decimal? YLJGDZ { get; set; }
        /// <summary>
        /// 医保实结差（超标扣款）
        /// </summary>
        public decimal? CBKK { get; set; }
        /// <summary>
        /// 现金实收差额
        /// </summary>
        public decimal? XJSSC { get; set; }

        public string JSJYLSH { get; set; }

        public DateTime? JSRQ { get; set; }

        public string BalanceNo { get; set; }
    }


    /// <summary>
    /// 报表导出通用条件
    /// </summary>
    public class exportExcleInPut
    {
        public List<string> CenterId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 结算方式 1 收费项目 2 结算方式 3 回款方式
        /// </summary>
        public int Model { get; set; }


    }
    public class ALLChargeCasesOutPut
    {
        public List<TableHeader> tableHeaders { get; set; }
        public DataTable SiRatioOutPuts { get; set; }
    }

    public class ChargeCasesModel
    {

        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public string DeteMonth { get; set; }
        public decimal SumPrice { get; set; }
        public int HdCount { get; set; }
        public string CenterId { get; set; }
        public string ShortName { get; set; }
        public string PatientAge { get; set; }

        public string ActualDialysisType { get; set; }

        public string SIInsuredType { get; set; }

    }

    /// <summary>
    /// 医保渝快保收费统计
    /// </summary>
    public class ChargeHifesPayOutPut
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNo { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// 就诊流水号
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 医保结算流水号
        /// </summary>
        public string JSJYLSH { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string XM { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string CardNum { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime JSRQ { get; set; }
        /// <summary>
        /// 确诊疾病
        /// </summary>
        public string RYZDMC { get; set; }
        /// <summary>
        /// 医疗费总金额
        /// </summary>
        public decimal? ZJE { get; set; }
        /// <summary>
        /// 渝快保赔付金额
        /// </summary>
        public decimal? hifes_pay { get; set; }

        public string StrJSRQ { get; set; }
        public string StrCYRQ { get; set; }
    }

    public class TempZYMZHModel
    {
        public string ZYMZH { get; set; }
        public string TFBZ { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// 物品医保上传量
    /// </summary>
    public class SIItemUploadInfoModel
    {
        /// <summary>
        /// 项目类别
        /// </summary>
        public string ItemType { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string XMMC { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 医保编码
        /// </summary>
        public string YBLSH { get; set; }
        /// <summary>
        /// 医院项目编码
        /// </summary>
        public string YYNM { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }

        public string strHiLevel { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal TotalCount { get; set; }
    }

    public class FeeDetailsModel
    {
        public int Id { get; set; }
        public string CardNum { get; set; }
        /// <summary>
        /// 处方单号
        /// </summary>
        public string RecipelNo { get; set; }
        /// <summary>
        /// 结算类别
        /// </summary>
        public string BalanceType { get; set; }
        /// <summary>
        /// 结算单号
        /// </summary>
        public string BalanceNo { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }
        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 销售总价
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime? BalanceDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public int? BalanceState { get; set; }
        public int? CancelFlag { get; set; }
        public string DetailsId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PatientNo { get; set; }
        public decimal PreRefundQty { get; set; }
        public decimal RefundQty { get; set; }
        public decimal RefundMoney { get; set; }
        public string ReBalanceNo { get; set; }
        public string CategoryName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 项目开立时间
        /// </summary>
        public DateTime? PrescriptionDetailFounderDate { get; set; }
        public int? ZFUpLoad { get; set; }
        public string CompareCode { get; set; }
        public string med_chrgitm_type { get; set; }
        public string OpenDeptId { get; set; }
        public string OpenDeptName { get; set; }
        public string OpenPersonId { get; set; }
        public string OpenPersonName { get; set; }
        /// <summary>
        /// 透析模式
        /// </summary>
        public string DialysisType { get; set; }

        /// <summary>
        /// 例次
        /// </summary>
        public string DialysisQty { get; set; }
        public string Remark { get; set; }
    }
}
