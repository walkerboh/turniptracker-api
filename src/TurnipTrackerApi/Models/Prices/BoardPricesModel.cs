using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Name { get; set; }

        public int BuyPrice { get; set; }

        public IEnumerable<PricesRecordModel> Prices { get; set; }

        //public PriceModel PriceModel => PriceModel.Get(Prices.ToList());
    }

    public class PricesRecordModel
    {
        public DayOfWeek Day { get; set; }

        public Period Period { get; set; }

        public int SellPrice { get; set; }
    }

    public class PriceModel
    {
        public int? MondayAm { get; set; }
        public int? MondayPm { get; set; }

        public int? TuesdayAm { get; set; }
        public int? TuesdayPm { get; set; }

        public int? WednesdayAm { get; set; }
        public int? WednesdayPm { get; set; }

        public int? ThursdayAm { get; set; }
        public int? ThursdayPm { get; set; }

        public int? FridayAm { get; set; }
        public int? FridayPm { get; set; }

        public int? SaturdayAm { get; set; }
        public int? SaturdayPm { get; set; }

        public static PriceModel Get(IList<PricesRecordModel> prices)
        {
            return new PriceModel
            {
                MondayAm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Monday) && p.Period.Equals(Period.AM))
                    ?.SellPrice,
                MondayPm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Monday) && p.Period.Equals(Period.PM))
                    ?.SellPrice,

                TuesdayAm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Tuesday) && p.Period.Equals(Period.AM))
                    ?.SellPrice,
                TuesdayPm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Tuesday) && p.Period.Equals(Period.PM))
                    ?.SellPrice,

                WednesdayAm = prices
                    .SingleOrDefault(p => p.Day.Equals(DayOfWeek.Wednesday) && p.Period.Equals(Period.AM))
                    ?.SellPrice,
                WednesdayPm = prices
                    .SingleOrDefault(p => p.Day.Equals(DayOfWeek.Wednesday) && p.Period.Equals(Period.PM))
                    ?.SellPrice,

                ThursdayAm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Thursday) && p.Period.Equals(Period.AM))
                    ?.SellPrice,
                ThursdayPm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Thursday) && p.Period.Equals(Period.PM))
                    ?.SellPrice,

                FridayAm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Friday) && p.Period.Equals(Period.AM))
                    ?.SellPrice,
                FridayPm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Friday) && p.Period.Equals(Period.PM))
                    ?.SellPrice,

                SaturdayAm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Saturday) && p.Period.Equals(Period.AM))
                    ?.SellPrice,
                SaturdayPm = prices.SingleOrDefault(p => p.Day.Equals(DayOfWeek.Saturday) && p.Period.Equals(Period.PM))
                    ?.SellPrice,
            };
        }
    }
}