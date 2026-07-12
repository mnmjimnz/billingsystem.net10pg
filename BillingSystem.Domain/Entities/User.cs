namespace BillingSystem.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public decimal? Salary { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? TerminationReason { get; set; }
    
    // HR Fields
    public string? JobTitle { get; set; }
    public string? DocumentId { get; set; } // DPI/Cedula/SSN
    public decimal? BaseBonus { get; set; }
    public decimal IncomeTaxPercentage { get; set; }
}
