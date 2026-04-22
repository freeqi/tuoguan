using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Helper
{

    /// <summary>
    /// 处理编码字符串或字符串
    /// </summary>
    public static class EncodingStrOrByte
    {
        /// <summary>
        /// 编码方式
        /// </summary>
        public enum EncodingType { UTF7, UTF8, UTF32, Unicode, BigEndianUnicode, ASCII, GB2312 };
        /// <summary>
        /// 处理指定编码的字符串，转换字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encodingType"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string str, EncodingType encodingType)
        {
            byte[] bytes = null;
            switch (encodingType)
            {
                //将要加密的字符串转换为指定编码的字节数组
                case EncodingType.UTF7:
                    bytes = Encoding.UTF7.GetBytes(str);
                    break;
                case EncodingType.UTF8:
                    bytes = Encoding.UTF8.GetBytes(str);
                    break;
                case EncodingType.UTF32:
                    bytes = Encoding.UTF32.GetBytes(str);
                    break;
                case EncodingType.Unicode:
                    bytes = Encoding.Unicode.GetBytes(str);
                    break;
                case EncodingType.BigEndianUnicode:
                    bytes = Encoding.BigEndianUnicode.GetBytes(str);
                    break;
                case EncodingType.ASCII:
                    bytes = Encoding.ASCII.GetBytes(str);
                    break;
                case EncodingType.GB2312:
                    bytes = Encoding.Default.GetBytes(str);
                    break;
            }
            return bytes;
        }

        /// <summary>
        /// 处理指定编码的字节数组，转换字符串
        /// </summary>
        /// <param name="myByte"></param>
        /// <param name="encodingType"></param>
        /// <returns></returns>
        public static string GetString(byte[] myByte, EncodingType encodingType)
        {
            string str = null;
            switch (encodingType)
            {
                //将要加密的字符串转换为指定编码的字节数组
                case EncodingType.UTF7:
                    str = Encoding.UTF7.GetString(myByte);
                    break;
                case EncodingType.UTF8:
                    str = Encoding.UTF8.GetString(myByte);
                    break;
                case EncodingType.UTF32:
                    str = Encoding.UTF32.GetString(myByte);
                    break;
                case EncodingType.Unicode:
                    str = Encoding.Unicode.GetString(myByte);
                    break;
                case EncodingType.BigEndianUnicode:
                    str = Encoding.BigEndianUnicode.GetString(myByte);
                    break;
                case EncodingType.ASCII:
                    str = Encoding.ASCII.GetString(myByte);
                    break;
                case EncodingType.GB2312:
                    str = Encoding.Default.GetString(myByte);
                    break;
            }
            return str;
        }
    }
}
