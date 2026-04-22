using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace CDGService.Store.Extensions
{
    /// <summary>
    /// SQLServer 数据库操作帮助类
    /// </summary>
    public static class SQLServerHelper
    {

        #region Public methods

        /// <summary>
        /// 根据指定数据库连接和参数，执行T-SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="connString">用作<see cref="SqlConnection"/>对象连接数据库的字符串</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需的一个或多个参数</param>
        /// <returns>受T-SQL语句所影响的记录行数</returns>
        public static Int32 ExecuteNonQuery(String connString, CommandType cmdType, String cmdText, params SqlParameter[] sqlParams)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                return ExecuteNonQuery(conn, cmdType, cmdText, sqlParams);
            }
        }

        /// <summary>
        /// 根据指定数据库连接和参数，执行T-SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="conn">连接数据库的<see cref="SqlConnection">对象</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需的一个或多个参数</param>
        /// <returns>受T-SQL语句所影响的记录行数</returns>
        public static Int32 ExecuteNonQuery(SqlConnection conn, CommandType cmdType, String cmdText, params SqlParameter[] sqlParams)
        {
            SqlCommand cmd = new SqlCommand();

            BuildCommand(conn, cmd, cmdType, cmdText, sqlParams);
            var rows = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            conn.Close();

            return rows;
        }

        /// <summary>
        /// 根据指定数据库连接和参数，执行T-SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="trans">用作事务执行的<see cref="SqlTransaction">对象</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需的一个或多个参数</param>
        /// <returns>受T-SQL语句所影响的记录行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, String cmdText, params SqlParameter[] sqlParams)
        {

            SqlCommand cmd = new SqlCommand();
            BuildCommand(trans.Connection, cmd, trans, cmdType, cmdText, sqlParams);
            var rows = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            return rows;
        }

        /// <summary>
        /// 根据指定数据库连接和参数，执行T-SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="committran">用作分布式事务执行的<see cref="CommittableTransaction">对象</see></param>
        /// <param name="connString">连接字符串</param>
        /// <param name="cmdType">命令字符串类型</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">参数</param>
        /// <returns>受T-SQL语句所影响的记录行数</returns>
        public static int ExecuteNonQuery(CommittableTransaction committran, String connString, CommandType cmdType, String cmdText,
            params SqlParameter[] sqlParams)
        {

            var cmd = new SqlCommand();
            var conn = new SqlConnection(connString);

            BuildCommand(committran, conn, cmd, cmdType, cmdText, sqlParams);

            var rows = 0;

            rows = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            conn.Close();

            return rows;
        }

        /// <summary>
        /// 根据指定数据库连接和参数执行SQL语句，生成并返回<see cref="SqlDataReader"/>类型的结果集。
        /// </summary>
        /// <param name="connString">用作<see cref="SqlConnection"/>对象连接数据库的字符串</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需的一个或多个参数</param>
        /// <returns>一个包含语句执行结果集的<see cref="SqlDataReader"/>实例</returns>
        public static SqlDataReader ExecuteReader(String connString, CommandType cmdType, String cmdText, params SqlParameter[] sqlParams)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connString);

            try
            {

                BuildCommand(conn, cmd, cmdType, cmdText, sqlParams);

                //如果SqlDataReader对象被关闭，则对应的SqlConnection也将被关闭
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

     

        /// <summary>
        /// 根据指定数据库连接和参数执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="connString">用作<see cref="SqlConnection"/>对象连接数据库的字符串</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需的一个或多个参数</param>
        /// <returns>返回结果集中第一行的第一列，若结果集为空，则为空引用。</returns>
        public static Object ExecuteScalar(String connString, CommandType cmdType, String cmdText, params SqlParameter[] sqlParams)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connString))
            {

                BuildCommand(conn, cmd, cmdType, cmdText, sqlParams);
                var val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();

                return val;
            }
        }

        /// <summary>
        /// 根据指定数据库连接和参数执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="conn">连接数据库的<see cref="SqlConnection">对象</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需的一个或多个参数</param>
        /// <returns>返回结果集中第一行的第一列，若结果集为空，则为空引用。</returns>
        public static Object ExecuteScalar(SqlConnection conn, CommandType cmdType, String cmdText, params SqlParameter[] sqlParams)
        {

            SqlCommand cmd = new SqlCommand();

            BuildCommand(conn, cmd, cmdType, cmdText, sqlParams);
            var val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            return val;
        }

        /// <summary>
        /// 构建用于T-SQL执行的输入参数。
        /// </summary>
        /// <param name="paramterName">参数名称，该名称应和列名保持一致</param>
        /// <param name="dbType">参数数据类型</param>
        /// <param name="size">参数长度</param>
        /// <param name="val">参数值</param>
        /// <returns>一个用于T-SQL执行的<see cref="System.Data.SqlClient.SqlParameter"/>实例。</returns>
        public static SqlParameter BuildInParameter(String paramterName, SqlDbType dbType, Object val)
        {
            if (val == null)
                throw new ArgumentNullException("val", "The null sql parameter is not allowed");

            return new SqlParameter(paramterName, val)
                {
                    SqlDbType = dbType,
                    Direction = ParameterDirection.Input
                };
        }

        /// <summary>
        /// 构建用于T-SQL执行的输入参数
        /// </summary>
        /// <param name="paramterName">参数名称，该名称应和列名保持一致</param>
        /// <param name="dbType">参数数据类型</param>
        /// <param name="size">参数长度</param>
        /// <param name="val">参数值</param>
        /// <param name="isReturnVal">参数是否为返回值参数，true：ReturnValue参数， false: Output参数。</param>
        /// <returns>一个用于T-SQL执行的<see cref="System.Data.SqlClient.SqlParameter"/>实例</returns>
        public static SqlParameter BuildOutParameter(String paramterName, SqlDbType dbType, Object val, Boolean isReturnVal)
        {
            if (val == null)
                throw new ArgumentNullException("val", "The null sql parameter is not allowed");

            var param = new SqlParameter(paramterName, val)
            {
                SqlDbType = dbType
            };

            if (isReturnVal)
                param.Direction = ParameterDirection.ReturnValue;
            else
                param.Direction = ParameterDirection.Output;

            return param;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 构建查询语句所需的参数。
        /// </summary>
        /// <param name="conn">连接数据库的<see cref="SqlConnection">对象</param>
        /// <param name="cmd">执行查询语句的<see cref="SqlCommand">对象</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需要的一个或多个参数</param>
        private static void BuildCommand(SqlConnection conn, SqlCommand cmd, CommandType cmdType, String cmdText, IList<SqlParameter> sqlParams)
        {
            BuildCommand(conn, cmd, null, cmdType, cmdText, sqlParams);
        }

        /// <summary>
        /// 构建查询语句所需的参数。
        /// </summary>
        /// <param name="conn">连接数据库的<see cref="SqlConnection">对象</param>
        /// <param name="cmd">执行查询语句的<see cref="SqlCommand">对象</param>
        /// <param name="trans">执行事务的<see cref="SqlTransaction">对象</param>
        /// <param name="cmdType">命令字符串类型（存储过程、T-SQL语句等）</param>
        /// <param name="cmdText">T-SQL语句或存储过程名</param>
        /// <param name="sqlParams">执行语句所需要的一个或多个参数</param>
        private static void BuildCommand(SqlConnection conn, SqlCommand cmd, SqlTransaction trans, CommandType cmdType, String cmdText, IList<SqlParameter> sqlParams)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (sqlParams != null || sqlParams.Count > 0)
            {
                foreach (SqlParameter param in sqlParams)
                    cmd.Parameters.Add(param);
            }
        }

        /// <summary>
        /// 构建执行T-SQL语句所需的参数。（可提交事务）
        /// </summary>
        /// <param name="committran">分布式事务对象</param>
        /// <param name="conn"></param>
        /// <param name="cmd"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="sqlParams"></param>
        private static void BuildCommand(CommittableTransaction committran, SqlConnection conn, SqlCommand cmd,
            CommandType cmdType, String cmdText, IList<SqlParameter> sqlParams)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            if (committran != null)
                conn.EnlistTransaction(committran);

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            cmd.CommandType = cmdType;

            if (sqlParams != null || sqlParams.Count > 0)
            {
                foreach (SqlParameter param in sqlParams)
                    cmd.Parameters.Add(param);
            }
        }
        #endregion

    }
}
