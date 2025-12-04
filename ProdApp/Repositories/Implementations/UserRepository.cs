using Microsoft.EntityFrameworkCore;
using ProdApp.Data;
using ProdApp.Models;
using ProdApp.Repositories.Interfaces;

namespace ProdApp.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ProdDbContext context) : base(context) { }

        public async Task<User?> GetByNameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }
    }
}
