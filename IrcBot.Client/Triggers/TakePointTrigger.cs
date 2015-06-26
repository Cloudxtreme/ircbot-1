﻿using System;
using System.Linq;
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

        private readonly string[] _knownNicks =
        {
            "rhaydeo", "NukeLaloosh", "mastadonn", "lewzer", "lazerbeast"
        };

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

            if (triggerArgs[0] == eventArgs.Data.Nick)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "You can't take points from yourself");
                return;
            }

            if (!_knownNicks.Contains(triggerArgs[0]))
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                    "Points can only be taken from: {0}", String.Join(", ", _knownNicks.OrderBy(x => x))));
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
