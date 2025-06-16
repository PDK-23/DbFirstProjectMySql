using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DbFirstProjectMySql.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var result = await _userService.CreateAsync(dto);
            if (result == null)
                return Conflict("Username already exists!");

            return Ok(result);
        }

        // PUT: api/Users/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserEditDto dto)
        {
            try
            {
                await _userService.UpdateAsync(dto, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }
    }
}
