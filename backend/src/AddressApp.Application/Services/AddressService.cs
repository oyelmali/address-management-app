using AutoMapper;
using AddressApp.Application.DTOs;
using AddressApp.Domain.Entities;
using AddressApp.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace AddressApp.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync()
        {
            var addresses = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto?> GetAddressByIdAsync(int id)
        {
            var address = await _repository.GetByIdAsync(id);
            return address != null ? _mapper.Map<AddressDto>(address) : null;
        }

        public async Task<AddressDto> CreateAddressAsync(CreateAddressDto dto)
        {
            var address = _mapper.Map<Address>(dto);
            address.CreatedAt = DateTime.UtcNow;
            
            var created = await _repository.AddAsync(address);
            return _mapper.Map<AddressDto>(created);
        }

        public async Task UpdateAddressAsync(int id, UpdateAddressDto dto)
        {
            var address = await _repository.GetByIdAsync(id);
            if (address == null)
                throw new Exception($"Address with id {id} not found");

            _mapper.Map(dto, address);
            address.UpdatedAt = DateTime.UtcNow;
            
            await _repository.UpdateAsync(address);
        }

        public async Task DeleteAddressAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AddressDto>> SearchAsync(string term)
        {
            var addresses = await _repository.SearchAsync(term);
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<BulkImportResultDto> BulkImportAsync(string[] textAddresses)
        {
            var result = new BulkImportResultDto();
            var addressesToAdd = new List<Address>();

            foreach (var text in textAddresses)
            {
                try
                {
                    var address = ParseSingleAddress(text);
                    if (address != null)
                    {
                        addressesToAdd.Add(address);
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Failed to parse: {text}. Error: {ex.Message}");
                    result.FailureCount++;
                }
            }

            if (addressesToAdd.Any())
            {
                await _repository.BulkAddAsync(addressesToAdd);
                result.SuccessCount = addressesToAdd.Count;
            }

            return result;
        }

        private Address? ParseSingleAddress(string line)
        {
            try
            {
                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                
                if (parts.Length < 3)
                    return null;

                var address = new Address
                {
                    Region = ExtractRegion(parts[0]),
                    City = ExtractCity(parts[1]),
                    CreatedAt = DateTime.UtcNow
                };

                // Extract branch type and number
                var branchMatch = Regex.Match(line, @"(Відділення|Поштомат)\s*№?(\d+)");
                if (branchMatch.Success)
                {
                    address.BranchType = branchMatch.Groups[1].Value;
                    address.BranchNumber = branchMatch.Groups[2].Value;
                }

                // Extract street address
                var addressMatch = Regex.Match(line, @":\s*([^,]+),");
                if (addressMatch.Success)
                {
                    address.Street = addressMatch.Groups[1].Value.Trim();
                }

                // Extract phone
                var phoneMatch = Regex.Match(line, @"\+?\d{10,}");
                if (phoneMatch.Success)
                {
                    address.Phone = phoneMatch.Value;
                }

                // Extract working hours
                var hoursMatches = Regex.Matches(line, @"(пн-[а-я]{2}|[а-я]{2}-[а-я]{2})\s+\d{2}:\d{2}-\d{2}:\d{2}");
                if (hoursMatches.Count > 0)
                {
                    address.WorkingHoursWeekdays = hoursMatches[0].Value;
                    if (hoursMatches.Count > 1)
                    {
                        address.WorkingHoursWeekend = hoursMatches[1].Value;
                    }
                }

                return address;
            }
            catch
            {
                return null;
            }
        }

        private string ExtractRegion(string text)
        {
            return text.Replace("обл.", "").Trim();
        }

        private string ExtractCity(string text)
        {
            return text.Trim();
        }
    }
}