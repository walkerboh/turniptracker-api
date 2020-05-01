using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;
using TurnipTallyApi.Extensions;
using TurnipTallyApi.Models.Boards;
using TurnipTallyApi.Models.BoardUsers;

namespace TurnipTallyApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly TurnipContext _context;

        public BoardsController(IMapper mapper, TurnipContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> UserBoards()
        {
            var userId = User.GetUserId();

            var user = await _context.RegisteredUsers.Include(u => u.BoardUsers).ThenInclude(bu => bu.Board)
                .Include(u => u.OwnedBoards)
                .SingleOrDefaultAsync(u => u.Id.Equals(userId));

            if (user == null)
            {
                return Unauthorized();
            }

            var boards = user.BoardUsers?.Select(u => u.Board).ToList();

            if (boards == null)
            {
                return NotFound();
            }

            boards.AddRange(user.OwnedBoards);
            boards = boards.Distinct().ToList();

            return Ok(_mapper.Map<IEnumerable<UserBoardsModel>>(boards));
        }

        [HttpGet("name/{urlName}")]
        public async Task<IActionResult> Board(string urlName)
        {
            var board = await _context.Boards.Include(b => b.Users)
                .SingleOrDefaultAsync(b => b.UrlName.Equals(urlName.ToLowerInvariant()));

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var regUserId = User.GetUserId();

            if (board.OwnerId.Equals(regUserId) || board.Users.Any(u => u.RegisteredUserId.Equals(regUserId)))
            {
                return Ok(_mapper.Map<BoardModel>(board));
            }

            return Ok(_mapper.Map<JoinBoardModel>(board));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Board(long id)
        {
            var board = await _context.Boards.Include(b => b.Users).SingleOrDefaultAsync(b => b.Id.Equals(id));

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var regUserId = User.GetUserId();

            if (board.OwnerId.Equals(regUserId) || board.Users.Any(u => u.RegisteredUserId.Equals(regUserId)))
            {
                return Ok(_mapper.Map<BoardModel>(board));
            }

            return Ok(_mapper.Map<JoinBoardModel>(board));
        }

        [HttpGet("{id}/weeks")]
        public async Task<IActionResult> BoardWeeks(long id)
        {
            var board = await _context.Boards.Include(b => b.Users).ThenInclude(u => u.Weeks)
                .SingleOrDefaultAsync(b => b.Id.Equals(id));

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var weeks = board.Users?.SelectMany(u => u.Weeks).ToList();
            var weekDates = weeks?.Any() ?? false
                ? weeks.Select(w => w.WeekDate).Distinct()
                : Enumerable.Empty<DateTime>();

            return Ok(weekDates);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(BoardCreateModel boardModel)
        {
            var board = _mapper.Map<Board>(boardModel);

            if (_context.Boards.Any(b => b.UrlName.Equals(board.UrlName)))
            {
                return BadRequest("Friendly name is already taken");
            }

            var regUser = await _context.RegisteredUsers.FindAsync(User.GetUserId());

            board.OwnerId = regUser.Id;

            var boardUser = new BoardUser
            {
                RegisteredUserId = board.OwnerId,
                Name = boardModel.UserDisplayName,
                Weeks = new List<Week>
                {
                    new Week
                    {
                        WeekDate = DateTimeExtensions.NowInLocale(regUser.TimezoneId).ToStartOfWeek()
                    }
                }
            };

            board.Users = new List<BoardUser>
            {
                boardUser
            };

            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Board), new {id = board.Id}, _mapper.Map<BoardModel>(board));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(long id)
        {
            var board = await _context.Boards.SingleOrDefaultAsync(b => b.Id.Equals(id));

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            board.Deleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}