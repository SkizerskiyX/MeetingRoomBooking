using MeetingRoomBooking.Application.Contracts;
using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
namespace MeetingRoom.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RoomCreateAsync(RoomDto roomDto, CancellationToken ct)
        {
            var room = roomDto.ToEntity();

            await _roomRepository.AddAsync(room, ct);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = room.Id }, room);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var room = await _roomRepository.GetByIdAsync(id, ct);
            if (room == null) return NotFound();
            return Ok(room);
        }
    }
}
