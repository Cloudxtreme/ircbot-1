using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class AolSayMessage : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}
