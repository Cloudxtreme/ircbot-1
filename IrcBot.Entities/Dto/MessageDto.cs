using IrcBot.Entities.Models;

namespace IrcBot.Entities.Dto
{
    public class MessageDto : BaseDto
    {
        public MessageDto()
        { }

        public MessageDto(Message message)
            : base(message)
        {
            Id = message.Id;
            Nick = message.Nick;
            Content = message.Content;
        }

        public int? Id { get; set; }

        public string Nick { get; set; }

        public string Content { get; set; }
    }
}
