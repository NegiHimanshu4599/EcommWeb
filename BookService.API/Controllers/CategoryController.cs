using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var getCategories = await _categoryService.GetAllAsync();
            return Ok(getCategories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var getCategory = await _categoryService.GetByIdAsync(id);
            return Ok(getCategory);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var created = await _categoryService.AddAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, created);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _categoryService.UpdateAsync(dto);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
