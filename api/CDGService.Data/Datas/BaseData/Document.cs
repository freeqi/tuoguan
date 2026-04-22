using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDGService.Data.Enums;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 文件实体
    /// </summary>
    public class Document : ISoftDelete
    {
        public string Id { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 封面路径
        /// </summary>
        public string CoverUrl { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }
        /// <summary>
        /// 发布人ID
        /// </summary>
        public string PublishPersonId { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [ForeignKey("PublishPersonId")]
        public virtual User user { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime EntryTime { get; set; }
        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 是否公开
        /// </summary>
        public bool IsPublish { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 文档原文件路径
        /// </summary>
        public string DocOriginalUrl { get; set; }

        public int DownloadCount { get; set; }
        public long FileSize { get; set; }
        public DocumentCatalog Catalog { get; set; }

        public KnowledgeType knowledgeType { get; set; }
        public bool IsDelete { get; set; }

        public int? DataState { get; set; }
    }
}
