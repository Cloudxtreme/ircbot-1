using System.Threading.Tasks;

using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers
{
    public interface ITrigger
    {
        void Execute(IrcClient client, string[] parameters);
    }
}
