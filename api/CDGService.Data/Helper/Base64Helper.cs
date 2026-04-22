
using CDGService.Data.Helper;
using System;

namespace CDGService.Data.Helper
{
    /// <summary>
    /// Base64-编码解码助手
    /// </summary>
    public static class Base64Helper
    {
        /// <summary>
        /// 编码：string to base64
        /// </summary>
        /// <param name="str">待编码字符串</param>
        /// <param name="encodingType">编码方式</param>
        /// <returns>string：base64编码字符串</returns>
        public static string Encode(string str, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            byte[] byteData = EncodingStrOrByte.GetBytes(str, encodingType);
            return Convert.ToBase64String(byteData, 0, byteData.Length);
        }

        /// <summary>
        /// 解码：base64 to string
        /// </summary>
        /// <param name="str">base64编码字符串</param>
        /// <param name="encodingType">编码方式</param>
        /// <returns>string：base64解码字符串</returns>
        public static string Decode(string str, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            byte[] byteData = Convert.FromBase64String(str);
            return EncodingStrOrByte.GetString(byteData, encodingType);
        }
    }
}
