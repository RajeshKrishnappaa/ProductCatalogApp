using AutoMapper;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using ProdApp.Repositories.Implementations;
using ProdApp.Services.Implementations;
using Xunit;

namespace ProdApp.Tests
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;

        public UserServiceTests()
        {
            _mapper = MapperHelper.GetMapper();
        }

        private UserService CreateService(ProdDbContext context)
        {
            var repo = new UserRepository(context);
            return new UserService(repo, context, _mapper);
        }

        [Fact]
        public async Task RegisterUser_ShouldCreateUser()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var dto = new RegisterDTO
            {
                UserName = "Rajesh",
                Email = "test@test.com",
                Password = "12345"
            };

            var result = await service.RegisterUserAsync(dto);

            Assert.Equal("Rajesh", result.UserName);
            Assert.Equal(1, context.Users.Count());
        }

        [Fact]
        public async Task GetByName_ShouldReturnNull_IfNotFound()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var result = await service.GetByNameAsync("Unknown");

            Assert.Null(result);
        }


        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenNotFound()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var deleted = await service.DeleteUserAsync(99);

            Assert.False(deleted);
        }
    }
}
