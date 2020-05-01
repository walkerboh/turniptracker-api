using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;
using TurnipTallyApi.Extensions;
using TurnipTallyApi.Models.BoardUsers;

namespace TurnipTallyApi.Controllers
{
    [Authorize]
    [Route("boards/{boardId}/users")]
    [ApiController]
    public class BoardUsersController : ControllerBase
    {
        private readonly TurnipContext _context;
        private readonly IMapper _mapper;

        public BoardUsersController(TurnipContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetBoardUsers(long boardId)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<BoardUserModel>>(board.Users));
        }

        [Route("{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetBoardUser(long boardId, long userId)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var user = board.Users?.SingleOrDefault(u => u.Id.Equals(userId));

            if(user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BoardUserModel>(user));
        }

        [HttpPost]
        public async Task<IActionResult> AddBoardUser(long boardId, [FromBody] BoardUserCreateModel model)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var regUser = await _context.RegisteredUsers.FindAsync(User.GetUserId());

            if(board.Users?.Any(u => u.RegisteredUserId.Equals(regUser.Id)) ?? false)
            {
                return BadRequest(new {message = "User is already a member of this board"});
            }

            if (board.Users?.Any(u => u.Name.Equals(model.DisplayName, StringComparison.InvariantCultureIgnoreCase)) ?? false)
            {
                return BadRequest(new {message = "Display name is already taken"});
            }

            var newUser = new BoardUser
            {
                Name = model.DisplayName,
                RegisteredUser = regUser,
                Weeks = new List<Week>
                {
                    new Week
                    {
                        WeekDate = DateTimeExtensions.NowInLocale(regUser.TimezoneId).ToStartOfWeek()
                    }
                }
            };

            board.Users ??= new List<BoardUser>();

            board.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBoardUser), new {boardId, userId = newUser.Id}, _mapper.Map<BoardUserModel>(newUser));
        }

        [Route("{userId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveBoardUser(long boardId, long userId)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var user = board.Users?.SingleOrDefault(u => u.Id.Equals(userId));

            if(user == null)
            {
                return NotFound();
            }

            _context.BoardUsers.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<Board> GetBoard(long boardId)
        {
            return await _context.Boards.Include(b=>b.Users).SingleOrDefaultAsync(b => b.Id.Equals(boardId));
        }
    }
}