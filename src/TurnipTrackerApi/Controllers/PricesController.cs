using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;
using TurnipTallyApi.Extensions;
using TurnipTallyApi.Models.Prices;

namespace TurnipTallyApi.Controllers
{
    [Authorize]
    [Route("prices")]
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

        [Route("boards/{boardId}/weeks/{date}")]
        [HttpGet]
        public async Task<IActionResult> BoardPrices(long boardId, DateTime date)
        {
            var board = await GetBoard(boardId);

            if (board == null || board.Deleted)
            {
                return NotFound();
            }

            var weekDate = date.ToStartOfWeek();

            var users = _context.BoardUsers.Where(u => u.BoardId.Equals(boardId) && !u.Deleted).Select(u => u.RegisteredUserId).ToList();

            var weeks = _context.Weeks.Include(w => w.Records).Where(w => w.WeekDate.Equals(weekDate) && users.Contains(w.UserId));

            var modelUsers = new List<PricesUserModel>();

            foreach(var week in weeks)
            {
                var u = _mapper.Map<PricesUserModel>(week);
                var user = board.Users.Single(bu => bu.RegisteredUserId.Equals(week.UserId));
                u.Name = user.Name;
                u.UserId = u.UserId;
                modelUsers.Add(u);
            }

            var model = new BoardPricesModel
            {
                WeekDate = weekDate,
                Users = modelUsers
            };

            return Ok(model);
        }

        [Route("users/weeks/{date}")]
        [HttpGet]
        public async Task<IActionResult> UserPrices (DateTime date)
        {
            var userId = User.GetUserId();

            var user = await _context.RegisteredUsers.Include(u => u.Weeks).ThenInclude(w=>w.Records).SingleAsync(u => u.Id.Equals(userId));

            var weekDate = date.ToStartOfWeek();

            var week = user.Weeks.SingleOrDefault(w => w.WeekDate.Equals(weekDate));

            if (week == null)
            {
                week = new Week
                {
                    WeekDate = weekDate
                };
                user.Weeks.Add(week);

                await _context.SaveChangesAsync();
            }

            var model = new BoardPricesModel
            {
                WeekDate = weekDate,
                Users = new List<PricesUserModel>
                {
                    _mapper.Map<PricesUserModel>(week)
                }
            };

            return Ok(model);
        }

        [Route("users/{userId}/buy")]
        [HttpPost]
        public async Task<IActionResult> Buy(long userId, [FromBody] BuyPriceModel model)
        {
            var user = await _context.RegisteredUsers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var weekDate = model.Date.ToStartOfWeek();

            var week = await _context.Weeks.SingleOrDefaultAsync(w =>
                w.UserId.Equals(user.Id) && w.WeekDate.Equals(weekDate));

            if (week == null)
            {
                week = new Week
                {
                    WeekDate = weekDate,
                    UserId = user.Id
                };
                await _context.Weeks.AddAsync(week);
            }

            week.BuyPrice = model.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Route("users/{userId}/sell")]
        [HttpPost]
        public async Task<IActionResult> Sell(long userId, [FromBody] SellPriceModel model)
        {
            var user = await _context.RegisteredUsers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var weekDate = model.Date.ToStartOfWeek();

            var week = await _context.Weeks.Include(w => w.Records).SingleOrDefaultAsync(w =>
                w.UserId.Equals(user.Id) && w.WeekDate.Equals(weekDate));

            if (week == null)
            {
                week = new Week
                {
                    WeekDate = weekDate,
                    UserId = user.Id
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