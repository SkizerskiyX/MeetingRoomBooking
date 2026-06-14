using MeetingRoomBooking.Application.Contracts.BookingContacts;
using MeetingRoomBooking.Application.Services;
using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using Moq;
using Xunit;

namespace MeetingRoomBooking.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepo;
        private readonly Mock<IRoomRepository> _mockRoomRepo;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepo = new Mock<IBookingRepository>();
            _mockRoomRepo = new Mock<IRoomRepository>();
            _bookingService = new BookingService(_mockBookingRepo.Object, _mockRoomRepo.Object);
        }

        [Fact]
        public async Task GetBookingByIdAsync_WhenBookingExists_ReturnsBookingDto()
        {
            var bookingId = Guid.NewGuid();
            var booking = new Booking
            {
                Id = bookingId,
                RoomId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            var result = await _bookingService.GetBookingByIdAsync(bookingId);

            Assert.NotNull(result);
            Assert.Equal(bookingId, result.Id);
        }

        [Fact]
        public async Task GetBookingByIdAsync_WhenBookingNotFound_ReturnsNull()
        {
            var bookingId = Guid.NewGuid();
            _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            var result = await _bookingService.GetBookingByIdAsync(bookingId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetBookingsByRoomIdAsync_ReturnsBookingDtos()
        {
            var roomId = Guid.NewGuid();
            var bookings = new List<Booking>
            {
                new Booking { Id = Guid.NewGuid(), RoomId = roomId, UserId = Guid.NewGuid(), StartTime = DateTimeOffset.UtcNow.AddHours(1), EndTime = DateTimeOffset.UtcNow.AddHours(2) },
                new Booking { Id = Guid.NewGuid(), RoomId = roomId, UserId = Guid.NewGuid(), StartTime = DateTimeOffset.UtcNow.AddHours(3), EndTime = DateTimeOffset.UtcNow.AddHours(4) }
            };

            _mockBookingRepo.Setup(x => x.GetByRoomIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            var result = await _bookingService.GetBookingsByRoomIdAsync(roomId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateBookingAsync_WhenRoomNotFound_ThrowsKeyNotFoundException()
        {
            var roomId = Guid.NewGuid();
            var bookingDto = new BookingDto
            {
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _mockRoomRepo.Setup(x => x.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Room?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _bookingService.CreateBookingAsync(roomId, bookingDto, Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateBookingAsync_WhenConflictExists_ThrowsInvalidOperationException()
        {
            var roomId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var bookingDto = new BookingDto
            {
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _mockRoomRepo.Setup(x => x.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Room { Id = roomId, Name = "Test Room", Capacity = 10 });
            _mockBookingRepo.Setup(x => x.HasConflictAsync(roomId, bookingDto.StartTime, bookingDto.EndTime, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookingService.CreateBookingAsync(roomId, bookingDto, userId));
        }

        [Fact]
        public async Task CreateBookingAsync_WhenValid_CreatesBooking()
        {
            var roomId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var bookingDto = new BookingDto
            {
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _mockRoomRepo.Setup(x => x.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Room { Id = roomId, Name = "Test Room", Capacity = 10 });
            _mockBookingRepo.Setup(x => x.HasConflictAsync(roomId, bookingDto.StartTime, bookingDto.EndTime, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockBookingRepo.Setup(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _bookingService.CreateBookingAsync(roomId, bookingDto, userId);

            Assert.NotNull(result);
            _mockBookingRepo.Verify(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenBookingNotFound_ThrowsKeyNotFoundException()
        {
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _bookingService.DeleteBookingAsync(bookingId, userId));
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenUserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var booking = new Booking
            {
                Id = bookingId,
                RoomId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _bookingService.DeleteBookingAsync(bookingId, userId));
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenAuthorized_DeletesBooking()
        {
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var booking = new Booking
            {
                Id = bookingId,
                RoomId = Guid.NewGuid(),
                UserId = userId,
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);
            _mockBookingRepo.Setup(x => x.DeleteAsync(bookingId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _bookingService.DeleteBookingAsync(bookingId, userId);

            _mockBookingRepo.Verify(x => x.DeleteAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
