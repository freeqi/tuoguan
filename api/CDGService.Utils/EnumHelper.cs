using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDGService.Utils.Attributes;

namespace CDGService.Utils
{
    public static class EnumHelper
    {
        /// <summary>
		/// 返回指定属性类的实例
		/// </summary>
		/// <param name="enumSubitem">枚举类子项</param>
		/// <param name="attributeType">属性类型</param>
		/// <returns></returns>
		private static object GetAttributeClass(this Enum enumSubitem, Type attributeType)
        {
            object[] customAttributes = enumSubitem.GetType().GetField(enumSubitem.ToString()).GetCustomAttributes(attributeType, false);
            if (customAttributes.Length == 0)
            {
                return null;
            }
            return customAttributes[0];
        }

        /// <summary>
		///  返回指定的属性
		/// </summary>
		/// <param name="enumSubitem">属性项</param>
		/// <typeparam name="T">属性类型</typeparam>
		/// <returns>返回得到的项</returns>
		public static T GetAttribute<T>(this Enum enumSubitem) where T : Attribute
        {
            return (T)((object)enumSubitem.GetAttributeClass(typeof(T)));
        }

        /// <summary>
		/// 返回枚举的描述信息，如果没有描述，返回枚举本身字符串
		/// </summary>
		/// <param name="enumSubitem"></param>
		/// <returns></returns>
		public static string ToChinese(this Enum enumSubitem)
        {
            string result;
            try
            {
                ChineseEnumAttribute attribute = enumSubitem.GetAttribute<ChineseEnumAttribute>();
                if (attribute == null)
                {
                    result = enumSubitem.ToString();
                }
                else
                {
                    result = attribute.ChineseName;
                }
            }
            catch (Exception)
            {
                result = enumSubitem.ToString();
            }
            return result;
        }


      
    }
}
