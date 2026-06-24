using MeetingRoomBooking.Application.Contracts.BookingContacts;
using MeetingRoomBooking.Application.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeetingRoomBooking.API.Controllers.Bookings
{
    [ApiController]
    [Authorize]
    [Route("api/rooms/{roomId}/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingAsync([FromRoute] Guid roomId, [FromRoute] Guid id, CancellationToken ct)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id, ct);
            if (booking is null) return NotFound();
            return Ok(booking);
        }

        [HttpGet]
        public async Task<IActionResult> GetByRoomIdAsync([FromRoute] Guid roomId, CancellationToken ct)
        {
            var bookings = await _bookingService.GetBookingsByRoomIdAsync(roomId, ct);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> AddBookingAsync([FromRoute] Guid roomId, [FromBody] BookingDto bookingDto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var booking = await _bookingService.CreateBookingAsync(roomId, bookingDto, userId, ct);
            return Created($"/api/rooms/{roomId}/bookings/{booking.Id}", booking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookingAsync([FromRoute] Guid roomId, [FromRoute] Guid id, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _bookingService.DeleteBookingAsync(id, userId, ct);
            return NoContent();
        }
    }
}
