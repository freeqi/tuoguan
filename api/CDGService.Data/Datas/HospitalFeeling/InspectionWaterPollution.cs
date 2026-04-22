using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 透析用水生物污染检验情况
    /// </summary>
    public class InspectionWaterPollution
    {

        public string Id { get; set; }
        public string CenterId { get; set; }
        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis CenterDialysis { get; set; }
        /// <summary>
        /// 水菌落数检验结果
        /// </summary>
        public string WaterColonyCount { get; set; }
        /// <summary>
        /// 标本
        /// </summary>
        public string Specimen { get; set; }
        /// <summary>
        /// 内毒素检验结果
        /// </summary>
        public string Endotoxin { get; set; }
        /// <summary>
        /// 检验日期
        /// </summary>
        public DateTime? DisinfectionTime { get; set; }
        /// <summary>
        /// 是否合格
        /// </summary>
        public bool IsQualified { get; set; }
        /// <summary>
        /// 消毒人
        /// </summary>
        public string DisinfectionUser { get; set; }
        /// <summary>
        /// 不合格说明
        /// </summary>
        public string UnqualifiedDescribe { get; set; }
        /// <summary>
        /// 不合格后续处理
        /// </summary>
        public string UnqualifiedDeal { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
    }
}
