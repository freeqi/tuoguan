using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto.AutoCenterPut
{
    public class SysPublishInfoInput : SysPublishInfo
    {

    }

    public class SysPublishInfoOutPut  
    {

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 系统中文名称
        /// </summary>
        public string SysCHSName { get; set; }
        /// <summary>
        /// 系统英文名称
        /// </summary>
        public string SysUSName { get; set; }
        /// <summary>
        /// 开发版本(svn-tag)
        /// </summary>
        public string DevTagsVersion { get; set; }
        /// <summary>
        /// 系统发布版本(针对中心用户)
        /// </summary>
        public string SysVersion { get; set; }
        /// <summary>
        /// 系统发布时间
        /// </summary>
        public DateTime PublishDate { get; set; }
        /// <summary>
        /// 系统类型
        /// </summary>
        public int SysType { get; set; }
        /// <summary>
        /// 更新修改详情
        /// </summary>
        public string UpdateInfo { get; set; }
        /// <summary>
        /// 发布中心
        /// </summary>
        public string UpdateCenter { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }
        /// <summary>
        /// 是否推送
        /// </summary>
        public bool isPush { get; set; }

    }


    public class AppPublishInfoInput : AppPublishInfo
    {

    }

    public class AppPublishInfoOutPut : AppPublishInfo
    {

    }

    public class SysPublishInfoQuery
    {
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
