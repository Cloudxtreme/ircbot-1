using IrcBot.Database.Repositories;
using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public class ChannelActivityService : Service<ChannelActivity>, IChannelActivityService
    {
        public ChannelActivityService(IRepositoryAsync<ChannelActivity> repository)
            : base(repository)
        { }
    }
}
