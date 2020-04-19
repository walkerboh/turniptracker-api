using System;
using TurnipTallyApi.Database.Entities;

namespace TurnipTallyApi.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToStartOfWeek(this DateTime dateTime)
        {
            var diff = (7 + (dateTime.Date.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        public static (DayOfWeek, Period) ToPriceTiming(this DateTime dateTime)
        {
            var period = dateTime.Hour < 12 ? Period.AM : Period.PM;
            return (dateTime.DayOfWeek, period);
        }
    }
}