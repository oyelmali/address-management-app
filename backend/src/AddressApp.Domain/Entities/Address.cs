namespace AddressApp.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; } // Guid yerine int
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string BranchNumber { get; set; } = string.Empty;
        public string BranchType { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty; // streetAddress yerine street
        public string Phone { get; set; } = string.Empty;
        public string WorkingHoursWeekdays { get; set; } = string.Empty;
        public string WorkingHoursWeekend { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}