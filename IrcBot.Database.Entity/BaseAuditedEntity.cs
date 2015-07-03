using System;

namespace IrcBot.Database.Entity
{
    public abstract class BaseAuditedEntity : BaseEntity
    {
        protected BaseAuditedEntity()
        {
            var now = DateTime.Now;

            Created = now;
            Modified = now;
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
