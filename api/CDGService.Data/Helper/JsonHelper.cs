using Newtonsoft.Json;
using System.IO;

namespace CDGService.Data.Helper
{
    /// <summary>
    /// json互转对象Object
    /// </summary>
    public static class JsonHelper
    {
        #region 对象转json字符串
        /// <summary>
        /// 从一个对象信息生成Json串
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>string</returns>
        public static string ObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion

        #region json字符串转换对象
        /// <summary>
        /// 从一个Json串生成对象信息
        /// </summary>
        /// <param name="jsonString">string</param>
        /// <param name="obj">object</param>
        /// <returns>object</returns>
        public static object JsonToObject(string jsonString, object obj)
        {
            return JsonConvert.DeserializeObject(jsonString, obj.GetType());
        }

        /// <summary>
        /// 从一个Json串生成泛型对象信息
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns>泛型对象</returns>
        public static T JsonToT<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        #endregion


        #region jsno文件IO流操作
        /// <summary>
        /// 将json数据写入json文件
        /// </summary>
        /// <param name="jsonString">json数据</param>
        /// <param name="filePath">文件相对路径</param>
        /// <returns>string</returns>
        public static void JsonFileInputStream(string jsonString, string filePath, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (!(string.IsNullOrWhiteSpace(jsonString) || string.IsNullOrWhiteSpace(filePath)))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    //获得字节数组
                    byte[] byteData = EncodingStrOrByte.GetBytes(jsonString, encodingType);
                    //开始写入
                    fs.Write(byteData, 0, byteData.Length);
                    //清空缓冲区、关闭流
                    fs.Flush();
                }
            }
        }
        #endregion

    }
}
