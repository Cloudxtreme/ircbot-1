using IrcBot.Database.Entity;

namespace IrcBot.Entities.Dto
{
    public abstract class BaseDto
    {
        protected BaseDto()
        { }

        protected BaseDto(BaseAuditedEntity entity)
        {
            Created = entity.Created.ToString("s");
            Modified = entity.Modified.ToString("s");
        }

        public string Created { get; set; }
        public string Modified { get; set; }
    }
}
