using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Utils.Attributes
{
    /// <summary>
    ///  枚举值的中文名称特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ChineseEnumAttribute : Attribute
    {
        /// <summary>
        ///  中文名称 
        /// </summary>
        public string ChineseName
        {
            get;
            private set;
        }

        /// <summary>
        /// 枚举值的中文名称
        /// </summary>
        /// <param name="chinese"></param>
        public ChineseEnumAttribute(string chinese)
        {
            this.ChineseName = chinese;
        }
    }
}
