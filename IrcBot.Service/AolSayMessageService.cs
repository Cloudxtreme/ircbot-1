using IrcBot.Database.Repositories;
using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public class AolSayMessageService : Service<AolSayMessage>, IAolSayMessageService
    {
        private readonly IRepositoryAsync<AolSayMessage> _repository;

        public AolSayMessageService(IRepositoryAsync<AolSayMessage> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
