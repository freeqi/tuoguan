using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CDGService.Data.Store
{

    /// <summary>
    /// 数据库操作仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// 返回所有Item
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<T[]> GetAllAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 根据条件获取数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 返回首个，如果为空，则返回默认值
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null);


        /// <summary>
        /// 返回首个结果，如果为空，则返回默认值
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate = null) ;

        /// <summary>
        /// 插入對象
        /// </summary>
        /// <param name="t"></param>
        void Insert(T t);
     
        void RemoveRange(T[] t);

        void Insert(IEnumerable<T> t) ;
        void Update(T t) ;
        void UpdateByAttach(T t) ;
        void Update(IEnumerable<T> list) ;
        void Remove(T t) ;
        void Remove(IEnumerable<T> list) ;
        void Remove(Expression<Func<T, bool>> wherExpression) ;

        Task<T> GetFirstOrDefaultAsync<TProperty>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TProperty>> navigationPropertyPath);

        Task<T[]> GetAllAsync<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 当前类型的实体
        /// </summary>
        IQueryable<T> Entities { get; }
    }
}