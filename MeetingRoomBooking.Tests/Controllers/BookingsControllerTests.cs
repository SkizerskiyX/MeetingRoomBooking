using MeetingRoomBooking.Application.Contracts.BookingContacts;
using MeetingRoomBooking.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MeetingRoomBooking.Application.Services.Abstraction;
using System.Security.Claims;
using Xunit;

namespace MeetingRoomBooking.Tests.Controllers
{
    public class BookingsControllerTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly MeetingRoomBooking.API.Controllers.Bookings.BookingsController _controller;

        public BookingsControllerTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _controller = new MeetingRoomBooking.API.Controllers.Bookings.BookingsController(_mockBookingService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetBookingAsync_WhenBookingExists_ReturnsOk()
        {
            var bookingId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var bookingDto = new BookingResponseDto
            {
                Id = bookingId,
                RoomId = roomId,
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _mockBookingService.Setup(x => x.GetBookingByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookingDto);

            var result = await _controller.GetBookingAsync(roomId, bookingId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(bookingDto, okResult.Value);
        }

        [Fact]
        public async Task GetBookingAsync_WhenBookingNotFound_ReturnsNotFound()
        {
            var bookingId = Guid.NewGuid();
            var roomId = Guid.NewGuid();

            _mockBookingService.Setup(x => x.GetBookingByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookingResponseDto?)null);

            var result = await _controller.GetBookingAsync(roomId, bookingId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByRoomIdAsync_ReturnsBookings()
        {
            var roomId = Guid.NewGuid();
            var bookings = new List<BookingResponseDto>
            {
                new BookingResponseDto { Id = Guid.NewGuid(), RoomId = roomId, StartTime = DateTimeOffset.UtcNow.AddHours(1), EndTime = DateTimeOffset.UtcNow.AddHours(2) }
            };

            _mockBookingService.Setup(x => x.GetBookingsByRoomIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            var result = await _controller.GetByRoomIdAsync(roomId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(bookings, okResult.Value);
        }

        [Fact]
        public async Task AddBookingAsync_WhenValid_ReturnsCreated()
        {
            var roomId = Guid.NewGuid();
            var bookingDto = new BookingDto
            {
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            var responseDto = new BookingResponseDto
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime
            };

            _mockBookingService.Setup(x => x.CreateBookingAsync(roomId, bookingDto, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseDto);

            var result = await _controller.AddBookingAsync(roomId, bookingDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(responseDto, createdResult.Value);
        }

        [Fact]
        public async Task AddBookingAsync_WhenUnauthorized_ReturnsUnauthorized()
        {
            var roomId = Guid.NewGuid();
            var bookingDto = new BookingDto
            {
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            var result = await _controller.AddBookingAsync(roomId, bookingDto, CancellationToken.None);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenAuthorized_ReturnsNoContent()
        {
            var bookingId = Guid.NewGuid();

            _mockBookingService.Setup(x => x.DeleteBookingAsync(bookingId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteBookingAsync(bookingId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenUnauthorized_ReturnsUnauthorized()
        {
            var bookingId = Guid.NewGuid();

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            var result = await _controller.DeleteBookingAsync(bookingId, CancellationToken.None);

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
