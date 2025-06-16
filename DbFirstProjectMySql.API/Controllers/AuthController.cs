using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Application.DTOs.User;
using DbFirstProjectMySql.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DbFirstProjectMySql.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshToken _refreshToken;
        private readonly JwtService _jwtService;

        public AuthController(IUserService userService, JwtService jwtService, IRefreshToken refreshTokenService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _refreshToken = refreshTokenService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            // Gán mặc định RoleId là 1 (User), hoặc sửa theo nhu cầu
            var userCreate = new UserCreateDto
            {
                Username = dto.Username,
                Password = dto.Password,
                RoleId = 1
            };

            var result = await _userService.RegisterAsync(userCreate);
            if (result == null)
                return Conflict("Username already exists!");

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            // Kiểm tra username + password
            var user = await _userService.GetByUsernameAsync(dto.Username);
            if (user == null)
                return Unauthorized("User not found");

            // Lấy entity để so sánh hash (UserService/Repository nên có hàm trả về entity)
            var entity = await _userService.GetEntityByUsernameAsync(dto.Username);
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, entity.PasswordHash))
                return Unauthorized("Invalid password");

            // Lấy role, id để sinh access token
            var accessToken = _jwtService.GenerateToken(entity.Id.ToString(), entity.Username, entity.RoleId.ToString());

            // Sinh refresh token (có thể lưu vào DB hoặc trả về luôn)
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Nếu muốn lưu refresh token vào DB (nên làm)
            await _refreshToken.SetUserRefreshToken(entity.Id, refreshToken);

            return Ok(new
            {
                accessToken,
                refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            // 1. Kiểm tra refresh token có tồn tại trong DB, còn hạn, chưa bị thu hồi
            var tokenInDb = await _refreshToken.GetValidRefreshTokenAsync(dto.RefreshToken);

            if (tokenInDb == null)
                return Unauthorized("Refresh token is invalid or expired!");

            // 2. Lấy userId từ token hoặc từ trường UserId
            var user = await _userService.GetByIdAsync(tokenInDb.UserId);
            if (user == null)
                return Unauthorized("User does not exist!");

            // 3. Sinh access token mới
            var accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Username, user.RoleId.ToString());

            // 4. Sinh refresh token mới và lưu vào DB
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var expiryTime = DateTime.UtcNow.AddDays(7);

            await _refreshToken.RevokeRefreshTokenAsync(tokenInDb); // Thu hồi token cũ (IsRevoked = true)
            await _refreshToken.AddRefreshToken(user.Id, newRefreshToken, expiryTime);

            return Ok(new
            {
                accessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirst("id")
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Invalid token.");

            var result = await _userService.ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword);
            if (!result.Success)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Password changed successfully." });
        }

    }
}
