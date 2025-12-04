using Xunit;
using ProdApp.Services.Implementations;
using ProdApp.DTOS;
using ProdApp.Models;
using Microsoft.Extensions.Configuration;

namespace ProdApp.Tests
{
    public class AuthServiceTests
    {

        [Fact]
        public async Task Login_ShouldReturnNull_ForWrongPassword()
        {
            // Arrange
            var context = TestDbContextFactory.Create();

            context.Users.Add(new User
            {
                UserName = "admin",
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
                Role = "Admin",
                RegisteredAt = DateTime.UtcNow
            });
            context.SaveChanges();

            var settings = new Dictionary<string, string>
            {
                {"JwtConfig:Key", "secret123456789secret"},
                {"JwtConfig:Issuer", "http://localhost"},
                {"JwtConfig:Audience", "ProdApp"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var service = new AuthService(context, config);

            // Act
            var result = await service.LoginAsync(new LoginDTO
            {
                UserName = "admin",
                Password = "wrong"
            });

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task Login_ShouldReturnToken_ForValidCredentials()
        {
            // Arrange
            var context = TestDbContextFactory.Create();

            context.Users.Add(new User
            {
                UserName = "user",
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                Role = "User",
                RegisteredAt = DateTime.UtcNow
            });
            context.SaveChanges();

            var settings = new Dictionary<string, string>
            {
                {"JwtConfig:Key", "secret123456789secret1234567yydeudfddfdddd"},
                {"JwtConfig:Issuer", "http://localhost"},
                {"JwtConfig:Audience", "ProdApp"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var service = new AuthService(context, config);

            // Act
            var result = await service.LoginAsync(new LoginDTO
            {
                UserName = "user",
                Password = "pass"
            });

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
        }
    }
}
