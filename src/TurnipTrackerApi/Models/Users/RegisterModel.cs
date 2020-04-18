using System.ComponentModel.DataAnnotations;

namespace TurnipTrackerApi.Models.Users
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}