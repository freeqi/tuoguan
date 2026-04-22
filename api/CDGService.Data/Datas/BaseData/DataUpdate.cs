using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
	/// <summary>
	/// 数据备份更新的参数
	/// </summary>
	public class DataUpdate
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///版本号 
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 文件存放的名字
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }


        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
    }
}
