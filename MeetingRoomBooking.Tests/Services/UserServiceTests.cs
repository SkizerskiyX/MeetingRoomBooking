using MeetingRoomBooking.Application.Services;
using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using Moq;
using Xunit;

namespace MeetingRoomBooking.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepo.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepo.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenUserExists_ReturnsUser()
        {
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = "testuser",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepo.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _userService.GetUserByEmailAsync(email);

            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task RegisterUserAsync_WhenEmailExists_ThrowsInvalidOperationException()
        {
            _mockUserRepo.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.RegisterUserAsync("test@example.com", "testuser", "password123"));
        }

        [Fact]
        public async Task RegisterUserAsync_WhenUsernameExists_ThrowsInvalidOperationException()
        {
            _mockUserRepo.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockUserRepo.Setup(x => x.ExistsByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.RegisterUserAsync("test@example.com", "testuser", "password123"));
        }

        [Fact]
        public async Task RegisterUserAsync_WhenValid_CreatesUser()
        {
            _mockUserRepo.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockUserRepo.Setup(x => x.ExistsByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockUserRepo.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _userService.RegisterUserAsync("test@example.com", "testuser", "password123", "Test User");

            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("testuser", result.Username);
            _mockUserRepo.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenUserNotFound_ReturnsNull()
        {
            _mockUserRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var result = await _userService.AuthenticateUserAsync("test@example.com", "password123");

            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenPasswordInvalid_ReturnsNull()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = UserService.HashPasswordForTesting("correctpassword123"),
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepo.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _userService.AuthenticateUserAsync("test@example.com", "wrongpassword");

            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenValid_ReturnsUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = UserService.HashPasswordForTesting("password123"),
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepo.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockUserRepo.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

          
            var result = await _userService.AuthenticateUserAsync("test@example.com", "password123");

            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task UpdateLastLoginAsync_WhenUserExists_UpdatesLastLogin()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepo.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockUserRepo.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _userService.UpdateLastLoginAsync(userId);

            _mockUserRepo.Verify(x => x.UpdateAsync(It.Is<User>(u => u.LastLoginAt != null), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
