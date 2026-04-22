using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 医疗质量和安全管理指标统计
    /// 月度指标
    /// </summary>
    public class MedicalIndexesMonth
    {

        public string Id { get; set; }
        public string CenterId { get; set; }
        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis centerDialysis { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MedicalIndicatorsMonth Indicators { get; set; }
        /// <summary>
        /// 上月
        /// </summary>
        public int LastMonth { get; set; }
        /// <summary>
        /// 本月
        /// </summary>
        public int CurrentMonth { get; set; }
        /// <summary>
        /// 上年本月
        /// </summary>
         public int? LastYear { get; set; }

        /// <summary>
        /// 同比
        /// </summary>
        public decimal? SameCompared { get; set; }
        /// <summary>
        /// 环比
        /// </summary>
        public decimal? Sequential { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MedicalStatisticalType StatisticalType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public string Founder { get; set; }
        /// <summary>
        /// 统计月份
        /// </summary>
        public DateTime? MondthDate { get; set; }
        public DateTime? FounderDate { get; set; }
        public int DataState { get; set; }

    }
}
