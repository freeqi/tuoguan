using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 中心端版本发布记录
    /// </summary>
    public class SysPublishInfo
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

        public virtual List<CenterPublishLog> CenterPublishLogs { get; set; }

    }


    public class CenterPublishLog
    {
        public string Id { get; set; }
        public string CenterPublishId { get; set; }
        [ForeignKey("CenterPublishId")]
        public virtual SysPublishInfo sysPublishInfo { get; set; }
        public string CenterId { get; set; }
        public DateTime? PublishTime { get; set; }

        public bool isPush { get; set; }
    }


    /// <summary>
    /// 中心端APP发布记录
    /// </summary>

    public class AppPublishInfo
    {

        public string Id { get; set; }
        public string AppCHSName { get; set; }
        public string AppUSName { get; set; }
        public string AppVersion { get; set; }
        public string AppDownUrl { get; set; }
        public string AppWgtDownUrl { get; set; }
        public int? IsForcedUpdate { get; set; }
        public int? AppUpdateType { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }

        public bool isPush { get; set; }

    }
}
