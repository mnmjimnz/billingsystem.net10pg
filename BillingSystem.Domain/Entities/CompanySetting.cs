namespace BillingSystem.Domain.Entities;

public class CompanySetting
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public string StoreTheme { get; set; } = "minimalist";
    public bool ShowStoreSlider { get; set; } = true;
    public int StoreProductsPerPage { get; set; } = 12;
    public int? ActiveThemeId { get; set; }
    public string SliderImage1 { get; set; } = "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?q=80&w=2070&auto=format&fit=crop";
    public string SliderImage2 { get; set; } = "https://images.unsplash.com/photo-1607082350899-7e105aa886ae?q=80&w=2070&auto=format&fit=crop";
    public string SliderImage3 { get; set; } = "https://images.unsplash.com/photo-1472851294608-062f824d29cc?q=80&w=2070&auto=format&fit=crop";
    
    // HR Global Settings
    public decimal SocialSecurityPercentage { get; set; } = 4.83m; // IGSS in Guatemala default
    public decimal IncomeTaxPercentage { get; set; } = 0m; // Default ISR
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
