using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly IUnityContainer _container;
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

            _client.OnErrorMessage += ClientOnErrorMessage;
            _client.OnChannelMessage += ClientOnChannelMessage;
            _client.OnJoin += ClientOnJoin;
            _client.OnPart += ClientOnPart;
            _client.OnQuit += ClientOnQuit;

            _container = new UnityContainer();

            _container
                .RegisterType<IDataContextAsync, IrcBotContext>(new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepositoryAsync<AolSayMessage>, Repository<AolSayMessage>>()
                .RegisterType<IRepositoryAsync<ChannelActivity>, Repository<ChannelActivity>>()
                .RegisterType<IRepositoryAsync<Message>, Repository<Message>>()
                .RegisterType<IRepositoryAsync<Point>, Repository<Point>>()
                .RegisterType<IRepositoryAsync<QueuedCommand>, Repository<QueuedCommand>>()
                .RegisterType<IRepositoryAsync<Quote>, Repository<Quote>>()
                .RegisterType<IAolSayMessageService, AolSayMessageService>()
                .RegisterType<IChannelActivityService, ChannelActivityService>()
                .RegisterType<IMessageService, MessageService>()
                .RegisterType<IPointService, PointService>()
                .RegisterType<IQueuedCommandService, QueuedCommandService>()
                .RegisterType<IQuoteService, QuoteService>();

            _triggers = new Dictionary<string, ITrigger>
            {
                { "!ud", new UrbanDictionaryTrigger() },
                { "!addpoint", new AddPointTrigger(_container) },
                { "!takepoint", new TakePointTrigger(_container) },
                { "!points", new PointsTrigger(_container) },
                { "!addquote", new AddQuoteTrigger(_container) },
                { "!quote", new QuoteTrigger(_container) },
                { "!addquotepoint", new AddQuotePointTrigger(_container) },
                { "!takequotepoint", new TakeQuotePointTrigger(_container) },
                { "!topquotes", new TopQuotesTrigger(_container) },
                { "!quotestats", new QuoteStatsTrigger(_container) },
                { "!aolsay", new AolSayTrigger(_container) },
                { "!aolsaygen", new AolSayGeneratorTrigger() },
                { "!echo", new EchoTrigger() },
                { "!insult", new InsultTrigger() },
                { "!seen", new SeenTrigger(_container) },
                { "!scream", new DonScreamTrigger(false) },
                { "!SCREAM", new DonScreamTrigger(true) }
            };

            var timer = new System.Timers.Timer(10000);

            timer.Elapsed += (sender, args) =>
            {
                var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
                var queuedCommandService = _container.Resolve<IQueuedCommandService>();

                var commands = queuedCommandService.Query().OrderBy(o => o.OrderByDescending(x => x.Created)).Select().ToList();

                foreach (var command in commands)
                {
                    _client.SendMessage(SendType.Message, ConfigurationManager.AppSettings["Channel"], command.Command);

                    queuedCommandService.Delete(command);

                    unitOfWork.SaveChanges();
                }
            };

            timer.Enabled = false;
            timer.Start();
        }

        public void Start()
        {
            _client.Connect(new[]
            {
                ConfigurationManager.AppSettings["Server"]
            }, Int32.Parse(ConfigurationManager.AppSettings["ServerPort"]));

            _client.Login(new []
            {
                ConfigurationManager.AppSettings["Nick"],
                ConfigurationManager.AppSettings["NickAlt"]
            }, ConfigurationManager.AppSettings["RealName"]);
            _client.RfcJoin(ConfigurationManager.AppSettings["Channel"]);

            _client.SendMessage(SendType.Message, "nickserv", String.Format(
                "identify {0}", ConfigurationManager.AppSettings["NickServPassword"]));

            _client.Listen();
            _client.Disconnect();
        }

        private void ClientOnErrorMessage(object sender, IrcEventArgs ircEventArgs)
        { }

        private void ClientOnChannelMessage(object sender, IrcEventArgs ircEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var messageService = _container.Resolve<IMessageService>();

            var message = ircEventArgs.Data.Message;

            messageService.Insert(new Message
            {
                Content = message,
                Nick = ircEventArgs.Data.Nick,
                ObjectState = ObjectState.Added
            });

            unitOfWork.SaveChanges();

            if (!message.StartsWith("!"))
            {
                return;
            }

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

        private void ClientOnJoin(object sender, JoinEventArgs joinEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var channelActivityService = _container.Resolve<IChannelActivityService>();

            channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Join,
                Nick = joinEventArgs.Who,
                ObjectState = ObjectState.Added
            });

            unitOfWork.SaveChanges();
        }

        private void ClientOnPart(object sender, PartEventArgs partEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var channelActivityService = _container.Resolve<IChannelActivityService>();

            channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Part,
                Nick = partEventArgs.Who,
                ObjectState = ObjectState.Added
            });

            unitOfWork.SaveChanges();
        }

        private void ClientOnQuit(object sender, QuitEventArgs quitEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var channelActivityService = _container.Resolve<IChannelActivityService>();

            channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Quit,
                Nick = quitEventArgs.Who,
                ObjectState = ObjectState.Added
            });

            unitOfWork.SaveChanges();
        }
    }
}
