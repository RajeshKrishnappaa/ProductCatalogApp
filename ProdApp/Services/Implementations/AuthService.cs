using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProdApp.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ProdDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ProdDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            var key = Encoding.UTF8.GetBytes(_config["JwtConfig:Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["JwtConfig:Issuer"],
                audience: _config["JwtConfig:Audience"],
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                },
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new LoginResponseDTO
            {
                UserName = user.UserName,
                Token = token,
                Expiry = tokenDescriptor.ValidTo,
                Role = user.Role
            };
        }
    }
}
