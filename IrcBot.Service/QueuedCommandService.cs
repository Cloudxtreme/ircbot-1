using IrcBot.Database.Repositories;
using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public class QueuedCommandService : Service<QueuedCommand>, IQueuedCommandService
    {
        public QueuedCommandService(IRepositoryAsync<QueuedCommand> repository)
            : base(repository)
        { }
    }
}
