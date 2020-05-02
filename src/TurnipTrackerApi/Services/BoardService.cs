using System.Linq;
using System.Threading.Tasks;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;
using TurnipTallyApi.Extensions;

namespace TurnipTallyApi.Services
{
    public interface IBoardService
    {
        public Task VerifyWeeks(TurnipContext context, Board board, string timezoneId);
    }

    public class BoardService : IBoardService
    {
        public async Task VerifyWeeks(TurnipContext context, Board board, string timezoneId)
        {
            var currentWeek = DateTimeExtensions.NowInLocale(timezoneId).ToStartOfWeek();

            foreach(var user in board.Users)
            {
                if (!user.Weeks.Any(w => w.WeekDate.Equals(currentWeek)))
                {
                    user.Weeks.Add(new Week
                    {
                        WeekDate = currentWeek
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}