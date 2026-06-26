namespace BillingSystem.Domain.Entities;

public class AccountsReceivable
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalDebt { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "PENDING";
    public DateTime CreatedAt { get; set; }
}

public class ReceivablePayment
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "INFO";
    public int? ReferenceId { get; set; }
    public bool IsResolved { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}
