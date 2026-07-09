namespace BillingSystem.Domain.Entities;

public class Theme
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PreviewImage { get; set; }
    public string Version { get; set; } = "1.0.0";
    public bool IsActive { get; set; }
    public DateTime InstalledDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
