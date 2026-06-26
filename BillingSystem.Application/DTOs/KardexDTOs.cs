namespace BillingSystem.Application.DTOs;

public class KardexDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
