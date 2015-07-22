using System;
using System.Collections.Generic;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;

namespace IrcBot.Client.Triggers
{
    public class InsultTrigger : IInsultTrigger
    {          
        private static readonly List<string> Insult = new List<string>
        {
            "You're so dumb you think manual labor is a Mexican!",
            "You're so ugly you'd make a train take a dirt road!",
            "Is that an accent, or is your mouth just full of sperm?",
            "You were born because your mother didn't believe in abortion; now she believes in infanticide.",
            "No one should be punished for accident of birth but you look too much like a wreck not to be.",
            "Yours was an unnatural birth; you came from a human being.",
            "I admire your because I've never had the courage it takes to be a liar, a thief and a cheat.",
            "Is your ass jealous of the amount of shit that just came out of your mouth?",
            "Your birth certificate is an apology letter from the condom factory.",
            "Your family tree must be a cactus because everybody on it is a prick.",
            "Yo're so ugly, when your mom dropped you off at school she got a fine for littering.",
            "Which sexual position produces the ugliest children? Ask your mother.",
            "If bullshit could float...you'd be the Admiral of the fleet!",
            "Aha, I see the Fuck-Up Fairy has visited us again!",
            "What's the difference between your girlfriend and a walrus? One has a moustache and smells of fish and the other is a walrus."
        };

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length < 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !insult <nick>");
                return;
            }

            var random = new Random(DateTime.Now.Millisecond);

            client.SendMessage(SendType.Message, eventArgs.Data.Channel,
                $"{string.Join(" ", triggerArgs)}: {Insult[random.Next(0, Insult.Count)]}");
        }
    }
}
