using Meebey.SmartIrc4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IrcBot.Client.Triggers
{
    public class EchoTrigger : ITrigger
    {
        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            var echoString = String.Join(" ", triggerArgs);

            if (triggerArgs.Length != 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !echo need to type something dumbass");
                return;
            }

            client.SendMessage(SendType.Message, eventArgs.Data.Channel, echoString);
        }
    }
}
