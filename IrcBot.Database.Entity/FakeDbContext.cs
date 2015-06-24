using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

using IrcBot.Database.Infrastructure;

namespace IrcBot.Database.Entity
{
    public abstract class FakeDbContext : IFakeDbContext
    {
        private readonly Dictionary<Type, object> _fakeDbSets;

        protected FakeDbContext()
        {
            _fakeDbSets = new Dictionary<Type, object>();
        }

        public int SaveChanges()
        {
            return default(int);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return new Task<int>(() => default(int));
        }

        public Task<int> SaveChangesAsync()
        {
            return new Task<int>(() => default(int));
        }

        public DbSet<T> Set<T>() where T : class
        {
            return (DbSet<T>)_fakeDbSets[typeof(T)];
        }

        public void AddFakeDbSet<T, TFakeDbSet>()
            where T : BaseEntity, new()
            where TFakeDbSet : FakeDbSet<T>, IDbSet<T>, new()
        {
            _fakeDbSets.Add(typeof(T), Activator.CreateInstance<TFakeDbSet>());
        }

        public void SyncObjectState<T>(T entity) where T : class, IObjectState
        { }

        public void SyncObjectStatePostCommit()
        { }

        public void Dispose()
        { }
    }
}
