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

            _client.OnErrorMessage += ClientOnErrorMessage;
            _client.OnChannelMessage += ClientOnChannelMessage;
            _client.OnQueryMessage += ClientOnQueryMessage;
            _client.OnJoin += ClientOnJoin;
            _client.OnPart += ClientOnPart;
            _client.OnQuit += ClientOnQuit;

            _container = new UnityContainer();

            _container
                .RegisterType<IDataContextAsync, IrcBotContext>(new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepositoryAsync<ChannelActivity>, Repository<ChannelActivity>>()
                .RegisterType<IRepositoryAsync<Message>, Repository<Message>>()
                .RegisterType<IRepositoryAsync<Point>, Repository<Point>>()
                .RegisterType<IRepositoryAsync<Quote>, Repository<Quote>>()
                .RegisterType<IChannelActivityService, ChannelActivityService>()
                .RegisterType<IMessageService, MessageService>()
                .RegisterType<IPointService, PointService>()
                .RegisterType<IQuoteService, QuoteService>();

            _triggers = new Dictionary<string, ITrigger>
            {
                { "!ud", new UrbanDictionaryTrigger() },
                { "!addpoint", new AddPointTrigger(_container.Resolve<IUnitOfWorkAsync>(), _container.Resolve<IPointService>()) },
                { "!takepoint", new TakePointTrigger(_container.Resolve<IUnitOfWorkAsync>(), _container.Resolve<IPointService>()) },
                { "!points", new PointsTrigger(_container.Resolve<IPointService>()) },
                { "!addquote", new AddQuoteTrigger(_container.Resolve<IUnitOfWorkAsync>(), _container.Resolve<IQuoteService>()) },
                { "!quote", new QuoteTrigger(_container.Resolve<IQuoteService>()) },
                { "!aolsay", new AolSayTrigger() },
                { "!echo", new EchoTrigger() },
                { "!insult", new InsultTrigger() },
                { "!seen", new SeenTrigger(_container.Resolve<IChannelActivityService>()) }
            };
        }

        public void Start()
        {
            _client.Connect(new[] { "irc.freenode.net" }, 6667);
            _client.Login(new [] { "shoryuken", "shoryuken_" }, "https://github.com/adamstirtan/ircbot");
            _client.RfcJoin(ChannelName);

            _client.Listen();
            _client.Disconnect();
        }

        private void ClientOnErrorMessage(object sender, IrcEventArgs ircEventArgs)
        { }

        private void ClientOnChannelMessage(object sender, IrcEventArgs ircEventArgs)
        {
            var message = ircEventArgs.Data.Message;

            if (message.StartsWith("!"))
            {
                var split = message.Split(new[] { ' ' });

                if (_triggers.ContainsKey(split[0]))
                {
                    _triggers[split[0]].Execute(
                        _client,
                        ircEventArgs,
                        split.Skip(1).Take(split.Length - 1).ToArray());
                }
                else if (split.Length == 1 && split[0].Equals("!help"))
                {
                    _client.SendMessage(SendType.Message, ircEventArgs.Data.Channel, String.Format("Commands: {0}",
                        String.Join(", ", _triggers.Select(x => x.Key).OrderBy(x => x).ToArray())));
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

        private void ClientOnQueryMessage(object sender, IrcEventArgs ircEventArgs)
        { }

        private void ClientOnJoin(object sender, JoinEventArgs joinEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var channelActivityService = _container.Resolve<IChannelActivityService>();

            var utcNow = DateTime.UtcNow;

            unitOfWork.BeginTransaction();

            channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Join,
                Nick = joinEventArgs.Who,
                Created = utcNow,
                Modified = utcNow,
                ObjectState = ObjectState.Added
            });

            unitOfWork.SaveChanges();
            unitOfWork.Commit();
        }

        private void ClientOnPart(object sender, PartEventArgs partEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var channelActivityService = _container.Resolve<IChannelActivityService>();

            var utcNow = DateTime.UtcNow;

            unitOfWork.BeginTransaction();

            channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Part,
                Nick = partEventArgs.Who,
                Created = utcNow,
                Modified = utcNow,
                ObjectState = ObjectState.Added
            });

            unitOfWork.SaveChanges();
            unitOfWork.Commit();
        }

        private void ClientOnQuit(object sender, QuitEventArgs quitEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var channelActivityService = _container.Resolve<IChannelActivityService>();

            var utcNow = DateTime.UtcNow;

            unitOfWork.BeginTransaction();

            channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Quit,
                Nick = quitEventArgs.Who,
                Created = utcNow,
                Modified = utcNow,
                ObjectState = ObjectState.Added
            });

            unitOfWork.SaveChanges();
            unitOfWork.Commit();
        }
    }
}
