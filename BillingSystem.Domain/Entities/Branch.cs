namespace BillingSystem.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal AvailableFunds { get; set; } = 0;
    public string Status { get; set; } = "OPEN";
}
