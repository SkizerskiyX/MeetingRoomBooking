using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Infrastructure.MeetingRoomContext;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.Email == email, ct);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.Username == username, ct);
        }
    }
}
