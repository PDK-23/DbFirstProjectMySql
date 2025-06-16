using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Application.DTOs.User;
using DbFirstProjectMySql.Application.Interfaces;
using DbFirstProjectMySql.Infrastructure.Entities;
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
        private readonly JwtService _jwtService;

        public AuthController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
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

            var result = await _userService.CreateAsync(userCreate);
            if (result == null)
                return Conflict("Username already exists!");

            return Ok(result);
        }


        // POST: api/Auth/login
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

            // Lấy role, id để sinh token
            var token = _jwtService.GenerateToken(entity.Id.ToString(), entity.Username, entity.RoleId.ToString());
            return Ok(new { token });
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
