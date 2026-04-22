using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CDGService.Data.Helper
{
    public static class Md5Helper
    {

        /// <summary>
        /// MD5 hash加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD5(this string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var md5 = new MD5CryptoServiceProvider();
            var result = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(s.Trim())));
            return result;
        }


        /// <summary>
        /// 从64位字符串解码
        /// </summary>
        /// <param name="base64string"></param>
        /// <returns></returns>
        public static byte[] Frombase64TobBytes(this string base64string)
        {
            return Convert.FromBase64String(base64string);
        }

        /// <summary>
        /// 从64位字符串解密
        /// </summary>
        /// <param name="base64string"></param>
        /// <returns></returns>
        public static string Frombase64ToString(this string base64string)
        {
            return System.Text.Encoding.Default.GetString(base64string.Frombase64TobBytes());
        }

        /// <summary>
        /// 数组转64位字符串
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] datas)
        {
            string strPath = Convert.ToBase64String(datas, 0, datas.Length);
            return strPath;
        }


        /// <summary>
        /// 字符串转成base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64String(this string str)
        {
            System.Text.Encoding encode = System.Text.Encoding.ASCII;
            byte[] bytedata = encode.GetBytes(str);
            return bytedata.ToBase64String();
        }



        #region 具体实现，参数是string类型的  

        #region AES密钥、向量处理
        private static readonly string _AesKey = "12345678900000001234567890000000"; //32位十六进制数作为秘钥
        private static readonly string _AesIv = "1234567890000000"; //16位十六进制数作为秘钥偏移向量
        #endregion

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="palinData">明文</param>
        /// <returns></returns>
        public static string Encrypt(this string palinData)
        {
            return PrivateEncrypt(palinData, _AesKey, _AesIv);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedData">密文</param>
        /// <returns></returns>
        public static string Decrypt(this string encryptedData)
        {
            return PrivateDecrypt(encryptedData, _AesKey, _AesIv);
        }

        /// <summary>
        /// 密钥检查
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns>bool</returns>
        private static bool CheckKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            if (16.Equals(key.Length) || 24.Equals(key.Length) || 32.Equals(key.Length))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 密钥偏移向量检查
        /// </summary>
        /// <param name="iv">密钥偏移向量</param>
        /// <returns>bool</returns>
        private static bool CheckIv(string iv)
        {
            if (string.IsNullOrWhiteSpace(iv))
                return false;
            if (16.Equals(iv.Length))
                return true;
            else
                return false;
        }
        /// <summary>  
        ///  加密 参数：string  
        /// </summary>  
        /// <param name="palinData">明文</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <param name="encodingType">编码方式</param>  
        /// <returns>string：密文</returns>  
        private static string PrivateEncrypt(string palinData, string key, string iv, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            if (string.IsNullOrWhiteSpace(palinData)) return null;
            if (!(CheckKey(key) && CheckIv(iv))) return palinData;

            byte[] keyBytes = EncodingStrOrByte.GetBytes(key.Substring(0, 32), encodingType);
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = EncodingStrOrByte.GetBytes(key.Substring(0, 32), encodingType);
                aesAlg.IV = EncodingStrOrByte.GetBytes(iv.Substring(0, 16), encodingType);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(palinData);
                        }
                        byte[] bytes = msEncrypt.ToArray();
                        return ByteArrayToHexString(bytes);
                    }
                }
            }
        }

        /// <summary>  
        ///  解密 参数：string  
        /// </summary>  
        /// <param name="encryptedData">密文</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <param name="encodingType">编码方式</param>  
        /// <returns>string：明文</returns>  
        private static string PrivateDecrypt(string encryptedData, string key, string iv, EncodingStrOrByte.EncodingType encodingType = EncodingStrOrByte.EncodingType.UTF8)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(encryptedData)) return null;
                if (!(CheckKey(key) && CheckIv(iv))) return encryptedData;

                byte[] inputBytes = HexStringToByteArray(encryptedData);
                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    aesAlg.Key = EncodingStrOrByte.GetBytes(key.Substring(0, 32), encodingType);
                    aesAlg.IV = EncodingStrOrByte.GetBytes(iv.Substring(0, 16), encodingType);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream(inputBytes))
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                            {
                                return srEncrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                return "";
            }

        }
        #endregion
        #region 辅助方法
        /// <summary>
        /// 将指定的16进制字符串转换为byte数组
        /// </summary>
        /// <param name="s">16进制字符串(如：“7F 2C 4A”或“7F2C4A”都可以)</param>
        /// <returns>16进制字符串对应的byte数组</returns>
        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// 将一个byte数组转换成一个格式化的16进制字符串
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <returns>格式化的16进制字符串</returns>
        private static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                //16进制数字
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                //16进制数字之间以空格隔开
                //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return sb.ToString().ToLower();
        }
        #endregion

    }
}
