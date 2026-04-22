using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Extenstions
{
    public static class HttpContextExtension
    {
        /// <summary>
        /// 扩展方法实现获取用户ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ip))
                ip = context.Connection.RemoteIpAddress.ToString();
            if (ip.IndexOf("::ffff:") != -1)
            {
                ip = ip.Substring(7);
            }
            return ip;
        }

        /// <summary>
        /// 获取当前服务器IP地址和端口号
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetServerIpv4AndPort(this HttpContext context)
        {
            int port = context.Connection.LocalPort;
            string ip4 = context.Connection.LocalIpAddress.MapToIPv4().ToString();
            return $"{ip4}:{port}";
        }
    }
}
