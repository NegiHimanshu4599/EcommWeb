using BookService.Application.DTOs;
using BookService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverTypeController : ControllerBase
    {
        private readonly ICoverTypeService _coverTypeService;
        public CoverTypeController(ICoverTypeService coverTypeService)
        {
            _coverTypeService = coverTypeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCoverTypes()
        {
            var getCoverTypes = await _coverTypeService.GetAllAsync();
            return Ok(getCoverTypes);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoverTypeById(int id)
        {
            var getCoverTypeById = await _coverTypeService.GetByIdAsync(id);
            return Ok(getCoverTypeById);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCoverType([FromBody] CoverTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var created = await _coverTypeService.AddAsync(dto);
            return CreatedAtAction(nameof(GetCoverTypeById), new { id = created.Id }, created);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCoverType([FromBody] CoverTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _coverTypeService.UpdateAsync(dto);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoverType(int id)
        {
            await _coverTypeService.DeleteAsync(id);
            return NoContent();
        }
    }
}