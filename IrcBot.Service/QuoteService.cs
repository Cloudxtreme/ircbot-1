using IrcBot.Database.Repositories;
using IrcBot.Entities.Models;

namespace IrcBot.Service
{
    public class QuoteService : Service<Quote>, IQuoteService
    {
        public QuoteService(IRepositoryAsync<Quote> repository)
            : base(repository)
        { }
    }
}
