using System;

using IrcBot.Database.Infrastructure;

namespace IrcBot.Database.DataContext
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();

        void SyncObjectState<T>(T entity) where T : class, IObjectState;

        void SyncObjectStatePostCommit();
    }
}
