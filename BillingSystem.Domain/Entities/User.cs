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
}
