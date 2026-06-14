using MeetingRoomBooking.Application.Contracts.BookingContacts;
using MeetingRoomBooking.Domain.Entities;

namespace MeetingRoomBooking.Application.Services.Abstraction
{
    public interface IBookingService
    {
        Task<BookingResponseDto?> GetBookingByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByRoomIdAsync(Guid roomId, CancellationToken ct = default);
        Task<BookingResponseDto> CreateBookingAsync(Guid roomId, BookingDto bookingDto, Guid userId, CancellationToken ct = default);
        Task DeleteBookingAsync(Guid id, Guid userId, CancellationToken ct = default);
    }
}
