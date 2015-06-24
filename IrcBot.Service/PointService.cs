using System.Linq;

using IrcBot.Database.Repositories;
using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public class PointService : Service<Point>, IPointService
    {
        private readonly IRepositoryAsync<Point> _repository;

        public PointService(IRepositoryAsync<Point> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public int Count(string nick)
        {
            return _repository.Query(x => x.Nick == nick).Select().Sum(x => x.Value);
        }
    }
}
