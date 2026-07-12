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
    private readonly ICashRegisterRepository _cashRepo;
    private readonly ISaleRepository _saleRepo;

    public BranchMovementService(
        IBranchMovementRepository movementRepository,
        IBranchRepository branchRepository,
        IAccountingService accountingService,
        ICashRegisterRepository cashRepo,
        ISaleRepository saleRepo)
    {
        _movementRepository = movementRepository;
        _branchRepository = branchRepository;
        _accountingService = accountingService;
        _cashRepo = cashRepo;
        _saleRepo = saleRepo;
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

            if (movement.CashRegisterId.HasValue)
            {
                var session = await _cashRepo.GetActiveSessionByRegisterAsync(movement.CashRegisterId.Value);
                if (session == null)
                    return Result<BranchMovement>.Failure("La caja seleccionada no tiene un turno abierto.");

                if (movement.Type == "OUT")
                {
                    var salesTotal = await _saleRepo.GetSessionSalesTotalAsync(session.UserId, session.OpeningTime);
                    var movsIn = await _movementRepository.GetSessionMovementsTotalAsync(movement.CashRegisterId.Value, session.OpeningTime, "IN");
                    var movsOut = await _movementRepository.GetSessionMovementsTotalAsync(movement.CashRegisterId.Value, session.OpeningTime, "OUT");
                    var currentCash = session.OpeningBalance + salesTotal + movsIn - movsOut;

                    if (currentCash < movement.Amount)
                        return Result<BranchMovement>.Failure($"Fondos insuficientes en la caja. Efectivo disponible: ${currentCash:F2}");
                }
                else if (movement.Type != "IN")
                {
                    return Result<BranchMovement>.Failure("Tipo de movimiento inválido (Debe ser IN o OUT).");
                }
            }
            else
            {
                if (movement.Type == "OUT")
                {
                    if (branch.AvailableFunds < movement.Amount)
                    {
                        return Result<BranchMovement>.Failure($"Fondos insuficientes en la bóveda/banco de la sucursal. Disponible: ${branch.AvailableFunds:F2}");
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
            }

            movement.Date = DateTime.UtcNow;
            var id = await _movementRepository.AddAsync(movement);
            movement.Id = id;

            scope.Complete();

            // Record accounting outside the transaction scope to avoid nested transaction issues
            // with AccountingRepository's internal BeginTransaction.
            if (movement.AccountId.HasValue)
            {
                await _accountingService.RecordBranchMovementAsync(movement);
            }

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
