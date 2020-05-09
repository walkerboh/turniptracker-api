using System.Linq;
using TurnipTallyApi.Services;

namespace TurnipTallyApi.Database.Data
{
    public static class TurnipInitializer
    {
        public static void Initialize(TurnipContext context)
        {
            context.Database.EnsureCreated();

            if(!context.RegisteredUsers.Any())
            {
                //var _ =new UserService(context).Create("a.b@com", "password", "Eastern Standard Time").Result;
            }
        }
    }
}