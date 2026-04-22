using System;
using System.IO;
using System.Text;

namespace CDGService.Utils
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetContent(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException($"文件{filePath}不存在");
            }
            var result = string.Empty;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    result = sr.ReadToEnd();
                }
                fs.Close();
            }
            return result;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="content">内容</param>
        public static void WriteLog(string filename, string content)
        {
            if (filename == "")
            {
                filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Error",
            $"Error{DateTime.Now.ToString("yyyyMMdd")}.log");
            }
            var dir = new FileInfo(filename).Directory;
            if (dir == null)
            {
                return;
            }
            if (!dir.Exists)
            {
                dir.Create();
            }
            try
            {
                using (StreamWriter sw = new StreamWriter(filename, true))
                {
                    sw.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}]{content}");
                    sw.Dispose();
                    sw.Close();
                }
            }
            catch (Exception)
            {


            }

        }


    }
}
