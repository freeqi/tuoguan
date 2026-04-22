using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class MedicalIndexesYear
    {
        public string Id { get; set; }
        /// <summary>
        /// 透析中心ID
        /// </summary>
        public string CenterId { get; set; }
        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis centerDialysis { get; set; }
        /// <summary>
        /// 指标
        /// </summary>
        public MedicalIndicatorsYear Indicators { get; set; }
        /// <summary>
        /// 上年度
        /// </summary>
        public double? LastYear { get; set; }
        /// <summary>
        /// 本年度
        /// </summary>
        public double? CurrentYear { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MedicalStatisticalYearType StatisticalType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public int DataState { get; set; }

    }
}
