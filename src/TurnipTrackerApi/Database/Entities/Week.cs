using System;
using System.Collections.Generic;

namespace TurnipTrackerApi.Database.Entities
{
    public class Week
    {
        public long BoardUserId { get; set; }

        public BoardUser BoardUser { get; set; }

        public DateTime WeekDate { get; set; }

        public int BuyPrice { get; set; }

        public ICollection<Record> Records { get; set; }
    }
}