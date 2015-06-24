using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers;
using IrcBot.Database.DataContext;
using IrcBot.Database.Entity;
using IrcBot.Database.Infrastructure;
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
        private readonly UnityContainer _container;

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

            _container = new UnityContainer();

            _container
                .RegisterType<IDataContextAsync, IrcBotContext>(new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepositoryAsync<Message>, Repository<Message>>()
                .RegisterType<IRepositoryAsync<Point>, Repository<Point>>()
                .RegisterType<IMessageService, MessageService>()
                .RegisterType<IPointService, PointService>();

            _triggers = new Dictionary<string, ITrigger>
            {
                { "!addpoint", new AddPointTrigger(_container.Resolve<IUnitOfWorkAsync>(), _container.Resolve<IPointService>()) },
                { "!takepoint", new TakePointTrigger(_container.Resolve<IUnitOfWorkAsync>(), _container.Resolve<IPointService>()) },
                { "!points", new PointsTrigger(_container.Resolve<IPointService>()) }
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

            if (message.StartsWith("!"))
            {
                var split = message.Split(new[] { ' ' });

                if (_triggers.ContainsKey(split[0]))
                {
                    _triggers[split[0]].Execute(_client, split.Skip(1).Take(split.Length - 1).ToArray());
                }
                else if (split.Length == 1 && split[0].Equals("!help"))
                {
                    _client.SendMessage(SendType.Message, ChannelName, String.Format("Commands: {0}",
                        String.Join(", ", _triggers.Select(x => x.Key).ToArray())));
                }
            }
            else
            {
                var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
                var messageService = _container.Resolve<IMessageService>();

                var utcNow = DateTime.UtcNow;

                messageService.Insert(new Message
                {
                    Content = message,
                    Nick = ircEventArgs.Data.Nick,
                    Created = utcNow,
                    Modified = utcNow,
                    ObjectState = ObjectState.Added
                });

                unitOfWork.SaveChanges();
            }
        }

        private void ClientOnOnQueryMessage(object sender, IrcEventArgs ircEventArgs)
        { }
    }
}
