namespace BillingSystem.Domain.Entities;

public class Sale : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string PaymentType { get; set; } = "CASH";
    public decimal AmountTendered { get; set; }
    public decimal Change { get; set; }
    public string Status { get; set; } = "PAID";
}
