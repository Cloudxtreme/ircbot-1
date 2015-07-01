using IrcBot.Database.Entity;

namespace IrcBot.Entities.Models
{
    public class User : BaseAuditedEntity
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
