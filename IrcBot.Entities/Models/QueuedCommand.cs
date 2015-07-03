using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class QueuedCommand : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Command { get; set; }
    }
}
