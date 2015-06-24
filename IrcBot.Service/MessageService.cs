using IrcBot.Database.Repositories;
using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public class MessageService : Service<Message>, IMessageService
    {
        private readonly IRepositoryAsync<Message> _repository;

        public MessageService(IRepositoryAsync<Message> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
