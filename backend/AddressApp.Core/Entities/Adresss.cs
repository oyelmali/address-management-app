namespace AddressApp.Core.Entities;

public class Address
{
    public int Id { get; set; }
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string BranchNumber { get; set; } = string.Empty;
    public string BranchType { get; set; } = string.Empty; // Відділення or Поштомат
    public string Street { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string WorkingHoursWeekdays { get; set; } = string.Empty;
    public string WorkingHoursWeekend { get; set; } = string.Empty;
    public string OriginalText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}