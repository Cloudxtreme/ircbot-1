using System;
using System.Linq;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class QuoteTrigger : ITrigger
    {
        private readonly IQuoteService _quoteService;

        public QuoteTrigger(IUnityContainer container)
        {
            _quoteService = container.Resolve<IQuoteService>();
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            var query = String.Join(" ", triggerArgs);

            int quoteId;

            Quote quote;

            if (Int32.TryParse(query, out quoteId))
            {
                quote = _quoteService.Find(quoteId);
            }
            else if (query.StartsWith("*") && query.EndsWith("*"))
            {
                quote = _quoteService
                    .Query(x => x.Content.Contains(query.Substring(1, query.Length - 2)))
                    .OrderBy(x => x.OrderBy(o => Guid.NewGuid()))
                    .Select()
                    .FirstOrDefault();
            }
            else if (query.StartsWith("*") && !query.EndsWith("*"))
            {
                quote = _quoteService
                    .Query(x => x.Content.EndsWith(query.Substring(1, query.Length - 1)))
                    .OrderBy(x => x.OrderBy(o => Guid.NewGuid()))
                    .Select()
                    .FirstOrDefault();
            }
            else if (!query.StartsWith("*") && query.EndsWith("*"))
            {
                quote = _quoteService
                    .Query(x => x.Content.StartsWith(query.Substring(0, query.Length - 1)))
                    .OrderBy(x => x.OrderBy(o => Guid.NewGuid()))
                    .Select()
                    .FirstOrDefault();
            }
            else
            {
                quote = _quoteService
                    .Query()
                    .OrderBy(x => x.OrderBy(o => Guid.NewGuid()))
                    .Select()
                    .FirstOrDefault();
            }

            if (quote == null)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                    "Nothing found for {0}", query));
                return;
            }

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                "{0}: {1} ({2} points)", quote.Id, quote.Content, quote.Points));
        }
    }
}
