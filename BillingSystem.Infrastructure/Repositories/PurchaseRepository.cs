using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly DbConnectionFactory _db;
    public PurchaseRepository(DbConnectionFactory db) => _db = db;

    public async Task<int> CreatePurchaseWithDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetail> details)
    {
        using var connection = _db.CreateConnection();
        var purchaseSql = @"
            INSERT INTO Purchases (InvoiceNumber, SupplierId, UserId, Total, PaymentType, AmountPaid, Status, CreatedAt, IsActive)
            VALUES (@InvoiceNumber, @SupplierId, @UserId, @Total, @PaymentType, @AmountPaid, @Status, CURRENT_TIMESTAMP, TRUE)
            RETURNING Id;";
        
        var purchaseId = await connection.ExecuteScalarAsync<int>(purchaseSql, purchase);

        foreach (var detail in details)
        {
            detail.PurchaseId = purchaseId;
            
            var detailSql = @"
                INSERT INTO PurchaseDetails (PurchaseId, ProductId, Quantity, UnitCost, Subtotal, CreatedAt, IsActive)
                VALUES (@PurchaseId, @ProductId, @Quantity, @UnitCost, @Subtotal, CURRENT_TIMESTAMP, TRUE);";
            await connection.ExecuteAsync(detailSql, detail);
        }

        return purchaseId;
    }

    public Task<Purchase?> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<Purchase>> GetAllAsync() => throw new NotImplementedException();
    public Task<int> AddAsync(Purchase entity) => throw new NotImplementedException();
    public Task<int> UpdateAsync(Purchase entity) => throw new NotImplementedException();
}
