using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TurnipTallyApi.Controllers
{
    [Authorize]
    [Route("board/{boardId}/users/{userId}/prices")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        
    }
}