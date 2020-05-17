using System.Collections.Generic;

namespace TurnipTallyApi.Database.Entities
{
    public class BoardUser
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long BoardId { get; set; }

        public Board Board { get; set; }

        public long RegisteredUserId { get; set; }

        public RegisteredUser RegisteredUser { get; set; }

        public bool Deleted { get; set; }
    }
}