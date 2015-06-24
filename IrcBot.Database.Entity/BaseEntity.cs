using System.ComponentModel.DataAnnotations.Schema;

using IrcBot.Database.Infrastructure;

namespace IrcBot.Database.Entity
{
    public abstract class BaseEntity : IObjectState
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; }
    }
}
