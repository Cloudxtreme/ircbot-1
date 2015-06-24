using System;
using System.Text;

using Meebey.SmartIrc4net;

namespace IrcBot.Client
{
    public class IrcBot
    {
        private const string ChannelName = "#cdnidle";

        private readonly IrcClient _client;

        public IrcBot()
        {
            _client = new IrcClient
            {
                Encoding = Encoding.UTF8,
                SendDelay = 200,
                ActiveChannelSyncing = true,
            };

            _client.OnErrorMessage += ClientOnOnErrorMessage;
            _client.OnChannelMessage += ClientOnOnChannelMessage;
            _client.OnQueryMessage += ClientOnOnQueryMessage;
        }

        public void Start()
        {
            _client.Connect(new[] { "irc.freenode.net" }, 6667);
            _client.Login("shoryuken", "https://github.com/adamstirtan/ircbot");
            _client.RfcJoin(ChannelName);

            _client.Listen();
            _client.Disconnect();
        }

        private void ClientOnOnErrorMessage(object sender, IrcEventArgs ircEventArgs)
        { }

        private void ClientOnOnChannelMessage(object sender, IrcEventArgs ircEventArgs)
        {

        }

        private void ClientOnOnQueryMessage(object sender, IrcEventArgs ircEventArgs)
        {
        }
    }
}
