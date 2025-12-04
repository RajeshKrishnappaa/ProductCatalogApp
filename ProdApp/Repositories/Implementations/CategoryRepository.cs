using ProdApp.Data;
using ProdApp.Models;
using ProdApp.Repositories.Interfaces;

namespace ProdApp.Repositories.Implementations
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ProdDbContext context) : base(context) { }
    }
}
