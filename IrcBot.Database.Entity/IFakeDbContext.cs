using System.Data.Entity;

using IrcBot.Database.DataContext;

namespace IrcBot.Database.Entity
{
    public interface IFakeDbContext : IDataContextAsync
    {
        DbSet<T> Set<T>() where T : class;

        void AddFakeDbSet<T, TFakeDbSet>()
            where T : BaseEntity, new()
            where TFakeDbSet : FakeDbSet<T>, IDbSet<T>, new();
    }
}
