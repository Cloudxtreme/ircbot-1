﻿using System;
using System.Text;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;

namespace IrcBot.Client.Triggers
{
    public class DonLoudScreamTrigger : IDonLoudScreamTrigger
    {
        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("B");

            for (int i = 0, n = random.Next(1, 8); i < n; i++)
            {
                stringBuilder.Append("W");
            }

            for (int i = 0, n = random.Next(1, 16); i < n; i++)
            {
                stringBuilder.Append("A");
            }

            for (int i = 0, n = random.Next(1, 16); i < n; i++)
            {
                stringBuilder.Append("U");
            }

            for (int i = 0, n = random.Next(1, 16); i < n; i++)
            {
                stringBuilder.Append("G");
            }

            for (int i = 0, n = random.Next(1, 8); i < n; i++)
            {
                stringBuilder.Append("H");
            }

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, stringBuilder.ToString());
        }
    }
}
