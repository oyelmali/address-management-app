using AddressApp.Domain.Entities;

namespace AddressApp.Domain.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address?> GetByIdAsync(int id);
        Task<Address> AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(int id);
        Task<IEnumerable<Address>> SearchAsync(string term);
        Task<IEnumerable<Address>> BulkAddAsync(IEnumerable<Address> addresses);
    }
}