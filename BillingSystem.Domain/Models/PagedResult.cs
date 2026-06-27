namespace BillingSystem.Domain.Models;

public class PagedResult<T>
{
    public System.Collections.Generic.IEnumerable<T> Items { get; set; } = new System.Collections.Generic.List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 10));
}