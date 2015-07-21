using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Common.MarkovChains;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class TalkTrigger : ITrigger
    {
        private readonly IMessageService _messageService;

        private readonly char[] _sentenceSeparators =
        {
            '.', '!', '?', ',', ';', ':'
        };

        private readonly Regex _cleanWordRegex = new Regex(@"[()\[\]{}'""`~]");

        public TalkTrigger(IUnityContainer container)
        {
            _messageService = container.Resolve<IMessageService>();
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 1)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !talk <nick>");
                return;
            }

            var nick = triggerArgs[0];

            var markovChain = new MarkovChain<string>();
            var messages = _messageService.Query(x => x.Nick == nick).Select();

            foreach (var message in messages)
            {
                var sentences = message.Content.Split(_sentenceSeparators);

                foreach (var sentence in sentences)
                {
                    string lastWord = null;

                    foreach (var word in sentence.Split(' ').Select(x => _cleanWordRegex.Replace(x, String.Empty)).Where(word => word.Length != 0))
                    {
                        markovChain.Train(lastWord, word);
                        lastWord = word;
                    }

                    markovChain.Train(lastWord, null);
                }
            }

            if (markovChain.Nodes.Count > 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel,
                    $"<@{nick}> {GenerateRandomMessage(markovChain)}");
            }
        }

        private static string GenerateRandomMessage(MarkovChain<string> markovChain)
        {
            var trials = 0;
            string[] words;

            do
            {
                words = markovChain.GenerateSequence().ToArray();
            } while (words.Length < 8 && trials++ < 30);

            return string.Join(" ", words);
        }
    }
}
