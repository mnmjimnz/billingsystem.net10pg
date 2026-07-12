namespace BillingSystem.Domain.Entities;

public class Attendance : BaseEntity
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public string Status { get; set; } = "Present"; // Present, Late, Absent
}

public class PayrollRun : BaseEntity
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
}

public class PayrollDetail : BaseEntity
{
    public int PayrollRunId { get; set; }
    public int UserId { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal ExtraHoursAmount { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal DeductionsAmount { get; set; }
    public decimal NetPay { get; set; }
    public string Observations { get; set; } = string.Empty;
}
