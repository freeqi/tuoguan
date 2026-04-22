using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas 
{



    #region 【5205】人员慢特病用药记录查询

    public class ChronicSpecialDiseaseDrugRecordModel
    {
        public string psn_no { get; set; }// 人员编号 字符型 30 Y 
        public DateTime begntime { get; set; }// 开始时间 日期时间型 Y yyyy-MM-dd HH:mm:ss
        public DateTime endtime { get; set; }//结束时间 日期时间型 yyyy-MM-dd HH:mm:ss

        public string Id { get; set; }
    }
    public class ChronicSpecialDiseaseDrugRecordResponseModel
    {
        public List<ChronicSpecialDiseaseDrugRecordResultModel> feedetail { get; set; }
    }
    public class ChronicSpecialDiseaseDrugRecordResultModel
    {
        public string feedetl_sn { get; set; }//费用明细流水号字符型 30 Y 
        public string rx_drord_no { get; set; }//处方/医嘱号 字符型 30
        public string fixmedins_code { get; set; }//定点医药机构编号字符型 12 
        public string fixmedins_name { get; set; }//定点医药机构名称字符型 200 Y 
        public string psn_no { get; set; }//人员编号 字符型 30 Y 
        public string med_type { get; set; }//医疗类别 字符型 6 Y Y 
        public DateTime fee_ocur_time { get; set; }//费用发生时间日期时间型Yyyyy-MM-ddHH:mm:ss
        public decimal cnt { get; set; }//数量 数值型 16,4 Y 
        public decimal pric { get; set; }//单价 数值型 16,6 Y
        public string chrgitm_lv { get; set; }//收费项目等级 字符型 3 Y
        public string hilist_code { get; set; }//医保目录编码 字符型 50 Y
        public string hilist_name { get; set; }//医保目录名称 字符型 200 Y
        public string list_type { get; set; }//目录类别 字符型 6 Y Y
        public string med_list_codg { get; set; }//医疗目录编码 字符型 50 Y
        public string medins_list_codg { get; set; }//医药机构目录编码字符型 150 Y
        public string medins_list_name { get; set; }//医药机构目录名称字符型 100 Y
        public string med_chrgitm_type { get; set; }//医疗收费项目类别字符型 6 Y Y
        public string prodname { get; set; }//商品名 字符型 200
        public string spec { get; set; }//规格 字符型 200 Y
        public string dosform_name { get; set; }//剂型名称 字符型 200
        public string lmt_used_flag { get; set; }//限制使用标志 字符型 3 Y
        public string hosp_prep_flag { get; set; }//医院制剂标志 字符型 3 Y
        public string hosp_appr_flag { get; set; }//医院审批标志 字符型 3 Y
        public string tcmdrug_used_way { get; set; }//中药使用方式 字符型 6 Y
        public string prodplac_type { get; set; }//生产地类别 字符型 6 Y
        public string bas_medn_flag { get; set; }//基本药物标志 字符型 3 Y
        public string hi_nego_drug_flag { get; set; }//医保谈判药品标志字符型 3 Y
        public string chld_medc_flag { get; set; }//儿童用药标志 字符型 3 Y
        public string etip_flag { get; set; }//外检标志 字符型 3 Y
        public string etip_hosp_code { get; set; }//外检医院编码 字符型 30
        public string dscg_tkdrug_flag { get; set; }//出院带药标志 字符型 3 Y
        public string list_sp_item_flag { get; set; }//目录特项标志 字符型 3 Y特检特治项目或特殊药品
        public string matn_fee_flag { get; set; }//生育费用标志 字符型 6 Y
    }
    #endregion

    /// <summary>
    /// 药品
    /// </summary>
    public class SI_YP
    {
        public string YLMLBM { get; set; }
        public string YPSPM { get; set; }
        public string TYMBH { get; set; }
        public string YPTYM { get; set; }
        public string HXMC { get; set; }
        public string BM { get; set; }
        public string YWMC { get; set; }
        public string ZCMC { get; set; }
        public string YJBWM { get; set; }
        public string YPJX { get; set; }
        public string YPJXMC { get; set; }
        public string YPLB { get; set; }
        public string YPLBMC { get; set; }
        public string YPGG { get; set; }
        public string YPGGDM { get; set; }
        public string ZZJX { get; set; }
        public string ZZGG { get; set; }
        public string ZZGGDM { get; set; }
        public string MCYL { get; set; }
        public string SYPC { get; set; }
        public string GJYPBH { get; set; }
        public string ZCYBZ { get; set; }
        public string FCFYBZ { get; set; }
        public string FCFYBZMC { get; set; }
        public string BZGG { get; set; }
        public string BZSL { get; set; }
        public Nullable<System.DateTime> KSRQ { get; set; }
        public Nullable<System.DateTime> JSRQ { get; set; }
        public string ZXBZSL { get; set; }
        public string ZXBZDW { get; set; }
        public string ZXBZDWMC { get; set; }
        public string ZXZJDWMC { get; set; }
        public string TSXJYPBZ { get; set; }
        public string TSYPBZ { get; set; }
        public string YPZCZH { get; set; }
        public string PZWH { get; set; }
        public string BZ { get; set; }
        public string WYJLH { get; set; }
        public Nullable<System.DateTime> CJSJ { get; set; }
        public Nullable<System.DateTime> GXSJ { get; set; }
        public string BBH { get; set; }
        public string BBHMC { get; set; }
        public string GSMC { get; set; }
        public string GJYBYPMLJX { get; set; }
        public string YBDJ { get; set; }
        public string XZSYFW { get; set; }
        public string XZSYBZ { get; set; }
        public string ZXJJDW { get; set; }
        public string ZXZJDW { get; set; }
        public string ZXJLDW { get; set; }
        public string ZXSYDW { get; set; }
        public string ZXXSDW { get; set; }
        public string SCQYMC { get; set; }
        public string SCQYBH { get; set; }
        public string ZHB { get; set; }
        public string YXBZ { get; set; }
        public Nullable<int> YPType { get; set; }
    }

    /// <summary>
    /// 耗材
    /// </summary>
    public   class SI_HC
    {
        public string YLMLBM { get; set; }
        public string HCMC { get; set; }
        public string WYBSM { get; set; }
        public string YBTYMDM { get; set; }
        public string YYTYM { get; set; }
        public string CPXH { get; set; }
        public string GGDM { get; set; }
        public string GG { get; set; }
        public string HCFL { get; set; }
        public string GGXH { get; set; }
        public string BZGG { get; set; }
        public string BZSL { get; set; }
        public string BZDW { get; set; }
        public string CCZHB { get; set; }
        public string YLQXGLLB { get; set; }
        public string YLQXGLLBMC { get; set; }
        public string ZCBAH { get; set; }
        public string ZCBACPMC { get; set; }
        public string PZRQ { get; set; }
        public string SCQYMC { get; set; }
        public string YXBS { get; set; }
        public string WYJLH { get; set; }
        public string BBH { get; set; }
        public string BBMC { get; set; }
        public Nullable<System.DateTime> KSRQ { get; set; }
        public Nullable<System.DateTime> JSRQ { get; set; }
    }

    /// <summary>
    /// 服务项目
    /// </summary>
    public   class SI_FWXM
    {
        public string LYMLBM { get; set; }
        public string JJDW { get; set; }
        public string JJDWMC { get; set; }
        public string ZLXMSM { get; set; }
        public string ZLCWNR { get; set; }
        public string ZLXMLH { get; set; }
        public string YXBZ { get; set; }
        public string BZ { get; set; }
        public string FWXMLB { get; set; }
        public string YLFWXMMC { get; set; }
        public string XMSM { get; set; }
        public Nullable<System.DateTime> KSRQ { get; set; }
        public Nullable<System.DateTime> JSRQ { get; set; }
        public string WYJLH { get; set; }
        public string BBH { get; set; }
        public string BBMC { get; set; }
    }

    /// <summary>
    /// 支付比例
    /// </summary>
    public   class SI_OwnExpenseInfo
    {
        public int ID { get; set; }
        public string hilist_code { get; set; }
        public string selfpay_prop_psn_type { get; set; }
        public string selfpay_prop_type { get; set; }
        public string insu_admdvs { get; set; }
        public Nullable<System.DateTime> begndate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<decimal> selfpay_prop { get; set; }
        public string vali_flag { get; set; }
        public string rid { get; set; }
        public Nullable<System.DateTime> updt_time { get; set; }
        public string crter_id { get; set; }
    }

    /// <summary>
    /// 限价
    /// </summary>
    public   class SI_CheckPriceInfo
    {
       
        public string rid { get; set; }
        public string hilist_code { get; set; }
        public string hilist_lmtpric_type { get; set; }
        public string overlmt_dspo_way { get; set; }
        public string insu_admdvs { get; set; }
        public string begndate { get; set; }
        public string enddate { get; set; }
        public Nullable<decimal> hilist_pric_uplmt_amt { get; set; }
        public string vali_flag { get; set; }
    
        public string updt_time { get; set; }
        public string crter_id { get; set; }
        public string crter_name { get; set; }
        public string  crte_time { get; set; }
        public string crte_optins_no { get; set; }
        public string opter_id { get; set; }
        public string opter_name { get; set; }
        public string opt_time { get; set; }
        public string optins_no { get; set; }
        public string tabname { get; set; }
        public string poolarea_no { get; set; }
    }


    /// <summary>
    /// 医保等级
    /// </summary>
    public   class SI_YBcatalog
    {
        public int Id { get; set; }
        public string hilist_code { get; set; }
        public string hilist_name { get; set; }
        public string insu_admdvs { get; set; }
        public string begndate { get; set; }
        public string enddate { get; set; }
        public string med_chrgitm_type { get; set; }
        public string chrgitm_lv { get; set; }
        public string lmt_used_flag { get; set; }
        public string list_type { get; set; }
        public string med_use_flag { get; set; }
        public string matn_used_flag { get; set; }
        public string hilist_use_type { get; set; }
        public string lmt_cpnd_type { get; set; }
        public string wubi { get; set; }
        public string pinyin { get; set; }
        public string memo { get; set; }
        public string vali_flag { get; set; }
        public string rid { get; set; }
        public string  updt_time { get; set; }
        public string crter_id { get; set; }
        public string crter_name { get; set; }
        public string  crte_time { get; set; }
        public string crte_optins_no { get; set; }
        public string opter_id { get; set; }
        public string opter_name { get; set; }
        public string  opt_time { get; set; }
        public string optins_no { get; set; }
        public string poolarea_no { get; set; }
    }
}
