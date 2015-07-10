using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class PointsTrigger : ITrigger
    {
        private readonly IPointService _pointService;

        public PointsTrigger(IUnityContainer container)
        {
            _pointService = container.Resolve<IPointService>();
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !points");
                return;
            }

            var dictionary = new Dictionary<string, int>();
            var points = _pointService.Query().Select();

            foreach (var point in points)
            {
                if (!dictionary.ContainsKey(point.Nick))
                {
                    dictionary.Add(point.Nick, 0);
                }

                dictionary[point.Nick] += point.Value;
            }

            foreach (var kvp in dictionary.OrderByDescending(x => x.Value))
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, String.Format("{0}: {1}", kvp.Key, kvp.Value));
            }
        }
    }
}
