namespace BillingSystem.Domain.Entities;

public class JournalEntry : BaseEntity
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty; // "SALE", "PURCHASE", etc.
    public int? ReferenceId { get; set; }
    public ICollection<JournalEntryDetail> Details { get; set; } = new List<JournalEntryDetail>();
}
