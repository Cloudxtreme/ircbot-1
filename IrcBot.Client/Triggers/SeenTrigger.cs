using System;
using System.Linq;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class SeenTrigger : ITrigger
    {
        private readonly IChannelActivityService _channelActivityService;

        public SeenTrigger(IUnityContainer container)
        {
            _channelActivityService = container.Resolve<IChannelActivityService>();
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length < 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !seen <nick>");
                return;
            }

            var nick = String.Join(" ", triggerArgs);

            if (client.GetChannel(eventArgs.Data.Channel).Users.Contains(nick))
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format("{0} is here now", nick));
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
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format("{0} has never been here", nick));
                return;
            }

            if (lastPart == null)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format("I've never seen {0} leave", nick));
                return;
            }

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var utcOffset = timeZoneInfo.GetUtcOffset(lastPart.Created);

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format(
                "{0} was last here on {1}", nick, lastPart.Created.Subtract(utcOffset)));
        }
    }
}
