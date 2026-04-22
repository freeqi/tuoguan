using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class DialysisSearchInput
    {
        /// <summary>
        /// 中心名称
        /// </summary>
        public string DialysisName { get; set; }

        /// <summary>
        /// 地区ID
        /// </summary>
        public int DialysisRegion { get; set; }

        /// <summary>
        /// 成立开始时间
        /// </summary>
        public DateTime? BeginSetUpDate { get; set; }

        /// <summary>
        /// 成立结束时间
        /// </summary>
        public DateTime? EndSetUpDate { get; set; }
        /// <summary>
        /// 运行开始时间
        /// </summary>
        public DateTime? BeginRunDate { get; set; }
        /// <summary>
        /// 运行结束时间
        /// </summary>
        public DateTime? EndRunDate { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }
    }
}
