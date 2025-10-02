using AddressApp.Core.Entities;

namespace AddressApp.Application.Interfaces;

public interface IAddressParserService
{
    Address? ParseFromText(string text);
}