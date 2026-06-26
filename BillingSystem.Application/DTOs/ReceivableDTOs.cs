namespace BillingSystem.Application.DTOs;

public class ReceivableDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
}
