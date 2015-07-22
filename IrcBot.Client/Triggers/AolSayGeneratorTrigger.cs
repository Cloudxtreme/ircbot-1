using System.Linq;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Common.MarkovChains;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class AolSayGeneratorTrigger : BaseTalkTrigger, IAolSayGeneratorTrigger
    {
        private readonly IAolSayMessageService _aolSayMessageService;

        public AolSayGeneratorTrigger(IAolSayMessageService aolSayMessageService)
        {
            _aolSayMessageService = aolSayMessageService;
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
                var sentences = message.Content.Split(SentenceSeparators);

                foreach (var sentence in sentences)
                {
                    string lastWord = null;

                    foreach (var word in sentence.Split(' ').Select(x => CleanWordRegex.Replace(x, string.Empty)).Where(word => word.Length != 0))
                    {
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

            return string.Join(" ", words);
        }
    }
}
