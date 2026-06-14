using MeetingRoomBooking.Application.Contracts.RoomContracts;
using MeetingRoomBooking.Application.Services;
using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using Moq;
using Xunit;

namespace MeetingRoomBooking.Tests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _mockRoomRepo;
        private readonly RoomService _roomService;

        public RoomServiceTests()
        {
            _mockRoomRepo = new Mock<IRoomRepository>();
            _roomService = new RoomService(_mockRoomRepo.Object);
        }

        [Fact]
        public async Task GetAllRoomsAsync_ReturnsRoomDtos()
        {
            var rooms = new List<Room>
            {
                new Room { Id = Guid.NewGuid(), Name = "Room 1", Capacity = 10 },
                new Room { Id = Guid.NewGuid(), Name = "Room 2", Capacity = 20 }
            };

            _mockRoomRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(rooms);

            var result = await _roomService.GetAllRoomsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetRoomByIdAsync_WhenRoomExists_ReturnsRoomDto()
        {
            var roomId = Guid.NewGuid();
            var room = new Room
            {
                Id = roomId,
                Name = "Test Room",
                Capacity = 10
            };

            _mockRoomRepo.Setup(x => x.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(room);

            var result = await _roomService.GetRoomByIdAsync(roomId);

            Assert.NotNull(result);
            Assert.Equal(roomId, result.Id);
        }

        [Fact]
        public async Task GetRoomByIdAsync_WhenRoomNotFound_ReturnsNull()
        {
            var roomId = Guid.NewGuid();
            _mockRoomRepo.Setup(x => x.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Room?)null);

            var result = await _roomService.GetRoomByIdAsync(roomId);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateRoomAsync_CreatesRoom()
        {
            var roomDto = new RoomDto
            {
                Name = "New Room",
                Capacity = 15
            };

            _mockRoomRepo.Setup(x => x.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _roomService.CreateRoomAsync(roomDto);

            Assert.NotNull(result);
            Assert.Equal("New Room", result.Name);
            _mockRoomRepo.Verify(x => x.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRoomAsync_UpdatesRoom()
        {
            var roomId = Guid.NewGuid();
            var roomDto = new RoomDto
            {
                Name = "Updated Room",
                Capacity = 20
            };

            _mockRoomRepo.Setup(x => x.UpdateAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _roomService.UpdateRoomAsync(roomId, roomDto);

            _mockRoomRepo.Verify(x => x.UpdateAsync(It.Is<Room>(r => r.Id == roomId && r.Name == "Updated Room"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteRoomAsync_DeletesRoom()
        {
            var roomId = Guid.NewGuid();
            _mockRoomRepo.Setup(x => x.DeleteAsync(roomId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _roomService.DeleteRoomAsync(roomId);

            _mockRoomRepo.Verify(x => x.DeleteAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
