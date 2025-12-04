using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProdApp.DTOS;
using ProdApp.Models;
using ProdApp.Services.Implementations;
using ProdApp.Services.Interfaces;
using Xunit;

namespace ProdApp.Tests
{
    public class ProductServiceTests
    {
        private readonly IMapper _mapper;

        public ProductServiceTests()
        {
            // AutoMapper is required because ProductService uses mapping internally.
            _mapper = MapperHelper.GetMapper();
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnCorrectTotal()
        {
            // Arrange
            //Using InMemoryDatabase (unique DB per test) to simulate SQL operations
            var context = TestDbContextFactory.Create();

            // Add a required category
            context.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "Shoes"
            });

            // Add two valid Product records
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
            // Save to the InMemory database
            context.SaveChanges();
            // HttpContextAccessor is needed in ProductService for generating image URLs
            var http = new HttpContextAccessor();
            // Create the service instance under test
            var service = new ProductService(context, _mapper, http);

            // Act
            var (total, items) = await service.GetAllProductsAsync(1, 5, null, null, null, null);

            // Assert
            Assert.Equal(2, total); // Total should be 2
            Assert.Equal(2, items.Count());// Items returned should be 2
        }


        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_IfNotFound()
        {
            var context = TestDbContextFactory.Create();
            var http = new HttpContextAccessor();

            var service = new ProductService(context, _mapper, http);
            // Trying to fetch a product that does not exist
            var product = await service.GetProductByIdAsync(10);

            Assert.Null(product); // Expected: NULL
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreate()
        {
            var context = TestDbContextFactory.Create();
            var http = new HttpContextAccessor();
            var service = new ProductService(context, _mapper, http);
            // DTO representing product input from frontend
            var dto = new ProductDTO
            {
                ProductName = "Shoe",
                Description = "Test Product",
                ImageUrl = "/test.jpg",
                Price = 1500,
                CategoryId = 1
            };

            var result = await service.CreateProductAsync(dto);

            Assert.Equal("Shoe", result.ProductName);// Check returned object
            Assert.Equal(1, context.Products.Count());// Confirm it was added to DB
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenNotFound()
        {
            var context = TestDbContextFactory.Create();
            var http = new HttpContextAccessor();
            var service = new ProductService(context, _mapper, http);
            // Trying to delete non-existing product
            var result = await service.DeleteProductAsync(100);

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldFilter_BySearchKeyword()
        {
            // Arrange
            var context = TestDbContextFactory.Create();

            // Required category
            context.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "Shoes"
            });

            // Add products
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

            var http = new HttpContextAccessor();
            var service = new ProductService(context, _mapper, http);

            // Act — search for keyword "Nike"
            var (total, items) = await service.GetAllProductsAsync(
                page: 1,
                pageSize: 10,
                q: "Nike",
                categoryId: null,
                minPrice: null,
                maxPrice: null
            );

            // Assert
            Assert.Equal(1, total);
            Assert.Single(items);
            Assert.Equal("Nike Runner", items.First().ProductName);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldFilter_ByPriceRange()
        {
            // Arrange
            var context = TestDbContextFactory.Create();

            context.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "Shoes"
            });

            context.Products.Add(new Product
            {
                ProductName = "Cheap Shoe",
                Description = "Budget item",
                ImageUrl = "/test.jpg",
                Price = 500,
                CategoryId = 1
            });

            context.Products.Add(new Product
            {
                ProductName = "Mid Shoe",
                Description = "Mid budget",
                ImageUrl = "/test.jpg",
                Price = 1500,
                CategoryId = 1
            });

            context.Products.Add(new Product
            {
                ProductName = "Expensive Shoe",
                Description = "Premium",
                ImageUrl = "/test.jpg",
                Price = 3000,
                CategoryId = 1
            });

            context.SaveChanges();

            var http = new HttpContextAccessor();
            var service = new ProductService(context, _mapper, http);

            // Act — filter price between 1000 and 2000
            var (total, items) = await service.GetAllProductsAsync(
                page: 1,
                pageSize: 10,
                q: null,
                categoryId: null,
                minPrice: 1000,
                maxPrice: 2000
            );

            // Assert — Expect only "Mid Shoe"
            Assert.Equal(1, total);
            Assert.Single(items);
            Assert.Equal("Mid Shoe", items.First().ProductName);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldFilter_ByCategory()
        {
            // Arrange
            var context = TestDbContextFactory.Create();

            // Add categories
            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Shoes" });
            context.Categories.Add(new Category { CategoryId = 2, CategoryName = "Clothes" });

            // Products
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

            var http = new HttpContextAccessor();
            var service = new ProductService(context, _mapper, http);

            // Act — filter based on Shoes (CategoryId = 1)
            var (total, items) = await service.GetAllProductsAsync(
                page: 1,
                pageSize: 10,
                q: null,
                categoryId: 1,
                minPrice: null,
                maxPrice: null
            );

            // Assert
            Assert.Equal(1, total);
            Assert.Single(items);
            Assert.Equal("Nike Shoe", items.First().ProductName);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldApplyPaginationCorrectly()
        {
            // Arrange
            var context = TestDbContextFactory.Create();

            // Required category for FK
            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Shoes" });

            // Add 10 products
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

            var http = new HttpContextAccessor();
            var service = new ProductService(context, _mapper, http);

            // Act — Get page 2 with pageSize = 3
            var (total, items) = await service.GetAllProductsAsync(
                page: 2,
                pageSize: 3,
                q: null,
                categoryId: null,
                minPrice: null,
                maxPrice: null
            );

            // Assert
            Assert.Equal(10, total);      // total products = 10
            Assert.Equal(3, items.Count()); // page 2 should contain 3 items

            // Make sure correct items appear in page 2
            Assert.Equal("Product 3", items.First().ProductName);

        }


    }
}
