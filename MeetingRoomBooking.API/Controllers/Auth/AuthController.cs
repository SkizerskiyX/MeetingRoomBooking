using MeetingRoomBooking.Application.Contracts.AuthContracts;
using MeetingRoomBooking.Application.Services.Abstraction;
using MeetingRoomBooking.Domain.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace MeetingRoomBooking.API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public AuthController(IUserService userService, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepo)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepo = refreshTokenRepo;
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
            // create refresh token
            var refreshToken = new MeetingRoomBooking.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = user.Id
            };
            await _refreshTokenRepo.AddAsync(refreshToken, ct);

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName
            };

            // set refresh token as secure HttpOnly cookie so browser keeps user logged in
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiresAt,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken ct)
        {
            var user = await _userService.AuthenticateUserAsync(loginDto.Email, loginDto.Password, ct);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Username);
            var refreshToken = new MeetingRoomBooking.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = user.Id
            };
            await _refreshTokenRepo.AddAsync(refreshToken, ct);

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName
            };

            // set refresh token as secure HttpOnly cookie so browser keeps user logged in
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiresAt,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto, CancellationToken ct)
        {
            // allow refresh token to be supplied either in request body or in secure cookie
            var providedToken = dto?.RefreshToken;
            if (string.IsNullOrEmpty(providedToken))
            {
                Request.Cookies.TryGetValue("refreshToken", out var cookieToken);
                providedToken = cookieToken;
            }
            if (string.IsNullOrEmpty(providedToken)) return BadRequest();
            var existing = await _refreshTokenRepo.GetByTokenAsync(providedToken, ct);
            if (existing == null || existing.ExpiresAt < DateTime.UtcNow || existing.RevokedAt != null)
                return Unauthorized();

            var user = existing.User;
            if (user == null) return Unauthorized();

            // rotate
            existing.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepo.RevokeAsync(existing.Id, ct);

            var newRt = new MeetingRoomBooking.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = user.Id
            };
            await _refreshTokenRepo.AddAsync(newRt, ct);

            var newAccess = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Username);
            var response = new AuthResponseDto
            {
                Token = newAccess,
                RefreshToken = newRt.Token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName
            };
            return Ok(response);
        }
    }
}
