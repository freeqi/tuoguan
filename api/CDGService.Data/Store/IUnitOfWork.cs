using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CDGService.Data.Store
{
    /// <summary>
    /// 工作单元，在数据仓储操作时，需要在同一上下文时，需要从此接口中调用
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken)) ;

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <returns></returns>
        int SaveChanges() ;

        /// <summary>
        /// 取消所有修改
        /// </summary>
        void DisChanges() ;

        /// <summary>
        /// 返回指定类型的仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRepository<T> GetStore<T>() where T : class ;
        /// <summary>
        /// 返回指定类型的仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> SqlQuery<T>(string sql, params object[] parameters) where T : class,new ();

    }
}
