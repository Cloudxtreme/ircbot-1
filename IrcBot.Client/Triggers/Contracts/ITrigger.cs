using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers.Contracts
{
    public interface ITrigger
    {
        void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs);
    }
}
