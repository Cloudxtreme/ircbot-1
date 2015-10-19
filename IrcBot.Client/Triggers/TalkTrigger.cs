using System.Linq;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;
using IrcBot.Common.MarkovChains;
using IrcBot.Service;

namespace IrcBot.Client.Triggers
{
    public class TalkTrigger : BaseTalkTrigger, ITalkTrigger
    {
        private readonly IMessageService _messageService;

        public TalkTrigger(IMessageService messageService)
        {
            _messageService = messageService; 
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

            int totalCount;
            var messages = _messageService
                .Query(x => x.Nick == nick)
                .OrderBy(o => o.OrderByDescending(x => x.Created))
                .SelectPage(1, 500, out totalCount);

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
