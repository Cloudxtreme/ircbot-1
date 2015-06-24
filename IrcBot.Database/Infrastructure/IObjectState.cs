using System.ComponentModel.DataAnnotations.Schema;

namespace IrcBot.Database.Infrastructure
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
