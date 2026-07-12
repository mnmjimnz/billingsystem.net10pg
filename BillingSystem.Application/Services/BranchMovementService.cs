using System.Transactions;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Domain.Models;

namespace BillingSystem.Application.Services;

public class BranchMovementService : IBranchMovementService
{
    private readonly IBranchMovementRepository _movementRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IAccountingService _accountingService;

    public BranchMovementService(
        IBranchMovementRepository movementRepository,
        IBranchRepository branchRepository,
        IAccountingService accountingService)
    {
        _movementRepository = movementRepository;
        _branchRepository = branchRepository;
        _accountingService = accountingService;
    }

    public async Task<Result<BranchMovement>> RegisterMovementAsync(BranchMovement movement)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(movement.BranchId);
            if (branch == null)
            {
                return Result<BranchMovement>.Failure("Sucursal no encontrada.");
            }

            if (branch.Status != "OPEN")
            {
                return Result<BranchMovement>.Failure("La sucursal está cerrada. No se pueden realizar movimientos.");
            }

            if (movement.Amount <= 0)
            {
                return Result<BranchMovement>.Failure("El monto debe ser mayor a cero.");
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (movement.Type == "OUT")
            {
                if (branch.AvailableFunds < movement.Amount)
                {
                    return Result<BranchMovement>.Failure($"Fondos insuficientes en la sucursal. Disponible: ${branch.AvailableFunds:F2}");
                }
                branch.AvailableFunds -= movement.Amount;
            }
            else if (movement.Type == "IN")
            {
                branch.AvailableFunds += movement.Amount;
            }
            else
            {
                return Result<BranchMovement>.Failure("Tipo de movimiento inválido (Debe ser IN o OUT).");
            }

            branch.UpdatedAt = DateTime.UtcNow;
            await _branchRepository.UpdateAsync(branch);

            movement.Date = DateTime.UtcNow;
            var id = await _movementRepository.AddAsync(movement);
            movement.Id = id;

            if (movement.AccountId.HasValue)
            {
                await _accountingService.RecordBranchMovementAsync(movement);
            }

            scope.Complete();
            return Result<BranchMovement>.Success(movement, "Movimiento registrado correctamente.");
        }
        catch (Exception ex)
        {
            return Result<BranchMovement>.Failure($"Error al registrar el movimiento: {ex.Message}");
        }
    }

    public async Task<IEnumerable<BranchMovement>> GetMovementsByBranchIdAsync(int branchId)
    {
        return await _movementRepository.GetByBranchIdAsync(branchId);
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedMovementsByBranchIdAsync(int branchId, int page, int pageSize)
    {
        return await _movementRepository.GetPagedByBranchIdAsync(branchId, page, pageSize);
    }
}
