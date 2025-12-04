using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using ProdApp.Services.Interfaces;

namespace ProdApp.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ProdDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public ProductService(ProdDbContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<(int total, IEnumerable<ProductDTO> items)> GetAllProductsAsync(
            int page, int pageSize, string? q,
            int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.ProductName.Contains(q) || p.Description.Contains(q));

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dto = _mapper.Map<IEnumerable<ProductDTO>>(items);

            return (total, dto);
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateProductAsync(ProductDTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> UploadImageAsync(ProductImageDTO dto)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            var req = _httpContext.HttpContext.Request;
            var imageUrl = $"{req.Scheme}://{req.Host}/images/{fileName}";

            var product = _mapper.Map<Product>(dto);
            product.ImageUrl = imageUrl;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO?> UpdateProductAsync(int id, ProductImageDTO dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            product.ProductName = dto.ProductName;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;

            if (dto.ImageFile != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = $"/images/{fileName}";
            }

            await _context.SaveChangesAsync();

            var dtoMapped = _mapper.Map<ProductDTO>(product);
            var req = _httpContext.HttpContext.Request;

            if (!dtoMapped.ImageUrl.StartsWith("http"))
                dtoMapped.ImageUrl = $"{req.Scheme}://{req.Host}{dtoMapped.ImageUrl}";

            return dtoMapped;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
