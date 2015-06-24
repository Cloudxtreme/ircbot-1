using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using LinqKit;

using IrcBot.Database.DataContext;
using IrcBot.Database.Infrastructure;
using IrcBot.Database.Repositories;
using IrcBot.Database.UnitOfWork;

namespace IrcBot.Database.Entity
{
    public class Repository<T> : IRepositoryAsync<T> where T : class, IObjectState
    {
        private readonly IDataContextAsync _context;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly DbSet<T> _dbSet;

        public Repository(IDataContextAsync context, IUnitOfWorkAsync unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;

            var dbContext = context as DbContext;

            if (dbContext != null)
            {
                _dbSet = dbContext.Set<T>();
            }
            else
            {
                var fakeContext = context as FakeDbContext;

                if (fakeContext != null)
                {
                    _dbSet = fakeContext.Set<T>();
                }
            }
        }

        public virtual T Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public virtual IQueryable<T> SelectQuery(string query, params object[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        public virtual void Insert(T entity)
        {
            entity.ObjectState = ObjectState.Added;

            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }

        public virtual void InsertRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        public virtual void InsertGraphRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void Update(T entity)
        {
            entity.ObjectState = ObjectState.Modified;

            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }

        public virtual void Delete(object id)
        {
            var entity = _dbSet.Find(id);

            Delete(entity);
        }

        public virtual void Delete(T entity)
        {
            entity.ObjectState = ObjectState.Deleted;

            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }

        public IQueryFluent<T> Query()
        {
            return new QueryFluent<T>(this);
        }

        public virtual IQueryFluent<T> Query(IQueryObject<T> queryObject)
        {
            return new QueryFluent<T>(this, queryObject);
        }

        public virtual IQueryFluent<T> Query(Expression<Func<T, bool>> query)
        {
            return new QueryFluent<T>(this, query);
        }

        public IQueryable<T> Queryable()
        {
            return _dbSet;
        }

        public IRepository<TRepository> GetRepository<TRepository>() where TRepository : class, IObjectState
        {
            return _unitOfWork.Repository<TRepository>();
        }

        public virtual async Task<T> FindAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public virtual async Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await _dbSet.FindAsync(cancellationToken, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await FindAsync(cancellationToken, keyValues);

            if (entity == null)
            {
                return false;
            }

            entity.ObjectState = ObjectState.Deleted;
            _dbSet.Attach(entity);

            return true;
        }

        internal IQueryable<T> Select(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }

            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        internal async Task<IEnumerable<T>> SelectAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            return await Select(filter, orderBy, includes, page, pageSize).ToListAsync();
        }

        public virtual void InsertOrUpdateGraph(T entity)
        {
            SyncObjectGraph(entity);

            _entitiesChecked = null;
            _dbSet.Attach(entity);
        }

        HashSet<object> _entitiesChecked;

        private void SyncObjectGraph(object entity)
        {
            if (_entitiesChecked == null)
            {
                _entitiesChecked = new HashSet<object>();
            }

            if (_entitiesChecked.Contains(entity))
            {
                return;
            }

            _entitiesChecked.Add(entity);

            var objectState = entity as IObjectState;

            if (objectState != null && objectState.ObjectState == ObjectState.Added)
            {
                _context.SyncObjectState((IObjectState)entity);
            }

            foreach (var property in entity.GetType().GetProperties())
            {
                var trackableReference = property.GetValue(entity, null) as IObjectState;

                if (trackableReference != null)
                {
                    if (trackableReference.ObjectState == ObjectState.Added)
                    {
                        _context.SyncObjectState((IObjectState)entity);
                    }

                    SyncObjectGraph(property.GetValue(entity, null));
                }

                var items = property.GetValue(entity, null) as IEnumerable<IObjectState>;

                if (items == null)
                {
                    continue;
                }

                foreach (var item in items)
                {
                    SyncObjectGraph(item);
                }
            }
        }
    }
}
