namespace BillingSystem.Application.DTOs;

public class ReportFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? BranchId { get; set; }
    public int? UserId { get; set; }
    public string? PaymentType { get; set; } // "CASH", "CREDIT", etc.
}

public class SalesReportDto
{
    public int SaleId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string PaymentType { get; set; } = string.Empty;
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TopSupplierDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int TotalPurchases { get; set; }
    public decimal TotalVolume { get; set; }
}

public class CashFlowDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "IN", "OUT"
    public string Category { get; set; } = string.Empty; // "SALE", "PURCHASE", "AR_PAYMENT", "AP_PAYMENT"
    public decimal Amount { get; set; }
    public int? BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
}

public class KardexReportDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty; // IN, OUT, ADJUST
    public string ReferenceType { get; set; } = string.Empty; // SALE, PURCHASE, etc.
    public int ReferenceId { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SalesComparisonDto
{
    public string Period { get; set; } = string.Empty; // Could be "2023-10-01", "2023-10", "Monday", etc.
    public decimal TotalSales { get; set; }
    public int SaleCount { get; set; }
}

public class DashboardStatsDto
{
    public decimal TodaySales { get; set; }
    public decimal TodayPurchases { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
}

public class DashboardDataDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public IEnumerable<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
}