namespace BillingSystem.Domain.Entities;

public class BranchMovement : BaseEntity
{
    public int BranchId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // 'IN' or 'OUT'
    public string Category { get; set; } = string.Empty; 
    public string? Description { get; set; }
    public int UserId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public int? CashRegisterId { get; set; }
    public CashRegister? CashRegister { get; set; }

    public int? AccountId { get; set; }
    public string PaymentMethod { get; set; } = "Cash"; // 'Cash', 'Bank'
    
    // Navigation properties
    public Branch? Branch { get; set; }
    public User? User { get; set; }
    public User? Employee { get; set; }
}
