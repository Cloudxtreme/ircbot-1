using System;
using System.Collections.Generic;

using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers
{
    public class InsultTrigger : ITrigger
    {          
        private static readonly List<string> Insult = new List<string>

        {
            "you suck",
            "has a little pecker",
            "You're so dumb you think manual labor is a Mexican!",
            "You're so ugly you'd make a train take a dirt road!",
            "Is that an accent, or is your mouth just full of sperm?",
            "I  hear that when your mother first saw you she decided to leave you on the front steps of a police station while she turned herself in"
        };

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length < 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !insult <nick>");
                return;
            }

            var random = new Random(DateTime.UtcNow.Millisecond);

            var insultString = String.Join(" ", Insult[random.Next(0, Insult.Count)]);

            var userString = String.Join(" ", triggerArgs);

            var finalString = String.Join(" ", (userString) , (insultString));

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, finalString);

        }
    }
}
