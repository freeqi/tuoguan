using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class DocumentOutput
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
      //  public string FileUrl { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string  PublishTime { get; set; }
        /// <summary>
        /// 发布人ID
        /// </summary>
        public string  PublishPersonId { get; set; }

        /// <summary>
        /// 发布人
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string  EntryTime { get; set; }
        /// <summary>
        /// 最近修改时间
        /// </summary>
        public string  LastModifyTime { get; set; }
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
        /// <summary>
        /// 下载次数
        /// </summary>
        public int DownloadCount { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public double? FileSize { get; set; }
        /// <summary>
        /// 类别中文名
        /// </summary>
        public string  StringCatalog { get; set; }
        public DocumentCatalog catalog { get; set; }

        /// <summary>
        /// 级别中文名
        /// </summary>
        public string StringKnowledgeType { get; set; }
        public KnowledgeType knowledgeType { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FielType FileType { get; set; }
    }


    public class DocKnowledgeOutPut
    {
        public KnowledgeType value { get; set; }
        public string label { get; set; }

        public int count { get; set; }
    }
}
