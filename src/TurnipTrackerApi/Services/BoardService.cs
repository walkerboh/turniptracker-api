using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

            var userIds = board.Users.Select(bu => bu.RegisteredUserId);

            foreach(var userId in userIds)
            {
                var user = await context.RegisteredUsers.Include(u => u.Weeks).SingleOrDefaultAsync(u => u.Id.Equals(userId));

                if(!user.Weeks.Any(w=>w.WeekDate.Equals(currentWeek)))
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