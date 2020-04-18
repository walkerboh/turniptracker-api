using System.Linq;
using TurnipTrackerApi.Services;

namespace TurnipTrackerApi.Database.Data
{
    public static class TurnipInitializer
    {
        public static void Initialize(TurnipContext context)
        {
            context.Database.EnsureCreated();

            if(!context.RegisteredUsers.Any())
            {
                var _ =new UserService(context).Create("a.b@com", "password").Result;
            }
        }
    }
}