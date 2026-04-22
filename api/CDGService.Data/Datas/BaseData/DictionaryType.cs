using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 字典类型
    /// </summary>
    public class DictionaryType : ISoftDelete
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 是否默认存在
        /// </summary>
        public bool Isdef { get; set; }
        /// <summary>
        /// 可否修改
        /// </summary>
        public bool IsAdjust { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifierDate { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        //public int AgyId { get; set; }
    }
}
