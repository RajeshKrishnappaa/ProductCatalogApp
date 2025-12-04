using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdApp.Data;
using ProdApp.DTOS;
using ProdApp.Models;

namespace ProdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ProdDbContext _context;
        private readonly IMapper _mapper;
        public CategoryController(ProdDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("All", Name = "GetAllCategories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllcategories()
        {
            var cat = await _context.Categories.ToListAsync();

            if (cat == null || cat.Count == 0)
                return NotFound("No categories found");

            var dto = _mapper.Map<IEnumerable<CategoryDTO>>(cat);

            return Ok(dto);
        }

        [HttpGet]
        [Route("{id:int}",Name = "GetCategoryById")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategory(int id)
        {
            if (id <= 0)
                return BadRequest();
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null)
                return NotFound();

            return Ok(cat);
        }

        [HttpPost]
        [Route("Create",Name ="AddCategory")]
        [Authorize(Roles ="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CategoryDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid Category");

            bool exists = await _context.Categories.AnyAsync(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower());
            if (exists)
                return BadRequest($"Category '{dto.CategoryName}' already exists");

            var cat = new Category { CategoryName = dto.CategoryName };
            await _context.Categories.AddAsync(cat);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetCategoryById", new { id = cat.CategoryId }, cat);
        }

        [HttpPut]
        [Route("Update/{id:int}", Name = "UpdateCategory")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] CategoryDTO updateddto)
        {
            if (id <= 0)
                return BadRequest();
            var existingCat = await _context.Categories.FindAsync(id);
            if (existingCat == null)
                return NotFound();

            existingCat.CategoryName = updateddto.CategoryName;
            _context.Categories.Update(existingCat);
            await _context.SaveChangesAsync();

            return Ok(existingCat);
        }

        [HttpDelete]
        [Route("Delete",Name ="DeleteCategory")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest();
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null)
                return NotFound();

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();

            return Ok(cat);
        }
    }
}
