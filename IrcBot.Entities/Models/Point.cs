using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class Point : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Nick { get; set; }
        public int Value { get; set; }
    }
}
