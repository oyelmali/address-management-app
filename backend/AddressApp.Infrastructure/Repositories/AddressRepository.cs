using Microsoft.EntityFrameworkCore;
using AddressApp.Core.Entities;
using AddressApp.Core.Interfaces;
using AddressApp.Infrastructure.Data;

namespace AddressApp.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _context;

    public AddressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Address>> GetAllAsync()
    {
        return await _context.Addresses
            .OrderBy(a => a.City)
            .ThenBy(a => a.BranchNumber)
            .ToListAsync();
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        return await _context.Addresses.FindAsync(id);
    }

    public async Task<Address> AddAsync(Address address)
    {
        address.CreatedAt = DateTime.UtcNow;
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task UpdateAsync(Address address)
    {
        address.UpdatedAt = DateTime.UtcNow;
        _context.Entry(address).State = EntityState.Modified;
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

    public async Task<IEnumerable<Address>> SearchAsync(string searchTerm)
{
    if (string.IsNullOrWhiteSpace(searchTerm))
        return await GetAllAsync();

    var term = searchTerm.Trim();
    
    // Tüm kayıtları getir ve in-memory'de case-insensitive ara
    var allAddresses = await _context.Addresses.ToListAsync();
    
    return allAddresses.Where(a =>
    {
        // StringComparison.OrdinalIgnoreCase yerine CurrentCultureIgnoreCase kullan
        var comparison = StringComparison.CurrentCultureIgnoreCase;
        
        if (a.City.Contains(term, comparison)) return true;
        if (a.Region.Contains(term, comparison)) return true;
        if (a.Street.Contains(term, comparison)) return true;
        if (a.BranchNumber.Contains(term, comparison)) return true;
        if (a.BranchType.Contains(term, comparison)) return true;
        if (a.Phone.Contains(term, StringComparison.Ordinal)) return true;
        
        if (!string.IsNullOrEmpty(a.OriginalText) && 
            a.OriginalText.Contains(term, comparison)) return true;
        
        var branchFull = $"{a.BranchType} {a.BranchNumber}";
        if (branchFull.Contains(term, comparison)) return true;
        
        var location = $"{a.City} {a.Region}";
        if (location.Contains(term, comparison)) return true;
        
        return false;
    })
    .OrderBy(a => a.City)
    .ThenBy(a => a.BranchNumber)
    .ToList();
}

    public async Task<int> BulkAddAsync(IEnumerable<Address> addresses)
    {
        await _context.Addresses.AddRangeAsync(addresses);
        return await _context.SaveChangesAsync();
    }
}