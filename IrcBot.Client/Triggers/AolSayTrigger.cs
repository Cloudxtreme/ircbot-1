using System;
using System.Linq;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AolSayTrigger : IAolSayTrigger
    {
        private readonly IAolSayMessageService _aolSayMessageService;

        public AolSayTrigger(IAolSayMessageService aolSayMessageService)
        {
            _aolSayMessageService = aolSayMessageService;
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !aolsay");
                return;
            }

            var aolSayMessage = _aolSayMessageService
                .Query()
                .OrderBy(x => x.OrderBy(o => Guid.NewGuid()))
                .Select()
                .FirstOrDefault();

            if (aolSayMessage != null)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, aolSayMessage.Content);
            }
        }
    }
}
