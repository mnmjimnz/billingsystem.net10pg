using BillingSystem.Application.DTOs;

namespace BillingSystem.Application.Interfaces;

public interface IKardexService
{
    Task<IEnumerable<KardexDto>> GetAllMovementsAsync(int? productId);
    Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize);
}

public interface IReceivableService
{
    Task<IEnumerable<ReceivableDto>> GetReceivablesAsync();
    Task RegisterPaymentAsync(int id, int userId, decimal amount, string notes);
    Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize);
}

public interface INotificationService
{
    Task DispatchNotificationAsync(string title, string message, string type, int? referenceId);
    Task ResolveNotificationAsync(int referenceId, string type);
}

public interface ISaleService
{
    Task<(int SaleId, string TicketNumber)> CreateSaleAsync(CreateSaleRequest request, int userId, int branchId);
}
