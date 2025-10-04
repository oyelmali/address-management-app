using Microsoft.AspNetCore.Mvc;
using AddressApp.Application.DTOs;
using AddressApp.Application.Services;

namespace AddressApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(IAddressService addressService, ILogger<AddressesController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
        {
            try
            {
                var addresses = await _addressService.GetAllAddressesAsync();
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses");
                return StatusCode(500, "An error occurred while fetching addresses");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDto>> GetById(int id)
        {
            try
            {
                var address = await _addressService.GetAddressByIdAsync(id);
                if (address == null)
                    return NotFound();
                    
                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address with id {Id}", id);
                return StatusCode(500, "An error occurred while fetching the address");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AddressDto>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(new List<AddressDto>());
                
            try
            {
                var addresses = await _addressService.SearchAsync(term);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching addresses with term {Term}", term);
                return StatusCode(500, "An error occurred while searching addresses");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AddressDto>> Create([FromBody] CreateAddressDto dto)
        {
            try
            {
                var created = await _addressService.CreateAddressAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address");
                return StatusCode(500, "An error occurred while creating the address");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressDto dto)
        {
            try
            {
                await _addressService.UpdateAddressAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address with id {Id}", id);
                return StatusCode(500, "An error occurred while updating the address");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _addressService.DeleteAddressAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address with id {Id}", id);
                return StatusCode(500, "An error occurred while deleting the address");
            }
        }

        [HttpPost("bulk-import")]
        public async Task<ActionResult<BulkImportResultDto>> BulkImport([FromBody] string[] textAddresses)
        {
            try
            {
                var result = await _addressService.BulkImportAsync(textAddresses);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk import");
                return StatusCode(500, "An error occurred during bulk import");
            }
        }
    }
}