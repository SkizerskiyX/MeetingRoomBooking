using MeetingRoomBooking.Application.Services.Abstraction;
using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace MeetingRoomBooking.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _userRepository.GetByIdAsync(id, ct);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _userRepository.GetByEmailAsync(email, ct);
        }

        public async Task<User> RegisterUserAsync(string email, string username, string password, string? fullName = null, CancellationToken ct = default)
        {
            if (await _userRepository.ExistsByEmailAsync(email, ct))
                throw new InvalidOperationException("User with this email already exists");

            if (await _userRepository.ExistsByUsernameAsync(username, ct))
                throw new InvalidOperationException("User with this username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = HashPassword(password),
                FullName = fullName,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, ct);
            return user;
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, ct);
            return user;
        }

        public async Task UpdateLastLoginAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user, ct);
            }
        }

        private static string HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashBytes = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);
            return Convert.ToBase64String(hashBytes);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);
            var salt = new byte[128];
            Array.Copy(hashBytes, 0, salt, 0, salt.Length);
            var hash = new byte[hashBytes.Length - salt.Length];
            Array.Copy(hashBytes, salt.Length, hash, 0, hash.Length);

            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }

        internal static string HashPasswordForTesting(string password)
        {
            return HashPassword(password);
        }
    }
}
