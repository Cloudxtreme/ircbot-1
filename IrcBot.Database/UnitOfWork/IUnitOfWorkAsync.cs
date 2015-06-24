using System.Threading;
using System.Threading.Tasks;

using IrcBot.Database.Infrastructure;
using IrcBot.Database.Repositories;

namespace IrcBot.Database.UnitOfWork
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        IRepositoryAsync<T> RepositoryAsync<T>() where T : class, IObjectState;
    }
}
