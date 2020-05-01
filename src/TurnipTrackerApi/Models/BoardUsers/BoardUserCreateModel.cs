using System.ComponentModel.DataAnnotations;

namespace TurnipTallyApi.Models.BoardUsers
{
    public class BoardUserCreateModel
    {
        [Required]
        public string DisplayName { get; set; }

        public string Password { get; set; }
    }
}