using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class DocumentInput
    {
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
     //   public string CoverUrl { get; set; }       
        /// <summary>
        /// 类别
        /// </summary>
        public DocumentCatalog Catalog { get; set; }
        public KnowledgeType knowledgeType { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }
        /// <summary>
        /// 是否公开
        /// </summary>
        //  public bool IsPublish { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }


    }
}
