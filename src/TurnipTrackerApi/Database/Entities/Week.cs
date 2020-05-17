using System;
using System.Collections.Generic;

namespace TurnipTallyApi.Database.Entities
{
    public class Week
    {
        public long UserId { get; set; }

        public RegisteredUser User { get; set; }

        public DateTime WeekDate { get; set; }

        public int? BuyPrice { get; set; }

        public ICollection<Record> Records { get; set; }
    }
}