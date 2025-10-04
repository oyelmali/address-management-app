using Xunit;
using Moq;
using AutoMapper;
using FluentAssertions;
using AddressApp.Application.Services;
using AddressApp.Application.DTOs;
using AddressApp.Application.Mappings;
using AddressApp.Domain.Interfaces;
using AddressApp.Domain.Entities;

namespace AddressApp.UnitTests.Services
{
    public class AddressServiceTests
    {
        private readonly Mock<IAddressRepository> _mockRepository;
        private readonly IMapper _mapper;
        private readonly AddressService _service;

        public AddressServiceTests()
        {
            _mockRepository = new Mock<IAddressRepository>();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AddressProfile>();
            });
            _mapper = config.CreateMapper();
            
            _service = new AddressService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAddressesAsync_ReturnsAllAddresses()
        {
            // Arrange
            var addresses = new List<Address>
            {
                new Address 
                { 
                    Id = Guid.NewGuid(), 
                    Region = "Zaporizhzhia", 
                    City = "Zaporizhzhia",
                    StreetAddress = "Test Street 1",
                    CreatedAt = DateTime.UtcNow 
                },
                new Address 
                { 
                    Id = Guid.NewGuid(), 
                    Region = "Kyiv", 
                    City = "Kyiv",
                    StreetAddress = "Test Street 2",
                    CreatedAt = DateTime.UtcNow 
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(addresses);

            // Act
            var result = await _service.GetAllAddressesAsync();

            // Assert
            result.Should().HaveCount(2);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAddressAsync_ValidAddress_ReturnsCreatedAddress()
        {
            // Arrange
            var createDto = new CreateAddressDto
            {
                Region = "Test Region",
                City = "Test City",
                StreetAddress = "Test Street"
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Address>()))
                          .ReturnsAsync((Address a) => a);

            // Act
            var result = await _service.CreateAddressAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Region.Should().Be(createDto.Region);
            result.City.Should().Be(createDto.City);
            result.StreetAddress.Should().Be(createDto.StreetAddress);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Address>()), Times.Once);
        }

        [Fact]
        public async Task BulkImportAsync_ParsesTextCorrectly()
        {
            // Arrange
            var textData = "Запорізька обл., Запоріжжя, Відділення №16 (до 30 кг на одне місце): просп. Леніна, 84, +380974805040, пн-ср 08:00-15:00";
            
            _mockRepository.Setup(r => r.BulkAddAsync(It.IsAny<IEnumerable<Address>>()))
                          .ReturnsAsync((IEnumerable<Address> addresses) => addresses);

            // Act
            var result = await _service.BulkImportAsync(textData);

            // Assert
            result.Should().HaveCount(1);
            var address = result.First();
            address.Region.Should().Contain("Запорізька");
            address.City.Should().Be("Запоріжжя");
            address.BranchNumber.Should().Be("16");
            address.Phone.Should().Be("+380974805040");
            _mockRepository.Verify(r => r.BulkAddAsync(It.IsAny<IEnumerable<Address>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAddressAsync_CallsRepository()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            await _service.DeleteAddressAsync(id);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_CallsRepositoryWithSearchTerm()
        {
            // Arrange
            var searchTerm = "Kyiv";
            var addresses = new List<Address>
            {
                new Address 
                { 
                    Id = Guid.NewGuid(), 
                    Region = "Kyiv", 
                    City = "Kyiv",
                    StreetAddress = "Test Street",
                    CreatedAt = DateTime.UtcNow 
                }
            };

            _mockRepository.Setup(r => r.SearchAsync(searchTerm)).ReturnsAsync(addresses);

            // Act
            var result = await _service.SearchAsync(searchTerm);

            // Assert
            result.Should().HaveCount(1);
            _mockRepository.Verify(r => r.SearchAsync(searchTerm), Times.Once);
        }
    }
}