using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 费用清单 
    /// </summary>
    public class BalanceMain
    {
        public string Id { get; set; }
        public string BalanceNo { get; set; }
       
        public decimal? SumPrice { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatePerson { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatePerson { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelPerson { get; set; }
        /// <summary>
        /// 结算状态 1全部结算 2完全退费 3部分结算 4部分退费
        /// </summary>
        public int? BalanceState { get; set; }
        public int? CancelFlag { get; set; }
        public DateTime? BalanceDate { get; set; }
        public string BalancePerson { get; set; }
        /// <summary>
        /// 费用结算类型 1医保结算 2非医保结算
        /// </summary>
        public int? BalanceType { get; set; }
        public string RecipelNos { get; set; }
        public string SourceBalanceNo { get; set; }
        /// <summary>
        /// 医保的结算流水号，非医保结算的为空，医保结算不允许为空
        /// </summary>
        public string SIBalanceSerialNo { get; set; }
        /// <summary>
        /// 医保结算原单号，冲销退费时有用
        /// </summary>
        public string SISourceBalanceNo { get; set; }
        public string PatientNo { get; set; }
        /// <summary>
        /// 0代表未日结 1代表已日结
        /// </summary>
        public int? IsDayBalanced { get; set; }
        public DateTime? DayBalanceDate { get; set; }
        public string DayBalanceNo { get; set; }
        public int IsCancelDayBalanced { get; set; }
        public DateTime? CancelDayBalanceDate { get; set; }
        public string CancelDayBalanceNo { get; set; }
        public string BalanaceTypeRemark { get; set; }
        public string CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual CenterDialysis center { get; set; }
        public DateTime? CollectData { get; set; }


    }
    /// <summary>
    /// 费用明细
    /// </summary>
    public class BalanceDetails
    {
        public string Id { get; set; }
        public string CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual CenterDialysis center { get; set; }
        public string RecipelNo { get; set; } 
        public string BalanceNo { get; set; }
        public string ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual MedicalItemRecord MedicalItemRecord { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string DoseInfo { get; set; }
        /// <summary>
        /// 用药途径名称
        /// </summary>
        public string UsageName { get; set; }
        public string UsageId { get; set; }
        /// <summary>
        /// 使用频次名称
        /// </summary>
        public string FrequencyName { get; set; }
        public string FrequencyId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string CompareCode { get; set; }
        public string Specifications { get; set; }
        public int? DataState { get; set; }
        public string Form { get; set; }
        public string FormName { get; set; }
        public string Remark { get; set; }
        public string OtherInfo { get; set; }
        /// <summary>
        /// 最小计量单位
        /// </summary>
        public string Unit { get; set; }
        public string UnitName { get; set; }
        public string DetailsId { get; set; }
        public int? RecipelType { get; set; }
        /// <summary>
        /// 开立人
        /// </summary>
        public string OpenPersonName { get; set; }
        public string OpenPersonId { get; set; }
        /// <summary>
        /// 开立科室
        /// </summary>
        public string OpenDeptName { get; set; }
        public string OpenDeptId { get; set; }
        /// <summary>
        /// 剂量单位名称
        /// </summary>
        public string DoseUnitName { get; set; }
        public string DoseUnitId { get; set; }
        /// <summary>
        /// 用药天数
        /// </summary>
        public string UsageDays { get; set; }
        /// <summary>
        /// 用药周期
        /// </summary>
        public string UsagePeriod { get; set; }
        public DateTime? OpenDateTime { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatePerson { get; set; }
        public decimal? DisplayPrice { get; set; }

        public DateTime? CollectData { get; set; }


    }
    /// <summary>
    /// 费用结算支付方式
    /// </summary>
    public class BalancePayType
    {
        public string Id { get; set; }
        public string BalanceNo { get; set; }
        /// <summary>
        /// 支付方式 1现金 2医保账户 3医保统筹合计 4支付宝 5微信 6银联卡
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal? CostMoney { get; set; }
        /// <summary>
        /// 四舍五入差额
        /// </summary>
        public decimal? DifferenceMoney { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? ReceivableMoney { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatePerson { get; set; }
        /// <summary>
        /// 支付方式说明
        /// </summary>
        public string PayTypeName { get; set; }
        public int? DataState { get; set; }

        public string CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual CenterDialysis centerDialysis { get; set; }
        public DateTime? CollectData { get; set; }
    }

    public class DayBalanceDetail
    {
        public string Id { get; set; }
        public string DayBalanceNo { get; set; }
        public string CategoryId { get; set; }
        public string ItemName { get; set; }
        public string ItemId { get; set; }
        public decimal? Money { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
    public class DayBalanceMain
    {
        public string Id { get; set; }
        public string DayBalanceNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatePersonName { get; set; }
        public string CreatePersonId { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
    public class SI_CFMX
    {
        public string Id { get; set; }
        public DateTime KFRQ { get; set; }
        public string YYNM { get; set; }
        public string YBLSH { get; set; }
        public string XMMC { get; set; }
        public decimal? DJ { get; set; }
        public decimal? SL { get; set; }
        public decimal? JE { get; set; }
        public string FYDJ { get; set; }
        public decimal? ZFBL { get; set; }
        public decimal? BZDJ { get; set; }
        public decimal? ZFJE { get; set; }
        public decimal? CBJE { get; set; }
        public string TYBZ { get; set; }
        public string JSBZ { get; set; }
        public DateTime? JSRQ { get; set; }
        public string JSJYLSH { get; set; }
        public string ZYMZH { get; set; }
        public string CFH { get; set; }
        public string CFJYLSH { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatePerson { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
        [ForeignKey("YYNM")]
        public virtual MedicalItemRecord MedicalItem { get; set; }
    }
    public class SI_JSMX
    {
        public DateTime? BalanceDate { get; set; }
        public string Id { get; set; }
        public string CenterId { get; set; }
        public string XM { get; set; }
        public string ZYMZH { get; set; }
        public string SHBZH { get; set; }
        public string YLLB { get; set; }
        public DateTime? JSRQ { get; set; }
        public string JSJYLSH { get; set; }
        public decimal? XJZF { get; set; }
        public decimal? BCFHYBFWFY { get; set; }
        public decimal? ZJE { get; set; }
        public string QH { get; set; }
        public string CBLB { get; set; }
        /// <summary>
        /// 退费标志 1正常结算 -1已退费
        /// </summary>
        public string TFBZ { get; set; }
        public decimal? TCZF { get; set; }
        public decimal? ZHZF { get; set; }
        public decimal? GWYBZ { get; set; }
        public decimal? GWYFH { get; set; }
        public decimal? DELP { get; set; }
        public string XZLB { get; set; }
        public decimal? MZJZ { get; set; }
        public decimal? YBZLZFS { get; set; }
        public decimal? DBZDDYLJGDZ { get; set; }
        public decimal? CBKK { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatePerson { get; set; }
        public string IsFPRY { get; set; }
        public decimal? JKFPYLJJ { get; set; }
        public decimal? JZTPBXJE { get; set; }
        public DateTime? CollectData { get; set; }
        public string medins_setl_id { get; set; }
        public decimal? hifes_pay { get; set; }
        public decimal? oth_pay { get; set; }
    }
    public class SI_JZDJMX
    {
        public string Id { get; set; }
        public string ZYMZH { get; set; }
        public string SHBZH { get; set; }
        public string DJLSH { get; set; }
        public string YLLB { get; set; }
        public DateTime RYRQ { get; set; }
        public string RYZD { get; set; }
        public string JBR { get; set; }
        public DateTime JBRQ { get; set; }
        public string KS { get; set; }
        public string YS { get; set; }
        public DateTime? CYRQ { get; set; }
        public string QZJB { get; set; }
        public string XZLB { get; set; }
        public string CBLB { get; set; }
        public string PatientNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatePerson { get; set; }
        /// <summary>
        /// 主诉
        /// </summary>
        public string ZS { get; set; }
        /// <summary>
        /// 症状描述
        /// </summary>
        public string ZZMS { get; set; }
        public string RYZDMC { get; set; }
        public string BFZ { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }
    public class SICompare
    {
        public string Id { get; set; }
        public string MedicalItemCode { get; set; }
        public string MedicalItemName { get; set; }
        public string CenterItemCode { get; set; }
        public string CenterItemChineseName { get; set; }
        public string CenterItemEnglishName { get; set; }
        public string CenterItemClass { get; set; }
        public decimal? OwnPayRate { get; set; }
        public decimal? CenterItemPrice { get; set; }
        public decimal? MedicalItemPrice { get; set; }
        public string CenterItemSpecifications { get; set; }
        public string MedicalItemSpecifications { get; set; }
        public string CenterItemManufacturer { get; set; }
        public string MedicalItemManufacturer { get; set; }
        public string CenterItemForm { get; set; }
        public string MedicalItemForm { get; set; }
        public string CenterItemMnemonic { get; set; }
        public string MedicalItemMnemonic { get; set; }
        public string CenterItemFeeTypeCode { get; set; }
        public string LimitContent { get; set; }
        public string Operator { get; set; }
        public DateTime? OperateDate { get; set; }
        public string ItemTypeCode { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }

}
