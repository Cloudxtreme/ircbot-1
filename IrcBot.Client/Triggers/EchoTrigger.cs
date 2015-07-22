using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;

namespace IrcBot.Client.Triggers
{
    public class EchoTrigger : IEchoTrigger
    {
        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length < 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !echo <content>");
                return;
            }

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, string.Join(" ", triggerArgs));
        }
    }
}
