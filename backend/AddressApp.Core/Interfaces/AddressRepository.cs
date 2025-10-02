using AddressApp.Core.Entities;

namespace AddressApp.Core.Interfaces;

public interface IAddressRepository
{
    Task<IEnumerable<Address>> GetAllAsync();
    Task<Address?> GetByIdAsync(int id);
    Task<Address> AddAsync(Address address);
    Task UpdateAsync(Address address);
    Task DeleteAsync(int id);
    Task<IEnumerable<Address>> SearchAsync(string searchTerm);
    Task<int> BulkAddAsync(IEnumerable<Address> addresses);
}