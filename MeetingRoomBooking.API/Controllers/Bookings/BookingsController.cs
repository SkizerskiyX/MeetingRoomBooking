using MeetingRoomBooking.Application.Contracts.BookingContacts;
using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeetingRoom.API.Controllers.Bookings
{
    [ApiController]
    [Authorize]
    [Route("api/rooms/{roomId}/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingsAsync([FromRoute] Guid roomId, [FromRoute] Guid id, CancellationToken ct)
        {
            var bookings = await _bookingRepository.GetByIdAsync(id, ct);
            if (bookings is null) return NotFound();
            return Ok(bookings);

        }
        [HttpGet]
        public async Task<IActionResult> GetByRoomIdAsync([FromRoute] Guid roomId, CancellationToken ct)
        {
            var bookings = await _bookingRepository.GetByRoomIdAsync(roomId, ct);
            return Ok(bookings);
        }

        [HttpPost]
            public async Task<IActionResult> AddBookingAsync(Guid roomId, BookingDto bookingDto, CancellationToken ct)
            {
                var hasConflict = await _bookingRepository.HasConflictAsync(roomId, bookingDto.StartTime, bookingDto.EndTime, ct);
                if (hasConflict) return Conflict("Room is already booked for this time");

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var booking = bookingDto.ToEntity(roomId, userId!);

                await _bookingRepository.AddAsync(booking, ct);

            return CreatedAtAction(nameof(GetBookingsAsync), new { id = booking.Id }, booking.ToDto());

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookingsAsync(Guid id, CancellationToken ct)
        {
            try
            {
                await _bookingRepository.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
}
}
