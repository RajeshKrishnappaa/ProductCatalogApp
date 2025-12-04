using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProdApp.Data;
using ProdApp.DTOS;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class LoginController : ControllerBase
    {
        private readonly ProdDbContext _context;
        private readonly IConfiguration _configuration;
        public LoginController(ProdDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (user == null)
                return Unauthorized("Invalid Username or password");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid Username or password");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,dto.UserName),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                Issuer = _configuration["JwtConfig:Issuer"],
                Audience = _configuration["JwtConfig:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var JwtConfig = tokenHandler.WriteToken(token);

            return Ok(new LoginResponseDTO
            {
                UserName = user.UserName,
                Token = JwtConfig,
                Expiry = tokenDescriptor.Expires.Value,
                Role = user.Role
            });
        }
    }
}
