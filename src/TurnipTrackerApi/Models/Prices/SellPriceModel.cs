using System;
using TurnipTallyApi.Database.Entities;

namespace TurnipTallyApi.Models.Prices
{
    public class SellPriceModel
    {
        public DateTime Date { get; set; }

        public int Price { get; set; }

        public DayOfWeek Day { get; set; }

        public Period Period { get; set; }
    }
}