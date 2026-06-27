using System.Transactions;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class CashRegisterService : ICashRegisterService
{
    private readonly ICashRegisterRepository _cashRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ISaleRepository _saleRepo;

    public CashRegisterService(ICashRegisterRepository cashRepo, IBranchRepository branchRepo, ISaleRepository saleRepo)
    {
        _cashRepo = cashRepo;
        _branchRepo = branchRepo;
        _saleRepo = saleRepo;
    }

    public async Task<CashRegisterSession?> GetActiveSessionAsync(int userId)
    {
        return await _cashRepo.GetActiveSessionAsync(userId);
    }

    public async Task<int> OpenSessionAsync(int userId, int branchId, decimal openingBalance)
    {
        var existing = await _cashRepo.GetActiveSessionAsync(userId);
        if (existing != null) throw new Exception("El usuario ya tiene una caja abierta.");

        var register = await _cashRepo.GetDefaultRegisterAsync(branchId);
        if (register == null) throw new Exception("No hay cajas registradoras disponibles en esta sucursal.");

        var session = new CashRegisterSession
        {
            CashRegisterId = register.Id,
            UserId = userId,
            OpeningBalance = openingBalance,
            Status = "OPEN"
        };
        return await _cashRepo.OpenSessionAsync(session);
    }

    public async Task<object?> GetSessionSummaryAsync(int userId)
    {
        var session = await _cashRepo.GetActiveSessionAsync(userId);
        if (session == null) return null;

        var salesTotal = await _saleRepo.GetSessionSalesTotalAsync(userId, session.OpeningTime);
        var expectedBalance = session.OpeningBalance + salesTotal;

        return new
        {
            session.OpeningBalance,
            salesTotal,
            expectedBalance,
            session.OpeningTime
        };
    }

    public async Task CloseSessionAsync(int userId)
    {
        var session = await _cashRepo.GetActiveSessionAsync(userId);
        if (session == null) throw new Exception("No hay ninguna sesión de caja abierta.");

        var salesTotal = await _saleRepo.GetSessionSalesTotalAsync(userId, session.OpeningTime);
        var calculatedBalance = session.OpeningBalance + salesTotal;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        session.ClosingBalance = calculatedBalance;
        session.DeclaredBalance = calculatedBalance; // Since no manual entry, declared = calculated

        await _cashRepo.CloseSessionAsync(session);

        // Move funds to branch
        var register = await _cashRepo.GetDefaultRegisterAsync(1); // Need branch ID from register
        var branch = await _branchRepo.GetByIdAsync(register!.BranchId);
        if (branch != null)
        {
            branch.AvailableFunds += calculatedBalance;
            await _branchRepo.UpdateAsync(branch);
        }

        scope.Complete();
    }
}
