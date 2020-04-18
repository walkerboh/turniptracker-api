namespace TurnipTrackerApi.Models.BoardUsers
{
    public class BoardUserModel
    {
        public long Id { get; set; }
        
        public string Name { get; set; }

        public long BoardId { get; set; }

        public long UserId { get; set; }
    }
}