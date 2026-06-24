using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Infrastructure.MeetingRoomContext;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(x => x.Token == token, ct);
        }

        public async Task RevokeAsync(Guid id, CancellationToken ct = default)
        {
            var rt = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (rt == null) return;
            rt.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }
}
