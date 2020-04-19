using System;

namespace TurnipTallyApi.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToStartOfWeek(this DateTime dateTime)
        {
            var diff = (7 + (dateTime.Date.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }
    }
}