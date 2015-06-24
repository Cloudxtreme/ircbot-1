using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using IrcBot.Database.Infrastructure;

namespace IrcBot.Database.Entity
{
    public abstract class FakeDbSet<T> : DbSet<T>, IDbSet<T> where T : BaseEntity, new()
    {
        private readonly ObservableCollection<T> _items;
        private readonly IQueryable _query;

        protected FakeDbSet()
        {
            _items = new ObservableCollection<T>();
            _query = _items.AsQueryable();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public Expression Expression
        {
            get { return _query.Expression; }
        }

        public Type ElementType
        {
            get { return _query.ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return _query.Provider; }
        }

        public override T Add(T entity)
        {
            _items.Add(entity);

            return entity;
        }

        public override T Remove(T entity)
        {
            _items.Remove(entity);

            return entity;
        }

        public override T Attach(T entity)
        {
            switch (entity.ObjectState)
            {
                case ObjectState.Modified:
                    _items.Remove(entity);
                    _items.Add(entity);
                    break;
                case ObjectState.Deleted:
                    _items.Remove(entity);
                    break;
                case ObjectState.Unchanged:
                case ObjectState.Added:
                    _items.Add(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return entity;
        }

        public override T Create()
        {
            return new T();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<T> Local
        {
            get { return _items; }
        }
    }
}
