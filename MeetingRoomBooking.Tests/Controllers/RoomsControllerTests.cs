using MeetingRoomBooking.Application.Contracts.RoomContracts;
using MeetingRoomBooking.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MeetingRoomBooking.Application.Services.Abstraction;
using Xunit;

namespace MeetingRoomBooking.Tests.Controllers
{
    public class RoomsControllerTests
    {
        private readonly Mock<IRoomService> _mockRoomService;
        private readonly MeetingRoomBooking.API.Controllers.Rooms.RoomsController _controller;

        public RoomsControllerTests()
        {
            _mockRoomService = new Mock<IRoomService>();
            _controller = new MeetingRoomBooking.API.Controllers.Rooms.RoomsController(_mockRoomService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsRooms()
        {
            var rooms = new List<RoomDto>
            {
                new RoomDto { Id = Guid.NewGuid(), Name = "Room 1", Capacity = 10 },
                new RoomDto { Id = Guid.NewGuid(), Name = "Room 2", Capacity = 20 }
            };

            _mockRoomService.Setup(x => x.GetAllRoomsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(rooms);

            var result = await _controller.GetAllAsync(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(rooms, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WhenRoomExists_ReturnsRoom()
        {
            var roomId = Guid.NewGuid();
            var room = new RoomDto
            {
                Id = roomId,
                Name = "Test Room",
                Capacity = 10
            };

            _mockRoomService.Setup(x => x.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            var result = await _controller.GetByIdAsync(roomId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(room, okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WhenRoomNotFound_ReturnsNotFound()
        {
            var roomId = Guid.NewGuid();

            _mockRoomService.Setup(x => x.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RoomDto?)null);

            var result = await _controller.GetByIdAsync(roomId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateRoomAsync_WhenValid_ReturnsCreated()
        {
            var roomDto = new RoomDto
            {
                Name = "New Room",
                Capacity = 15
            };

            var createdRoom = new RoomDto
            {
                Id = Guid.NewGuid(),
                Name = roomDto.Name,
                Capacity = roomDto.Capacity
            };

            _mockRoomService.Setup(x => x.CreateRoomAsync(roomDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdRoom);

            var result = await _controller.CreateRoomAsync(roomDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdRoom, createdResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_WhenValid_ReturnsNoContent()
        {
            var roomId = Guid.NewGuid();
            var roomDto = new RoomDto
            {
                Name = "Updated Room",
                Capacity = 20
            };

            _mockRoomService.Setup(x => x.UpdateRoomAsync(roomId, roomDto, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateAsync(roomId, roomDto, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_WhenValid_ReturnsNoContent()
        {
            var roomId = Guid.NewGuid();

            _mockRoomService.Setup(x => x.DeleteRoomAsync(roomId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(roomId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
