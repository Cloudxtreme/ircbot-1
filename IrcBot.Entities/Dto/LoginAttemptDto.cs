using System.ComponentModel.DataAnnotations;

namespace IrcBot.Entities.Dto
{
    public class LoginAttemptDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
