using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CDGService.Data.Store;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;
using System.Reflection;
using System;

namespace CDGService.Store.Store
{
    internal class UnitOfWork : IUnitOfWork
    {
        public readonly AppDb _appDb;

        public UnitOfWork(AppDb appDb)
        {
            _appDb = appDb;
        }

        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        private static readonly object Lockobj = new object();

        public IRepository<T> GetStore<T>() where T : class
        {
            lock (Lockobj)
            {
                var key = typeof(T).ToString();
                if (key == "CDGService.Data.PurchaseDetail")
                {
                }
                object value;
                if (_cache.TryGetValue(key, out value))
                {
                    IRepository<T> v = value as IRepository<T>;
                    if (v != null) return v;
                }

                var result = new Repository<T>(_appDb);
                _cache.Add(key, result);
                return result;

            }
        }


        public Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken))
        {
            lock (savalack)
            {

                return _appDb?.SaveChangesAsync(token);
            }
        }

        private static readonly object savalack = new object();

        public int SaveChanges()
        {
            lock (savalack)
            {
                return _appDb.SaveChanges();
            }
        }

        public void DisChanges()
        {
            _appDb?.ChangeTracker?.DetectChanges();
        }

        public void Dispose()
        {
            _cache.Clear();
            _appDb?.Dispose();

        }

        public IEnumerable<T> SqlQuery<T>(string sql, params object[] parameters) where T : class,new ()
        {
            var data =  _appDb?.Database.SqlQuery<T>(sql, parameters);
            return data;
        }
    }


    public static class DbContextExtensions
    {
        private static void CombineParams(ref DbCommand command, params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if (!parameter.ParameterName.Contains("@"))
                        parameter.ParameterName = $"@{parameter.ParameterName}";
                    command.Parameters.Add(parameter);
                }
            }
        }

        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection dbConn, params object[] parameters)
        {
            DbConnection conn = facade.GetDbConnection();
            dbConn = conn;
            conn.Open();
            DbCommand cmd = conn.CreateCommand();
            if (facade.IsSqlServer())
            {
                cmd.CommandText = sql;
                CombineParams(ref cmd, parameters);
            }
            return cmd;
        }

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            DbCommand cmd = CreateCommand(facade, sql, out DbConnection conn, parameters);
            DbDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt;
        }

        public static IEnumerable<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            DataTable dt = SqlQuery(facade, sql, parameters);
            return dt.ToEnumerable<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                ts[i] = t;
                i++;
            }
            return ts;
        }
    }
}
