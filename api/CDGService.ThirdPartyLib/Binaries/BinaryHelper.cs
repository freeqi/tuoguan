using System;
using System.IO;
using System.Text;

namespace CDGService.ThirdPartyLib.Binaries
{
    /// <summary>
    /// 二进制帮助类
    /// </summary>
    public static class BinaryHelper
    {
        /// <summary>
        /// 获取原始二进制数据
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public static byte[] GetOriginByteDatas(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            long len = fi.Length;
            byte[] buffer;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                buffer = new byte[len];
                fs.Read(buffer, 0, (int)len);
            }
            return buffer;
        }

        /// <summary>
        /// 读取16位整数
        /// </summary>
        /// <param name="btData">二进制数据源</param>
        /// <param name="startIndex">读取数据索引</param>
        /// <returns>整数</returns>
        public static int ReadInt16(this byte[] btData, int startIndex)
        {
            var result = BitConverter.ToInt16(btData, startIndex);
            return result;
        }

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="btData">二进制数据源</param>
        /// <param name="startIndex">读取数据索引</param>
        /// <returns>整数</returns>
        public static int ReadInt32(this byte[] btData, int startIndex)
        {
            var result = BitConverter.ToInt32(btData, startIndex);
            return result;
        }

        /// <summary>
        /// 写入16位整数
        /// </summary>
        /// <param name="btData">二进制数据</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="source">数据源</param>
        public static void WriteInt16(this byte[] btData, int startIndex, short source)
        {
            var btSrc = BitConverter.GetBytes(source);
            Array.ConstrainedCopy(btSrc, 0, btData, startIndex, 2);
        }

        /// <summary>
        /// 写入32位整数
        /// </summary>
        /// <param name="btData">二进制数据</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="source">数据源</param>
        public static void WriteInt32(this byte[] btData, int startIndex, int source)
        {
            var btSrc = BitConverter.GetBytes(source);
            Array.ConstrainedCopy(btSrc, 0, btData, startIndex, 4);
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="btData">二进制数据源</param>
        /// <param name="startIndex">读取数据索引</param>
        /// <param name="length">读取字符数量</param>
        /// <returns>字符串</returns>
        public static string ReadString(this byte[] btData, int startIndex, int length)
        {
            var result = Encoding.Default.GetString(btData, startIndex, length);
            return result;
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="btData">二进制流</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="source">字符串数据</param>
        public static void WriteString(this byte[] btData, int startIndex, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return;
            }
            var btStr = Encoding.Default.GetBytes(source);
            Array.ConstrainedCopy(btStr, 0, btData, startIndex, source.Length);
        }
    }
}
