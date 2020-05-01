namespace TurnipTallyApi.Models.BoardUsers
{
    public class JoinBoardModel
    {
        public long Id { get; set; }

        public bool PrivateBoard { get; set; }

        public bool Join { get; } = true;

        public string DisplayName { get; set; }
    }
}