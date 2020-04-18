using System.Security.Claims;
using TurnipTrackerApi.Exceptions;

namespace TurnipTrackerApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var idString = user.FindFirst(ClaimTypes.Name)?.Value;

            if(idString == null || !long.TryParse(idString, out var id))
            {
                throw new ApplicationException("User id is not valid");
            }

            return id;
        }
    }
}