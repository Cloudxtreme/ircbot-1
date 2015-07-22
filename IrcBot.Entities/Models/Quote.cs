using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class Quote : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }
}
