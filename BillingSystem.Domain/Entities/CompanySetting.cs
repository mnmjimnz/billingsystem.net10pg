namespace BillingSystem.Domain.Entities;

public class CompanySetting
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
