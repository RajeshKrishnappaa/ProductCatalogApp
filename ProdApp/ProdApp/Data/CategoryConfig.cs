using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdApp.Models;

namespace ProdApp.Data
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryId);
            builder.Property(c => c.CategoryName).IsRequired().HasMaxLength(255);

            builder.HasData(
                new Category
                {
                   CategoryId = 1,
                    CategoryName = "Shoes"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Slipper"
                });
        }
    }
}