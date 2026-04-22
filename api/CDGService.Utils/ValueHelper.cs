using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Utils
{
    public static class ValueHelper
    {
        /// <summary>
        ///double 默认取两位有效小数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static double? ToRound(this double? value, int pos = 2)
        {
            if (!value.HasValue)
                return value;
            return Math.Round(value.Value, pos);
        }
        /// <summary>
        /// 默认保留两位小数，四舍五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal ToRounds(this decimal value, int pos = 2)
        {
            return Math.Round(value, pos);
        }
        /// <summary>
        /// 保留小数，位数不足在后自动补位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal ToFillRounds(this decimal value, int pos = 2)
        {
            var newvalue = Math.Round(value, pos);
            newvalue = Convert.ToDecimal(newvalue.ToString().PadRight(pos, '0'));

            return newvalue;
        }
        /// <summary>
        /// 默认取两位有效小数且不四舍五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns>返回保留两位小数且不四舍五入</returns>
        public static decimal ToFloorRound(this decimal value, int pos = 2)
        {
            int tes = 100;
            decimal temp = 100.00M;
            if (pos == 3) { tes = 1000; temp = 100.000M; }
            if (pos == 4)
            { tes = 10000; temp = 10000.0000M; }
            return Math.Floor(value * tes) / temp;
        }

        public static string NumtoChinese(this decimal s)
        {
            s = Math.Round(s, 2);//四舍五入到两位小数，即分
            string[] n = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            //数字转大写
            string[] d = { "", "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿" };
            //不同位置的数字要加单位
            List<string> needReplace = new List<string> { "零拾", "零佰", "零仟", "零万", "零亿", "亿万", "零元", "零零", "零角", "零分" };
            List<string> afterReplace = new List<string> { "零", "零", "零", "万", "亿", "亿", "元", "零", "", "" };//特殊情况用replace剔除
            string e = s % 1 == 0 ? "整" : "";//金额是整数要加一个“整”结尾
            string re = "";
            Int64 a = (Int64)(s * 100);
            int k = 1;
            while (a != 0)
            {//初步转换为大写+单位
                re = n[a % 10] + d[k] + re;
                a = a / 10;
                k = k < 11 ? k + 1 : 4;
            }
            string need = needReplace.Where(tb => re.Contains(tb)).FirstOrDefault<string>();
            while (need != null)
            {
                int i = needReplace.IndexOf(need);
                re = re.Replace(needReplace[i], afterReplace[i]);
                need = needReplace.Where(tb => re.Contains(tb)).FirstOrDefault<string>();
            }//循环排除特殊情况
            re = re == "" ? "" : re + e;
            return re;
        }

      
        public static string ToASCII(string str)
        {
            byte[] textbuf = Encoding.Default.GetBytes(str);

            string textAscii = string.Empty;//用来存储转换过后的ASCII码

            for (int i = 0; i < textbuf.Length; i++)
            {
                textAscii += textbuf[i].ToString("X");
            }
            return textAscii;
        }

        public static string ToTextStr(string textAscii)
        {
            //将ASCII字符转换为汉字

            string textStr = string.Empty;

            int k = 0;//字节移动偏移量

            byte[] buffer = new byte[textAscii.Length / 2];//存储变量的字节

            for (int i = 0; i < textAscii.Length / 2; i++)
            {

                //每两位合并成为一个字节

                buffer[i] = byte.Parse(textAscii.Substring(k, 2), System.Globalization.NumberStyles.HexNumber);

                k = k + 2;

            }
            //将字节转化成汉字 
            textStr = Encoding.Default.GetString(buffer);
            return textStr;
        }


        /// <summary>
        /// 时间种子生成随机code（长度8位）
        /// </summary>
        /// <param name="myChar">业务字母</param>
        /// <param name="min">最小范围</param>
        /// <param name="max">最大范围</param>
        /// <returns></returns>
        public static string TimeNowRandom(char myChar, int min = 1000000, int max = 10000000)
        {
            var rd = new Random();//DateTime.Now.Second
            return $"{myChar}{rd.Next(min, max)}"; // (生成min-max之间的随机数，不包括max)
        }

    }



}
