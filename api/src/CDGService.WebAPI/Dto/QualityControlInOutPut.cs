using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class QualityControlQueryInPut
    {
        public string CenterId { get; set; }
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime? BeginTime { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        // public object Title { get; set; }

        public string QueryName{ get; set; }

        public  Dictionary<string, bool> Title = new Dictionary<string, bool>();
        //public DateTime EndTime { get; set; }
    }

    public class QualityControlQueryInPuts
    {
        public string CenterId { get; set; }
        /// <summary>
        /// 统计时间
        /// </summary>
        public int BeginTime { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        // public object Title { get; set; }

        public Dictionary<string, bool> Title = new Dictionary<string, bool>();
        //public DateTime EndTime { get; set; }
    }
    public class SelTitle
    {
        public string Key { get; set; }
        public bool Value { get; set; }
    }

    /// <summary>
    /// 质控-血常规
    /// </summary>
    public class QualityControlOutPut
    {
        public QualityControlChartOutPut[] qualityControlChartOutPuts { get; set; }

        public QualityControlDataOutPut[] qualityControlDataOutPuts { get; set; }
    }


    /// <summary>
    /// 图形数据
    /// </summary>
    public class QualityControlChartOutPut
    {
        /// <summary>
        /// count
        /// </summary>
        public double value { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }
    }
    /// <summary>
    /// 表单数据
    /// </summary>
    public class QualityControlDataOutPut
    {
        /// <summary>
        /// 中心名称
        /// </summary>
        public string CenterName { get; set; }

        /// <summary>
        /// 患者人数
        /// </summary>
        public string PanterCount { get; set; }
        /// <summary>
        /// 检查时间
        /// </summary>
        public string Times { get; set; }
        /// <summary>
        /// 检查人数
        /// </summary>
        public int checkCount { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// 合格率
        /// </summary>
        public string PercentOfPass { get; set; }
    }

    //
    public class BloodBiochemicalDataOutPut
    {
        /// <summary>
        /// 中心名称
        /// </summary>
        public string CenterName { get; set; }

        /// <summary>
        /// 患者人数
        /// </summary>
        public string PanterCount { get; set; }
        /// <summary>
        /// 检查时间
        /// </summary>
        public string Times { get; set; }
      public string Title { get; set; }
        /// <summary>
        /// 肝功=检查人数
        /// </summary>
        public string LcheckCount { get; set; }
        /// <summary>
        /// 肾功-检查人数
        /// </summary>
        public string RcheckCount { get; set; }
        /// <summary>
        /// 电解质-检查人数
        /// </summary>
        public string ScheckCount { get; set; }
        /// <summary>
        /// 合格率
        /// </summary>
       // public string PercentOfPass { get; set; }
    }

    /// <summary>
    /// 血压
    /// </summary>
    public class QualityHypertensionDataOutPut
    {
        /// <summary>
        /// 中心名称
        /// </summary>
        public string CenterName { get; set; }

        /// <summary>
        /// 患者人数
        /// </summary>
        public string PanterCount { get; set; }
        /// <summary>
        /// 检查时间
        /// </summary>
        public string Times { get; set; }
        public string Title { get; set; }
        /// <summary>
        ///血压＜140/90MMHG的60岁以下患者
        /// </summary>
        public string LcheckCount { get; set; }
        /// <summary>
        /// 血压＜150/90MMHG的60岁以上患者
        /// </summary>
        public string RcheckCount { get; set; }
        /// <summary>
        /// 电解质-检查人数
        /// </summary>
     //   public string ScheckCount { get; set; }
        /// <summary>
        /// 合格率
        /// </summary>
       // public string PercentOfPass { get; set; }
    }

    /// <summary>
    /// 血红蛋白
    /// </summary>
    public class HemoglobinDataOutPut
    {
        /// <summary>
        /// 中心名称
        /// </summary>
        public string CenterName { get; set; }

        /// <summary>
        /// 患者人数
        /// </summary>
        public string PanterCount { get; set; }
        /// <summary>
        /// 检查时间
        /// </summary>
        public string Times { get; set; }
        public string Title { get; set; }
        /// <summary>
        ///检查人数
        /// </summary>
        public string CheckCount { get; set; }
        /// <summary>
        /// 血红蛋白≥100G/L的患者人数
        /// </summary>
        public string RcheckCount { get; set; }
        /// <summary>
        /// 电解质-检查人数
        /// </summary>
     //   public string ScheckCount { get; set; }
        /// <summary>
        /// 合格率
        /// </summary>
       // public string PercentOfPass { get; set; }
    }

}
