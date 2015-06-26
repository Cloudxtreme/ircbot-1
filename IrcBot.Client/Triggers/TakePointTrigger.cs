using System;

using Meebey.SmartIrc4net;

using IrcBot.Database.Infrastructure;
using IrcBot.Database.UnitOfWork;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public sealed class TakePointTrigger : ITrigger
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPointService _pointService;

        public TakePointTrigger(IUnitOfWorkAsync unitOfWork, IPointService pointService)
        {
            _unitOfWork = unitOfWork;
            _pointService = pointService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !takepoint <nick>");
                return;
            }

            var utcNow = DateTime.UtcNow;

            _pointService.Insert(new Point
            {
                Nick = triggerArgs[0],
                Value = -1,
                Created = utcNow,
                Modified = utcNow,
                ObjectState = ObjectState.Added
            });

            _unitOfWork.SaveChanges();

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                "{0} has {1} points", triggerArgs[0], _pointService.Count(triggerArgs[0])));
        }
    }
}
