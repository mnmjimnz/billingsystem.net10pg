using BillingSystem.Application.DTOs;
using BillingSystem.Application.Interfaces;
using BillingSystem.Domain.Interfaces;

namespace BillingSystem.Application.Services;

public class KardexService : IKardexService
{
    private readonly IKardexRepository _kardexRepo;

    public KardexService(IKardexRepository kardexRepo)
    {
        _kardexRepo = kardexRepo;
    }

    public async Task<IEnumerable<KardexDto>> GetAllMovementsAsync(int? productId)
    {
        var data = await _kardexRepo.GetAllMovementsAsync(productId);
        // Map dynamic to DTO
        return data.Select(d => new KardexDto
        {
            Id = d.id,
            ProductId = d.productid,
            ProductName = d.productname,
            Barcode = d.barcode,
            MovementType = d.movementtype,
            ReferenceType = d.referencetype,
            ReferenceId = d.referenceid,
            Quantity = d.quantity,
            PreviousStock = d.previousstock,
            NewStock = d.newstock,
            Description = d.description,
            CreatedAt = d.createdat
        });
    }
}
