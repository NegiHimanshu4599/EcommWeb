using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAddressesAsync()
        {
            var user = GetUserId();
            var getAddresses = await _addressService.GetAddresses(user);
            if (!getAddresses.Any())
                return NotFound("Empty List");
            return Ok(getAddresses);
        }
        [HttpPost]
        public async Task<IActionResult> SaveAddressAsync([FromBody] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = GetUserId();
            var created = await _addressService.SaveAddress(user, dto);
            return CreatedAtAction(nameof(GetAllAddressesAsync), new { id = created.Id }, created);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody]UpdateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = GetUserId();
            await _addressService.UpdateAddress(user, dto);
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var user = GetUserId();
            await _addressService.DeleteAddress(user, id);
            return NoContent();
        }
        [HttpGet("default")]
        public async Task<IActionResult> DefaultAddress()
        {
            var user = GetUserId();
            var result = await _addressService.GetDefaultAddress(user);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new KeyNotFoundException("UserId Not Found");
            return userId;
        }
    }
}
