using DbFirstProjectMySql.Application.DTOs;
using DbFirstProjectMySql.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DbFirstProjectMySql.API.Controllers
{
    [ApiController]
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
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
        //{
        //    if (id != dto.Id) return BadRequest("ID mismatch");
        //    await _userService.UpdateAsync(dto);
        //    return Ok();
        //}

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }
    }
}
