using MeetingRoomBooking.Domain.Entities;

namespace MeetingRoomBooking.Application.Services.Abstraction
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default);
        Task<User> RegisterUserAsync(string email, string username, string password, string? fullName = null, CancellationToken ct = default);
        Task<User?> AuthenticateUserAsync(string email, string password, CancellationToken ct = default);
        Task UpdateLastLoginAsync(Guid userId, CancellationToken ct = default);
    }
}
