using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class Message : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Nick { get; set; }
        public string Content { get; set; }
    }
}
