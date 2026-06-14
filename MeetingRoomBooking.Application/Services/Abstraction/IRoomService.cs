using MeetingRoomBooking.Application.Contracts.RoomContracts;

namespace MeetingRoomBooking.Application.Services.Abstraction
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync(CancellationToken ct = default);
        Task<RoomDto?> GetRoomByIdAsync(Guid id, CancellationToken ct = default);
        Task<RoomDto> CreateRoomAsync(RoomDto roomDto, CancellationToken ct = default);
        Task UpdateRoomAsync(Guid id, RoomDto roomDto, CancellationToken ct = default);
        Task DeleteRoomAsync(Guid id, CancellationToken ct = default);
    }
}
