using System.Text.RegularExpressions;

namespace IrcBot.Client.Triggers
{
    public abstract class BaseTalkTrigger
    {
        protected readonly char[] SentenceSeparators = { '.', '!', '?', ',', ';', ':' };
        protected readonly Regex CleanWordRegex = new Regex(@"[()\[\]{}'""`~]");
    }
}
