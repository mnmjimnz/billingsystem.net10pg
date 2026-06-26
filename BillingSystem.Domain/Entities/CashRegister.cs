namespace BillingSystem.Domain.Entities;

public class CashRegister : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public string Description { get; set; } = string.Empty;
}
