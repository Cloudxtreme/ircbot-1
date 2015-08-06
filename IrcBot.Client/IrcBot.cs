using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers;
using IrcBot.Client.Triggers.Contracts;
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
        private readonly Dictionary<string, Type> _triggers;

        public IrcBot()
        {
            _client = new IrcClient
            {
                Encoding = Encoding.UTF8,
                SendDelay = 200,
                ActiveChannelSyncing = true,
            };
            
            _client.OnChannelMessage += ClientOnChannelMessage;
            _client.OnJoin += ClientOnJoin;
            _client.OnPart += ClientOnPart;
            _client.OnQuit += ClientOnQuit;
            
            _container = new UnityContainer();

            _container
                .RegisterType<IDataContextAsync, IrcBotContext>(new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new ContainerControlledLifetimeManager())
                .RegisterType<IAddPointTrigger, AddPointTrigger>()
                .RegisterType<IAddQuoteTrigger, AddQuoteTrigger>()
                .RegisterType<IAolSayTrigger, AolSayTrigger>()
                .RegisterType<IAolSayGeneratorTrigger, AolSayGeneratorTrigger>()
                .RegisterType<IClaimQuoteTrigger, ClaimQuoteTrigger>()
                .RegisterType<IDonLoudScreamTrigger, DonLoudScreamTrigger>()
                .RegisterType<IDonScreamTrigger, DonScreamTrigger>()
                .RegisterType<IEchoTrigger, EchoTrigger>()
                .RegisterType<IGoogleImageSearchTrigger, GoogleImageSearchTrigger>()
                .RegisterType<IInsultTrigger, InsultTrigger>()
                .RegisterType<IPointsTrigger, PointsTrigger>()
                .RegisterType<IQuoteStatsTrigger, QuoteStatsTrigger>()
                .RegisterType<IQuoteTrigger, QuoteTrigger>()
                .RegisterType<ISeenTrigger, SeenTrigger>()
                .RegisterType<ITakePointTrigger, TakePointTrigger>()
                .RegisterType<ITalkTrigger, TalkTrigger>()
                .RegisterType<IUrbanDictionaryTrigger, UrbanDictionaryTrigger>()
                .RegisterType<IRepositoryAsync<AolSayMessage>, Repository<AolSayMessage>>()
                .RegisterType<IRepositoryAsync<ChannelActivity>, Repository<ChannelActivity>>()
                .RegisterType<IRepositoryAsync<Message>, Repository<Message>>()
                .RegisterType<IRepositoryAsync<Point>, Repository<Point>>()
                .RegisterType<IRepositoryAsync<Quote>, Repository<Quote>>()
                .RegisterType<IAolSayMessageService, AolSayMessageService>()
                .RegisterType<IChannelActivityService, ChannelActivityService>()
                .RegisterType<IMessageService, MessageService>()
                .RegisterType<IPointService, PointService>()
                .RegisterType<IQuoteService, QuoteService>();

            _triggers = new Dictionary<string, Type>
            {
                { "!addpoint", typeof (AddPointTrigger) },
                { "!addquote", typeof (AddQuoteTrigger) },
                { "!aolsaygen", typeof (AolSayGeneratorTrigger) },
                { "!aolsay", typeof (AolSayTrigger) },
                { "!claim", typeof (ClaimQuoteTrigger) },
                { "!SCREAM", typeof (DonLoudScreamTrigger) },
                { "!scream", typeof (DonScreamTrigger) },
                { "!echo", typeof (EchoTrigger) },
                { "!gimage", typeof (GoogleImageSearchTrigger) },
                { "!insult", typeof (InsultTrigger) },
                { "!points", typeof (PointsTrigger) },
                { "!quotestats", typeof (QuoteStatsTrigger) },
                { "!quote", typeof (QuoteTrigger) },
                { "!seen", typeof (SeenTrigger) },
                { "!takepoint", typeof (TakePointTrigger) },
                { "!talk", typeof (TalkTrigger) },
                { "!ud", typeof (UrbanDictionaryTrigger) }
            };
        }

        public void Start()
        {
            _client.Connect(new[]
            {
                ConfigurationManager.AppSettings["Server"]
            }, int.Parse(ConfigurationManager.AppSettings["ServerPort"]));

            _client.Login(new []
            {
                ConfigurationManager.AppSettings["Nick"],
                ConfigurationManager.AppSettings["NickAlt"]
            }, ConfigurationManager.AppSettings["RealName"]);
            _client.RfcJoin(ConfigurationManager.AppSettings["Channel"]);

            _client.SendMessage(SendType.Message, "nickserv",
                $"identify {ConfigurationManager.AppSettings["NickServPassword"]}");

            _client.Listen();
            _client.Disconnect();
        }

        private void ClientOnChannelMessage(object sender, IrcEventArgs ircEventArgs)
        {
            var unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            var messageService = _container.Resolve<IMessageService>();

            var message = ircEventArgs.Data.Message;

            if (message == null)
            {
                return;
            }

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

            var split = message.Split(' ');

            if (_triggers.ContainsKey(split[0]))
            {
                var trigger = _container.Resolve(_triggers[split[0]]) as ITrigger;

                trigger?.Execute(
                    _client,
                    ircEventArgs,
                    split.Skip(1).Take(split.Length - 1).ToArray());
            }
            else if (split.Length == 1 && split[0].Equals("!help"))
            {
                _client.SendMessage(SendType.Message, ircEventArgs.Data.Channel,
                    $"Commands: {string.Join(", ", _triggers.Select(x => x.Key).OrderBy(x => x).ToArray())}");
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
