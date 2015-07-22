using System.Linq;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class QuoteStatsTrigger : IQuoteStatsTrigger
    {
        private readonly IQuoteService _quoteService;

        public QuoteStatsTrigger(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            var query = string.Join(" ", triggerArgs);

            int quotes;

            if (query.StartsWith("*") && query.EndsWith("*"))
            {
                quotes = _quoteService
                    .Query(x => x.Content.Contains(query.Substring(1, query.Length - 2)))
                    .Select()
                    .Count();
            }
            else if (query.StartsWith("*") && !query.EndsWith("*"))
            {
                quotes = _quoteService
                    .Query(x => x.Content.EndsWith(query.Substring(1, query.Length - 1)))
                    .Select()
                    .Count();
            }
            else if (!query.StartsWith("*") && query.EndsWith("*"))
            {
                quotes = _quoteService
                    .Query(x => x.Content.StartsWith(query.Substring(0, query.Length - 1)))
                    .Select()
                    .Count();
            }
            else if (!string.IsNullOrEmpty(query))
            {
                quotes = _quoteService
                    .Query(x => x.Content == query)
                    .Select()
                    .Count();
            }
            else
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"There are {_quoteService.Query().Select().Count()} quotes");
                return;
            }

            switch (quotes)
            {
                case 0:
                    client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"There are no quotes matching {query}");
                    break;
                case 1:
                    client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"There is 1 quote matching {query}");
                    break;
                default:
                    client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"There are {quotes} quotes matching {query}");
                    break;
            }
        }
    }
}
