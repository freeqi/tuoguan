using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
namespace CDGService.Data.Helper
{
    public class DataTypeConvertHelper
    {
        /// <summary>
        /// Convert a List{T} to a DataTable.
        /// </summary>
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            int Length = props.Where(t => !t.GetMethod.IsVirtual).Count();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetMethod.IsVirtual)
                    continue;
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[Length];
                int i = 0;
                foreach (var pitem in props)
                {
                    if (pitem.GetMethod.IsVirtual)
                        continue;
                    values[i] = pitem.GetValue(item, null);
                    i++;
                }
                //for (int i = 0; i < props.Length; i++)
                //{
                //    if (props[i].GetMethod.IsVirtual)
                //        continue;
                //    values[i] = props[i].GetValue(item, null);
                //}

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
    }
}
