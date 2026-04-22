using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace CDGService.Utils
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>  
        /// 二进制压缩  
        /// </summary>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static byte[] Compress(this byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>  
        /// 二进制解压缩  
        /// </summary>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static byte[] Decompress(this byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                MemoryStream msreader = new MemoryStream();
                byte[] buffer = new byte[0x1000];
                while (true)
                {
                    int reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static string CompressString(this string str)
        {
            string compressString = "";
            byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            byte[] compressAfterByte = Compress(compressBeforeByte);
            //compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);    
            compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;
        }

        public static string DecompressString(this string str)
        {
            string compressString = "";
            //byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);    
            byte[] compressBeforeByte = Convert.FromBase64String(str);
            byte[] compressAfterByte = Decompress(compressBeforeByte);
            compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            return compressString;
        }



        /// <summary>
        /// 处理成ID值字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToStrsString(List<string> list, bool isRetEmptyString = true)
        {
            if (list == null || list.Count <= 0)
            {
                if (isRetEmptyString)
                    return string.Empty;
                return "' '";
            }
            else
            {
                var tempStr = string.Empty;
                var count = list.Count;
                if (count == 1)
                {
                    return "\'" + list[0] + "\'";
                }
                else
                {
                    for (var i = 0; i < count; i++)
                    {
                        if (i != count - 1)
                        {
                            tempStr += "\'" + list[i] + "\'" + ",";
                        }
                        else
                        {
                            tempStr += "\'" + list[i] + "\'";
                        }
                    }
                    return tempStr;
                }
            }
        }


        public static string GetRandomPwd(string CenterCode)
        {
            var rd = new Random();
            
         
            var domNum = rd.Next(100001,999999);
           string pwd = $"{CenterCode.Substring(0, 6)}{domNum}";
            //159863abc3d6
             
            return pwd; //  
        }

    }
}
