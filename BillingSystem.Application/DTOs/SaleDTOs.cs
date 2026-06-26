using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.DTOs;

public class CreateSaleRequest
{
    public int? CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string PaymentType { get; set; } = "CASH";
    public decimal AmountTendered { get; set; }
    public decimal Change { get; set; }
    public List<SaleDetail> Details { get; set; } = new();
}
