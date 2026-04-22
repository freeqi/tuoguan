using CDGService.Data.Helper;
using CDGService.Store.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace CDGService.Store
{
    public class XmlSql
    {
        /// <summary>
        /// 缓存时间：S秒
        /// </summary>
        private const int cacheSeconds = 60 * 60 * 10; //10小时

        private string _GetWebCurrentPath
        {
            get
            {
                return AppContext.BaseDirectory;//获取web发布物理根路径
            }
        }

        /// <summary>
        /// xml文件夹路径获取
        /// </summary>
        /// <returns>string</returns>
        private string GetXmlConfigPath()
        {
            string xmlConfigPath = string.Empty;
            string physicalWebRootPath = _GetWebCurrentPath; //获取（物理）应用程序Web跟路径
            xmlConfigPath = CustomCacheHelper.GetSingleObj().Get<string>(physicalWebRootPath); //读取缓存
            if (string.IsNullOrWhiteSpace(xmlConfigPath))
            {
                xmlConfigPath = physicalWebRootPath + @"Config\SystemConfig.xml"; //SqlXml配置文件夹路径
                CustomCacheHelper.GetSingleObj().SetForSeconds(physicalWebRootPath, xmlConfigPath.Trim(), cacheSeconds); //设置缓存s秒
            }
            return xmlConfigPath.Trim();
        }

        /// <summary>
        /// 获取查询sql
        /// </summary>
        /// <param name="xmlSqlParameter">xmlSql文件中的节点参数</param>
        /// <returns>string：QuerySql</returns>
        protected string GetQuerySql(XmlSqlParameter xmlSqlParameter)
        {
            //xml文件路径获取
            string sqlXmlConfigPath = GetXmlConfigPath();
            string key = "sqlXmlQueryPath";
            string sqlXmlQueryPath = CustomCacheHelper.GetSingleObj().Get<string>(key); //读取缓存
            if (string.IsNullOrWhiteSpace(sqlXmlQueryPath))
            {
                sqlXmlQueryPath = XmlHelper.GetSingleObj().GetXmlConfig(sqlXmlConfigPath, "SqlQueryPath", "sql查询配置文件路径");
                sqlXmlQueryPath = _GetWebCurrentPath + sqlXmlQueryPath.Trim();
                CustomCacheHelper.GetSingleObj().SetForSeconds(key, sqlXmlQueryPath.Trim(), cacheSeconds); //设置缓存s秒
            }
            return XmlHelper.GetSingleObj().GetXmlSql(sqlXmlQueryPath, xmlSqlParameter.ClassName, xmlSqlParameter.SqlFunName, xmlSqlParameter.SqlFunRemark, XmlHelper.QueryType.LinqToXmlSql);
        }

        /// <summary>
        /// 获取非查询sql
        /// </summary>
        /// <param name="xmlSqlParameter">xmlSql文件中的节点参数</param>
        /// <returns>string：NoQuerySql</returns>
        protected string GetNoQuerySql(XmlSqlParameter xmlSqlParameter)
        {
            //xml文件路径获取
            string sqlXmlConfigPath = GetXmlConfigPath();
            string key = "sqlXmlNoQueryPath";
            string sqlXmlNoQueryPath = CustomCacheHelper.GetSingleObj().Get<string>(key); //读取缓存
            if (string.IsNullOrWhiteSpace(sqlXmlNoQueryPath))
            {
                sqlXmlNoQueryPath = XmlHelper.GetSingleObj().GetXmlConfig(sqlXmlConfigPath, "SqlNoQueryPath", "sql非查询配置文件路径");
                sqlXmlNoQueryPath = _GetWebCurrentPath + sqlXmlNoQueryPath.Trim();
                CustomCacheHelper.GetSingleObj().SetForSeconds(key, sqlXmlNoQueryPath.Trim(), cacheSeconds); //设置缓存s秒
            }
            return XmlHelper.GetSingleObj().GetXmlSql(sqlXmlNoQueryPath, xmlSqlParameter.ClassName, xmlSqlParameter.SqlFunName, xmlSqlParameter.SqlFunRemark, XmlHelper.QueryType.LinqToXmlLamda);
        }

        /// <summary>
        /// XML-sql语句参数校验
        /// </summary>
        /// <param name="className">类模块名称</param>
        /// <param name="sqlFunName">当前执行sql的自定义方法名称</param>
        /// <param name="sqlFunRemark">sql语句逻辑描述</param>
        /// <returns>XmlSqlParameter</returns>
        protected XmlSqlParameter GetXmlSqlParameter(string className,string sqlFunName,string sqlFunRemark)
        {
            if (string.IsNullOrWhiteSpace(className)|| string.IsNullOrWhiteSpace(sqlFunName)|| string.IsNullOrWhiteSpace(sqlFunRemark))
            {
                throw new FormatException("XmlSqlParameter不能为空，请填写对应的描述信息！");
            }
            else
            {
                return new XmlSqlParameter()
                {
                    ClassName = className,
                    SqlFunName = sqlFunName,
                    SqlFunRemark = sqlFunRemark
                };
            }
        }

        /// <summary>
        /// 检测数据表是否有数据【true 序列化实体模型为列表集合| false 空列表集合】
        /// </summary>
        /// <typeparam name="T">反序列化模型</typeparam>
        /// <param name="dt">实体数据表对象</param>
        /// <returns>Tuple<bool, List<T>></returns>
        protected Tuple<bool, List<T>> GetTupleByList<T>(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                string dtJson = JsonHelper.ObjectToJson(dt);
                var listT = JsonHelper.JsonToT<List<T>>(dtJson);
                return Tuple.Create(true, listT);
            }
            else
            {
                var listT = new List<T>();
                return Tuple.Create(false, listT);
            }
        }

        /// <summary>
        /// 检测数据表是否有数据【true 实体模型| false 空】
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected Tuple<bool, T> GetTuple<T>(DataTable dt)
        {
            var t = default(T);
            if (dt != null && dt.Rows.Count == 1)
            {
                string dtJson = JsonHelper.ObjectToJson(dt);
                var listT = JsonHelper.JsonToT<List<T>>(dtJson);
                foreach (T item in listT)
                {
                    t = item;
                }
                return Tuple.Create(true, t);
            }
            return Tuple.Create(false, t);
        }

        /// <summary>
        /// 检测数据List<T>是否有数据【true 序列化实体模型为列表集合| false 空列表集合】
        /// </summary>
        /// <typeparam name="T">反序列化模型</typeparam>
        /// <param name="dt">实体数据表对象</param>
        /// <returns>Tuple<bool, List<T>></returns>
        protected Tuple<bool, List<T>> GetTupleByListT<T>(List<T> tList)
        {
            if (tList.Count > 0 && tList != null)
            {
                return Tuple.Create(true, tList);
            }
            else
            {
                return Tuple.Create(false, tList);
            }
        }

        /// <summary>
        /// 获取某个对象的[公有属性]的名称,类型,值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Dictionary<int, string> GetPropertyValue<T>(T obj, bool isGetValue, out List<SqlParameter> sqlParameters)
        {
            var nameDic = new Dictionary<int, string>();
            sqlParameters = new List<SqlParameter>();
            if (obj == null)
            {
                return nameDic;
            }
            Type t = obj.GetType();//获得该类的Type           
            var i = 0;
            //再用Type.GetProperties获得PropertyInfo[],然后就可以用foreach 遍历了
            foreach (PropertyInfo pi in t.GetProperties())
            {
                var name = pi.Name;//获得属性的名字,后面就可以根据名字判断来进行些自己想要的操作
                var value = pi.GetValue(obj, null);//用pi.GetValue获得值
                nameDic.Add(i, name.ToString());
                if (isGetValue)
                {
                    sqlParameters.Add(new SqlParameter($"@{name}", value));
                }
                i++;
            }
            return nameDic;
            // return sb.ToString();
        }
        /// <summary>
        /// 拼接单表根据条件获取所有数据的sql
        /// </summary>
        /// <param name="entity">需查询table的Model</param>
        /// <param name="tableName">需查询表名</param>
        /// <param name="sqlWhere">查询所需的条件【断言】</param>
        /// <param name="showFieldNames">需要展示的字段名，参数格式为"[fieldname1],[fieldname2],……[fieldnameN]"</param>
        /// <returns>sql</returns>
        public string GetAllSingleTableSql<T>(T entity, string tableName, string sqlWhere, string showFieldNames = null)
        {
            string names = string.Empty;
            if (!string.IsNullOrWhiteSpace(showFieldNames))
            {
                names = showFieldNames;
            }
            else
            {
                var sqlParameters = new List<SqlParameter>();
                var dic = GetPropertyValue(entity, false, out sqlParameters);

                foreach (var name in dic.Values)
                {

                    if (name.Equals(dic[dic.Values.Count - 1]))
                    {
                        names += $"[{name}]";
                    }
                    else
                    {
                        names += $"[{name}],";
                    }
                }

            }
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append($" select {names}");
            sqlStr.Append($" from [{tableName}]");
            sqlStr.Append($" where 1=1 {sqlWhere}");

            return sqlStr.ToString();
        }
    }
}
