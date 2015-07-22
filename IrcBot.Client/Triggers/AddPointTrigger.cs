using System;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Database.Infrastructure;
using IrcBot.Entities.Models;
using IrcBot.Database.UnitOfWork;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public sealed class AddPointTrigger : IAddPointTrigger
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPointService _pointService;
        private readonly IQuoteService _quoteService;

        public AddPointTrigger(IUnitOfWorkAsync unitOfWork, IPointService pointService, IQuoteService quoteService)
        {
            _unitOfWork = unitOfWork;
            _pointService = pointService;
            _quoteService = quoteService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !addpoint <quote number>");
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
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"{eventArgs.Data.Nick}: quote number {quoteId} does not exist");
                return;
            }

            var nick = quote.Author.Replace("_", "").Replace("-", "").Replace("\\", "");

            if (nick == "Unknown")
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"Quote {quoteId} was imported, use !claim {quoteId} if it is yours");
                return;
            }

            var utcNow = DateTime.UtcNow;

            _pointService.Insert(new Point
            {
                Nick = nick,
                Value = 1,
                Created = utcNow,
                Modified = utcNow,
                ObjectState = ObjectState.Added
            });

            _unitOfWork.SaveChanges();

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"Point added for quote {quoteId}");
        }
    }
}
