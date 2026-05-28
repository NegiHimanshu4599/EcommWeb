using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using BookService.Application.Services;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("soft-deleted")]
        public async Task<IActionResult> SoftDeleteBooks()
        {
            var getCategory = await _categoryService.GetAllSoftDeleteCategory();
            return Ok(getCategory);
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
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteBook(int id)
        {
            await _categoryService.PermanentDeleteAsync(id);
            return NoContent();
        }
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreAsync(int id)
        {
            await _categoryService.RestoreAsync(id);
            return NoContent();
        }
        [HttpGet("trash-dashboard")]
        public async Task<IActionResult> TrashDashboard()
        {
            var result = await _categoryService.GetTrashDashboardAsync();
            return Ok(result);
        }
        [HttpPost("trash")]
        public async Task<IActionResult>GetTrashCategories([FromBody] BookFilterDto dto)
        {
            dto.IsDeletedPage = true;
            var result = await _categoryService.GetTrashCategories(dto);
            return Ok(result);
        }
    }
}
