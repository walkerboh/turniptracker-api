using System;
using TurnipTallyApi.Extensions;
using Xunit;

namespace TurnipTallyApi.UnitTests.Extensions
{
    public class DateTimeExtensionsTests
    {
        private readonly DateTime[] _weekOfDates =
        {
            new DateTime(2020, 4, 5),
            new DateTime(2020, 4, 6),
            new DateTime(2020, 4, 7),
            new DateTime(2020, 4, 8),
            new DateTime(2020, 4, 9),
            new DateTime(2020, 4, 10),
            new DateTime(2020, 4, 11)
        };

        [Fact]
        public void ToStartOfWeekAllDaysOfWeekReturnSunday()
        {
            foreach(var date in _weekOfDates)
            {
                var weekStart = date.ToStartOfWeek();
                Assert.Equal(new DateTime(2020, 4, 5), weekStart);
            }
        }

        [Fact]
        public void NowInLocaleReturnsCorrectTimeForTimezone()
        {
            var expected = DateTime.Now.Date;
            var actual = DateTimeExtensions.NowInLocale(TimeZoneInfo.Local.Id).Date;
            Assert.Equal(expected, actual);
        }
    }
}