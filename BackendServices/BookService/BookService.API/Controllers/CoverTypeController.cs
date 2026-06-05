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
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCoverTypes()
        {
            var getCoverTypes = await _coverTypeService.GetAllAsync();
            return Ok(getCoverTypes);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("soft-deleted")]
        public async Task<IActionResult> SoftDeleteBooks()
        {
            var getCategory = await _coverTypeService.GetAllSoftDeleteCoverType();
            return Ok(getCategory);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoverTypeById(int id)
        {
            var getCoverTypeById = await _coverTypeService.GetByIdAsync(id);
            return Ok(getCoverTypeById);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCoverType([FromBody] CoverTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var created = await _coverTypeService.AddAsync(dto);
            return CreatedAtAction(nameof(GetCoverTypeById), new { id = created.Id }, created);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateCoverType([FromBody] CoverTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _coverTypeService.UpdateAsync(dto);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoverType(int id)
        {
            await _coverTypeService.DeleteAsync(id);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteBook(int id)
        {
            await _coverTypeService.PermanentDeleteAsync(id);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreAsync(int id)
        {
            await _coverTypeService.RestoreAsync(id);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("trash-dashboard")]
        public async Task<IActionResult> TrashDashboard()
        {
            var result = await _coverTypeService.GetTrashDashboardAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("trash")]
        public async Task<IActionResult> GetTrashCoverTypes([FromBody] BookFilterDto dto)
        {
            dto.IsDeletedPage = true;
            var result = await _coverTypeService.GetTrashCoverTypes(dto);
            return Ok(result);
        }
    }
}