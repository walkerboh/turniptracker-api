using System.Collections.Generic;
using TurnipTallyApi.Models.BoardUsers;

namespace TurnipTallyApi.Models.Boards
{
    public class BoardModel
    {
        public long Id { get; set; }

        public string UrlName { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<BoardUserModel> Users { get; set; }
    }
}