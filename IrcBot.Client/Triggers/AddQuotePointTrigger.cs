using System;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Database.Infrastructure;
using IrcBot.Database.UnitOfWork;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AddQuotePointTrigger : ITrigger
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IQuoteService _quoteService;

        public AddQuotePointTrigger(IUnityContainer container)
        {
            _unitOfWork = container.Resolve<IUnitOfWorkAsync>();
            _quoteService = container.Resolve<IQuoteService>();
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !addquotepoint <quote #>");
                return;
            }

            int quoteId;

            if (Int32.TryParse(triggerArgs[0], out quoteId))
            {
                var quote = _quoteService.Find(quoteId);

                if (quote == null)
                {
                    client.SendMessage(SendType.Message, eventArgs.Data.Channel, "That quote doesn't exist");
                }
                else
                {
                    quote.Points++;
                    quote.ObjectState = ObjectState.Modified;

                    _quoteService.Update(quote);
                    _unitOfWork.SaveChanges();

                    client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                        "Quote {0} has {1} points", quote.Id, quote.Points));
                }
            }
            else
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !addquotepoint <quote #>");
            }
        }
    }
}
