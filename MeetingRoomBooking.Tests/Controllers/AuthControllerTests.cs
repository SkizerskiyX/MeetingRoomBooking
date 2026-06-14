using MeetingRoomBooking.Application.Contracts.AuthContracts;
using MeetingRoomBooking.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MeetingRoomBooking.Application.Services.Abstraction;
using Xunit;

namespace MeetingRoomBooking.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly MeetingRoomBooking.API.Controllers.Auth.AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _controller = new MeetingRoomBooking.API.Controllers.Auth.AuthController(
                _mockUserService.Object,
                _mockJwtTokenService.Object);
        }

        [Fact]
        public async Task Register_WhenValid_ReturnsAuthResponse()
        {
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "password123",
                FullName = "Test User"
            };

            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = registerDto.Email,
                Username = registerDto.Username,
                FullName = registerDto.FullName
            };

            var token = "test-jwt-token";

            _mockUserService.Setup(x => x.RegisterUserAsync(
                registerDto.Email,
                registerDto.Username,
                registerDto.Password,
                registerDto.FullName,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockJwtTokenService.Setup(x => x.GenerateToken(user.Id, user.Email, user.Username))
                .Returns(token);

            var result = await _controller.Register(registerDto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal(token, response.Token);
            Assert.Equal(user.Id, response.UserId);
        }

        [Fact]
        public async Task Login_WhenValid_ReturnsAuthResponse()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = loginDto.Email,
                Username = "testuser"
            };

            var token = "test-jwt-token";

            _mockUserService.Setup(x => x.AuthenticateUserAsync(
                loginDto.Email,
                loginDto.Password,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockJwtTokenService.Setup(x => x.GenerateToken(user.Id, user.Email, user.Username))
                .Returns(token);

            var result = await _controller.Login(loginDto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal(token, response.Token);
        }

        [Fact]
        public async Task Login_WhenInvalid_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _mockUserService.Setup(x => x.AuthenticateUserAsync(
                loginDto.Email,
                loginDto.Password,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.User?)null);

            var result = await _controller.Login(loginDto, CancellationToken.None);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
