using ProdApp.DTOS;

namespace ProdApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<(int total, IEnumerable<ProductDTO> items)> GetAllProductsAsync(
            int page, int pageSize, string? q,
            int? categoryId, decimal? minPrice, decimal? maxPrice);

        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(ProductDTO dto);
        Task<ProductDTO> UploadImageAsync(ProductImageDTO dto);
        Task<ProductDTO?> UpdateProductAsync(int id, ProductImageDTO dto);
        Task<bool> DeleteProductAsync(int id);
    }
}
