using System.ComponentModel.DataAnnotations;

namespace ProdApp.DTOS
{
    public class ProductImageDTO
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
