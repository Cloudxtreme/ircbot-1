using System;
using System.Data;

using IrcBot.Database.Infrastructure;
using IrcBot.Database.Repositories;

namespace IrcBot.Database.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);

        bool Commit();

        void Rollback();

        IRepository<T> Repository<T>() where T : class, IObjectState;

        void Dispose(bool disposing);
    }
}
