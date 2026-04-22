using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas 
{

    public class UseWay:ISoftDelete
    {

        public string Id { get; set; }
        /// <summary>
        /// 系统编码W开头
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 代码值
        /// </summary>
        public string WayCode { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string WayDescribe { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public WayEnum WayType { get; set; }
        public int  DataState { get; set; }
        public string Remark { get; set; }
        public string  Founder { get; set; }
        public DateTime  FounderDate { get; set; }
        public string  Modifier { get; set; }
        public DateTime  ModifierDate { get; set; }
        public bool IsDelete { get; set; }
    }
}
