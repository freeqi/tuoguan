using CDGService.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Enums
{

    public enum DicType
    {
        /// <summary>
        /// 学历
        /// </summary>
        Education = 1,
        /// <summary>
        /// 性别
        /// </summary> 
        Sex = 3,
        /// <summary>
        /// 职位
        /// </summary>
        Position = 4,
        /// <summary>
        /// 职称
        /// </summary>
        JobTitle = 5,
        /// <summary>
        /// 血源性疾病
        /// </summary>
        BloodBorne = 9,
        /// <summary>
        /// 医保类型
        /// </summary>
        SIInsuredType = 10,
        /// <summary>
        /// 设备型号
        /// </summary>
        EquiModel = 14,
    }



    /// <summary>
    /// 学历
    /// </summary>
    public enum EducationEnum
    {
        /// <summary>
        /// 其他
        /// </summary>
        Other = 0,
        /// <summary>
        /// 小学
        /// </summary>
        Primary = 25,
        /// <summary>
        /// 初中
        /// </summary>
        Junior = 2,
        /// <summary>
        /// 高中
        /// </summary>
        Senior = 26,
        /// <summary>
        /// 中专
        /// </summary>
        Secondary = 4,
        /// <summary>
        /// 大专
        /// </summary>
        JuniorCollege = 5,
        /// <summary>
        /// 本科
        /// </summary>
        RegularCollege = 26,
        /// <summary>
        /// 硕士
        /// </summary>
        Master = 28,
        /// <summary>
        /// 博士
        /// </summary>
        Doctor = 29,
    }


    public enum Ages
    {
        [ChineseEnum("20岁以下")]
        Age20 = 20,
        [ChineseEnum("20-30岁")]
        Age30 = 30,
        [ChineseEnum("31-40岁")]
        Age40 = 40,
        [ChineseEnum("41-50岁")]
        Age50 = 50,
        [ChineseEnum("51-60岁")]
        Age60 = 60,
        [ChineseEnum("60岁以上")]
        Age70 = 120
    }

    public enum TreatmentModelEnum
    {
        [ChineseEnum("SCUF")]
        Tf20caa638b6649caa1c2ed5e3a9d7e1b = 61,
        [ChineseEnum("HD")]
        T71f40374f5224a7889def6ed1f254770 = 62,
        [ChineseEnum("HF")]
        T2482b6bc70334fe49eb78174b7fd05a7 = 63,
        [ChineseEnum("HP")]
        Tfe5d8bb9e67f4260b15e0befe1717df0 = 64,
        [ChineseEnum("HDF")]
        T65d6a26e824f4deb9927614a994a2896 = 65,
        [ChineseEnum("HD+HP")]
        Tbe297a7f9a9f4233a1b37ba474a0395f = 66,
        [ChineseEnum("HFHD")]
        T034b307fd8bd4c93b252cce333524cf7 = 67,    
        [ChineseEnum("CRRT")]
        Tbc7ea03f702a4eacacec809eff09bbe4 = 68,
    }

    public enum EquiState
    {
        [ChineseEnum("正常使用")]
        Eeaf8cb315c9f4b409390e6a55bf57814 = 1,
        [ChineseEnum("维修中")]
        E7c6455e419d44575887de7ea2ad2c46c = 2,
        [ChineseEnum("已报废")]
        E1281344f7cae41e9867cbd4cfbabab18 = 3,

    }


    public enum EquiModel
    {
        [ChineseEnum("SWS-6000")]
        SWS6000 = 76,
        [ChineseEnum("SWS-6000A")]
        SWS6000A = 77,
        [ChineseEnum("SWS-4000")]
        SWS4000 = 78,
        [ChineseEnum("SWS-4000A")]
        SWS4000A = 79,
        [ChineseEnum("SWS-4000B")]
        SWS4000B = 80,
    }

    /*
     0-1年的数量、比例，1-2年的数量、比例，2-3年的数量、比例，3-4年的数量、比例，4-5年的数量、比例，5-6年的数量、比例，6年以上的数量
     */
    public enum EquiUseYear
    {
        [ChineseEnum("0-1年")]
        One = 1,
        [ChineseEnum("1-2年")]
        Two = 2,
        [ChineseEnum("2-3年")]
        Three = 3,
        [ChineseEnum("3-4年")]
        Four = 4,
        [ChineseEnum("4-5年")]
        Five = 5,
        [ChineseEnum("5-6年")]
        Six = 6,
        [ChineseEnum("6年以上")]
        Seven = 20
    }


    public enum PHEnum
    {

        PH = 90,//	PH值离子浓度
        BacterialCulture = 91,//  细菌培养检测
        Endotoxin = 92,// 内毒素检测
    }




    public enum MaincategorieslEnum
    {
        /*
         Id	Name
140	医疗器材
141	医疗设备
142	药品
143	办公用品
             */
        [ChineseEnum("医疗器材")]
        YLQC = 140,
        [ChineseEnum("医疗设备")]
        YLSB = 141,
        [ChineseEnum("药品")]
        YP = 142,
        [ChineseEnum("办公用品")]
        BGYP = 143,
        [ChineseEnum("电子设备")]
        DZSB = 2158,

        [ChineseEnum("其他")]
        QT = 2159,
    }
    /// <summary>
    /// 单位类型 
    /// </summary>
    public enum UnitEnum
    {
        /*
       1制剂单位：瓶、片、支……
2包装单位：箱、盒、包……
3剂量单位：克、毫克……
4其他通用单位：个，台、辆
             */
        [ChineseEnum("全部")]
        AllUnit = 0,
        [ChineseEnum("制剂单位")]
        ZJUnit = 1,
        [ChineseEnum("包装单位")]
        BZUnit = 2,
        [ChineseEnum("剂量单位")]
        JLUnit = 3,
        [ChineseEnum("其他通用单位")]
        QTUnit = 4,

    }

    /// <summary>
    /// 用药类型 
    /// </summary>
    public enum WayEnum
    {
        /*
        1用药途径
        2使用频次 
             */
        [ChineseEnum("全部")]
        ALL = 0,
        [ChineseEnum("用药途径")]
        SYPC = 1,
        [ChineseEnum("使用频次")]
        YYTJ = 2,
    }

    public enum MedicalItemTypeEnum
    {
        /*
卫生耗材
固定资产
低值易耗品
诊疗项目
其他
药品
 /// <summary>
        /// 医疗物品编码
        /// 系统自动生成：
        /// 药品类型编码D开头，耗材类编码U开头（系统内唯一）
        /// 固定资产G 低值L  诊疗 T
         */
        /// <summary>
        /// 药品
        /// </summary>
        [ChineseEnum("药品")]
        D = 1,
        /// <summary>
        /// 卫生耗材
        /// </summary>
        [ChineseEnum("卫生耗材")]
        U = 2,
        /// <summary>
        /// 固定资产
        /// </summary>
        [ChineseEnum("固定资产")]
        G = 3,
        /// <summary>
        /// 低值易耗品
        /// </summary>
        [ChineseEnum("低值易耗品")]
        L = 4,
        /// <summary>
        /// 诊疗项目
        /// </summary>
        [ChineseEnum("诊疗项目")]
        T = 5,
        /// <summary>
        /// 其他
        /// </summary>
        [ChineseEnum("其他")]
        A = 6,




    }

    /// <summary>
    /// 药品类型
    /// </summary>
    public enum DrugsTypeEnum
    {
        [ChineseEnum("口服药（中成药）")]
        KF_Z = 0,
        [ChineseEnum("口服药（西药）")]
        KF_X = 1,
        [ChineseEnum("针剂药（中成药）")]
        ZJ_Z = 2,
        [ChineseEnum("针剂药（西药）")]
        ZJ_X = 3,
        [ChineseEnum("外用药（中成药）")]
        WY_Z = 4,
        [ChineseEnum("外用药（西药）")]
        WY_X = 5,
        [ChineseEnum("抢救药品")]
        QJYP = 6,
    }

    public enum MaterialTypeEnum
    {
        [ChineseEnum("有价卫材")]
        YJWC = 0,
        [ChineseEnum("普通卫材")]
        PTWC = 1

    }

    public enum AssetsItemEnum
    {
        [ChineseEnum("专用设备")]
        ZYSB = 0,
        [ChineseEnum("一般设备")]
        YBSB = 1,
        [ChineseEnum("其他固定资产")]
        QTGDZC = 2,
    }
    public enum LowItemEnum
    {
        [ChineseEnum("办公用品")]
        BGYP = 0,
        [ChineseEnum("医疗用品")]
        YLYP = 1,
        [ChineseEnum("氧气用品")]
        YQYP = 2,
        [ChineseEnum("清洁用品")]
        QJYP = 3,
        [ChineseEnum("五金用品")]
        WJYP = 4,
        [ChineseEnum("生活用品")]
        SHYP = 5,
        [ChineseEnum("棉纤用品")]
        MQYP = 6,

    }

    /// <summary>
    /// 居民特殊就诊标记 
    /// </summary>
    public enum JMCategories
    {
        /*
         10  药店购药 12  普通门诊 13  慢性病门诊 15  重大疾病门诊 16  意外伤害门诊 17  耐多药结核门诊 18  儿童两病门诊 19  产前检查 21  普通住院 22  转入住院 23  耐多药结核住院 24  儿童两病住院 25  重大疾病住院 26  住院分娩 27  急诊转住院 31  康复项目 此字段即：居民医疗类别 
         */
        [ChineseEnum("药店购药")]
        YDGY = 10,
        [ChineseEnum("普通门诊")]
        PTMZ = 12,
        [ChineseEnum("慢性病门诊")]
        MXBMZ = 13,
        [ChineseEnum("重大疾病门诊")]
        ZDJBMZ = 15,
        [ChineseEnum("意外伤害门诊")]
        YWSHMZ = 16,
        [ChineseEnum("耐多药结核门诊")]
        NDYJHMZ = 17,
    }

    /// <summary>
    /// 职工特殊就诊标记 
    /// </summary>
    public enum ZGCategories
    {
        /*
         10  药店购药 11  普通门诊 13  特病门诊 14  急诊 15  超声乳化白内障摘除 16  生育门诊 17  工伤门诊 18  工伤康复门诊 19  工伤辅助器具 21  住院 22  转入住院 23  出院家庭病床 24  高龄家庭病床 25  急诊转住院 26  工伤康复住院 
 
         */
        [ChineseEnum("药店购药")]
        YDGY = 10,
        [ChineseEnum("普通门诊")]
        PTMZ = 11,
        [ChineseEnum("特病门诊")]
        MXBMZ = 13,
        [ChineseEnum("急诊")]
        ZDJBMZ = 14,

    }


    public enum ALLCategories
    {
        /*
         10  药店购药 11  普通门诊 13  特病门诊 14  急诊 15  超声乳化白内障摘除 16  生育门诊 17  工伤门诊 18  工伤康复门诊 19  工伤辅助器具 21  住院 22  转入住院 23  出院家庭病床 24  高龄家庭病床 25  急诊转住院 26  工伤康复住院 
 
         */
        [ChineseEnum("门诊挂号")]
        MZGH = 12,
        [ChineseEnum("普通门诊")]
        PTMZ = 11,
        [ChineseEnum("急诊")]
        JZ = 13,
        [ChineseEnum("门诊慢特病")]
        MZMTB = 14,
        [ChineseEnum("重大疾病门诊")]
        ZDJBMZ = 9901,

        [ChineseEnum("普通门诊")]
        ZF = 0,
    }




    public enum PayTypeEnum
    {
        /*
         支付方式 1现金 2医保账户 3医保统筹合计 4支付宝 5微信 6银联卡
        现金差额 = 0,
        现金 = 1,
        医保账户 = 2,
        医保统筹合计 = 3,
        支付宝 = 4,
        微信 = 5,
        银联卡 = 6,
        统筹 = 7,
        一般诊疗支付 = 8,
        民政救助 = 9,
        公务员补助 = 10,
        大额 = 11,
        单病种垫资 = 12,
        超标扣款 = 13
         */
        [ChineseEnum("现金")]
        XJ = 1,
        [ChineseEnum("医保账户")]
        YBZF = 2,
        [ChineseEnum("医保统筹合计")]
        YBTCHJ = 3,
        [ChineseEnum("支付宝")]
        ZFB = 4,
        [ChineseEnum("微信")]
        WX = 5,
        [ChineseEnum("银联卡")]
        YLK = 6,
        [ChineseEnum("统筹")]
        TC = 7,
        [ChineseEnum("开业优惠")]
        YBZLZB = 8,
        [ChineseEnum("民政救助")]
        MZJZ = 9,
        [ChineseEnum("公务员补助")]
        GWYBZ = 10,
        [ChineseEnum("大额")]
        DG = 11,
        [ChineseEnum("单病种垫资")]
        DBZDZ = 12,
        [ChineseEnum("超标扣款")]
        CBKK = 13,

    }


    public enum BusinessTypeEnum
    {
        /*
          /// <summary>
        /// 1租金（门诊部）、2租金（宿舍）、3物管、4水电、5工资、6社保、7福利、
        /// 8爱心基金、9折旧费、10装修/无形资产摊销、11车辆费用（含维修保养）、12 其他
        /// </summary>
         */
        [ChineseEnum("门诊部")]
        mzbzj = 1,
        [ChineseEnum("宿舍")]
        sszj = 2,
        [ChineseEnum("物管")]
        wg = 3,
        [ChineseEnum("水电")]
        sd = 4,
        [ChineseEnum("工资")]
        gz = 5,
        [ChineseEnum("社保")]
        sb = 6,
        [ChineseEnum("福利")]
        fl = 7,
        [ChineseEnum("爱心基金")]
        axjj = 8,
        [ChineseEnum("折旧费")]
        zjf = 9,
        [ChineseEnum("装修")]
        zx = 10,
        [ChineseEnum("车辆费用")]
        clfy = 11,
        [ChineseEnum("其他")]
        qt = 12

    }
    /// <summary>
    /// 传染病
    /// </summary>
    public enum Contagion
    {
        /// <summary>
        /// 乙肝
        /// </summary>
        HBV = 1,
        /// <summary>
        /// 丙肝
        /// </summary>
        HCV = 2,
        /// <summary>
        /// 乙肝+丙肝
        /// </summary>
        HBVHCV = 3,
        /// <summary>
        /// HIV
        /// </summary>
        HIV = 4,
        /// <summary>
        /// 梅毒
        /// </summary>
        MD = 5
    }




}
