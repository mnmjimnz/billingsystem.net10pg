using System.Transactions;
using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class ReceivableService : IReceivableService
{
    private readonly IReceivableRepository _receivableRepo;
    private readonly INotificationRepository _notificationRepo;
    private readonly INotificationService _notificationService;

    public ReceivableService(
        IReceivableRepository receivableRepo, 
        INotificationRepository notificationRepo,
        INotificationService notificationService)
    {
        _receivableRepo = receivableRepo;
        _notificationRepo = notificationRepo;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<ReceivableDto>> GetReceivablesAsync()
    {
        var data = await _receivableRepo.GetReceivablesAsync();
        return data.Select(d => new ReceivableDto
        {
            Id = d.id,
            SaleId = d.saleid,
            CustomerId = d.customerid,
            CustomerName = d.customername,
            TicketNumber = d.ticketnumber,
            TotalDebt = d.totaldebt,
            AmountPaid = d.amountpaid,
            Balance = d.balance,
            DueDate = d.duedate,
            Status = d.status,
            CreatedAt = d.createdat
        });
    }

    public async Task RegisterPaymentAsync(int id, int userId, decimal amount, string notes)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var account = await _receivableRepo.GetByIdAsync(id);
        if (account == null) throw new Exception("Cuenta no encontrada.");
        if (account.Balance < amount) throw new Exception("El monto del pago supera el saldo pendiente.");

        var payment = new ReceivablePayment
        {
            AccountId = id,
            UserId = userId,
            Amount = amount,
            Notes = notes
        };

        account.AmountPaid += amount;
        account.Balance -= amount;
        if (account.Balance <= 0)
        {
            account.Status = "PAID";
        }

        await _receivableRepo.UpdateAccountAndAddPaymentAsync(account, payment);

        if (account.Balance <= 0)
        {
            await _notificationRepo.MarkResolvedAsync(id, "CREDIT_SALE");
            await _notificationService.ResolveNotificationAsync(id, "CREDIT_SALE");
        }

        scope.Complete();
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync(string search, int page, int pageSize)
    {
        return await _receivableRepo.GetPagedAsync(search, page, pageSize);
    }

    public async Task<IEnumerable<ReceivablePayment>> GetPaymentsAsync(int accountId)
    {
        return await _receivableRepo.GetPaymentsAsync(accountId);
    }
}
