using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using ProdApp.Repositories.Implementations;
using ProdApp.Services.Implementations;
using Xunit;

namespace ProdApp.Tests
{
    public class ProductServiceTests
    {
        private readonly IMapper _mapper;

        public ProductServiceTests()
        {
            // AutoMapper instance for test
            _mapper = MapperHelper.GetMapper();
        }

        // =====================================================
        // Helper to create service instance with repository
        // =====================================================
        private ProductService CreateService(ProdDbContext context)
        {
            var repo = new ProductRepository(context);
            var http = new HttpContextAccessor();
            return new ProductService(repo, context, _mapper, http);
        }

        // =====================================================
        // TEST 1 — Basic GetAll Products
        // =====================================================
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnCorrectTotal()
        {
            var context = TestDbContextFactory.Create();

            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Shoes" });

            context.Products.Add(new Product
            {
                ProductName = "Nike",
                Description = "Test",
                ImageUrl = "/test.jpg",
                Price = 1000,
                CategoryId = 1
            });

            context.Products.Add(new Product
            {
                ProductName = "Puma",
                Description = "Test",
                ImageUrl = "/test.jpg",
                Price = 2000,
                CategoryId = 1
            });

            context.SaveChanges();

            var service = CreateService(context);

            var (total, items) = await service.GetAllProductsAsync(1, 5, null, null, null, null);

            Assert.Equal(2, total);
            Assert.Equal(2, items.Count());
        }

        // =====================================================
        // TEST 2 — GetProductById returns NULL
        // =====================================================
        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_IfNotFound()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var product = await service.GetProductByIdAsync(10);

            Assert.Null(product);
        }

        // =====================================================
        // TEST 3 — Create new Product
        // =====================================================
        [Fact]
        public async Task CreateProductAsync_ShouldCreate()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var dto = new ProductDTO
            {
                ProductName = "Shoe",
                Description = "Test Product",
                ImageUrl = "/test.jpg",
                Price = 1500,
                CategoryId = 1
            };

            var created = await service.CreateProductAsync(dto);

            Assert.Equal("Shoe", created.ProductName);
            Assert.Equal(1, context.Products.Count());
        }

        // =====================================================
        // TEST 4 — Delete non-existing product
        // =====================================================
        [Fact]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenNotFound()
        {
            var context = TestDbContextFactory.Create();
            var service = CreateService(context);

            var result = await service.DeleteProductAsync(100);

            Assert.False(result);
        }

        // =====================================================
        // TEST 5 — Search keyword filter
        // =====================================================
        [Fact]
        public async Task GetAllProductsAsync_ShouldFilter_BySearchKeyword()
        {
            var context = TestDbContextFactory.Create();

            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Shoes" });

            context.Products.Add(new Product
            {
                ProductName = "Nike Runner",
                Description = "Running shoes",
                ImageUrl = "/test.jpg",
                Price = 2000,
                CategoryId = 1
            });

            context.Products.Add(new Product
            {
                ProductName = "Puma Casual",
                Description = "Casual shoes",
                ImageUrl = "/test.jpg",
                Price = 3000,
                CategoryId = 1
            });

            context.SaveChanges();

            var service = CreateService(context);

            var (total, items) = await service.GetAllProductsAsync(
                1, 10, "Nike", null, null, null
            );

            Assert.Equal(1, total);
            Assert.Single(items);
            Assert.Equal("Nike Runner", items.First().ProductName);
        }

        // =====================================================
        // TEST 6 — Price range filter
        // =====================================================
        [Fact]
        public async Task GetAllProductsAsync_ShouldFilter_ByPriceRange()
        {
            var context = TestDbContextFactory.Create();

            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Shoes" });

            context.Products.Add(new Product
            {
                ProductName = "Cheap Shoe",
                Description = "Budget",
                ImageUrl = "/test.jpg",
                Price = 500,
                CategoryId = 1
            });

            context.Products.Add(new Product
            {
                ProductName = "Mid Shoe",
                Description = "Mid range",
                ImageUrl = "/test.jpg",
                Price = 1500,
                CategoryId = 1
            });

            context.Products.Add(new Product
            {
                ProductName = "Exp Shoe",
                Description = "Expensive",
                ImageUrl = "/test.jpg",
                Price = 3000,
                CategoryId = 1
            });

            context.SaveChanges();

            var service = CreateService(context);

            var (total, items) = await service.GetAllProductsAsync(
                1, 10, null, null, 1000, 2000
            );

            Assert.Equal(1, total);
            Assert.Equal("Mid Shoe", items.First().ProductName);
        }

        // =====================================================
        // TEST 7 — Category filter
        // =====================================================
        [Fact]
        public async Task GetAllProductsAsync_ShouldFilter_ByCategory()
        {
            var context = TestDbContextFactory.Create();

            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Shoes" });
            context.Categories.Add(new Category { CategoryId = 2, CategoryName = "Clothes" });

            context.Products.Add(new Product
            {
                ProductName = "Nike Shoe",
                Description = "Footwear",
                ImageUrl = "/test.jpg",
                Price = 2000,
                CategoryId = 1
            });

            context.Products.Add(new Product
            {
                ProductName = "Puma Shirt",
                Description = "Clothing",
                ImageUrl = "/test.jpg",
                Price = 1500,
                CategoryId = 2
            });

            context.SaveChanges();

            var service = CreateService(context);

            var (total, items) = await service.GetAllProductsAsync(
                1, 10, null, 1, null, null
            );

            Assert.Equal(1, total);
            Assert.Equal("Nike Shoe", items.First().ProductName);
        }

        // =====================================================
        // TEST 8 — Pagination
        // =====================================================
        [Fact]
        public async Task GetAllProductsAsync_ShouldApplyPaginationCorrectly()
        {
            var context = TestDbContextFactory.Create();

            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Shoes" });

            for (int i = 1; i <= 10; i++)
            {
                context.Products.Add(new Product
                {
                    ProductName = "Product " + i,
                    Description = "Test",
                    ImageUrl = "/test.jpg",
                    Price = 1000 + i,
                    CategoryId = 1
                });
            }

            context.SaveChanges();

            var service = CreateService(context);

            var (total, items) = await service.GetAllProductsAsync(
                2, 3, null, null, null, null
            );

            Assert.Equal(10, total);
            Assert.Equal(3, items.Count());
            Assert.Equal("Product 3", items.First().ProductName);
        }
    }
}
