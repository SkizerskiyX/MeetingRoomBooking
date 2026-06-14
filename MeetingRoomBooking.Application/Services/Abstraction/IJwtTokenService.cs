namespace MeetingRoomBooking.Application.Services.Abstraction
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string email, string username);
    }
}
