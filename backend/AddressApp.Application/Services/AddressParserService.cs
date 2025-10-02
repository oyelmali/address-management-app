using System.Text.RegularExpressions;
using AddressApp.Application.Interfaces;
using AddressApp.Core.Entities;

namespace AddressApp.Application.Services;

public class AddressParserService : IAddressParserService
{
    public Address? ParseFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            var address = new Address
            {
                OriginalText = text
            };

            // Parse Region: "Запорізька обл." or "Київська обл."
            var regionMatch = Regex.Match(text, @"^([^,]+?)\s+обл\.");
            if (regionMatch.Success)
                address.Region = regionMatch.Groups[1].Value.Trim();

            // Parse City: "обл., Запоріжжя" or "обл., Київ"
            var cityMatch = Regex.Match(text, @"обл\.,\s+([^,]+?)(?:,|$)");
            if (cityMatch.Success)
                address.City = cityMatch.Groups[1].Value.Trim();

            // Parse Branch Type and Number - Updated regex to handle both formats
            // Format 1: "Відділення №16"
            // Format 2: "Поштомат InPost 24/7, №2029"
            var branchMatch = Regex.Match(text, @"(Відділення|Поштомат\s+InPost\s+24/7|Поштомат)[^№]*№(\d+)");
            if (branchMatch.Success)
            {
                address.BranchType = branchMatch.Groups[1].Value.Trim();
                address.BranchNumber = branchMatch.Groups[2].Value.Trim();
            }

            // Parse Street: after ":" and before phone
            // This handles addresses with parentheses like "(цілодобовий поштомат біля маг."Billa")"
            var streetMatch = Regex.Match(text, @":\s+([^+]+?)(?=,\s*\+)");
            if (streetMatch.Success)
            {
                var street = streetMatch.Groups[1].Value.Trim();
                // Remove trailing comma if exists
                street = street.TrimEnd(',', ' ');
                address.Street = street;
            }

            // Parse Phone: "+380..."
            var phoneMatch = Regex.Match(text, @"(\+380\d{9})");
            if (phoneMatch.Success)
                address.Phone = phoneMatch.Groups[1].Value.Trim();

            // Parse Working Hours - handle both formats
            var hoursMatch = Regex.Match(text, @"(пн-[а-яА-Я]{2}\s+\d{2}:\d{2}-\d{2}:\d{2}).*?,\s*(сб-нд\s+\d{2}:\d{2}-\d{2}:\d{2})");
            if (hoursMatch.Success)
            {
                address.WorkingHoursWeekdays = hoursMatch.Groups[1].Value.Trim();
                address.WorkingHoursWeekend = hoursMatch.Groups[2].Value.Trim();
            }

            return address;
        }
        catch
        {
            return null;
        }
    }
}