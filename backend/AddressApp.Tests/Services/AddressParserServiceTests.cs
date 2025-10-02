using AddressApp.Application.Services;
using FluentAssertions;
using Xunit;

namespace AddressApp.Tests.Services;

public class AddressParserServiceTests
{
    private readonly AddressParserService _parserService;

    public AddressParserServiceTests()
    {
        _parserService = new AddressParserService();
    }

    [Fact]
    public void ParseFromText_ValidInput_ReturnsCorrectAddress()
    {
        // Arrange
        var input = "Запорізька обл., Запоріжжя, Відділення №16 (до 30 кг на одне місце): просп. Леніна, 84, +380974805040, пн-ср 08:00-15:00, сб-нд 10:00-12:00";

        // Act
        var result = _parserService.ParseFromText(input);

        // Assert
        result.Should().NotBeNull();
        result!.Region.Should().Be("Запорізька");
        result.City.Should().Be("Запоріжжя");
        result.BranchNumber.Should().Be("16");
        result.BranchType.Should().Contain("Відділення");
        result.Street.Should().Contain("просп. Леніна, 84");
        result.Phone.Should().Be("+380974805040");
        result.WorkingHoursWeekdays.Should().Be("пн-ср 08:00-15:00");
        result.WorkingHoursWeekend.Should().Be("сб-нд 10:00-12:00");
    }

    [Fact]
    public void ParseFromText_PostomatInput_ParsesCorrectly()
    {
        // Arrange
        var input = "Київська обл., Київ, Поштомат InPost 24/7, №2029: пр-т Повітрофлотський, 56а (цілодобовий поштомат біля маг.\"Billa\"), +380974803040, пн-ср 08:00-15:00, сб-нд 10:00-20:00";

        // Act
        var result = _parserService.ParseFromText(input);

        // Assert
        result.Should().NotBeNull();
        result!.Region.Should().Be("Київська");
        result.City.Should().Be("Київ");
        result.BranchNumber.Should().Be("2029");
        result.BranchType.Should().Contain("Поштомат");
    }

    [Fact]
    public void ParseFromText_EmptyString_ReturnsNull()
    {
        // Arrange
        var input = "";

        // Act
        var result = _parserService.ParseFromText(input);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void ParseFromText_InvalidInput_ReturnsNull(string input)
    {
        // Act
        var result = _parserService.ParseFromText(input);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFromText_MultipleAddresses_ParsesAllCorrectly()
    {
        // Arrange
        var inputs = new[]
        {
            "Запорізька обл., Запоріжжя, Відділення №17 (до 30 кг на одне місце): вул. Авраменка, 13, +380974678040, пн-ср 09:00-16:00, сб-нд 10:00-13:00",
            "Київська обл., Київ, Відділення №1: вул. Пирогівський шлях, 135, +38097487640, пн-ср 08:00-19:00, сб-нд 10:00-16:00"
        };

        // Act & Assert
        foreach (var input in inputs)
        {
            var result = _parserService.ParseFromText(input);
            result.Should().NotBeNull();
            result!.Region.Should().NotBeNullOrEmpty();
            result.City.Should().NotBeNullOrEmpty();
            result.BranchNumber.Should().NotBeNullOrEmpty();
        }
    }
}