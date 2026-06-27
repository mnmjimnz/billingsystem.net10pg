using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly DbConnectionFactory _db;

    public SettingsRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<CompanySetting> GetSettingsAsync()
    {
        using var connection = _db.CreateConnection();
        var settings = await connection.QueryFirstOrDefaultAsync<CompanySetting>("SELECT * FROM CompanySettings LIMIT 1");
        if (settings == null)
        {
            settings = new CompanySetting { CompanyName = "Nexus POS", TaxPercentage = 13 };
        }
        return settings;
    }

    public async Task UpdateSettingsAsync(CompanySetting settings)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            UPDATE CompanySettings 
            SET CompanyName = @CompanyName,
                Address = @Address,
                Phone = @Phone,
                Email = @Email,
                TaxPercentage = @TaxPercentage,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE Id = (SELECT Id FROM CompanySettings LIMIT 1);";
        await connection.ExecuteAsync(sql, settings);
    }
}
