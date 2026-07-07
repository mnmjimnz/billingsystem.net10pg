namespace BillingSystem.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    public string Status { get; set; } = "PENDING";
    public string DeliveryAddress { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? ReceiverName { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
    public decimal Total { get; set; }
    public List<OrderDetail> Details { get; set; } = new();
}
