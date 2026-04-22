using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Enums
{
    /// <summary>
    /// 在职状态
    /// </summary>
    public enum WorkStateType
    {
        /// <summary>
        /// 其它
        /// </summary>
        Other = 0,
        /// <summary>
        /// 在岗
        /// </summary>
        Jobing = 1,
        /// <summary>
        /// 待岗
        /// </summary>
        AwaitJob = 2,
        /// <summary>
        /// 离职
        /// </summary>
        Dimission = 3,
    }
}
