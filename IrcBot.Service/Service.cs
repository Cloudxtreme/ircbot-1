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
    public abstract class Service<T> : IService<T> where T : class, IObjectState
    {
        private readonly IRepositoryAsync<T> _repository;

        protected Service(IRepositoryAsync<T> repository)
        {
            _repository = repository;
        }

        public virtual T Find(params object[] keyValues)
        {
            return _repository.Find(keyValues);
        }

        public virtual IQueryable<T> SelectQuery(string query, params object[] parameters)
        {
            return _repository.SelectQuery(query, parameters).AsQueryable();
        }

        public virtual void Insert(T entity)
        {
            _repository.Insert(entity);
        }

        public virtual void InsertRange(IEnumerable<T> entities)
        {
            _repository.InsertRange(entities);
        }

        public virtual void InsertOrUpdateGraph(T entity)
        {
            _repository.InsertOrUpdateGraph(entity);
        }

        public virtual void InsertGraphRange(IEnumerable<T> entities)
        {
            _repository.InsertGraphRange(entities);
        }

        public virtual void Update(T entity)
        {
            _repository.Update(entity);
        }

        public virtual void Delete(object id)
        {
            _repository.Delete(id);
        }

        public virtual void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public virtual IQueryFluent<T> Query()
        {
            return _repository.Query();
        }

        public virtual IQueryFluent<T> Query(IQueryObject<T> queryObject)
        {
            return _repository.Query(queryObject);
        }

        public virtual IQueryFluent<T> Query(Expression<Func<T, bool>> query)
        {
            return _repository.Query(query);
        }

        public virtual async Task<T> FindAsync(params object[] keyValues)
        {
            return await _repository.FindAsync(keyValues);
        }

        public virtual async Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await _repository.FindAsync(cancellationToken, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await _repository.DeleteAsync(cancellationToken, keyValues);
        }

        public virtual IQueryable<T> Queryable()
        {
            return _repository.Queryable();
        }
    }
}
