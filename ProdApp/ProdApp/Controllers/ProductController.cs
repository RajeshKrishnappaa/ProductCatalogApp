using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;
using System.Collections.Generic;

namespace ProdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProdDbContext _context;
        private readonly IMapper _mapper;
        public ProductController(ProdDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

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
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

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
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dto = _mapper.Map<IEnumerable<ProductDTO>>(items);

            return Ok(new
            {
                total,
                items = dto
            });
        }


        [HttpGet]
        [Route("{id:int}", Name = "GetProductById")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProduct(int id)
        {

            if (id <= 0)
                return BadRequest("Invalid Product Id");

            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound($"Product with Id {id} Not Found");

            var dto = _mapper.Map<ProductDTO>(product);

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Add", Name = "AddProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] ProductDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid Data");

            var product = _mapper.Map<Product>(dto);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var createdDto = _mapper.Map<ProductDTO>(product);

            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, createdDto);
        }

        [HttpPost("UploadImage")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage([FromForm] ProductImageDTO dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                return BadRequest("Image is required.");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";

            var product = _mapper.Map<Product>(dto);
            product.ImageUrl = imageUrl;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var createdDto = _mapper.Map<ProductDTO>(product);

            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, createdDto);
        }


        [HttpPut]
            [Authorize(Roles = "Admin")]
            [Route("Update/{id:int}", Name = "UpdateProduct")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductImageDTO dto)
            {
            ModelState.Remove("ImageFile");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound($"Product with Id {id} not found");

                product.ProductName = dto.ProductName;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.CategoryId = dto.CategoryId;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = $"/images/{fileName}";
            }
            await _context.SaveChangesAsync();

            var updatedDto = _mapper.Map<ProductDTO>(product);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            if (!string.IsNullOrEmpty(updatedDto.ImageUrl) && !updatedDto.ImageUrl.StartsWith("http"))
            {
                updatedDto.ImageUrl = baseUrl + updatedDto.ImageUrl;
            }
            return Ok(updatedDto);
            }
        

            [HttpDelete]
            [Route("Delete/{id:int}", Name = "DeleteProduct")]
            [Authorize(Roles = "Admin")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult> DeleteProduct(int id)
            {
                if (id <= 0)
                    return BadRequest();
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound($"Product with Id{id} Not Found");

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(product);
            }
        }
    }
