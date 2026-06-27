using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class CashRegisterRepository : ICashRegisterRepository
{
    private readonly DbConnectionFactory _db;

    public CashRegisterRepository(DbConnectionFactory db) => _db = db;

    public async Task<CashRegisterSession?> GetActiveSessionAsync(int userId)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CashRegisterSession>(
            "SELECT * FROM CashRegisterSessions WHERE UserId = @UserId AND Status = 'OPEN' ORDER BY CreatedAt DESC LIMIT 1",
            new { UserId = userId }
        );
    }

    public async Task<int> OpenSessionAsync(CashRegisterSession session)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            INSERT INTO CashRegisterSessions (CashRegisterId, UserId, OpeningTime, OpeningBalance, Status, CreatedAt, IsActive)
            VALUES (@CashRegisterId, @UserId, CURRENT_TIMESTAMP, @OpeningBalance, 'OPEN', CURRENT_TIMESTAMP, TRUE)
            RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, session);
    }

    public async Task CloseSessionAsync(CashRegisterSession session)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            UPDATE CashRegisterSessions 
            SET ClosingTime = CURRENT_TIMESTAMP, 
                ClosingBalance = @ClosingBalance, 
                DeclaredBalance = @DeclaredBalance, 
                Status = 'CLOSED', 
                UpdatedAt = CURRENT_TIMESTAMP 
            WHERE Id = @Id;";
        await connection.ExecuteAsync(sql, session);
    }

    public async Task<CashRegister?> GetDefaultRegisterAsync(int branchId)
    {
        using var connection = _db.CreateConnection();
        // Return the first available cash register for the branch. Create one if none exist.
        var reg = await connection.QueryFirstOrDefaultAsync<CashRegister>("SELECT * FROM CashRegisters WHERE BranchId = @BranchId", new { BranchId = branchId });
        if (reg == null)
        {
            var sql = @"INSERT INTO CashRegisters (Name, BranchId, Description, CreatedAt, IsActive) 
                        VALUES ('Caja Principal', @BranchId, 'Caja por defecto', CURRENT_TIMESTAMP, TRUE) 
                        RETURNING Id;";
            var id = await connection.ExecuteScalarAsync<int>(sql, new { BranchId = branchId });
            return await connection.QueryFirstOrDefaultAsync<CashRegister>("SELECT * FROM CashRegisters WHERE Id = @Id", new { Id = id });
        }
        return reg;
    }
}
