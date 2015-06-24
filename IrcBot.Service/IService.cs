using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using IrcBot.Database.Infrastructure;
using IrcBot.Database.Repositories;

namespace IrcBot.Service
{
    public interface IService<T> where T : IObjectState
    {
        T Find(params object[] keyValues);

        IQueryable<T> SelectQuery(string query, params object[] parameters);

        void Insert(T entity);

        void InsertRange(IEnumerable<T> entities);

        void InsertOrUpdateGraph(T entity);

        void InsertGraphRange(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(object id);

        void Delete(T entity);

        IQueryFluent<T> Query();

        IQueryFluent<T> Query(IQueryObject<T> queryObject);

        IQueryFluent<T> Query(Expression<Func<T, bool>> query);

        Task<T> FindAsync(params object[] keyValues);

        Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues);

        Task<bool> DeleteAsync(params object[] keyValues);

        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);

        IQueryable<T> Queryable();
    }
}
