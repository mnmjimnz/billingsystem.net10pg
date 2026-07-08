namespace BillingSystem.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
