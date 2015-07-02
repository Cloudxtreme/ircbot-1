using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class ChannelActivity : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Nick { get; set; }
        public UserAction Action { get; set; }
    }
}
