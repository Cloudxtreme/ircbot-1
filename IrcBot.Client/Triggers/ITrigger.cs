using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers
{
    public interface ITrigger
    {
        void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs);
    }
}
