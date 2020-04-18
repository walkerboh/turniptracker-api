using System.ComponentModel.DataAnnotations;

namespace TurnipTrackerApi.Models.Boards
{
    public class BoardCreateModel
    {
        [Required]
        [MaxLength(50)]
        public string UrlName { get; set; }

        [Required]
        public string DisplayName { get; set; }
    }
}