﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers;
using IrcBot.Database.DataContext;
using IrcBot.Database.Entity;
using IrcBot.Database.Repositories;
using IrcBot.Database.UnitOfWork;
using IrcBot.Entities;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Client
{
    public class IrcBot
    {
        private const string ChannelName = "#cdnidle";

        private readonly IrcClient _client;
        private readonly Dictionary<string, ITrigger> _triggers; 

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

            var container = new UnityContainer();

            container
                .RegisterType<IDataContextAsync, IrcBotContext>(new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepositoryAsync<Message>, Repository<Message>>()
                .RegisterType<IRepositoryAsync<Point>, Repository<Point>>()
                .RegisterType<IMessageService, MessageService>()
                .RegisterType<IPointService, PointService>();

            _triggers = new Dictionary<string, ITrigger>
            {
                { "!addpoint", new AddPointTrigger(container.Resolve<IUnitOfWorkAsync>(), container.Resolve<IPointService>()) },
                { "!takepoint", new TakePointTrigger(container.Resolve<IUnitOfWorkAsync>(), container.Resolve<IPointService>()) },
                { "!points", new PointsTrigger(container.Resolve<IPointService>()) }
            };
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
            var message = ircEventArgs.Data.Message;

            if (!message.StartsWith("!"))
            {
                return;
            }

            var split = message.Split(new[] { ' ' });

            if (_triggers.ContainsKey(split[0]))
            {
                _triggers[split[0]].Execute(_client, split.Skip(1).Take(split.Length - 1).ToArray());
            }
        }

        private void ClientOnOnQueryMessage(object sender, IrcEventArgs ircEventArgs)
        { }
    }
}
