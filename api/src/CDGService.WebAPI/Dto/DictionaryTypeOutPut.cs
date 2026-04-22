using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 字典类型输出实体
    /// </summary>
    public class DictionaryTypeOutPut
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id  { get; set; }
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
        /// 是否默认存在
        /// </summary>
        public bool Isdef { get; set; }
        /// <summary>
        /// 可否修改
        /// </summary>
        public bool IsAdjust { get; set; }
    }
}
