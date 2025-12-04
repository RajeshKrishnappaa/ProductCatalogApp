using Microsoft.EntityFrameworkCore;
using ProdApp.Data;
using ProdApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ProdApp.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ProdDbContext _context;
        protected readonly DbSet<T> _db;

        public Repository(ProdDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _db.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _db;

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _db.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }

        public void Remove(T entity)
        {
            _db.Remove(entity);
        }
    }
}
