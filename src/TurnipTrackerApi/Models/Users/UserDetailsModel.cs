using System;
using System.Collections.Generic;
using TurnipTallyApi.Models.Boards;

namespace TurnipTallyApi.Models.Users
{
    public class UserDetailsModel
    {
        public long Id { get; set; }

        public IEnumerable<DateTime> Weeks { get; set; }

        public IEnumerable<BoardModel> OwnedBoards { get; set; }

        public IEnumerable<BoardModel> MemberBoards { get; set; }
    }
}