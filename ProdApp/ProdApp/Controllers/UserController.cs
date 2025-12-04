using AutoMapper;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ProdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ProdDbContext _context;
        private readonly IMapper _mapper;
        public UserController(ProdDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
                return BadRequest("UserName already exists");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                RegisteredAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok($"{dto.UserName} registered successfully as User" );
            return CreatedAtRoute("GetUserById", new { id = user.UserId }, _mapper.Map<UserDTO>(user));

        }

        [HttpGet]
        [Route("All",Name ="GetAllUsers")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var user = await _context.Users.ToListAsync();
            
            if (user == null||user.Count==0)
                return NotFound("No users found");

            var dto = _mapper.Map<IEnumerable<UserDTO>>(user);

            return Ok(dto);
        }

        [HttpGet]
        [Route("{id:int}",Name ="GetUserById")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUser(int id)
        {
            if (id <= 0)
                return BadRequest("Provide valid user");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("No User found");

            var dto = _mapper.Map<UserDTO>(user);
            return Ok(dto);
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "GetUserByName")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUserName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Provide valid name");

            var user = await _context.Users.FirstOrDefaultAsync(u=>u.UserName.ToLower() == name.ToLower());
            if (user == null)
                return NotFound("No User found with name : {name}");

            var dto = _mapper.Map<UserDTO>(user);
            return Ok(dto);
        }

        [HttpDelete]
        [Route("Delete",Name ="DeleteUser")]
        [Authorize(Roles="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Deleteuser(int id)
        {
            if (id <= 0)
                return BadRequest();
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

    }
}
