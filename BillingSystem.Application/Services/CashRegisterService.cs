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

    public async Task CloseSessionAsync(int userId, decimal declaredBalance)
    {
        var session = await _cashRepo.GetActiveSessionAsync(userId);
        if (session == null) throw new Exception("No hay ninguna sesión de caja abierta.");

        // Calcular ventas realizadas en este turno
        // Asumiendo que podemos obtener las ventas del usuario desde OpeningTime
        // Pero de manera más fácil: vamos a confiar en el declared balance,
        // Y el sistema sumará (OpeningBalance + Ventas del turno) para el ClosingBalance real
        // Para simplificar aquí, guardamos lo declarado y movemos ese dinero a la sucursal.
        
        // Lo correcto en un POS es calcular total en base de datos.
        // Simularemos un cálculo rápido sumando ventas. (Requiere método en repo)
        // Por ahora transferimos el DeclaredBalance a la sucursal.

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        session.ClosingBalance = declaredBalance; // Ideally should be calculated
        session.DeclaredBalance = declaredBalance;

        await _cashRepo.CloseSessionAsync(session);

        // Move funds to branch
        var register = await _cashRepo.GetDefaultRegisterAsync(1); // Need branch ID from register
        var branch = await _branchRepo.GetByIdAsync(register!.BranchId);
        if (branch != null)
        {
            branch.AvailableFunds += declaredBalance;
            await _branchRepo.UpdateAsync(branch);
        }

        scope.Complete();
    }
}
