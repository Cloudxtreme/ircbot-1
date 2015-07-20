using System;
using System.Linq;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AolSayTrigger : ITrigger
    {
        private readonly IAolSayMessageService _aolSayMessageService;

        public AolSayTrigger(IUnityContainer container)
        {
            _aolSayMessageService = container.Resolve<IAolSayMessageService>();
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
