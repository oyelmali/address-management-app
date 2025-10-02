namespace AddressApp.Application.DTOs;

public class AddressDto
{
    public int Id { get; set; }
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string BranchNumber { get; set; } = string.Empty;
    public string BranchType { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string WorkingHoursWeekdays { get; set; } = string.Empty;
    public string WorkingHoursWeekend { get; set; } = string.Empty;
}

public class CreateAddressDto
{
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string BranchNumber { get; set; } = string.Empty;
    public string BranchType { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string WorkingHoursWeekdays { get; set; } = string.Empty;
    public string WorkingHoursWeekend { get; set; } = string.Empty;
}

public class UpdateAddressDto
{
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string BranchNumber { get; set; } = string.Empty;
    public string BranchType { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string WorkingHoursWeekdays { get; set; } = string.Empty;
    public string WorkingHoursWeekend { get; set; } = string.Empty;
}

public class BulkImportResultDto
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<string> Errors { get; set; } = new();
}