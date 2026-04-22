using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Helper
{
    public static class IdHelper
    {
        /// <summary>
        /// 返回16位字符串表示形式的值System.Guid实例
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string tostring16(this Guid guid)
        {
            long i = 1;
            foreach (byte b in guid.ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return $"{i - DateTime.Now.Ticks:x}";
        }



        public static long tolong19(this Guid id)
        {
            byte[] buffer = id.ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
        /// <summary>
        /// 返回32位字符串表示形式的值System.Guid实例
        /// 如：e0a953c3ee6040eaa9fae2b667060e09 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string tostring32(this Guid id)
        {
            return id.ToString("N");
        }
    }
}
