using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 血透中心
    /// </summary>
    public class CenterDialysis : ISoftDelete
    {

        public string Id { get; set; }
        /// <summary>
        /// 机构编码
        /// </summary>
        public string DialysisCode { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string DialysisName { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// 所在地区ID
        /// </summary>
        public string DialysisRegionID { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        [ForeignKey(nameof(DialysisRegionID))]
        public virtual SysRegion DialysisRegions { get; set; }
        /// <summary>
        /// 负责人ID
        /// </summary>

        public string DialysisContactManID { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>

        [ForeignKey(nameof(DialysisContactManID))]
        public virtual Employee DialysisContactMan { get; set; }




        //public string DialysisContactMan { get; set; }
        public int SortNnm { get; set; }
        /// <summary>
        ///护士长ID
        /// </summary>
        public string HeadNurseId { get; set; }

        //   public Employee HeadNurseUser { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string DialysisPhone { get; set; }
        /// <summary>
        /// 详细地址
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
        /// 成立时间
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


        public string DialysisImg { get; set; }
        ///// <summary>
        ///// 图片
        ///// </summary>
        public string DialysisImg1 { get; set; }
        ///// <summary>
        ///// 图片
        ///// </summary>
        public string DialysisImg2 { get; set; }
        ///// <summary>
        ///// 图片
        ///// </summary>
        public string DialysisImg3 { get; set; }
        ///// <summary>
        ///// 图片
        ///// </summary>
        public string DialysisImg4 { get; set; }

        public string AddMan { get; set; }

        public DateTime? AddTime { get; set; }
        public string ModifyMan { get; set; }
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 中心端接口地址
        /// </summary>
        public string CenterUrl { get; set; }

        public string NationalCode { get; set; }
        public string NationalName { get; set; }

        /// <summary>
        /// 中心端前端地址
        /// </summary>
        public string CenterWebURL { get; set; }

        public string CenterConn { get; set; }
    }
}
