using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var getBooks = await _bookService.GetAllAsync();
            return Ok(getBooks);
        }
        [Authorize(Roles="Admin")]
        [HttpGet("soft-deleted")]
        public async Task<IActionResult> SoftDeleteBooks()
        {
            var getBooks = await _bookService.GetAllSoftDeleteBooks();
            return Ok(getBooks);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var getBook = await _bookService.GetByIdAsync(id);
            return Ok(getBook);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var created = await _bookService.AddAsync(dto);
            return CreatedAtAction(nameof(GetBookById), new { id = created.Id }, created);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromForm] UpdateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _bookService.UpdateAsync(dto);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteBook(int id)
        {
            await _bookService.PermanentDeleteAsync(id);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreAsync(int id)
        {
            await _bookService.RestoreAsync(id);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("filter")]
        public async Task<IActionResult> FilterBooks([FromBody] BookFilterDto dto)
        {
            var result = await _bookService.GetBookByFilter(dto);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("bulk")]
        public async Task<IActionResult> GetBooksByIds([FromBody] IEnumerable<int> ids)
        {
            try
            {
                var result = await _bookService.GetBulkBooks(ids);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());

            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("trash-dashboard")]
        public async Task<IActionResult> TrashDashboard()
        {
            var result = await _bookService.GetTrashDashboardAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("trash")]
        public async Task<IActionResult> GetTrashBooks([FromBody] BookFilterDto dto)
        {
            dto.IsDeletedPage = true;
            var result = await _bookService.GetTrashBooks(dto);
            return Ok(result);
        }
    }
}