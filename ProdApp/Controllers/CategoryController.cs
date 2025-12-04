using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdApp.DTOS;
using ProdApp.Services.Interfaces;

namespace ProdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: api/Category/All
        [HttpGet("All")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _service.GetAllAsync();

            if (!categories.Any())
                return NotFound("No categories found");

            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id:int}", Name = "GetCategoryById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID");

            var category = await _service.GetByIdAsync(id);

            if (category == null)
                return NotFound("Category not found");

            return Ok(category);
        }

        // POST: api/Category/Create
        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CategoryDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            var created = await _service.CreateAsync(dto);

            return CreatedAtRoute("GetCategoryById", new { id = created.CategoryId }, created);
        }

        // PUT: api/Category/Update/5
        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDTO dto)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var updated = await _service.UpdateAsync(id, dto);

            if (updated == null)
                return NotFound("Category not found");

            return Ok(updated);
        }

        // DELETE: api/Category/Delete?id=5
        [HttpDelete("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID");

            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Category not found");

            return Ok("Category deleted successfully");
        }
    }
}
