using System;
using System.Collections.Generic;
using TurnipTallyApi.Database.Entities;

namespace TurnipTallyApi.Models.Prices
{
    public class BoardPricesModel
    {
        public DateTime WeekDate { get; set; }

        public IEnumerable<PricesUserModel> Users { get; set; }
    }

    public class PricesUserModel
    {
        public long BoardUserId { get; set; }

        public long UserId { get; set; }

        public string Name { get; set; }

        public int? BuyPrice { get; set; }

        public IEnumerable<PricesRecordModel> Prices { get; set; }
    }

    public class PricesRecordModel
    {
        public DayOfWeek Day { get; set; }

        public Period Period { get; set; }

        public int? SellPrice { get; set; }
    }
}