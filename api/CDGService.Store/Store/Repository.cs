using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CDGService.Data.Store;

namespace CDGService.Store.Store
{
    /// <summary>
    /// 仓储的实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDb _db;

        public Repository(AppDb db)
        {
            _db = db;

        }


        /// <summary>
        /// 当前类型的实体
        /// </summary>
        public IQueryable<T> Entities => _db.Set<T>();

        public void RemoveRange(T[] t)
        {
            this._db.Set<T>().RemoveRange(t);

        }
        public Task<T[]> GetAllAsync<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath, CancellationToken token = default(CancellationToken))
        { 
            return Entities.Include(navigationPropertyPath).ToArrayAsync(token);

        }
        public Task<T[]> GetAllAsync(CancellationToken token = default(CancellationToken))
        { 
            return Entities.ToArrayAsync(token);

        }

        public Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                predicate = t => true;
            return Entities.Where(predicate).FirstOrDefaultAsync();
        }


        public Task<T> GetFirstOrDefaultAsync<TProperty>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            return Entities.Where(predicate).Include(navigationPropertyPath).FirstOrDefaultAsync();
        }


        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                predicate = t => true;
            return Entities.Where(predicate).FirstOrDefault();
        }

        public IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return this.Entities;

            return Entities
                 .Where(predicate);
        }


        public void Insert(T t)
        {
            this._db.Set<T>()
                .Add(t);
        }


        public void Insert(IEnumerable<T> t)
        {
            _db.Set<T>().AddRange(t);
        }
        public void Update(T t)
        {
            _db.Set<T>().Update(t);
        }

        public void UpdateByAttach(T t)
        {
            _db.Set<T>()
                      .Attach(t);
            _db.Entry(t).State = EntityState.Modified;

        }

        public void Update(IEnumerable<T> list)
        {
            _db.Set<T>().UpdateRange(list);
        }

        public void Remove(T t)
        {
            if (t == null) return;

            _db.Set<T>().Remove(t);
        }

        public void Remove(IEnumerable<T> list)
        {
            _db.Set<T>().RemoveRange(list);
        }

        public void Remove(Expression<Func<T, bool>> wherExpression)
        {
            var db = _db.Set<T>();
            db.RemoveRange(db.Where(wherExpression));
        }
    }
}