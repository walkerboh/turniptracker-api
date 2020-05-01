using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;
using TurnipTallyApi.Extensions;
using TurnipTallyApi.Models.Prices;

namespace TurnipTallyApi.Controllers
{
    [Authorize]
    [Route("boards/{boardId}")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly TurnipContext _context;
        private readonly IMapper _mapper;

        public PricesController(TurnipContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Route("prices/{date}")]
        [HttpGet]
        public async Task<IActionResult> BoardPrices(long boardId, DateTime date)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var weekDate = date.ToStartOfWeek();

            var users = _context.BoardUsers.Where(u => u.BoardId.Equals(boardId) && !u.Deleted).Select(u => u.Id).ToList();

            var weeks = _context.Weeks.Include(w => w.Records).Include(w => w.BoardUser)
                .Where(w => w.WeekDate.Equals(weekDate) && users.Contains(w.BoardUserId));

            var model = new BoardPricesModel
            {
                WeekDate = weekDate,
                Users = _mapper.Map<IEnumerable<PricesUserModel>>(weeks)
            };

            return Ok(model);
        }

        [Route("users/{userId}/prices/buy")]
        [HttpPost]
        public async Task<IActionResult> Buy(long boardId, long userId, [FromBody] BuyPriceModel model)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var user = board.Users?.SingleOrDefault(u => u.Id.Equals(userId));

            if (user == null)
            {
                return NotFound();
            }

            var weekDate = model.Date.ToStartOfWeek();

            var week = await _context.Weeks.SingleOrDefaultAsync(w =>
                w.BoardUserId.Equals(user.Id) && w.WeekDate.Equals(weekDate));

            if (week == null)
            {
                week = new Week
                {
                    WeekDate = weekDate,
                    BoardUserId = user.Id
                };
                await _context.Weeks.AddAsync(week);
            }

            week.BuyPrice = model.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Route("users/{userId}/prices/sell")]
        [HttpPost]
        public async Task<IActionResult> Sell(long boardId, long userId, [FromBody] SellPriceModel model)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var user = board.Users?.SingleOrDefault(u => u.Id.Equals(userId));

            if (user == null)
            {
                return NotFound();
            }

            var weekDate = model.Date.ToStartOfWeek();

            var week = await _context.Weeks.Include(w => w.Records).SingleOrDefaultAsync(w =>
                w.BoardUserId.Equals(user.Id) && w.WeekDate.Equals(weekDate));

            if (week == null)
            {
                week = new Week
                {
                    WeekDate = weekDate,
                    BoardUserId = user.Id
                };
                await _context.Weeks.AddAsync(week);
            }

            week.Records ??= new List<Record>();

            var record = week.Records.SingleOrDefault(r => r.Day.Equals(model.Day) && r.Period.Equals(model.Period));

            if (record == null)
            {
                record = new Record
                {
                    Day = model.Day,
                    Period = model.Period
                };
                week.Records.Add(record);
            }

            record.SellPrice = model.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<Board> GetBoard(long boardId)
        {
            return await _context.Boards.Include(b => b.Users).SingleOrDefaultAsync(b => b.Id.Equals(boardId));
        }
    }
}