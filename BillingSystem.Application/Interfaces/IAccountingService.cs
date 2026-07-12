using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.Interfaces;

public interface IAccountingService
{
    Task RecordSaleAsync(Order order, decimal costOfGoodsSold);
    Task RecordPosSaleAsync(Sale sale, decimal costOfGoodsSold);
    Task RecordPurchaseAsync(Purchase purchase);
    Task RecordPayrollAsync(PayrollRun payrollRun, IEnumerable<dynamic> details);
    Task<bool> RecordBranchMovementAsync(BranchMovement movement);
    Task RecordManualEntryAsync(string description, string referenceType, int? referenceId, IEnumerable<JournalEntryDetail> details);
}
