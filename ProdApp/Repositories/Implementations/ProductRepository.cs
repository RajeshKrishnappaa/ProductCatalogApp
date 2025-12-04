using Microsoft.EntityFrameworkCore;
using ProdApp.Data;
using ProdApp.Models;
using ProdApp.Repositories.Interfaces;

namespace ProdApp.Repositories.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ProdDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
    }
}
