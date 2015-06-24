using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public interface IPointService : IService<Point>
    {
        int Count(string nick);
    }
}
