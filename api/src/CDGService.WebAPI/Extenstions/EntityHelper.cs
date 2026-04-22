using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Extenstions
{
    public class EntityHelper
    {
        public static void CoptyProperty<P, T>(P src, T target)
        {
            try
            {
                var srcTypes = src.GetType();//获得类型  
                var typed = typeof(T);
                foreach (PropertyInfo sp in srcTypes.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo dp in typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType)//判断属性名是否相同  
                        {

                            var data = sp.GetValue(src, null);
                            if (data + "" != "" && data + "" != "0")
                                dp.SetValue(target, sp.GetValue(src, null), null);//获得s对象属性的值复制给d对象的属性  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static void CoptyPropertys<P, T>(P src, T target)
        {
            try
            {
                //
                var srcTypes = src.GetType();//获得类型  
                var typed = typeof(T);
                foreach (PropertyInfo sp in srcTypes.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo dp in typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType)//判断属性名是否相同  
                        {

                            var data = sp.GetValue(src, null);
                            //   if (data + "" != "" && data + "" != "0")
                            dp.SetValue(target, sp.GetValue(src, null), null);//获得s对象属性的值复制给d对象的属性  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }



    }
}
