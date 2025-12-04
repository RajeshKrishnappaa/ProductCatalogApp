using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdApp.DTOS;
using ProdApp.Services.Interfaces;

namespace ProdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;

        public ProductController(IProductService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: api/Product/All
        [HttpGet("All")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts(
            int page = 1,
            int pageSize = 5,
            string? q = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var (total, items) = await _service.GetAllProductsAsync(
                page, pageSize, q, categoryId, minPrice, maxPrice);

            return Ok(new { total, items });
        }

        // GET: api/Product/5
        [HttpGet("{id:int}", Name = "GetProductById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _service.GetProductByIdAsync(id);

            if (product == null)
                return NotFound($"Product with ID {id} not found");

            return Ok(product);
        }

        // POST: api/Product/Add
        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid Data");

            var created = await _service.CreateProductAsync(dto);

            return CreatedAtRoute("GetProductById",
                new { id = created.ProductId },
                created);
        }

        // POST: api/Product/UploadImage
        [HttpPost("UploadImage")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage([FromForm] ProductImageDTO dto)
        {
            var created = await _service.UploadImageAsync(dto);

            return CreatedAtRoute("GetProductById",
                new { id = created.ProductId },
                created);
        }

        // PUT: api/Product/Update/5
        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductImageDTO dto)
        {
            var updated = await _service.UpdateProductAsync(id, dto);

            if (updated == null)
                return NotFound("Product not found");

            return Ok(updated);
        }

        // DELETE: api/Product/Delete/5
        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _service.DeleteProductAsync(id);

            if (!deleted)
                return NotFound("No product found");

            return Ok("Product deleted successfully");
        }
    }
}
