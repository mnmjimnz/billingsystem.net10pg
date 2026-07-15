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
    private readonly IBranchMovementRepository _movementRepo;

    public CashRegisterService(ICashRegisterRepository cashRepo, IBranchRepository branchRepo, ISaleRepository saleRepo, IBranchMovementRepository movementRepo)
    {
        _cashRepo = cashRepo;
        _branchRepo = branchRepo;
        _saleRepo = saleRepo;
        _movementRepo = movementRepo;
    }

    public async Task<CashRegisterSession?> GetActiveSessionAsync(int userId)
    {
        return await _cashRepo.GetActiveSessionAsync(userId);
    }

    public async Task<IEnumerable<CashRegister>> GetRegistersByBranchAsync(int branchId)
    {
        return await _cashRepo.GetByBranchIdAsync(branchId);
    }

    public async Task<int> OpenSessionAsync(int userId, int cashRegisterId, decimal openingBalance)
    {
        var existing = await _cashRepo.GetActiveSessionAsync(userId);
        if (existing != null) throw new Exception("El usuario ya tiene una caja abierta.");

        var registerInUse = await _cashRepo.GetActiveSessionByRegisterAsync(cashRegisterId);
        if (registerInUse != null) throw new Exception("La caja seleccionada ya está aperturada por otro usuario.");

        var register = await _cashRepo.GetByIdAsync(cashRegisterId);
        if (register == null) throw new Exception("La caja seleccionada no existe.");

        var branch = await _branchRepo.GetByIdAsync(register.BranchId);
        if (branch == null) throw new Exception("La sucursal de esta caja no existe.");

        if (branch.AvailableFunds < openingBalance)
            throw new Exception($"La sucursal no tiene fondos suficientes. Saldo disponible: ${branch.AvailableFunds:F2}");

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // Deduct from branch
        branch.AvailableFunds -= openingBalance;
        await _branchRepo.UpdateAsync(branch);

        var session = new CashRegisterSession
        {
            CashRegisterId = register.Id,
            UserId = userId,
            OpeningBalance = openingBalance,
            Status = "OPEN"
        };
        var sessionId = await _cashRepo.OpenSessionAsync(session);

        scope.Complete();
        return sessionId;
    }

    public async Task<object?> GetSessionSummaryAsync(int userId)
    {
        var session = await _cashRepo.GetActiveSessionAsync(userId);
        if (session == null) return null;

        var salesTotal = await _saleRepo.GetSessionSalesTotalAsync(userId, session.OpeningTime);
        var movsIn = await _movementRepo.GetSessionMovementsTotalAsync(session.CashRegisterId, session.OpeningTime, "IN");
        var movsOut = await _movementRepo.GetSessionMovementsTotalAsync(session.CashRegisterId, session.OpeningTime, "OUT");
        
        var expectedBalance = session.OpeningBalance + salesTotal + movsIn - movsOut;

        return new
        {
            session.OpeningBalance,
            salesTotal,
            movsIn,
            movsOut,
            expectedBalance,
            session.OpeningTime
        };
    }

    public async Task CloseSessionAsync(int userId)
    {
        var session = await _cashRepo.GetActiveSessionAsync(userId);
        if (session == null) throw new Exception("No hay ninguna sesión de caja abierta.");

        var salesTotal = await _saleRepo.GetSessionSalesTotalAsync(userId, session.OpeningTime);
        var movsIn = await _movementRepo.GetSessionMovementsTotalAsync(session.CashRegisterId, session.OpeningTime, "IN");
        var movsOut = await _movementRepo.GetSessionMovementsTotalAsync(session.CashRegisterId, session.OpeningTime, "OUT");
        
        var calculatedBalance = session.OpeningBalance + salesTotal + movsIn - movsOut;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        session.ClosingBalance = calculatedBalance;
        session.DeclaredBalance = calculatedBalance; // Since no manual entry, declared = calculated

        await _cashRepo.CloseSessionAsync(session);

        // Move funds to branch
        var register = await _cashRepo.GetByIdAsync(session.CashRegisterId);
        if (register != null)
        {
            var branch = await _branchRepo.GetByIdAsync(register.BranchId);
            if (branch != null)
            {
                branch.AvailableFunds += calculatedBalance;
                await _branchRepo.UpdateAsync(branch);
            }
        }

        scope.Complete();
    }
}
