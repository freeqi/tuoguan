using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class DocumentSearchInput
    {
      public string Id { get; set; }
        /// <summary>
      /// 文件名称
      /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public DocumentCatalog Catalog { get; set; }
        public KnowledgeType knowledgeType { get; set; }

        public int PageSize { get; set; }
        public int PageNum { get; set; }
    }
}
