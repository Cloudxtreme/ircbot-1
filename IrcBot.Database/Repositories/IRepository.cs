using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using IrcBot.Database.Infrastructure;

namespace IrcBot.Database.Repositories
{
    public interface IRepository<T> where T : class, IObjectState
    {
        T Find(params object[] keyValues);

        IQueryable<T> SelectQuery(string query, params object[] parameters);

        void Insert(T entity);

        void InsertRange(IEnumerable<T> entities);

        void InsertOrUpdateGraph(T entity);

        void InsertGraphRange(IEnumerable<T> entnties);

        void Update(T entity);

        void Delete(object id);

        void Delete(T entity);

        IQueryFluent<T> Query();

        IQueryFluent<T> Query(IQueryObject<T> queryObject);

        IQueryFluent<T> Query(Expression<Func<T, bool>> query);

        IQueryable<T> Queryable();

        IRepository<TRepository> GetRepository<TRepository>() where TRepository : class, IObjectState;
    }
}
