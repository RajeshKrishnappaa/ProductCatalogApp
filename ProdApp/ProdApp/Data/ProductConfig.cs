using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdApp.Models;

namespace ProdApp.Data
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasColumnType("nvarchar(max)");
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.ImageUrl).HasMaxLength(500);

            builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Product
                {
                    ProductId=11,
                    ProductName="Nike Runnerr",
                    Description="Shoe from Nike.",
                    Price=2999m,
                    ImageUrl= "https://localhost:7189/images/nikerunner.jpg",
                    CategoryId=1
                },
                new Product
                {
                    ProductId=12,
                    ProductName="Puma Runnerr",
                    Description="Shoe from Puma",
                    Price=4999m,
                    ImageUrl="https://localhost:7189/images/pumarunner.jpg",
                    CategoryId=1
                });
        }
    }
}