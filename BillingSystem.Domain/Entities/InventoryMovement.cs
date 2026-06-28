namespace BillingSystem.Domain.Entities;

public class InventoryMovement : BaseEntity
{
    public int ProductId { get; set; }
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public Product? Product { get; set; }
    public string MovementType { get; set; } = string.Empty; // "IN", "OUT"
    public string ReferenceType { get; set; } = string.Empty; // "SALE", "PURCHASE", "ADJUSTMENT"
    public int? ReferenceId { get; set; } // SaleId or PurchaseId
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string Description { get; set; } = string.Empty;
}
