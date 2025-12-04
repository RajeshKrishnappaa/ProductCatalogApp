using AutoMapper;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using ProdApp.Repositories.Implementations;
using ProdApp.Services.Implementations;
using Xunit;

namespace ProdApp.Tests
{
    public class CategoryServiceTests
    {
        private readonly IMapper _mapper;

        public CategoryServiceTests()
        {
            _mapper = MapperHelper.GetMapper();
        }

        private CategoryService CreateService(ProdDbContext context)
        {
            var repo = new CategoryRepository(context);
            return new CategoryService(repo, context, _mapper);
        }

        [Fact]
        public async Task CreateCategory_ShouldWork()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var dto = new CategoryDTO { CategoryName = "Shoes" };

            var result = await service.CreateAsync(dto);

            Assert.Equal("Shoes", result.CategoryName);
            Assert.Equal(1, context.Categories.Count());
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmpty_WhenNoData()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var result = await service.GetAllAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnFalse_WhenNotFound()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var result = await service.DeleteAsync(50);

            Assert.False(result);
        }
    }
}
