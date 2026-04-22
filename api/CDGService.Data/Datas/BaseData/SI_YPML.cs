using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 医保中心药品目录
    /// </summary>
    public class SI_YPMLS
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        ///  中心编码
        /// </summary>
        public string YPLSH { get; set; }
        /// <summary>
        ///药品编码
        /// </summary>
        public string YPBM { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        public string TYM { get; set; }
        /// <summary>
        /// 助记码
        /// </summary>
        public string TYMZJM { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string SPM { get; set; }
        /// <summary>
        /// 商品名助记码
        /// </summary>
        public string SPMZJM { get; set; }
        /// <summary>
        /// 英文名
        /// </summary>
        public string YWM { get; set; }
        /// <summary>
        /// 费用分类
        /// </summary>
        public string LBDM { get; set; }
        public string CFYBZ { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string YLFYDJ { get; set; }
        public string GSFYDJ { get; set; }
        public string SYFYDJ { get; set; }
        /// <summary>
        /// 批发价
        /// </summary>
        public decimal? PFJ { get; set; }
        /// <summary>
        /// 基准价格
        /// </summary>
        public decimal? YLBZDJ { get; set; }
        public decimal? GSBZDJ { get; set; }
        public decimal? SYBZDJ { get; set; }
        /// <summary>
        /// 自费比例
        /// </summary>
        public decimal? YLZFBL { get; set; }
        public decimal? SYZFBL { get; set; }
        public decimal? GSZFBL { get; set; }
        public string JX { get; set; }
        public decimal? BZSL { get; set; }
        public string BZDW { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string HL { get; set; }
        public string HLDW { get; set; }
        public string RL { get; set; }
        public string RLDW { get; set; }
        public string GMP { get; set; }
        public string YCMC { get; set; }
        public string YPXJFS { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? BGSJ { get; set; }
        public string TQFYDJ { get; set; }
        public decimal? TQZFBL { get; set; }
        public decimal? TQBZDJ { get; set; }
        public string CJFYDJ { get; set; }
        public decimal? CJZFBL { get; set; }
        public decimal? CJBZDJ { get; set; }
        public string WSSSYBZ { get; set; }
        public string XETSYBZ { get; set; }
        public string XMZSYBZ { get; set; }
        public string JCYBZ { get; set; }
        public string ZZSJBZ { get; set; }
        public string ZZSJSBJG { get; set; }
        /// <summary>
        /// 限制使用说明
        /// </summary>
        public string BZ { get; set; }
        public string GSFZQJBJ { get; set; }
        public string GSKFXNBJ { get; set; }
        public string GSFPGXXMBJ { get; set; }
        //1	180172	190203316170300	维A酸乳膏 WASRG   芙林 FL  Tretinoin Cream	1902		1			14.43	16.60	NULL NULL	0.0000	NULL NULL    乳膏剂	1.00	支	25	g 湖北恒安药业有限公司	0	2018-06-25		NULL NULL        NULL NULL				1			△		NULL NULL
        /// <summary>
        /// 国家药品代码
        /// </summary>
        public string GJYPDM { get; set; }
    }

    public class SI_ZLXMS
    {
        public int id { get; set; }
        public string XMLSH { get; set; }
        public string XMBM { get; set; }
        public string XMMC { get; set; }
        public string ZJM { get; set; }
        public decimal? TPJ { get; set; }
        public decimal? YLBZJ { get; set; }
        public decimal? GSBZJ { get; set; }
        public decimal? SYBZJ { get; set; }
        public string DW { get; set; }
        public string YLFYDJ { get; set; }
        public string GSFYDJ { get; set; }
        public string SYFYDJ { get; set; }
        public decimal? YLZFBL { get; set; }
        public decimal? GSZFBL { get; set; }
        public decimal? SYZFBL { get; set; }
        public decimal? TXBL { get; set; }
        public string XJFS { get; set; }
        public string LSF { get; set; }
        public string BZ { get; set; }
        public DateTime? BGSJ { get; set; }
        public string TPXMBZ { get; set; }
        public string TQFYDJ { get; set; }
        public decimal? TQZFBL { get; set; }
        public decimal? TQBZDJ { get; set; }
        public string CJFYDJ { get; set; }
        public decimal? CJZFBL { get; set; }
        public decimal? CJBZDJ { get; set; }
        public string TSYTBJ { get; set; }
        public string XMNH { get; set; }
        public string CWNR { get; set; }
        public string GSFZQJBJ { get; set; }
        public string GSKFXMBJ { get; set; }
        public string GSFPGXXMBJ { get; set; }
        public string GSKTFBJ { get; set; }
        public string NHYCXHC { get; set; }
        public string JJSM { get; set; }
        public decimal? ZGCFJL { get; set; }
        public decimal? ZGCFYL15 { get; set; }
        public decimal? ZGCFYL20 { get; set; }
        public decimal? ZGCFCB { get; set; }
        public decimal? ZGCFZF { get; set; }
        public decimal? JMCFJL { get; set; }
        public decimal? JMCFYL15 { get; set; }
        public decimal? JMCFYL20 { get; set; }
        public decimal? JMCFCB { get; set; }
        public decimal? JMCFZF { get; set; }
        public decimal? GSCFJL { get; set; }
        public decimal? GSCFCB { get; set; }
        public decimal? GSCFZF { get; set; }
        public decimal? SYCFJL { get; set; }
        public decimal? SYCFYL15 { get; set; }
        public decimal? SYCFYL20 { get; set; }
        public decimal? SYCFCB { get; set; }
        public decimal? SYCFZF { get; set; }
        public decimal? TQCFJL { get; set; }
        public decimal? TQCFYL15 { get; set; }
        public decimal? TQCFYL20 { get; set; }
        public decimal? TQCFCB { get; set; }
        public decimal? TQCFZF { get; set; }
        public DateTime? ZZSJ { get; set; }
        public DateTime? JBSJ { get; set; }
        public decimal? ZGDEBXBZ { get; set; }
        public decimal? JMDEBXBZ { get; set; }
        public string YLGGXMBJ { get; set; }
        /// <summary>
        /// 国家项目代码
        /// </summary>
        public string GJXMDM { get; set; }
        /// <summary>
        /// 国家项目分类
        /// </summary>
        public string GJXMFL { get; set; }
    }

    /// <summary>
    /// 病种目录
    /// </summary>
    public class SI_BZML
    { 
      //  id BZBM    BZMC ZJM BZFL TJM JMBZFL 

        public int id { get; set; }

        public string BZBM { get; set; }
        public string BZMC { get; set; }
        public string ZJM { get; set; }

        public string BZFL { get; set; }

        public string TJM { get; set; }

        public string JMBZFL { get; set; }
    }
}
