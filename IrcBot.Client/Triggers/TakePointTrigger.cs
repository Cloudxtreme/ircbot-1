using System;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Database.Infrastructure;
using IrcBot.Database.UnitOfWork;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public sealed class TakePointTrigger : ITakePointTrigger
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPointService _pointService;
        private readonly IQuoteService _quoteService;

        public TakePointTrigger(IUnitOfWorkAsync unitOfWork, IPointService pointService, IQuoteService quoteService)
        {
            _unitOfWork = unitOfWork;
            _pointService = pointService;
            _quoteService = quoteService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !takepoint <quote number>");
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

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"Point taken for quote {quoteId}");
        }
    }
}
