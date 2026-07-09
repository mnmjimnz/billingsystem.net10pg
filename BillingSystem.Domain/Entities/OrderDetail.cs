namespace BillingSystem.Domain.Entities;

public class OrderDetail : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? ProductName { get; set; }
}
