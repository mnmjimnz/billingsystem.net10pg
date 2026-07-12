namespace BillingSystem.Domain.Entities;

public class BankAccount : BaseEntity
{
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public decimal CurrentBalance { get; set; } = 0m;
    
    // The GL account associated with this bank account
    public int LinkedAccountId { get; set; }
}

public class BankReconciliation : BaseEntity
{
    public int BankAccountId { get; set; }
    public DateTime StatementDate { get; set; }
    public decimal StatementBalance { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Reconciled
    public string Notes { get; set; } = string.Empty;
}

public class BankReconciliationDetail : BaseEntity
{
    public int BankReconciliationId { get; set; }
    public int JournalEntryDetailId { get; set; } // The specific debit/credit line in the journal
    public bool IsCleared { get; set; } = false;
}
