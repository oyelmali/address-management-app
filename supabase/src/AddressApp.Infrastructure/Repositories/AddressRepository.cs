using Microsoft.EntityFrameworkCore;
using AddressApp.Domain.Entities;
using AddressApp.Domain.Interfaces;
using AddressApp.Infrastructure.Data;

namespace AddressApp.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _context.Addresses
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task<Address> AddAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Address>> SearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetAllAsync();

            var query = _context.Addresses.AsQueryable();
            
            term = term.ToLower();
            
            query = query.Where(a => 
                a.Region.ToLower().Contains(term) ||
                a.City.ToLower().Contains(term) ||
                a.BranchNumber.Contains(term) ||
                a.BranchType.ToLower().Contains(term) ||
                a.Street.ToLower().Contains(term) ||
                a.Phone.Contains(term)
            );

            return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Address>> BulkAddAsync(IEnumerable<Address> addresses)
        {
            await _context.Addresses.AddRangeAsync(addresses);
            await _context.SaveChangesAsync();
            return addresses;
        }
    }
}