using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Practices.Unity;

using Meebey.SmartIrc4net;

using IrcBot.Common.MarkovChains;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AolSayGeneratorTrigger : ITrigger
    {
        private readonly IAolSayMessageService _aolSayMessageService;
        private readonly char[] _sentenceSeparators =
        {
            '.', '!', '?', ',', ';', ':'
        };

        private readonly Regex _cleanWordRegex = new Regex(@"[()\[\]{}'""`~]");

        public AolSayGeneratorTrigger(IUnityContainer container)
        {
            _aolSayMessageService = container.Resolve<IAolSayMessageService>();
        }

        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length != 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !aolsaygen");
                return;
            }

            var markovChain = new MarkovChain<string>();
            var messages = _aolSayMessageService.Query().Select();

            foreach (var message in messages)
            {
                var sentences = message.Content.Split(_sentenceSeparators);

                foreach (var sentence in sentences)
                {
                    string lastWord = null;

                    foreach (var word in sentence.Split(' ').Select(x => _cleanWordRegex.Replace(x, String.Empty)))
                    {
                        if (word.Length == 0)
                        {
                            continue;
                        }

                        markovChain.Train(lastWord, word);
                        lastWord = word;
                    }

                    markovChain.Train(lastWord, null);
                }
            }

            if (markovChain.Nodes.Count > 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, GenerateRandomAolSayMessage(markovChain));
            }
        }

        private static string GenerateRandomAolSayMessage(MarkovChain<string> markovChain)
        {
            var trials = 0;
            string[] words;

            do
            {
                words = markovChain.GenerateSequence().ToArray();
            } while (words.Length < 3 && trials++ < 10);

            return String.Join(" ", words);
        }
    }
}
