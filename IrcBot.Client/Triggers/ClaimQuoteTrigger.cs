using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Database.UnitOfWork;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class ClaimQuoteTrigger : IClaimQuoteTrigger
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IQuoteService _quoteService;

        public ClaimQuoteTrigger(IUnitOfWorkAsync unitOfWork, IQuoteService quoteService)
        {
            _unitOfWork = unitOfWork;
            _quoteService = quoteService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !claim <quote number>");
                return;
            }

            int quoteId;

            if (!int.TryParse(triggerArgs[0], out quoteId))
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"{eventArgs.Data.Nick}: that's not a quote number");
                return;
            }

            var quote = _quoteService.Find(quoteId);

            if (quote == null)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"{eventArgs.Data.Nick}: quote number {quoteId} doesn't exist");
                return;
            }

            if (quote.Author != "Unknown")
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"{eventArgs.Data.Nick}: that quote is already claimed");
                return;
            }

            if (quote.Author == eventArgs.Data.Nick)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"{eventArgs.Data.Nick}: you're already the author of quote {quoteId}");
                return;
            }

            quote.Author = eventArgs.Data.Nick;

            _quoteService.Update(quote);
            _unitOfWork.SaveChanges();
        }
    }
}
