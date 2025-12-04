using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdApp.DTOS;
using ProdApp.Services.Interfaces;

namespace ProdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        // POST: api/User/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.RegisterUserAsync(dto);

            return Ok($"{created.UserName} registered successfully");
        }

        // GET: api/User/All
        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsersAsync();

            if (!users.Any())
                return NotFound("No users found");

            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id:int}", Name = "GetUserById")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _service.GetUserByIdAsync(id);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // GET: api/User/name/Rajesh
        [HttpGet("name/{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var user = await _service.GetUserByNameAsync(name);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // DELETE: api/User/Delete/5
        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _service.DeleteUserAsync(id);

            if (!result)
                return NotFound("User not found");

            return Ok("User deleted successfully");
        }
    }
}
