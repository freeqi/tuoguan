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
    /// 日透析次数盘点 
    /// </summary>
    public class CurePatternCheck
    {

        public string Id { get; set; }

        public string CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual CenterDialysis centerDialysis { get; set; }
        /// <summary>
        /// 治疗模式， 字典表 SCU，HDF，HD+HP……
        /// </summary>
        public string CurePattern { get; set; }
        /// <summary>
        /// 治疗使用透析器型号150G，	FX60，	15_UC，	FX80……
        /// </summary>
        public CureModelEnum CureModel { get; set; }
        /// <summary>
        /// 日次数
        /// </summary>
        public int CureNumber { get; set; }
        /// <summary>
        /// 盘点人（默认系统自动盘点）
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 盘点入库时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        /// <summary>
        /// 盘点日期
        /// </summary>
        public DateTime? CheckDate { get; set; }

    }
}
