using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace CDGService.Data.Helper
{
    public class SingleTableBaseSql
    {
        #region 单例模式
        //创建私有化静态obj锁  
        private static readonly object _ObjLock = new object();
        //创建私有静态字段，接收类的实例化对象  
        private static volatile SingleTableBaseSql _SingleTableBaseSql = null;
        //构造函数私有化  
        private SingleTableBaseSql() { }
        //创建单利对象资源并返回  
        public static SingleTableBaseSql GetSingleObj()
        {
            if (_SingleTableBaseSql == null)
            {
                lock (_ObjLock)
                {
                    if (_SingleTableBaseSql == null)
                        _SingleTableBaseSql = new SingleTableBaseSql();
                }
            }
            return _SingleTableBaseSql;
        }
        #endregion

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
        /// 拼接单表【根据条件】统计查询
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="sqlWhere">sql查询条件可选</param>
        /// <returns></returns>
        public string GetTotalSingleTableSql(string tableName, string sqlWhere = null)
        {
            if (string.IsNullOrWhiteSpace(sqlWhere))
                return $"SELECT COUNT(*) AS 'Sum' FROM [dbo].[{tableName}];";
            else
                return $"SELECT COUNT(*) AS 'Sum' FROM [dbo].[{tableName}] WHERE 1=1 {sqlWhere};";
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

        /// <summary>
        /// 拼接单表根据条件获取单条数据的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">需查询table的Model</param>
        /// <param name="tableName">需查询表名</param>
        /// <param name="sqlWhere">查询所需的条件</param>
        /// <param name="showFieldNames">需要展示的字段名，参数格式为"[fieldname1],[fieldname2],……[fieldnameN]"</param>
        /// <returns>sql</returns>
        public string GetSingleTableSql<T>(T entity, string tableName, string sqlWhere, string showFieldNames = null)
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

        /// <summary>
        /// 拼接单表新增的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">需查询table的Model</param>
        /// <param name="tableName">需查询表名</param>
        /// <param name="sqlParameters">拼sql所需参数</param>
        /// <returns>sql</returns>
        public string InsertSingleTableSql<T>(T entity, string tableName, out List<SqlParameter> sqlParameters)
        {
            sqlParameters = new List<SqlParameter>();
            var dic = GetPropertyValue(entity, true, out sqlParameters);
            string names = string.Empty;
            string values = string.Empty;
            foreach (var name in dic.Values)
            {
                if (name.Equals(dic[dic.Values.Count - 1]))
                {
                    names += $"[{name}]";
                    values += $"@{name}";
                }
                else
                {
                    names += $"[{ name}],";
                    values += $"@{name},";
                }

            }
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append($" insert into [{tableName}] ({names})");
            sqlStr.Append($" values ({values})");

            return sqlStr.ToString();
        }

        /// <summary>
        /// 拼接单表根据条件更新的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">需查询table的Model</param>
        /// <param name="tableName">需查询表名</param>
        /// <param name="sqlParameters">拼sql所需参数</param>
        /// <param name="sqlWhere">更新和排序的条件</param>
        /// <param name="updateFieldName">需要更新字段，格式:[FieldName1]=@FieldName1,[FieldName2]=@FieldName2,……[FieldNameN]=@FieldNameN</param>
        /// <returns>sql</returns>
        public string UpdateSingleTableSql<T>(T entity, string tableName, string sqlWhere, out List<SqlParameter> sqlParameters, string updateFieldNames = null)
        {
            sqlParameters = new List<SqlParameter>();
            string setValues = string.Empty;
            if (!string.IsNullOrWhiteSpace(updateFieldNames))
            {
                setValues = updateFieldNames;
            }
            else
            {
                var dic = GetPropertyValue(entity, true, out sqlParameters);
                foreach (var name in dic.Values)
                {
                    if (name.Equals(dic[dic.Values.Count - 1]))
                    {
                        setValues += $"[{name}]=@{name}";
                    }
                    else
                    {
                        setValues += $"[{name}]=@{name},";
                    }

                }
            }
            StringBuilder sqlStr = new StringBuilder();//酒店 软件
            sqlStr.Append($" update [{tableName}]");
            sqlStr.Append($" set {setValues}");
            sqlStr.Append($" where 1=1 {sqlWhere}");

            return sqlStr.ToString();
        }

        /// <summary>
        /// 逻辑删除（更新删除）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tableName">表名</param>
        /// <param name="sqlCondition">删除条件</param>
        /// <returns></returns>
        public string DeleteSingleTableSql(string tableName, string sqlCondition)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append($" Update  [{tableName}]");
            sqlStr.Append($" Set  [DataState]=@DataState,[Modifier]=@Modifier,[ModifierDate]=@ModifierDate");
            sqlStr.Append($" where 1=1 {sqlCondition}");
            return sqlStr.ToString();
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="sqlCondition">删除条件</param>
        /// <returns></returns>
        public string PhysicalDeleteSingleTableSql(string tableName, string sqlCondition)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append($" Delete From [{tableName}]");
            sqlStr.Append($" where 1=1 {sqlCondition}");
            return sqlStr.ToString();
        }


        /// <summary>
        /// 获取分页查询sql
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="tbName">查询的表名称</param>
        /// <param name="sqlWhere">查询条件</param>
        /// <param name="keyValues">key：排序字段名称,val：排序类型</param>
        /// <returns>bool,string：sql</returns>
        public Tuple<bool, string> GetPagingQuerySql(int page, int rows, string tbName, Dictionary<string, string> keyValues, string sqlWhere="")
        {
            var sb = new StringBuilder();
            int min = (page - 1) * rows;
            int max = min + rows;

            string sortName = string.Empty;
            string sortType = string.Empty;

            if (keyValues != null && keyValues.Count > 0)
            {
                foreach (var item in keyValues)
                {
                    sortName = item.Key;
                    sortType = item.Value;
                    if (!(string.IsNullOrWhiteSpace(sortName) || string.IsNullOrWhiteSpace(sortType)))
                        sb.Append($"[{sortName}] {sortType},");
                }
            }

            var sortDic = sb.ToString();
            if (string.IsNullOrWhiteSpace(sortDic))
                return Tuple.Create(false, "");

            sortDic = sortDic.Substring(0, sortDic.Length - 1);
            string sql = $"SELECT T2.N, T1.* FROM [{tbName}] AS T1, (SELECT TOP({max}) ROW_NUMBER() OVER(ORDER BY {sortDic}) AS N, ID FROM [{tbName}] WHERE 1=1 {sqlWhere}) AS T2 WHERE T1.ID = T2.ID AND T2.N > {min} ORDER BY T2.N ASC;";
            return Tuple.Create(true, sql);
        }

    }
}
