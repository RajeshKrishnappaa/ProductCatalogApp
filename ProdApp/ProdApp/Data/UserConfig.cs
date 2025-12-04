using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdApp.Models;

namespace ProdApp.Data
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId).ValueGeneratedOnAdd();
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Role).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            
        }
    }
}