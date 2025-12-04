using AutoMapper;
using ProdApp.DTOS;
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

        [Fact]
        public async Task RegisterUser_ShouldCreateUser()
        {
            var context = TestDbContextFactory.Create();
            var service = new UserService(context, _mapper);

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
        public async Task GetUserByName_ShouldReturnNull_IfNotFound()
        {
            var context = TestDbContextFactory.Create();
            var service = new UserService(context, _mapper);

            var result = await service.GetUserByNameAsync("Unknown");

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenNotFound()
        {
            var context = TestDbContextFactory.Create();
            var service = new UserService(context, _mapper);

            var deleted = await service.DeleteUserAsync(99);

            Assert.False(deleted);
        }
    }
}
