using System;

using Meebey.SmartIrc4net;

using IrcBot.Database.Infrastructure;
using IrcBot.Database.UnitOfWork;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AddQuoteTrigger : ITrigger
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IQuoteService _quoteService;

        public AddQuoteTrigger(IUnitOfWorkAsync unitOfWork, IQuoteService quoteService)
        {
            _unitOfWork = unitOfWork;
            _quoteService = quoteService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length == 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !addquote <content>");
                return;
            }

            var quote = new Quote
            {
                Author = eventArgs.Data.Nick,
                Content = String.Join(" ", triggerArgs),
                ObjectState = ObjectState.Added
            };

            _quoteService.Insert(quote);

            _unitOfWork.SaveChanges();

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                "Saved! http://cdnidle.azurewebsites.net/quotes/{0}", quote.Id));
        }
    }
}
