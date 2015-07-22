using System;
using System.Linq;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class SeenTrigger : ISeenTrigger
    {
        private readonly IChannelActivityService _channelActivityService;

        public SeenTrigger(IChannelActivityService channelActivityService)
        {
            _channelActivityService = channelActivityService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length < 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !seen <nick>");
                return;
            }

            var nick = string.Join(" ", triggerArgs);

            if (client.GetChannel(eventArgs.Data.Channel).Users.Contains(nick))
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"{nick} is here now");
                return;
            }

            var lastJoin = _channelActivityService.Query(x =>
                x.Action == UserAction.Join && x.Nick == nick)
                .OrderBy(o => o.OrderByDescending(x => x.Created))
                .Select()
                .Take(1)
                .SingleOrDefault();

            var lastPart = _channelActivityService.Query(x =>
                (x.Action == UserAction.Part || x.Action == UserAction.Quit) && x.Nick == nick)
                .OrderBy(o => o.OrderByDescending(x => x.Created))
                .Select()
                .Take(1)
                .SingleOrDefault();

            if (lastJoin == null)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"{nick} has never been here");
                return;
            }

            if (lastPart == null)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, $"I've never seen {nick} leave");
                return;
            }

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var utcOffset = timeZoneInfo.GetUtcOffset(lastPart.Created);

            client.SendMessage(SendType.Message, eventArgs.Data.Channel,
                $"{nick} was last here on {lastPart.Created.Add(utcOffset)}");
        }
    }
}
