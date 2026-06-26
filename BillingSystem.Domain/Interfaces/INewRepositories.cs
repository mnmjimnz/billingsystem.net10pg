using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IKardexRepository
{
    Task<IEnumerable<dynamic>> GetAllMovementsAsync(int? productId);
    Task AddMovementAsync(InventoryMovement movement);
}

public interface IReceivableRepository
{
    Task<IEnumerable<dynamic>> GetReceivablesAsync();
    Task<AccountsReceivable?> GetByIdAsync(int id);
    Task UpdateAccountAndAddPaymentAsync(AccountsReceivable account, ReceivablePayment payment);
    Task<int> CreateAsync(AccountsReceivable account);
    Task AddPaymentAsync(ReceivablePayment payment);
}

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetUnreadAsync();
    Task MarkAsReadAsync(int id);
    Task MarkResolvedAsync(int referenceId, string type);
    Task AddAsync(Notification notification);
}
