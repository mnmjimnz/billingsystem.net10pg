using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IKardexRepository
{
    Task<IEnumerable<dynamic>> GetAllMovementsAsync(int? productId);
    Task AddMovementAsync(InventoryMovement movement);
    Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize);
}

public interface IReceivableRepository
{
    Task<IEnumerable<dynamic>> GetReceivablesAsync();
    Task<AccountsReceivable?> GetByIdAsync(int id);
    Task UpdateAccountAndAddPaymentAsync(AccountsReceivable account, ReceivablePayment payment);
    Task<int> CreateAsync(AccountsReceivable account);
    Task AddPaymentAsync(ReceivablePayment payment);
    Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize);
    Task<IEnumerable<ReceivablePayment>> GetPaymentsAsync(int accountId);
}

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetUnreadAsync();
    Task MarkAsReadAsync(int id);
    Task MarkResolvedAsync(int referenceId, string type);
    Task AddAsync(Notification notification);
}
