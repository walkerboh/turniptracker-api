using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;
using TurnipTallyApi.Extensions;
using TurnipTallyApi.Helpers.Settings;
using TurnipTallyApi.Models.Boards;
using TurnipTallyApi.Models.Prices;
using TurnipTallyApi.Models.Users;
using TurnipTallyApi.Services;
using ApplicationException = TurnipTallyApi.Exceptions.ApplicationException;

namespace TurnipTallyApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ApplicationSettings _settings;
        private readonly TurnipContext _context;

        public UsersController(TurnipContext context, IMapper mapper, IUserService userService,
            IOptions<ApplicationSettings> settings)
        {
            _mapper = mapper;
            _userService = userService;
            _settings = settings.Value;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
            {
                return BadRequest(new {message = "Username or password is incorrect"});
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                user.Id,
                user.Email,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var user = await _userService.Create(model.Email, model.Password, model.TimezoneId);

                var currentWeek = DateTimeExtensions.NowInLocale(model.TimezoneId).ToStartOfWeek();

                user.Weeks = new List<Week>
                {
                    new Week
                    {
                        WeekDate = currentWeek
                    }
                };

                return CreatedAtAction(nameof(GetById), new {id = user.Id}, new {user.Id, user.Email});
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [AllowAnonymous]
        [HttpGet("timezones")]
        public IActionResult Timezones()
        {
            return Ok(_context.Timezones.OrderBy(t => t.Order));
        }

        [AllowAnonymous]
        [HttpPost("passwordResetEmail")]
        public async Task<IActionResult> PasswordResetEmail([FromBody] PasswordEmailModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest(new {message = "Email must be provided to reset password."});
            }

            await _userService.SendPasswordReset(model.Email);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("passwordReset/{key}")]
        public async Task<IActionResult> PasswordResetVerify(Guid key)
        {
            await _context.CleanPasswordResets();

            var reset = await _context.PasswordResets.FirstOrDefaultAsync(pr => pr.Key.Equals(key));

            if(reset == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("passwordReset")]
        public async Task<IActionResult> PasswordReset([FromBody] UpdatePasswordModel model)
        {
            if(!model.Key.HasValue || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest();
            }

            await _context.CleanPasswordResets();

            var reset = await _context.PasswordResets.FirstOrDefaultAsync(pr => pr.Key.Equals(model.Key.Value));

            if (reset == null || reset.ExpiryDate < DateTime.UtcNow)
            {
                return NotFound();
            }

            try
            {
                await _userService.UpdatePassword(reset.RegisteredUserId, model.Password);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return NoContent();
        }

        [HttpPost("updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordModel model)
        {
            try
            {
                await _userService.UpdatePassword(User.GetUserId(), model.Password);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var user = _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _context.RegisteredUsers.Include(u => u.Weeks).Include(u => u.OwnedBoards)
                .Include(u => u.BoardUsers).ThenInclude(bu => bu.Board)
                .SingleOrDefaultAsync(u => u.Id.Equals(User.GetUserId()));

            if (user == null)
            {
                return NotFound();
            }

            var weekDate = DateTimeExtensions.NowInLocale(user.TimezoneId).ToStartOfWeek();

            if (!user.Weeks.Any(w => w.WeekDate.Equals(weekDate)))
            {
                user.Weeks.Add(new Week
                {
                    WeekDate = weekDate
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new UserDetailsModel
            {
                Id = user.Id,
                Weeks = user.Weeks.Select(w => w.WeekDate).OrderByDescending(d => d),
                OwnedBoards = _mapper.Map<IEnumerable<BoardModel>>(user.OwnedBoards.Where(b => !b.Deleted)),
                MemberBoards =
                    _mapper.Map<IEnumerable<BoardModel>>(user.BoardUsers.Select(bu => bu.Board)
                        .Where(b => !b.Deleted))
            });
        }
    }
}