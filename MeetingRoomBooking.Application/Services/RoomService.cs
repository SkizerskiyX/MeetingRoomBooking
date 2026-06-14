using MeetingRoomBooking.Application.Contracts.RoomContracts;
using MeetingRoomBooking.Application.Services.Abstraction;
using MeetingRoomBooking.Domain.Abstraction;

namespace MeetingRoomBooking.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync(CancellationToken ct = default)
        {
            var rooms = await _roomRepository.GetAllAsync(ct);
            return rooms.Select(r => r.ToDto());
        }

        public async Task<RoomDto?> GetRoomByIdAsync(Guid id, CancellationToken ct = default)
        {
            var room = await _roomRepository.GetByIdAsync(id, ct);
            return room?.ToDto();
        }

        public async Task<RoomDto> CreateRoomAsync(RoomDto roomDto, CancellationToken ct = default)
        {
            var room = roomDto.ToEntity();
            await _roomRepository.AddAsync(room, ct);
            return room.ToDto();
        }

        public async Task UpdateRoomAsync(Guid id, RoomDto roomDto, CancellationToken ct = default)
        {
            var room = roomDto.ToEntity();
            room.Id = id;
            await _roomRepository.UpdateAsync(room, ct);
        }

        public async Task DeleteRoomAsync(Guid id, CancellationToken ct = default)
        {
            await _roomRepository.DeleteAsync(id, ct);
        }
    }
}
