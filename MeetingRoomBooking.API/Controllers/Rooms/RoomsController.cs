using MeetingRoomBooking.Application.Contracts.RoomContracts;
using MeetingRoomBooking.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.API.Controllers.Rooms
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        {
            var rooms = await _roomService.GetAllRoomsAsync(ct);
            return Ok(rooms);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var room = await _roomService.GetRoomByIdAsync(id, ct);
            if (room == null) return NotFound();
            return Ok(room);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoomAsync(RoomDto roomDto, CancellationToken ct)
        {
            var room = await _roomService.CreateRoomAsync(roomDto, ct);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = room.Id }, room);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, RoomDto roomDto, CancellationToken ct)
        {
            await _roomService.UpdateRoomAsync(id, roomDto, ct);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
        {
            await _roomService.DeleteRoomAsync(id, ct);
            return NoContent();
        }
    }
}
