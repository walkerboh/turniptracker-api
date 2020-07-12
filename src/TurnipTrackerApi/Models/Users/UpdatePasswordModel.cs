using System;
using System.ComponentModel.DataAnnotations;

namespace TurnipTallyApi.Models.Users
{
    public class UpdatePasswordModel
    {
        public Guid? Key { get; set; }

        [Required]
        public string Password { get; set; }
    }
}