using MeetingRoomBooking.Application.Contracts.AuthContracts;
using MeetingRoomBooking.Application.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IUserService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken ct)
        {
            var user = await _userService.RegisterUserAsync(
                registerDto.Email,
                registerDto.Username,
                registerDto.Password,
                registerDto.FullName,
                ct);

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Username);

            var response = new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken ct)
        {
            var user = await _userService.AuthenticateUserAsync(loginDto.Email, loginDto.Password, ct);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Username);

            var response = new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName
            };

            return Ok(response);
        }
    }
}
