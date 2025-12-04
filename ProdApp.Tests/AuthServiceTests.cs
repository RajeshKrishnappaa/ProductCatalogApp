using Microsoft.Extensions.Configuration;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using ProdApp.Repositories.Implementations;
using ProdApp.Services.Implementations;
using Xunit;

namespace ProdApp.Tests
{
    public class AuthServiceTests
    {
        private AuthService CreateService(ProdDbContext context, IConfiguration config)
        {
            var repo = new UserRepository(context);
            return new AuthService(repo, config);
        }

        [Fact]
        public async Task Login_ShouldReturnNull_ForWrongPassword()
        {
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
                {"JwtConfig:Key", "mysuperstrongsecretkey1234567890ABC!@"},
                {"JwtConfig:Issuer", "http://localhost"},
                {"JwtConfig:Audience", "ProdApp"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var service = CreateService(context, config);

            var result = await service.LoginAsync(new LoginDTO
            {
                UserName = "admin",
                Password = "wrong"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_ForValidCredentials()
        {
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
                {"JwtConfig:Key", "THIS_IS_A_VERY_LONG_TEST_KEY_1234567890_ABCDE"},
                {"JwtConfig:Issuer", "http://localhost"},
                {"JwtConfig:Audience", "ProdApp"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var service = CreateService(context, config);

            var result = await service.LoginAsync(new LoginDTO
            {
                UserName = "user",
                Password = "pass"
            });

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
        }
    }
}
