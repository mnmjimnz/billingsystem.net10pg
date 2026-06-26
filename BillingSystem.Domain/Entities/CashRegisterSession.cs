namespace BillingSystem.Domain.Entities;

public class CashRegisterSession : BaseEntity
{
    public int CashRegisterId { get; set; }
    public CashRegister? CashRegister { get; set; }
    public int UserId { get; set; } // Cajero
    public User? User { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime? ClosingTime { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; } // Lo que dice el sistema
    public decimal? DeclaredBalance { get; set; } // Lo que el cajero cuenta (para diferencias)
    public string Status { get; set; } = "OPEN"; // OPEN, CLOSED
}
