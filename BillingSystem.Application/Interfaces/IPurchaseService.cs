using BillingSystem.Application.DTOs;

namespace BillingSystem.Application.Interfaces;

public interface IPurchaseService
{
    Task<int> CreatePurchaseAsync(PurchaseDto dto, int userId);
    Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize);
}
