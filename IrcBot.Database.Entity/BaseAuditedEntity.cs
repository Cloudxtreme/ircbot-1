using System;

namespace IrcBot.Database.Entity
{
    public abstract class BaseAuditedEntity : BaseEntity
    {
        protected BaseAuditedEntity()
        {
            var utcNow = DateTime.UtcNow;

            Created = utcNow;
            Modified = utcNow;
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
