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
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var getBooks = await _bookService.GetAllAsync();
            return Ok(getBooks);
        }
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
        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var created = await _bookService.AddAsync(dto);
            return CreatedAtAction(nameof(GetBookById), new { id = created.Id }, created);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromForm] UpdateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _bookService.UpdateAsync(dto);
            return NoContent();
        }   
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteBook(int id)
        {
            await _bookService.PermanentDeleteAsync(id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreAsync(int id)
        {
            await _bookService.RestoreAsync(id);
            return NoContent();
        }
        [HttpPost("filter")]
        public async Task<IActionResult> FilterBooks([FromBody] BookFilterDto dto)
        {
            var result = await _bookService.GetBookByFilter(dto);
            return Ok(result);
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> GetBooksByIds([FromBody] List<int> ids)
        {
            var result = await _bookService.GetBulkBooks(ids);
            return Ok(result);
        }
        [HttpGet("trash-dashboard")]
        public async Task<IActionResult> TrashDashboard()
        {
            var result = await _bookService.GetTrashDashboardAsync();
            return Ok(result);
        }
        [HttpPost("trash")]
        public async Task<IActionResult> GetTrashBooks([FromBody] BookFilterDto dto)
        {
            dto.IsDeletedPage = true;

            var result = await _bookService.GetTrashBooks(dto);

            return Ok(result);
        }
    }
}