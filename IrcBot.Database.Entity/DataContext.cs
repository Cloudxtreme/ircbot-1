using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

using IrcBot.Database.DataContext;
using IrcBot.Database.Infrastructure;

namespace IrcBot.Database.Entity
{
    public class DataContext : DbContext, IDataContextAsync
    {
        private readonly Guid _instanceId;

        private bool _disposed;

        public DataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            _instanceId = Guid.NewGuid();

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public override int SaveChanges()
        {
            SyncObjectStatePreCommit();

            var changes = base.SaveChanges();

            SyncObjectStatePostCommit();

            return changes;
        }

        public override async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(CancellationToken.None);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SyncObjectStatePreCommit();

            var changesAsync = await base.SaveChangesAsync(cancellationToken);

            SyncObjectStatePostCommit();

            return changesAsync;
        }

        public void SyncObjectState<T>(T entity) where T : class, IObjectState
        {
            Entry(entity).State = StateHelper.ConvertState(entity.ObjectState);
        }

        private void SyncObjectStatePreCommit()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                entry.State = StateHelper.ConvertState(((IObjectState)entry.Entity).ObjectState);
            }
        }

        public void SyncObjectStatePostCommit()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                ((IObjectState)entry.Entity).ObjectState = StateHelper.ConvertState(entry.State);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                { }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
