using IrcBot.Database.Repositories;
using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IRepositoryAsync<User> _repository; 

        public UserService(IRepositoryAsync<User> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
