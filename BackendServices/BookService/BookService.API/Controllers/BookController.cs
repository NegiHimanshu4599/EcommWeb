using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost("filter")]
        public async Task<IActionResult> FilterBooks([FromBody] BookFilterDto dto)
        {
            var result = await _bookService.GetBookByFilter(dto);
            return Ok(result);
        }
    }
}
