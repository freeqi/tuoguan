using CDGService.Utils;
using CDGService.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CDGService.Data.Helper
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 转换为枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns>枚举值</returns>
        public static T ToEnum<T>(this int source)
        {
            var result = source.ToString().ToEnum<T>();
            return result;
        }

        /// <summary>
        /// 转换为枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns>枚举值</returns>
        public static T ToEnum<T>(this string source)
        {
            var result = (T)Enum.Parse(typeof(T), source);
            return result;
        }

        public static List<EnumberEntity> EnumToList<T>(int count, int index = 0)
        {
            List<EnumberEntity> list = new List<EnumberEntity>();
            int i = 0;
            int NowIndex = 0;
            if (count == 0) count = 9999;
            foreach (Enum e in Enum.GetValues(typeof(T)))
            {
               
                if (NowIndex == index && count>i)
                {
                    EnumberEntity m = new EnumberEntity();
                    ChineseEnumAttribute attribute = e.GetAttribute<ChineseEnumAttribute>();
                    if (attribute == null)
                    {
                        m.Desction = e.ToString();
                    }
                    else
                    {
                        m.Desction = attribute.ChineseName;
                    }


                    m.EnumValue = Convert.ToInt32(e);
                    m.EnumName = e.ToString();
                    // m.Desction = e.ToChinese();
                    list.Add(m);
                    i++;
                }
                else
                    NowIndex++;
            }
            return list;
        }
    }
}

