using CDGService.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Enums
{

    /// <summary>
    /// 医疗月度指标值
    /// </summary>
    public enum MedicalIndicatorsMonth
    {
        #region -- 月度观察指标
        [ChineseEnum("治疗总人次")]
        CureSum = 1,
        [ChineseEnum("普通透析")]
        HD = 2,
        [ChineseEnum("高通血透")]
        HFHD = 3,
        [ChineseEnum("透析滤过")]
        HDF = 4,
        [ChineseEnum("血液灌流")]
        HD_HP = 5,
        [ChineseEnum("严重并发症发生例次")]
        Complications = 6,
        #endregion

        #region -- 患者人数
        [ChineseEnum("就诊人数和")]
        SeeDoctorSum = 7,
        [ChineseEnum("职工医保")]
        WorkerHealth = 8,
        [ChineseEnum("居民医保")]
        ResidentsHealth = 9,
        [ChineseEnum("其他")]
        OtherHealth = 10,

        #endregion
        #region -- 归转人数
        [ChineseEnum("新增患者")]
        NewPatients = 11,
        [ChineseEnum("死亡")]
        DeathPatients = 12,
        [ChineseEnum("转院")]
        TransferPatients = 13,
        [ChineseEnum("病重住院")]
        ICUPatients = 14,
        [ChineseEnum("转腹膜透析")]
        ZFMTXPatients = 15,
        [ChineseEnum("转肾移植")]
        ZSYZPatients = 16,
        #endregion
        #region -- 月度统计指标
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

        #endregion
    }

    /// <summary>
    /// 医疗月度统计类型
    /// </summary>
    public enum MedicalStatisticalType
    {
        /// <summary>
        /// 月度观察指标
        /// </summary>
        ObserveIndicators = 1,
        /// <summary>
        /// 患者人数
        /// </summary>
        Patients = 2,
        /// <summary>
        /// 归转人数
        /// </summary>
        Regression = 3,
        /// <summary>
        /// 月度统计指标
        /// </summary>
        StatisticalIndicators = 4,

    }


    /// <summary>
    /// 年度统计类型
    /// </summary>
    public enum MedicalStatisticalYearType
    {
        /// <summary>
        /// 年度观察指标
        /// </summary>
        ObserveIndicators = 1,
        /// <summary>
        /// 感染检测
        /// </summary>
        Infection = 2,
        /// <summary>
        /// 血管通路类型
        /// </summary>
        VascularAccess = 3,
        /// <summary>
        /// 年度统计指标
        /// </summary>
        StatisticalIndicators = 4,

    }

    /// <summary>
    /// 年度统计指标
    /// </summary>
    public enum MedicalIndicatorsYear
    {
        [ChineseEnum("透析总治疗人次记录")]
        TXZZLRC = 1,
        [ChineseEnum("普通血透")]
        HD = 2,
        [ChineseEnum("高通血液透析")]
        HFHD = 3,
        [ChineseEnum("血液透析滤过")]
        HDF = 4,
        [ChineseEnum("血液灌流")]
        HD_HP = 5,
        [ChineseEnum("单纯超滤")]
        SCUF = 6,
        [ChineseEnum("血液滤过")]
        XYLG = 7,
        [ChineseEnum("死亡总例数")]
        SWZLS = 8,
        [ChineseEnum("严重并发症发生例次")]
        BFZFSLC = 9,
        [ChineseEnum("血透转腹膜")]
        XTZFM = 10,
        [ChineseEnum("转肾移植例次")]
        ZSYZLC = 11,
        [ChineseEnum("HBsAg转阳例次")]
        HBsAgZY = 12,
        [ChineseEnum("HBeAg转阳")]
        HBeAgZY = 13,
        [ChineseEnum("HCV抗体转阳")]
        HCVKTZY = 14,
        [ChineseEnum("动静脉内瘘")]
        DJMLL = 15,
        [ChineseEnum("中心静脉导管（NTC）")]
        NTC = 16,
        [ChineseEnum("中心静脉导管（TCC）")]
        TCC = 17,
        [ChineseEnum("动静脉直接穿刺")]
        DJMZJCC = 18,
        [ChineseEnum("人工血管")]
        RGXG = 19,
        [ChineseEnum("年度死亡率")]
        NDSWL = 20,
        [ChineseEnum("透析前血压达标率")]
        TXQXYDBL = 21,
        [ChineseEnum("透析后血压达标率")]
        TXHXYDBL = 22,

        [ChineseEnum("钙（2.10-2.50mmol/L）")]
        GA = 23,
        [ChineseEnum("磷（1.13-1.78mmol/L）")]
        LIN = 24,
        [ChineseEnum("PTH（150-300pg/L）")]
        PIH = 25,
        [ChineseEnum("血红蛋白（110-120g/L）")]
        XHDB110 = 26,
        [ChineseEnum("血红蛋白（100 - 130g/L）")]
        XHDB100 = 27,
        [ChineseEnum("Kt/V达标率（≥1.2）")]
        KTV = 28,
        [ChineseEnum("URR值达标率（≥60%）")]
        URR = 29,
    }

    /// <summary>
    /// 费用出处
    /// </summary>

    public enum CostSource
    {
        [ChineseEnum("药房")]
        YF = 1,
        [ChineseEnum("护理部")]
        HLB = 2,
        [ChineseEnum("肾病科")]
        SBK = 3,
        [ChineseEnum("物资库房")]
        WZKF = 4,

    }

    /*
     150G 15_UC	FX80	PS160	15_LC	FX80	150G	FX10	FX60	15_UC	FX80	PS160	15_LC	15_UC	FX80	150G	FX10

         */
    /// <summary>
    /// 透析器型号
    /// </summary>
    public enum CureModelEnum
    {
        [ChineseEnum("NV-1.5T")]
        CNV15T = 2,
        [ChineseEnum("15_LC")]
        C15_LC = 3,
        [ChineseEnum("PS160")]
        CPS160 = 4,
        [ChineseEnum("FX800")]
        CFX800 = 5,
        [ChineseEnum("FX600")]
        CFX600 = 6,
        [ChineseEnum("NV-1.8T")]
        CNV18T = 7,
        [ChineseEnum("150G")]
        C150G = 8,
        [ChineseEnum("FX10")]
        CFX10 = 9,
        [ChineseEnum("FX60")]
        CFX60 = 10,
        [ChineseEnum("15_UC")]
        C15_UC = 1,
        [ChineseEnum("FX80")]
        CFX80 = 11,
        [ChineseEnum("FX8")]
        CFX8 = 12,
    }







}
