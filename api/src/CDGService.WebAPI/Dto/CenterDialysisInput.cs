using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 机构输入实体
    /// </summary>
    public class CenterDialysisInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 机构编码
        /// </summary>
        public string DialysisCode { get; set; }
        /// <summary>
        /// 机构名称 - 非空
        /// </summary>
        public string DialysisName { get; set; }
        /// <summary>
        /// 所在地区ID - 非空
        /// </summary>
        public string DialysisRegionID { get; set; }

        /// <summary>
        /// 院长
        /// </summary>
        // public string  DialysisContactMan  { get; set; }
        /// <summary>
        /// 负责人ID
        /// </summary>
        public string DialysisContactManID { get; set; }

        public string HeadNurseId { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string DialysisPhone { get; set; }
        /// <summary>
        /// 详细地址 - 非空
        /// </summary>
        public string DialysisAddress { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public float DialysisLng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public float DialysisLat { get; set; }
        /// <summary>
        /// 成立时间 - 非空
        /// </summary>
        public DateTime? SetUpDate { get; set; }
        /// <summary>
        /// 运行时间
        /// </summary>
        public DateTime? RunDate { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string DialysisDetails { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int DataState { get; set; }

        ///// <summary>
        ///// 图片
        ///// </summary>

        public List<string> DialysisImg { get; set; }


    }
}
