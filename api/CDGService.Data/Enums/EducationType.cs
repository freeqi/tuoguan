using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Enums
{
    /// <summary>
    /// 学历
    /// </summary>
    public enum EducationType
    {
        /// <summary>
        /// 其他
        /// </summary>
        Other=0,
        /// <summary>
        /// 小学
        /// </summary>
        Primary = 1,
        /// <summary>
        /// 初中
        /// </summary>
        Junior = 2,
        /// <summary>
        /// 高中
        /// </summary>
        Senior = 3,
        /// <summary>
        /// 中专
        /// </summary>
        Secondary = 4,
        /// <summary>
        /// 大专
        /// </summary>
        JuniorCollege = 5,
        /// <summary>
        /// 本科
        /// </summary>
        RegularCollege = 6,
        /// <summary>
        /// 硕士
        /// </summary>
        Master=7,
        /// <summary>
        /// 博士
        /// </summary>
        Doctor=8,
    }
}
