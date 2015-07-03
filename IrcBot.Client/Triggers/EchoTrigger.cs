using System;

using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers
{
    public class EchoTrigger : ITrigger
    {
        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length < 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !echo need to type something dumbass");
                return;
            }

            var echoString = String.Join(" ", triggerArgs);

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, echoString);
        }
    }
}
