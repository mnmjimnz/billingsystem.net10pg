namespace BillingSystem.Domain.Entities;

public class PayablePayment : BaseEntity
{
    public int AccountId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
