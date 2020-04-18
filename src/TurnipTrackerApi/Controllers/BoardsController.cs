using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TurnipTrackerApi.Database;
using TurnipTrackerApi.Database.Entities;
using TurnipTrackerApi.Extensions;
using TurnipTrackerApi.Models.Boards;

namespace TurnipTrackerApi.Controllers
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

            var user = await _context.RegisteredUsers.FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var boards = user.BoardUsers?.Select(u => u.Board).ToList();

            if(boards == null)
            {
                return NotFound();
            }

            boards.AddRange(user.OwnedBoards);
            boards = boards.Distinct().ToList();

            return Ok(boards.Select(b => new {b.DisplayName, b.UrlName}));
        }

        [HttpGet("name/{urlName}")]
        public async Task<IActionResult> Board(string urlName)
        {
            var board = await _context.Boards.SingleOrDefaultAsync(b => b.UrlName.Equals(urlName.ToLowerInvariant()));

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            return Ok(board);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Board(long id)
        {
            var board = await _context.Boards.SingleOrDefaultAsync(b => b.Id.Equals(id));

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            return Ok(board);
        }

        [HttpGet("{id}/weeks")]
        public async Task<IActionResult> BoardWeeks(long id)
        {
            var board = await _context.Boards.SingleOrDefaultAsync(b => b.Id.Equals(id));

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var weeks = board.Users?.SelectMany(u => u.Weeks).ToList();
            var weekDates = weeks?.Any() ?? false ? weeks.Select(w => w.WeekDate).Distinct() : Enumerable.Empty<DateTime>();

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

            board.OwnerId = User.GetUserId();

            await _context.Boards.AddAsync(_mapper.Map<Board>(board));
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Board), new {id = board.Id}, board);
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