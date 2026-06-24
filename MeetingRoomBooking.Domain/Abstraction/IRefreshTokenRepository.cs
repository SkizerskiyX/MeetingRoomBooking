using MeetingRoomBooking.Domain.Entities;

namespace MeetingRoomBooking.Domain.Abstraction
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task RevokeAsync(Guid id, CancellationToken ct = default);
    }
}
