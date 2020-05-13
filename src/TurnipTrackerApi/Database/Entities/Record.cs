using System;

namespace TurnipTallyApi.Database.Entities
{
    public class Record
    {
        public long BoardUserId { get; set; }

        public DateTime WeekDate { get; set; }

        public Week Week { get; set; }

        public DayOfWeek Day { get; set; }

        public Period Period { get; set; }

        public int? SellPrice { get; set; }
    }

    public enum Period
    {
        AM,
        PM
    }
}