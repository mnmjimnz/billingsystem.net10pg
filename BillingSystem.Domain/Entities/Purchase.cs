namespace BillingSystem.Domain.Entities;

public class Purchase : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public decimal Total { get; set; }
    public string PaymentType { get; set; } = "CASH";
    public decimal AmountPaid { get; set; }
    public string Status { get; set; } = "PAID";
}
