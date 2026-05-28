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
        [HttpGet("soft-deleted")]
        public async Task<IActionResult> SoftDeleteBooks()
        {
            var getCategory = await _coverTypeService.GetAllSoftDeleteCoverType();
            return Ok(getCategory);
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
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteBook(int id)
        {
            await _coverTypeService.PermanentDeleteAsync(id);
            return NoContent();
        }
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreAsync(int id)
        {
            await _coverTypeService.RestoreAsync(id);
            return NoContent();
        }
        [HttpGet("trash-dashboard")]
        public async Task<IActionResult> TrashDashboard()
        {
            var result = await _coverTypeService.GetTrashDashboardAsync();
            return Ok(result);
        }
        [HttpPost("trash")]
        public async Task<IActionResult> GetTrashCoverTypes([FromBody] BookFilterDto dto)
        {
            dto.IsDeletedPage = true;
            var result = await _coverTypeService.GetTrashCoverTypes(dto);
            return Ok(result);
        }
    }
}