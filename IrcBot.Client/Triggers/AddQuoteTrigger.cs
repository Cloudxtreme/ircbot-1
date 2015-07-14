using System;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Database.Infrastructure;
using IrcBot.Database.UnitOfWork;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AddQuoteTrigger : ITrigger
    {
        private static readonly string[] SaveMessages =
        {
            "Bazinga", "Badabing", "bleep-bloop-bop", "Well that was witty",
            "trollololol", "lol", "(this better be about don)", "ROFLJO",
            "buurrrrrppppp", "BWWWAAAAUUUUUGGGHHH", "Level up", "Snap"
        };

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IQuoteService _quoteService;

        public AddQuoteTrigger(IUnityContainer container)
        {
            _unitOfWork = container.Resolve<IUnitOfWorkAsync>();
            _quoteService = container.Resolve<IQuoteService>();
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
                "{0}! Quote {1} has been added", SaveMessages[new Random(DateTime.Now.Millisecond).Next(SaveMessages.Length)], quote.Id));
        }
    }
}
