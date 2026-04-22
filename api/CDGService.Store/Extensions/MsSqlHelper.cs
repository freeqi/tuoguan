using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CDGService.Store
{
    /// <summary>
    /// MS-SQLServer帮助类
    /// </summary>
    public sealed class MsSqlHelper
    {
        #region 单利模式
        /// <summary>
        /// 1.构造函数私有化
        /// </summary>
      //  public MsSqlHelper() { }

        /// <summary>
        /// 2.创建私有化静态字段锁
        /// </summary>
        private static readonly object _ObjLock = new object();

        /// <summary>
        /// 3.创建私有化类对象，接收类的实例化对象
        /// volatile 关键字促进线程安全，保障线程有序执行
        /// </summary>
        private static volatile MsSqlHelper _MsSqlHelper = null;

        /// <summary>
        /// 4.创建类实例化对象
        /// </summary>
        /// <returns></returns>
        public static MsSqlHelper GetSingleObj()
        {
            if (_MsSqlHelper == null)
            {
                lock (_ObjLock) //保证只有一线程操作
                {
                    if (_MsSqlHelper == null)
                    {
                        _MsSqlHelper = new MsSqlHelper();
                    }
                }
            }
            return _MsSqlHelper;
        }
        #endregion


        #region 获取连接字符串
        /// <summary>
        /// 数据库类型名称
        /// </summary>
        public enum DbTypeName { MsSqlServer, MsSqlServerLog };

        // 获取数据库json配置文件路径
        private static readonly string _DbJsonConfigPath = string.Empty;

        // 数据库链接字符串
        public static string _ConnString ;


        #endregion

        #region 接口实现
        #region （添加，更新，删除）操作
        #region 单个数据（添加，更新，删除）
        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// Insert插入,Delete删除,Update更新  
        /// </summary>  
        /// <param name="cmdText">sql语句</param>  
        /// <param name="sqlParams">[可选]sql参数化</param>  
        /// <returns>int：受影响的行数</returns>  
        public int ExecNonQuery(string cmdText, params SqlParameter[] sqlParams)
        {
            int rowsCount = 0;
            using (SqlConnection conn = new SqlConnection(_ConnString))  // 建立数据库连接对象  
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text; //指定cmd命令类型为文本类型(默认，可不写);  
                    cmd.CommandText = cmdText; //sql语句或存储过程                         
                    if (sqlParams != null && sqlParams.Length > 0) //检查参数组是否有数据  
                    {
                        foreach (SqlParameter sqlParam in sqlParams)
                        {
                            //判断参数是否为null，是则转为数据库接受的DBnull
                            if (sqlParam.Value == null)
                            {
                                sqlParam.Value = DBNull.Value;
                            }
                            cmd.Parameters.Add(sqlParam); //参数格式化，防止sql注入  
                        }
                    }
                    rowsCount = cmd.ExecuteNonQuery(); //执行非查询命令，接收受影响行数，大于0的话表示添加成功  
                    cmd.Parameters.Clear();
                }
                CloseConnection(conn);
            }
            return rowsCount;
        }
        #endregion

        #region 批量添加数据
        /// <summary>  
        /// 1.批量插入数据【Bulk】，并返回受影响的行数  
        /// </summary>  
        /// <param name="dt">DataTable</param> 
        /// <param name="dtName">表名称</param>  
        /// <returns>int：受影响的行数</returns>  
        public int InsertBulkToDB(DataTable dt, string dtName)
        {
            string tableName = dtName ?? dt.TableName;
            int rowsCount = 0;
            string sql = $"SELECT TOP(0) * FROM {tableName};"; //查询空表结构
            var sourceDt = GetDataTable(sql); //数据源DataTable对象
            using (SqlConnection conn = new SqlConnection(_ConnString))
            {
                OpenConnection(conn);
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        bulkCopy.DestinationTableName = tableName; //数据表名称
                        bulkCopy.BatchSize = dt.Rows.Count;
                        for (int i = 0; i < sourceDt.Columns.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (sourceDt.Columns[i].ColumnName.Equals(dt.Columns[j].ColumnName))
                                {
                                    bulkCopy.ColumnMappings.Add(sourceDt.Columns[i].ColumnName, dt.Columns[j].ColumnName); //数据源对象和目标列映射，保证结构一致
                                }
                            }
                        }
                        bulkCopy.WriteToServer(dt);
                        rowsCount += bulkCopy.BatchSize;
                    }
                    CloseConnection(conn);
                }
            }
            return rowsCount;
        }
        /// <summary>
        /// 事务执行NoQuerySQL(多语句)和批量新增数据
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int ExecSqlAndInsertBulkToDBByTran(List<Tuple<string, SqlParameter[]>> list, DataTable dt)
        {
            SqlTransaction transaction = null; // 创建事务对象  
            string sql = $"SELECT TOP(0) * FROM {dt.TableName};"; //查询空表结构
            var sourceDt = GetDataTable(sql); //数据源DataTable对象
            try
            {
                int count = 0;
                // 建立数据库连接对象  
                using (SqlConnection conn = new SqlConnection(_ConnString))
                {
                    OpenConnection(conn);
                    //开始事务操作  
                    using (transaction = conn.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.Transaction = transaction;
                            foreach (var tuple in list)
                            {
                                cmd.CommandText = tuple.Item1.Trim(); //sql语句  
                                //sql参数化  
                                if (tuple.Item2 is SqlParameter[] parameters && parameters.Length > 0)
                                {
                                    foreach (SqlParameter parameter in parameters)
                                    {
                                        //参数格式化，防止sql注入  
                                        //var name = parameter.ParameterName;  
                                        //var val = parameter.Value;  
                                        //cmd.Parameters.Add(new SqlParameter(name, val));  

                                        //判断参数是否为null，是则转为数据库接受的DBnull
                                        if (parameter.Value == null)
                                        {
                                            parameter.Value = DBNull.Value;
                                        }

                                        cmd.Parameters.Add(parameter); //等效代码  
                                    }
                                }
                                count += cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }

                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, transaction))
                        {
                            if (dt != null && dt.Rows.Count != 0)
                            {
                                sqlBulkCopy.DestinationTableName = dt.TableName; //数据表名称  
                                sqlBulkCopy.BatchSize = dt.Rows.Count;
                                for (int i = 0; i < sourceDt.Columns.Count; i++)
                                {
                                    for (int j = 0; j < dt.Columns.Count; j++)
                                    {
                                        if (sourceDt.Columns[i].ColumnName.Equals(dt.Columns[j].ColumnName))
                                        {
                                            sqlBulkCopy.ColumnMappings.Add(sourceDt.Columns[i].ColumnName, dt.Columns[j].ColumnName); //数据源对象和目标列映射，保证结构一致
                                        }
                                    }
                                }
                                sqlBulkCopy.WriteToServer(dt);
                                count += sqlBulkCopy.BatchSize;

                                //transaction.Commit(); //事务提交  
                            }
                        }
                        transaction.Commit(); //事务提交
                        CloseConnection(conn);
                        return count;
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback(); //事务回滚  
                throw new Exception(ex.Message, ex);
            }
        }

        public void TableValuedToDB2(DataTable dt)
        {
            SqlConnection sqlConn = new SqlConnection(_ConnString);
            const string TSqlStatement =
             "insert into BulkTestTable (Id,UserName,Pwd)" +
             " SELECT nc.Id, nc.UserName,nc.Pwd" +
             " FROM @NewBulkTestTvp AS nc";
            SqlCommand cmd = new SqlCommand(TSqlStatement, sqlConn);
            SqlParameter catParam = cmd.Parameters.AddWithValue("@NewBulkTestTvp", dt);
            catParam.SqlDbType = SqlDbType.Structured; //一种特殊的数据类型，用于指定表值中包含的结构化数据
            //表值参数的名字叫BulkUdt，在上面的建立测试环境的SQL中有。  
            catParam.TypeName = "dbo.BulkUdt";
            try
            {
                sqlConn.Open();
                if (dt != null && dt.Rows.Count != 0)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConn.Close();
            }
        }


        /// <summary>
        /// 2.批量插入数据【表值参数(Table-Valued Parameters)】,需数据库建表值类型
        /// 参考：https://blog.csdn.net/cokeboxs/article/details/51123779
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tSql"></param>
        /// <param name="tvpName">表值参数名</param>
        /// <param name="bulkTvp">临时表</param>
        public void TableValuedToDB(DataTable dt, string tSql, string tvpName, string bulkTvp)
        {
            using (SqlConnection conn = new SqlConnection(_ConnString))
            {
                using (SqlCommand cmd = new SqlCommand(tSql, conn))
                {
                    SqlParameter catParam = cmd.Parameters.AddWithValue($"@{bulkTvp.Trim()}", dt);
                    catParam.SqlDbType = SqlDbType.Structured; //一种特殊的数据类型，用于指定表值中包含的结构化数据
                    catParam.TypeName = $"dbo.{tvpName.Trim()}";
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        OpenConnection(conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                CloseConnection(conn);
            }
        }

        #endregion
        #endregion

        #region 事务操作Transactions  
        /// <summary>  
        /// 多语句（sql）事务操作  
        /// 带参执行（sql参数化）  
        /// </summary>  
        /// <param name="dic">Dictionary<string,SqlParameter[]></param>  
        /// <returns>int</returns>  
        public int ExecSqlTranByParams(Dictionary<string, SqlParameter[]> dic)
        {
            SqlTransaction transaction = null; // 创建事务对象  
            try
            {
                int count = 0;
                // 建立数据库连接对象  
                using (SqlConnection conn = new SqlConnection(_ConnString))
                {
                    OpenConnection(conn);
                    //开始事务操作  
                    using (transaction = conn.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.Transaction = transaction;
                            foreach (var item in dic)
                            {
                                cmd.CommandText = item.Key.Trim(); //sql语句  
                                SqlParameter[] parameters = item.Value;//sql参数化  

                                if (parameters != null && parameters.Length > 0)
                                {
                                    foreach (SqlParameter parameter in parameters)
                                    {
                                        //参数格式化，防止sql注入  
                                        //var name = parameter.ParameterName;  
                                        //var val = parameter.Value;  
                                        //cmd.Parameters.Add(new SqlParameter(name, val));  

                                        //判断参数是否为null，是则转为数据库接受的DBnull
                                        if (parameter.Value == null)
                                        {
                                            parameter.Value = DBNull.Value;
                                        }

                                        cmd.Parameters.Add(parameter); //等效代码  
                                    }
                                }
                                count += cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            transaction.Commit(); //事务提交  
                            return count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback(); //事务回滚  
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 多语句（sql）事务操作  
        /// 带参执行（sql参数化）  
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int ExecSqlTranByParams(List<Tuple<string, SqlParameter[]>> list)
        {
            SqlTransaction transaction = null; // 创建事务对象  
            try
            {
                int count = 0;
                // 建立数据库连接对象  
                using (SqlConnection conn = new SqlConnection(_ConnString))
                {
                    OpenConnection(conn);
                    //开始事务操作  
                    using (transaction = conn.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.Transaction = transaction;
                            foreach (var tuple in list)
                            {
                                cmd.CommandText = tuple.Item1.Trim(); //sql语句  
                                //sql参数化  
                                if (tuple.Item2 is SqlParameter[] parameters && parameters.Length > 0)
                                {
                                    foreach (SqlParameter parameter in parameters)
                                    {
                                        //参数格式化，防止sql注入  
                                        //var name = parameter.ParameterName;  
                                        //var val = parameter.Value;  
                                        //cmd.Parameters.Add(new SqlParameter(name, val));  

                                        //判断参数是否为null，是则转为数据库接受的DBnull
                                        if (parameter.Value == null)
                                        {
                                            parameter.Value = DBNull.Value;
                                        }

                                        cmd.Parameters.Add(parameter); //等效代码  
                                    }
                                }
                                count += cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            transaction.Commit(); //事务提交  
                            return count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback(); //事务回滚  
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>  
        /// 多语句（sql）事务操作  
        /// 无参执行  
        /// </summary>  
        /// <param name="sqlList">多条SQL语句</param>          
        /// <returns>bool</returns>  
        public bool ExecSqlTran(List<string> sqlList)
        {
            SqlTransaction transaction = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnString))
                {
                    OpenConnection(conn);
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        //开始事务操作  
                        using (transaction = conn.BeginTransaction())
                        {
                            cmd.Transaction = transaction;
                            foreach (var strsql in sqlList)
                            {
                                if (strsql != null)
                                {
                                    cmd.CommandText = strsql.Trim();
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit(); //事务提交  
                        }
                        cmd.Parameters.Clear();
                        return true; //事务执行成功返回true  
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback(); //事务回滚  
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region 查询操作
        /// <summary>  
        /// 返回数据库表DataTable  
        /// </summary>  
        /// <param name="cmdText">sql语句</param>  
        /// <param name="sqlParams">sql参数化</param>  
        /// <returns>DataTable</returns>  
        public DataTable GetDataTable(string cmdText, params SqlParameter[] sqlParams)
        {
            using (DataTable dt = new DataTable())
            {
                
                using (SqlConnection conn = new SqlConnection(_ConnString))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = cmdText;
                        cmd.CommandTimeout = 240;
                        if (sqlParams != null && sqlParams.Length > 0)
                        {
                            foreach (SqlParameter parameter in sqlParams)
                            {
                                //判断参数是否为null，是则转为数据库接受的DBnull
                                if (parameter.Value == null)
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                //参数格式化，防止sql注入  
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        //适配器自动打开数据库连接  
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                        cmd.Parameters.Clear();
                    }
                    CloseConnection(conn);
                }
                return dt;
            }
        }

        /// <summary>  
        /// 返回数据库表DataTable  
        /// </summary>  
        /// <param name="cmdText">sql语句</param>  
        /// <param name="sqlParams">sql参数化</param>  
        /// <returns>DataTable</returns>  
        public DataTable GetDataTable(string cmdText,string connString, params SqlParameter[] sqlParams)
        {
            using (DataTable dt = new DataTable())
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = cmdText;
                        cmd.CommandTimeout = 240;
                        if (sqlParams != null && sqlParams.Length > 0)
                        {
                            foreach (SqlParameter parameter in sqlParams)
                            {
                                //判断参数是否为null，是则转为数据库接受的DBnull
                                if (parameter.Value == null)
                                {
                                    parameter.Value = DBNull.Value;
                                }
                                //参数格式化，防止sql注入  
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        //适配器自动打开数据库连接  
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                        cmd.Parameters.Clear();
                    }
                    CloseConnection(conn);
                }
                return dt;
            }
        }

        /// <summary>
        /// 执行多sql语句，返回数据集
        /// </summary>
        /// <param name="sqlTuples">list：tabNames[可选],sql,SqlParameter[]</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(List<Tuple<string, string, SqlParameter[]>> sqlTuples)
        {
            using (DataSet ds = new DataSet())
            {
                string tabName = string.Empty; //tab名称
                string cmdText = string.Empty; //sql语句   
                SqlParameter[] sqlParams = null;//sql参数化  

                if (sqlTuples != null && sqlTuples.Count > 0)
                {
                    foreach (var tuple in sqlTuples)
                    {
                        tabName = tuple.Item1; //tab名称
                        cmdText = tuple.Item2; //sql语句   
                        sqlParams = tuple.Item3;//sql格式化参数
                        using (SqlConnection conn = new SqlConnection(_ConnString))
                        {
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = cmdText;
                                //检查参数组是否有数据  
                                if (sqlParams != null && sqlParams.Length > 0)
                                    cmd.Parameters.AddRange(sqlParams);

                                //适配器自动打开数据库连接  
                                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                {
                                    if (string.IsNullOrWhiteSpace(tabName))
                                        da.Fill(ds);
                                    else
                                        da.Fill(ds, tabName); // 将tabName查询结果集合填入DataSet中，并且将DataTable命名为tabName  
                                }
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                }
                return ds;
            }
        }

        /// <summary>  
        /// 数据读取器，逐行读取数据库表的所有字段的值  
        /// 取值reader["字段名"]    
        /// </summary>  
        /// <param name="cmdText">sql语句</param>  
        /// <param name="sqlParams">sql参数化</param>  
        /// <returns>SqlDataReader</returns>  
        public IDataReader GetDataReader(string cmdText, params SqlParameter[] sqlParams)
        {
            using (SqlConnection conn = new SqlConnection(_ConnString))
            {
                OpenConnection(conn);
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    //检查参数组是否有数据  
                    if (sqlParams != null && sqlParams.Length > 0)
                        cmd.Parameters.AddRange(sqlParams);

                    //dataReader = cmd.ExecuteReader(); //快速执行查询命令(快速读取数据)并返回结果  
                    //保证当SqlDataReader对象被关闭时，其依赖的连接也会被自动关闭CommandBehavior.  
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        cmd.Parameters.Clear();
                        CloseDataReader(reader);
                        cmd.Dispose();
                        return reader;
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql语句，返回第一行第一列的值
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="sqlParams">sql格式化参数</param>
        /// <returns>object</returns>
        public object ExecScalar(string cmdText, params SqlParameter[] sqlParams)
        {
            object id = null; //接收数据表自增长Id 
            using (SqlConnection conn = new SqlConnection(_ConnString)) // 建立数据库连接对象  
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText; //sql语句或存储过程  
                    if (sqlParams != null && sqlParams.Length > 0) //检查参数组是否有数据  
                    {
                        foreach (SqlParameter parameter in sqlParams)
                        {
                            //判断参数是否为null，是则转为数据库接受的DBnull
                            if (parameter.Value == null)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            //参数格式化，防止sql注入  
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    id = cmd.ExecuteScalar(); //执行命令，返回第一行第一列的值 obj
                    cmd.Parameters.Clear();
                }
                CloseConnection(conn);
            }
            return id;
        }
        #endregion

        #region 执行存储过程
        /// <summary>
        /// 存储过程，返回int（受影响的行数）
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="sqlParams">[可选]sql参数化</param>
        /// <returns>int</returns>
        public int GetCountByExecProc(string procName, params SqlParameter[] sqlParams)
        {
            using (SqlConnection conn = new SqlConnection(_ConnString))
            {
                //打开连接.    
                OpenConnection(conn);
                //创建Command对象  
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = procName;
                    cmd.CommandType = CommandType.StoredProcedure; //指定cmd命令类型为存储过程  
                    //检查参数组是否有数据                                           
                    if (sqlParams != null && sqlParams.Length > 0) cmd.Parameters.AddRange(sqlParams);

                    int count = cmd.ExecuteNonQuery(); //执行非查询命令,并返回受影响的行数  
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    return count;
                }
            }
        }

        /// <summary>
        /// 存储过程，返回SqlDataReader
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="sqlParams">[可选]sql参数化</param>
        /// <returns>IDataReader[数据读取器SqlDataReader]</returns>
        public IDataReader GetDataReaderByExecProc(string procName, params SqlParameter[] sqlParams)
        {
            using (SqlConnection conn = new SqlConnection(_ConnString))
            {
                OpenConnection(conn);
                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; //指定cmd类型为存储过程  
                    //检查可选参数组是否有数据  
                    if (sqlParams != null && sqlParams.Length > 0) cmd.Parameters.AddRange(sqlParams);

                    // 数据读取器SqlDataReader  
                    //dataReader = cmd.ExecuteReader(); //快速执行查询命令(快速读取数据)并返回结果  
                    //保证当SqlDataReader对象被关闭时，其依赖的连接也会被自动关闭CommandBehavior.  
                    using (SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        cmd.Parameters.Clear();
                        return dataReader;
                    }
                }
            }
        }


        /// <summary>
        /// 存储过程,返回数据集
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="tabName">[可选]数据源映射表名称</param>
        /// <param name="sqlParams">[可选]sql参数化</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSetByExecProc(string procName, string tabName = null, params SqlParameter[] sqlParams)
        {
            using (SqlConnection conn = new SqlConnection(_ConnString))
            {
                var ds = new DataSet();
                using (SqlCommand cmd = new SqlCommand(procName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; //指定cmd类型为存储过程  
                    //检查可选参数组是否有数据  
                    if (sqlParams != null && sqlParams.Length > 0) cmd.Parameters.AddRange(sqlParams);

                    //适配器自动打开数据库连接  
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        if (string.IsNullOrWhiteSpace(tabName))
                            da.Fill(ds);
                        else
                            da.Fill(ds, tabName);
                    }
                    cmd.Parameters.Clear();
                    CloseConnection(conn);
                }
                return ds;
            }
        }
        #endregion
        #endregion

        #region 辅助方法
        /// <summary>  
        /// 根据数据源（数据库表）动态构建DataTable类型对象，并填充sql参数化数据  
        /// </summary>  
        /// <param name="tabName">表名称</param>  
        /// <param name="listDic">键值对数据值</param>  
        /// <returns>DataTable</returns>  
        public DataTable CreateDataTable(string tabName, List<Dictionary<string, object>> listDic)
        {
            #region 1.查询数据源（数据库表）  
            string sqlQuery = $"select * from {tabName} where 1!=1";
            DataTable dt = GetSingleObj().GetDataTable(sqlQuery);
            #endregion

            #region 2.构建DataTable构架：根据数据源（数据库表）动态构建DataTable的构架  
            DataTable myDt = new DataTable(tabName.Trim());
            //完成数据库表动态创建DataTable的构架，但是里面是没有任何数据的    
            foreach (DataColumn dc in dt.Columns)
            {
                string columnName = dc.ColumnName.ToString().Trim(); //获取表列名称  
                string columnType = dc.DataType.ToString().Trim(); //获取表类型名称  
                myDt.Columns.Add(new DataColumn(columnName, Type.GetType(columnType)));
            }
            #endregion

            #region 3.DataTable填充数据：在构建好的DataTable类型中填充（Sql参数化）数据  
            //在构建好的myDt(DataTable类型)中填充数据（批量数据） 
            foreach (var dic in listDic)
            {
                DataRow dr = myDt.NewRow();
                foreach (var item in dic)
                {
                    string columnName = item.Key;
                    if (item.Value == null)
                        dr[columnName] = DBNull.Value;
                    else
                        dr[columnName] = item.Value;
                }
                myDt.Rows.Add(dr);
            }
            #endregion
            return myDt;
        }
        #endregion

        #region DataTable,DataSet,DataReader 数据对象处理  
        /// <summary>  
        /// 获取数据表DataTable中的值，处理转换为ArrayList  
        /// </summary>  
        /// <param name="ds">DataSet</param>  
        /// <param name="isCount">[可选]是否统计数据行</param>  
        /// <returns>ArrayList</returns>  
        public ArrayList ConvertDataTableToArrayList(DataTable dt, bool isCount = false)
        {
            var list = new ArrayList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var dic = new Dictionary<string, string>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string key = dt.Columns[j].ColumnName.ToString();
                    string val = dt.Rows[i][j].ToString().Trim();
                    dic[key] = val;
                }
                list.Add(dic);
            }
            if (isCount)
            {
                var dicCount = new Dictionary<string, int>
                {
                    { "RowsCount", dt.Rows.Count } //统计查询数据行数  
                };
                list.Add(dicCount);
            }
            return list;
        }

        /// <summary>  
        /// 获取数据集DataSet中的值，处理转换为List<ArrayList>  
        /// </summary>  
        /// <param name="ds">DataSet</param>  
        /// <param name="isCount">[可选]是否统计数据行</param>  
        /// <returns>List<ArrayList></returns>  
        public List<ArrayList> ConvertDataSetToList(DataSet ds, bool isCount = false)
        {
            var list = new List<ArrayList>();
            //获取ds中表的数量  
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                var al = new ArrayList{
                    ConvertDataTableToArrayList(ds.Tables[i], isCount)
                };
                if (isCount)
                {
                    var dicCount = new Dictionary<string, int>
                    {
                        { "TablesCount", ds.Tables.Count } //统计查询数据表行数  
                    };
                    al.Add(dicCount);
                }
                list.Add(al);
            }
            return list;
        }

        /// <summary>  
        /// 数据读取器【DataReader】转换DataTable  
        /// </summary>  
        /// <param name="dataReader">数据读取器</param>  
        /// <returns>DataTable</returns>  
        public DataTable ConvertDataReaderToDataTable(SqlDataReader dataReader)
        {
            using (DataTable dt = new DataTable())
            {
                //动态添加表的数据列    
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    using (DataColumn myDataColumn = new DataColumn())
                    {
                        myDataColumn.DataType = dataReader.GetFieldType(i);
                        myDataColumn.ColumnName = dataReader.GetName(i);
                        dt.Columns.Add(myDataColumn);
                    }
                }

                //添加表的数据    
                while (dataReader.Read())
                {
                    DataRow myDataRow = dt.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        myDataRow[i] = dataReader[i].ToString();
                    }
                    dt.Rows.Add(myDataRow);
                }
                //关闭数据读取器DataReader  
                CloseDataReader(dataReader);
                return dt;
            }
        }
        #endregion

        #region 开启连接SqlConnection.Open  
        /// <summary>  
        /// 打开OracleConnection  
        /// </summary>  
        /// <param name="conn">数据库连接对象</param>  
        private static void OpenConnection(SqlConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, null);
            }
        }
        #endregion

        #region 关闭连接，释放资源  
        /// <summary>  
        /// 关闭Connection  
        /// </summary>  
        /// <param name="conn">数据库(Oracle)连接对象</param>  
        private static void CloseConnection(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();//释放资源  
            }
        }

        /// <summary>  
        /// 关闭DataReader  
        /// </summary>  
        /// <param name="dataReader">数据读取器对象</param>  
        private static void CloseDataReader(SqlDataReader dataReader)
        {
            if (dataReader.IsClosed == false) dataReader.Close();
        }
        #endregion
    }

}
