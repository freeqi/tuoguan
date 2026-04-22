using CDGService.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Enums
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        //1001：数据访问日志
        //1002：数据更新日志
        //1003：数据删除日志
        //2001：用户登录日志
        //4001：异常日志
        //5001：采集日志
        /// <summary>
        /// 数据访问日志
        /// </summary>
        [ChineseEnum("数据访问")]
        DataAccess = 1001,
        /// <summary>
        /// 数据更新日志
        /// </summary>
        [ChineseEnum("数据更新")]
        DataUpdate = 1002,
        /// <summary>
        /// 数据删除日志
        /// </summary>
        [ChineseEnum("数据删除")]
        DataDelete = 1003,
        /// <summary>
        /// 数据添加日志
        /// </summary>
        [ChineseEnum("数据添加")]
        DataAdded = 1004,
        /// <summary>
        /// 用户登录日志
        /// </summary>
        UserLogin = 2001,
        /// <summary>
        /// 密码修改日志
        /// </summary>
        [ChineseEnum("修改密码")]
        PassWordModify = 2002,
        /// <summary>
        /// 异常日志
        /// </summary>
        Exception = 4001,

        /// <summary>
        /// 采集日志
        /// </summary>
        [ChineseEnum("数据采集")]
        CollectLog = 5001,


    }

    /// <summary>
    /// 响应模型类别码
    /// </summary>
    public enum CodeType { Success = 200, Error = 500, NullData = 300, SystemException = 360, NoAuthority = 403, NotFind = 404 };
}
