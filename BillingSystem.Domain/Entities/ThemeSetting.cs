namespace BillingSystem.Domain.Entities;

public class ThemeSetting
{
    public int Id { get; set; }
    public int ThemeId { get; set; }
    public string PrimaryColor { get; set; } = "#000000";
    public string SecondaryColor { get; set; } = "#ffffff";
    public string FontFamily { get; set; } = "Inter, sans-serif";
    public string BorderRadius { get; set; } = "0px";
    public string? MainBannerUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string ButtonStyle { get; set; } = "solid";
    public int ProductsPerRow { get; set; } = 4;
    public string ProductCardStyle { get; set; } = "clean";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
