using System.Collections.Generic;

namespace TurnipTrackerApi.Database.Entities
{
    public class Board
    {
        public long Id { get; set; }

        public string UrlName { get; set; }

        public string DisplayName { get; set; }

        public bool Private { get; set; }

        public string EditKey { get; set; }

        public long OwnerId { get; set; }

        public RegisteredUser Owner { get; set; }

        public ICollection<BoardUser> Users { get; set; }

        public bool Deleted { get; set; }
    }
}