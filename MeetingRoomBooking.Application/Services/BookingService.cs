using MeetingRoomBooking.Application.Contracts.BookingContacts;
using MeetingRoomBooking.Application.Services.Abstraction;
using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;

namespace MeetingRoomBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(Guid id, CancellationToken ct = default)
        {
            var booking = await _bookingRepository.GetByIdAsync(id, ct);
            return booking?.ToDto();
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByRoomIdAsync(Guid roomId, CancellationToken ct = default)
        {
            var bookings = await _bookingRepository.GetByRoomIdAsync(roomId, ct);
            return bookings.Select(b => b.ToDto());
        }

        public async Task<BookingResponseDto> CreateBookingAsync(Guid roomId, BookingDto bookingDto, Guid userId, CancellationToken ct = default)
        {
            var room = await _roomRepository.GetByIdAsync(roomId, ct);
            if (room == null)
                throw new KeyNotFoundException($"Room with id {roomId} not found");

            var hasConflict = await _bookingRepository.HasConflictAsync(roomId, bookingDto.StartTime, bookingDto.EndTime, ct);
            if (hasConflict)
                throw new InvalidOperationException("Room is already booked for this time slot");

            var booking = bookingDto.ToEntity(roomId, userId.ToString());
            await _bookingRepository.AddAsync(booking, ct);
            return booking.ToDto();
        }

        public async Task DeleteBookingAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var booking = await _bookingRepository.GetByIdAsync(id, ct);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with id {id} not found");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own bookings");

            await _bookingRepository.DeleteAsync(id, ct);
        }
    }
}
