using AddressApp.Core.Entities;
using AddressApp.Infrastructure.Data;
using AddressApp.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AddressApp.Tests.Repositories;

public class AddressRepositoryTests
{
    private DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ValidAddress_AddsSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        var repository = new AddressRepository(context);

        var address = new Address
        {
            Region = "Київська",
            City = "Київ",
            BranchNumber = "1",
            Street = "вул. Тестова, 1",
            Phone = "+380501234567"
        };

        // Act
        var result = await repository.AddAsync(address);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAddresses()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        var repository = new AddressRepository(context);

        await repository.AddAsync(new Address { Region = "Київська", City = "Київ", BranchNumber = "1", Street = "Test 1", Phone = "+380501111111" });
        await repository.AddAsync(new Address { Region = "Запорізька", City = "Запоріжжя", BranchNumber = "2", Street = "Test 2", Phone = "+380502222222" });

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsAddress()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        var repository = new AddressRepository(context);

        var address = await repository.AddAsync(new Address { Region = "Київська", City = "Київ", BranchNumber = "1", Street = "Test", Phone = "+380501234567" });

        // Act
        var result = await repository.GetByIdAsync(address.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(address.Id);
    }

    [Fact]
    public async Task SearchAsync_ValidTerm_ReturnsMatchingAddresses()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        var repository = new AddressRepository(context);

        await repository.AddAsync(new Address { Region = "Київська", City = "Київ", BranchNumber = "1", Street = "Test", Phone = "+380501234567" });
        await repository.AddAsync(new Address { Region = "Запорізька", City = "Запоріжжя", BranchNumber = "2", Street = "Test", Phone = "+380502222222" });

        // Act
        var result = await repository.SearchAsync("Київ");

        // Assert
        result.Should().HaveCount(1);
        result.First().City.Should().Be("Київ");
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesAddress()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        var repository = new AddressRepository(context);

        var address = await repository.AddAsync(new Address { Region = "Київська", City = "Київ", BranchNumber = "1", Street = "Test", Phone = "+380501234567" });

        // Act
        await repository.DeleteAsync(address.Id);
        var result = await repository.GetByIdAsync(address.Id);

        // Assert
        result.Should().BeNull();
    }
}