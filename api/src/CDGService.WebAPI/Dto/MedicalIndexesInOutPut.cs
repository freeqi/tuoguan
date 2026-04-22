using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class MedicalIndexesMonthQueryInPut
    {
        /// <summary>
        /// 机构ID 查询全部时使用另外一个接口
        /// </summary>
        public string CenterId { get; set; }

        /// <summary>
        /// 统计日期-- 月指标时间 
        /// </summary>
        public DateTime QueryDateTime { get; set; }
        /// <summary>
        /// 指标类型
        /// 1、月度观察指标
        /// 2、患者人数
        /// 3、归转人数
        /// 4、月度统计指标
        /// </summary>
        public MedicalStatisticalType medicalStatisticalType { get; set; }
        /// <summary>
        /// 月度指标项
        /// </summary>
       // public MedicalIndicatorsMonth medicalIndicatorsMonth{ get; set; }
    }



    public class ALLCenterMedicalIndexesMonthQueryInPut
    {

        /// <summary>
        /// 统计日期-- 月指标时间 
        /// </summary>
        public DateTime QueryDateTime { get; set; }
        /// <summary>
        /// 指标类型
        /// 1、月度观察指标
        /// 2、患者人数
        /// 3、归转人数
        /// 4、月度统计指标
        /// </summary>
        //  public MedicalStatisticalType medicalStatisticalType { get; set; }
        /// <summary>
        /// 月度指标项
        /// </summary>
        public MedicalIndicatorsMonth medicalIndicatorsMonth { get; set; }
    }


    public class KeyValue
    {

        public int Key { get; set; }

        public string Value { get; set; }
    }

    public class MedicalIndexesMonthOutPut
    {

        public string Id { get; set; }
        public string CenterId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MedicalIndicatorsMonth Indicators { get; set; }
        /// <summary>
        /// 类型中文
        /// </summary>
        public string IndicatorsName { get; set; }
        /// <summary>
        /// 上月
        /// </summary>
        public int LastMonth { get; set; }
        /// <summary>
        /// 本月
        /// </summary>
        public int CurrentMonth { get; set; }
        /// <summary>
        /// 同比
        /// </summary>
        public string SameCompared { get; set; }
        /// <summary>
        /// 环比
        /// </summary>
        public string Sequential { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public int DataState { get; set; }

        public CellClassName cellClassName { get; set; }

        /// <summary>
        /// 死亡名单
        /// </summary>
        public List<Tag> tags { get; set; }

    }
    public class CellClassName
    {
        public string IndicatorsName { get; set; }
    }

    public class Tag
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TagTime { get; set; }

        public int Age { get; set; }
    }


    public class AllCenterMedicalIndexesMonthOutPut
    {

        public string Id { get; set; }
        public string CenterId { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MedicalIndicatorsMonth Indicators { get; set; }
        /// <summary>
        /// 类型中文
        /// </summary>
        public string IndicatorsName { get; set; }
        /// <summary>
        /// 上月
        /// </summary>
        public int LastMonth { get; set; }
        /// <summary>
        /// 本月
        /// </summary>
        public int CurrentMonth { get; set; }
        /// <summary>
        /// 同比
        /// </summary>
        public string SameCompared { get; set; }
        /// <summary>
        /// 环比
        /// </summary>
        public string Sequential { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public int DataState { get; set; }

        /// <summary>
        /// 死亡名单
        /// </summary>
        public List<Tag> tags { get; set; }
    }


    //年度

    public class MedicalIndexesYearQueryInPut
    {
        /// <summary>
        /// 机构ID 查询全部时使用另外一个接口
        /// </summary>
        public string CenterId { get; set; }

        /// <summary>
        /// 统计日期-- 月指标时间 
        /// </summary>
        public DateTime QueryDateTime { get; set; }
        /// <summary>
        /// 指标类型
        ///  1)	年度观察指标
        ///  2)	感染检测
        ///  3)	血管通路类别
        ///  4)	年度统计指标
        /// </summary>
        public MedicalStatisticalYearType medicalStatisticalType { get; set; }
        /// <summary>
        /// 死亡名单
        /// </summary>
        public List<Tag> tags { get; set; }
        /// <summary>
        /// 月度指标项
        /// </summary>
       // public MedicalIndicatorsMonth medicalIndicatorsMonth{ get; set; }
    }



    public class ALLCenterMedicalIndexesYearQueryInPut
    {

        /// <summary>
        /// 统计日期-- 月指标时间 
        /// </summary>
        public DateTime QueryDateTime { get; set; }
        /// <summary>
        /// 指标类型
        /// 1、月度观察指标
        /// 2、患者人数
        /// 3、归转人数
        /// 4、月度统计指标
        /// </summary>
        //  public MedicalStatisticalType medicalStatisticalType { get; set; }
        /// <summary>
        /// 年度度指标项
        /// </summary>
        public MedicalIndicatorsYear medicalIndicatorsYear { get; set; }
    }

    public class AllCenterMedicalIndexesYearOutPut
    {

        public string Id { get; set; }
        public string CenterId { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MedicalIndicatorsYear Indicators { get; set; }
        /// <summary>
        /// 类型中文
        /// </summary>
        public string IndicatorsName { get; set; }
        /// <summary>
        /// 上年度
        /// </summary>
        public string LastYear { get; set; }
        /// <summary>
        /// 本年度
        /// </summary>
        public string CurrentYear { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string StatisticalType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public int DataState { get; set; }
        /// <summary>
        /// 死亡名单
        /// </summary>
        public List<Tag> tags { get; set; }
    }

    public class MedicalIndexesYearOutPut
    {

        public string Id { get; set; }
        public string CenterId { get; set; }
        /// <summary>
        /// 指标
        /// </summary>
        public MedicalIndicatorsYear Indicators { get; set; }
        /// <summary>
        /// 指标中文
        /// </summary>
        public string IndicatorsName { get; set; }

        /// <summary>
        /// 上年度
        /// </summary>
        public string LastYear { get; set; }
        /// <summary>
        /// 本年度
        /// </summary>
        public string CurrentYear { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string StatisticalType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public int DataState { get; set; }
        /// <summary>
        /// 死亡名单
        /// </summary>
        public List<Tag> tags { get; set; }
    }


}
