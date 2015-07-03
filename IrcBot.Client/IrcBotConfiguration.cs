namespace IrcBot.Client
{
    public class IrcBotConfiguration
    {
        public IrcBotConfiguration()
        {
            Nick = "DerekSmooch";
            NickAlt = "DerekSmooch_";
            RealName = "D. Gooch";
            Channels = new[] {"#cdnidle"};
        }

        public string Nick { get; set; }
        public string NickAlt { get; set; }
        public string RealName { get; set; }
        public string[] Channels { get; set; }
        public string[] Servers { get; set; }
    }
}
