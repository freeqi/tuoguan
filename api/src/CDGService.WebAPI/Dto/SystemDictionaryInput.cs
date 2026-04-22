using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 数据字典输入实体
    /// </summary>
    public class SystemDictionaryInput
    {
        /// <summary>
        /// ID
        /// </summary>
        public  string Id { get; set; }
        /// <summary>
        ///  名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 数据类型ID
        /// </summary>
        public string  TypeId { get; set; }
        /// <summary>
        /// 下拉排序号
        /// </summary>
        public int ShowSortNo { get; set; }
       public int DataState { get; set; }
    }

    public class DictionarySearchInput
    {
        public string  Id { get; set; }
        public string  TypeId { get; set; }
        public string typeCode { get; set; }
        public string Name { get; set; }
    }
}
