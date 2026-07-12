namespace BillingSystem.Domain.Entities;

public class Account : BaseEntity
{
    public string Code { get; set; } = string.Empty; // e.g. "1.01.01"
    public string Name { get; set; } = string.Empty; // e.g. "Caja General"
    
    // Type of account: Asset, Liability, Equity, Revenue, Expense, Cost
    public string Type { get; set; } = string.Empty; 
    
    public int? ParentAccountId { get; set; }
    
    // Level in the hierarchy (e.g. 1 = Rubro, 2 = Cuenta Mayor, 3 = Subcuenta)
    public int Level { get; set; } = 1;
    
    // Whether this account allows direct transactions (usually false for parent accounts)
    public bool AllowsTransactions { get; set; } = true;
    
    public string Description { get; set; } = string.Empty;
}
