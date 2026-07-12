using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class AccountingService : IAccountingService
{
    private readonly IAccountingRepository _accountingRepo;

    public AccountingService(IAccountingRepository accountingRepo)
    {
        _accountingRepo = accountingRepo;
    }

    private async Task<int> GetAccountIdByCode(string code)
    {
        var accounts = await _accountingRepo.GetAccountsAsync();
        var account = accounts.FirstOrDefault(a => a.Code == code);
        if (account == null) throw new Exception($"Accounting error: Account with code {code} not found.");
        return account.Id;
    }

    public async Task RecordSaleAsync(Order order, decimal costOfGoodsSold)
    {
        var cajaId = await GetAccountIdByCode("1.01.01");
        var cxcId = await GetAccountIdByCode("1.01.03");
        var ventasId = await GetAccountIdByCode("4.01.01");
        var costoVentasId = await GetAccountIdByCode("5.01.01");
        var inventarioId = await GetAccountIdByCode("1.01.04");

        var entry = new JournalEntry
        {
            Date = DateTime.UtcNow,
            Description = $"Venta según Orden #{order.Id}",
            ReferenceType = "SALE",
            ReferenceId = order.Id
        };

        var details = new List<JournalEntryDetail>();

        // Ingreso por venta
        if (order.Status == "Paid" || order.Status == "Delivered")
        {
            details.Add(new JournalEntryDetail { AccountId = cajaId, Debit = order.Total });
        }
        else
        {
            details.Add(new JournalEntryDetail { AccountId = cxcId, Debit = order.Total });
        }
        details.Add(new JournalEntryDetail { AccountId = ventasId, Credit = order.Total });

        // Costo de venta
        if (costOfGoodsSold > 0)
        {
            details.Add(new JournalEntryDetail { AccountId = costoVentasId, Debit = costOfGoodsSold });
            details.Add(new JournalEntryDetail { AccountId = inventarioId, Credit = costOfGoodsSold });
        }

        await _accountingRepo.AddJournalEntryAsync(entry, details);
    }

    public async Task RecordPurchaseAsync(Purchase purchase)
    {
        var inventarioId = await GetAccountIdByCode("1.01.04");
        var cajaId = await GetAccountIdByCode("1.01.01");
        var cxpId = await GetAccountIdByCode("2.01.01");

        var entry = new JournalEntry
        {
            Date = DateTime.UtcNow,
            Description = $"Compra según registro #{purchase.Id}",
            ReferenceType = "PURCHASE",
            ReferenceId = purchase.Id
        };

        var details = new List<JournalEntryDetail>
        {
            new JournalEntryDetail { AccountId = inventarioId, Debit = purchase.Total }
        };

        if (purchase.Status == "Paid")
        {
            details.Add(new JournalEntryDetail { AccountId = cajaId, Credit = purchase.Total });
        }
        else
        {
            details.Add(new JournalEntryDetail { AccountId = cxpId, Credit = purchase.Total });
        }

        await _accountingRepo.AddJournalEntryAsync(entry, details);
    }

    public async Task RecordPayrollAsync(PayrollRun payrollRun, IEnumerable<dynamic> detailsDynamic)
    {
        var sueldosGastoId = await GetAccountIdByCode("6.01.01");
        var sueldosPagarId = await GetAccountIdByCode("2.01.04");
        var retencionesId = await GetAccountIdByCode("2.01.03");
        var bancosId = await GetAccountIdByCode("1.01.02");

        decimal totalBase = 0;
        decimal totalDeductions = 0;
        decimal totalNet = 0;

        foreach (var d in detailsDynamic)
        {
            totalBase += (decimal)d.BaseSalary + (decimal)d.BonusAmount + (decimal)d.ExtraHoursAmount;
            totalDeductions += (decimal)d.DeductionsAmount;
            totalNet += (decimal)d.NetPay;
        }

        var entry = new JournalEntry
        {
            Date = DateTime.UtcNow,
            Description = $"Nómina periodo {payrollRun.PeriodStart:yyyy-MM-dd} al {payrollRun.PeriodEnd:yyyy-MM-dd}",
            ReferenceType = "PAYROLL",
            ReferenceId = payrollRun.Id
        };

        var details = new List<JournalEntryDetail>
        {
            new JournalEntryDetail { AccountId = sueldosGastoId, Debit = totalBase },
            new JournalEntryDetail { AccountId = retencionesId, Credit = totalDeductions },
            new JournalEntryDetail { AccountId = bancosId, Credit = totalNet }
        };

        await _accountingRepo.AddJournalEntryAsync(entry, details);
    }

    public async Task<bool> RecordBranchMovementAsync(BranchMovement movement)
    {
        if (movement.AccountId == null) return false;

        var accounts = await _accountingRepo.GetAccountsAsync();
        var selectedAccount = accounts.FirstOrDefault(a => a.Id == movement.AccountId.Value);
        if (selectedAccount == null) return false;

        int cajaId = accounts.FirstOrDefault(a => a.Code == "1.01.01")?.Id ?? 0;
        int bancoId = accounts.FirstOrDefault(a => a.Code == "1.01.02")?.Id ?? 0;

        int cashAccountId = movement.PaymentMethod == "Bank" ? bancoId : cajaId;

        if (cashAccountId == 0) return false;

        var entry = new JournalEntry
        {
            Date = movement.Date,
            Description = $"Movimiento de Sucursal - {movement.Category} - {movement.Description}",
            ReferenceType = "Movement",
            ReferenceId = movement.Id
        };

        var details = new List<JournalEntryDetail>();

        if (movement.Type == "IN")
        {
            details.Add(new JournalEntryDetail { AccountId = cashAccountId, Debit = movement.Amount });
            details.Add(new JournalEntryDetail { AccountId = selectedAccount.Id, Credit = movement.Amount });
        }
        else if (movement.Type == "OUT")
        {
            details.Add(new JournalEntryDetail { AccountId = selectedAccount.Id, Debit = movement.Amount });
            details.Add(new JournalEntryDetail { AccountId = cashAccountId, Credit = movement.Amount });
        }

        await _accountingRepo.AddJournalEntryAsync(entry, details);
        return true;
    }

    public async Task RecordManualEntryAsync(string description, string referenceType, int referenceId, IEnumerable<JournalEntryDetail> details)
    {
        var entry = new JournalEntry
        {
            Date = DateTime.UtcNow,
            Description = description,
            ReferenceType = referenceType,
            ReferenceId = referenceId
        };
        await _accountingRepo.AddJournalEntryAsync(entry, details);
    }
}
