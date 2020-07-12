using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;

namespace TurnipTallyApi.Services
{
    public interface IUserService
    {
        RegisteredUser Authenticate(string email, string password);
        Task<RegisteredUser> Create(string email, string password, string timezoneId);
        RegisteredUser GetById(long id);
        Task SendPasswordReset(string email);
        Task UpdatePassword(long id, string newPassword);
    }

    public class UserService : IUserService
    {
        private readonly TurnipContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _email;

        public UserService(TurnipContext context, ILogger<UserService> logger, IEmailService email)
        {
            _context = context;
            _logger = logger;
            _email = email;
        }

        public RegisteredUser Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _context.RegisteredUsers.SingleOrDefault(u => u.Email.Equals(email));

            if (user == null)
            {
                return null;
            }

            return !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) ? null : user;
        }

        public async Task<RegisteredUser> Create(string email, string password, string timezoneId)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ApplicationException("Password is required");
            }

            if (_context.RegisteredUsers.Any(u => u.Email.Equals(email.ToLowerInvariant())))
            {
                throw new ApplicationException($"The email {email} is already registered");
            }

            (byte[] hash, byte[] salt) = HashPassword(password);

            var user = new RegisteredUser
            {
                Email = email.ToLowerInvariant(),
                PasswordHash = hash,
                PasswordSalt = salt,
                TimezoneId = timezoneId
            };

            await _context.RegisteredUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered");

            return user;
        }

        public RegisteredUser GetById(long id)
        {
            return _context.RegisteredUsers.Find(id);
        }

        public async Task UpdatePassword(long id, string newPassword)
        {
            var user = await _context.RegisteredUsers.SingleOrDefaultAsync(ru => ru.Id.Equals(id));

            if(user == null)
            {
                throw new ApplicationException("User not found for password reset");
            }

            var (hash, salt) = HashPassword(newPassword);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            await _context.SaveChangesAsync();
        }

        public async Task SendPasswordReset(string email)
        {
            var user = await _context.RegisteredUsers.FirstOrDefaultAsync(ru =>
                ru.Email.Equals(email.ToLowerInvariant()));

            if(user == null)
            {
                return;
            }

            var passwordReset = new PasswordReset
            {
                RegisteredUserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddMinutes(30),
                Key = Guid.NewGuid()
            };

            await _context.PasswordResets.AddAsync(passwordReset);
            await _context.SaveChangesAsync();

            var body = GeneratePasswordResetEmailBody(passwordReset.Key);

            _email.SendEmail(user.Email, body, "Turnip Tally Password Reset", false);
        }

        private static (byte[] hash, byte[] salt) HashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be empty or only whitespace", nameof(password));

            using var hmac = new HMACSHA512();

            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return (hash, salt);
        }

        private static bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be empty or only whitespace", nameof(password));
            if (hash.Length != 64) throw new ArgumentException("Invalid length of password hash", nameof(hash));
            if (salt.Length != 128) throw new ArgumentException("Invalid length of password salt", nameof(salt));

            using var hmac = new HMACSHA512(salt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return !computedHash.Where((t, i) => t != hash[i]).Any();
        }

        private static string GeneratePasswordResetEmailBody(Guid key)
        {
            return
                $"Hello,\r\n\r\nIt looks you have requested a password reset. Here is the link to reset your password, it is valid for 20 minutes:\r\n\r\nhttps://turniptally.com/resetPassword?key={key}\r\n\r\nIf you didn't request a password reset, please reply to this email.\r\n\r\nTurnip Tally";
        }
    }
}