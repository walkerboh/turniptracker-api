using System.ComponentModel.DataAnnotations;

namespace TurnipTallyApi.Models.Boards
{
    public class BoardCreateModel
    {
        [Required]
        [MaxLength(50)]
        public string UrlName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public bool Private { get; set; }

        public string Password { get; set; }

        [Required]
        public string UserDisplayName { get; set; }
    }
}