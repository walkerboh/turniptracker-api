using System.ComponentModel.DataAnnotations;

namespace TurnipTallyApi.Models.Users
{
    public class AuthenticateModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}