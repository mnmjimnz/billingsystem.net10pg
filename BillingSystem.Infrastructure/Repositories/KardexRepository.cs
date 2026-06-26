using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class KardexRepository : IKardexRepository
{
    private readonly DbConnectionFactory _db;
    public KardexRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<dynamic>> GetAllMovementsAsync(int? productId)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT 
                im.Id,
                im.ProductId,
                p.Name as ProductName,
                p.Barcode,
                im.MovementType,
                im.ReferenceType,
                im.ReferenceId,
                im.Quantity,
                im.PreviousStock,
                im.NewStock,
                im.Description,
                im.CreatedAt
            FROM InventoryMovements im
            JOIN Products p ON im.ProductId = p.Id
            WHERE (@ProductId IS NULL OR im.ProductId = @ProductId)
            ORDER BY im.CreatedAt DESC";

        return await connection.QueryAsync(sql, new { ProductId = productId });
    }

    public async Task AddMovementAsync(InventoryMovement movement)
    {
        using var connection = _db.CreateConnection();
        var movementSql = @"
            INSERT INTO InventoryMovements (ProductId, MovementType, ReferenceType, ReferenceId, Quantity, PreviousStock, NewStock, Description, CreatedAt, IsActive)
            VALUES (@ProductId, @MovementType, @ReferenceType, @ReferenceId, @Quantity, @PreviousStock, @NewStock, @Description, CURRENT_TIMESTAMP, TRUE);";
        await connection.ExecuteAsync(movementSql, movement);
    }
}
