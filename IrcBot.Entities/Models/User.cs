using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class User : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Nick { get; set; }
    }
}
