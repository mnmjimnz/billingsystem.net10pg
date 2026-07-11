namespace BillingSystem.Domain.Entities;
using System;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public int? MaxUses { get; set; }
    public int CurrentUses { get; set; }
    
}
