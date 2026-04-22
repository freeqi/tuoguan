using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 字典数据
    /// </summary>
    public class SystemDictionary : ISoftDelete
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///  名称
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

        public string DCode { get; set; }
        public string TypeCode { get; set; }
        /// <summary>
        /// 数据类型ID
        /// </summary>
        public string  TypeId { get; set; }

        [ForeignKey(nameof(TypeId))]
        public virtual DictionaryType DictionaryType { get; set; }
        /// <summary>
        /// 下拉排序号
        /// </summary>
        public int ShowSortNo { get; set; }
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
        public DateTime? FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        public bool IsCentDic { get; set; }
    }
}
