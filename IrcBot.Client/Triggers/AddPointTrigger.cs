using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers
{
    public sealed class AddPointTrigger : ITrigger
    {
        public void Execute(IrcClient client, string[] parameters)
        {
            client.SendMessage(SendType.Message, "#cdnidle", "Testing responding to triggers");
        }
    }
}
