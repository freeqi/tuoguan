using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 机构输出实体
    /// </summary>
    public class CenterDialysisOutPut
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
        /// 所在地区ID
        /// </summary>
        public List<string> DialysisRegionID { get; set; } = new List<string>();
        /// <summary>
        /// 地区
        /// </summary>

        public string DialysisRegion { get; set; }
        /// <summary>
        /// 负责人ID
        /// </summary>
        public string DialysisContactManID { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string DialysisContactMan { get; set; }

        public string HeadNurseId { get; set; }

        /// <summary>
        /// 护士长
        /// </summary>
        public string HeadNurseMan { get; set; }


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
        public string SetUpDate { get; set; }
        /// <summary>
        /// 运行时间
        /// </summary>
        public string RunDate { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string DialysisDetails { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public List<string > DialysisImgs { get; set; } = new List<string>();
        /// <summary>
        /// 状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string ModifyTime { get; set; }

        public string CenterWebURL { get; set; }

        public string CenterWebuser { get; set; }
        public string CenterWebpwd { get; set; }



    }

    public class CenterListOutPut
    {
        public string Id { get; set; }        
        /// <summary>
        /// 机构名称
        /// </summary>
        public string DialysisName { get; set; }

    }

    public class ImageFlieData

    {
        public string url { get; set; }
        public string imageType { get; set; }
    }

    /// <summary>
    /// 地图标记
    /// </summary>
    public class DialysisMapOutPut
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
        /// 位置
        /// </summary>
        public float[] position { get; set; }
        /// <summary>
        /// 经度
        ///// </summary>
        //public float DialysisLng { get; set; }
        ///// <summary>
        ///// 纬度
        ///// </summary>
        //public float DialysisLat { get; set; }

    }
    /// <summary>
    /// 根据地区统计机构输出实体
    /// </summary>
    public class DialysisStatisticalByCityOutPut
    {
        /// <summary>
        /// 地区ID
        /// </summary>
        public string RegionId { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string RetionName { get; set; }
        /// <summary>
        /// 机构总量
        /// </summary>
        public int DialysisCount { get; set; }
        /// <summary>
        /// 百分比
        /// </summary>
        public string Percentage { get; set; }

    }
    /// <summary>
    /// 根据运行年份统计机构输出实体
    /// </summary>
    public class DialysisStatisticalByYearOutPut
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 总量
        /// </summary>
        public int DialysisCount { get; set; }
        /// <summary>
        /// 较上年增长数量
        /// </summary>
        public int growth { get; set; }

    }


    public class AllTotal
    {
        public int CenterCount { get; set; }

        public int EmpCount { get; set; }

        public int MachineCount { get; set; }
        public int patientCount { get; set; }
    }


    public class OrganizationInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 机构简码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string NameUS { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 地址坐标X
        /// </summary>
        public string AddrX { get; set; }

        /// <summary>
        /// 地址坐标Y
        /// </summary>
        public string AddrY { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string LxTel { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string PersonLiable { get; set; }

        /// <summary>
        /// 机构形象照片
        /// </summary>
        public string FigureUrl { get; set; }

        /// <summary>
        /// 机构形象照片
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }

        /// <summary>
        /// 机构创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public short? DataState { get; set; }
        public string NationalCode { get; set; }
        public string NationalName { get; set; }
    }

}
