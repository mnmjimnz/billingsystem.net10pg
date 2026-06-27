using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.DTOs;

public class PurchaseDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int UserId { get; set; }
    public int BranchId { get; set; }
    public decimal Total { get; set; }
    public string PaymentType { get; set; } = "CASH";
    public decimal AmountPaid { get; set; }
    public List<PurchaseDetail> Details { get; set; } = new();
}
