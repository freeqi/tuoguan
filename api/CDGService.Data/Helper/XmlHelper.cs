using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CDGService.Data.Helper
{
    public class XmlHelper
    {
        #region 单例模式
        //创建私有化静态obj锁  
        private static readonly object _ObjLock = new object();
        //创建私有静态字段，接收类的实例化对象  
        private static volatile XmlHelper _XmlHelper = null;
        //构造函数私有化  
        private XmlHelper() { }
        //创建单利对象资源并返回  
        public static XmlHelper GetSingleObj()
        {
            if (_XmlHelper == null)
            {
                lock (_ObjLock)
                {
                    if (_XmlHelper == null)
                        _XmlHelper = new XmlHelper();
                }
            }
            return _XmlHelper;
        }
        #endregion

        /// <summary>
        /// 缓存时间：S秒
        /// </summary>
        private const int cacheSeconds = 60 * 60;

        #region XmlSql
        /// <summary>
        /// Linq查询类型
        /// </summary>
        public enum QueryType { LinqToXmlSql = 1, LinqToXmlLamda = 2 };
        /// <summary>
        /// 条件查询xml节点文本
        /// </summary>
        /// <param name="xmlFilePath">xml文件路径</param>
        /// <param name="className">节点名称</param>
        /// <param name="sqlFun">子节点名称</param>
        /// <param name="sqlFunRemark">子节点备注</param>
        /// <param name="queryType">QueryType：Linq查询类型</param>
        /// <returns>string：sql语句</returns>
        public string GetXmlSql(string xmlFilePath, string className, string sqlFun, string sqlFunRemark, QueryType queryType)
        {
            string sql = string.Empty;
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                return sql;

            string key = GetXmlSqlCacheKey(xmlFilePath, className, sqlFun, sqlFunRemark); //获取缓存key
            sql = CustomCacheHelper.GetSingleObj().Get<string>(key); //读取缓存
            if (string.IsNullOrWhiteSpace(sql))
            {
                XDocument doc = null;
                try
                {
                    doc = XDocument.Load(xmlFilePath); //将XML文件加载进来
                }
                catch (Exception e)
                {
                    throw new Exception("SystemConfig.xml文件路劲配置错误!"+e.Message);
                }

                XElement root = doc.Root; //获取到XML的根元素进行操作，SqlQuery

                switch (queryType)
                {
                    case QueryType.LinqToXmlSql:
                        {
                            var sqls = from item in root.Descendants(className)
                                       where item.Element(sqlFun).FirstAttribute.Value.Contains(sqlFunRemark)
                                       select item.Element(sqlFun).Value;
                            foreach (var item in sqls)
                            {
                                if (item != null)
                                    sql = item;
                            }
                        }
                        break;
                    case QueryType.LinqToXmlLamda:
                        {
                            var sqls = root.Descendants(className)  //定位节点 
                                   .Where(p => p.Element(sqlFun).FirstAttribute.Value.Contains(sqlFunRemark)) //条件筛选
                                   .Select(p => new { Val = p.Element(sqlFun).Value }); //取值
                            foreach (var item in sqls)
                            {
                                if (item != null)
                                    sql = item.Val;
                            }
                        }
                        break;
                }

                if (!string.IsNullOrWhiteSpace(sql))
                    CustomCacheHelper.GetSingleObj().SetForSeconds(key, sql.Trim(), cacheSeconds);  //设置缓存s秒
            }

            if (!string.IsNullOrWhiteSpace(sql))
                return sql.Trim();
            else
                return null;
        }

        /// <summary>
        /// 获取xmlSql的Memory缓存key
        /// </summary>
        /// <param name="xmlFilePath">xml文件路径</param>
        /// <param name="className">类功能模块名称</param>
        /// <param name="sqlFun">sql方法名称</param>
        /// <param name="sqlFunRemark">sql方法备注</param>
        /// <returns></returns>
        private string GetXmlSqlCacheKey(string xmlFilePath, string className, string sqlFun, string sqlFunRemark)
        {
            string key = string.Empty;
            if (!(string.IsNullOrWhiteSpace(xmlFilePath) || string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(sqlFun) || string.IsNullOrWhiteSpace(sqlFunRemark)))
                key = Base64Helper.Encode($"{xmlFilePath.Trim()}.{className.Trim()}.{sqlFun.Trim()}.{sqlFunRemark.Trim()}");
            return key;
        }

        #endregion

        #region XmlConfig
        /// <summary>
        /// 获取xml配置文件中指定节点的txt文本
        /// </summary>
        /// <param name="xmlFilePath">xml文件路径</param>
        /// <param name="xmlNode">节点名称</param>
        /// <param name="xmlRemark">节点备注</param>
        /// <returns>string：txt文本</returns>
        public string GetXmlConfig(string xmlFilePath, string xmlNode, string xmlRemark)
        {
            string xmlTxt = "";
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                return xmlTxt;

            string key = GetXmlConfigCacheKey(xmlFilePath,xmlNode, xmlRemark); //获取缓存key
            xmlTxt = CustomCacheHelper.GetSingleObj().Get<string>(key); //读取缓存

            if (string.IsNullOrWhiteSpace(xmlTxt))
            {
                XDocument doc = XDocument.Load(xmlFilePath); //将XML文件加载进来
                XElement root = doc.Root; //获取到XML的根元素进行操作，SqlQuery

                foreach (XElement item in root.Elements())
                {
                    if (xmlNode.Equals(item.Name.LocalName) && xmlRemark.Equals(item.FirstAttribute.Value.Trim()))
                        xmlTxt = item.Value;
                }
                CustomCacheHelper.GetSingleObj().SetForSeconds(key, xmlTxt.Trim(), cacheSeconds); //设置缓存s秒
            }
            return xmlTxt.Trim();
        }

        /// <summary>
        /// 获取XmlConfig的Memory缓存key
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="sqlXml"></param>
        /// <param name="sqlXmlRemark"></param>
        /// <returns></returns>
        private string GetXmlConfigCacheKey(string xmlFilePath, string sqlXml, string sqlXmlRemark)
        {
            string key = string.Empty;
            if (!(string.IsNullOrWhiteSpace(xmlFilePath) || string.IsNullOrWhiteSpace(sqlXml) || string.IsNullOrWhiteSpace(sqlXmlRemark)))
                key = Base64Helper.Encode($"{xmlFilePath.Trim()}.{sqlXml.Trim()}.{sqlXmlRemark.Trim()}");
            return key;
        }
        #endregion
    }
}
