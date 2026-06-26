namespace BillingSystem.Domain.Entities;

public class AccountsPayable : BaseEntity
{
    public int PurchaseId { get; set; }
    public int SupplierId { get; set; }
    public decimal TotalDebt { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "PENDING";
}
