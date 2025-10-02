using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AddressApp.Application.DTOs;
using AddressApp.Application.Interfaces;
using AddressApp.Core.Entities;
using AddressApp.Core.Interfaces;

namespace AddressApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly IAddressRepository _repository;
    private readonly IAddressParserService _parserService;
    private readonly IMapper _mapper;
    private readonly ILogger<AddressesController> _logger;

    public AddressesController(
        IAddressRepository repository,
        IAddressParserService parserService,
        IMapper mapper,
        ILogger<AddressesController> logger)
    {
        _repository = repository;
        _parserService = parserService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all addresses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
    {
        var addresses = await _repository.GetAllAsync();
        var addressDtos = _mapper.Map<IEnumerable<AddressDto>>(addresses);
        return Ok(addressDtos);
    }

    /// <summary>
    /// Get address by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressDto>> GetById(int id)
    {
        var address = await _repository.GetByIdAsync(id);
        if (address == null)
            return NotFound(new { message = $"Address with ID {id} not found" });

        var addressDto = _mapper.Map<AddressDto>(address);
        return Ok(addressDto);
    }

    /// <summary>
    /// Create new address
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AddressDto>> Create([FromBody] CreateAddressDto dto)
    {
        var address = _mapper.Map<Address>(dto);
        var createdAddress = await _repository.AddAsync(address);
        var addressDto = _mapper.Map<AddressDto>(createdAddress);

        return CreatedAtAction(nameof(GetById), new { id = addressDto.Id }, addressDto);
    }

    /// <summary>
    /// Update existing address
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressDto dto)
    {
        var existingAddress = await _repository.GetByIdAsync(id);
        if (existingAddress == null)
            return NotFound(new { message = $"Address with ID {id} not found" });

        _mapper.Map(dto, existingAddress);
        existingAddress.Id = id; // Ensure ID doesn't change
        await _repository.UpdateAsync(existingAddress);

        return NoContent();
    }

    /// <summary>
    /// Delete address
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var address = await _repository.GetByIdAsync(id);
        if (address == null)
            return NotFound(new { message = $"Address with ID {id} not found" });

        await _repository.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Search addresses by term
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AddressDto>>> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return await GetAll();

        var addresses = await _repository.SearchAsync(term);
        var addressDtos = _mapper.Map<IEnumerable<AddressDto>>(addresses);
        return Ok(addressDtos);
    }

    /// <summary>
    /// Bulk import addresses from text
    /// </summary>
    [HttpPost("bulk-import")]
    [ProducesResponseType(typeof(BulkImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BulkImportResultDto>> BulkImport([FromBody] List<string> textAddresses)
    {
        if (textAddresses == null || !textAddresses.Any())
            return BadRequest(new { message = "No addresses provided" });

        var result = new BulkImportResultDto();
        var addressesToAdd = new List<Address>();

        foreach (var text in textAddresses)
        {
            try
            {
                var parsedAddress = _parserService.ParseFromText(text);
                if (parsedAddress != null)
                {
                    addressesToAdd.Add(parsedAddress);
                    result.SuccessCount++;
                }
                else
                {
                    result.FailureCount++;
                    result.Errors.Add($"Failed to parse: {text.Substring(0, Math.Min(50, text.Length))}...");
                }
            }
            catch (Exception ex)
            {
                result.FailureCount++;
                result.Errors.Add($"Error parsing: {ex.Message}");
                _logger.LogError(ex, "Error parsing address text");
            }
        }

        if (addressesToAdd.Any())
        {
            await _repository.BulkAddAsync(addressesToAdd);
        }

        return Ok(result);
    }
    
}