using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers
{
    public class DonScreamTrigger : ITrigger
    {
        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            client.SendMessage(SendType.Message, eventArgs.Data.Channel, "BWWAAAAAAUUUUUGGGGGHH");
        }
    }
}
