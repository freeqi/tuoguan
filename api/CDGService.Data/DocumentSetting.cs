using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.Data
{
    public class DocumentSetting
    {

        public string DocumentRoot { get; set; }
        public string PdfRoot { get; set; }

		public string ApkRoot { get; set; }

        public string ImageRoot { get; set; }

        /// <summary>
        /// 病历目录
        /// </summary>
        public string MedicalRecords { get; set; }
        /// <summary>
        /// Excle导出目录
        /// </summary>
        public string ExportExcel { get; set; }
        public string BaseUrl { get; set; }
    }
}
