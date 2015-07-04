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

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMessageService _messageService;
        private readonly IQueuedCommandService _queuedCommandService;
        private readonly IChannelActivityService _channelActivityService;

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

            var container = new UnityContainer();

            container
                .RegisterType<IDataContextAsync, IrcBotContext>(new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new ContainerControlledLifetimeManager())
                .RegisterType<IRepositoryAsync<ChannelActivity>, Repository<ChannelActivity>>()
                .RegisterType<IRepositoryAsync<Message>, Repository<Message>>()
                .RegisterType<IRepositoryAsync<Point>, Repository<Point>>()
                .RegisterType<IRepositoryAsync<QueuedCommand>, Repository<QueuedCommand>>()
                .RegisterType<IRepositoryAsync<Quote>, Repository<Quote>>()
                .RegisterType<IChannelActivityService, ChannelActivityService>()
                .RegisterType<IMessageService, MessageService>()
                .RegisterType<IPointService, PointService>()
                .RegisterType<IQueuedCommandService, QueuedCommandService>()
                .RegisterType<IQuoteService, QuoteService>();

            _triggers = new Dictionary<string, ITrigger>
            {
                { "!ud", new UrbanDictionaryTrigger() },
                { "!addpoint", new AddPointTrigger(container.Resolve<IUnitOfWorkAsync>(), container.Resolve<IPointService>()) },
                { "!takepoint", new TakePointTrigger(container.Resolve<IUnitOfWorkAsync>(), container.Resolve<IPointService>()) },
                { "!points", new PointsTrigger(container.Resolve<IPointService>()) },
                { "!addquote", new AddQuoteTrigger(container.Resolve<IUnitOfWorkAsync>(), container.Resolve<IQuoteService>()) },
                { "!quote", new QuoteTrigger(container.Resolve<IQuoteService>()) },
                { "!quotestats", new QuoteStatsTrigger(container.Resolve<IQuoteService>()) },
                { "!aolsay", new AolSayTrigger() },
                { "!echo", new EchoTrigger() },
                { "!insult", new InsultTrigger() },
                { "!seen", new SeenTrigger(container.Resolve<IChannelActivityService>()) }
            };

            _unitOfWork = container.Resolve<IUnitOfWorkAsync>();
            _messageService = container.Resolve<IMessageService>();
            _queuedCommandService = container.Resolve<IQueuedCommandService>();
            _channelActivityService = container.Resolve<IChannelActivityService>();

            var timer = new System.Timers.Timer(10000);

            timer.Elapsed += (sender, args) =>
            {
                var commands = _queuedCommandService.Query().OrderBy(o => o.OrderByDescending(x => x.Created)).Select().ToList();

                foreach (var command in commands)
                {
                    _client.SendMessage(SendType.Message, ChannelName, command.Command);

                    _queuedCommandService.Delete(command);
                    
                    _unitOfWork.SaveChanges();
                }
            };

            timer.Enabled = false;
            timer.Start();
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

            _messageService.Insert(new Message
            {
                Content = message,
                Nick = ircEventArgs.Data.Nick,
                ObjectState = ObjectState.Added
            });

            _unitOfWork.SaveChanges();

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
            _channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Join,
                Nick = joinEventArgs.Who,
                ObjectState = ObjectState.Added
            });

            _unitOfWork.SaveChanges();
        }

        private void ClientOnPart(object sender, PartEventArgs partEventArgs)
        {
            _channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Part,
                Nick = partEventArgs.Who,
                ObjectState = ObjectState.Added
            });

            _unitOfWork.SaveChanges();
        }

        private void ClientOnQuit(object sender, QuitEventArgs quitEventArgs)
        {
            _channelActivityService.Insert(new ChannelActivity
            {
                Action = UserAction.Quit,
                Nick = quitEventArgs.Who,
                ObjectState = ObjectState.Added
            });

            _unitOfWork.SaveChanges();
        }
    }
}
