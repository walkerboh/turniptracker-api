using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TurnipTallyApi.Database;
using TurnipTallyApi.Database.Entities;

namespace TurnipTallyApi.Services
{
    public interface IUserService
    {
        RegisteredUser Authenticate(string email, string password);
        Task<RegisteredUser> Create(string email, string password);
        RegisteredUser GetById(long id);
    }

    public class UserService : IUserService
    {
        private readonly TurnipContext _context;

        public UserService(TurnipContext context)
        {
            _context = context;
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

        public async Task<RegisteredUser> Create(string email, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ApplicationException("Password is required");
            }

            if (_context.RegisteredUsers.Any(u => u.Email.Equals(email)))
            {
                throw new Exceptions.ApplicationException($"The email {email} is already registered");
            }

            (byte[] hash, byte[] salt) = HashPassword(password);

            var user = new RegisteredUser
            {
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _context.RegisteredUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public RegisteredUser GetById(long id)
        {
            return _context.RegisteredUsers.Find(id);
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
    }
}