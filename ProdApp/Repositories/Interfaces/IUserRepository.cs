using ProdApp.Models;

namespace ProdApp.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByNameAsync(string username);
    }
}
