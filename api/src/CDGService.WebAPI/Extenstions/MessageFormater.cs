using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Extenstions
{
    /// <summary>
    /// 消息格式化字符串
    /// </summary>
    public static  class MessageFormater
    {
        /// <summary>
        /// 请提供参数{name}格式化字符串
        /// 请提供有效的**
        /// </summary>
        public static string PrameterNeedProvider(string paramName)
        {
            return $"请提供有效的{paramName}";
        }

        /// <summary>
        /// 参数{name}不存在格式化字符串
        /// **不存在
        /// </summary>
        public static string PrameterNeedExist(string paramName)
        {
            return $"{paramName}不存在";
        }

        /// <summary>
        /// 参数{name}格式不正确
        /// **格式不正确
        /// </summary>
        public static string PrameterFormateNotValid(string paramName)
        {
            return $"{paramName}格式不正确";
        }

        /// <summary>
        /// 参数{name}不存在格式化字符串
        /// **使用的**已经被占用
        /// </summary>
        public static string PrameterValueIsUsed(string paramName,string paramValue)
        {
            return $"{paramName}使用的[{paramValue}]已经被占用";
        }

        /// <summary>
        /// 数据库返回结果为空
        /// **返回结果为空
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string PrameterResultsIsNull(string paramName)
        {
            return $"{paramName}返回结果为空";
        }
    }
}
