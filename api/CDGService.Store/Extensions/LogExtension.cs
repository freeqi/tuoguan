using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CDGService.Store.Extensions
{
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static class LogExtension
    {
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <typeparam name="TLog">日志类型</typeparam>
        /// <param name="logger">日志对象</param>
        /// <param name="operation">操作</param>
        /// <param name="ex">异常对象</param>
        /// <param name="extraInfo">额外信息</param>
        public static void LogException<TLog>(this ILogger<TLog> logger, string operation, Exception ex, string extraInfo = null)
            where TLog : class
        {
            var sbMessage = new StringBuilder($"操作[{operation}]时出现异常：{ex?.Message}{Environment.NewLine}StackTrace:{ex?.StackTrace}");
            if (!string.IsNullOrEmpty(extraInfo))
            {
                sbMessage.AppendLine(extraInfo);
            }
            logger.LogError(sbMessage.ToString());
        }

    }
}
