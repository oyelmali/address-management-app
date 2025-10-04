using AddressApp.Application.DTOs;

namespace AddressApp.Application.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
        Task<AddressDto?> GetAddressByIdAsync(int id);
        Task<AddressDto> CreateAddressAsync(CreateAddressDto dto);
        Task UpdateAddressAsync(int id, UpdateAddressDto dto);
        Task DeleteAddressAsync(int id);
        Task<IEnumerable<AddressDto>> SearchAsync(string term);
        Task<BulkImportResultDto> BulkImportAsync(string[] textAddresses);
    }
}