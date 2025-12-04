using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using ProdApp.Services.Interfaces;

namespace ProdApp.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ProdDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(ProdDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateAsync(CategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO?> UpdateAsync(int id, CategoryDTO dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            category.CategoryName = dto.CategoryName;

            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
