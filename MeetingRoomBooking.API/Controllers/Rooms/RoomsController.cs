using MeetingRoomBooking.Application.Contracts.RoomContracts;
using MeetingRoomBooking.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.API.Controllers.Rooms
{
    [ApiController]
    [Route("api/rooms")]
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
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
        {
            var room = await _roomService.GetRoomByIdAsync(id, ct);
            if (room == null) return NotFound();
            return Ok(room);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoomAsync([FromBody] RoomDto roomDto, CancellationToken ct)
        {
            var room = await _roomService.CreateRoomAsync(roomDto, ct);
            return Created($"/api/rooms/{room.Id}", room);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] RoomDto roomDto, CancellationToken ct)
        {
            await _roomService.UpdateRoomAsync(id, roomDto, ct);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        {
            await _roomService.DeleteRoomAsync(id, ct);
            return NoContent();
        }
    }
}
