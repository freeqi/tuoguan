using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 库房目录
    /// </summary>
    public class WarehouseCatalog: ISoftDelete
    {
        public string   Id { get; set; }
        /// <summary>
        /// 系统编码 H
        /// </summary>
        public string HouseSysCode { get; set; }
        /// <summary>
        /// 人工编码-根据编码规范
        /// </summary>
        public string HouseWorkCode { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string    ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        public bool IsDelete { get; set; }

        public int? CataIndex { get; set; }

        public string FirstLetter { get; set; }
    }
}
