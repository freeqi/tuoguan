using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Utils;

namespace CDGService.WebAPI.Extenstions
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// 编写普通日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void WriteCommLog(string content)
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Common",
                $"CDGServiceWebApi{DateTime.Now.ToString("yyyyMMdd")}.log");
            FileHelper.WriteLog(filename, content);
        }

        /// <summary>
        /// 编写普通日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void WriteCommLog(string content, string filename)
        {
            if (filename == "")
                filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Common",
                  $"CDGServiceWebApi{DateTime.Now.ToString("yyyyMMdd")}.log");
            FileHelper.WriteLog(filename, content);
        }

        /// <summary>
        /// 编写错误日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void WriteErrLog(string content)
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Error",
                $"Error{DateTime.Now.ToString("yyyyMMdd")}.log");
            FileHelper.WriteLog(filename, content);
        }
    }
}