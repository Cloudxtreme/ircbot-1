using System;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Database.Infrastructure;
using IrcBot.Database.UnitOfWork;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AddQuoteTrigger : IAddQuoteTrigger
    {
        private static readonly string[] SaveMessages =
        {
            "Bazinga", "Badabing", "bleep-bloop-bop", "Well that was witty",
            "trollololol", "lol", "(this better be about don)", "ROFLJO",
            "buurrrrrppppp", "BWWWAAAAUUUUUGGGHHH", "Level up", "Snap",
            "bing bong ding dong", "Here comes a new challenger", "FINISH HIM",
            "All your base are belong to us", "Nifty", "oooOOooOOooOOOoOOoO"
        };

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
                Content = string.Join(" ", triggerArgs),
                ObjectState = ObjectState.Added
            };

            _quoteService.Insert(quote);

            _unitOfWork.SaveChanges();

            client.SendMessage(SendType.Message, eventArgs.Data.Channel,
                $"{SaveMessages[new Random(DateTime.Now.Millisecond).Next(SaveMessages.Length)]}! Quote {quote.Id} has been added");
        }
    }
}
