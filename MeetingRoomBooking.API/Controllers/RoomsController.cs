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
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        {
            var room = await _roomRepository.GetAllAsync(ct);
            return Ok(room);
            
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var room = await _roomRepository.GetByIdAsync(id, ct);
            if (room == null) return NotFound();
            return Ok(room);
        }
        [HttpPost]
        public async Task<IActionResult> RoomCreateAsync(RoomDto roomDto, CancellationToken ct)
        {
            var room = roomDto.ToEntity();

            await _roomRepository.AddAsync(room, ct);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = room.Id }, room.ToDto());
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, RoomDto roomDto, CancellationToken ct)
        {
            var room = roomDto.ToEntity();
            room.Id = id;
            try
            {
                await _roomRepository.UpdateAsync(room, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
        {
            try
            {
                await _roomRepository.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        
    }
}
