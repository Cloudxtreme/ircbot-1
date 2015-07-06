using System;
using System.Linq;

using Meebey.SmartIrc4net;

using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class TopQuotesTrigger : ITrigger
    {
        private readonly IQuoteService _quoteService;

        public TopQuotesTrigger(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length > 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !topquotes");
                return;
            }

            var quotes = _quoteService
                .Query()
                .OrderBy(o => o.OrderByDescending(x => x.Points))
                .Select()
                .Take(5);

            foreach (var quote in quotes)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                    "{0}: {1} ({2} points)", quote.Id, quote.Content, quote.Points));
            }
        }
    }
}
